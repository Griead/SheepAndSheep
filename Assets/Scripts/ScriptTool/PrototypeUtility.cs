using UnityEngine;

public static class PrototypeUtility
{
    private static string GameCardEditorConfigPath = "Data/Xml/GameCardConfigPrototype";
    public static GameCardEditorConfig GameCardEditorConfig;

    public static void Init()
    {
        LoadPrototype();
    }
    
    private static void LoadPrototype()
    {
        TextAsset xmlTextAsset = Resources.Load<TextAsset>(GameCardEditorConfigPath);
        GameCardEditorConfig = FileUtility.ReadXmlStringToData<GameCardEditorConfig>(xmlTextAsset.text);
    }
}