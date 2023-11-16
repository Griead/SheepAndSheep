using System;
using System.Collections.Generic;
using UnityEngine;

public class GameMainUI : UIBaseView
{
    protected override UIType Type => UIType.GameMainUI;
    protected override string LoadPath => "";

    private Transform m_Root;

    private Dictionary<int, GameCardItem[][]> m_ItemsMapLayerDict = new Dictionary<int, GameCardItem[][]>() ;

    private GameCardEditorConfig m_EditorConfig = new GameCardEditorConfig();
    private void Awake()
    {
        m_Root = transform.Find("Entity/Root");
    }

    private void Start()
    {
        // m_EditorConfig = PrototypeUtility.GameCardEditorConfig;
    }
    
    public override void Show(object[] parames)
    {
        base.Show(parames);

        int level = (int)parames[0];
        GameLevelUtility.CreateGrid(m_Root, new GameLevelConfig(), ref m_ItemsMapLayerDict);
    }
}