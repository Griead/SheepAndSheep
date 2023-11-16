
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine.Rendering;

public class GameModifierEditor : EditorWindow
{
    private Vector2 m_Scroller = Vector2.zero;
    #region 物品修改器
    /// <summary>
    /// 物品修改器折叠开关
    /// </summary>
    private bool m_GetPropFlotout;
    /// <summary>
    /// 物品ID
    /// </summary>
    private int m_PropID = -1;
    /// <summary>
    /// 生成的物品数量
    /// </summary>
    private int m_CreatPropNum = 0;
    #endregion

    [MenuItem("Tools/测试/游戏修改器 #1")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(GameModifierEditor), false, "游戏修改器");
    }

    private void OnGUI()
    {
        m_Scroller = EditorGUILayout.BeginScrollView(m_Scroller);

        EditorGUILayout.Space();

        #region 测试
        m_GetPropFlotout = EditorGUILayout.BeginFoldoutHeaderGroup(m_GetPropFlotout, "测试");
        if (m_GetPropFlotout)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("测试修改器");
            // m_PropID = EditorGUILayout.IntField("物品ID: ", m_PropID);
            // m_CreatPropNum = EditorGUILayout.IntField("生成数量: ", m_CreatPropNum);

            if (GUILayout.Button("写入编辑器表"))
            {
                Debug.Log("测试");
                
                var m_Config = new GameCardEditorConfig();
                m_Config.GameCardConfigList = new List<GameCardConfig>() { new GameCardConfig() };
                FileUtility.WriteXmlToFile(GetPrototypePath(), m_Config);
            }
            if (GUILayout.Button("读取编辑器表"))
            {
                TextAsset xmlTextAsset = Resources.Load<TextAsset>("Data/Xml/GameCardConfigPrototype");
                GameCardEditorConfig _config = FileUtility.ReadXmlStringToData<GameCardEditorConfig>(xmlTextAsset.text); 
            }

            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space();

        #endregion

        EditorGUILayout.EndScrollView();
    }
    
    private string GetPrototypeRootFile()
    {
        string _rootpath = "/Resources/Data/";
        _rootpath += GetRootPath;

        return FileUtility.GetDataPath() + _rootpath;
    }
    
    private string GetPrototypePath()
    {
        return GetResourcesLoadPath() + ".xml";
    }

    private string GetResourcesLoadPath()
    {
        return GetPrototypeRootFile() + PrototypePath;
    }
    
    private string GetRootPath => "Xml/";
    
    private string PrototypePath => "GameCardConfigPrototype";
}
