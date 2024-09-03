using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class PathUpdater : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown targetDropdown;
    [SerializeField]
    private TMP_Dropdown pathOptionDropdown;
    [SerializeField]
    private List<GameObject> navTargetObjects;
    [SerializeField]
    private GameObject elevatorEntry1F;
    [SerializeField]
    private GameObject elevatorExit2F;
    [SerializeField]
    private GameObject elevatorEntry2F;
    [SerializeField]
    private GameObject elevatorExit1F;
    [SerializeField]
    private LineRenderer line;
    public GameObject indicator; // �÷��̾� ������Ʈ�� �����ϴ� ����

    private ElevatorPathHandler elevatorPathHandler;

    private void Start()
    {
        if (line == null)
        {
            line = GetComponent<LineRenderer>();
        }

        elevatorPathHandler = GetComponent<ElevatorPathHandler>();

        // ��Ӵٿ� �ʱ�ȭ
        InitializeDropdowns();

        // �ʱ� ��� ���
        if (navTargetObjects.Count > 0)
        {
            /** Android���� ��ǥ ���� �޼ҵ�*/
            GetLocation();

            UpdatePath(navTargetObjects[0].transform.position);
        }
    }

    private void InitializeDropdowns()
    {
        // Target Dropdown �ʱ�ȭ
        targetDropdown.ClearOptions();
        List<string> targetNames = new List<string>();
        foreach (GameObject target in navTargetObjects)
        {
            targetNames.Add(target.name);
        }
        targetDropdown.AddOptions(targetNames);
        targetDropdown.onValueChanged.AddListener(OnTargetDropdownValueChanged);

        // Path Option Dropdown �ʱ�ȭ
        pathOptionDropdown.ClearOptions();
        List<string> pathOptions = new List<string> { "���", "����������" };
        pathOptionDropdown.AddOptions(pathOptions);
        pathOptionDropdown.onValueChanged.AddListener(OnPathOptionDropdownValueChanged);
    }

    private void OnTargetDropdownValueChanged(int index)
    {
        UpdatePath(navTargetObjects[index].transform.position);
    }

    private void OnPathOptionDropdownValueChanged(int index)
    {
        if (navTargetObjects.Count > 0)
        {
            UpdatePath(navTargetObjects[targetDropdown.value].transform.position);
        }
    }

    private void UpdatePath(Vector3 targetPosition)
    {
        if (pathOptionDropdown.value == 1) // ���������� �ɼ��� ���õ� ���
        {
            Vector3 elevatorEntry;
            Vector3 elevatorExit;

            if (IsOnFirstFloor(transform.position))
            {
                elevatorEntry = elevatorEntry1F.transform.position;
                elevatorExit = elevatorExit2F.transform.position;
            }
            else
            {
                elevatorEntry = elevatorEntry2F.transform.position;
                elevatorExit = elevatorExit1F.transform.position;
            }

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

    private bool IsOnFirstFloor(Vector3 position)
    {
        return position.y < -2.29f; // ����: Y ��ǥ�� -2.29 �̻��� ��� 3������ ����
    }

    private bool IsOnSecondFloor(Vector3 position)
    {
        return position.y >= 1.2f; // ����: Y ��ǥ�� 1.2 �̻��� ��� 4������ ����
    }

    public void SetTargetByObject(GameObject target)
    {
        // ��ǥ ������ �����ϴ� ����
        Debug.Log($"Target set to: {target.name}");

        // Ÿ�� ��Ӵٿ��� �ε����� ã�� ������Ʈ
        int index = navTargetObjects.IndexOf(target);
        if (index >= 0)
        {
            targetDropdown.value = index;
        }

        UpdatePath(target.transform.position);
    }

    /** Android���� ��ǥ ����*/
    public void GetLocation()
    {
        /** Android�� GetLocation �޼ҵ�κ��� ��ǥ ���� */
        AndroidJavaClass ajc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject ajo = ajc.GetStatic<AndroidJavaObject>("currentActivity");
        string location = ajo.Call<string>("GetLocation");

        string[] locationVector3 = location.Split(',');

        /** Indicator(�÷��̾�) ��ġ �Ҵ� */
        indicator.transform.position = new Vector3(float.Parse(locationVector3[0]), float.Parse(locationVector3[1]), float.Parse(locationVector3[2]));

        /** Android���� Log���(��ǥ ���� Ȯ�ο�) */
        ajo.Call("SendToastFromUnity", location);

        /** Android���� Log���(Indicator�� ��ǥ�� ����ƴ��� Ȯ�ο�) */
        string playerLocation = "";
        playerLocation = indicator.transform.position + "";
        ajo.Call("SendToastFromUnity", playerLocation);
    }
}