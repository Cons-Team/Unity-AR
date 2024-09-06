using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ElevatorPathHandler : MonoBehaviour
{
    [SerializeField] private LineRenderer line; // ��θ� ǥ���� LineRenderer
    private Coroutine elevatorCoroutine; // ���������� ��� ����� ���� �ڷ�ƾ

    private void Start()
    {
        // LineRenderer�� �Ҵ���� ���� ��� ������Ʈ���� ��������
        if (!line) line = GetComponent<LineRenderer>();
    }

    // ���������͸� �̿��� ��� ��� �޼���
    public void CalculatePathUsingElevator(Vector3 startPosition, Vector3 targetPosition, Vector3 elevatorEntry, Vector3 elevatorExit)
    {
        // ���� �ڷ�ƾ�� ���� ���̸� ����
        if (elevatorCoroutine != null)
        {
            StopCoroutine(elevatorCoroutine);
        }
        // ���ο� ��� ��� �ڷ�ƾ ����
        elevatorCoroutine = StartCoroutine(CalculatePathCoroutine(startPosition, targetPosition, elevatorEntry, elevatorExit));
    }

    // ���������� ��θ� ����ϴ� �ڷ�ƾ
    private IEnumerator CalculatePathCoroutine(Vector3 startPosition, Vector3 targetPosition, Vector3 elevatorEntry, Vector3 elevatorExit)
    {
        List<Vector3> completePath = new List<Vector3>();

        // ���� �������� ���������� �Ա����� ��� ���
        NavMeshPath pathToElevator = new NavMeshPath();
        NavMesh.CalculatePath(startPosition, elevatorEntry, NavMesh.AllAreas, pathToElevator);
        completePath.AddRange(pathToElevator.corners);

        // ��� ǥ�� ������Ʈ
        UpdateLineRenderer(completePath);

        // ���������� �̵� ��� �ð�
        yield return new WaitForSeconds(2);

        // ���������� �ⱸ���� ��ǥ �������� ��� ���
        NavMeshPath pathFromElevator = new NavMeshPath();
        NavMesh.CalculatePath(elevatorExit, targetPosition, NavMesh.AllAreas, pathFromElevator);
        completePath.AddRange(pathFromElevator.corners);

        // ���� ��� ǥ�� ������Ʈ
        UpdateLineRenderer(completePath);
    }

    // LineRenderer�� ����Ͽ� ��� ǥ�� ������Ʈ
    private void UpdateLineRenderer(List<Vector3> pathPoints)
    {
        line.positionCount = pathPoints.Count;
        line.SetPositions(pathPoints.ToArray());
        line.enabled = true;
    }
}