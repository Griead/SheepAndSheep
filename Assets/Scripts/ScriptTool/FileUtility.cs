using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 文件读取工具
/// </summary>
public static class FileUtility
{
    #region 平台

    /// <summary>
    /// 获取当前平台路径
    /// </summary>
    /// <returns></returns>
    public static string GetDataPath()
    {
        //iphone平台
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
            path = path.Substring(0, path.LastIndexOf('/'));
            return path + "/Documents";
        }
        //Andrlid平台
        else if (Application.platform == RuntimePlatform.Android)
        {
            return Application.dataPath;
        }
        //pc平台
        else if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            return Application.dataPath;
        }
        //unity3d
        else
        {
            return Application.dataPath;
        }
    }

    #endregion

    #region 文件流读取

    /// <summary>
    /// 判断文件是否存在：
    /// </summary>
    public static bool HasFile(string fileName)
    {
        return File.Exists(fileName);
    }


    /// <summary>
    /// 编辑器模式读取文件并转成字符串数组-按行读取
    /// </summary>
    /// <param name="_filepath"></param>
    /// <returns></returns>
    public static string[] ReadEditorFileToString_Line(string _filepath)
    {
        return File.ReadAllLines(_filepath);
    }

    /// <summary>
    /// 获取文件字节内容
    /// </summary>
    /// <param name="_contents"></param>
    /// <returns></returns>
    public static string GetFileByteContent(byte[] _contents)
    {
        if (_contents == null)
            return null;

        return Encoding.UTF8.GetString(_contents).Trim();
    }

    /// <summary>
    /// 读取XML转换数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_filepath"></param>
    /// <returns></returns>
    public static T ReadXmlToData<T>(string _filepath) where T : class
    {
        StreamReader sReader = File.OpenText(_filepath);
        string dataString = sReader.ReadToEnd();
        sReader.Close();

        return ReadXmlStringToData<T>(dataString);
    }

    /// <summary>
    /// 读取xml字符串转换数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_content"></param>
    /// <returns></returns>
    public static T ReadXmlStringToData<T>(string _content) where T : class
    {
        using (StringReader sr = new StringReader(_content))
        {
            XmlSerializer xmldes = new XmlSerializer(typeof(T));
            return xmldes.Deserialize(sr) as T;
        }
    }

    /// <summary>
    /// 读取Json数据并转换成数据(文件流方式)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_filename"></param>
    /// <returns></returns>
    public static T ReadFileJsonToData<T>(string _filename) where T : class, new()
    {
        string _json = ReadFileData(_filename);
        T _jsoncontent = new T();
        _jsoncontent = JsonUtility.FromJson<T>(_json);
        return _jsoncontent;
    }

    /// <summary>
    /// 将Json转换成数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_content"></param>
    /// <returns></returns>
    public static T ReadJsonTurnData<T>(string _content) where T : class, new()
    {
        T _jsoncontent = new T();
        _jsoncontent = JsonUtility.FromJson<T>(_content);
        return _jsoncontent;
    }

    /// <summary>
    /// 读取文件数据
    /// </summary>
    /// <param name="_path"></param>
    /// <returns></returns>
    public static string ReadFileData(string _path)
    {
        return File.ReadAllText(_path);
    }

    #endregion

    #region 文件流写入

    /// <summary>
    /// xml写入文件流
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_filepath"></param>
    /// <param name="_content"></param>
    public static void WriteXmlToFile<T>(string _filepath, T _content)
    {
        FileStream _file;
        if (!File.Exists(_filepath))
        {
            _file = File.Create(_filepath);
        }
        else
            _file = File.OpenWrite(_filepath);

        MemoryStream memoryStream = new MemoryStream();
        XmlSerializer xs = new XmlSerializer(typeof(T));
        //清空内容
        _file.SetLength(0);
        xs.Serialize(_file, _content);
        _file.Close();
    }

    /// <summary>
    /// 字节写入文件
    /// </summary>
    /// <param name="_filepath"></param>
    /// <param name="_content"></param>
    public static void WriteByteToFile(string _filepath, byte[] _content)
    {
        File.WriteAllBytes(_filepath, _content);
    }
    
    public static void WriteFileJson<T>(string _path, T _config) where T : class, new()
    {
        var _setting = new Newtonsoft.Json.JsonSerializerSettings()
        {
            TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All
        };
        string _content = Newtonsoft.Json.JsonConvert.SerializeObject(_config, Newtonsoft.Json.Formatting.Indented, _setting);
        File.WriteAllText(_path, _content, Encoding.UTF8);
    }

    public static T ReadFileJson<T>(string _path) where T : class, new()
    {
        string _content = File.ReadAllText(_path, Encoding.UTF8);
        var _setting = new Newtonsoft.Json.JsonSerializerSettings()
        {
            TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All
        };
        var config = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(_content, _setting);
        return config;
    }

    #endregion

    #region 文件流复制

    /// <summary>
    /// 文件流复制
    /// </summary>
    public static void FileCopy(string _from, string _target, bool _overwrite = true)
    {
        File.Copy(_from, _target, _overwrite);
    }

    #endregion

    #region 创建文件夹
    /// <summary>
    /// 创建文件夹
    /// </summary>
    /// <param name="_path"></param>
    public static void CreateFileFolder(string _path)
    {
        if (!Directory.Exists(_path))
            Directory.CreateDirectory(_path);
    }

    #endregion


    /// <summary>
    /// 编辑器加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_path"></param>
    /// <returns></returns>
    public static T EditorLoadAsset<T>(string _path) where T : Object
    {
#if !UNITY_EDITOR
        return null;
#endif
        if (string.IsNullOrEmpty(_path))
            return null;

#if UNITY_EDITOR
        return AssetDatabase.LoadAssetAtPath(_path, typeof(T)) as T;
#endif
        return null;
    }

    /// <summary>
    /// 编辑器获取资源路径
    /// </summary>
    /// <param name="_obj"></param>
    /// <returns></returns>
    public static string EditorGetAssetPath(Object _obj)
    {
#if !UNITY_EDITOR
        return null;
#endif

        if (_obj == null)
            return "";

#if UNITY_EDITOR
        return AssetDatabase.GetAssetPath(_obj);
#endif
        return null;
    }
}
