using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ElevatorPathHandler : MonoBehaviour
{
    [SerializeField] private LineRenderer line; // 경로를 표시할 LineRenderer
    private Coroutine elevatorCoroutine; // 엘리베이터 경로 계산을 위한 코루틴

    private void Start()
    {
        // LineRenderer가 할당되지 않은 경우 컴포넌트에서 가져오기
        if (!line) line = GetComponent<LineRenderer>();
    }

    // 엘리베이터를 이용한 경로 계산 메서드
    public void CalculatePathUsingElevator(Vector3 startPosition, Vector3 targetPosition, Vector3 elevatorEntry, Vector3 elevatorExit)
    {
        // 기존 코루틴이 실행 중이면 중지
        if (elevatorCoroutine != null)
        {
            StopCoroutine(elevatorCoroutine);
        }
        // 새로운 경로 계산 코루틴 시작
        elevatorCoroutine = StartCoroutine(CalculatePathCoroutine(startPosition, targetPosition, elevatorEntry, elevatorExit));
    }

    // 엘리베이터 경로를 계산하는 코루틴
    private IEnumerator CalculatePathCoroutine(Vector3 startPosition, Vector3 targetPosition, Vector3 elevatorEntry, Vector3 elevatorExit)
    {
        List<Vector3> completePath = new List<Vector3>();

        // 시작 지점에서 엘리베이터 입구까지 경로 계산
        NavMeshPath pathToElevator = new NavMeshPath();
        NavMesh.CalculatePath(startPosition, elevatorEntry, NavMesh.AllAreas, pathToElevator);
        completePath.AddRange(pathToElevator.corners);

        // 경로 표시 업데이트
        UpdateLineRenderer(completePath);

        // 엘리베이터 이동 대기 시간
        yield return new WaitForSeconds(2);

        // 엘리베이터 출구에서 목표 지점까지 경로 계산
        NavMeshPath pathFromElevator = new NavMeshPath();
        NavMesh.CalculatePath(elevatorExit, targetPosition, NavMesh.AllAreas, pathFromElevator);
        completePath.AddRange(pathFromElevator.corners);

        // 최종 경로 표시 업데이트
        UpdateLineRenderer(completePath);
    }

    // LineRenderer를 사용하여 경로 표시 업데이트
    private void UpdateLineRenderer(List<Vector3> pathPoints)
    {
        line.positionCount = pathPoints.Count;
        line.SetPositions(pathPoints.ToArray());
        line.enabled = true;
    }
}