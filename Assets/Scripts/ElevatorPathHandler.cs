using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ElevatorPathHandler : MonoBehaviour
{
    [SerializeField] private LineRenderer line; // 경로를 표시할 LineRenderer
    private Coroutine elevatorCoroutine; // 엘리베이터 경로 계산을 위한 코루틴
    [SerializeField] private int elevatorTravelTime = 20; // 엘리베이터 이동 대기 시간
    [SerializeField] private PathUpdater pathUpdater; // PathUpdater 참조

    private void Start()
    {
        // LineRenderer가 할당되지 않은 경우 컴포넌트에서 가져오기
        if (!line) line = GetComponent<LineRenderer>();
    }

    // 엘리베이터를 이용한 경로 계산 메서드
    public void CalculatePathUsingElevator(Vector3 startPosition, Vector3 targetPosition, Vector3 elevatorEntry, Vector3 elevatorExit, Action<List<Vector3>> onPathCalculated)
    {
        // 엘리베이터를 사용할 때의 경로 계산 로직
        List<Vector3> pathToElevator = CalculatePath(startPosition, elevatorEntry);
        List<Vector3> pathFromElevator = CalculatePath(elevatorExit, targetPosition);

        List<Vector3> completePath = new List<Vector3>(pathToElevator);
        completePath.Add(elevatorEntry);
        completePath.Add(elevatorExit);
        completePath.AddRange(pathFromElevator);

        onPathCalculated?.Invoke(completePath);
    }

    private List<Vector3> CalculatePath(Vector3 start, Vector3 end)
    {
        List<Vector3> path = new List<Vector3>();

        // NavMeshPath 또는 다른 경로 계산 로직을 사용하여 경로를 계산
        NavMeshPath navMeshPath = new NavMeshPath();
        NavMesh.CalculatePath(start, end, NavMesh.AllAreas, navMeshPath);

        path.AddRange(navMeshPath.corners);

        return path;
    }

    private IEnumerator CalculatePath(Vector3 startPosition, Vector3 targetPosition, Vector3 elevatorEntry, Vector3 elevatorExit)
    {
        List<Vector3> completePath = new List<Vector3>();

        // 출발 위치에서 엘리베이터 입구까지 경로 계산
        NavMeshPath pathToElevator = new NavMeshPath();
        NavMesh.CalculatePath(startPosition, elevatorEntry, NavMesh.AllAreas, pathToElevator);
        completePath.AddRange(pathToElevator.corners);

        // 경로 표시 업데이트
        UpdateLineRenderer(completePath);

        // 디버그 로그 추가
        Debug.Log($"엘리베이터 대기 시간: {elevatorTravelTime}초");

        // 엘리베이터 이동 대기 시간
        yield return new WaitForSeconds(elevatorTravelTime); // 대기 시간 설정

        // 엘리베이터 출구에서 목표 지점까지 경로 계산
        NavMeshPath pathFromElevator = new NavMeshPath();
        NavMesh.CalculatePath(elevatorExit, targetPosition, NavMesh.AllAreas, pathFromElevator);
        completePath.AddRange(pathFromElevator.corners);

        // 최종 경로 표시 업데이트
        UpdateLineRenderer(completePath);

        // 도착 예정 시간 업데이트
        if (pathUpdater != null)
        {
            pathUpdater.UpdateArrivalTimeWithElevator(elevatorTravelTime, completePath); // 엘리베이터 대기 시간 포함
        }

        // 디버그 로그 추가
        Debug.Log($"엘리베이터 경로 계산 완료. 시작 위치: {startPosition}, 목표 위치: {targetPosition}, 엘리베이터 대기 시간: {elevatorTravelTime}");
    }

    // LineRenderer를 사용하여 경로 표시 업데이트
    private void UpdateLineRenderer(List<Vector3> pathPoints)
    {
        line.positionCount = pathPoints.Count;
        line.SetPositions(pathPoints.ToArray());
        line.enabled = true;
    }

    // 엘리베이터 이동 시간 계산 메서드
    public void GetElevatorTravelTime(out float elevatorTravelTime)
    {
        // 예시로 20초를 엘리베이터 대기 시간으로 설정
        elevatorTravelTime = 20f; // 필요에 따라 수정 가능
    }
}