using System;
using UnityEngine.UI;

public class EndMainUI : UIBaseView
{
    protected override UIType Type => UIType.EndMainUI;
    protected override string LoadPath => "UI/EndMainUI";

    private Button m_NextLevelBtn;

    private Text m_NextLevelText;
    
    private void Awake()
    {
        m_NextLevelBtn = transform.Find("Entity/Button").GetComponent<Button>();
        m_NextLevelText = m_NextLevelBtn.transform.Find("Text").GetComponent<Text>();
    }

    private void Start()
    {
        m_NextLevelBtn.onClick.AddListener(NextLevel);
    }

    public override void Show(object[] parameter)
    {
        base.Show(parameter);
        int level = PlayerSaveUtility.m_SaveData.MaxLevel;
        m_NextLevelText.text =$"第{level}关";
        
        AudioManager.Instance.PlaySound(GameDefine.UIAudio_Success);
    }

    private void NextLevel()
    {
        //音效
        AudioManager.Instance.PlaySound(GameDefine.UIAudio_TabClick);
        
        int level = PlayerSaveUtility.m_SaveData.MaxLevel;
        // var config = PrototypeUtility.GameCardEditorConfig.GetLevelConfig(level + 1);
        UIUtility.ReleaseUIView(UIType.EndMainUI);
        UIUtility.LoadUIView<GameMainUI>(UIType.GameMainUI, new object[] {level});
    }
}