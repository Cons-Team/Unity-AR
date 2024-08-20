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
        if (lineRenderer != null && lineRenderer.material != null)
        {
            Color materialColor = lineRenderer.material.color;
            materialColor.a = opacity; // 알파 채널을 사용하여 투명도 조절
            lineRenderer.material.color = materialColor;
        }
    }
}