using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace enemy
{
    //状态枚举
    public enum StateType
    {
        Idle, React, Dead, Hit, Chase, Patrol, Attack
    }

    [Serializable]
    public class Parameter
    {
        //HP
        public float health;
        //移动速度
        public float moveSpeed;
        //追击速度
        public float chaseSpeed;
        //停止时间
        public float idleTime;
        //巡逻范围, 两个TransForm的数组
        public Transform[] patrolPoints;
        //追击范围
        public Transform[] chasePoints;
        //动画器
        public Animator animator;
        //刚体
        public Rigidbody2D rigidbody2D;
        //碰撞体
        public Collider2D collider2D;
        //攻击判定圆心
        public Transform attackPoints;
        //攻击判定半径
        public float attackArea;
        //攻击判定层
        public LayerMask targetLayer;
        //声明检测的对象
        public Transform target;
        //是否被击中
        public bool isHit = false;
        //销毁时间
        public float destroyTime;
        //被击退的速度
        public float hitSpeed;
        //被击方向
        public Vector2 direction;
        //被击中效果
        public Animator hitAdnimator;
        //是否死亡
        public bool isDead;

    }

    public class MyFSM : MonoBehaviour
    {

        //当前状态
        private IState curState;
        //状态字典
        private Dictionary<StateType, IState> states = new Dictionary<StateType, IState>();
        //声明敌人参数类
        public Parameter parameter = new Parameter();
        //死亡属性
        public bool isDead
        {
            get { return parameter.isDead; }
        }
        // Start is called before the first frame update
        void Awake()
        {
            //字典中添加对应的键值对
            states.Add(StateType.Idle, new enemy.IdleState(this));
            states.Add(StateType.React, new enemy.ReactState(this));
            states.Add(StateType.Dead, new enemy.DeadState(this));
            states.Add(StateType.Hit, new enemy.HitState(this));
            states.Add(StateType.Chase, new enemy.ChaseState(this));
            states.Add(StateType.Patrol, new enemy.PatrolState(this));
            states.Add(StateType.Attack, new enemy.AttackState(this));

            parameter.animator = GetComponent<Animator>();
            parameter.rigidbody2D = GetComponent<Rigidbody2D>();
            parameter.hitAdnimator = transform.GetChild(2).GetComponent<Animator>();
            parameter.collider2D = GetComponent<Collider2D>();

            //开始时将Enmey的状态置为Idle
            TransitionState(StateType.Idle);
        }

        // Update is called once per frame
        void Update()
        {

            if (Input.GetKey(KeyCode.P))
            {
                parameter.isHit = true;
            }
            curState.OnUpdate();
        }

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

        //碰撞检测，发现玩家
        private void OnTriggerEnter2D(Collider2D other)
        {
            //Debug.Log(other);
            if (other != null)
            {
                //Debug.Log($"Area detect {other.transform.name}");
                //判断检测到的物体是否为玩家
                if (other.CompareTag("Player"))
                {

                    //是则获取玩家的tranform
                    parameter.target = other.transform;
                    //TransitionState(StateType.Chase);
                }
            }
        }



        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                parameter.target = null;
            }
        }

        //在窗口中绘制攻击范围圆心
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(parameter.attackPoints.position, parameter.attackArea);
        }
        //销毁死亡敌人目标
        public void DestroyObj()
        {
            Destroy(gameObject);
        }
        //被击接口，提供给外部调用
        public void GetHit(Vector2 direction)
        {
            if (!parameter.isDead)
            {
                //设置朝向为direction（攻击来源）的反方向
                transform.localScale = new Vector3(-direction.x, 1, 1);
                parameter.isHit = true;
                //Enemy朝向攻击来源方向
                parameter.direction = direction;
                //切换为被击状态
                TransitionState(StateType.Hit);
                parameter.hitAdnimator.SetTrigger("Hit");
            }
        }
    }
}
