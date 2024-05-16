using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NaviTest : MonoBehaviour
{
    [SerializeField]
    private Camera topDownCamera;
    [SerializeField]
    private GameObject navTargetObject;
    [SerializeField]
    private float playerWalkSpeed = 3.0f; // 플레이어 걷는 속도

    private NavMeshPath path;
    private LineRenderer line;
    private NavMeshAgent agent; // NavMeshAgent 추가

    private bool lineToggle = false;

    // 라인 업데이트 간격
    private float updateInterval = 0.1f; // 0.1초마다 업데이트

    private float timeSinceLastUpdate = 0f;

    private void Start()
    {
        path = new NavMeshPath();
        line = transform.GetComponent<LineRenderer>();
        agent = GetComponent<NavMeshAgent>(); // NavMeshAgent 컴포넌트 가져오기
        agent.speed = playerWalkSpeed; // 플레이어의 걷는 속도 설정
    }

    private void Update()
    {
        // 라인 업데이트 간격 체크
        timeSinceLastUpdate += Time.deltaTime;
        if (timeSinceLastUpdate >= updateInterval)
        {
            // 일정 시간이 지난 후에만 라인 업데이트
            timeSinceLastUpdate = 0f;

            if (lineToggle)
            {
                NavMesh.CalculatePath(transform.position, navTargetObject.transform.position, NavMesh.AllAreas, path);
                line.positionCount = path.corners.Length;
                line.SetPositions(path.corners);
                line.enabled = true;
            }
        }

        // 터치 입력을 통한 라인 토글
        if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
        {
            lineToggle = !lineToggle;
        }
    }
}