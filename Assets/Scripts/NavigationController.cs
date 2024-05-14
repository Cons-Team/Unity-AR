using UnityEngine;
using UnityEngine.AI;

public class NavigationController : MonoBehaviour
{

    public Vector3 TargetPosition { get; set; } = Vector3.zero; // ��ǥ ������ ��ġ�� �����ϰ� �������� �Ӽ�.

    public NavMeshPath CalculatedPath { get; private set; } // ���� ��θ� �������� �Ӽ�.

    private void Start()
    {
        CalculatedPath = new NavMeshPath(); // ���� ��� �ʱ�ȭ.
    }

    private void Update()
    {
        if (TargetPosition != Vector3.zero)
        { // ��ǥ ������ �����Ǿ� ������
            NavMesh.CalculatePath(transform.position, TargetPosition, NavMesh.AllAreas, CalculatedPath); // ���� ��ġ���� ��ǥ ���������� ��� ���.
        }
    }
}