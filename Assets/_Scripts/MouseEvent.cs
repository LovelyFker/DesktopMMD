using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Xml;
using System.IO;
using System.Reflection;

/// <summary>
/// 鼠标控制脚本，需与MMD模型控制脚本放在同一级object下
/// </summary>
[RequireComponent(typeof(Collider))]
public class MouseEvent : MonoBehaviour
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    [DllImport("user32.dll")]
    static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);
    [DllImport("user32.dll")]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    [DllImport("user32.dll")]
    static extern bool GetCursorPos(out POINT lpPoint);

    public Animator animator;

    private GameObject UIButton;
    //舞蹈动画数量
    private int animatorClipCount;
    /*public AudioSource audioSource;
    public AnimationClip[] animClips;
    public AudioClip[] audioClips;
    private string xmlPath;
    private string bgmName;*/
    /// <summary>
    /// MMD模型控制脚本
    /// </summary>
    private MMD4MecanimModel mmdModelScript;
    /// <summary>
    /// 定义程序名字符串获取窗口句柄
    /// </summary>
    private string strProduct;
    private int RandomNum;
    //判断舞蹈动画是否播放
    private bool IsPlayAnim = false;
    //判断模型位置是否已经重置
    public bool Reset { get; set; } = false;
    //判断是否继续播放动画
    public bool GoOnPlaying { get; private set; } = false;
    //记录模型初始位置旋转角
    private Vector3 defaultTransformPosition;
    private Quaternion defaultTransformRotation;
    //判断鼠标是否位于碰撞器内
    private bool IsMouseInCollider = false;
    //判断鼠标是否拖拽
    private bool IsDrag = false;
    //临时记录窗体位置，用于限制窗体于屏幕内
    private int widthTemp;
    private int heightTemp;

    private const int SWP_SHOWWINDOW = 0x0040;

    private IntPtr hwnd;
    private int MouseDrag_X;
    private int MouseDrag_Y;
    private int Window_X = 400;
    private int Window_Y = 300;
    private int Window_Width;
    private int Window_Height;
    private POINT MousePoint;

    private void OnEnable()
    {
        Window_X = WindowPostion.PosX;
        Window_Y = WindowPostion.PosY;
        Window_Width = WindowPostion.Width;
        Window_Height = WindowPostion.Height;

        mmdModelScript = this.transform.GetComponent<MMD4MecanimModel>();
        UIButton = GameObject.FindGameObjectWithTag("UIButton");
        //初始化audioSource
        mmdModelScript.audioSource = this.transform.parent.GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        strProduct = Application.productName;

#if !UNITY_EDITOR
        //获取程序窗体句柄
        hwnd = FindWindow(null, strProduct);
        //设置窗口默认位置
        SetWindowPos(hwnd, -1, Window_X, Window_Y, Window_Width, Window_Height, SWP_SHOWWINDOW);
#endif

        //xmlPath = Application.dataPath + "/Xml";

        //获取animator中animationClip的数量
        animatorClipCount = animator.runtimeAnimatorController.animationClips.Length;
        //记录模型初始位置及旋转角
        defaultTransformPosition = this.transform.position;
        defaultTransformRotation = this.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        GetCursorPos(out MousePoint);

        if (Reset)
        {
            //模型位置重置的平滑化
            this.transform.position = Vector3.Slerp(this.transform.position, defaultTransformPosition, 0.1f);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, defaultTransformRotation, 0.1f);
        }

        WindowPostion.Width = Window_Width;
        WindowPostion.Height = Window_Height;

