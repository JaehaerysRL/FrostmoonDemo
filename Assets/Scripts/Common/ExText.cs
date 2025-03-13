using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(ExText))]
public class ExTextEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("pulseEnable"));
        if (serializedObject.FindProperty("pulseEnable").boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("pulseScaleCurve"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("pulsePosCurve"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("pulseSpeed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("pulsePeriod"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("pulseMoveDistance"));
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("gradientEnable"));
        if (serializedObject.FindProperty("gradientEnable").boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("gradient"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("gradientType"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("gradientAngle"));
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("bendEnable"));
        if (serializedObject.FindProperty("bendEnable").boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("bendCurve"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("bendOffset"));
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("shadowEnable"));
        if (serializedObject.FindProperty("shadowEnable").boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("shadowOffset"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("shadowLevels"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("shadowColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("shadowAlphaDecay"));
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("glowEnable"));
        if (serializedObject.FindProperty("glowEnable").boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("glowRadius"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("glowLayers"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("glowColor"));
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("thicknessEnable"));
        if (serializedObject.FindProperty("thicknessEnable").boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("thickness"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("thicknessColor"));
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("outlineEnable"));
        if (serializedObject.FindProperty("outlineEnable").boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("outlineOffset"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("outlineStrength"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("outlineColor"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif

public class ExText : Text
{
    [Header("动态脉冲")]
    [HideInInspector] public bool pulseEnable = false;
    [HideInInspector] public AnimationCurve pulseScaleCurve;
    [HideInInspector] public AnimationCurve pulsePosCurve;
    [HideInInspector] public float pulseSpeed = 1;
    [HideInInspector] public float pulsePeriod = 2f;
    [HideInInspector] public float pulseMoveDistance = 0;

    [Header("渐变效果")]
    [HideInInspector] public bool gradientEnable = false;
    [HideInInspector] public Gradient gradient = new Gradient();
    [HideInInspector] public GradientType gradientType = GradientType.Horizontal;
    [HideInInspector][Range(0, 360)] public float gradientAngle = 0f;

    [Header("弯曲效果")]
    [HideInInspector] public bool bendEnable = false;
    [HideInInspector] public AnimationCurve bendCurve;
    [HideInInspector] public float bendOffset = 0;

    [Header("阴影效果")]
    [HideInInspector] public bool shadowEnable = false;
    [HideInInspector] public Vector2 shadowOffset = new Vector2(2, -2);
    [HideInInspector] public int shadowLevels = 3;
    [HideInInspector] public Color shadowColor = Color.black;
    [HideInInspector] public float shadowAlphaDecay = 0.5f;

    [Header("发光效果")]
    [HideInInspector] public bool glowEnable = false;
    [HideInInspector] public float glowRadius = 1f;
    [HideInInspector] public int glowLayers = 4;
    [HideInInspector] public Color glowColor = new Color(255, 255, 255, 0.8f);

    [Header("厚度效果")]
    [HideInInspector] public bool thicknessEnable = false;
    [HideInInspector] public float thickness = 0;
    [HideInInspector] public Color thicknessColor = Color.white;

    [Header("描边效果")]
    [HideInInspector] public bool outlineEnable = false;
    [HideInInspector] public float outlineOffset = 2;
    [HideInInspector] public int outlineStrength = 4;
    [HideInInspector] public Color outlineColor = Color.black;

    public enum GradientType { Horizontal, Vertical, Angle, Radial }

    const string LOG_TAG = "ExText";

    private List<UIVertex> vertexCache = new List<UIVertex>();
    private Vector3 originPos;
    private float uivMinX, uivMaxX, uivWidth;

    protected override void Awake()
    {
        base.Awake();
        originPos = transform.localPosition;
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            float time = Time.time;
            if (pulseEnable)
            {
                float pt = Mathf.Repeat(time * pulseSpeed, pulsePeriod) / pulsePeriod;
                float scale = pulseScaleCurve.Evaluate(pt);
                float pos = pulsePosCurve.Evaluate(pt) * pulseMoveDistance;

                transform.localScale = new Vector3(scale, scale, scale);
                transform.localPosition = new Vector3(originPos.x, originPos.y + pos, originPos.z);
            }
        }
    }

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        base.OnPopulateMesh(toFill);
        vertexCache.Clear();
        toFill.GetUIVertexStream(vertexCache);
        toFill.Clear();
        vertexCache = RemoveRedundant(vertexCache);

        if (!vertexCache.Any()) return;

        ApplyEffects(toFill, ref vertexCache);
        AddUIVertexStream(toFill, vertexCache);
    }

    private void ApplyEffects(VertexHelper toFill, ref List<UIVertex> vertices)
    {
        uivMinX = vertices.Min(v => v.position.x);
        uivMaxX = vertices.Max(v => v.position.x);
        uivWidth = uivMaxX - uivMinX;

        if (uivWidth < 0) return;
        // Apply Effect Here
        if (gradientEnable) ApplyGradient(ref vertices);
        if (bendEnable) ApplyBend(ref vertices);
        if (shadowEnable) ApplyShadow(toFill, ref vertices);
        if (glowEnable) ApplyGlow(toFill, vertices);
        if (thicknessEnable) ApplyThickness(ref vertices);
        if (outlineEnable) ApplyOutline(toFill, ref vertices);
    }

    private List<UIVertex> RemoveRedundant(List<UIVertex> list)
    {
        List<UIVertex> l = new List<UIVertex>();
        for (int i = 0, max = list.Count; i < max; i++)
        {
            int remainder = i % 6;
            if (remainder == 3 || remainder == 5)
            {
                continue;
            }
            l.Add(list[i]);
        }
        return l;
    }

    private void AddUIVertexStream(VertexHelper toFill, List<UIVertex> list)
    {
        int exist = toFill.currentVertCount;
        for (int i = 0, max = list.Count / 4; i < max; i++)
        {
            toFill.AddVert(list[i * 4 + 0]);
            toFill.AddVert(list[i * 4 + 1]);
            toFill.AddVert(list[i * 4 + 2]);
            toFill.AddVert(list[i * 4 + 3]);
            toFill.AddTriangle(i * 4 + 0 + exist, i * 4 + 1 + exist, i * 4 + 2 + exist);
            toFill.AddTriangle(i * 4 + 2 + exist, i * 4 + 3 + exist, i * 4 + 0 + exist);
        }
    }

    private void ApplyGradient(ref List<UIVertex> vertices)
    {
        // 计算文本的包围盒范围
        float minX = vertices.Min(v => v.position.x);
        float maxX = vertices.Max(v => v.position.x);
        float minY = vertices.Min(v => v.position.y);
        float maxY = vertices.Max(v => v.position.y);
        Vector2 center = new Vector2((minX + maxX) * 0.5f, (minY + maxY) * 0.5f);
        float width = maxX - minX;
        float height = maxY - minY;

        for (int i = 0; i < vertices.Count; i++)
        {
            UIVertex v = vertices[i];
            Vector2 pos = v.position;
            float t = 0f;

            switch (gradientType)
            {
                case GradientType.Horizontal:
                    t = width == 0 ? 0 : (pos.x - minX) / width;
                    break;
                case GradientType.Vertical:
                    t = height == 0 ? 0 : (pos.y - minY) / height;
                    break;
                case GradientType.Angle:
                    Vector2 dir = pos - center;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    angle = (angle + gradientAngle + 360) % 360;
                    t = angle / 360f;
                    break;
                case GradientType.Radial:
                    Vector2 delta = pos - center;
                    float maxDist = new Vector2(width * 0.5f, height * 0.5f).magnitude;
                    t = maxDist == 0 ? 0 : delta.magnitude / maxDist;
                    break;
            }

            // 应用渐变颜色（保留原始透明度）
            Color color = gradient.Evaluate(t);
            color.a *= v.color.a; // 保持原有Alpha
            v.color = color;
            vertices[i] = v;
        }
    }

    private void ApplyBend(ref List<UIVertex> vertices)
    {
        int totalVertices = vertices.Count;
        float segmentSize = 1f / totalVertices;
        float deltaX = segmentSize * uivWidth;
        float invUIVWidth = 1f / uivWidth;

        for (int i = 0, quadCount = totalVertices >> 2; i < quadCount; i++)
        {
            int baseIdx = i << 2;
            UIVertex v0 = vertices[baseIdx];
            UIVertex v1 = vertices[baseIdx + 1];
            UIVertex v2 = vertices[baseIdx + 2];
            UIVertex v3 = vertices[baseIdx + 3];

            // Process left edge (v0-v3)
            Vector2 midLeft = (v0.position + v3.position) * 0.5f;
            float verticalSpan = (v0.position.y - v3.position.y) * 0.5f;
            float t = (midLeft.x - uivMinX) * invUIVWidth;

            float curveValue = bendCurve.Evaluate(t);
            float nextCurveValue = bendCurve.Evaluate(t + segmentSize);
            midLeft.y += curveValue * bendOffset;

            Vector2 tangent = new Vector2(deltaX, (nextCurveValue - curveValue) * bendOffset);
            Vector2 normal = new Vector2(-tangent.y, tangent.x).normalized;

            v0.position = midLeft + normal * verticalSpan;
            v3.position = midLeft - normal * verticalSpan;

            // Process right edge (v1-v2)
            Vector2 midRight = (v1.position + v2.position) * 0.5f;
            verticalSpan = (v1.position.y - v2.position.y) * 0.5f;
            t = (midRight.x - uivMinX) * invUIVWidth;

            curveValue = bendCurve.Evaluate(t);
            float prevCurveValue = bendCurve.Evaluate(t - segmentSize);
            midRight.y += curveValue * bendOffset;

            tangent = new Vector2(deltaX, (curveValue - prevCurveValue) * bendOffset);
            normal = new Vector2(-tangent.y, tangent.x).normalized;

            v1.position = midRight + normal * verticalSpan;
            v2.position = midRight - normal * verticalSpan;

            vertices[baseIdx] = v0;
            vertices[baseIdx + 1] = v1;
            vertices[baseIdx + 2] = v2;
            vertices[baseIdx + 3] = v3;
        }
    }

    private void ApplyShadow(VertexHelper toFill, ref List<UIVertex> vertices)
    {
        if (shadowLevels <= 0 || shadowAlphaDecay <= 0) return;

        List<UIVertex> shadowVertices = new List<UIVertex>();
        Color currentColor = shadowColor;

        for (int level = 0; level < shadowLevels; level++)
        {
            currentColor.a = shadowColor.a * Mathf.Pow(shadowAlphaDecay, level);
            Vector2 offset = shadowOffset * (level + 1);

            foreach (UIVertex v in vertices)
            {
                UIVertex vt = v;
                vt.position += (Vector3)offset;
                vt.color = (Color)currentColor * vt.color; // 混合原有顶点颜色
                shadowVertices.Add(vt);
            }
        }

        AddUIVertexStream(toFill, shadowVertices);
    }

    private void ApplyGlow(VertexHelper toFill, List<UIVertex> vertices)
    {
        if (glowRadius <= 0 || glowLayers <= 0) return;

        const int DIRECTIONS = 8;
        List<UIVertex> glowVertices = new List<UIVertex>();

        for (int layer = 0; layer < glowLayers; layer++)
        {
            float radius = glowRadius * (layer + 1) / glowLayers;
            Color color = glowColor;
            color.a *= 1 - (float)layer / glowLayers;

            for (int d = 0; d < DIRECTIONS; d++)
            {
                float angle = 2 * Mathf.PI * d / DIRECTIONS;
                Vector2 offset = new Vector2(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius
                );

                foreach (UIVertex v in vertices)
                {
                    UIVertex vt = v;
                    vt.position += (Vector3)offset;
                    vt.color = color;
                    glowVertices.Add(vt);
                }
            }
        }

        AddUIVertexStream(toFill, glowVertices);
    }

    private void ApplyThickness(ref List<UIVertex> vertices)
    {
        List<UIVertex> thick = new List<UIVertex>(vertices);
        Vector3 offset = new Vector3(0, thickness, 0);
        for (int i = 0, max = thick.Count; i < max; i++)
        {
            UIVertex v = thick[i];
            v.position += offset;
            v.color = thicknessColor;
            thick[i] = v;
        }
        vertices.InsertRange(0, thick);
    }

    private void ApplyOutline(VertexHelper toFill, ref List<UIVertex> vertices)
    {
        if (outlineStrength <= 0) return;

        List<UIVertex> outline = new List<UIVertex>();
        List<Vector3> offset = new List<Vector3>();
        for (int i = 0; i < outlineStrength; i++)
        {
            offset.Add(new Vector3(
                Mathf.Sin(2 * Mathf.PI / outlineStrength * i) * outlineOffset,
                Mathf.Cos(2 * Mathf.PI / outlineStrength * i) * outlineOffset,
                0));
        }
        for (int i = 0; i < outlineStrength; i++)
        {
            for (int j = 0, max = vertices.Count; j < max; j++)
            {
                UIVertex v = vertices[j];
                v.position += offset[i];
                v.color = outlineColor;
                outline.Add(v);
            }
        }

        AddUIVertexStream(toFill, outline);
    }
}

