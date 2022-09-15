using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    //用于判断与墙体地面的关系
    [Header("Layer")]
    public LayerMask groundLayer; //墙体所在层

    [Space]
    public bool onGround; //是否在地上
    public bool onWall;  //是否在墙上
    public bool onRightWall;  //右墙
    public bool onLeftWall;  //左墙
    public int wallSide;

    [Space]

    [Header("Collision")]
    public Transform groundTF;
    //用于判断左右两侧是否接触墙体
    public Vector3 wallOffset;
    public float collisionRadius = 0.25f; //判断距离

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //更新与墙体的关系
        onGround = Physics2D.OverlapCircle(groundTF.position, collisionRadius, groundLayer);

        onLeftWall = Physics2D.OverlapCircle(transform.position - wallOffset, collisionRadius, groundLayer);
        onRightWall = Physics2D.OverlapCircle(transform.position + wallOffset, collisionRadius, groundLayer);
        onWall = onLeftWall || onRightWall;
        
        wallSide = onRightWall ? 1 : -1;
    }

    //在窗口中绘制攻击范围圆心，与左右触墙判定
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(groundTF.position, collisionRadius);
        //Gizmos.DrawWireSphere(celling.position, 1f);// MyFSM中用于判定是否可以起身的TF
        Gizmos.DrawWireSphere(transform.position - wallOffset, collisionRadius);
        Gizmos.DrawWireSphere(transform.position + wallOffset, collisionRadius);

    }
}
