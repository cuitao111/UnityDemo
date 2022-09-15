using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace enemy
{
    public class IdleState : IState
    {

        private enemy.MyFSM manager;
        private enemy.Parameter parameter;
        //到巡逻点idle一段时间
        private float timer;

        public IdleState(MyFSM manager)
        {
            //根据传入的manager 初始化
            this.manager = manager;
            parameter = manager.parameter;
        }

        public void OnEnter()
        {
            //播放idle动画
            parameter.animator.Play("idle");
        }

        public void OnUpdate()
        {
            //idle时被击中
            if (parameter.isHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            //更新计时器
            timer += Time.deltaTime;
            //如果发现了目标 且目标在追击范围内 切换到反应状态
            if (parameter.target != null &&
                parameter.target.position.x > parameter.chasePoints[0].position.x &&
                parameter.target.position.x < parameter.chasePoints[1].position.x)
            {
                //Debug.Log(parameter.target);
                manager.TransitionState(StateType.React);
            }

            //如果计时器小于0
            if (timer >= parameter.idleTime)
            {
                //进入patrol状态
                manager.TransitionState(StateType.Patrol);
            }
        }

        public void OnExit()
        {
            timer = 0;
        }
    }


    //发现玩家反应
    public class ReactState : IState
    {

        private MyFSM manager;
        private Parameter parameter;

        //声明动画播放的进度
        private AnimatorStateInfo info;

        public ReactState(MyFSM manager)
        {
            //根据传入的manager 初始化
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public void OnEnter()
        {
            parameter.animator.Play("react");
        }

        public void OnUpdate()
        {
            //react时被击中
            if (parameter.isHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            //反应时朝向玩家
            manager.FlipTo(parameter.target);
            //实时获取动画状态到info中
            info = parameter.animator.GetCurrentAnimatorStateInfo(0);
            //如果已经完成了95%，认为已完成
            if (info.normalizedTime >= 0.95f)
            {
                manager.TransitionState(StateType.Chase);
            }
        }

        public void OnExit()
        {

        }
    }


    //敌人死亡
    public class DeadState : IState
    {

        private MyFSM manager;
        private Parameter parameter;

        private float timer;
        public DeadState(MyFSM manager)
        {
            //根据传入的manager 初始化
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public void OnEnter()
        {
            parameter.animator.Play("dead");
            parameter.isDead = true;
        }

        public void OnUpdate()
        {
            timer += Time.deltaTime;
            if (timer >= parameter.destroyTime)
            {
                manager.DestroyObj();
            }
        }

        public void OnExit()
        {

        }
    }



    //敌人被击中
    public class HitState : IState
    {

        private enemy.MyFSM manager;
        private Parameter parameter;
        //播放进度
        private AnimatorStateInfo info;

        public HitState(MyFSM manager)
        {
            //根据传入的manager 初始化
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public void OnEnter()
        {
            parameter.animator.Play("hit");
            //血量减少
            parameter.health--;
        }

        public void OnUpdate()
        {
            //获取实时进度
            info = parameter.animator.GetCurrentAnimatorStateInfo(0);
            //Enemy被击退
            parameter.rigidbody2D.velocity = parameter.hitSpeed * parameter.direction;

            if (parameter.health <= 0)
            {
                //血量为0，执行Dead
                manager.TransitionState(StateType.Dead);
            }
            else if (info.normalizedTime >= 0.95)
            {
                //执行完毕，锁定玩家并追击
                manager.FlipTo(parameter.target);
                //如果此时没有target会报错
                //manager.transform.position = Vector2.MoveTowards(manager.transform.position,
                //    parameter.target.position, parameter.chaseSpeed * Time.deltaTime);
                parameter.target = GameObject.FindWithTag("Player").transform;
                manager.TransitionState(StateType.Chase);
            }
        }

        public void OnExit()
        {
            parameter.isHit = false;
        }
    }

    //敌人追击
    public class ChaseState : IState
    {

        private MyFSM manager;
        private Parameter parameter;

        public ChaseState(MyFSM manager)
        {
            //根据传入的manager 初始化
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public void OnEnter()
        {
            parameter.animator.Play("walk");
        }

        public void OnUpdate()
        {
            //chase时被击中
            if (parameter.isHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            //Flipto始终朝向玩家manager.target
            manager.FlipTo(parameter.target);

            //若丢了目标或者超出追击范围
            if (parameter.target == null ||
                manager.transform.position.x < parameter.chasePoints[0].position.x ||
                manager.transform.position.x > parameter.chasePoints[1].position.x)
            {
                manager.TransitionState(StateType.Idle);
            }
            else
            {
                manager.transform.position = Vector2.MoveTowards(manager.transform.position, parameter.target.position,
                parameter.chaseSpeed * Time.deltaTime);

                //如果靠target够近，且处于追击状态，切换到攻击状态
                //圆形范围检测player层的碰撞体
                Collider2D col = Physics2D.OverlapCircle(parameter.attackPoints.position,
                    parameter.attackArea, parameter.targetLayer);
                if (col != null)
                {
                    //Debug.Log($"attack detect{col.transform.name}");
                    if (col.CompareTag("Player"))
                    {
                        manager.TransitionState(StateType.Attack);
                    }
                }
            }
        }

        public void OnExit()
        {

        }
    }


    //敌人巡逻
    public class PatrolState : IState
    {

        private MyFSM manager;
        private Parameter parameter;
        //巡逻点下标
        private int patrolIndex;

        public PatrolState(MyFSM manager)
        {
            //根据传入的manager 初始化
            this.manager = manager;
            this.parameter = manager.parameter;
            patrolIndex = 0;
        }

        public void OnEnter()
        {
            parameter.animator.Play("walk");
        }

        public void OnUpdate()
        {
            //巡逻时被击中
            if (parameter.isHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            //如果发现了目标 且目标在追击范围内 切换到反应状态
            if (parameter.target != null &&
                parameter.target.position.x > parameter.chasePoints[0].position.x &&
                parameter.target.position.x < parameter.chasePoints[1].position.x)
            {
                manager.TransitionState(StateType.React);
            }
            //使enemy始终朝向当前巡逻点
            manager.FlipTo(parameter.patrolPoints[patrolIndex]);
            //更新enemy位置
            manager.transform.position = Vector2.MoveTowards(manager.transform.position,
                parameter.patrolPoints[patrolIndex].position, parameter.moveSpeed * Time.deltaTime);
            //如果移动到index对应的巡逻点，切换到idle
            //距离很近可以认为，移动到了对应的点
            float distance = Vector2.Distance(manager.transform.position, parameter.patrolPoints[patrolIndex].position);
            //Debug.Log($"distacne:{distance}");
            if (distance < 1.5f)
            {
                //切换到idle
                manager.TransitionState(StateType.Idle);
            }
        }

        public void OnExit()
        {
            patrolIndex++;
            if (patrolIndex >= parameter.patrolPoints.Length)
            {
                patrolIndex = 0;
            }
        }
    }

    //敌人攻击
    public class AttackState : IState
    {

        private MyFSM manager;
        private Parameter parameter;
        //动画百分比
        private AnimatorStateInfo info;

        public AttackState(MyFSM manager)
        {
            //根据传入的manager 初始化
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public void OnEnter()
        {
            parameter.animator.Play("atk");
        }

        public void OnUpdate()
        {
            //攻击时被击中
            if (parameter.isHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            info = parameter.animator.GetCurrentAnimatorStateInfo(0);

            if (info.normalizedTime >= 0.95)
            {
                manager.TransitionState(StateType.Chase);
            }
        }

        public void OnExit()
        {

        }
    }

}


