using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NaviTest : MonoBehaviour
{
    [SerializeField]
    private Camera topDownCamera;
    [SerializeField]
    private GameObject navTargetObject;
    [SerializeField]
    private float playerWalkSpeed = 3.0f; // �÷��̾� �ȴ� �ӵ�

    private NavMeshPath path;
    private LineRenderer line;
    private NavMeshAgent agent; // NavMeshAgent �߰�

    private bool lineToggle = false;

    // ���� ������Ʈ ����
    private float updateInterval = 0.1f; // 0.1�ʸ��� ������Ʈ

    private float timeSinceLastUpdate = 0f;

    private void Start()
    {
        path = new NavMeshPath();
        line = transform.GetComponent<LineRenderer>();
        agent = GetComponent<NavMeshAgent>(); // NavMeshAgent ������Ʈ ��������
        agent.speed = playerWalkSpeed; // �÷��̾��� �ȴ� �ӵ� ����
    }

    private void Update()
    {
        // ���� ������Ʈ ���� üũ
        timeSinceLastUpdate += Time.deltaTime;
        if (timeSinceLastUpdate >= updateInterval)
        {
            // ���� �ð��� ���� �Ŀ��� ���� ������Ʈ
            timeSinceLastUpdate = 0f;

            if (lineToggle)
            {
                NavMesh.CalculatePath(transform.position, navTargetObject.transform.position, NavMesh.AllAreas, path);
                line.positionCount = path.corners.Length;
                line.SetPositions(path.corners);
                line.enabled = true;
            }
        }

        // ��ġ �Է��� ���� ���� ���
        if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
        {
            lineToggle = !lineToggle;
        }
    }
}