using UnityEngine;
using UnityEngine.UI;

public class LineRendererController : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;

    [Range(0, 1)]
    [SerializeField]
    private float lineOpacity = 1.0f; // 초기 투명도

    [SerializeField]
    private Slider opacitySlider; // 투명도 조절 슬라이더

    private void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        if (opacitySlider != null)
        {
            // 슬라이더 초기화 및 리스너 설정
            opacitySlider.value = lineOpacity;
            opacitySlider.onValueChanged.AddListener(SetLineOpacity);
        }

        // 초기 투명도 설정
        SetLineOpacity(lineOpacity);
    }

    // 라인의 투명도를 설정하는 메서드
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

    // 라인의 색상을 설정하는 메서드
    public void SetLineColor(Color color)
    {
        if (lineRenderer != null)
        {
            color.a = lineOpacity; // 투명도 유지
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }
    }

    // 라인의 위치를 설정하는 메서드
    public void SetLinePositions(Vector3[] positions)
    {
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = positions.Length;
            lineRenderer.SetPositions(positions);
        }
    }

    // 라인을 활성화/비활성화하는 메서드
    public void SetLineVisibility(bool isVisible)
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = isVisible;
        }
    }
}