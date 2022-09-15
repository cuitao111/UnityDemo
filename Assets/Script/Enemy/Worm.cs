using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : MonoBehaviour
{
    //攻击Transform
    public Transform attackTf;
    //动画器
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        //初始化动画器
        animator = GetComponent<Animator>();
        //循环调用攻击函数
        InvokeRepeating("Attack", 2, 2);
    }

    public void Attack()
    {
        animator.Play("atk");
    }

    //创建子弹
    public void CreateBullet()
    {
        //使用Instantiate初始化bullet
        GameObject obj = Instantiate(Resources.Load("bullet")) as GameObject;
        obj.transform.position = attackTf.position;
        obj.AddComponent<Bullet>();

    }
}
