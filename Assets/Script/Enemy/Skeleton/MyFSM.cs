using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace enemy
{
    //״̬ö��
    public enum StateType
    {
        Idle, React, Dead, Hit, Chase, Patrol, Attack
    }

    [Serializable]
    public class Parameter
    {
        //HP
        public float health;
        //�ƶ��ٶ�
        public float moveSpeed;
        //׷���ٶ�
        public float chaseSpeed;
        //ֹͣʱ��
        public float idleTime;
        //Ѳ�߷�Χ, ����TransForm������
        public Transform[] patrolPoints;
        //׷����Χ
        public Transform[] chasePoints;
        //������
        public Animator animator;
        //����
        public Rigidbody2D rigidbody2D;
        //��ײ��
        public Collider2D collider2D;
        //�����ж�Բ��
        public Transform attackPoints;
        //�����ж��뾶
        public float attackArea;
        //�����ж���
        public LayerMask targetLayer;
        //�������Ķ���
        public Transform target;
        //�Ƿ񱻻���
        public bool isHit = false;
        //����ʱ��
        public float destroyTime;
        //�����˵��ٶ�
        public float hitSpeed;
        //��������
        public Vector2 direction;
        //������Ч��
        public Animator hitAdnimator;
        //�Ƿ�����
        public bool isDead;

    }

    public class MyFSM : MonoBehaviour
    {

        //��ǰ״̬
        private IState curState;
        //״̬�ֵ�
        private Dictionary<StateType, IState> states = new Dictionary<StateType, IState>();
        //�������˲�����
        public Parameter parameter = new Parameter();
        //��������
        public bool isDead
        {
            get { return parameter.isDead; }
        }
        // Start is called before the first frame update
        void Awake()
        {
            //�ֵ�����Ӷ�Ӧ�ļ�ֵ��
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

            //��ʼʱ��Enmey��״̬��ΪIdle
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
                //�����ǰ״̬��Ϊ�գ��˳�״̬
                curState.OnExit();
            }

            //�ҵ���Ӧ��״̬��ֵ��curState
            curState = states[state];

            //ִ����״̬��OnEnter
            curState.OnEnter();
        }

        public void FlipTo(Transform target)
        {
            if (target != null)
            {
                //�жϵ�ǰ�����ˣ���x��λ����targetλ��
                if (transform.position.x > target.position.x)
                {
                    //play��enemy��࣬����transform�ĳ���
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                else if (transform.position.x < target.position.x)
                {
                    //�����Ҳ�
                    transform.localScale = new Vector3(1, 1, 1);
                }
            }

        }

        //��ײ��⣬�������
        private void OnTriggerEnter2D(Collider2D other)
        {
            //Debug.Log(other);
            if (other != null)
            {
                //Debug.Log($"Area detect {other.transform.name}");
                //�жϼ�⵽�������Ƿ�Ϊ���
                if (other.CompareTag("Player"))
                {

                    //�����ȡ��ҵ�tranform
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

        //�ڴ����л��ƹ�����ΧԲ��
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(parameter.attackPoints.position, parameter.attackArea);
        }
        //������������Ŀ��
        public void DestroyObj()
        {
            Destroy(gameObject);
        }
        //�����ӿڣ��ṩ���ⲿ����
        public void GetHit(Vector2 direction)
        {
            if (!parameter.isDead)
            {
                //���ó���Ϊdirection��������Դ���ķ�����
                transform.localScale = new Vector3(-direction.x, 1, 1);
                parameter.isHit = true;
                //Enemy���򹥻���Դ����
                parameter.direction = direction;
                //�л�Ϊ����״̬
                TransitionState(StateType.Hit);
                parameter.hitAdnimator.SetTrigger("Hit");
            }
        }
    }
}
