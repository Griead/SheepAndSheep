using System.IO;
using UnityEngine;

public static class PlayerSaveUtility
{
    public static GameLevelData m_SaveData;

    private static string m_SaveFolderPath;
    private static string m_SaveFilePath;

    public static void Init()
    {
        m_SaveFolderPath = Path.Combine(GetDataPath(), GameDefine.PlayDataSaveFolderPath).Replace("\\", "/");
        m_SaveFilePath = m_SaveFolderPath + GameDefine.PlayDataSaveFilePath;
        LoadData();
    }

    private static string GetDataPath()
    {
        // return Application.persistentDataPath;
        return Application.streamingAssetsPath;
    }
    
    /// <summary>
    /// 更新关卡
    /// </summary>
    public static void UpdateLevel(int level)
    {
        if (m_SaveData is null)
            m_SaveData = new GameLevelData();
        
        m_SaveData.MaxLevel = level;
    }
    
    /// <summary>
    /// 保存数据
    /// </summary>
    public static void SaveData()
    {
        FileUtility.CreateFileFolder(m_SaveFolderPath);
        
        FileUtility.WriteFileJson(m_SaveFilePath, m_SaveData);
    }

    /// <summary>
    /// 读取数据
    /// </summary>
    private static void LoadData()
    {
        FileUtility.CreateFileFolder(m_SaveFolderPath);

        if (FileUtility.HasFile(m_SaveFilePath))
        {
            var jsonData = FileUtility.ReadFileJson<GameLevelData>(m_SaveFilePath);
            m_SaveData = jsonData;
        }
        else
        {
            m_SaveData = new GameLevelData();
            
            SaveData();
        }   
    }
}