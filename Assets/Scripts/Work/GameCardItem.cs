using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

public class GameCardItem : MonoBehaviour
{
    /// <summary>
    /// 数据
    /// </summary>
    public GameCardConfig m_Config;

    /// <summary>
    /// 配置
    /// </summary>
    public GameCardData m_Data;

    private Image[] m_IconArray;

    private Button m_Button;

    private Transform m_Mask;

    private TweenerCore<Vector3, Vector3, VectorOptions> m_ScalseTween;

    private bool m_Freeze;
    
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

        m_Freeze = false;
    }

    /// <summary>
    /// 设置遮罩
    /// </summary>
    /// <param name="IsMask"></param>
    public void SetMask(bool IsMask)
    {
        if (!IsMask)
        {
            m_Mask.GetComponent<Image>().DOFade(IsMask ? 0.5f : 0, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                m_Mask.gameObject.SetActive(IsMask);
            });
        }
        else
        {
            m_Mask.gameObject.SetActive(IsMask);
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
        if(m_Freeze)
            return;
        
        //音效
        AudioManager.Instance.PlaySound(GameDefine.Audio_ItemClick);
        
        GameLevelUtility.CardEnterBag(this);
    }

    /// <summary>
    /// 接收背包消息
    /// </summary>
    public void ReceiveBag(Vector2 aimPos)
    {
        m_Freeze = true;
        GameLevelUtility.CardItemDelOperate(m_Data.Layer, m_Data.Horizontal, m_Data.Vertical);
        GameLevelUtility.CheckBottomCardMask(m_Data.Layer, m_Data.Horizontal, m_Data.Vertical, false);
        
        transform.DOLocalMove(aimPos, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
        {
            //检测是否胜利
            GameLevelUtility.CheckGameSuccess();
            
            //发送背包检测三连
            GameLevelUtility.CheckTripleCard(this);
        });
    }

    /// <summary>
    /// 接受到三连的请求
    /// </summary>
    public void ReceiveTriple()
    {
        CardDestroy();
    }

    /// <summary>
    /// 卡牌三连销毁
    /// </summary>
    private void CardDestroy()
    {
        if(m_ScalseTween != null)
            return;

        transform.GetComponent<CanvasGroup>().DOFade(0.3f, 0.3f).SetEase(Ease.Linear);

        m_ScalseTween = transform.DOScale(0.3f, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            transform.gameObject.SetActive(false);
        });
    }

    private void Update()
    {
        
    }

    private void OnDestroy()
    {
        
    }
}
