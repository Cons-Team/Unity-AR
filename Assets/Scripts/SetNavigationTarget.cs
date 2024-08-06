/*using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SetNavigationTarget : MonoBehaviour
{
    [SerializeField]
    private Camera topDownCamera;
    [SerializeField]
    private TMP_Dropdown targetDropdown;
    [SerializeField]
    private TMP_Dropdown pathOptionDropdown; // 엘리베이터/계단 선택 옵션을 추가
    [SerializeField]
    private List<GameObject> navTargetObjects;
    [SerializeField]
    private GameObject elevatorEntry1F; // 1층 엘리베이터 입구 위치
    [SerializeField]
    private GameObject elevatorExit2F;  // 2층 엘리베이터 출구 위치
    [SerializeField]
    private GameObject elevatorEntry2F; // 2층 엘리베이터 입구 위치
    [SerializeField]
    private GameObject elevatorExit1F;  // 1층 엘리베이터 출구 위치

    private NavMeshPath path;
    private LineRenderer line;
    private Coroutine elevatorCoroutine;

    private void Start()
    {
        path = new NavMeshPath();
        line = transform.GetComponent<LineRenderer>();

        // Target Dropdown 초기화
        targetDropdown.ClearOptions();
        List<string> targetNames = new List<string>();
        foreach (GameObject target in navTargetObjects)
        {
            targetNames.Add(target.name);
        }
        targetDropdown.AddOptions(targetNames);
        targetDropdown.onValueChanged.AddListener(OnTargetDropdownValueChanged);

        // Path Option Dropdown 초기화
        pathOptionDropdown.ClearOptions();
        List<string> pathOptions = new List<string> { "Stairs", "Elevator" };
        pathOptionDropdown.AddOptions(pathOptions);
        pathOptionDropdown.onValueChanged.AddListener(OnPathOptionDropdownValueChanged);

        // 초기 경로 계산
        if (navTargetObjects.Count > 0)
        {
            UpdatePath(navTargetObjects[0].transform.position);
        }
    }

    private void OnTargetDropdownValueChanged(int index)
    {
        if (elevatorCoroutine != null)
        {
            StopCoroutine(elevatorCoroutine);
        }
        UpdatePath(navTargetObjects[index].transform.position);
    }

    private void OnPathOptionDropdownValueChanged(int index)
    {
        // 경로 옵션이 변경될 때 경로를 다시 계산
        if (navTargetObjects.Count > 0)
        {
            if (elevatorCoroutine != null)
            {
                StopCoroutine(elevatorCoroutine);
            }
            UpdatePath(navTargetObjects[targetDropdown.value].transform.position);
        }
    }

    private void UpdatePath(Vector3 targetPosition)
    {
        Debug.Log("UpdatePath called");
        if (pathOptionDropdown.value == 1) // 엘리베이터 옵션이 선택된 경우
        {
            // 엘리베이터를 사용하는 경로 계산
            elevatorCoroutine = StartCoroutine(CalculatePathUsingElevator(transform.position, targetPosition));
        }
        else
        {
            // 계단을 사용하는 기본 경로 계산
            NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path);
            if (path.corners.Length > 0)
            {
                line.positionCount = path.corners.Length;
                line.SetPositions(path.corners);
            }
            line.enabled = true;
            Debug.Log("Path calculated using stairs");
        }
    }

    private IEnumerator CalculatePathUsingElevator(Vector3 startPosition, Vector3 targetPosition)
    {
        List<Vector3> completePath = new List<Vector3>();
        Debug.Log("CalculatePathUsingElevator started");

        if (IsOnFirstFloor(startPosition) && IsOnSecondFloor(targetPosition))
        {
            Debug.Log("Navigating from 1F to 2F using elevator");

            // 1층에서 2층으로 이동
            NavMeshPath pathToElevator = new NavMeshPath();
            NavMesh.CalculatePath(startPosition, elevatorEntry1F.transform.position, NavMesh.AllAreas, pathToElevator);
            completePath.AddRange(pathToElevator.corners);

            // 경로 표시
            line.positionCount = completePath.Count;
            line.SetPositions(completePath.ToArray());
            line.enabled = true;

            Debug.Log("Navigating to elevator entry on 1F");

            // 2초 대기 (엘리베이터 이동 시간)
            yield return new WaitForSeconds(2);

            // 엘리베이터 이동 후 2층에서 목표 위치까지
            NavMeshPath pathFromElevator = new NavMeshPath();
            NavMesh.CalculatePath(elevatorExit2F.transform.position, targetPosition, NavMesh.AllAreas, pathFromElevator);
            completePath.AddRange(pathFromElevator.corners);

            Debug.Log("Navigating from elevator exit on 2F to target");
        }
        else if (IsOnSecondFloor(startPosition) && IsOnFirstFloor(targetPosition))
        {
            Debug.Log("Navigating from 2F to 1F using elevator");

            // 2층에서 1층으로 이동
            NavMeshPath pathToElevator = new NavMeshPath();
            NavMesh.CalculatePath(startPosition, elevatorEntry2F.transform.position, NavMesh.AllAreas, pathToElevator);
            completePath.AddRange(pathToElevator.corners);

            // 경로 표시
            line.positionCount = completePath.Count;
            line.SetPositions(completePath.ToArray());
            line.enabled = true;

            Debug.Log("Navigating to elevator entry on 2F");

            // 2초 대기 (엘리베이터 이동 시간)
            yield return new WaitForSeconds(2);

            // 엘리베이터 이동 후 1층에서 목표 위치까지
            NavMeshPath pathFromElevator = new NavMeshPath();
            NavMesh.CalculatePath(elevatorExit1F.transform.position, targetPosition, NavMesh.AllAreas, pathFromElevator);
            completePath.AddRange(pathFromElevator.corners);

            Debug.Log("Navigating from elevator exit on 1F to target");
        }
        else
        {
            Debug.Log("Navigating on the same floor");

            // 동일 층 내에서 이동
            NavMeshPath directPath = new NavMeshPath();
            NavMesh.CalculatePath(startPosition, targetPosition, NavMesh.AllAreas, directPath);
            completePath.AddRange(directPath.corners);
        }

        // 최종 경로 표시
        line.positionCount = completePath.Count;
        line.SetPositions(completePath.ToArray());
        line.enabled = true;

        Debug.Log("Final path calculated and set");
    }

    private bool IsOnFirstFloor(Vector3 position)
    {
        // 여기서 1층 여부를 확인하는 로직을 구현하세요 (예: Y 좌표 기반으로 확인)
        return position.y < -2.29f; // 예시: Y 좌표가 1.5 이하인 경우 1층으로 간주
    }

    private bool IsOnSecondFloor(Vector3 position)
    {
        // 여기서 2층 여부를 확인하는 로직을 구현하세요 (예: Y 좌표 기반으로 확인)
        return position.y >= 1.2f; // 예시: Y 좌표가 1.5 이상인 경우 2층으로 간주
    }

    private void Update()
    {
        // 현재 위치가 변경되는 경우 경로를 업데이트
        if (navTargetObjects.Count > 0)
        {
            if (elevatorCoroutine == null)
            {
                UpdatePath(navTargetObjects[targetDropdown.value].transform.position);
            }
        }
    }
}*/