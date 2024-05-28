using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI; // UI를 사용하기 위해 추가
using TMPro; // 네임스페이스

public class NaviTest : MonoBehaviour
{
    [SerializeField]
    private Camera topDownCamera;
    [SerializeField]
    private List<GameObject> navTargetObjects; // 여러 네비게이션 목적지 리스트
    [SerializeField]
    private float playerWalkSpeed = 3.0f; // 플레이어 걷는 속도
    [SerializeField]
    private TMP_Dropdown destinationDropdown; // UI 추가 목적지 리스트

    private NavMeshPath path;
    private LineRenderer line;
    private NavMeshAgent agent; // NavMeshAgent 추가

    private bool lineToggle = false;

    // 라인 업데이트 간격
    private float updateInterval = 0.1f; // 0.1초마다 업데이트

    private float timeSinceLastUpdate = 0f;

    private int currentTargetIndex = -1; // 현재 네비게이션 목적지 인덱스, 초기값 -1 (선택 안된 상태)

    private void Start()
    {
        path = new NavMeshPath();
        line = transform.GetComponent<LineRenderer>();
        agent = GetComponent<NavMeshAgent>(); // NavMeshAgent 컴포넌트 가져오기
        agent.speed = playerWalkSpeed; // 플레이어의 걷는 속도 설정

        // Dropdown 초기화 및 이벤트 리스너 추가
        InitializeDropdown();
        destinationDropdown.onValueChanged.AddListener(OnDestinationSelected);
    }

    private void Update()
    {
        // 라인 업데이트 간격 체크
        timeSinceLastUpdate += Time.deltaTime;
        if (timeSinceLastUpdate >= updateInterval)
        {
            // 일정 시간이 지난 후에만 라인 업데이트
            timeSinceLastUpdate = 0f;

            if (lineToggle && currentTargetIndex >= 0 && currentTargetIndex < navTargetObjects.Count)
            {
                NavMesh.CalculatePath(transform.position, navTargetObjects[currentTargetIndex].transform.position, NavMesh.AllAreas, path);
                line.positionCount = path.corners.Length;
                line.SetPositions(path.corners);
                line.enabled = true;
            }
        }

        // 플레이어가 현재 목적지에 도달했는지 확인
        if (currentTargetIndex >= 0 && Vector3.Distance(transform.position, navTargetObjects[currentTargetIndex].transform.position) < 0.5f)
        {
            lineToggle = false;
            line.enabled = false;
            currentTargetIndex = -1; // 목적지 도달 후 초기화
        }
    }

    private void InitializeDropdown()
    {
        destinationDropdown.ClearOptions();
        List<string> options = new List<string>();
        foreach (GameObject target in navTargetObjects)
        {
            options.Add(target.name);
        }
        destinationDropdown.AddOptions(options);
    }

    public void OnDestinationSelected(int index)
    {
        currentTargetIndex = index;
        lineToggle = true;
    }

    // 비콘으로부터 받은 좌표에 따라 플레이어의 위치를 업데이트하는 메서드
    public void UpdatePlayerPosition(float[] coordinates)
    {
        if (coordinates.Length == 3)
        {
            Vector3 newPosition = new Vector3(coordinates[0], coordinates[2], coordinates[1]); // x, z, y 형식이라고 가정
            transform.position = newPosition;
            agent.Warp(newPosition); // NavMeshAgent를 새로운 위치로 이동
        }
    }
}