using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 记录窗体位置及大小
/// </summary>
public static class WindowPostion
{
    public static int PosX { get; set; } = 400;

    public static int PosY { get; set; } = 300;

    public static int Width { get; set; } = Screen.width;

    public static int Height { get; set; } = Screen.height;
}
