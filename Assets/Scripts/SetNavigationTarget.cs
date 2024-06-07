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
    private List<GameObject> navTargetObjects;

    private NavMeshPath path;
    private LineRenderer line;

    private void Start()
    {
        path = new NavMeshPath();
        line = transform.GetComponent<LineRenderer>();

        // Dropdown 초기화
        targetDropdown.ClearOptions();
        List<string> targetNames = new List<string>();
        foreach (GameObject target in navTargetObjects)
        {
            targetNames.Add(target.name);
        }
        targetDropdown.AddOptions(targetNames);
        targetDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // 초기 경로 계산
        if (navTargetObjects.Count > 0)
        {
            UpdatePath(navTargetObjects[0].transform.position);
        }
    }

    private void OnDropdownValueChanged(int index)
    {
        UpdatePath(navTargetObjects[index].transform.position);
    }

    private void UpdatePath(Vector3 targetPosition)
    {
        NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path);
        line.positionCount = path.corners.Length;
        line.SetPositions(path.corners);
        line.enabled = true;
    }

    private void Update()
    {
        // 현재 위치가 변경되는 경우 경로를 업데이트
        if (navTargetObjects.Count > 0)
        {
            UpdatePath(navTargetObjects[targetDropdown.value].transform.position);
        }
    }
}