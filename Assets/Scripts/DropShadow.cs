using System.Collections.Generic;
using UnityEditor;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/Effects/DropShadow", 14)]
    public class DropShadow : BaseMeshEffect
    {
        [SerializeField] private Color shadowColor = new Color(0f, 0f, 0f, 0.5f); // 그림자 색상
        [SerializeField] private Vector2 shadowDistance = new Vector2(1f, -1f); // 그림자 거리
        [SerializeField] private bool m_UseGraphicAlpha = true; // 그래픽의 알파 값을 사용할지 여부
        public int iterations = 5; // 그림자 반복 횟수
        public Vector2 shadowSpread = Vector2.one; // 그림자의 확산 정도

        protected DropShadow() { }

#if UNITY_EDITOR
        // 에디터에서 값이 변경될 때 호출되는 메서드
        protected override void OnValidate()
        {
            EffectDistance = shadowDistance;
            base.OnValidate();
        }
#endif

        // 그림자 색상 설정 및 가져오기
        public Color effectColor
        {
            get => shadowColor;
            set
            {
                shadowColor = value;
                if (graphic != null)
                    graphic.SetVerticesDirty();
            }
        }

        // 그림자 확산 정도 설정 및 가져오기
        public Vector2 ShadowSpread
        {
            get => shadowSpread;
            set
            {
                shadowSpread = value;
                if (graphic != null)
                    graphic.SetVerticesDirty();
            }
        }

        // 그림자 반복 횟수 설정 및 가져오기
        public int Iterations
        {
            get => iterations;
            set
            {
                iterations = value;
                if (graphic != null)
                    graphic.SetVerticesDirty();
            }
        }

        // 그림자 거리 설정 및 가져오기
        public Vector2 EffectDistance
        {
            get => shadowDistance;
            set
            {
                shadowDistance = value;
                if (graphic != null)
                    graphic.SetVerticesDirty();
            }
        }

        // 그래픽의 알파 값 사용 여부 설정 및 가져오기
        public bool useGraphicAlpha
        {
            get => m_UseGraphicAlpha;
            set
            {
                m_UseGraphicAlpha = value;
                if (graphic != null)
                    graphic.SetVerticesDirty();
            }
        }

        // 그림자 효과를 적용하는 메서드
        private void DropShadowEffect(List<UIVertex> verts)
        {
            UIVertex vt;
            int count = verts.Count;
            List<UIVertex> vertsCopy = new List<UIVertex>(verts);
            verts.Clear();

            // 지정된 반복 횟수만큼 그림자 적용
            for (int i = 0; i < iterations; i++)
            {
                float factor = (float)i / iterations;
                foreach (UIVertex original in vertsCopy)
                {
                    vt = original;
                    Vector3 position = vt.position;

                    // 그림자 확산 및 위치 변경
                    position.x *= (1 + shadowSpread.x * factor * 0.01f);
                    position.y *= (1 + shadowSpread.y * factor * 0.01f);
                    position.x += shadowDistance.x * factor;
                    position.y += shadowDistance.y * factor;
                    vt.position = position;

                    // 그림자 색상 설정
                    Color32 color = shadowColor;
                    color.a = (byte)(color.a / iterations);
                    vt.color = color;

                    verts.Add(vt);
                }
            }

            // 원본 정점을 추가하여 본래 요소를 유지
            verts.AddRange(vertsCopy);
        }

        // 메쉬 수정 메서드
        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
                return;

            List<UIVertex> output = new List<UIVertex>();
            vh.GetUIVertexStream(output);

            DropShadowEffect(output);

            vh.Clear();
            vh.AddUIVertexTriangleStream(output);
        }
    }
}