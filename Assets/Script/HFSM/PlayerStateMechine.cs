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
    [Header("补偿速度")]
    public float lightSpeed = 0.8f;
    [Header("打击感")]
    public float shakeTime = 0.15f;
    public int lightPause = 5;
    public float lightStrength = 0.06f;
    [Space]

    //输入水平轴的值 作为成员 后面使用
    public float input_x;
    //输入竖直轴的值 作为成员 后面使用
    public float input_y;
    //获取刚体
    public Rigidbody2D rigidbody2D;
    //速度
    public float speed = 8.0f;
    //跳跃速度
    public float JumpSpeed = 10.0f;
    //滑铲速度
    public float slideSpeed = 10.0f;
    //滑铲时间
    public float slideTime = 2.0f;
    //起身速度
    public float standSpeed = 10.0f;
    //头部碰撞器,用于实现滑铲时开关
    public Collider2D headCollider2D;
    //头顶transform,用于判断是否可以起身
    public Transform celling;
    //地层layer
    public LayerMask groundLayer;
    //获取动画对象
    public Animator animator;
    //是否在攻击状态
    public bool isAttack = false;
    //获取子对象， 攻击
    public Transform attackTF;
    //声明当前连击数
    public int comboStep;
    //允许combo的时间，在此时间内按下键才可以连击
    public float interval = 1.0f;
    //计时器
    public float timer;
    //区分轻重攻击
    public string attackType;
    //功能和groundTf相同，判断是否在地面
    [SerializeField] private Vector3 check;
    //maxHP
    public int maxHealth;
    // curHp
    public int curHealth;
    //是否死亡
    public bool isDead;
    //是否遭受攻击
    public bool isHit;
    //被击方向
    public Vector2 direction;
    //被击退的速度
    public float hitSpeed;
    //与墙体的关系
    public Collision coll;
    //是否可以移动
    public bool canMove;
    //跳跃相关
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    //是否冲刺
    public bool isDashing = false;
}

public class PlayerStateMechine : MonoBehaviour
{

    [SerializeField] private PlayerParameter parameter = new PlayerParameter();           //声明玩家参数类
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
        groundState.AddState(PlayerState.IDLE, new Idle(animator, true));   //添加Idle状态
        groundState.AddState(PlayerState.RUN, new Run(transform, animator, OnInputChanged, true)); //添加 Run状态

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