#if !UNITY_EDITOR

        if (LeapMotionGesturesManager.Instance.leftHandGesture == LeapMotionGesturesManager.Gesture.Grab
            && LeapMotionGesturesManager.Instance.rightHandGesture == LeapMotionGesturesManager.Gesture.Grab)
        {
            LeapMotionWindowSet(ref Window_Width, ref Window_Height);

            //屏幕边缘放大有可能超出屏幕范围，放大后调整位置
            Window_X = Window_X + Window_Width > Screen.currentResolution.width ? Screen.currentResolution.width - Window_Width : Window_X;
            Window_Y = Window_Y + Window_Height > Screen.currentResolution.height ? Screen.currentResolution.height - Window_Height : Window_Y;

            //记录当前模型角色的窗体位置及缩放大小
            WindowPostion.PosX = Window_X;
            WindowPostion.PosY = Window_Y;
            WindowPostion.Width = Window_Width;
            WindowPostion.Height = Window_Height;

            SetWindowPos(hwnd, -1, Window_X, Window_Y, Window_Width, Window_Height, SWP_SHOWWINDOW);
        }

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            ZoomUp(ref Window_Width, ref Window_Height);
            //屏幕边缘放大有可能超出屏幕范围，放大后调整位置
            Window_X = Window_X + Window_Width > Screen.currentResolution.width ? Screen.currentResolution.width - Window_Width : Window_X;
            Window_Y = Window_Y + Window_Height > Screen.currentResolution.height ? Screen.currentResolution.height - Window_Height : Window_Y;

            //记录当前模型角色的窗体位置及缩放大小
            WindowPostion.PosX = Window_X;
            WindowPostion.PosY = Window_Y;
            WindowPostion.Width = Window_Width;
            WindowPostion.Height = Window_Height;

            SetWindowPos(hwnd, -1, Window_X, Window_Y, Window_Width, Window_Height, SWP_SHOWWINDOW);
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {   
            //窗体缩小不会改变窗体位置，因此不需要记录（根据左上角缩放，左上角的点即为窗体位置）
            ZoomIn(ref Window_Width, ref Window_Height);
            SetWindowPos(hwnd, -1, Window_X, Window_Y, Window_Width, Window_Height, SWP_SHOWWINDOW);
        }

        if (IsDrag)
        {
            //临时记录窗体位置、判断是否拖出屏幕外、并调整到屏幕内
            widthTemp = Window_X + MousePoint.X - MouseDrag_X;
            heightTemp = Window_Y + MousePoint.Y - MouseDrag_Y;

            widthTemp = widthTemp + Window_Width > Screen.currentResolution.width ? Screen.currentResolution.width - Window_Width : widthTemp;
            heightTemp = heightTemp + Window_Height > Screen.currentResolution.height ? Screen.currentResolution.height - Window_Height : heightTemp;

            widthTemp = widthTemp < 0 ? 0 : widthTemp;
            heightTemp = heightTemp < 0 ? 0 : heightTemp;

            //记录当前模型角色的缩放大小
            WindowPostion.Width = Window_Width;
            WindowPostion.Height = Window_Height;

            SetWindowPos(hwnd, -1, 
                widthTemp, 
                heightTemp, 
                Window_Width, Window_Height, SWP_SHOWWINDOW);
        }
