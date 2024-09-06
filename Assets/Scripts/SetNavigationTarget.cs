using UnityEngine;
using UnityEngine.AI;

public class SetNavigationTarget : MonoBehaviour
{
    [SerializeField] private SearchWindowManager searchWindowManager; // 검색 창 관리 스크립트 참조
    [SerializeField] private LineRenderer lineRenderer; // 경로를 표시할 LineRenderer

    private NavMeshPath navMeshPath;

    private void Start()
    {
        navMeshPath = new NavMeshPath(); // NavMeshPath 초기화
    }

    // 검색 창을 여는 버튼 클릭 시 호출
    public void OnOpenSearchWindowButtonClicked()
    {
        if (searchWindowManager)
            searchWindowManager.OpenSearchWindow();
        else
            Debug.LogError("SetNavigationTarget에 SearchWindowManager가 할당되지 않았습니다.");
    }

    // 목표 지점을 설정하고 경로를 업데이트
    public void SetTargetByObject(GameObject target)
    {
        if (target)
        {
            Vector3 targetPosition = target.transform.position;
            UpdatePath(targetPosition);
            Debug.Log($"네비게이션 목표가 설정되었습니다: {target.name}");
        }
        else
            Debug.LogWarning("목표가 없습니다.");
    }

    // 경로를 업데이트하고 LineRenderer를 설정
    private void UpdatePath(Vector3 targetPosition)
    {
        // 현재 위치에서 목표 지점까지의 경로 계산
        NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, navMeshPath);

        if (navMeshPath.corners.Length > 0)
        {
            lineRenderer.positionCount = navMeshPath.corners.Length;
            lineRenderer.SetPositions(navMeshPath.corners);
            lineRenderer.enabled = true;
            Debug.Log("경로가 업데이트되고 LineRenderer가 설정되었습니다.");
        }
        else
        {
            Debug.LogWarning("경로 계산 실패 또는 경로에 코너가 없습니다.");
        }
    }
}