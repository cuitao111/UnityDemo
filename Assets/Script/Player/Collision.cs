using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    //�����ж���ǽ�����Ĺ�ϵ
    [Header("Layer")]
    public LayerMask groundLayer; //ǽ�����ڲ�

    [Space]
    public bool onGround; //�Ƿ��ڵ���
    public bool onWall;  //�Ƿ���ǽ��
    public bool onRightWall;  //��ǽ
    public bool onLeftWall;  //��ǽ
    public int wallSide;

    [Space]

    [Header("Collision")]
    public Transform groundTF;
    //�����ж����������Ƿ�Ӵ�ǽ��
    public Vector3 wallOffset;
    public float collisionRadius = 0.25f; //�жϾ���

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //������ǽ��Ĺ�ϵ
        onGround = Physics2D.OverlapCircle(groundTF.position, collisionRadius, groundLayer);

        onLeftWall = Physics2D.OverlapCircle(transform.position - wallOffset, collisionRadius, groundLayer);
        onRightWall = Physics2D.OverlapCircle(transform.position + wallOffset, collisionRadius, groundLayer);
        onWall = onLeftWall || onRightWall;
        
        wallSide = onRightWall ? 1 : -1;
    }

    //�ڴ����л��ƹ�����ΧԲ�ģ������Ҵ�ǽ�ж�
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(groundTF.position, collisionRadius);
        //Gizmos.DrawWireSphere(celling.position, 1f);// MyFSM�������ж��Ƿ���������TF
        Gizmos.DrawWireSphere(transform.position - wallOffset, collisionRadius);
        Gizmos.DrawWireSphere(transform.position + wallOffset, collisionRadius);

    }
}
