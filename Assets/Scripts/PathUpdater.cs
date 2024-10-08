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
    [SerializeField] private TMP_Text arrivalTimeText; // 도착 예정 시간을 표시할 텍스트
    [SerializeField] private float movementSpeed = 0.3f; // 이동 속도

    private ElevatorPathHandler elevatorPathHandler;
    float maxDistance = 6.0f;

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
            UpdateArrivalTime(navTargetObjects[0].transform.position);
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
        Vector3 targetPosition = navTargetObjects[index].transform.position;
        UpdatePath(targetPosition);
        UpdateArrivalTime(targetPosition); // 도착 시간 업데이트 호출
    }

    // 경로 옵션 드롭다운 값 변경 시 경로 업데이트
    private void OnPathOptionDropdownValueChanged(int index)
    {
        if (navTargetObjects.Count > 0)
        {
            Vector3 targetPosition = navTargetObjects[targetDropdown.value].transform.position;
            UpdatePath(targetPosition);
            UpdateArrivalTime(targetPosition); // 도착 시간 업데이트 호출
        }
    }

    // 경로를 업데이트하고 경로 옵션에 따라 계단 또는 엘리베이터 경로를 설정
    private void UpdatePath(Vector3 targetPosition)
    {
        if (pathOptionDropdown.value == 1) // 엘리베이터를 사용하는 경우
        {
            Vector3 elevatorEntry = IsOnFirstFloor(transform.position) ? elevatorEntry1F.transform.position : elevatorEntry2F.transform.position;
            Vector3 elevatorExit = IsOnFirstFloor(transform.position) ? elevatorExit2F.transform.position : elevatorExit1F.transform.position;

            // 엘리베이터를 사용하여 경로 계산
            elevatorPathHandler.CalculatePathUsingElevator(transform.position, targetPosition, elevatorEntry, elevatorExit, completePath =>
            {
                // 엘리베이터 대기 시간 포함하여 도착 예정 시간 계산
                elevatorPathHandler.GetElevatorTravelTime(out float elevatorTravelTime);
                UpdateArrivalTimeWithElevator(elevatorTravelTime, completePath);

                // 경로를 LineRenderer로 업데이트
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
            // 계단을 사용하는 기본 경로 계산
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path);

            if (path.corners.Length > 0)
            {
                List<Vector3> validCorners = new List<Vector3>();

                // 경로 업데이트 시 좌표 사이의 거리 확인
                for (int i = 0; i < path.corners.Length - 1; i++)
                {
                    if (Vector3.Distance(path.corners[i], path.corners[i + 1]) > maxDistance) // maxDistance값으로 렌더러 길이 제한
                    {
                        Debug.LogWarning("경로가 너무 깁니다. 잘못된 경로 제거");
                        continue;
                    }
                    // 정상 경로만 리스트에 추가
                    validCorners.Add(path.corners[i]);
                }

                // 마지막 점 추가
                validCorners.Add(path.corners[path.corners.Length - 1]);

                // 유효한 경로를 LineRenderer에 적용
                line.positionCount = validCorners.Count;
                line.SetPositions(validCorners.ToArray());
                line.enabled = true;
            }

            // 계단을 사용할 때의 도착 예정 시간 업데이트
            UpdateArrivalTime(targetPosition);
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

        // 모든 목표 오브젝트 중 선택된 목표만 활성화
        for (int i = 0; i < navTargetObjects.Count; i++)
        {
            if (navTargetObjects[i] == target)
            {
                navTargetObjects[i].SetActive(true);  // 선택된 목표 활성화
            }
            else
            {
                navTargetObjects[i].SetActive(false); // 나머지 목표 비활성화
            }
        }

        // 드롭다운에서 해당 목표 지점 선택
        int index = navTargetObjects.IndexOf(target);
        if (index >= 0)
        {
            targetDropdown.value = index;
        }

        // 경로 및 도착 시간 업데이트
        UpdatePath(target.transform.position);
        UpdateArrivalTime(target.transform.position);
    }

    private void UpdateArrivalTime(Vector3 targetPosition)
    {
        float totalDistance = Vector3.Distance(transform.position, targetPosition);
        float travelTime = totalDistance / movementSpeed;

        
        if (pathOptionDropdown.value == 0)
        {
            // 계단을 사용할 때
            float estimatedArrivalTime = Time.time + travelTime;
            System.DateTime arrivalTime = System.DateTime.Now.AddSeconds(travelTime);
            int minutes = Mathf.FloorToInt(travelTime / 60);
            int seconds = Mathf.FloorToInt(travelTime % 60);

            if (arrivalTimeText != null)
            {
                arrivalTimeText.text = $"도착 예정 시간: {minutes}분 {seconds}초 후";
            }

            // 디버그 로그 추가
            Debug.Log($"도착 예정 시간 업데이트: 목표 지점: {targetPosition}, 총 거리: {totalDistance}, 이동 시간: {travelTime}, 도착 예정 시간: {minutes}분 {seconds}초 후");
        }
    }

    public void UpdateArrivalTimeWithElevator(float elevatorWaitTime, List<Vector3> completePath)
    {
        if (completePath == null || completePath.Count == 0)
        {
            Debug.LogWarning("경로가 비어있거나 null입니다.");
            return;
        }

        // 전체 거리 계산
        float totalDistance = 0f;
        for (int i = 0; i < completePath.Count - 1; i++)
        {
            totalDistance += Vector3.Distance(completePath[i], completePath[i + 1]);
        }

        // 이동 시간 계산
        float travelTime = totalDistance / movementSpeed;
        float totalTravelTime = travelTime + elevatorWaitTime;

        // 현재 시간에 이동 시간과 대기 시간을 더하여 도착 예정 시간 계산
        int minutes = Mathf.FloorToInt(totalTravelTime / 60);
        int seconds = Mathf.FloorToInt(totalTravelTime % 60);

        if (arrivalTimeText != null)
        {
            arrivalTimeText.text = $"도착 예정 시간: {minutes}분 {seconds}초 후";
        }

        // 디버그 로그 추가
        string finalDestination = completePath.Count > 0 ? $"({completePath[completePath.Count - 1].x}, {completePath[completePath.Count - 1].y}, {completePath[completePath.Count - 1].z})" : "없음";
        Debug.Log($"도착 예정 시간 업데이트: 목표 지점: {finalDestination}, 총 거리: {totalDistance}, 이동 시간: {travelTime}초, 엘리베이터 대기 시간: {elevatorWaitTime}초, 도착 예정 시간: {minutes}분 {seconds}초 후");
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