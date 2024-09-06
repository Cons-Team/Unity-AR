using System.Collections.Generic;
using UnityEditor;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/Effects/DropShadow", 14)]
    public class DropShadow : BaseMeshEffect
    {
        [SerializeField] private Color shadowColor = new Color(0f, 0f, 0f, 0.5f); // �׸��� ����
        [SerializeField] private Vector2 shadowDistance = new Vector2(1f, -1f); // �׸��� �Ÿ�
        [SerializeField] private bool m_UseGraphicAlpha = true; // �׷����� ���� ���� ������� ����
        public int iterations = 5; // �׸��� �ݺ� Ƚ��
        public Vector2 shadowSpread = Vector2.one; // �׸����� Ȯ�� ����

        protected DropShadow() { }

#if UNITY_EDITOR
        // �����Ϳ��� ���� ����� �� ȣ��Ǵ� �޼���
        protected override void OnValidate()
        {
            EffectDistance = shadowDistance;
            base.OnValidate();
        }
#endif

        // �׸��� ���� ���� �� ��������
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

        // �׸��� Ȯ�� ���� ���� �� ��������
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

        // �׸��� �ݺ� Ƚ�� ���� �� ��������
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

        // �׸��� �Ÿ� ���� �� ��������
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

        // �׷����� ���� �� ��� ���� ���� �� ��������
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

        // �׸��� ȿ���� �����ϴ� �޼���
        private void DropShadowEffect(List<UIVertex> verts)
        {
            UIVertex vt;
            int count = verts.Count;
            List<UIVertex> vertsCopy = new List<UIVertex>(verts);
            verts.Clear();

            // ������ �ݺ� Ƚ����ŭ �׸��� ����
            for (int i = 0; i < iterations; i++)
            {
                float factor = (float)i / iterations;
                foreach (UIVertex original in vertsCopy)
                {
                    vt = original;
                    Vector3 position = vt.position;

                    // �׸��� Ȯ�� �� ��ġ ����
                    position.x *= (1 + shadowSpread.x * factor * 0.01f);
                    position.y *= (1 + shadowSpread.y * factor * 0.01f);
                    position.x += shadowDistance.x * factor;
                    position.y += shadowDistance.y * factor;
                    vt.position = position;

                    // �׸��� ���� ����
                    Color32 color = shadowColor;
                    color.a = (byte)(color.a / iterations);
                    vt.color = color;

                    verts.Add(vt);
                }
            }

            // ���� ������ �߰��Ͽ� ���� ��Ҹ� ����
            verts.AddRange(vertsCopy);
        }

        // �޽� ���� �޼���
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