using UnityEngine;
using UnityEngine.AI;

public class SetNavigationTarget : MonoBehaviour
{
    [SerializeField]
    private SearchWindowManager searchWindowManager;  // �˻� â ���� ��ũ��Ʈ ����

    [SerializeField]
    private LineRenderer lineRenderer;  // ��θ� ǥ���� LineRenderer

    private NavMeshPath navMeshPath;

    private void Start()
    {
        // NavMeshPath �ʱ�ȭ
        navMeshPath = new NavMeshPath();
    }

    // �˻� â�� ���� ��ư �̺�Ʈ �ڵ鷯
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

    // ��ǥ ������ �����ϰ� ��θ� ������Ʈ�ϴ� �޼���
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

    // ��θ� ������Ʈ�ϴ� �޼���
    private void UpdatePath(Vector3 targetPosition)
    {
        // ���� ������Ʈ�� ��ġ���� ��ǥ ���������� ��� ���
        NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, navMeshPath);

        if (navMeshPath.corners.Length > 0)
        {
            // LineRenderer�� ����Ͽ� ��θ� ǥ��
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