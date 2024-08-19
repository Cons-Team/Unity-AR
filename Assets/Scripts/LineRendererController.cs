using UnityEngine;
using UnityEngine.UI;

public class LineRendererController : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;

    [Range(0, 1)]
    [SerializeField]
    private float lineOpacity = 1.0f; // �ʱ� ����

    [SerializeField]
    private Slider opacitySlider; // ���� ���� �����̴�

    private void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        if (opacitySlider != null)
        {
            // �����̴� �ʱ�ȭ �� ������ ����
            opacitySlider.value = lineOpacity;
            opacitySlider.onValueChanged.AddListener(SetLineOpacity);
        }

        // �ʱ� ���� ����
        SetLineOpacity(lineOpacity);
    }

    // ������ ������ �����ϴ� �޼���
    public void SetLineOpacity(float opacity)
    {
        lineOpacity = opacity;
        if (lineRenderer != null)
        {
            Color lineColor = lineRenderer.startColor;
            lineColor.a = opacity;
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
        }
    }

    // ������ ������ �����ϴ� �޼���
    public void SetLineColor(Color color)
    {
        if (lineRenderer != null)
        {
            color.a = lineOpacity; // ���� ����
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }
    }

    // ������ ��ġ�� �����ϴ� �޼���
    public void SetLinePositions(Vector3[] positions)
    {
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = positions.Length;
            lineRenderer.SetPositions(positions);
        }
    }

    // ������ Ȱ��ȭ/��Ȱ��ȭ�ϴ� �޼���
    public void SetLineVisibility(bool isVisible)
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = isVisible;
        }
    }
}