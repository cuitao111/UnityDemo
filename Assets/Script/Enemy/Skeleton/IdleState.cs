using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace enemy
{
    public class IdleState : IState
    {

        private enemy.MyFSM manager;
        private enemy.Parameter parameter;
        //��Ѳ�ߵ�idleһ��ʱ��
        private float timer;

        public IdleState(MyFSM manager)
        {
            //���ݴ����manager ��ʼ��
            this.manager = manager;
            parameter = manager.parameter;
        }

        public void OnEnter()
        {
            //����idle����
            parameter.animator.Play("idle");
        }

        public void OnUpdate()
        {
            //idleʱ������
            if (parameter.isHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            //���¼�ʱ��
            timer += Time.deltaTime;
            //���������Ŀ�� ��Ŀ����׷����Χ�� �л�����Ӧ״̬
            if (parameter.target != null &&
                parameter.target.position.x > parameter.chasePoints[0].position.x &&
                parameter.target.position.x < parameter.chasePoints[1].position.x)
            {
                //Debug.Log(parameter.target);
                manager.TransitionState(StateType.React);
            }

            //�����ʱ��С��0
            if (timer >= parameter.idleTime)
            {
                //����patrol״̬
                manager.TransitionState(StateType.Patrol);
            }
        }

        public void OnExit()
        {
            timer = 0;
        }
    }


    //������ҷ�Ӧ
    public class ReactState : IState
    {

        private MyFSM manager;
        private Parameter parameter;

        //�����������ŵĽ���
        private AnimatorStateInfo info;

        public ReactState(MyFSM manager)
        {
            //���ݴ����manager ��ʼ��
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public void OnEnter()
        {
            parameter.animator.Play("react");
        }

        public void OnUpdate()
        {
            //reactʱ������
            if (parameter.isHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            //��Ӧʱ�������
            manager.FlipTo(parameter.target);
            //ʵʱ��ȡ����״̬��info��
            info = parameter.animator.GetCurrentAnimatorStateInfo(0);
            //����Ѿ������95%����Ϊ�����
            if (info.normalizedTime >= 0.95f)
            {
                manager.TransitionState(StateType.Chase);
            }
        }

        public void OnExit()
        {

        }
    }


    //��������
    public class DeadState : IState
    {

        private MyFSM manager;
        private Parameter parameter;

        private float timer;
        public DeadState(MyFSM manager)
        {
            //���ݴ����manager ��ʼ��
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



    //���˱�����
    public class HitState : IState
    {

        private enemy.MyFSM manager;
        private Parameter parameter;
        //���Ž���
        private AnimatorStateInfo info;

        public HitState(MyFSM manager)
        {
            //���ݴ����manager ��ʼ��
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public void OnEnter()
        {
            parameter.animator.Play("hit");
            //Ѫ������
            parameter.health--;
        }

        public void OnUpdate()
        {
            //��ȡʵʱ����
            info = parameter.animator.GetCurrentAnimatorStateInfo(0);
            //Enemy������
            parameter.rigidbody2D.velocity = parameter.hitSpeed * parameter.direction;

            if (parameter.health <= 0)
            {
                //Ѫ��Ϊ0��ִ��Dead
                manager.TransitionState(StateType.Dead);
            }
            else if (info.normalizedTime >= 0.95)
            {
                //ִ����ϣ�������Ҳ�׷��
                manager.FlipTo(parameter.target);
                //�����ʱû��target�ᱨ��
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

    //����׷��
    public class ChaseState : IState
    {

        private MyFSM manager;
        private Parameter parameter;

        public ChaseState(MyFSM manager)
        {
            //���ݴ����manager ��ʼ��
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public void OnEnter()
        {
            parameter.animator.Play("walk");
        }

        public void OnUpdate()
        {
            //chaseʱ������
            if (parameter.isHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            //Fliptoʼ�ճ������manager.target
            manager.FlipTo(parameter.target);

            //������Ŀ����߳���׷����Χ
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

                //�����target�������Ҵ���׷��״̬���л�������״̬
                //Բ�η�Χ���player�����ײ��
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


    //����Ѳ��
    public class PatrolState : IState
    {

        private MyFSM manager;
        private Parameter parameter;
        //Ѳ�ߵ��±�
        private int patrolIndex;

        public PatrolState(MyFSM manager)
        {
            //���ݴ����manager ��ʼ��
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
            //Ѳ��ʱ������
            if (parameter.isHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            //���������Ŀ�� ��Ŀ����׷����Χ�� �л�����Ӧ״̬
            if (parameter.target != null &&
                parameter.target.position.x > parameter.chasePoints[0].position.x &&
                parameter.target.position.x < parameter.chasePoints[1].position.x)
            {
                manager.TransitionState(StateType.React);
            }
            //ʹenemyʼ�ճ���ǰѲ�ߵ�
            manager.FlipTo(parameter.patrolPoints[patrolIndex]);
            //����enemyλ��
            manager.transform.position = Vector2.MoveTowards(manager.transform.position,
                parameter.patrolPoints[patrolIndex].position, parameter.moveSpeed * Time.deltaTime);
            //����ƶ���index��Ӧ��Ѳ�ߵ㣬�л���idle
            //����ܽ�������Ϊ���ƶ����˶�Ӧ�ĵ�
            float distance = Vector2.Distance(manager.transform.position, parameter.patrolPoints[patrolIndex].position);
            //Debug.Log($"distacne:{distance}");
            if (distance < 1.5f)
            {
                //�л���idle
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

    //���˹���
    public class AttackState : IState
    {

        private MyFSM manager;
        private Parameter parameter;
        //�����ٷֱ�
        private AnimatorStateInfo info;

        public AttackState(MyFSM manager)
        {
            //���ݴ����manager ��ʼ��
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public void OnEnter()
        {
            parameter.animator.Play("atk");
        }

        public void OnUpdate()
        {
            //����ʱ������
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


