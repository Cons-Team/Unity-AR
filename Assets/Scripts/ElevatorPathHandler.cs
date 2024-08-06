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

        // 엘리베이터를 사용하는 경로 계산
        NavMeshPath pathToElevator = new NavMeshPath();
        NavMesh.CalculatePath(startPosition, elevatorEntry, NavMesh.AllAreas, pathToElevator);
        completePath.AddRange(pathToElevator.corners);

        // 경로 표시
        UpdateLineRenderer(completePath);

        yield return new WaitForSeconds(2); // 엘리베이터 이동 시간

        // 엘리베이터 이동 후 목표 위치까지
        NavMeshPath pathFromElevator = new NavMeshPath();
        NavMesh.CalculatePath(elevatorExit, targetPosition, NavMesh.AllAreas, pathFromElevator);
        completePath.AddRange(pathFromElevator.corners);

        // 최종 경로 표시
        UpdateLineRenderer(completePath);
    }

    private void UpdateLineRenderer(List<Vector3> pathPoints)
    {
        line.positionCount = pathPoints.Count;
        line.SetPositions(pathPoints.ToArray());
        line.enabled = true;
    }
}