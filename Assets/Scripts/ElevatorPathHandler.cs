using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ElevatorPathHandler : MonoBehaviour
{
    [SerializeField] private LineRenderer line; // ��θ� ǥ���� LineRenderer
    private Coroutine elevatorCoroutine; // ���������� ��� ����� ���� �ڷ�ƾ
    [SerializeField] private int elevatorTravelTime = 20; // ���������� �̵� ��� �ð�
    [SerializeField] private PathUpdater pathUpdater; // PathUpdater ����

    private void Start()
    {
        // LineRenderer�� �Ҵ���� ���� ��� ������Ʈ���� ��������
        if (!line) line = GetComponent<LineRenderer>();
    }

    // ���������͸� �̿��� ��� ��� �޼���
    public void CalculatePathUsingElevator(Vector3 startPosition, Vector3 targetPosition, Vector3 elevatorEntry, Vector3 elevatorExit, Action<List<Vector3>> onPathCalculated)
    {
        // ���������͸� ����� ���� ��� ��� ����
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

        // NavMeshPath �Ǵ� �ٸ� ��� ��� ������ ����Ͽ� ��θ� ���
        NavMeshPath navMeshPath = new NavMeshPath();
        NavMesh.CalculatePath(start, end, NavMesh.AllAreas, navMeshPath);

        path.AddRange(navMeshPath.corners);

        return path;
    }

    private IEnumerator CalculatePath(Vector3 startPosition, Vector3 targetPosition, Vector3 elevatorEntry, Vector3 elevatorExit)
    {
        List<Vector3> completePath = new List<Vector3>();

        // ��� ��ġ���� ���������� �Ա����� ��� ���
        NavMeshPath pathToElevator = new NavMeshPath();
        NavMesh.CalculatePath(startPosition, elevatorEntry, NavMesh.AllAreas, pathToElevator);
        completePath.AddRange(pathToElevator.corners);

        // ��� ǥ�� ������Ʈ
        UpdateLineRenderer(completePath);

        // ����� �α� �߰�
        Debug.Log($"���������� ��� �ð�: {elevatorTravelTime}��");

        // ���������� �̵� ��� �ð�
        yield return new WaitForSeconds(elevatorTravelTime); // ��� �ð� ����

        // ���������� �ⱸ���� ��ǥ �������� ��� ���
        NavMeshPath pathFromElevator = new NavMeshPath();
        NavMesh.CalculatePath(elevatorExit, targetPosition, NavMesh.AllAreas, pathFromElevator);
        completePath.AddRange(pathFromElevator.corners);

        // ���� ��� ǥ�� ������Ʈ
        UpdateLineRenderer(completePath);

        // ���� ���� �ð� ������Ʈ
        if (pathUpdater != null)
        {
            pathUpdater.UpdateArrivalTimeWithElevator(elevatorTravelTime, completePath); // ���������� ��� �ð� ����
        }

        // ����� �α� �߰�
        Debug.Log($"���������� ��� ��� �Ϸ�. ���� ��ġ: {startPosition}, ��ǥ ��ġ: {targetPosition}, ���������� ��� �ð�: {elevatorTravelTime}");
    }

    // LineRenderer�� ����Ͽ� ��� ǥ�� ������Ʈ
    private void UpdateLineRenderer(List<Vector3> pathPoints)
    {
        line.positionCount = pathPoints.Count;
        line.SetPositions(pathPoints.ToArray());
        line.enabled = true;
    }

    // ���������� �̵� �ð� ��� �޼���
    public void GetElevatorTravelTime(out float elevatorTravelTime)
    {
        // ���÷� 20�ʸ� ���������� ��� �ð����� ����
        elevatorTravelTime = 20f; // �ʿ信 ���� ���� ����
    }
}