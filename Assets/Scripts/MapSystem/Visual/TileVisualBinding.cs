using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class TileVisualBinding : MonoBehaviour
{
    public Image image; // UI图像，Shader挂载在这里
    [HideInInspector] public MaterialPropertyBlock block;

    void Awake()
    {
        if (image == null) image = GetComponent<Image>();
        block = new MaterialPropertyBlock();
    }

    public void ApplyVisual(float snowBlend, float heightOffset)
    {
        if (image != null && image.material != null)
        {
            image.material.SetFloat("_SnowBlend", snowBlend);
            image.material.SetFloat("_HeightOffset", heightOffset);
        }
    }
}
