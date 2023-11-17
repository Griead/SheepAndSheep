using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Transform CanvasRoot;
    
    private void Awake()
    {
        GameLevelUtility.Init();
        PlayerSaveUtility.Init();
        PrototypeUtility.Init();
        UIUtility.Init();
        UIUtility.SetRoot(CanvasRoot);
    }
    

    private void Start()
    {
        //背景音乐
        AudioManager.Instance.PlayBackgroundMusic(GameDefine.BackGroupMusic_Triple);
        
        UIUtility.LoadUIView<StartMainUI>(UIType.StartMainUI, null);
    }
}