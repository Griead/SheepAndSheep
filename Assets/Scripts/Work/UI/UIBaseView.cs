using UnityEngine;

public abstract class UIBaseView : MonoBehaviour
{
    protected abstract UIType Type { get; }
    protected abstract string LoadPath { get; }

    public virtual void Show(object[] parames)
    {
        
    }
}