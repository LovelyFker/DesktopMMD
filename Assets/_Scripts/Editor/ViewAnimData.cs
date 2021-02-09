using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class ViewAnimData : EditorWindow
{
    private static string xmlPath;

    private List<bool> toggleList = new List<bool>();
    private Vector2 scrollViewPos;

    
    private void OnEnable()
    {
        xmlPath = Application.dataPath + "/Xml";
    }

    private void OnGUI()
    {
        if (File.Exists(xmlPath + "/MMD.xml"))
        {
            EditorGUILayout.HelpBox("展示关联动画数据~", MessageType.Info);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath + "/MMD.xml");

            XmlNodeList xmlNodeList = xmlDoc.SelectNodes("Animation/Name");
            scrollViewPos = EditorGUILayout.BeginScrollView(scrollViewPos, false, true, GUILayout.Height(250));
            GUIStyle highLightStyle = new GUIStyle();
            highLightStyle.normal.textColor = Color.green;
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                //限制toggleList长度，防止内存滥用
                if (toggleList.Count < xmlNodeList.Count)
                    toggleList.Add(false);

                toggleList[i] = EditorGUILayout.ToggleLeft("动画名称：" + xmlNodeList.Item(i).InnerText, toggleList[i]);
                EditorGUILayout.LabelField("背景音乐BGM名称：" + xmlNodeList.Item(i).Attributes.Item(0).Value, highLightStyle);
                EditorGUILayout.LabelField("Tag标签号：" + xmlNodeList.Item(i).Attributes.Item(1).Value);
                EditorGUILayout.LabelField("=========================");
            }
            EditorGUILayout.EndScrollView();
        }
        else
        {
            EditorGUILayout.HelpBox("无关联动画数据~", MessageType.Info);
        }

        if (GUILayout.Button("删除所选关联数据", GUILayout.Height(30)))
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath + "/MMD.xml");

            XmlNodeList xmlNodeList = xmlDoc.SelectNodes("Animation/Name");
            XmlNode root = xmlDoc.SelectSingleNode("Animation");
            foreach(bool b in toggleList)
            {
                if (b)
                {
                    this.ShowNotification(new GUIContent("删除成功!!!"));
                    break;
                }
                this.ShowNotification(new GUIContent("未选择任何关联数据!!!"));
            }

            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                if (toggleList[i])
                {
                    root.RemoveChild(xmlNodeList[i]);
                    toggleList[i] = false;
                }
            }
            xmlDoc.Save(xmlPath + "/MMD.xml");
        }
    }
}
