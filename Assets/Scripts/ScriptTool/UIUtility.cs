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
    
    public static Transform GetRoot()
    {
        return CanvasRoot;
    }
    
    /// <summary>
    /// 加载视图
    /// </summary>
    /// <param name="type"></param>
    /// <param name="parameter"></param>
    public static void LoadUIView<T>(UIType type, object[] parameter) where T : UIBaseView , new()
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
        
        UIBaseDict[type].Show(parameter);
    }

    /// <summary>
    /// 释放视图
    /// </summary>
    /// <param name="type"></param>
    public static void ReleaseUIView(UIType type)
    {
        if (UIBaseDict.TryGetValue(type, out var view))
        {
            UIBaseDict.Remove(type);
            Object.DestroyImmediate(view.gameObject);
        }
    }
    

    public static T GetUIView<T>(UIType type) where T : UIBaseView
    {
        if (!UIBaseDict.ContainsKey(type))
        {
            Debug.Log("未找到视图");
            return null;
        }
        else
        {
            return UIBaseDict[type] as T;
        }
    }
}