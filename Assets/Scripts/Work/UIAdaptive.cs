using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAdaptive : MonoBehaviour
{
    public Transform Aim;
    
    void Start()
    {
        var _root = UIUtility.GetRoot().GetComponent<RectTransform>().sizeDelta;
        var _rect = new Vector2(1080, 2400);
        var _widthscale = _root.x / _rect.x;
        float _heightscale = _root.y / _rect.y;
        
        
        float _scale = _widthscale < _heightscale ? _widthscale : _heightscale;
        Aim.localScale *= _scale;
    }
}
