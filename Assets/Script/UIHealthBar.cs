using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    //公有静态属性
    public static UIHealthBar Instance { get; private set; }
    //Image属性mask
    public Image mask;
    //原始长度
    public float originalLen;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //获取原始长度
        originalLen = mask.rectTransform.rect.width;
    }

    public void SetValue(float value)
    {
        //根据传入的value设置mask长度
        mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value * originalLen);
    }
}
