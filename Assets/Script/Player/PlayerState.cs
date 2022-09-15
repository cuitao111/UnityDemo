using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace player
{
    public class IdleState : IState
    {
        //״̬���ƻ�
        private player.MyFSM manager;
        //����
        private player.Parameter parameter;

        public IdleState(MyFSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public virtual void OnEnter()
        {
            parameter.animator.Play("Idle");
        }

        public virtual void OnUpdate()
        {
            if (parameter.isHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            //�Ƚ���λ�õ��ƶ�
            parameter.input_x = Input.GetAxis("Horizontal");
            if (parameter.input_x != 0)
            {
                //״̬ת��Ϊrun
                manager.TransitionState(StateType.Run);
            }
            //����ƶ����߼����ߣ�����Ƿ񹥻���������������޷���Ծ���ƶ�
            if (Input.GetKeyDown(KeyCode.J) && !parameter.isAttack)
            {
                //�ɹ���State �жϹ����Ķ���
                manager.TransitionState(StateType.Atk);
            }
            //��Ծ�͹���ֻ��һ��i������Դ���
            if (Input.GetKeyDown(KeyCode.Space) && parameter.coll.onGround)
            {
                //��Ծ
                manager.TransitionState(StateType.Jump);
            }

        }

        public virtual void OnExit()
        {
            //Debug.Log("Idle Exit.");
        }


    }

    public class RunState : IState
    {
        //״̬���ƻ�
        private player.MyFSM manager;
        //����
        private player.Parameter parameter;

        public RunState(MyFSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public virtual void OnEnter()
        {
            parameter.animator.Play("Run");
        }

        public virtual void OnUpdate()
        {
            if (parameter.isHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            //��ִ��run���߼�
            //�����ű����� ���� ת��
            manager.transform.localScale = new Vector3(Mathf.Sign(parameter.input_x), 1, 1);
            //����ˮƽ�ٶȣ��᲻��Խ��Խ��?
            parameter.rigidbody2D.velocity = new Vector2(parameter.input_x * parameter.speed,
                            parameter.rigidbody2D.velocity.y);
            //�κ�״̬������������״̬ת��
            if (Input.GetKeyDown(KeyCode.J) && !parameter.isAttack)
            {
                //�ɹ���State �жϹ����Ķ���
                manager.TransitionState(StateType.Atk);
            }
            //��Ծ�͹���ֻ��һ��i������Դ���
            if (Input.GetKeyDown(KeyCode.Space) && parameter.coll.onGround)
            {
                //��Ծ
                manager.TransitionState(StateType.Jump);
            }
            //����������Ϊ0ʱ���л���Idle
            parameter.input_x = Input.GetAxis("Horizontal");
            if(parameter.input_x == 0)
            {
                manager.TransitionState(StateType.Idle);
            }
            //���ܹ����вſ��Ի���
            if (Input.GetKeyDown(KeyCode.L) && parameter.coll.onGround)
            {
                manager.TransitionState(StateType.Slide);
            }
            
        }

        public virtual void OnExit()
        {

        }
    }

    //����״̬
    public class SlideState: IState
    {

        //״̬���ƻ�
        private player.MyFSM manager;
        //����
        private player.Parameter parameter;
        //�������Ž���
        public AnimatorStateInfo info;
        //����ʱ��
        public float timer;
        public SlideState(MyFSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public virtual void OnEnter()
        {
            parameter.animator.Play("Slide");
            parameter.rigidbody2D.velocity = new Vector2(parameter.input_x / Mathf.Abs(parameter.input_x) * parameter.slideSpeed, parameter.rigidbody2D.velocity.y);
            //��ͷ������ײ����Ϊfalse
            parameter.headCollider2D.enabled = false;
        }

        public virtual void OnUpdate()
        {
            timer += Time.deltaTime;

            if (parameter.isHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            info = parameter.animator.GetCurrentAnimatorStateInfo(0);
            //��Ҫ�жϻ����������Ƿ�ͷ�����ڵ�
            //����������ڵ���Ҫ���ݽ�����ٶȣ�һֱ������������ Physics2D.OverlapCircle(
            if(Physics2D.OverlapCircle(parameter.celling.position, 0.5f, parameter.groundLayer))
            {
                //��������һֱ���ֻ���,ֻ��Ҫinput�ṩ �ķ��򼴿�
                parameter.rigidbody2D.velocity = new Vector2(parameter.input_x / Mathf.Abs(parameter.input_x) * parameter.slideSpeed, parameter.rigidbody2D.velocity.y);
            }else if(Input.GetKeyDown(KeyCode.Space) && parameter.coll.onGround)
            {
                manager.TransitionState(StateType.Jump);
            }
            else if(timer > parameter.slideTime)
            {
                //Mathf.Abs(parameter.rigidbody2D.velocity.x) < 1.0f && if(info.normalizedTime > 0.95f)
                //��ˮƽ�ٶȵľ���ֵС��0.5ʱ��ִ��������
                manager.TransitionState(StateType.Stand);
            }
            else
            {
                parameter.rigidbody2D.velocity = new Vector2(parameter.input_x / Mathf.Abs(parameter.input_x) * parameter.slideSpeed, parameter.rigidbody2D.velocity.y);
            }

        }

        public virtual void OnExit()
        {
            //��ͷ������ײ����Ϊtrue
            parameter.headCollider2D.enabled = true;
            timer = 0;
        }
    }

    //��������
    public class StandState : IState
    {
        //״̬���ƻ�
        private player.MyFSM manager;
        //����
        private player.Parameter parameter;
        //��¼����ִ�н���
        private AnimatorStateInfo info;

        public StandState(MyFSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public virtual void OnEnter()
        {
            parameter.animator.Play("Stand");
            parameter.rigidbody2D.velocity = new Vector2(parameter.input_x / Mathf.Abs(parameter.input_x) * parameter.standSpeed, parameter.rigidbody2D.velocity.y);
        }

        public virtual void OnUpdate()
        {
            info = parameter.animator.GetCurrentAnimatorStateInfo(0);
            if(info.normalizedTime > 0.8f)
            {
                manager.TransitionState(StateType.Idle);
            }
        }

        public virtual void OnExit()
        {

        }
    }



    //��Ծ״̬
    public class JumpState : IState
    {
        //״̬���ƻ�
        private player.MyFSM manager;
        //����
        private player.Parameter parameter;

        public JumpState(MyFSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public virtual void OnEnter()
        {

            //������Ծ��y���ٶȣ�ִֻ��һ�Σ�����OnEnter��
            parameter.rigidbody2D.velocity = new Vector2(parameter.input_x * parameter.speed,
                            parameter.JumpSpeed);
            //parameter.rigidbody2D.velocity = new Vector2(parameter.rigidbody2D.velocity.x, parameter.JumpSpeed);
            parameter.animator.Play("Jump");
        }

        public virtual void OnUpdate()
        {
            if (parameter.isHit)
            {
                manager.TransitionState(StateType.Hit);
            }

            parameter.input_x = Input.GetAxis("Horizontal");
            //�����ű����� ���� ת��
            manager.transform.localScale = new Vector3(Mathf.Sign(parameter.input_x), 1, 1);
            //ˮƽ�ƶ�
            //parameter.rigidbody2D.velocity = new Vector2(parameter.input_x * parameter.speed,
            //                parameter.rigidbody2D.velocity.y);

            if (parameter.coll.onGround && !parameter.isDashing)
            {
                manager.gameObject.GetComponent<BetterJumping>().enabled = true;
            }
            

            //����ǽ��λ����Ϣ
            if (Input.GetKeyDown(KeyCode.I) && (parameter.coll.onLeftWall || parameter.coll.onRightWall))
            {
                manager.TransitionState(StateType.WallGrab);
            }

            //�κ�״̬������������״̬ת��
            if (Input.GetKeyDown(KeyCode.J) && !parameter.isAttack)
            {
                //�ɹ���State �жϹ����Ķ���
                manager.TransitionState(StateType.Atk);
            }
            //��y���ٶȵ�0���л���fall
            if (parameter.rigidbody2D.velocity.y < 0)
            {
                manager.TransitionState(StateType.Fall);
            }
        }

        public virtual void OnExit()
        {

        }
    }

    //����״̬
    public class FallState : IState
    {
        //״̬���ƻ�
        private player.MyFSM manager;
        //����
        private player.Parameter parameter;

        public FallState(MyFSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public virtual void OnEnter()
        {
            parameter.animator.Play("Fall");
        }

        public virtual void OnUpdate()
        {
            if (parameter.isHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            //����ǽ��λ����Ϣ
            if (Input.GetKey(KeyCode.I) && (parameter.coll.onLeftWall || parameter.coll.onRightWall))
            {
                manager.TransitionState(StateType.WallGrab);
            }
            //�κ�״̬������������״̬ת��
            if (Input.GetKeyDown(KeyCode.J) && !parameter.isAttack)
            {
                //�ɹ���State �жϹ����Ķ���
                manager.TransitionState(StateType.Atk);
            }
            //�����������棬�л���Idle
            if (parameter.coll.onGround)
            {
                manager.TransitionState(StateType.Idle);
            }
        }

        public virtual void OnExit()
        {
            parameter.isAttack = false;
        }
    }


    public class AtkState : IState
    {
        //״̬���ƻ�
        private player.MyFSM manager;
        //����
        private player.Parameter parameter;
        //�������Ž���
        private AnimatorStateInfo info;

        public AtkState(MyFSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public virtual void OnEnter()
        {
            parameter.isAttack = true;
            parameter.attackType = "Light";
            parameter.comboStep++;
            //����comboStep����ִ�еĶ���
            //ѭ�����ι���
            if (parameter.comboStep > 3)
            {
                parameter.comboStep = 1;
            }
            //����interval��timer
            parameter.timer = parameter.interval;
            switch (parameter.comboStep)
            {
                case 1:
                    parameter.animator.Play("Atk1");
                    break;
                case 2:
                    parameter.animator.Play("Atk2");
                    break;
                case 3:
                    parameter.animator.Play("Atk3");
                    break;
            }
        }

        public virtual void OnUpdate()
        {
            if (parameter.isHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            if (parameter.timer > 0)
            {
                parameter.timer -= Time.deltaTime;
                if (parameter.timer < 0)
                {
                    //����comboStep��timer
                    parameter.timer = 0;
                    parameter.comboStep = 0;
                }
            }

            //manager.Invoke("AttackEnd", 0.65f);

            // ������ṥ������ƶ�����,�����ɵ�ǰ�泯�ķ������������������,���й�������Ҫ�����ٶ�
            if (parameter.attackType == "Light" && parameter.coll.onGround)
            {
                parameter.rigidbody2D.velocity = new Vector2(manager.transform.localScale.x * parameter.lightSpeed,
                    parameter.rigidbody2D.velocity.y);
            }

            info = parameter.animator.GetCurrentAnimatorStateInfo(0);
            //��������������
            if(info.normalizedTime > 0.95f)
            {
                manager.TransitionState(StateType.Idle);
            }
        }

        public virtual void OnExit()
        {
            parameter.isAttack = false;
        }

        
    }

    public class HitState : IState
    {
        //״̬���ƻ�
        private player.MyFSM manager;
        //����
        private player.Parameter parameter;
        //���Ž���
        private AnimatorStateInfo info;

        public HitState(MyFSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public virtual void OnEnter()
        {
            parameter.isHit = true;
            parameter.animator.Play("Hurt");
        }

        public virtual void OnUpdate()
        {
            //����һ�������˵��ٶ�
            parameter.rigidbody2D.velocity = parameter.hitSpeed * parameter.direction;
            
            info = parameter.animator.GetCurrentAnimatorStateInfo(0);
            //Debug.Log($"Hit info: {info.normalizedTime}");
            if (info.normalizedTime > 0.95f)
            {
                manager.TransitionState(StateType.Idle);
            }
        }

        public virtual void OnExit()
        {
            parameter.isHit = false;
            //Debug.Log("Hit exit");
        }
    }

    public class DeadState : IState
    {
        //״̬���ƻ�
        private player.MyFSM manager;
        //����
        private player.Parameter parameter;

        public DeadState(MyFSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public virtual void OnEnter()
        {
            parameter.animator.Play("Dead");
        }

        public virtual void OnUpdate()
        {

        }

        public virtual void OnExit()
        {

        }
    }

    //��ǽ
    public class WallGrabState : IState
    {
        //״̬���ƻ�
        private player.MyFSM manager;
        //����
        private player.Parameter parameter;

        public WallGrabState(MyFSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public virtual void OnEnter()
        {
            //ת��
            manager.transform.localScale = new Vector3(parameter.coll.wallSide, 1, 1);
            //manager.transform.localScale = parameter.coll.onLeftWall ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
            parameter.animator.Play("WallGrab");
            parameter.rigidbody2D.gravityScale = 0f;
            parameter.rigidbody2D.velocity = Vector2.zero;
        }

        public virtual void OnUpdate()
        {
            if (Input.GetKeyUp(KeyCode.I))
            {
                //�ɿ�I����ʼ����
                manager.TransitionState(StateType.Fall);
            }
            parameter.input_y = Input.GetAxis("Vertical");
            if(parameter.input_y > 0f)
            {
                //������
                manager.TransitionState(StateType.WallClimb);
            }
            else if (parameter.input_y < 0f)
            {
                //���»���
                manager.TransitionState(StateType.WallSlide);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                manager.TransitionState(StateType.Jump);
            }
        }

        public virtual void OnExit()
        {
            //�ָ�����
            parameter.rigidbody2D.gravityScale = 3f;
        }
    }

    //��ǽ
    public class WallSlideState : IState
    {
        //״̬���ƻ�
        private player.MyFSM manager;
        //����
        private player.Parameter parameter;

        public WallSlideState(MyFSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public virtual void OnEnter()
        {
            parameter.animator.Play("WallGrab");
            parameter.rigidbody2D.gravityScale = 0f;
            parameter.rigidbody2D.velocity = Vector2.zero;
            //�����ٶ�
            parameter.rigidbody2D.velocity = new Vector2(parameter.rigidbody2D.velocity.x, -13);
        }

        public virtual void OnUpdate()
        {
            if (Input.GetKeyUp(KeyCode.I))
            {
                //�ɿ�I����ʼ����
                manager.TransitionState(StateType.Fall);
            }
            parameter.input_y = Input.GetAxis("Vertical");
            if (parameter.input_y > 0f)
            {
                //������
                manager.TransitionState(StateType.WallClimb);
            }
            else if (parameter.input_y == 0f)
            {
                //��ǽ
                manager.TransitionState(StateType.WallGrab);
            }
            //�»������˵���
            if (parameter.coll.onGround)
            {
                manager.TransitionState(StateType.Idle);
            }
        }

        public virtual void OnExit()
        {
            //�ָ�����
            parameter.rigidbody2D.gravityScale = 3f;
        }
    }

    //��ǽ
    public class WallClimbState : IState
    {
        //״̬���ƻ�
        private player.MyFSM manager;
        //����
        private player.Parameter parameter;

        public WallClimbState(MyFSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public virtual void OnEnter()
        {
            parameter.animator.Play("WallClimb");
            parameter.rigidbody2D.gravityScale = 0f;
            parameter.rigidbody2D.velocity = Vector2.zero;
            //�����ٶ�
            parameter.rigidbody2D.velocity = new Vector2(parameter.rigidbody2D.velocity.x, 10);
        }

        public virtual void OnUpdate()
        {
            if (Input.GetKeyUp(KeyCode.I))
            {
                //�ɿ�I����ʼ����
                manager.TransitionState(StateType.Fall);
            }
            parameter.input_y = Input.GetAxis("Vertical");
            if (parameter.input_y < 0f)
            {
                //�»�
                manager.TransitionState(StateType.WallSlide);
            }
            else if (parameter.input_y == 0f)
            {
                ///��ǽ
                manager.TransitionState(StateType.WallGrab);
            }
        }

        public virtual void OnExit()
        {
            //�ָ�����
            parameter.rigidbody2D.gravityScale = 3f;
        }
    }

    //��ǽ��
    public class WallJumpState : IState
    {
        //״̬���ƻ�
        private player.MyFSM manager;
        //����
        private player.Parameter parameter;

        public WallJumpState(MyFSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public virtual void OnEnter()
        {
            parameter.animator.Play("WallClimb");
            parameter.rigidbody2D.gravityScale = 0f;
            parameter.rigidbody2D.velocity = Vector2.zero;
            //�����ٶ�
            parameter.rigidbody2D.velocity = new Vector2(parameter.rigidbody2D.velocity.x, 10);
        }

        public virtual void OnUpdate()
        {
            if (Input.GetKeyUp(KeyCode.I))
            {
                //�ɿ�I����ʼ����
                manager.TransitionState(StateType.Fall);
            }
            parameter.input_y = Input.GetAxis("Vertical");
            if (parameter.input_y < 0f)
            {
                //�»�
                manager.TransitionState(StateType.WallSlide);
            }
            else if (parameter.input_y == 0f)
            {
                ///��ǽ
                manager.TransitionState(StateType.WallGrab);
            }
        }

        public virtual void OnExit()
        {
            //�ָ�����
            parameter.rigidbody2D.gravityScale = 3f;
        }
    }

}