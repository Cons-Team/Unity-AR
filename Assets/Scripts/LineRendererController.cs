using UnityEngine;
using UnityEngine.UI;

public class LineRendererController : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer; // LineRenderer ������Ʈ
    [Range(0, 1)][SerializeField] private float lineOpacity = 1.0f; // �ʱ� ���� (0: ������ ����, 1: ������ ������)
    [SerializeField] private Slider opacitySlider; // ���� ������ ���� �����̴�

    private void Start()
    {
        // LineRenderer�� �Ҵ���� ���� ���, ���� GameObject���� ��������
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        // �����̴��� �����Ǿ� ������ �ʱ�ȭ �� ������ �߰�
        if (opacitySlider != null)
        {
            opacitySlider.value = lineOpacity; // �����̴� �ʱⰪ ����
            opacitySlider.onValueChanged.AddListener(SetLineOpacity); // �����̴� �� ���� �� ȣ��� �޼��� ���
        }

        // �ʱ� ���� ����
        SetLineOpacity(lineOpacity);
    }

    // LineRenderer�� ������ �����ϴ� �޼���
    public void SetLineOpacity(float opacity)
    {
        lineOpacity = opacity;

        if (lineRenderer != null && lineRenderer.material != null)
        {
            Color materialColor = lineRenderer.material.color;
            materialColor.a = opacity; // ���� ���� �����Ͽ� ���� ����
            lineRenderer.material.color = materialColor;
        }
    }
}