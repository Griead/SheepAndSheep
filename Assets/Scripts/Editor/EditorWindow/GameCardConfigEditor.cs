using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Work;

public class GameCardConfigEditor : OdinWindowBaseEditor<GameCardEditorConfig>
{
    protected override string Title => "游戏配置";
    protected override string PrototypePath => "GameCardConfigPrototype.xml";
    
    [MenuItem("Tools/配置/游戏卡牌编辑器")]
    private static void Open()
    {
        var window = GetWindow<GameCardConfigEditor>();
    }
    
    protected override void OnInitData()
    {
        m_Config = new GameCardEditorConfig();
        m_Config.GameCardConfigList.Add(new GameCardConfig());
    }
    
    protected override void OnDrawMenuTree(OdinMenuTree _tree)
    {
        base.OnDrawMenuTree(_tree);
        _tree.Add($"配置",m_Config);
    }
    
    protected override void OnBeginDrawEditors()
    {
        var selected = this.MenuTree.Selection.FirstOrDefault();
        var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;
        SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
        {
            if (selected != null)
            {
                GUILayout.Label(selected.Name);
            }

            if (selected.Value is GameCardConfig _GameCardConfig)
            {
                if (SirenixEditorGUI.ToolbarButton(new GUIContent("复制并粘贴到新条目")))
                {
                    m_Config.GameCardConfigList.Add(Clone(_GameCardConfig));
                    base.ForceMenuTreeRebuild();
                }
            }
            
            if (SirenixEditorGUI.ToolbarButton(new GUIContent("刷新")))
            {
                base.ForceMenuTreeRebuild();
            }

            if (SirenixEditorGUI.ToolbarButton(new GUIContent("保存所有数据")))
            {
                SaveMethod();
            }
            
            SirenixEditorGUI.EndHorizontalToolbar();
        }
    }
    
    protected override void SaveMethod()
    {
        base.SaveMethod();
    }
    
    private static T Clone<T>(T RealObject)
    {
        using (Stream stream = new MemoryStream())
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stream, RealObject);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)serializer.Deserialize(stream);
        }
    }
}