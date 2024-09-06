using UnityEngine;
using UnityEngine.UI;

public class LineRendererController : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer; // LineRenderer 컴포넌트
    [Range(0, 1)][SerializeField] private float lineOpacity = 1.0f; // 초기 투명도 (0: 완전히 투명, 1: 완전히 불투명)
    [SerializeField] private Slider opacitySlider; // 투명도 조절을 위한 슬라이더

    private void Start()
    {
        // LineRenderer가 할당되지 않은 경우, 현재 GameObject에서 가져오기
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        // 슬라이더가 설정되어 있으면 초기화 및 리스너 추가
        if (opacitySlider != null)
        {
            opacitySlider.value = lineOpacity; // 슬라이더 초기값 설정
            opacitySlider.onValueChanged.AddListener(SetLineOpacity); // 슬라이더 값 변경 시 호출될 메서드 등록
        }

        // 초기 투명도 설정
        SetLineOpacity(lineOpacity);
    }

    // LineRenderer의 투명도를 설정하는 메서드
    public void SetLineOpacity(float opacity)
    {
        lineOpacity = opacity;

        if (lineRenderer != null && lineRenderer.material != null)
        {
            Color materialColor = lineRenderer.material.color;
            materialColor.a = opacity; // 알파 값을 변경하여 투명도 조절
            lineRenderer.material.color = materialColor;
        }
    }
}