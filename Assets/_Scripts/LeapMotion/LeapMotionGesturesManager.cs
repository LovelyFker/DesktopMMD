using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using Leap;
using System.Runtime.InteropServices;

public sealed class LeapMotionGesturesManager : MonoBehaviour
{
    public static LeapMotionGesturesManager Instance
    {
        get
        {
            if (mInstance == null)
            {
                GameObject root = new GameObject("_LeapMotionGesturesManager");
                mInstance = root.AddComponent<LeapMotionGesturesManager>();
            }
            return mInstance;
        }
    }
    private static LeapMotionGesturesManager mInstance;

    public HandModelBase leftHandModel;
    public HandModelBase rightHandModel;

    private LeapServiceProvider mProvider;
    private Frame mFrame;
    private Hand mHand;
    /// <summary>
    /// 抓取时双手初始距离
    /// </summary>
    private float defaultDistance = 0f;
    /// <summary>
    /// 判断双手抓取状态
    /// </summary>
    private bool IsGrab = false;
    //测试信息输出
    private string TestInfo;

    public Gesture leftHandGesture { get; private set; }
    public Gesture rightHandGesture { get; private set; }
    /// <summary>
    /// 距离比率，根据抓取手势距离比率调整窗口大小
    /// </summary>
    public float distanceRatio { get; private set; }

    public enum Gesture
    {
        /// <summary>
        /// 检测到手部（普通状态）
        /// </summary>
        Hand = 0,
        /// <summary>
        /// 握拳
        /// </summary>
        Fist = 1,
        /// <summary>
        /// 抓取状态
        /// </summary>
        Grab = 2
    }

    private void Start()
    {
        //初始化
        mProvider = FindObjectOfType<LeapServiceProvider>() as LeapServiceProvider;
        distanceRatio = 1f;
    }

    private void Update()
    {
        mFrame = mProvider.CurrentFrame;

        switch (mFrame.Hands.Count)
        {
            case 0:
                TestInfo = "没有检测到双手!";
                break;
            case 1:
                TestInfo = "检测到一只手!";
                break;
            case 2:
                TestInfo = "检测到双手!";
                break;
            default:
                TestInfo = "默认";
                break;
        }

        if (mFrame.Hands.Count == 2) {
            leftHandGesture = Gesture.Hand;
            rightHandGesture = Gesture.Hand;
            if (IsGrabState(mFrame.Hands[0]))
            {
                if (mFrame.Hands[0].IsLeft)
                    leftHandGesture = Gesture.Grab;
                if (mFrame.Hands[0].IsRight)
                    rightHandGesture = Gesture.Grab;
            }

            if (IsGrabState(mFrame.Hands[1]))
            {
                if (mFrame.Hands[1].IsLeft)
                    leftHandGesture = Gesture.Grab;
                if (mFrame.Hands[1].IsRight)
                    rightHandGesture = Gesture.Grab;
            }
        }
        else
        {
            leftHandGesture = Gesture.Hand;
            rightHandGesture = Gesture.Hand;
        }

        if (leftHandGesture == Gesture.Grab && rightHandGesture == Gesture.Grab)
        {
            if (!IsGrab)
            {
                defaultDistance = HandsDistance(mFrame);
                IsGrab = true;
            }
            else
            {
                distanceRatio = Mathf.Pow(HandsDistance(mFrame) / defaultDistance, 1.0f / 4.0f);
                defaultDistance = HandsDistance(mFrame);
            }
        }
        else
        {
            distanceRatio = 1f;
            IsGrab = false;
        }
    }

    /// <summary>
    /// 双手距离
    /// </summary>
    private float HandsDistance(Frame frame)
    {
        if (frame.Hands.Count == 2)
        {
            return Vector3.Distance(new Vector3(frame.Hands[0].PalmPosition.x, frame.Hands[0].PalmPosition.y, frame.Hands[0].PalmPosition.z),
                                    new Vector3(frame.Hands[1].PalmPosition.x, frame.Hands[1].PalmPosition.y, frame.Hands[1].PalmPosition.z));
        }
        else
        {
            return 0f;
        }
    }

    /// <summary>
    /// 判断抓取手势
    /// </summary>
    /// <param name="hand">手</param>
    /// <returns>抓取状态</returns>
    private bool IsGrabState (Hand hand)
    {
        if (hand.GrabStrength > 0.9f)
            return true;
        else
            return false;
    }

    private void OnGUI()
    {
        GUIStyle mStyle = new GUIStyle();
        mStyle.fontSize = 29;
        mStyle.fontStyle = FontStyle.Bold;
        mStyle.normal.textColor = Color.green;
        //GUI.Label(new Rect(0, 50, 200, 200), TestInfo, mStyle);
    }
}
