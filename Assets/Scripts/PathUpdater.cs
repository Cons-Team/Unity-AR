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
    public GameObject indicator; // 플레이어 오브젝트를 지정하는 변수

    private ElevatorPathHandler elevatorPathHandler;

    private void Start()
    {
        if (line == null)
        {
            line = GetComponent<LineRenderer>();
        }

        elevatorPathHandler = GetComponent<ElevatorPathHandler>();

        // 드롭다운 초기화
        InitializeDropdowns();

        // 초기 경로 계산
        if (navTargetObjects.Count > 0)
        {
            /** Android에서 좌표 수신 메소드*/
            GetLocation();

            UpdatePath(navTargetObjects[0].transform.position);
        }
    }

    private void InitializeDropdowns()
    {
        // Target Dropdown 초기화
        targetDropdown.ClearOptions();
        List<string> targetNames = new List<string>();
        foreach (GameObject target in navTargetObjects)
        {
            targetNames.Add(target.name);
        }
        targetDropdown.AddOptions(targetNames);
        targetDropdown.onValueChanged.AddListener(OnTargetDropdownValueChanged);

        // Path Option Dropdown 초기화
        pathOptionDropdown.ClearOptions();
        List<string> pathOptions = new List<string> { "계단", "엘리베이터" };
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
        if (pathOptionDropdown.value == 1) // 엘리베이터 옵션이 선택된 경우
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
            // 계단을 사용하는 기본 경로 계산
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
        return position.y < -2.29f; // 예시: Y 좌표가 -2.29 이상인 경우 3층으로 간주
    }

    private bool IsOnSecondFloor(Vector3 position)
    {
        return position.y >= 1.2f; // 예시: Y 좌표가 1.2 이상인 경우 4층으로 간주
    }

    public void SetTargetByObject(GameObject target)
    {
        // 목표 지점을 설정하는 로직
        Debug.Log($"Target set to: {target.name}");

        // 타겟 드롭다운의 인덱스를 찾고 업데이트
        int index = navTargetObjects.IndexOf(target);
        if (index >= 0)
        {
            targetDropdown.value = index;
        }

        UpdatePath(target.transform.position);
    }

    /** Android에서 좌표 수신*/
    public void GetLocation()
    {
        /** Android의 GetLocation 메소드로부터 좌표 수신 */
        AndroidJavaClass ajc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject ajo = ajc.GetStatic<AndroidJavaObject>("currentActivity");
        string location = ajo.Call<string>("GetLocation");

        string[] locationVector3 = location.Split(',');

        /** Indicator(플레이어) 위치 할당 */
        indicator.transform.position = new Vector3(float.Parse(locationVector3[0]), float.Parse(locationVector3[1]), float.Parse(locationVector3[2]));

        /** Android에서 Log찍기(좌표 수신 확인용) */
        ajo.Call("SendToastFromUnity", location);

        /** Android에서 Log찍기(Indicator에 좌표가 저장됐는지 확인용) */
        string playerLocation = "";
        playerLocation = indicator.transform.position + "";
        ajo.Call("SendToastFromUnity", playerLocation);
    }
}