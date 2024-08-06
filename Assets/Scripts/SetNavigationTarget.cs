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
    private TMP_Dropdown pathOptionDropdown; // ����������/��� ���� �ɼ��� �߰�
    [SerializeField]
    private List<GameObject> navTargetObjects;
    [SerializeField]
    private GameObject elevatorEntry1F; // 1�� ���������� �Ա� ��ġ
    [SerializeField]
    private GameObject elevatorExit2F;  // 2�� ���������� �ⱸ ��ġ
    [SerializeField]
    private GameObject elevatorEntry2F; // 2�� ���������� �Ա� ��ġ
    [SerializeField]
    private GameObject elevatorExit1F;  // 1�� ���������� �ⱸ ��ġ

    private NavMeshPath path;
    private LineRenderer line;
    private Coroutine elevatorCoroutine;

    private void Start()
    {
        path = new NavMeshPath();
        line = transform.GetComponent<LineRenderer>();

        // Target Dropdown �ʱ�ȭ
        targetDropdown.ClearOptions();
        List<string> targetNames = new List<string>();
        foreach (GameObject target in navTargetObjects)
        {
            targetNames.Add(target.name);
        }
        targetDropdown.AddOptions(targetNames);
        targetDropdown.onValueChanged.AddListener(OnTargetDropdownValueChanged);

        // Path Option Dropdown �ʱ�ȭ
        pathOptionDropdown.ClearOptions();
        List<string> pathOptions = new List<string> { "Stairs", "Elevator" };
        pathOptionDropdown.AddOptions(pathOptions);
        pathOptionDropdown.onValueChanged.AddListener(OnPathOptionDropdownValueChanged);

        // �ʱ� ��� ���
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
        // ��� �ɼ��� ����� �� ��θ� �ٽ� ���
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
        if (pathOptionDropdown.value == 1) // ���������� �ɼ��� ���õ� ���
        {
            // ���������͸� ����ϴ� ��� ���
            elevatorCoroutine = StartCoroutine(CalculatePathUsingElevator(transform.position, targetPosition));
        }
        else
        {
            // ����� ����ϴ� �⺻ ��� ���
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

            // 1������ 2������ �̵�
            NavMeshPath pathToElevator = new NavMeshPath();
            NavMesh.CalculatePath(startPosition, elevatorEntry1F.transform.position, NavMesh.AllAreas, pathToElevator);
            completePath.AddRange(pathToElevator.corners);

            // ��� ǥ��
            line.positionCount = completePath.Count;
            line.SetPositions(completePath.ToArray());
            line.enabled = true;

            Debug.Log("Navigating to elevator entry on 1F");

            // 2�� ��� (���������� �̵� �ð�)
            yield return new WaitForSeconds(2);

            // ���������� �̵� �� 2������ ��ǥ ��ġ����
            NavMeshPath pathFromElevator = new NavMeshPath();
            NavMesh.CalculatePath(elevatorExit2F.transform.position, targetPosition, NavMesh.AllAreas, pathFromElevator);
            completePath.AddRange(pathFromElevator.corners);

            Debug.Log("Navigating from elevator exit on 2F to target");
        }
        else if (IsOnSecondFloor(startPosition) && IsOnFirstFloor(targetPosition))
        {
            Debug.Log("Navigating from 2F to 1F using elevator");

            // 2������ 1������ �̵�
            NavMeshPath pathToElevator = new NavMeshPath();
            NavMesh.CalculatePath(startPosition, elevatorEntry2F.transform.position, NavMesh.AllAreas, pathToElevator);
            completePath.AddRange(pathToElevator.corners);

            // ��� ǥ��
            line.positionCount = completePath.Count;
            line.SetPositions(completePath.ToArray());
            line.enabled = true;

            Debug.Log("Navigating to elevator entry on 2F");

            // 2�� ��� (���������� �̵� �ð�)
            yield return new WaitForSeconds(2);

            // ���������� �̵� �� 1������ ��ǥ ��ġ����
            NavMeshPath pathFromElevator = new NavMeshPath();
            NavMesh.CalculatePath(elevatorExit1F.transform.position, targetPosition, NavMesh.AllAreas, pathFromElevator);
            completePath.AddRange(pathFromElevator.corners);

            Debug.Log("Navigating from elevator exit on 1F to target");
        }
        else
        {
            Debug.Log("Navigating on the same floor");

            // ���� �� ������ �̵�
            NavMeshPath directPath = new NavMeshPath();
            NavMesh.CalculatePath(startPosition, targetPosition, NavMesh.AllAreas, directPath);
            completePath.AddRange(directPath.corners);
        }

        // ���� ��� ǥ��
        line.positionCount = completePath.Count;
        line.SetPositions(completePath.ToArray());
        line.enabled = true;

        Debug.Log("Final path calculated and set");
    }

    private bool IsOnFirstFloor(Vector3 position)
    {
        // ���⼭ 1�� ���θ� Ȯ���ϴ� ������ �����ϼ��� (��: Y ��ǥ ������� Ȯ��)
        return position.y < -2.29f; // ����: Y ��ǥ�� 1.5 ������ ��� 1������ ����
    }

    private bool IsOnSecondFloor(Vector3 position)
    {
        // ���⼭ 2�� ���θ� Ȯ���ϴ� ������ �����ϼ��� (��: Y ��ǥ ������� Ȯ��)
        return position.y >= 1.2f; // ����: Y ��ǥ�� 1.5 �̻��� ��� 2������ ����
    }

    private void Update()
    {
        // ���� ��ġ�� ����Ǵ� ��� ��θ� ������Ʈ
        if (navTargetObjects.Count > 0)
        {
            if (elevatorCoroutine == null)
            {
                UpdatePath(navTargetObjects[targetDropdown.value].transform.position);
            }
        }
    }
}*/