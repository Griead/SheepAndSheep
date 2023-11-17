using System.Collections.Generic;
using UnityEngine;
using Work;

public static class GameLevelUtility
{
    private static string m_CardPrefabPath = "Prefab/GameCardItem";
    private static GameObject m_CardModel;
    private static GameLevelConfig m_CurLevelConfig;
    private static Dictionary<int, GameCardItem[][]> m_ItemsMapLayerDict;
    private static GameObject[][] m_ParentMaps;

    private static GameCardBagItem m_BagItem;
    
    public static void Init()
    {
        m_CardModel = Resources.Load<GameObject>(m_CardPrefabPath);
    }

    /// <summary>
    /// 首先生成格子
    /// </summary>
    public static void CreateGrid(Transform root, GameLevelConfig levelConfig, GameCardBagItem bagItem)
    {
        m_BagItem = bagItem;
        m_CurLevelConfig = levelConfig;
        m_ItemsMapLayerDict = new Dictionary<int, GameCardItem[][]>();
        //准备设置父级数据
        SetRoot(root, levelConfig.BottomMaxSize, levelConfig.LayerSizeArray.Length);   
        for (int n = 0; n < levelConfig.LayerSizeArray.Length; n++)
        {
            int curLayerMaxSize = levelConfig.BottomMaxSize - n;
            //获取第一层的左上角坐标
            Vector2 leftTop = new Vector2(root.localPosition.x - (curLayerMaxSize / 2f * GameDefine.GameCardItemSize), root.localPosition.z + (curLayerMaxSize / 2f * GameDefine.GameCardItemSize));
            
            //偏移修正
            leftTop = new Vector2(leftTop.x + GameDefine.GameCardItemSize / 2f,
                leftTop.y + GameDefine.GameCardItemSize / 2f);
            
            Dictionary<int, List<int>> GridDict = new Dictionary<int, List<int>>();
            for (int i = 0; i < curLayerMaxSize; i++)
            {
                List<int> verticalIndexList = new List<int>();
                for (int j = 0; j < curLayerMaxSize; j++)
                {
                    verticalIndexList.Add(j);
                }
                GridDict.Add(i, verticalIndexList);
            }
            
            int layer = n + 1;
            int CreateCount = 0;
            while (CreateCount < levelConfig.LayerSizeArray[n])
            {
                //随机 直到数量足够
                int horizontal = -1;
                int vertical = -1;
                if (RandomCardPosIndex(ref GridDict, ref horizontal, ref vertical))
                {                
                    var go = Object.Instantiate(m_CardModel, GetRoot(layer, vertical));
                    go.name = $"Item_L{layer}_H{horizontal}_V{vertical}";
                    go.GetComponent<RectTransform>().anchoredPosition = CalculateCardLocation(leftTop, horizontal, vertical);
                    var gameCardItem = go.AddComponent<GameCardItem>();
                    gameCardItem.SetData(RandomCardItem(levelConfig.ContainCardEnumArray), new GameCardData() {Layer = layer,Horizontal = horizontal, Vertical = vertical} );
                    //赋值数据
                    if (m_ItemsMapLayerDict.TryGetValue(layer, out var itemMaps))
                    {
                        itemMaps[horizontal][vertical] = gameCardItem;
                    }
                    else
                    {
                        var newLayerMap = new GameCardItem[curLayerMaxSize][];
                        for (int x = 0; x < curLayerMaxSize; x++)
                        {
                            newLayerMap[x] = new GameCardItem[curLayerMaxSize];
                        }
                        newLayerMap[horizontal][vertical] = gameCardItem;
                        m_ItemsMapLayerDict.Add(layer, newLayerMap);
                    }

                    CheckBottomCardMask(layer, horizontal, vertical, IsCover: true);
                    CreateCount ++;
                }
                else
                {
                    //跳出
                    break;
                }
            }
            
            // //首先生成第一层
            // for (int i = 0; i < curLayerMaxSize; i++)
            // {
            //     for (int j = 0; j < curLayerMaxSize; j++)
            //     {
            //         int layer = n + 1;
            //         int horizontal = i;
            //         int vertical = j;
            //         
            //         var go = Object.Instantiate(m_CardModel, root);
            //         go.GetComponent<RectTransform>().anchoredPosition = CalculateCardLocation(leftTop, horizontal, vertical);
            //         var gameCardItem = go.AddComponent<GameCardItem>();
            //         gameCardItem.SetData(RandomCardItem(levelConfig.ContainCardEnumArray), new GameCardData() {Layer = layer,Horizontal = horizontal, Vertical = vertical} );
            //
            //         if (ItemsMapLayerDict.TryGetValue(layer, out var itemMaps))
            //         {
            //             itemMaps[horizontal][vertical] = gameCardItem;
            //         }
            //         else
            //         {
            //             var newLayerMap = new GameCardItem[levelConfig.BottomMaxSize][];
            //             for (int x = 0; x < levelConfig.BottomMaxSize; x++)
            //             {
            //                 newLayerMap[x] = new GameCardItem[levelConfig.BottomMaxSize];
            //             }
            //             newLayerMap[horizontal][vertical] = gameCardItem;
            //             ItemsMapLayerDict.Add(layer, newLayerMap);
            //         }
            //     }
            // }
        }
    }

