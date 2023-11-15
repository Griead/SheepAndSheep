using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameCardItem : MonoBehaviour
{
    private void Awake()
    {
        
    }

    private void Start()
    {
        
    }
    
    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="data"></param>
    private void SetData(GameCardData data)
    {
        
    }

    /// <summary>
    /// 进入背包请求发送
    /// </summary>
    private void SendEnterBag()
    {
        //向背包发送请求
    }

    /// <summary>
    /// 接收背包消息
    /// </summary>
    private void ReceiveBag(Vector3 aimPos)
    {
        transform.DOLocalMove(aimPos, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            SendBagCheckTriple();
        });
    }

    /// <summary>
    /// 发送背包检测三连
    /// </summary>
    private void SendBagCheckTriple()
    {
        
    }

    /// <summary>
    /// 接受到三连的请求
    /// </summary>
    public void Triple()
    {
        CardDestroy();
    }

    /// <summary>
    /// 卡牌三连销毁
    /// </summary>
    private void CardDestroy()
    {
        
    }

    private void Update()
    {
        
    }

    private void OnDestroy()
    {
        
    }
}
