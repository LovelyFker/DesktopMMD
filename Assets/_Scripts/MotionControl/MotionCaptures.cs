using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动作捕获
/// </summary>
public static class MotionCaptures
{
    private delegate void MotionCaptureHandler();
    private static event MotionCaptureHandler MotionCaptureEventLog;

    public enum MotionType
    {
        Idle = 0,
        Walk = 1,
        Run = 2,
        Jump = 3
    }

    public static void MotionCaptureEventLogFire()
    {
        MotionCaptureEventLog += SendCaptureMSG;
        MotionCaptureEventLog += PrintCaptureMSG;
        MotionCaptureEventLog();
    }

    public static MotionType GetMotionType()
    {
        return MotionType.Idle;
    }

    private static void SendCaptureMSG()
    {
        Debug.Log("Send capturing message!");
    }

    private static void PrintCaptureMSG()
    {
        Debug.Log("Has captured!");
    }
}