    private static Vector3 CalculateCardLocation(Vector2 leftTop, int horizontal, int vertical)
    {
        return new Vector2(leftTop.x + horizontal * GameDefine.GameCardItemSize,
            leftTop.y - vertical * GameDefine.GameCardItemSize);
    }

    private static GameCardConfig RandomCardItem(GameCardEnum[] array)
    {
       return new GameCardConfig() {Type = array[Random.Range(0, array.Length)]};
    }
    
    /// <summary>
    /// 随机卡牌坐标
    /// </summary>
    /// <returns></returns>
    private static bool RandomCardPosIndex(ref Dictionary<int, List<int>> GridDict, ref int horizontalResultIndex,
        ref int verticalResultIndex)
    {
        if (GridDict.Count > 0)
        {
            //随机横
            int horizontalMaxCount = GridDict.Count;
            int horizontalIndex = Random.Range(0, horizontalMaxCount);
            List<int> _temp = new List<int>(GridDict.Keys);
            horizontalResultIndex = _temp[horizontalIndex];
            
            //随机纵
            int verticalMaxCount = GridDict[horizontalResultIndex].Count;
            int verticalIndex = Random.Range(0, verticalMaxCount);
            verticalResultIndex = GridDict[horizontalResultIndex][verticalIndex];

            //移除纵列表中数据
            GridDict[horizontalResultIndex].RemoveAt(verticalIndex);

            //检测如果纵列表中物数据 则移除横列表数据
            if (GridDict[horizontalResultIndex].Count <= 0)
                GridDict.Remove(horizontalResultIndex);
            return true;
        }
        else
        {
            return false;
        }
    }
    
    /// <summary>
    /// 检测下层卡牌遮罩
    /// </summary>
    /// <param name="layer">层</param>
    /// <param name="horizontalTempIndex"> 横次序 </param>
    /// <param name="verticalTempIndex"> 纵次序 </param>
    /// <param name="IsCover"> 覆盖还是移除 </param>
    public static void CheckBottomCardMask( int layer, int horizontalIndex, int verticalIndex, bool IsCover)
    {
        //向下层检索
        int nextLayer = layer - 1;
        if (m_ItemsMapLayerDict.TryGetValue(nextLayer, out var maps))
        {
            if (maps is null)
                return;
            
            int horizontalTempIndex = horizontalIndex;
            int verticalTempIndex = verticalIndex;
            if (maps[horizontalTempIndex][verticalTempIndex] != null)
            {
                if(IsCover || !CheckTopCardMask(nextLayer, horizontalTempIndex, verticalTempIndex))
                {
                    maps[horizontalTempIndex][verticalTempIndex].SetMask(IsCover);
                    CheckBottomCardMask(nextLayer, horizontalTempIndex, verticalTempIndex, IsCover);
                }
            }

            horizontalTempIndex = horizontalIndex + 1;
            verticalTempIndex = verticalIndex;
            if (maps[horizontalTempIndex][verticalTempIndex] != null)
            {
                if(IsCover || !CheckTopCardMask(nextLayer, horizontalTempIndex, verticalTempIndex))
                {
                    maps[horizontalTempIndex][verticalTempIndex].SetMask(IsCover);
                    CheckBottomCardMask(nextLayer, horizontalTempIndex, verticalTempIndex, IsCover);
                }
            }

            horizontalTempIndex = horizontalIndex;
            verticalTempIndex = verticalIndex + 1;
            if (maps[horizontalTempIndex][verticalTempIndex] != null)
            {
                if(IsCover || !CheckTopCardMask(nextLayer, horizontalTempIndex, verticalTempIndex))
                {
                    maps[horizontalTempIndex][verticalTempIndex].SetMask(IsCover);
                    CheckBottomCardMask(nextLayer, horizontalTempIndex, verticalTempIndex, IsCover);
                }
            }

            horizontalTempIndex = horizontalIndex + 1;
            verticalTempIndex = verticalIndex + 1;
            if (maps[horizontalTempIndex][verticalTempIndex] != null)
            {
                if(IsCover || !CheckTopCardMask(nextLayer, horizontalTempIndex, verticalTempIndex))
                {
                    maps[horizontalTempIndex][verticalTempIndex].SetMask(IsCover);
                    CheckBottomCardMask(nextLayer, horizontalTempIndex, verticalTempIndex, IsCover);
                }
            }
            // if (maps[horizontalTempIndex][verticalTempIndex] != null)
            // {
            //     if(!IsCover && CheckTopCardMask(nextLayer, horizontalTempIndex, verticalTempIndex))
            //         return;
            //     
            //     maps[horizontalTempIndex][verticalTempIndex].SetMask(IsCover);
            //     CheckBottomCardMask(nextLayer, horizontalTempIndex, verticalTempIndex, IsCover);
            // }
        }
    }
    
