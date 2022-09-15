using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    //���о�̬����
    public static UIHealthBar Instance { get; private set; }
    //Image����mask
    public Image mask;
    //ԭʼ����
    public float originalLen;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //��ȡԭʼ����
        originalLen = mask.rectTransform.rect.width;
    }

    public void SetValue(float value)
    {
        //���ݴ����value����mask����
        mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value * originalLen);
    }
}
