/*using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NaviTest : MonoBehaviour
{
    [SerializeField]
    private Camera topDownCamera;
    [SerializeField]
    private List<GameObject> navTargetObjects;
    [SerializeField]
    private float playerWalkSpeed = 3.0f;
    [SerializeField]
    private Dropdown destinationDropdown;
    [SerializeField]
    private Image mapImage; // 2D 지도를 나타내는 UI Image 추가

    private NavMeshPath path;
    private LineRenderer line;
    private NavMeshAgent agent;

    private bool lineToggle = false;
    private Vector3[] linePositions; // 네비게이션 경로의 선분 위치 배열

    private int currentTargetIndex = -1;

    private void Start()
    {
        path = new NavMeshPath();
        line = transform.GetComponent<LineRenderer>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = playerWalkSpeed;

        InitializeDropdown();
        destinationDropdown.onValueChanged.AddListener(OnDestinationSelected);
    }

    private void Update()
    {
        if (lineToggle && currentTargetIndex >= 0 && currentTargetIndex < navTargetObjects.Count)
        {
            NavMesh.CalculatePath(transform.position, navTargetObjects[currentTargetIndex].transform.position, NavMesh.AllAreas, path);
            linePositions = path.corners; // 경로의 모든 선분 위치 가져오기
            UpdateLineRenderer();
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

    // 버튼을 통해 2D 지도에서 특정 지점을 클릭했을 때 호출되는 메서드
    public void OnMapPointClicked(Vector3 mapPoint)
    {
        RaycastHit hit;
        Ray ray = topDownCamera.ScreenPointToRay(mapPoint);

        if (Physics.Raycast(ray, out hit))
        {
            NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 10f, NavMesh.AllAreas); // 클릭한 지점에 가까운 네비게이션 가능한 위치 가져오기
            SetDestination(navHit.position);
        }
    }

    // 클릭한 지점으로 플레이어 이동
    private void SetDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    // 라인 렌더러 업데이트
    private void UpdateLineRenderer()
    {
        line.positionCount = linePositions.Length;
        line.SetPositions(linePositions);

        // 2D 지도 상에 라인 그리기
        Vector2[] linePositions2D = new Vector2[linePositions.Length];
        for (int i = 0; i < linePositions.Length; i++)
        {
            linePositions2D[i] = topDownCamera.WorldToViewportPoint(linePositions[i]); // 세계 좌표를 뷰포트 좌표로 변환
        }
        mapImage.GetComponent<MapLineRenderer>().DrawLine(linePositions2D); // MapLineRenderer 스크립트의 DrawLine 메서드 호출
    }
}*/