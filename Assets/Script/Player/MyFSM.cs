using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace player
{
    //״̬ö��
    public enum StateType
    {
        Idle, Run, Jump, Fall, Atk, Slide, Dead, Hit, Stand, WallGrab, WallSlide, WallClimb, WallJump
    }

    [Serializable]
    public class Parameter
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
        //[SerializeField] private LayerMask layer;
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

    public class MyFSM : MonoBehaviour
    {

        //��ǰ״̬
        private IState curState;
        //״̬�ֵ�
        private Dictionary<StateType, IState> states = new Dictionary<StateType, IState>();
        //������Ҳ�����
        public Parameter parameter = new Parameter();
        //��������
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
            //�ֵ�����Ӷ�Ӧ�ļ�ֵ��
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

            //��ʼʱ��Enmey��״̬��ΪIdle
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
            //�Ȳ�д�����Ƿ�������
        }

        //״̬ת��
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

        //ת������
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

        //Э��ʵ��ʱͣ
        private void SlowTime(float timer)
        {
            StopAllCoroutines();
            StartCoroutine(SlowTimeCo(timer));
        }

        public IEnumerator SlowTimeCo(float timer)
        {
            Time.timeScale = 0.25f;//�޸�ʱ��
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

        //��ײ��⣬��������
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other != null)
            {
                //�����ȡ���˵���
                if (other.CompareTag("Enemy") && !other.GetComponent<enemy.MyFSM>().isDead)
                {
                    if (parameter.attackType == "Light")
                    {
                        CameraShake.Instance.DoShake(parameter.lightStrength, parameter.lightSpeed);
                        AttackScene.Instance.HitPause(parameter.lightPause);
                    }
                    //CameraShake.Instance.DoShake(0.06f, 0.35f);

                    //������ҵĳ��򣬵��õ��˱����е��߼�
                    if (transform.localScale.x > 0)
                    {
                        //��ҳ��Ҳ����
                        other.GetComponent<enemy.MyFSM>().GetHit(Vector2.right);
                    }
                    else if (transform.localScale.x < 0)
                    {
                        other.GetComponent<enemy.MyFSM>().GetHit(Vector2.left);
                    }

                }
                if (parameter.isHit)
                {
                    //��������У�ֱ�ӷ��أ������ö���
                    return;
                }
                //��⵽bullet�߼�
                if (other.CompareTag("Bullet") &&
                    transform.position.x > other.transform.position.x)
                {
                    //���������
                    //CameraShake.Instance.DoShake(0.06f, 0.35f);
                    CameraShake.Instance.DoShake(parameter.lightStrength, parameter.lightSpeed);
                    AttackScene.Instance.HitPause(parameter.lightPause);
                    //bulletת��
                    other.GetComponent<Bullet>().Flip();
                    //���ʱͣЧ��
                    SlowTime(0.15f);
                }
            }
        }

        
        //����Ŀ��
        public void DestroyObj()
        {
            Destroy(this);
        }
        //�����ӿڣ��ṩ���ⲿ����
        public void GetHit(Vector2 direction)
        {
            if (!parameter.isDead)
            {
                //���ó���Ϊdirection��������Դ���ķ�����
                transform.localScale = new Vector3(-direction.x, 1, 1);
                //Enemy���򹥻���Դ����
                parameter.direction = direction;
                //�л�Ϊ����״̬
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
            //����Ѫ��
            UIHealthBar.Instance.SetValue(parameter.curHealth /(float) parameter.maxHealth);
        }
    }
}
