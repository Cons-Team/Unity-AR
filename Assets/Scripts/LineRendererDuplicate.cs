using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererDuplicate : MonoBehaviour
{
    public LineRenderer originalLine; // ���� ���� �������� �Ҵ�
    public GameObject linePrefab; // Line Renderer�� �پ� �ִ� �������� �Ҵ�
    public float yOffset = 2.54f; // Y������ ������ ������

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

        // ���ο� ���� �������� ����
        GameObject newLineObj = Instantiate(linePrefab, transform.position, transform.rotation);
        LineRenderer newLine = newLineObj.GetComponent<LineRenderer>();

        if (newLine == null)
        {
            Debug.LogError("Prefab does not contain a Line Renderer component.");
            return;
        }

        // ���� ���� �������� ����Ʈ ��������
        int pointCount = originalLine.positionCount;
        if (pointCount == 0)
        {
            Debug.LogError("Original Line Renderer has no points.");
            return;
        }

        Vector3[] originalPositions = new Vector3[pointCount];
        originalLine.GetPositions(originalPositions);

        // ���� ������ Y ��ǥ�� 2.54��ŭ ������Ų ����Ʈ �迭 ����
        Vector3[] newPositions = new Vector3[pointCount];
        for (int i = 0; i < pointCount; i++)
        {
            newPositions[i] = originalPositions[i] + new Vector3(0, yOffset, 0);
        }

        // ���ο� ���� �������� ��ǥ ����
        newLine.positionCount = pointCount;
        newLine.SetPositions(newPositions);

        Debug.Log("New Line Renderer created with offset positions.");
    }
}