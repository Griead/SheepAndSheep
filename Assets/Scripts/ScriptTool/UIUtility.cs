using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public static class UIUtility
{
    private static Dictionary<UIType, string> UIPathDict;

    private static Dictionary<UIType, UIBaseView> UIBaseDict;

    private static Transform CanvasRoot;
    
    public static void Init()
    {
        UIPathDict = new Dictionary<UIType, string>();
        UIPathDict.Add(UIType.StartMainUI, "UI/StartMainUI");
        UIPathDict.Add(UIType.EndMainUI, "UI/EndMainUI");
        UIPathDict.Add(UIType.GameMainUI, "UI/GameMainUI");
        
        UIBaseDict = new Dictionary<UIType, UIBaseView>();
    }

    public static void SetRoot(Transform root)
    {
        CanvasRoot = root;
    }
    
    private  static Transform GetRoot()
    {
        return CanvasRoot;
    }
    
    /// <summary>
    /// 加载视图
    /// </summary>
    /// <param name="type"></param>
    /// <param name="parames"></param>
    public static void LoadUIView<T>(UIType type, object[] parames) where T : UIBaseView , new()
    {
        if (!UIPathDict.ContainsKey(type))
        {
            Debug.LogError("未找到加载路径");
            return;
        }
        if (!UIBaseDict.ContainsKey(type))
        {
            var model = Resources.Load<GameObject>(UIPathDict[type]);
            var gameObject = Object.Instantiate(model, GetRoot());
            var uIView = gameObject.AddComponent<T>();
            UIBaseDict.Add(type, uIView);
        }
        UIBaseDict[type].Show(parames);
    }
}