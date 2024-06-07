/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro; // TextMeshPro 네임스페이스 추가

public class NaviTest : MonoBehaviour
{
    [SerializeField]
    private Camera topDownCamera;
    [SerializeField]
    private List<GameObject> navTargetObjects; // 여러 네비게이션 목적지 리스트
    [SerializeField]
    private float playerWalkSpeed = 3.0f; // 플레이어 걷는 속도
    [SerializeField]
    private TMP_InputField searchInputField; // 검색 입력 필드
    [SerializeField]
    private TMP_Dropdown destinationDropdown; // 목적지 선택을 위한 Dropdown
    [SerializeField]
    private Toggle elderlyToggle; // 노약자 옵션을 위한 Toggle UI
    [SerializeField]
    private Transform elevatorStart; // 엘리베이터 시작 위치
    [SerializeField]
    private Transform elevatorEnd; // 엘리베이터 종료 위치

    private NavMeshPath path;
    private LineRenderer line;
    private NavMeshAgent agent;
    private bool lineToggle = false;
    private float updateInterval = 0.1f;
    private float timeSinceLastUpdate = 0f;
    private List<int> selectedTargetIndices = new List<int>(); // 선택된 목적지 인덱스 리스트
    private bool isUsingElevator = false; // 엘리베이터 사용 중 여부

    private void Start()
    {
        path = new NavMeshPath();
        line = transform.GetComponent<LineRenderer>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = playerWalkSpeed;

        InitializeDropdown();
        destinationDropdown.onValueChanged.AddListener(OnDestinationSelected);
        searchInputField.onValueChanged.AddListener(OnSearchValueChanged);
    }

    private void Update()
    {
        timeSinceLastUpdate += Time.deltaTime;
        if (timeSinceLastUpdate >= updateInterval)
        {
            timeSinceLastUpdate = 0f;

            if (lineToggle && selectedTargetIndices.Count > 0)
            {
                if (elderlyToggle.isOn && !isUsingElevator)
                {
                    StartCoroutine(UseElevatorAndNavigate());
                }
                else
                {
                    CalculatePathToCurrentTargets();
                }
            }
        }

        if (selectedTargetIndices.Count > 0 && Vector3.Distance(transform.position, navTargetObjects[selectedTargetIndices[0]].transform.position) < 0.5f)
        {
            selectedTargetIndices.RemoveAt(0);
            if (selectedTargetIndices.Count == 0)
            {
                lineToggle = false;
                line.enabled = false;
            }
        }
    }

    private void InitializeDropdown()
    {
        destinationDropdown.ClearOptions();
        List<string> options = new List<string>();
        foreach (GameObject target in navTargetObjects)
        {
            options.Add(target.name);
        }
        destinationDropdown.AddOptions(options);
    }

    public void OnDestinationSelected(int index)
    {
        selectedTargetIndices.Add(index);
        lineToggle = true;
    }

    public void OnSearchValueChanged(string searchText)
    {
        List<string> filteredOptions = new List<string>();
        for (int i = 0; i < navTargetObjects.Count; i++)
        {
            if (navTargetObjects[i].name.ToLower().Contains(searchText.ToLower()))
            {
                filteredOptions.Add(navTargetObjects[i].name);
            }
        }
        destinationDropdown.ClearOptions();
        destinationDropdown.AddOptions(filteredOptions);
    }

    private void CalculatePathToCurrentTargets()
    {
        if (selectedTargetIndices.Count == 0)
            return;

        Vector3 startPosition = transform.position;
        List<Vector3> fullPath = new List<Vector3>();

        foreach (int targetIndex in selectedTargetIndices)
        {
            if (NavMesh.CalculatePath(startPosition, navTargetObjects[targetIndex].transform.position, NavMesh.AllAreas, path))
            {
                fullPath.AddRange(path.corners);
                startPosition = navTargetObjects[targetIndex].transform.position;
            }
            else
            {
                Debug.LogError("Failed to calculate path to target.");
                line.enabled = false;
                return;
            }
        }

        UpdateLineRenderer(fullPath.ToArray());
    }

    private IEnumerator UseElevatorAndNavigate()
    {
        isUsingElevator = true;
        agent.enabled = false;

        yield return MoveTo(elevatorStart.position);
        yield return MoveElevator(elevatorStart.position, elevatorEnd.position);

        transform.position = elevatorEnd.position;
        agent.enabled = true;
        agent.Warp(elevatorEnd.position);

        CalculatePathToCurrentTargets();
        isUsingElevator = false;
    }

    private IEnumerator MoveTo(Vector3 destination)
    {
        float elapsedTime = 0f;
        float duration = 1f;
        Vector3 initialPosition = transform.position;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(initialPosition, destination, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = destination;
    }

    private IEnumerator MoveElevator(Vector3 start, Vector3 end)
    {
        float elapsedTime = 0f;
        float duration = 2f;
        Vector3 initialPosition = start;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(initialPosition, end, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
    }

    private void UpdateLineRenderer(Vector3[] corners)
    {
        line.positionCount = corners.Length;
        line.SetPositions(corners);
        line.enabled = true;
    }

    public void UpdatePlayerPosition(float[] coordinates)
    {
        if (coordinates.Length == 3)
        {
            Vector3 newPosition = new Vector3(coordinates[0], coordinates[2], coordinates[1]);
            transform.position = newPosition;
            agent.Warp(newPosition);
        }
    }
}*/