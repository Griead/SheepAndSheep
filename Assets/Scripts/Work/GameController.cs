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
        UIUtility.LoadUIView<StartMainUI>(UIType.StartMainUI, null);
    }
}