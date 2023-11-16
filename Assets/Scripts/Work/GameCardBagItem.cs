using System;
using UnityEngine;

public class GameCardBagItem :MonoBehaviour
{


    private void Awake()
    {
        
    }

    private void Start()
    {
        
    }

    public void SetData(int gridCount)
    {
        BagItemArray = new GameCardItem[gridCount];
    }

    /// <summary>
    /// 添加一个新卡牌进来
    /// </summary>
    public void AddCard()
    {
        //判断是否需要腾格子
        
        //要回传一个新格子的位置
    }
    /// <summary>
    /// 格子数量
    /// </summary>
    private int GridCount;

    private GameCardItem[] BagItemArray;
    /// <summary>
    /// 查找卡牌在背包的次序
    /// </summary>
    // private int FindCardBagItemIndex(GameCardConfig config)
    // {
    //     int index = -1;
    //     for (int i = 0; i < BagItemArray.Length; i++)
    //     {
    //         //空格子
    //         if (BagItemArray[i] is null)
    //         {
    //             index = i;
    //             break;
    //         }
    //         else
    //         {
    //             //如果类型相同则排到其后边 后边格子依次 
    //             if (BagItemArray[i].m_Config.Type == config.Type)
    //             {
    //                 
    //             }
    //         }
    //     }
    // }
}