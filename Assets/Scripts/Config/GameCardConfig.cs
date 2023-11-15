using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;
using Work;

[Serializable]
public class GameCardEditorConfig : BaseEditorConfig
{
    public GameCardEditorConfig()
    {
        GameCardConfigList = new List<GameCardConfig>();
    }
    
    public List<GameCardConfig> GameCardConfigList;
}

[Serializable]
public class GameCardConfig
{
    public GameCardEnum Type;
    
    /// <summary>
    /// 活动显示图片资源路径
    /// </summary>
    [ReadOnly]  public string ImagePath;
    
    [ShowInInspector]
    public Sprite UISprite
    {
        get { return FileUtility.EditorLoadAsset<Sprite>(ImagePath); }
        set { ImagePath = FileUtility.EditorGetAssetPath(value); }
    }
}

/// <summary>
/// 游戏内特殊工具
/// </summary>
[Serializable]
public class GameSpecialTool
{
}
    
/// <summary>
/// 游戏关卡配置
/// </summary>
[Serializable]
public class GameLevelConfig
{
    /// <summary>
    /// 包含的卡牌类型
    /// </summary>
    public GameCardEnum[] ContainCardEnumArray;

    /// <summary>
    /// 包含的特殊工具数组
    /// </summary>
    public GameSpecialToolEnum[] ContainSpecialToolEnumArray;
    
    /// <summary>
    /// 底层最大尺寸 越向上层尺寸依次减一
    /// </summary>
    public int BottomMaxSize;
    
    /// <summary>
    /// 每层的具体item数量
    /// </summary>
    public int[] LayerSizeArray;
    
    /// <summary>
    /// 最大背包格子数量
    /// </summary>
    public int MaxBagItemCount;
    
    /// <summary>
    /// 额外的卡牌条目数量数组
    /// </summary>
    public int[] ExtraItemArray;
}