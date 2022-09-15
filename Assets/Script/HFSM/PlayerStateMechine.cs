using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;


public enum PlayerState
{
    IDLE,
    RUN,
}

public enum SuperState
{
    NULL,
    GROUND
}


public class PlayerParameter
{
    [Header("�����ٶ�")]
    public float lightSpeed = 0.8f;
    [Header("�����")]
    public float shakeTime = 0.15f;
    public int lightPause = 5;
    public float lightStrength = 0.06f;
    [Space]

    //����ˮƽ���ֵ ��Ϊ��Ա ����ʹ��
    public float input_x;
    //������ֱ���ֵ ��Ϊ��Ա ����ʹ��
    public float input_y;
    //��ȡ����
    public Rigidbody2D rigidbody2D;
    //�ٶ�
    public float speed = 8.0f;
    //��Ծ�ٶ�
    public float JumpSpeed = 10.0f;
    //�����ٶ�
    public float slideSpeed = 10.0f;
    //����ʱ��
    public float slideTime = 2.0f;
    //�����ٶ�
    public float standSpeed = 10.0f;
    //ͷ����ײ��,����ʵ�ֻ���ʱ����
    public Collider2D headCollider2D;
    //ͷ��transform,�����ж��Ƿ��������
    public Transform celling;
    //�ز�layer
    public LayerMask groundLayer;
    //��ȡ��������
    public Animator animator;
    //�Ƿ��ڹ���״̬
    public bool isAttack = false;
    //��ȡ�Ӷ��� ����
    public Transform attackTF;
    //������ǰ������
    public int comboStep;
    //����combo��ʱ�䣬�ڴ�ʱ���ڰ��¼��ſ�������
    public float interval = 1.0f;
    //��ʱ��
    public float timer;
    //�������ع���
    public string attackType;
    //���ܺ�groundTf��ͬ���ж��Ƿ��ڵ���
    [SerializeField] private Vector3 check;
    //maxHP
    public int maxHealth;
    // curHp
    public int curHealth;
    //�Ƿ�����
    public bool isDead;
    //�Ƿ����ܹ���
    public bool isHit;
    //��������
    public Vector2 direction;
    //�����˵��ٶ�
    public float hitSpeed;
    //��ǽ��Ĺ�ϵ
    public Collision coll;
    //�Ƿ�����ƶ�
    public bool canMove;
    //��Ծ���
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    //�Ƿ���
    public bool isDashing = false;
}

public class PlayerStateMechine : MonoBehaviour
{

    [SerializeField] private PlayerParameter parameter = new PlayerParameter();           //������Ҳ�����
    [SerializeField] private float input_x;
    [SerializeField] private Animator animator;

    private StateMachine<SuperState, string> fsm;
    private StateMachine<SuperState, PlayerState, string> groundState;

    


    private void Start()
    {
        //parameter.animator = GetComponent<Animator>();
        //parameter.rigidbody2D = GetComponent<Rigidbody2D>();
        //parameter.attackTF = transform.Find("attack");
        //parameter.coll = GetComponent<Collision>();

        //ground Super State
        groundState = new StateMachine<SuperState, PlayerState, string>();
        groundState.AddState(PlayerState.IDLE, new Idle(animator, true));   //���Idle״̬
        groundState.AddState(PlayerState.RUN, new Run(transform, animator, OnInputChanged, true)); //��� Run״̬

        //Idle->walk input_x > 0 && 
        groundState.AddTransition(PlayerState.IDLE, PlayerState.RUN,
            transition => input_x != 0);
        groundState.AddTransition(PlayerState.RUN, PlayerState.IDLE,
            transition => input_x < 0);
        //groundState.AddTransition(PlayerState.IDLE, PlayerState.RUN);
        //groundState.AddTransition(PlayerState.RUN, PlayerState.IDLE);


        fsm = new StateMachine<SuperState, string>();
        fsm.AddState(SuperState.GROUND, groundState);

        fsm.Init();
    }


    private void Update()
    {
        input_x = Input.GetAxis("Horizontal");
        fsm.OnLogic();
    }

    private void OnInputChanged(float input_x)
    {
        parameter.input_x = input_x;
    }
}
