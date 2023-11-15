using UnityEngine;

public class GameLevelManager :MonoBehaviour
{
    public Transform m_Root;

    public GameObject m_GridPrefab;

    /// <summary>
    /// 当前的关卡配置
    /// </summary>
    public GameLevelConfig m_CurConfig;

    /// <summary>
    /// 首先生成格子
    /// </summary>
    public void CreateGrid()
    {
        
    }
}