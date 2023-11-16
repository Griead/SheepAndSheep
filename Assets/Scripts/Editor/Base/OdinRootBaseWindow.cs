using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Odin窗口基类根节点
/// </summary>
public abstract class OdinRootBaseWindow : OdinMenuEditorWindow
{
    protected abstract string Title { get; }

    protected abstract bool CloseSaveTips { get; }

    /// <summary>
    /// 是否绘制工具栏
    /// </summary>
    protected virtual bool IsDrawToolBar { get; } = false;
    /// <summary>
    /// 是否打开工具
    /// </summary>
    private bool m_IsOpenTool;

    protected OdinRootBaseWindow()
    {
        titleContent = new GUIContent(Title);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (CloseSaveTips && UnityEditor.EditorUtility.DisplayDialog("保存提示", "是否保存", "是", "否"))
            SaveMethod();
    }

    protected abstract void SaveMethod();

    protected override OdinMenuTree BuildMenuTree()
    {
        var _tree = new OdinMenuTree(true);
        _tree.DefaultMenuStyle.IconSize = 28.00f;
        _tree.Config.DrawSearchToolbar = true;

        OnDrawMenuTree(_tree);

        return _tree;
    }
    /// <summary>
    /// 目录绘制
    /// </summary>
    /// <param name="_tree"></param>
    protected virtual void OnDrawMenuTree(OdinMenuTree _tree) { }

    /// <summary>
    /// 目录增加
    /// </summary>
    /// <param name="_title"></param>
    /// <param name="_value"></param>
    protected void OnMenuTreeAdd(string _title, object _value)
    {
        MenuTree.Add(_title, _value);
    }
    /// <summary>
    /// 目录增加并选中
    /// </summary>
    /// <param name="_title"></param>
    /// <param name="_value"></param>
    protected void OnMenuTreeAddOrSelect(string _title, object _value)
    {
        OnMenuTreeAdd(_title, _value);
        base.TrySelectMenuItemWithObject(_value);
    }

    protected override void OnBeginDrawEditors()
    {
        var selected = this.MenuTree.Selection.FirstOrDefault();
        string _title = selected != null ? selected.Name : null;
        OnDrawToolBar(_title, () =>
        {
            if (IsDrawToolBar)
                m_IsOpenTool = SirenixEditorGUI.Foldout(m_IsOpenTool, m_IsOpenTool ? "关闭快捷工具" : "显示快捷工具");
        }, () =>
        {
            OnDrawBeginEditor();

            if (SirenixEditorGUI.ToolbarButton(new GUIContent("刷新")))
            {
                OnRebuildMenuTree();
                base.ForceMenuTreeRebuild();
            }

            if (SirenixEditorGUI.ToolbarButton(new GUIContent("保存所有数据")))
            {
                SaveMethod();
            }
        });

        if (IsDrawToolBar)
            DrawToolBar();

        OnDrawBeginNextLineEditor();
    }
    /// <summary>
    /// 刷新目录
    /// </summary>
    protected virtual void OnRebuildMenuTree() { }
    /// <summary>
    /// 绘制工具栏
    /// </summary>
    /// <param name="_action"></param>
    protected void OnDrawToolBar(string _title, Action _topaction, Action _action)
    {
        var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;
        SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
        _topaction?.Invoke();

        if (!string.IsNullOrEmpty(_title))
            GUILayout.Label(_title);

        _action?.Invoke();
        SirenixEditorGUI.EndHorizontalToolbar();
    }

    /// <summary>
    /// 绘制右上角按钮
    /// </summary>
    protected virtual void OnDrawBeginEditor() { }
    /// <summary>
    /// 绘制工具栏
    /// </summary>
    private void DrawToolBar()
    {
        if (!m_IsOpenTool)
            return;

        OnDrawToolBar("便捷工具", null, () =>
        {
            OnDrawToolBar();
        });
    }
    protected virtual void OnDrawToolBar() { }
    /// <summary>
    /// 绘制右上角下一行按钮
    /// </summary>
    protected virtual void OnDrawBeginNextLineEditor() { }

    /// <summary>
    /// 按钮绘制
    /// </summary>
    /// <param name="_content"></param>
    /// <param name="_event"></param>
    protected void OnToolBarButton(string _content, Action _event)
    {
        if (SirenixEditorGUI.ToolbarButton(new GUIContent(_content)))
        {
            _event?.Invoke();
        }
    }
    /// <summary>
    /// Log显示
    /// </summary>
    /// <param name="_content"></param>
    protected void OnShowLog(string _content)
    {
        EditorWindow.focusedWindow.ShowNotification(new GUIContent(_content));
    }
}
