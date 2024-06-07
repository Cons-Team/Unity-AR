using System.Collections;
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
    private List<GameObject> navTargetObjects; // ���� �׺���̼� ������ ����Ʈ

    private NavMeshPath path;
    private LineRenderer line;
    private Vector3 lastPosition;

    private void Start()
    {
        path = new NavMeshPath();
        line = GetComponent<LineRenderer>();

        // Dropdown �ʱ�ȭ
        targetDropdown.ClearOptions();
        List<string> targetNames = new List<string>();
        foreach (GameObject target in navTargetObjects)
        {
            targetNames.Add(target.name);
        }
        targetDropdown.AddOptions(targetNames);
        targetDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // �ʱ� ��� ���
        if (navTargetObjects.Count > 0)
        {
            UpdatePath(navTargetObjects[0].transform.position);
        }

        // �ȵ���̵� �ڵ�
        AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
        string coordinate = androidJavaObject.Call<string>("GetCoordinate");
        UpdatePlayerPosition(coordinate);
    }

    private void OnDropdownValueChanged(int index)
    {
        UpdatePath(navTargetObjects[index].transform.position);
    }

    private void UpdatePath(Vector3 targetPosition)
    {
        NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path);
        if (path.status == NavMeshPathStatus.PathComplete)
        {
            line.positionCount = path.corners.Length;
            line.SetPositions(path.corners);
            line.enabled = true;
        }
    }

    private void Update()
    {
        if (navTargetObjects.Count > 0)
        {
            if (Vector3.Distance(transform.position, lastPosition) > 0.1f)
            {
                lastPosition = transform.position;
                UpdatePath(navTargetObjects[targetDropdown.value].transform.position);
            }
        }
    }

    // ���� ��ǥ�� ���� �÷��̾��� ��ġ�� ������Ʈ�ϴ� �޼���
    private void UpdatePlayerPosition(string coordinate)
    {
        string[] coordinates = coordinate.Split(',');
        if (coordinates.Length == 3)
        {
            float x = float.Parse(coordinates[0]);
            float y = float.Parse(coordinates[1]);
            float z = float.Parse(coordinates[2]);
            transform.position = new Vector3(x, y, z);
        }
    }
}