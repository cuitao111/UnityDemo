using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    //获得摄像机的x轴增幅添加到要移动的背景上
    public Transform Cam;           //摄像机的位置
    public float moveRate;          //前中后背景的移动比率
    private float startPoint;       //要添加组件的物体的开始的位置
    // Start is called before the first frame update
    void Start()
    {
        startPoint = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(startPoint + Cam.position.x * moveRate, transform.position.y);
    }

    public void ResetPos()
    {
        transform.position = new Vector2(startPoint, transform.position.y);
    }
}
