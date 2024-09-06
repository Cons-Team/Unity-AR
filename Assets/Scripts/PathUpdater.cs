using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class PathUpdater : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown targetDropdown; // 목표 지점 선택 드롭다운
    [SerializeField] private TMP_Dropdown pathOptionDropdown; // 경로 옵션 드롭다운
    [SerializeField] private List<GameObject> navTargetObjects; // 네비게이션 목표 오브젝트 목록
    [SerializeField] private GameObject elevatorEntry1F; // 1층 엘리베이터 입구
    [SerializeField] private GameObject elevatorExit2F; // 2층 엘리베이터 출구
    [SerializeField] private GameObject elevatorEntry2F; // 2층 엘리베이터 입구
    [SerializeField] private GameObject elevatorExit1F; // 1층 엘리베이터 출구
    [SerializeField] private LineRenderer line; // 경로를 표시할 LineRenderer
    public GameObject indicator; // 플레이어 오브젝트

    private ElevatorPathHandler elevatorPathHandler;

    private void Start()
    {
        // LineRenderer가 할당되지 않은 경우 컴포넌트에서 가져오기
        if (!line) line = GetComponent<LineRenderer>();

        // 엘리베이터 경로 처리기 초기화
        elevatorPathHandler = GetComponent<ElevatorPathHandler>();

        // 드롭다운 초기화
        InitializeDropdowns();

        // 초기 경로 계산
        if (navTargetObjects.Count > 0)
        {
            GetLocation(); // Android에서 좌표 수신 메소드
            UpdatePath(navTargetObjects[0].transform.position);
        }
    }

    // 드롭다운 초기화
    private void InitializeDropdowns()
    {
        // 목표 지점 드롭다운 초기화
        targetDropdown.ClearOptions();
        List<string> targetNames = navTargetObjects.ConvertAll(target => target.name);
        targetDropdown.AddOptions(targetNames);
        targetDropdown.onValueChanged.AddListener(OnTargetDropdownValueChanged);

        // 경로 옵션 드롭다운 초기화
        pathOptionDropdown.ClearOptions();
        pathOptionDropdown.AddOptions(new List<string> { "계단", "엘리베이터" });
        pathOptionDropdown.onValueChanged.AddListener(OnPathOptionDropdownValueChanged);
    }

    // 목표 지점 드롭다운 값 변경 시 경로 업데이트
    private void OnTargetDropdownValueChanged(int index)
    {
        UpdatePath(navTargetObjects[index].transform.position);
    }

    // 경로 옵션 드롭다운 값 변경 시 경로 업데이트
    private void OnPathOptionDropdownValueChanged(int index)
    {
        if (navTargetObjects.Count > 0)
        {
            UpdatePath(navTargetObjects[targetDropdown.value].transform.position);
        }
    }

    // 경로를 업데이트하고 경로 옵션에 따라 계단 또는 엘리베이터 경로를 설정
    private void UpdatePath(Vector3 targetPosition)
    {
        if (pathOptionDropdown.value == 1) // 엘리베이터 옵션이 선택된 경우
        {
            Vector3 elevatorEntry = IsOnFirstFloor(transform.position) ? elevatorEntry1F.transform.position : elevatorEntry2F.transform.position;
            Vector3 elevatorExit = IsOnFirstFloor(transform.position) ? elevatorExit2F.transform.position : elevatorExit1F.transform.position;

            // 엘리베이터를 사용하여 경로 계산
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

    // 현재 위치가 1층에 있는지 확인
    private bool IsOnFirstFloor(Vector3 position)
    {
        return position.y < -2.29f; // Y 좌표 기준으로 3층 판별
    }

    // 현재 위치가 2층에 있는지 확인
    private bool IsOnSecondFloor(Vector3 position)
    {
        return position.y >= 1.2f; // Y 좌표 기준으로 4층 판별
    }

    // 목표 지점을 오브젝트로 설정하고 경로 업데이트
    public void SetTargetByObject(GameObject target)
    {
        Debug.Log($"목표가 설정되었습니다: {target.name}");

        int index = navTargetObjects.IndexOf(target);
        if (index >= 0)
        {
            targetDropdown.value = index;
        }

        UpdatePath(target.transform.position);
    }

    // Android에서 좌표 수신
    public void GetLocation()
    {
        // Android의 GetLocation 메소드로부터 좌표 수신
        AndroidJavaClass ajc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject ajo = ajc.GetStatic<AndroidJavaObject>("currentActivity");
        string location = ajo.Call<string>("GetLocation");

        string[] locationVector3 = location.Split(',');

        // Indicator(플레이어) 위치 할당
        indicator.transform.position = new Vector3(float.Parse(locationVector3[0]), float.Parse(locationVector3[1]), float.Parse(locationVector3[2]));

        // Android에서 Log 출력 (좌표 수신 확인용)
        ajo.Call("SendToastFromUnity", location);

        // Android에서 Log 출력 (Indicator에 좌표가 저장됐는지 확인용)
        string playerLocation = indicator.transform.position.ToString();
        ajo.Call("SendToastFromUnity", playerLocation);
    }
}