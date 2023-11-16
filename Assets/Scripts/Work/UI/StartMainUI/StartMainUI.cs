using System;
using UnityEngine.UI;

public class StartMainUI : UIBaseView
{
    protected override UIType Type => UIType.StartMainUI;
    
    protected override string LoadPath => "UI/StartMainUI";

    private Button StartBtn;

    private Text LevelText;
    
    private int m_Level;
    
    public override void Show(object[] parames)
    {
        base.Show(parames);

        // m_Level = PlayerSaveUtility.m_SaveData.MaxLevel;
        m_Level = 1;
        LevelText.text = $"当前关卡:第{m_Level}关";
    }
    private void Awake()
    {
        StartBtn = transform.Find("Entity/Button").GetComponent<Button>();
        LevelText = transform.Find("Entity/LevelText").GetComponent<Text>();
    }

    private void Start()
    {
        StartBtn.onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        UIUtility.LoadUIView<GameMainUI>(UIType.GameMainUI, new object[]{ 1 });
    }
}