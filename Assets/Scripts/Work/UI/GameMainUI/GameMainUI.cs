using System;
using System.Collections.Generic;
using UnityEngine;

public class GameMainUI : UIBaseView
{
    protected override UIType Type => UIType.GameMainUI;
    protected override string LoadPath => "";

    private Transform m_Root;

    private Dictionary<int, GameCardItem[][]> m_ItemsMapLayerDict;

    private GameCardEditorConfig m_EditorConfig;
    private void Awake()
    {
        m_Root = transform.Find("Entity/Root");
    }

    private void Start()
    {
        
    }
    
    public override void Show(object[] parameter)
    {
        base.Show(parameter);
        
        int level = PlayerSaveUtility.m_SaveData.MaxLevel;
        
        m_EditorConfig = PrototypeUtility.GameCardEditorConfig;
        m_ItemsMapLayerDict = new Dictionary<int, GameCardItem[][]>();
        
        GameLevelUtility.CreateGrid(m_Root, m_EditorConfig.GetLevelConfig(level), ref m_ItemsMapLayerDict);
    }
}