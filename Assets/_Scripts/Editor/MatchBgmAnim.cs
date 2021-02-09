using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Xml;
using System.IO;
using UnityEditorInternal;

public class MatchBgmAnim : EditorWindow
{
    AnimationClip m_animationClip;
    AudioClip m_audioClip;
    string tag;
    private static string xmlPath;
    private string tagNum = "1";
    private bool positveInt = false;

    #region 测试变量
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;
    #endregion

    private void OnEnable()
    {
        xmlPath = Application.dataPath + "/Xml";
        if (!Directory.Exists(xmlPath))
            Directory.CreateDirectory(xmlPath);
    }

    [MenuItem("Custom/MatchBgmAnim")]
    private static void Init()
    {
        MatchBgmAnim window = (MatchBgmAnim)GetWindow(typeof(MatchBgmAnim));
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 300);
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("动画clip", EditorStyles.label);
        m_animationClip = (AnimationClip)EditorGUILayout.ObjectField(m_animationClip, typeof(AnimationClip), true);
        GUILayout.Label("音乐bgm", EditorStyles.label);
        m_audioClip = (AudioClip)EditorGUILayout.ObjectField(m_audioClip, typeof(AudioClip), true);
        tagNum = EditorGUILayout.TextField("标签号", tagNum);


        groupEnabled = EditorGUILayout.BeginToggleGroup("Test data no useful", groupEnabled);
        myBool = EditorGUILayout.Toggle("Toggle", myBool);
        myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup();

        if (GUILayout.Button("View"))
        {
            Debug.Log("查看所有关联动画数据~");
            EditorWindow DataWindow = (ViewAnimData)GetWindow(typeof(ViewAnimData));
            DataWindow.position = new Rect(Screen.width / 2 + 200, Screen.height / 2 + 50, 250, 400);
            DataWindow.titleContent = new GUIContent("关联动画数据");
            DataWindow.Show();
        }

        if (GUILayout.Button("Apply"))
        {
            try
            {
                string anim_name = m_animationClip.name;
                string audio_name = m_audioClip.name;

                //监测标签号，必须为整数，否则抛出异常
                try
                {
                    int i = int.Parse(tagNum);
                    positveInt = true;
                }
                catch
                {
                    positveInt = false;
                    this.ShowNotification(new GUIContent("标签号必须为整数!!"));
                }

                if ((anim_name != null) && (audio_name != null) && (tagNum != null) && positveInt)
                {
                    try
                    {
                        WriteXML(anim_name, audio_name, tagNum);
                    }
                    
                    catch
                    {
                        this.ShowNotification(new GUIContent("XML文件写入错误!"));
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
                this.ShowNotification(new GUIContent("动画clip、音乐bgm、标签号不能为空!"));
                //WinDllImport.MessageBox(IntPtr.Zero, "动画clip与音乐bgm不能为空!", "错误信息", 0);
            }
        }
    }

    private void WriteXML(string anim, string bgm, string tag)
    {
        InitXML();

        if (File.Exists(xmlPath + "/MMD.xml"))
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath + "/MMD.xml");

            XmlNodeList xmlNodeList = xmlDoc.SelectNodes("Animation/Name");
            foreach (XmlNode node in xmlNodeList)
            {
                if (node.InnerText == anim)
                {
                    Debug.Log("已存在该动画结点~");
                    node.Attributes.Item(0).Value = bgm;
                    node.Attributes.Item(1).Value = tag;
                    xmlDoc.Save(xmlPath + "/MMD.xml");
                    this.ShowNotification(new GUIContent("修改关联数据成功"));
                    return;
                }
            }

            XmlNode nodeName = xmlDoc.CreateNode(XmlNodeType.Element, "Name", null);
            XmlAttribute nodeBGM = xmlDoc.CreateAttribute("BGM");
            XmlAttribute nodeTag = xmlDoc.CreateAttribute("Tag");
            nodeName.InnerText = anim;
            nodeBGM.Value = bgm;
            nodeTag.Value = tag;

            XmlNode root = xmlDoc.SelectSingleNode("Animation");
            root.AppendChild(nodeName);
            nodeName.Attributes.Append(nodeBGM);
            nodeName.Attributes.Append(nodeTag);
            xmlDoc.Save(xmlPath + "/MMD.xml");

            this.ShowNotification(new GUIContent("添加关联数据成功~"));
        }
    }

    private void InitXML()
    {
        if (!File.Exists(xmlPath + "/MMD.xml"))
        {
            //设置xml结点元素
            XmlDocument xmlDoc = new XmlDocument();
            XmlDeclaration dec = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlNode root = xmlDoc.CreateNode(XmlNodeType.Element, "Animation", null);
            XmlNode nodeName = xmlDoc.CreateNode(XmlNodeType.Element, "Name", null);
            XmlAttribute nodeBGM = xmlDoc.CreateAttribute("BGM");
            XmlAttribute nodeTag = xmlDoc.CreateAttribute("Tag");

            //设置节点内面的内容
            nodeName.InnerText = "动画名称";
            nodeBGM.InnerText = "背景音乐名称";
            nodeTag.InnerText = "动画标签";
            xmlDoc.AppendChild(dec);
            xmlDoc.AppendChild(root);
            root.AppendChild(nodeName);
            nodeName.Attributes.Append(nodeBGM);
            nodeName.Attributes.Append(nodeTag);

            //创建xml文件
            xmlDoc.Save(xmlPath + "/MMD.xml");
        }
    }
}
