using UnityEngine;
using UnityEngine.AI;

public class SetNavigationTarget : MonoBehaviour
{
    [SerializeField]
    private SearchWindowManager searchWindowManager;  // 검색 창 관리 스크립트 참조

    [SerializeField]
    private LineRenderer lineRenderer;  // 경로를 표시할 LineRenderer

    private NavMeshPath navMeshPath;

    private void Start()
    {
        // NavMeshPath 초기화
        navMeshPath = new NavMeshPath();
    }

    // 검색 창을 여는 버튼 이벤트 핸들러
    public void OnOpenSearchWindowButtonClicked()
    {
        if (searchWindowManager != null)
        {
            searchWindowManager.OpenSearchWindow();
        }
        else
        {
            Debug.LogError("SearchWindowManager is not assigned in SetNavigationTarget.");
        }
    }

    // 목표 지점을 설정하고 경로를 업데이트하는 메서드
    public void SetTargetByObject(GameObject target)
    {
        if (target != null)
        {
            Vector3 targetPosition = target.transform.position;
            UpdatePath(targetPosition);
            Debug.Log($"Navigation target set to: {target.name}");
        }
        else
        {
            Debug.LogWarning("Target is null.");
        }
    }

    // 경로를 업데이트하는 메서드
    private void UpdatePath(Vector3 targetPosition)
    {
        // 현재 오브젝트의 위치에서 목표 지점까지의 경로 계산
        NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, navMeshPath);

        if (navMeshPath.corners.Length > 0)
        {
            // LineRenderer를 사용하여 경로를 표시
            lineRenderer.positionCount = navMeshPath.corners.Length;
            lineRenderer.SetPositions(navMeshPath.corners);
            lineRenderer.enabled = true;

            Debug.Log("Path updated and LineRenderer set.");
        }
        else
        {
            Debug.LogWarning("Failed to calculate path or path has no corners.");
        }
    }
}