using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

public class ImageShake : MonoBehaviour
{
    private Image m_Image;
    [LabelText("抖动范围")]
    public float m_ShakeDuration = 0.5f;
    [LabelText("抖动间隔")]
    public float m_ShakeInterval = 2f;
    [LabelText("抖动强度")]
    public float m_ShakeStrength = 5;
    [LabelText("抖动幅度")]
    public float m_ShakeVibrato = 20;
    [LabelText("抖动随机程度")]
    public float m_ShakeRandomness = 0;

    private void Awake()
    {
        m_Image = transform.GetComponent<Image>();
    }

    private void Start()
    {
        // 在开始时启动抖动协程
        StartCoroutine(ShakeImageRoutine());
    }

    private IEnumerator ShakeImageRoutine()
    {
        while (true)
        {
            // 使用DOTween进行抖动
            m_Image.rectTransform.DOShakePosition(m_ShakeDuration, strength: 5, vibrato: 20, randomness: 0);

            // 等待一定时间
            yield return new WaitForSeconds(Random.Range(0, m_ShakeInterval));
        }
    }
}