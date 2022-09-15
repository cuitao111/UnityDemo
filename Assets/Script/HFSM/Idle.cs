using FSM;
using UnityEngine;

public class Idle : StateBase<PlayerState>
{
    private Animator animator;              //动画器
    private Timer timer;                    //计时器，不需要Time.detailTime

    public Idle(Animator animator, bool needsExitTime) : base(needsExitTime)
    {
        this.animator = animator;
        timer = new Timer();
    }

    public override void OnEnter()
    {
        animator.SetTrigger("Idle");
    }

    public override void OnLogic()
    {
        //先进行位置的移动,只更新输入的值即可，状态转换由FSM实现
        //parameter.input_x = Input.GetAxis("Horizontal");
        //判断是否可以转换到其他状态
        //if (parameter.input_x != 0 ||
        //    (Input.GetKeyDown(KeyCode.J) && !parameter.isAttack) ||
        //    (Input.GetKeyDown(KeyCode.Space) && parameter.coll.onGround))
        //{
        //    fsm.StateCanExit();
        //}

    }

    public override void OnExit()
    {
        //清楚Idle开关
        animator.ResetTrigger("Idle");
    }
}