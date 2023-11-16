using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameCardItem : MonoBehaviour
{
    /// <summary>
    /// 数据
    /// </summary>
    private GameCardConfig m_Config;

    /// <summary>
    /// 配置
    /// </summary>
    private GameCardData m_Data;

    private Image[] m_IconArray;

    private Button m_Button;

    private Transform m_Mask;
    
    private void Awake()
    {
        m_IconArray = transform.Find("IconGroup").GetComponentsInChildren<Image>();
        m_Button = transform.Find("Button").GetComponent<Button>();
        m_Mask = transform.Find("Mask");
    }

    private void Start()
    {
        m_Button.onClick.AddListener(SendEnterBag);
    }
    
    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="config"></param>
    /// <param name="data"></param>
    public void SetData(GameCardConfig config ,GameCardData data)
    {
        m_Config = config;
        m_Data = data;

        for (int i = 0; i < m_IconArray.Length; i++)
        {
            m_IconArray[i].enabled = ((int)config.Type == i);
        }
    }

    public void Refresh()
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
