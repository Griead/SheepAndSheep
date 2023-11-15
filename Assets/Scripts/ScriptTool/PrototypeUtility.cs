using UnityEngine;

public static class PrototypeUtility
{
    private static string GameCardEditorConfigPath = "Data/Xml/GameCardConfigEditor";
    public static GameCardEditorConfig GameCardEditorConfig;

    public static void LoadPrototype()
    {
        TextAsset xmlTextAsset = Resources.Load<TextAsset>(GameCardEditorConfigPath);
        GameCardEditorConfig = FileUtility.ReadXmlStringToData<GameCardEditorConfig>(xmlTextAsset.text);
    }
}