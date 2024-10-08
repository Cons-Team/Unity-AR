using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class PathUpdater : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown targetDropdown; // ��ǥ ���� ���� ��Ӵٿ�
    [SerializeField] private TMP_Dropdown pathOptionDropdown; // ��� �ɼ� ��Ӵٿ�
    [SerializeField] private List<GameObject> navTargetObjects; // �׺���̼� ��ǥ ������Ʈ ���
    [SerializeField] private GameObject elevatorEntry1F; // 1�� ���������� �Ա�
    [SerializeField] private GameObject elevatorExit2F; // 2�� ���������� �ⱸ
    [SerializeField] private GameObject elevatorEntry2F; // 2�� ���������� �Ա�
    [SerializeField] private GameObject elevatorExit1F; // 1�� ���������� �ⱸ
    [SerializeField] private LineRenderer line; // ��θ� ǥ���� LineRenderer
    public GameObject indicator; // �÷��̾� ������Ʈ
    [SerializeField] private TMP_Text arrivalTimeText; // ���� ���� �ð��� ǥ���� �ؽ�Ʈ
    [SerializeField] private float movementSpeed = 0.3f; // �̵� �ӵ�

    private ElevatorPathHandler elevatorPathHandler;
    float maxDistance = 6.0f;

    private void Start()
    {
        // LineRenderer�� �Ҵ���� ���� ��� ������Ʈ���� ��������
        if (!line) line = GetComponent<LineRenderer>();

        // ���������� ��� ó���� �ʱ�ȭ
        elevatorPathHandler = GetComponent<ElevatorPathHandler>();

        // ��Ӵٿ� �ʱ�ȭ
        InitializeDropdowns();

        // �ʱ� ��� ���
        if (navTargetObjects.Count > 0)
        {
            GetLocation(); // Android���� ��ǥ ���� �޼ҵ�
            UpdatePath(navTargetObjects[0].transform.position);
            UpdateArrivalTime(navTargetObjects[0].transform.position);
        }
    }

    // ��Ӵٿ� �ʱ�ȭ
    private void InitializeDropdowns()
    {
        // ��ǥ ���� ��Ӵٿ� �ʱ�ȭ
        targetDropdown.ClearOptions();
        List<string> targetNames = navTargetObjects.ConvertAll(target => target.name);
        targetDropdown.AddOptions(targetNames);
        targetDropdown.onValueChanged.AddListener(OnTargetDropdownValueChanged);

        // ��� �ɼ� ��Ӵٿ� �ʱ�ȭ
        pathOptionDropdown.ClearOptions();
        pathOptionDropdown.AddOptions(new List<string> { "���", "����������" });
        pathOptionDropdown.onValueChanged.AddListener(OnPathOptionDropdownValueChanged);
    }

    // ��ǥ ���� ��Ӵٿ� �� ���� �� ��� ������Ʈ
    private void OnTargetDropdownValueChanged(int index)
    {
        Vector3 targetPosition = navTargetObjects[index].transform.position;
        UpdatePath(targetPosition);
        UpdateArrivalTime(targetPosition); // ���� �ð� ������Ʈ ȣ��
    }

    // ��� �ɼ� ��Ӵٿ� �� ���� �� ��� ������Ʈ
    private void OnPathOptionDropdownValueChanged(int index)
    {
        if (navTargetObjects.Count > 0)
        {
            Vector3 targetPosition = navTargetObjects[targetDropdown.value].transform.position;
            UpdatePath(targetPosition);
            UpdateArrivalTime(targetPosition); // ���� �ð� ������Ʈ ȣ��
        }
    }

    // ��θ� ������Ʈ�ϰ� ��� �ɼǿ� ���� ��� �Ǵ� ���������� ��θ� ����
    private void UpdatePath(Vector3 targetPosition)
    {
        if (pathOptionDropdown.value == 1) // ���������͸� ����ϴ� ���
        {
            Vector3 elevatorEntry = IsOnFirstFloor(transform.position) ? elevatorEntry1F.transform.position : elevatorEntry2F.transform.position;
            Vector3 elevatorExit = IsOnFirstFloor(transform.position) ? elevatorExit2F.transform.position : elevatorExit1F.transform.position;

            // ���������͸� ����Ͽ� ��� ���
            elevatorPathHandler.CalculatePathUsingElevator(transform.position, targetPosition, elevatorEntry, elevatorExit, completePath =>
            {
                // ���������� ��� �ð� �����Ͽ� ���� ���� �ð� ���
                elevatorPathHandler.GetElevatorTravelTime(out float elevatorTravelTime);
                UpdateArrivalTimeWithElevator(elevatorTravelTime, completePath);

                // ��θ� LineRenderer�� ������Ʈ
                if (line != null)
                {
                    line.positionCount = completePath.Count;
                    line.SetPositions(completePath.ToArray());
                    line.enabled = true;
                }
            });
        }
        else
        {
            // ����� ����ϴ� �⺻ ��� ���
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path);

            if (path.corners.Length > 0)
            {
                List<Vector3> validCorners = new List<Vector3>();

                // ��� ������Ʈ �� ��ǥ ������ �Ÿ� Ȯ��
                for (int i = 0; i < path.corners.Length - 1; i++)
                {
                    if (Vector3.Distance(path.corners[i], path.corners[i + 1]) > maxDistance) // maxDistance������ ������ ���� ����
                    {
                        Debug.LogWarning("��ΰ� �ʹ� ��ϴ�. �߸��� ��� ����");
                        continue;
                    }
                    // ���� ��θ� ����Ʈ�� �߰�
                    validCorners.Add(path.corners[i]);
                }

                // ������ �� �߰�
                validCorners.Add(path.corners[path.corners.Length - 1]);

                // ��ȿ�� ��θ� LineRenderer�� ����
                line.positionCount = validCorners.Count;
                line.SetPositions(validCorners.ToArray());
                line.enabled = true;
            }

            // ����� ����� ���� ���� ���� �ð� ������Ʈ
            UpdateArrivalTime(targetPosition);
        }
    }

    // ���� ��ġ�� 1���� �ִ��� Ȯ��
    private bool IsOnFirstFloor(Vector3 position)
    {
        return position.y < -2.29f; // Y ��ǥ �������� 3�� �Ǻ�
    }

    // ���� ��ġ�� 2���� �ִ��� Ȯ��
    private bool IsOnSecondFloor(Vector3 position)
    {
        return position.y >= 1.2f; // Y ��ǥ �������� 4�� �Ǻ�
    }

    // ��ǥ ������ ������Ʈ�� �����ϰ� ��� ������Ʈ
    public void SetTargetByObject(GameObject target)
    {
        Debug.Log($"��ǥ�� �����Ǿ����ϴ�: {target.name}");

        // ��� ��ǥ ������Ʈ �� ���õ� ��ǥ�� Ȱ��ȭ
        for (int i = 0; i < navTargetObjects.Count; i++)
        {
            if (navTargetObjects[i] == target)
            {
                navTargetObjects[i].SetActive(true);  // ���õ� ��ǥ Ȱ��ȭ
            }
            else
            {
                navTargetObjects[i].SetActive(false); // ������ ��ǥ ��Ȱ��ȭ
            }
        }

        // ��Ӵٿ�� �ش� ��ǥ ���� ����
        int index = navTargetObjects.IndexOf(target);
        if (index >= 0)
        {
            targetDropdown.value = index;
        }

        // ��� �� ���� �ð� ������Ʈ
        UpdatePath(target.transform.position);
        UpdateArrivalTime(target.transform.position);
    }

    private void UpdateArrivalTime(Vector3 targetPosition)
    {
        float totalDistance = Vector3.Distance(transform.position, targetPosition);
        float travelTime = totalDistance / movementSpeed;

        
        if (pathOptionDropdown.value == 0)
        {
            // ����� ����� ��
            float estimatedArrivalTime = Time.time + travelTime;
            System.DateTime arrivalTime = System.DateTime.Now.AddSeconds(travelTime);
            int minutes = Mathf.FloorToInt(travelTime / 60);
            int seconds = Mathf.FloorToInt(travelTime % 60);

            if (arrivalTimeText != null)
            {
                arrivalTimeText.text = $"���� ���� �ð�: {minutes}�� {seconds}�� ��";
            }

            // ����� �α� �߰�
            Debug.Log($"���� ���� �ð� ������Ʈ: ��ǥ ����: {targetPosition}, �� �Ÿ�: {totalDistance}, �̵� �ð�: {travelTime}, ���� ���� �ð�: {minutes}�� {seconds}�� ��");
        }
    }

    public void UpdateArrivalTimeWithElevator(float elevatorWaitTime, List<Vector3> completePath)
    {
        if (completePath == null || completePath.Count == 0)
        {
            Debug.LogWarning("��ΰ� ����ְų� null�Դϴ�.");
            return;
        }

        // ��ü �Ÿ� ���
        float totalDistance = 0f;
        for (int i = 0; i < completePath.Count - 1; i++)
        {
            totalDistance += Vector3.Distance(completePath[i], completePath[i + 1]);
        }

        // �̵� �ð� ���
        float travelTime = totalDistance / movementSpeed;
        float totalTravelTime = travelTime + elevatorWaitTime;

        // ���� �ð��� �̵� �ð��� ��� �ð��� ���Ͽ� ���� ���� �ð� ���
        int minutes = Mathf.FloorToInt(totalTravelTime / 60);
        int seconds = Mathf.FloorToInt(totalTravelTime % 60);

        if (arrivalTimeText != null)
        {
            arrivalTimeText.text = $"���� ���� �ð�: {minutes}�� {seconds}�� ��";
        }

        // ����� �α� �߰�
        string finalDestination = completePath.Count > 0 ? $"({completePath[completePath.Count - 1].x}, {completePath[completePath.Count - 1].y}, {completePath[completePath.Count - 1].z})" : "����";
        Debug.Log($"���� ���� �ð� ������Ʈ: ��ǥ ����: {finalDestination}, �� �Ÿ�: {totalDistance}, �̵� �ð�: {travelTime}��, ���������� ��� �ð�: {elevatorWaitTime}��, ���� ���� �ð�: {minutes}�� {seconds}�� ��");
    }

    // Android���� ��ǥ ����
    public void GetLocation()
    {
        // Android�� GetLocation �޼ҵ�κ��� ��ǥ ����
        AndroidJavaClass ajc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject ajo = ajc.GetStatic<AndroidJavaObject>("currentActivity");
        string location = ajo.Call<string>("GetLocation");

        string[] locationVector3 = location.Split(',');

        // Indicator(�÷��̾�) ��ġ �Ҵ�
        indicator.transform.position = new Vector3(float.Parse(locationVector3[0]), float.Parse(locationVector3[1]), float.Parse(locationVector3[2]));

        // Android���� Log ��� (��ǥ ���� Ȯ�ο�)
        ajo.Call("SendToastFromUnity", location);

        // Android���� Log ��� (Indicator�� ��ǥ�� ����ƴ��� Ȯ�ο�)
        string playerLocation = indicator.transform.position.ToString();
        ajo.Call("SendToastFromUnity", playerLocation);
    }
}