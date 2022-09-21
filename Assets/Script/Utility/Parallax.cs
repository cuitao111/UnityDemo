using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    //����������x��������ӵ�Ҫ�ƶ��ı�����
    public Transform Cam;           //�������λ��
    public float moveRate;          //ǰ�к󱳾����ƶ�����
    private float startPoint;       //Ҫ������������Ŀ�ʼ��λ��
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
