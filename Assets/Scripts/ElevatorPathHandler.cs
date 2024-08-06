using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ElevatorPathHandler : MonoBehaviour
{
    [SerializeField]
    private LineRenderer line;

    private Coroutine elevatorCoroutine;

    private void Start()
    {
        if (line == null)
        {
            line = GetComponent<LineRenderer>();
        }
    }

    public void CalculatePathUsingElevator(Vector3 startPosition, Vector3 targetPosition, Vector3 elevatorEntry, Vector3 elevatorExit)
    {
        if (elevatorCoroutine != null)
        {
            StopCoroutine(elevatorCoroutine);
        }
        elevatorCoroutine = StartCoroutine(CalculatePathCoroutine(startPosition, targetPosition, elevatorEntry, elevatorExit));
    }

    private IEnumerator CalculatePathCoroutine(Vector3 startPosition, Vector3 targetPosition, Vector3 elevatorEntry, Vector3 elevatorExit)
    {
        List<Vector3> completePath = new List<Vector3>();

        // ���������͸� ����ϴ� ��� ���
        NavMeshPath pathToElevator = new NavMeshPath();
        NavMesh.CalculatePath(startPosition, elevatorEntry, NavMesh.AllAreas, pathToElevator);
        completePath.AddRange(pathToElevator.corners);

        // ��� ǥ��
        UpdateLineRenderer(completePath);

        yield return new WaitForSeconds(2); // ���������� �̵� �ð�

        // ���������� �̵� �� ��ǥ ��ġ����
        NavMeshPath pathFromElevator = new NavMeshPath();
        NavMesh.CalculatePath(elevatorExit, targetPosition, NavMesh.AllAreas, pathFromElevator);
        completePath.AddRange(pathFromElevator.corners);

        // ���� ��� ǥ��
        UpdateLineRenderer(completePath);
    }

    private void UpdateLineRenderer(List<Vector3> pathPoints)
    {
        line.positionCount = pathPoints.Count;
        line.SetPositions(pathPoints.ToArray());
        line.enabled = true;
    }
}