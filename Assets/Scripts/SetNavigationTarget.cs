using UnityEngine;
using UnityEngine.AI;

public class SetNavigationTarget : MonoBehaviour
{
    [SerializeField] private SearchWindowManager searchWindowManager; // �˻� â ���� ��ũ��Ʈ ����
    [SerializeField] private LineRenderer lineRenderer; // ��θ� ǥ���� LineRenderer

    private NavMeshPath navMeshPath;

    private void Start()
    {
        navMeshPath = new NavMeshPath(); // NavMeshPath �ʱ�ȭ
    }

    // �˻� â�� ���� ��ư Ŭ�� �� ȣ��
    public void OnOpenSearchWindowButtonClicked()
    {
        if (searchWindowManager)
            searchWindowManager.OpenSearchWindow();
        else
            Debug.LogError("SetNavigationTarget�� SearchWindowManager�� �Ҵ���� �ʾҽ��ϴ�.");
    }

    // ��ǥ ������ �����ϰ� ��θ� ������Ʈ
    public void SetTargetByObject(GameObject target)
    {
        if (target)
        {
            Vector3 targetPosition = target.transform.position;
            UpdatePath(targetPosition);
            Debug.Log($"�׺���̼� ��ǥ�� �����Ǿ����ϴ�: {target.name}");
        }
        else
            Debug.LogWarning("��ǥ�� �����ϴ�.");
    }

    // ��θ� ������Ʈ�ϰ� LineRenderer�� ����
    private void UpdatePath(Vector3 targetPosition)
    {
        // ���� ��ġ���� ��ǥ ���������� ��� ���
        NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, navMeshPath);

        if (navMeshPath.corners.Length > 0)
        {
            lineRenderer.positionCount = navMeshPath.corners.Length;
            lineRenderer.SetPositions(navMeshPath.corners);
            lineRenderer.enabled = true;
            Debug.Log("��ΰ� ������Ʈ�ǰ� LineRenderer�� �����Ǿ����ϴ�.");
        }
        else
        {
            Debug.LogWarning("��� ��� ���� �Ǵ� ��ο� �ڳʰ� �����ϴ�.");
        }
    }
}