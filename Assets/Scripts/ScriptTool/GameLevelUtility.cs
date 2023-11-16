using System.Collections.Generic;
using UnityEngine;
using Work;

public static class GameLevelUtility
{
    public static Transform m_Root;

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
            Vector2 leftTop = new Vector2(root.localPosition.x - (curLayerMaxSize / 2 * GameDefine.GameCardItemSize), root.localPosition.z - (curLayerMaxSize / 2 * GameDefine.GameCardItemSize))    ;
        
            //首先生成第一层
            for (int i = 0; i < curLayerMaxSize; i++)
            {
                for (int j = 0; j < curLayerMaxSize; j++)
                {
                    int layer = n + 1;
                    int horizontal = i;
                    int vertical = j;
                    
                    var go = Object.Instantiate(m_CardModel, CalculateCardLocation(leftTop, horizontal, vertical), Quaternion.identity, root);
                    var gameCardItem = go.AddComponent<GameCardItem>();
                    gameCardItem.SetData(RandomCardItem(levelConfig.ContainCardEnumArray), new GameCardData() {Layer = layer,Horizontal = horizontal, Vertical = vertical} );

                    if (ItemsMapLayerDict.TryGetValue(layer, out var itemMaps))
                    {
                        itemMaps[horizontal][vertical] = gameCardItem;
                    }
                    else
                    {
                        var newLayerMap = new GameCardItem[levelConfig.BottomMaxSize][];
                        for (int x = 0; x < levelConfig.BottomMaxSize; x++)
                        {
                            newLayerMap[x] = new GameCardItem[levelConfig.BottomMaxSize];
                        }
                        newLayerMap[horizontal][vertical] = gameCardItem;
                        ItemsMapLayerDict.Add(layer, newLayerMap);
                    }
                }
            }
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
}