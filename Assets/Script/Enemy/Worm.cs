using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : MonoBehaviour
{
    //����Transform
    public Transform attackTf;
    //������
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        //��ʼ��������
        animator = GetComponent<Animator>();
        //ѭ�����ù�������
        InvokeRepeating("Attack", 2, 2);
    }

    public void Attack()
    {
        animator.Play("atk");
    }

    //�����ӵ�
    public void CreateBullet()
    {
        //ʹ��Instantiate��ʼ��bullet
        GameObject obj = Instantiate(Resources.Load("bullet")) as GameObject;
        obj.transform.position = attackTf.position;
        obj.AddComponent<Bullet>();

    }
}
