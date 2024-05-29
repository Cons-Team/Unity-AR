/*using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NaviTest : MonoBehaviour
{
    [SerializeField]
    private Camera topDownCamera;
    [SerializeField]
    private List<GameObject> navTargetObjects;
    [SerializeField]
    private float playerWalkSpeed = 3.0f;
    [SerializeField]
    private Dropdown destinationDropdown;
    [SerializeField]
    private Image mapImage; // 2D ������ ��Ÿ���� UI Image �߰�

    private NavMeshPath path;
    private LineRenderer line;
    private NavMeshAgent agent;

    private bool lineToggle = false;
    private Vector3[] linePositions; // �׺���̼� ����� ���� ��ġ �迭

    private int currentTargetIndex = -1;

    private void Start()
    {
        path = new NavMeshPath();
        line = transform.GetComponent<LineRenderer>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = playerWalkSpeed;

        InitializeDropdown();
        destinationDropdown.onValueChanged.AddListener(OnDestinationSelected);
    }

    private void Update()
    {
        if (lineToggle && currentTargetIndex >= 0 && currentTargetIndex < navTargetObjects.Count)
        {
            NavMesh.CalculatePath(transform.position, navTargetObjects[currentTargetIndex].transform.position, NavMesh.AllAreas, path);
            linePositions = path.corners; // ����� ��� ���� ��ġ ��������
            UpdateLineRenderer();
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

    // ��ư�� ���� 2D �������� Ư�� ������ Ŭ������ �� ȣ��Ǵ� �޼���
    public void OnMapPointClicked(Vector3 mapPoint)
    {
        RaycastHit hit;
        Ray ray = topDownCamera.ScreenPointToRay(mapPoint);

        if (Physics.Raycast(ray, out hit))
        {
            NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 10f, NavMesh.AllAreas); // Ŭ���� ������ ����� �׺���̼� ������ ��ġ ��������
            SetDestination(navHit.position);
        }
    }

    // Ŭ���� �������� �÷��̾� �̵�
    private void SetDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    // ���� ������ ������Ʈ
    private void UpdateLineRenderer()
    {
        line.positionCount = linePositions.Length;
        line.SetPositions(linePositions);

        // 2D ���� �� ���� �׸���
        Vector2[] linePositions2D = new Vector2[linePositions.Length];
        for (int i = 0; i < linePositions.Length; i++)
        {
            linePositions2D[i] = topDownCamera.WorldToViewportPoint(linePositions[i]); // ���� ��ǥ�� ����Ʈ ��ǥ�� ��ȯ
        }
        mapImage.GetComponent<MapLineRenderer>().DrawLine(linePositions2D); // MapLineRenderer ��ũ��Ʈ�� DrawLine �޼��� ȣ��
    }
}*/