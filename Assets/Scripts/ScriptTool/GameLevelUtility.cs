using System.Collections.Generic;
using UnityEngine;
using Work;

public static class GameLevelUtility
{
    private static string m_CardPrefabPath = "Prefab/GameCardItem";
    public static GameObject m_CardModel;
    private static GameLevelConfig m_CurLevelConfig;
    public static void Init()
    {
        m_CardModel = Resources.Load<GameObject>(m_CardPrefabPath);
    }

    /// <summary>
    /// 首先生成格子
    /// </summary>
    public static void CreateGrid(Transform root, GameLevelConfig levelConfig, ref Dictionary<int, GameCardItem[][]> ItemsMapLayerDict)
    {
        for (int n = 0; n < levelConfig.LayerSizeArray.Length; n++)
        {
            int curLayerMaxSize = levelConfig.BottomMaxSize - n;
            //获取第一层的左上角坐标
            Vector2 leftTop = new Vector2(root.localPosition.x - (curLayerMaxSize / 2f * GameDefine.GameCardItemSize), root.localPosition.z - (curLayerMaxSize / 2f * GameDefine.GameCardItemSize));
        
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
                    var go = Object.Instantiate(m_CardModel, root);
                    go.GetComponent<RectTransform>().anchoredPosition = CalculateCardLocation(leftTop, horizontal, vertical);
                    var gameCardItem = go.AddComponent<GameCardItem>();
                    gameCardItem.SetData(RandomCardItem(levelConfig.ContainCardEnumArray), new GameCardData() {Layer = layer,Horizontal = horizontal, Vertical = vertical} );
                    //赋值数据
                    if (ItemsMapLayerDict.TryGetValue(layer, out var itemMaps))
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
                        ItemsMapLayerDict.Add(layer, newLayerMap);
                    }

                    CheckBottomCardMask(ItemsMapLayerDict, layer, horizontal, vertical, IsCover: true);
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
            leftTop.y + vertical * GameDefine.GameCardItemSize);
    }

    private static GameCardConfig RandomCardItem(GameCardEnum[] array)
    {
       return new GameCardConfig() {Type = array[Random.Range(0, array.Length)]};
    }
    
    /// <summary>
    /// 随机卡牌坐标
    /// </summary>
    /// <param name="GridDict"></param>
    /// <param name="horizontalIndex"></param>
    /// <param name="verticalIndex"></param>
    /// <returns></returns>
    private static bool RandomCardPosIndex(ref Dictionary<int,List<int>> GridDict, ref int horizontalIndex, ref int verticalIndex)
    {
        if (GridDict.Count > 0)
        {
            //随机横
            int horizontalMaxCount = GridDict.Keys.Count;
            horizontalIndex = Random.Range(0, horizontalMaxCount);

            //随机纵
            int verticalMaxCount = GridDict[horizontalIndex].Count;
            verticalIndex = Random.Range(0, verticalMaxCount);
            
            //移除纵列表中数据
            GridDict[horizontalIndex].Remove(verticalIndex);
            
            //检测如果纵列表中物数据 则移除横列表数据
            if (GridDict[horizontalIndex].Count <= 0)
                GridDict.Remove(horizontalIndex);
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
    /// <param name="horizontalIndex"> 横次序 </param>
    /// <param name="verticalIndex"> 纵次序 </param>
    /// <param name="IsCover"> 覆盖还是移除 </param>
    private static void CheckBottomCardMask(Dictionary<int, GameCardItem[][]> ItemsMapLayerDict, int layer, int horizontalIndex, int verticalIndex, bool IsCover)
    {
        //向下层检索
        int nextLayer = layer - 1;
        if (ItemsMapLayerDict.TryGetValue(nextLayer, out var maps))
        {
            if (maps is null)
                return;

            if (maps[horizontalIndex][verticalIndex] != null)
            {
                maps[horizontalIndex][verticalIndex].SetMask(IsCover);
                CheckBottomCardMask(ItemsMapLayerDict, nextLayer, horizontalIndex, verticalIndex, IsCover);
            }

            if (maps[horizontalIndex + 1][verticalIndex] != null)
            {
                maps[horizontalIndex + 1][verticalIndex].SetMask(IsCover);
                CheckBottomCardMask(ItemsMapLayerDict, nextLayer, horizontalIndex + 1, verticalIndex, IsCover);
            }

            if (maps[horizontalIndex][verticalIndex + 1] != null)
            {
                maps[horizontalIndex][verticalIndex + 1].SetMask(IsCover);
                CheckBottomCardMask(ItemsMapLayerDict, nextLayer, horizontalIndex, verticalIndex + 1, IsCover);
            }

            if (maps[horizontalIndex + 1][verticalIndex + 1] != null)
            {
                maps[horizontalIndex + 1][verticalIndex + 1].SetMask(IsCover);
                CheckBottomCardMask(ItemsMapLayerDict, nextLayer, horizontalIndex + 1, verticalIndex + 1, IsCover);
            }
        }
    }
}