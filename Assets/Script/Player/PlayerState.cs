using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace player
{
    public class IdleState : IState
    {
        //状态控制机
        private player.MyFSM manager;
        //参数
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
            //先进行位置的移动
            parameter.input_x = Input.GetAxis("Horizontal");
            if (parameter.input_x != 0)
            {
                //状态转换为run
                manager.TransitionState(StateType.Run);
            }
            //如果移动的逻辑不走，检测是否攻击，如果攻击，则无法跳跃和移动
            if (Input.GetKeyDown(KeyCode.J) && !parameter.isAttack)
            {
                //由攻击State 判断攻击的段数
                manager.TransitionState(StateType.Atk);
            }
            //跳跃和攻击只有一种i情况可以触发
            if (Input.GetKeyDown(KeyCode.Space) && parameter.coll.onGround)
            {
                //跳跃
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
        //状态控制机
        private player.MyFSM manager;
        //参数
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
            //先执行run的逻辑
            //改缩放比例， 镜像 转向
            manager.transform.localScale = new Vector3(Mathf.Sign(parameter.input_x), 1, 1);
            //更改水平速度，会不会越来越快?
            parameter.rigidbody2D.velocity = new Vector2(parameter.input_x * parameter.speed,
                            parameter.rigidbody2D.velocity.y);
            //任何状态都可以往攻击状态转换
            if (Input.GetKeyDown(KeyCode.J) && !parameter.isAttack)
            {
                //由攻击State 判断攻击的段数
                manager.TransitionState(StateType.Atk);
            }
            //跳跃和攻击只有一种i情况可以触发
            if (Input.GetKeyDown(KeyCode.Space) && parameter.coll.onGround)
            {
                //跳跃
                manager.TransitionState(StateType.Jump);
            }
            //当横轴输入为0时，切换回Idle
            parameter.input_x = Input.GetAxis("Horizontal");
            if(parameter.input_x == 0)
            {
                manager.TransitionState(StateType.Idle);
            }
            //奔跑过程中才可以滑铲
            if (Input.GetKeyDown(KeyCode.L) && parameter.coll.onGround)
            {
                manager.TransitionState(StateType.Slide);
            }
            
        }

        public virtual void OnExit()
        {

        }
    }

    //滑铲状态
    public class SlideState: IState
    {

        //状态控制机
        private player.MyFSM manager;
        //参数
        private player.Parameter parameter;
        //动画播放进度
        public AnimatorStateInfo info;
        //滑行时间
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
            //将头部的碰撞设置为false
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
            //需要判断滑铲过程中是否头部有遮挡
            //如果顶部有遮挡需要根据进入的速度，一直滑铲出该区域 Physics2D.OverlapCircle(
            if(Physics2D.OverlapCircle(parameter.celling.position, 0.5f, parameter.groundLayer))
            {
                //不能起身，一直保持滑行,只需要input提供 的方向即可
                parameter.rigidbody2D.velocity = new Vector2(parameter.input_x / Mathf.Abs(parameter.input_x) * parameter.slideSpeed, parameter.rigidbody2D.velocity.y);
            }else if(Input.GetKeyDown(KeyCode.Space) && parameter.coll.onGround)
            {
                manager.TransitionState(StateType.Jump);
            }
            else if(timer > parameter.slideTime)
            {
                //Mathf.Abs(parameter.rigidbody2D.velocity.x) < 1.0f && if(info.normalizedTime > 0.95f)
                //当水平速度的绝对值小于0.5时，执行起身动作
                manager.TransitionState(StateType.Stand);
            }
            else
            {
                parameter.rigidbody2D.velocity = new Vector2(parameter.input_x / Mathf.Abs(parameter.input_x) * parameter.slideSpeed, parameter.rigidbody2D.velocity.y);
            }

        }

        public virtual void OnExit()
        {
            //将头部的碰撞设置为true
            parameter.headCollider2D.enabled = true;
            timer = 0;
        }
    }

    //滑铲起身
    public class StandState : IState
    {
        //状态控制机
        private player.MyFSM manager;
        //参数
        private player.Parameter parameter;
        //记录动画执行进度
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



    //跳跃状态
    public class JumpState : IState
    {
        //状态控制机
        private player.MyFSM manager;
        //参数
        private player.Parameter parameter;

        public JumpState(MyFSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public virtual void OnEnter()
        {

            //给定跳跃的y轴速度，只执行一次，放在OnEnter中
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
            //改缩放比例， 镜像 转向
            manager.transform.localScale = new Vector3(Mathf.Sign(parameter.input_x), 1, 1);
            //水平移动
            //parameter.rigidbody2D.velocity = new Vector2(parameter.input_x * parameter.speed,
            //                parameter.rigidbody2D.velocity.y);

            if (parameter.coll.onGround && !parameter.isDashing)
            {
                manager.gameObject.GetComponent<BetterJumping>().enabled = true;
            }
            

            //更新墙体位置信息
            if (Input.GetKeyDown(KeyCode.I) && (parameter.coll.onLeftWall || parameter.coll.onRightWall))
            {
                manager.TransitionState(StateType.WallGrab);
            }

            //任何状态都可以往攻击状态转换
            if (Input.GetKeyDown(KeyCode.J) && !parameter.isAttack)
            {
                //由攻击State 判断攻击的段数
                manager.TransitionState(StateType.Atk);
            }
            //当y轴速度到0，切换至fall
            if (parameter.rigidbody2D.velocity.y < 0)
            {
                manager.TransitionState(StateType.Fall);
            }
        }

        public virtual void OnExit()
        {

        }
    }

    //降落状态
    public class FallState : IState
    {
        //状态控制机
        private player.MyFSM manager;
        //参数
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
            //更新墙体位置信息
            if (Input.GetKey(KeyCode.I) && (parameter.coll.onLeftWall || parameter.coll.onRightWall))
            {
                manager.TransitionState(StateType.WallGrab);
            }
            //任何状态都可以往攻击状态转换
            if (Input.GetKeyDown(KeyCode.J) && !parameter.isAttack)
            {
                //由攻击State 判断攻击的段数
                manager.TransitionState(StateType.Atk);
            }
            //当降落至地面，切换到Idle
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
        //状态控制机
        private player.MyFSM manager;
        //参数
        private player.Parameter parameter;
        //动画播放进度
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
            //根据comboStep决定执行的动作
            //循环三次攻击
            if (parameter.comboStep > 3)
            {
                parameter.comboStep = 1;
            }
            //设置interval给timer
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
                    //重置comboStep和timer
                    parameter.timer = 0;
                    parameter.comboStep = 0;
                }
            }

            //manager.Invoke("AttackEnd", 0.65f);

            // 如果是轻攻击添加移动补偿,方向由当前面朝的方向决定，而不是输入,空中攻击不需要补偿速度
            if (parameter.attackType == "Light" && parameter.coll.onGround)
            {
                parameter.rigidbody2D.velocity = new Vector2(manager.transform.localScale.x * parameter.lightSpeed,
                    parameter.rigidbody2D.velocity.y);
            }

            info = parameter.animator.GetCurrentAnimatorStateInfo(0);
            //如果动画播放完毕
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
        //状态控制机
        private player.MyFSM manager;
        //参数
        private player.Parameter parameter;
        //播放进度
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
            //给定一个被击退的速度
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
        //状态控制机
        private player.MyFSM manager;
        //参数
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

    //扒墙
    public class WallGrabState : IState
    {
        //状态控制机
        private player.MyFSM manager;
        //参数
        private player.Parameter parameter;

        public WallGrabState(MyFSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public virtual void OnEnter()
        {
            //转向
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
                //松开I键开始下落
                manager.TransitionState(StateType.Fall);
            }
            parameter.input_y = Input.GetAxis("Vertical");
            if(parameter.input_y > 0f)
            {
                //向上爬
                manager.TransitionState(StateType.WallClimb);
            }
            else if (parameter.input_y < 0f)
            {
                //向下滑动
                manager.TransitionState(StateType.WallSlide);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                manager.TransitionState(StateType.Jump);
            }
        }

        public virtual void OnExit()
        {
            //恢复重力
            parameter.rigidbody2D.gravityScale = 3f;
        }
    }

    //滑墙
    public class WallSlideState : IState
    {
        //状态控制机
        private player.MyFSM manager;
        //参数
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
            //上爬速度
            parameter.rigidbody2D.velocity = new Vector2(parameter.rigidbody2D.velocity.x, -13);
        }

        public virtual void OnUpdate()
        {
            if (Input.GetKeyUp(KeyCode.I))
            {
                //松开I键开始下落
                manager.TransitionState(StateType.Fall);
            }
            parameter.input_y = Input.GetAxis("Vertical");
            if (parameter.input_y > 0f)
            {
                //向上爬
                manager.TransitionState(StateType.WallClimb);
            }
            else if (parameter.input_y == 0f)
            {
                //扒墙
                manager.TransitionState(StateType.WallGrab);
            }
            //下滑遇到了地面
            if (parameter.coll.onGround)
            {
                manager.TransitionState(StateType.Idle);
            }
        }

        public virtual void OnExit()
        {
            //恢复重力
            parameter.rigidbody2D.gravityScale = 3f;
        }
    }

    //爬墙
    public class WallClimbState : IState
    {
        //状态控制机
        private player.MyFSM manager;
        //参数
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
            //上爬速度
            parameter.rigidbody2D.velocity = new Vector2(parameter.rigidbody2D.velocity.x, 10);
        }

        public virtual void OnUpdate()
        {
            if (Input.GetKeyUp(KeyCode.I))
            {
                //松开I键开始下落
                manager.TransitionState(StateType.Fall);
            }
            parameter.input_y = Input.GetAxis("Vertical");
            if (parameter.input_y < 0f)
            {
                //下滑
                manager.TransitionState(StateType.WallSlide);
            }
            else if (parameter.input_y == 0f)
            {
                ///扒墙
                manager.TransitionState(StateType.WallGrab);
            }
        }

        public virtual void OnExit()
        {
            //恢复重力
            parameter.rigidbody2D.gravityScale = 3f;
        }
    }

    //蹬墙跳
    public class WallJumpState : IState
    {
        //状态控制机
        private player.MyFSM manager;
        //参数
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
            //上爬速度
            parameter.rigidbody2D.velocity = new Vector2(parameter.rigidbody2D.velocity.x, 10);
        }

        public virtual void OnUpdate()
        {
            if (Input.GetKeyUp(KeyCode.I))
            {
                //松开I键开始下落
                manager.TransitionState(StateType.Fall);
            }
            parameter.input_y = Input.GetAxis("Vertical");
            if (parameter.input_y < 0f)
            {
                //下滑
                manager.TransitionState(StateType.WallSlide);
            }
            else if (parameter.input_y == 0f)
            {
                ///扒墙
                manager.TransitionState(StateType.WallGrab);
            }
        }

        public virtual void OnExit()
        {
            //恢复重力
            parameter.rigidbody2D.gravityScale = 3f;
        }
    }

}