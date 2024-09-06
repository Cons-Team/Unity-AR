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

    private ElevatorPathHandler elevatorPathHandler;

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
        UpdatePath(navTargetObjects[index].transform.position);
    }

    // ��� �ɼ� ��Ӵٿ� �� ���� �� ��� ������Ʈ
    private void OnPathOptionDropdownValueChanged(int index)
    {
        if (navTargetObjects.Count > 0)
        {
            UpdatePath(navTargetObjects[targetDropdown.value].transform.position);
        }
    }

    // ��θ� ������Ʈ�ϰ� ��� �ɼǿ� ���� ��� �Ǵ� ���������� ��θ� ����
    private void UpdatePath(Vector3 targetPosition)
    {
        if (pathOptionDropdown.value == 1) // ���������� �ɼ��� ���õ� ���
        {
            Vector3 elevatorEntry = IsOnFirstFloor(transform.position) ? elevatorEntry1F.transform.position : elevatorEntry2F.transform.position;
            Vector3 elevatorExit = IsOnFirstFloor(transform.position) ? elevatorExit2F.transform.position : elevatorExit1F.transform.position;

            // ���������͸� ����Ͽ� ��� ���
            elevatorPathHandler.CalculatePathUsingElevator(transform.position, targetPosition, elevatorEntry, elevatorExit);
        }
        else
        {
            // ����� ����ϴ� �⺻ ��� ���
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path);

            if (path.corners.Length > 0)
            {
                line.positionCount = path.corners.Length;
                line.SetPositions(path.corners);
            }
            line.enabled = true;
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

        int index = navTargetObjects.IndexOf(target);
        if (index >= 0)
        {
            targetDropdown.value = index;
        }

        UpdatePath(target.transform.position);
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