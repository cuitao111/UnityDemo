using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace player
{
    //状态枚举
    public enum StateType
    {
        Idle, Run, Jump, Fall, Atk, Slide, Dead, Hit, Stand, WallGrab, WallSlide, WallClimb, WallJump
    }

    [Serializable]
    public class Parameter
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
        //[SerializeField] private LayerMask layer;
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

    public class MyFSM : MonoBehaviour
    {

        //当前状态
        private IState curState;
        //状态字典
        private Dictionary<StateType, IState> states = new Dictionary<StateType, IState>();
        //声明玩家参数类
        public Parameter parameter = new Parameter();
        //击中属性
        public bool isHit
        {
            get
            {
                return parameter.isHit;
            }

            set
            {
                parameter.isHit = value; 
            }
        }
        // Start is called before the first frame update
        void Awake()
        {
            //字典中添加对应的键值对
            states.Add(StateType.Idle, new player.IdleState(this));
            states.Add(StateType.Run, new player.RunState(this));
            states.Add(StateType.Jump, new player.JumpState(this));
            states.Add(StateType.Fall, new player.FallState(this));
            states.Add(StateType.Atk, new player.AtkState(this));
            states.Add(StateType.Dead, new player.DeadState(this));
            states.Add(StateType.Hit, new player.HitState(this));
            states.Add(StateType.Slide, new player.SlideState(this));
            states.Add(StateType.Stand, new player.StandState(this));
            states.Add(StateType.WallGrab, new player.WallGrabState(this));
            states.Add(StateType.WallSlide, new player.WallSlideState(this));
            states.Add(StateType.WallClimb, new player.WallClimbState(this));

            parameter.animator = GetComponent<Animator>();
            parameter.rigidbody2D = GetComponent<Rigidbody2D>();
            parameter.attackTF = transform.Find("attack");
            parameter.coll = GetComponent<Collision>();

            //开始时将Enmey的状态置为Idle
            TransitionState(StateType.Idle);
        }

        // Update is called once per frame
        void Update()
        {
            curState.OnUpdate();
        }

        private void LateUpdate()
        {
            if (parameter.isAttack)
            {
                return;
            }
            //先不写，看是否有问题
        }

        //状态转换
        public void TransitionState(StateType state)
        {
            if (curState != null)
            {
                //如果当前状态不为空，退出状态
                curState.OnExit();
            }

            //找到对应的状态赋值给curState
            curState = states[state];

            //执行新状态的OnEnter
            curState.OnEnter();
        }

        //转换朝向
        public void FlipTo(Transform target)
        {
            if (target != null)
            {
                //判断当前（敌人）的x轴位置与target位置
                if (transform.position.x > target.position.x)
                {
                    //play在enemy左侧，更新transform的朝向
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                else if (transform.position.x < target.position.x)
                {
                    //朝向右侧
                    transform.localScale = new Vector3(1, 1, 1);
                }
            }

        }

        //协程实现时停
        private void SlowTime(float timer)
        {
            StopAllCoroutines();
            StartCoroutine(SlowTimeCo(timer));
        }

        public IEnumerator SlowTimeCo(float timer)
        {
            Time.timeScale = 0.25f;//修改时间
            while (timer >= 0)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    break;
                }
                yield return null;
            }
            Time.timeScale = 1;
        }

        //碰撞检测，弹反攻击
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other != null)
            {
                //如果获取到了敌人
                if (other.CompareTag("Enemy") && !other.GetComponent<enemy.MyFSM>().isDead)
                {
                    if (parameter.attackType == "Light")
                    {
                        CameraShake.Instance.DoShake(parameter.lightStrength, parameter.lightSpeed);
                        AttackScene.Instance.HitPause(parameter.lightPause);
                    }
                    //CameraShake.Instance.DoShake(0.06f, 0.35f);

                    //传递玩家的朝向，调用敌人被击中的逻辑
                    if (transform.localScale.x > 0)
                    {
                        //玩家朝右侧击打
                        other.GetComponent<enemy.MyFSM>().GetHit(Vector2.right);
                    }
                    else if (transform.localScale.x < 0)
                    {
                        other.GetComponent<enemy.MyFSM>().GetHit(Vector2.left);
                    }

                }
                if (parameter.isHit)
                {
                    //如果被击中，直接返回，不调用抖动
                    return;
                }
                //检测到bullet逻辑
                if (other.CompareTag("Bullet") &&
                    transform.position.x > other.transform.position.x)
                {
                    //抖动摄像机
                    //CameraShake.Instance.DoShake(0.06f, 0.35f);
                    CameraShake.Instance.DoShake(parameter.lightStrength, parameter.lightSpeed);
                    AttackScene.Instance.HitPause(parameter.lightPause);
                    //bullet转向
                    other.GetComponent<Bullet>().Flip();
                    //添加时停效果
                    SlowTime(0.15f);
                }
            }
        }

        
        //销毁目标
        public void DestroyObj()
        {
            Destroy(this);
        }
        //被击接口，提供给外部调用
        public void GetHit(Vector2 direction)
        {
            if (!parameter.isDead)
            {
                //设置朝向为direction（攻击来源）的反方向
                transform.localScale = new Vector3(-direction.x, 1, 1);
                //Enemy朝向攻击来源方向
                parameter.direction = direction;
                //切换为被击状态
                TransitionState(StateType.Hit);
            }
        }

        


        public bool CollectCherry()
        {
            if(parameter.curHealth < parameter.maxHealth)
            {
                ChangeHealth(-1);
                return true;
            }
            return false;
        }

        public void ChangeHealth(int value)
        {
            parameter.curHealth += value;

            UIHealthBar ui = UIHealthBar.Instance;
            //设置血量
            UIHealthBar.Instance.SetValue(parameter.curHealth /(float) parameter.maxHealth);
        }
    }
}