    /// <summary>
    /// 检测顶层是否有卡牌遮罩
    /// </summary>
    /// <param name="ItemsMapLayerDict"></param>
    /// <param name="layer"></param>
    /// <param name="horizontalIndex"></param>
    /// <param name="verticalIndex"></param>
    /// <returns></returns>
    public static bool CheckTopCardMask(int layer, int horizontalIndex, int verticalIndex)
    {
        //向上层检索
        int nextLayer = layer + 1;
        if (m_ItemsMapLayerDict.TryGetValue(nextLayer, out var maps))
        {
            if(maps is null)
                return false;

            int horizontalTempIndex = horizontalIndex;
            int verticalTempIndex = verticalIndex;

            if (horizontalTempIndex >= 0 && verticalTempIndex >= 0 &&
                horizontalTempIndex < maps.Length && verticalTempIndex < maps[horizontalTempIndex].Length &&
                maps[horizontalTempIndex][verticalTempIndex] != null)
            {
                return true;
            }

            horizontalTempIndex = horizontalIndex - 1;
            verticalTempIndex = verticalIndex;
            if (horizontalTempIndex >= 0 && verticalTempIndex >= 0 &&
                horizontalTempIndex < maps.Length && verticalTempIndex < maps[horizontalTempIndex].Length &&
                maps[horizontalTempIndex][verticalTempIndex] != null)
            {
                return true;
            }
            
            horizontalTempIndex = horizontalIndex;
            verticalTempIndex = verticalIndex - 1;
            if (horizontalTempIndex >= 0 && verticalTempIndex >= 0 &&
                horizontalTempIndex < maps.Length && verticalTempIndex < maps[horizontalTempIndex].Length &&
                maps[horizontalTempIndex][verticalTempIndex] != null)
            {
                return true;
            }
            
            horizontalTempIndex = horizontalIndex - 1;
            verticalTempIndex = verticalIndex - 1;
            if (horizontalTempIndex >= 0 && verticalTempIndex >= 0 &&
                horizontalTempIndex < maps.Length && verticalTempIndex < maps[horizontalTempIndex].Length &&
                maps[horizontalTempIndex][verticalTempIndex] != null)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 设置当前层父级
    /// </summary>
    /// <returns></returns>
    private static void SetRoot(Transform root, int maxSize, int layer)
    {
        m_ParentMaps = new GameObject[layer][];
        for (int i = 0; i < layer; i++)
        {
            m_ParentMaps[i] = new GameObject[maxSize];
            for (int j = 0; j < maxSize; j++)
            {
                m_ParentMaps[i][j] = new GameObject();
                m_ParentMaps[i][j].name = $"Parent_{i}_{j + 1}";
                m_ParentMaps[i][j].transform.SetParent(root);
                m_ParentMaps[i][j].transform.position = root.position;
            }
        }
    }
    
    /// <summary>
    /// 获取当前父级
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="vertical"></param>
    /// <returns></returns>
    private static Transform GetRoot(int layer, int vertical)
    {
        return m_ParentMaps[layer - 1][vertical].transform;
    }
    
    /// <summary>
    /// 卡牌条目删除操作
    /// </summary>
    public static void CardItemDelOperate(int layer, int horizontal, int vertical)
    {
        //赋值数据
        if (m_ItemsMapLayerDict.TryGetValue(layer, out var itemMaps))
        {
            itemMaps[horizontal][vertical] = null;
        }
        else
        {
            //已经移除过了
            Debug.LogError("已经移除过了");
        }
    }

    /// <summary>
    /// 检测游戏成功
    /// </summary>
    public static bool CheckGameSuccess()
    {
        foreach (var maps in m_ItemsMapLayerDict.Values)
        {
            for (int i = 0; i < maps.Length; i++)
            {
                for (int j = 0; j < maps[i].Length; j++)
                {
                    if (maps[i][j] != null)
                        return false;
                }
            }
        }
        
        //胜利 打开结束面板
        UIUtility.LoadUIView<EndMainUI>(UIType.EndMainUI, null);
        
        PlayerSaveUtility.UpdateLevel(m_CurLevelConfig.Level);
        
        return true;
    }
    

    #region 与背包交互

    public static void CardEnterBag(GameCardItem cardItem)
    {
        m_BagItem.AddCard(cardItem);
    }

    public static void CheckTripleCard(GameCardItem cardItem)
    {
        m_BagItem.CheckCardTriple(cardItem);
    }

    public static void ClearBag()
    {
        // m_BagItem.ClearBag();
        
        //胜利 打开结束面板
        UIUtility.LoadUIView<EndMainUI>(UIType.EndMainUI, null);
        UIUtility.ReleaseUIView(UIType.GameMainUI);
        
        PlayerSaveUtility.UpdateLevel(m_CurLevelConfig.Level + 1);
    }
    #endregion
}