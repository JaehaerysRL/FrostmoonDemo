using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class Utils
{
    #region 坐标转换
    public const float FEET_TO_UNITS = 60f; // DND的尺寸转换为Unity单位的比例

    // 将世界坐标转换为网格坐标
    public static int2 WorldToGrid(Vector3 worldPos)
    {
        return new int2(
            Mathf.FloorToInt(worldPos.x / FEET_TO_UNITS),
            Mathf.FloorToInt(worldPos.y / FEET_TO_UNITS)
        );
    }

    // 将网格坐标转换为世界坐标
    public static Vector3 GridToWorld(int2 gridPos, float height = 0)
    {
        return new Vector3(
            gridPos.x * FEET_TO_UNITS,
            gridPos.y * FEET_TO_UNITS,
            height
        );

    }
    #endregion
}