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

    private static Stack<GameCardItem>[] m_ExtraStackMaps;
    
    public static void Init()
    {
        m_CardModel = Resources.Load<GameObject>(m_CardPrefabPath);
    }

    /// <summary>
    /// 首先生成格子
    /// </summary>
    public static void CreateGrid(Transform root, Transform[] extraRootArray, GameLevelConfig levelConfig, GameCardBagItem bagItem)
    {
        m_BagItem = bagItem;
        m_CurLevelConfig = levelConfig;
        m_ItemsMapLayerDict = new Dictionary<int, GameCardItem[][]>();
        //准备设置父级数据
        SetRoot(root, levelConfig.BottomMaxSize, levelConfig.LayerSizeArray.Length);
        //准备数据
        var gameCardConfigArray = SetRandomCardConfig(levelConfig.ContainCardEnumArray, levelConfig.LayerSizeArray, levelConfig.ExtraItemArray);

        int totalCount = 0;
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
            int createCount = 0;
            while (createCount < levelConfig.LayerSizeArray[n])
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
                    gameCardItem.SetData(gameCardConfigArray[totalCount], new GameCardData() {Layer = layer,Horizontal = horizontal, Vertical = vertical} );
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
                    totalCount++;
                    createCount ++;
                }
                else
                {
                    //跳出
                    break;
                }
            }
        }

        int extraItemListCount = levelConfig.ExtraItemArray.Length;
        m_ExtraStackMaps = new Stack<GameCardItem>[extraItemListCount];
        for (int i = 0; i < levelConfig.ExtraItemArray.Length; i++)
        {
            int extraIndex = i;
            m_ExtraStackMaps[extraIndex] = new Stack<GameCardItem>();
            for (int j = 0; j < levelConfig.ExtraItemArray[i]; j++)
            {
                var go = Object.Instantiate(m_CardModel, extraRootArray[extraIndex]);
                go.name = $"Item_Extra{extraIndex}_{j}";
                go.GetComponent<RectTransform>().anchoredPosition = CalculateCardExtraLocation(extraRootArray[extraIndex], extraIndex, j);
                var gameCardItem = go.AddComponent<GameCardItem>();
                m_ExtraStackMaps[extraIndex].Push(gameCardItem);
                gameCardItem.SetExtraData(gameCardConfigArray[totalCount], extraIndex);
                ExtraCheckBottomCardMask(extraIndex, true);
                totalCount++;
            }
            
            ExtraCheckBottomCardMask(extraIndex, false);
        }
    }

    private static Vector3 CalculateCardLocation(Vector2 leftTop, int horizontal, int vertical)
    {
        return new Vector2(leftTop.x + horizontal * GameDefine.GameCardItemSize,
            leftTop.y - vertical * GameDefine.GameCardItemSize);
    }
    
    private static Vector3 CalculateCardExtraLocation(Transform root, int listIndex, int Index)
    {
        if (listIndex == 0)
        {
            //左边
            return new Vector2(root.GetComponent<RectTransform>().anchoredPosition.x + Index * GameDefine.GameCardItemSize * 0.3f,
                root.GetComponent<RectTransform>().anchoredPosition.y); //root.GetComponent<RectTransform>().anchoredPosition.y - Index * GameDefine.GameCardItemSize * 0.1f
        }
        else if(listIndex == 1)
        {
            //右边
            return new Vector2(root.GetComponent<RectTransform>().anchoredPosition.x - Index * GameDefine.GameCardItemSize * 0.3f,
                root.GetComponent<RectTransform>().anchoredPosition.y);
        }
        else
        {
            return Vector3.zero;
        }
    }
    
    /// <summary>
    /// 设置随机卡牌配置
    /// </summary>
    /// <param name="cardEnumArray"></param>
    /// <param name="layerSizeArray"></param>
    /// <param name="extraArray"></param>
    private static GameCardConfig[] SetRandomCardConfig(GameCardEnum[] cardEnumArray, int[] layerSizeArray, int[] extraArray)
    {
        //计算总数
        int _itemCount = 0;
        for (int i = 0; i < layerSizeArray.Length; i++)
        {
            _itemCount += layerSizeArray[i];
        }
        for (int i = 0; i < extraArray.Length; i++)
        {
            _itemCount += extraArray[i];
        }
        
        // 平均之后数量
        int _averageCount = _itemCount / 3;
        
        //顺序数组
        int[] totalIndex = new int[_itemCount];
        for (int i = 0; i < _itemCount; i++)
        {
            totalIndex[i] = i;
        }
        //打乱数组
        System.Random random = new System.Random();
        for (int i = totalIndex.Length - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);

            //交换元素
            (totalIndex[i], totalIndex[j]) = (totalIndex[j], totalIndex[i]);
        }

        //乱序索引数组赋值
        GameCardConfig[] gameCardConfigArray = new GameCardConfig[_itemCount];
        int _point = 0;
        for (int i = 0; i < _averageCount; i++)
        {
            var gameCardEnum = cardEnumArray[random.Next(0, cardEnumArray.Length)];
            gameCardConfigArray[totalIndex[_point++]] = new GameCardConfig(){Type = gameCardEnum};
            gameCardConfigArray[totalIndex[_point++]] = new GameCardConfig(){Type = gameCardEnum};
            gameCardConfigArray[totalIndex[_point++]] = new GameCardConfig(){Type = gameCardEnum};
        }
        //剩余补充
        var remainGameCardEnum = cardEnumArray[random.Next(0, cardEnumArray.Length)];
        for (int i = _point; i < _itemCount; i++)
        {
            gameCardConfigArray[totalIndex[i]] = new GameCardConfig() { Type = remainGameCardEnum };
        }

        return gameCardConfigArray;
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
    
    public static void ExtraCheckBottomCardMask(int extraIndex, bool IsCover)
    {
        if (m_ExtraStackMaps[extraIndex] != null && m_ExtraStackMaps[extraIndex].Count > 0)
        {
            m_ExtraStackMaps[extraIndex].Peek().SetMask(IsCover);
        }
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

    public static void ExtraCardItemDelOperate(int extraIndex)
    {
        if (m_ExtraStackMaps[extraIndex] != null && m_ExtraStackMaps[extraIndex].Count > 0)
        {
            m_ExtraStackMaps[extraIndex].Pop();
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

        int extraMapsLength = m_ExtraStackMaps.Length;
        for (int i = 0; i < extraMapsLength; i++)
        {
            if (m_ExtraStackMaps[i].Count > 0)
            {
                return false;
            }
        }
        
        //更新关卡数据
        PlayerSaveUtility.UpdateLevel(m_CurLevelConfig.Level + 1);
        
        //胜利 打开结束面板
        UIUtility.LoadUIView<EndMainUI>(UIType.EndMainUI, null);
        UIUtility.ReleaseUIView(UIType.GameMainUI);
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
        m_BagItem.ClearBag();
    }
    #endregion
}