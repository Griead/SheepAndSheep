using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;
using Work;
using Random = UnityEngine.Random;

[Serializable]
public class GameCardEditorConfig : BaseEditorConfig
{
    public GameCardEditorConfig()
    {
        GameCardConfigList = new List<GameCardConfig>();
        GameLevelConfigList = new List<GameLevelConfig>();
    }
    
    public List<GameCardConfig> GameCardConfigList;

    public List<GameLevelConfig> GameLevelConfigList;
    
    /// <summary>
    /// 获取等级配置
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public GameLevelConfig GetLevelConfig(int level)
    {
        if (level < GameLevelConfigList.Count)
        {
            return GameLevelConfigList[level - 1];
        }
        else
        {
            var config = new GameLevelConfig(level);
            return config;
        }
    }
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
    public GameLevelConfig()
    {
        ContainCardEnumArray = new GameCardEnum[] { GameCardEnum.Cat, GameCardEnum.Catnip, GameCardEnum.CatBag, GameCardEnum.CatBowl, GameCardEnum.CatFood };
        ContainSpecialToolEnumArray = new GameSpecialToolEnum[] { };

        BottomMaxSize = 6;
        LayerSizeArray = new int[] { 30, 21, 9 };
        MaxBagItemCount = 6;
        ExtraItemArray = new[] { 6, 6 };
    }

    public GameLevelConfig(int level)
    {
        //随机元素
        ContainCardEnumArray = new GameCardEnum[] { GameCardEnum.Cat, GameCardEnum.Catnip, GameCardEnum.CatBag, GameCardEnum.CatBowl, GameCardEnum.CatFood };
        List<GameCardEnum> EnumList = new List<GameCardEnum>();
        int enumCount = Random.Range(4, 6);
        for (int i = 0; i < enumCount; i++)
        {
            EnumList.Add(ContainCardEnumArray[Random.Range(0,ContainCardEnumArray.Length)]);
        }
        ContainCardEnumArray = EnumList.ToArray();
        
        ContainSpecialToolEnumArray = new GameSpecialToolEnum[] { };

        Level = level;
        BottomMaxSize = Random.Range(6,8);

        List<int> layerSizeList = new List<int>();
        int layer = Random.Range(3, 6);
        for (int i = 0; i < layer; i++)
        {
            int max =(BottomMaxSize - i) * (BottomMaxSize - i) ;
            int min = Mathf.FloorToInt(max * 0.8f);

            int randomResult = Random.Range(min, max + 1);
            layerSizeList.Add(randomResult);
        }
        
        LayerSizeArray = layerSizeList.ToArray();
        MaxBagItemCount = 6;
        
        ExtraItemArray = new[] { 6, 6 };
    }

    public int Level;
    
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