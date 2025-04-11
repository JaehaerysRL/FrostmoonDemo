using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class Utils
{
    #region 坐标转换
    // 将世界坐标转换为网格坐标
    public static int2 WorldToGrid(Vector3 worldPos)
    {
        return new int2(
            Mathf.FloorToInt(worldPos.x / GlobalConst.GridUnitSize),
            Mathf.FloorToInt(worldPos.y / GlobalConst.GridUnitSize)
        );
    }

    public static int2 UIToGrid(Vector2 worldPos)
    {
        return new int2(
            Mathf.FloorToInt(worldPos.x / GlobalConst.GridPixelSize),
            Mathf.FloorToInt(worldPos.y / GlobalConst.GridPixelSize)
        );
    }

    // 将网格坐标转换为世界坐标
    public static Vector3 GridToWorld(int2 gridPos, float height = 0)
    {
        return new Vector3(
            gridPos.x * GlobalConst.GridUnitSize,
            gridPos.y * GlobalConst.GridUnitSize,
            height
        );

    }

    public static Vector2 GridToUI(int2 gridPos)
    {
        return new Vector2(
            gridPos.x * GlobalConst.GridPixelSize,
            gridPos.y * GlobalConst.GridPixelSize
        );
    }

    #endregion
}