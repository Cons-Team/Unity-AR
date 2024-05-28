using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI; // UI�� ����ϱ� ���� �߰�
using TMPro; // ���ӽ����̽�

public class NaviTest : MonoBehaviour
{
    [SerializeField]
    private Camera topDownCamera;
    [SerializeField]
    private List<GameObject> navTargetObjects; // ���� �׺���̼� ������ ����Ʈ
    [SerializeField]
    private float playerWalkSpeed = 3.0f; // �÷��̾� �ȴ� �ӵ�
    [SerializeField]
    private TMP_Dropdown destinationDropdown; // UI �߰� ������ ����Ʈ

    private NavMeshPath path;
    private LineRenderer line;
    private NavMeshAgent agent; // NavMeshAgent �߰�

    private bool lineToggle = false;

    // ���� ������Ʈ ����
    private float updateInterval = 0.1f; // 0.1�ʸ��� ������Ʈ

    private float timeSinceLastUpdate = 0f;

    private int currentTargetIndex = -1; // ���� �׺���̼� ������ �ε���, �ʱⰪ -1 (���� �ȵ� ����)

    private void Start()
    {
        path = new NavMeshPath();
        line = transform.GetComponent<LineRenderer>();
        agent = GetComponent<NavMeshAgent>(); // NavMeshAgent ������Ʈ ��������
        agent.speed = playerWalkSpeed; // �÷��̾��� �ȴ� �ӵ� ����

        // Dropdown �ʱ�ȭ �� �̺�Ʈ ������ �߰�
        InitializeDropdown();
        destinationDropdown.onValueChanged.AddListener(OnDestinationSelected);
    }

    private void Update()
    {
        // ���� ������Ʈ ���� üũ
        timeSinceLastUpdate += Time.deltaTime;
        if (timeSinceLastUpdate >= updateInterval)
        {
            // ���� �ð��� ���� �Ŀ��� ���� ������Ʈ
            timeSinceLastUpdate = 0f;

            if (lineToggle && currentTargetIndex >= 0 && currentTargetIndex < navTargetObjects.Count)
            {
                NavMesh.CalculatePath(transform.position, navTargetObjects[currentTargetIndex].transform.position, NavMesh.AllAreas, path);
                line.positionCount = path.corners.Length;
                line.SetPositions(path.corners);
                line.enabled = true;
            }
        }

        // �÷��̾ ���� �������� �����ߴ��� Ȯ��
        if (currentTargetIndex >= 0 && Vector3.Distance(transform.position, navTargetObjects[currentTargetIndex].transform.position) < 0.5f)
        {
            lineToggle = false;
            line.enabled = false;
            currentTargetIndex = -1; // ������ ���� �� �ʱ�ȭ
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
        currentTargetIndex = index;
        lineToggle = true;
    }

    // �������κ��� ���� ��ǥ�� ���� �÷��̾��� ��ġ�� ������Ʈ�ϴ� �޼���
    public void UpdatePlayerPosition(float[] coordinates)
    {
        if (coordinates.Length == 3)
        {
            Vector3 newPosition = new Vector3(coordinates[0], coordinates[2], coordinates[1]); // x, z, y �����̶�� ����
            transform.position = newPosition;
            agent.Warp(newPosition); // NavMeshAgent�� ���ο� ��ġ�� �̵�
        }
    }
}