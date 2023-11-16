using System;
using System.Collections.Generic;
using UnityEngine;

public class GameMainUI : UIBaseView
{
    protected override UIType Type => UIType.GameMainUI;
    protected override string LoadPath => "";

    private Transform m_Root;

    private GameCardEditorConfig m_EditorConfig;

    private GameCardBagItem m_BagItem;
    private void Awake()
    {
        m_Root = transform.Find("Entity/Root");
        m_BagItem = transform.Find("Entity/GameCardBagItem").GetComponent<GameCardBagItem>();
    }

    private void Start()
    {
    }
    
    public override void Show(object[] parameter)
    {
        base.Show(parameter);
        
        int level = PlayerSaveUtility.m_SaveData.MaxLevel;
        
        m_EditorConfig = PrototypeUtility.GameCardEditorConfig;
        var levelConfig = m_EditorConfig.GetLevelConfig(level);
        
        //设置背包数据
        m_BagItem.SetData(levelConfig.MaxBagItemCount);
        
        GameLevelUtility.CreateGrid(m_Root, levelConfig, m_BagItem);
    }
}