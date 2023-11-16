using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Odin类型窗口基类
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class OdinWindowBaseEditor<T> : OdinRootBaseWindow where T : class, new()
{
    protected T m_Config;
    public T GetConfig => m_Config;

    protected abstract string PrototypePath { get; }

    protected virtual string FilePath { get; } = "";

    protected override bool CloseSaveTips => true;

    protected override void OnEnable()
    {
        base.OnEnable();

        InitEditor();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var _tree = new OdinMenuTree(true);
        _tree.DefaultMenuStyle.IconSize = 28.00f;
        _tree.Config.DrawSearchToolbar = true;

        OnDrawMenuTree(_tree);

        return _tree;
    }

    protected override void SaveMethod()
    {
        SaveXml();
        OnSaveMethod();
    }
    /// <summary>
    /// 保存-重写方法
    /// </summary>
    protected virtual void OnSaveMethod() { }


    /// <summary>
    /// 初始化编辑器
    /// </summary>
    private void InitEditor()
    {
        FileUtility.CreateFileFolder(GetPrototypeRootFile());

        if (!FileUtility.HasFile(GetPrototypePath()))
        {
            InitData();
            SaveXml();
        }
        else
        {
            m_Config = LoadDataLogic();
        }
    }
    protected virtual T LoadDataLogic()
    {
        return FileUtility.ReadXmlToData<T>(GetPrototypePath());
    }

    /// <summary>
    /// 获取表路径
    /// </summary>
    /// <returns></returns>
    protected string GetPrototypePath()
    {
        return GetPrototypeRootFile() + PrototypePath;
    }

    public string GetLoadPath => GetPrototypePath();

    /// <summary>
    /// 获取表的根目录文件夹
    /// </summary>
    /// <returns></returns>
    protected virtual string GetPrototypeRootFile()
    {
        string _filepath = string.IsNullOrEmpty(FilePath) ? "" : FilePath;

        string _rootpath = "/Resources/Data/";
        _rootpath += GetRootPath;

        return FileUtility.GetDataPath() + _rootpath + _filepath;
    }

    protected virtual string GetRootPath => "Xml/";

    /// <summary>
    /// 配置表生成（没有表时）
    /// </summary>
    private void InitData()
    {
        m_Config = new T();
        OnInitData();
    }
    /// <summary>
    /// 配置表生成（没有表时）
    /// </summary>
    protected abstract void OnInitData();

    protected void SaveXml()
    {
        OnSaveConfigLogic();

        AssetDatabase.Refresh();
    }
    protected virtual void OnSaveConfigLogic()
    {
        FileUtility.WriteXmlToFile(GetPrototypePath(), m_Config);
    }

    /// <summary>
    /// 保存服务器数据
    /// </summary>
    protected void OnSaveServerData<D>(D _serverdata)
    {
        string _xmlname = PrototypePath.Replace(".xml", null);
        _xmlname = _xmlname.Replace(".asset", null);
    }
}
