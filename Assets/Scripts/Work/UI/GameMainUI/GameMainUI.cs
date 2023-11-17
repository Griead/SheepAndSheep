using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class GameMainUI : UIBaseView
{
    protected override UIType Type => UIType.GameMainUI;
    protected override string LoadPath => "";

    private Transform m_Root;

    private GameCardEditorConfig m_EditorConfig;

    private GameCardBagItem m_BagItem;

    private RectTransform m_EffectRoot;

    private GameObject m_SnowPrefab;

    private UnityEngine.UI.Button m_CloudBtn;

    private bool m_IsInit = false;
    
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        m_Root = transform.Find("Entity/Root");
        m_BagItem = transform.Find("Entity/GameCardBagItem").GetComponent<GameCardBagItem>();
        m_EffectRoot = transform.Find("Entity/EffectRoot").GetComponent<RectTransform>();
        m_CloudBtn = transform.Find("Entity/Cloud").GetComponent<UnityEngine.UI.Button>();

        m_IsInit = true;
    }
    

    private void Start()
    {
        m_SnowPrefab = Resources.Load<GameObject>("Prefab/SnowPrefab");
        m_CloudBtn.onClick.AddListener(ClearBag);
    }
    
    public override void Show(object[] parameter)
    {
        base.Show(parameter);
        
        if(!m_IsInit)
            Init();
        
        // int level = PlayerSaveUtility.m_SaveData.MaxLevel;
        int level = (int)parameter[0];
        
        m_EditorConfig = PrototypeUtility.GameCardEditorConfig;
        var levelConfig = m_EditorConfig.GetLevelConfig(level);
        
        //设置背包数据
        m_BagItem.SetData(levelConfig.MaxBagItemCount);
        
        GameLevelUtility.CreateGrid(m_Root, levelConfig, m_BagItem);
    }

    private float duringTime = 0;
    private void Update()
    {
        duringTime += Time.deltaTime;
        if (duringTime > 3)
        {
            duringTime = 0;
            SetEffect();
        }
    }

    private void SetEffect()
    {
        //雪-----------------
        float minX = -540f;
        float maxX = 540f;
        float fallDuration = 50f;

        //初始
        GameObject snowflake = Instantiate(m_SnowPrefab, m_EffectRoot);
        var canvasGroup = snowflake.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        
        //生成位置
        Vector3 randomPosition = new Vector3(Random.Range(minX, maxX), Random.Range(-10f, 10f), 0f);
        snowflake.transform.localPosition = randomPosition;

        //旋转
        int randomRotateSpeed = Random.Range(30, 55);
        snowflake.transform.DOLocalRotate(new Vector3(0f, 0f, 360), randomRotateSpeed, RotateMode.LocalAxisAdd);
            
        //渐显
        canvasGroup.DOFade(1, 0.8f);
        
        // 随机生成下落时间
        float randomDuration = Random.Range(fallDuration * 0.8f, fallDuration * 1.2f);

        //随机位置
        int randomY = Random.Range(-1400, -2400);
        float endY = snowflake.GetComponent<RectTransform>().anchoredPosition.y + randomY;
        
        // 下落动画使用DOTween
        snowflake.transform.DOLocalMoveY(endY, randomDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => { Destroy(snowflake); });
    }

    private void ClearBag()
    {
        //音效
        AudioManager.Instance.PlaySound(GameDefine.Audio_TabClick);
        
        GameLevelUtility.ClearBag();
    }
}