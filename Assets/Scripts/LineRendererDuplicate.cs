using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererDuplicate : MonoBehaviour
{
    public LineRenderer originalLine; // 기존 라인 렌더러를 할당
    public GameObject linePrefab; // Line Renderer가 붙어 있는 프리팹을 할당
    public float yOffset = 2.54f; // Y축으로 복사할 오프셋

    void Start()
    {
        if (originalLine == null)
        {
            Debug.LogError("Original Line Renderer is not assigned.");
            return;
        }

        if (linePrefab == null)
        {
            Debug.LogError("Line Renderer Prefab is not assigned.");
            return;
        }

        // 새로운 라인 렌더러를 생성
        GameObject newLineObj = Instantiate(linePrefab, transform.position, transform.rotation);
        LineRenderer newLine = newLineObj.GetComponent<LineRenderer>();

        if (newLine == null)
        {
            Debug.LogError("Prefab does not contain a Line Renderer component.");
            return;
        }

        // 기존 라인 렌더러의 포인트 가져오기
        int pointCount = originalLine.positionCount;
        if (pointCount == 0)
        {
            Debug.LogError("Original Line Renderer has no points.");
            return;
        }

        Vector3[] originalPositions = new Vector3[pointCount];
        originalLine.GetPositions(originalPositions);

        // 기존 라인의 Y 좌표를 2.54만큼 증가시킨 포인트 배열 생성
        Vector3[] newPositions = new Vector3[pointCount];
        for (int i = 0; i < pointCount; i++)
        {
            newPositions[i] = originalPositions[i] + new Vector3(0, yOffset, 0);
        }

        // 새로운 라인 렌더러에 좌표 적용
        newLine.positionCount = pointCount;
        newLine.SetPositions(newPositions);

        Debug.Log("New Line Renderer created with offset positions.");
    }
}