#endif

        #region 弃用的匹配动画与bgm代码
        /*if (IsMouseInCollider)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("鼠标右键按下~");
                IsPlayAnim = !IsPlayAnim;

                if (IsPlayAnim)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(xmlPath + "/MMD.xml");
                    XmlNodeList xmlNodeList = xmlDoc.SelectNodes("Animation/Name");

                    //当前Range(1, 5)，取值为[1, 2, 3, 4]
                    RandomNum = UnityEngine.Random.Range(1, animClips.Length);
                    bgmName = null;

                    foreach (XmlNode node in xmlNodeList)
                    {
                        
                        if (node.InnerText == animClips[RandomNum - 1].name)
                        {
                            bgmName = node.Attributes.Item(0).Value;

                            //动画名称异常检测
                            try
                            {
                                tagNum = int.Parse(node.Attributes.Item(1).Value);
                            }
                            catch (Exception e)
                            {
                                Debug.Log(e);
                                exceptionFlag = true;
                            }
                            finally
                            {
                                if (exceptionFlag)
                                {
                                    tagNum = 1;
                                    exceptionFlag = false;
                                }
                            }
                            
                            animator.SetInteger("Random", tagNum);
                            break;
                        }
                    }

                    Debug.Log(bgmName);
                    foreach (AudioClip clip in audioClips)
                    {
                        *//*if (clip.name.Equals(bgmName))
                        {
                            audioSource.clip = clip;
                            audioSource.Play();
                            break;
                        }*//*
                    }
                }
                else
                {
                    animator.SetTrigger("ReturnIdle");
                    animator.SetInteger("Random", 0);
                    audioSource.Stop();

                    //修正动画改变的模型位置
                    *//*this.transform.GetChild(0).position = defaultTransformPosition;
                    this.transform.GetChild(0).rotation = defaultTransformRotation;*//*
                }
            }
        }*/
        #endregion

        if (IsMouseInCollider)
        {
            if (Input.GetMouseButtonDown(1) && !Reset)
            {
                if (!IsPlayAnim)
                {
                    IsPlayAnim = true;
                    GoOnPlaying = true;
                    RandomNum = UnityEngine.Random.Range(1, animatorClipCount);
                    animator.SetInteger("Random", RandomNum);
                }
                else
                {
                    //WaitForReset = true;
                    GoOnPlaying = false;
                    animator.SetTrigger("ReturnIdle");
                  //  animator.SetInteger("Random", 0);

                  //  StartCoroutine("ResetModel");
                }
            }
        }

        if (IsPlayAnim)
        {
            UIButton.SetActive(false);
        }
        else
        {
            UIButton.SetActive(true);
        }
    }

    IEnumerator ResetModel()
    {
        ResetMMDModelMorph();
        IsPlayAnim = false;
        yield return new WaitForSeconds(0.8f);
        Reset = false;
    }

    IEnumerator ResetModelGoOnPlaying()
    {
        ResetMMDModelMorph();
        yield return new WaitForSeconds(0.8f);
        animator.SetInteger("Random", UnityEngine.Random.Range(1, animatorClipCount));
        Reset = false;
    }

    /// <summary>
    /// 重置MMD模型表情动画
    /// </summary>
    private void ResetMMDModelMorph()
    {
        foreach (MMD4MecanimModelImpl.Morph morph in mmdModelScript.morphList)
        {
            morph._animWeight = 0;
        }
    }

    private void OnMouseDrag()
    {
        if (!IsDrag) {
            MouseDrag_X = MousePoint.X;
            MouseDrag_Y = MousePoint.Y;
        }
        IsDrag = true;
    }

    private void OnMouseUp()
    {
        if (IsDrag)
        {
            IsDrag = false;
            //记录鼠标拖拽位移、保存窗体位置
            Window_X += MousePoint.X - MouseDrag_X;
            Window_Y += MousePoint.Y - MouseDrag_Y;

            Window_X = Window_X + Window_Width > Screen.currentResolution.width ? Screen.currentResolution.width - Window_Width : Window_X;
            Window_Y = Window_Y + Window_Height > Screen.currentResolution.height ? Screen.currentResolution.height - Window_Height : Window_Y;

            Window_X = Window_X < 0 ? 0 : Window_X;
            Window_Y = Window_Y < 0 ? 0 : Window_Y;

            //记录当前角色模型的窗体位置
            WindowPostion.PosX = Window_X;
            WindowPostion.PosY = Window_Y;
        }
    }

    private void OnMouseEnter()
    {
        IsMouseInCollider = true;
    }

    private void OnMouseExit()
    {
        IsMouseInCollider = false;
    }

    //放大窗体数据记录
    private void ZoomUp(ref int width, ref int height)
    {
        if (width < 500 + 50 * 4 && height < 550 + 55 * 4)
        {
            width += 50;
            height += 55;
        }
    }

    //缩小窗体数据记录
    private void ZoomIn(ref int width, ref int height)
    {
        if (width > 500 && height > 550)
        {
            width -= 50;
            height -= 55;
        }
    }

    /// <summary>
    /// LeapMotion手势调整窗体大小数据记录
    /// </summary>
    private void LeapMotionWindowSet(ref int width, ref int height)
    {
        float w = (float)width;
        float h = (float)height;
        w *= LeapMotionGesturesManager.Instance.distanceRatio;
        h *= LeapMotionGesturesManager.Instance.distanceRatio;

        if (w > 700)
            width = 700;
        else if (w < 500)
            width = 500;
        else
            width = (int)w;

        if (h > 770)
            height = 770;
        else if (h < 550)
            height = 550;
        else
            height = (int)h;
    }

    private void OnGUI()
    {
        //GUI.Label(new Rect(0, 0, 100, 200), "Mouse Drag " + IsDrag.ToString() + "~");
    }
}
