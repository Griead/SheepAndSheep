using System;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class GameCardBagItem :MonoBehaviour
{
    private Transform Root;
    
    /// <summary>
    /// 格子数量
    /// </summary>
    private int GridCount;

    private GameCardItem[] BagItemArray;
    private void Awake()
    {
        Root = transform.Find("Root");
    }

    public void SetData(int gridCount)
    {
        BagItemArray = new GameCardItem[gridCount];
    }

    /// <summary>
    /// 添加一个新卡牌进来
    /// </summary>
    public void AddCard(GameCardItem cardItem)
    {
        //判断是否背包已满
        bool Isfull = true;
        for (int i = 0; i < BagItemArray.Length; i++)
        {
            if (BagItemArray[i] is null)
            {
                Isfull = false;
                break;
            }
        }
        if (Isfull)
            return;
        
        //要回传一个新格子的位置
        cardItem.transform.SetParent(Root);
        int Index = FindCardBagItemIndex(cardItem);
        cardItem.ReceiveBag(GetIndexPos(Index));
    }
    
    /// <summary>
    /// 检测背包卡牌三联
    /// </summary>
    public void CheckCardTriple(GameCardItem cardItem)
    {
        List<int> cardItemLiat = new List<int>(3);
        for (int i = 0; i < BagItemArray.Length; i++)
        {
            if (BagItemArray[i].m_Config.Type == cardItem.m_Config.Type)
            {
                cardItemLiat.Add(i);
                
                if (cardItemLiat.Count >= 3)
                {
                    //处理 这三张卡牌
                    for (int j = 0; j < cardItemLiat.Count; j++)
                    {
                        BagItemArray[cardItemLiat[j]].ReceiveTriple();
                        BagItemArray[cardItemLiat[j]] = null;
                    }
                    
                    // 整理卡牌数组，将后边的卡牌依次前移
                    int emptyIndex = cardItemLiat[0];
                    for (int k = cardItemLiat[2] + 1; k < BagItemArray.Length; k++)
                    {
                        if (BagItemArray[k] != null)
                        {
                            BagItemArray[emptyIndex++] = BagItemArray[k];
                            
                            BagItemArray[k] = null;
                            
                            if(BagItemArray[emptyIndex] != null)
                                BagItemArray[emptyIndex].GetComponent<RectTransform>().DOLocalMove(GetIndexPos(emptyIndex), 0.3f);
                        }
                    }
                
                    break;
                }
            }
        }
    }
    
    /// <summary>
    /// 查找卡牌在背包的次序
    /// </summary>
    private int FindCardBagItemIndex(GameCardItem cardItem)
    {
        int index = -1;
        for (int i = 0; i < BagItemArray.Length; i++)
        {
            //空格子
            if (BagItemArray[i] is null)
            {
                BagItemArray[i] = cardItem;
                index = i;
                break;
            }
            else
            {
                //如果类型相同则排到其后边 
                if (BagItemArray[i].m_Config.Type == cardItem.m_Config.Type)
                {
                    index = i + 1;
                    
                    //后边格子依次向后移
                    for (int j = BagItemArray.Length - 1; j > index; j--)
                    {
                        BagItemArray[j] = BagItemArray[j - 1];
                        
                        //移动到新位置
                        if(BagItemArray[j] != null)
                            BagItemArray[j].GetComponent<RectTransform>().DOLocalMove(GetIndexPos(j), 0.3f);
                    }
                    
                    BagItemArray[index] = cardItem;
                    

                    break;
                }
            }
        }

        return index;
    }

    /// <summary>
    /// 获取次序位置
    /// </summary>
    /// <param name="Index"></param>
    /// <returns></returns>
    private Vector2 GetIndexPos(int Index)
    {
        Vector2 zeroVector2 = Root.localPosition;

        int MaxBagCount = BagItemArray.Length;
        float MaxBagCountHalf = MaxBagCount / 2f;

        Vector2 firstVector2 = new Vector2(zeroVector2.x - MaxBagCountHalf * GameDefine.GameCardItemSize + GameDefine.GameCardItemSize / 2f, zeroVector2.y);

        Vector2 curVector2 = new Vector2(firstVector2.x + Index * GameDefine.GameCardItemSize, firstVector2.y);
        
        return curVector2;
    }
    
}