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
        if (lineRenderer != null && lineRenderer.material != null)
        {
            Color materialColor = lineRenderer.material.color;
            materialColor.a = opacity; // ���� ä���� ����Ͽ� ���� ����
            lineRenderer.material.color = materialColor;
        }
    }
}