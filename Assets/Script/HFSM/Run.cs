using FSM;
using System;
using UnityEngine;


public class Run : StateBase<PlayerState>
{
    private Animator animator;
    private Transform playerTransform;           //获取玩家的transform,用于改变方向和速度
    private Vector3 direction;
    Action<float> onInputChanged;

    public Run(
        Transform transform,
        Animator animator,
        Action<float> onInputChanged,
        bool needsExitTime) : base(needsExitTime)
    {
        this.animator = animator;
        this.playerTransform = transform;
        this.onInputChanged = onInputChanged;
    }

    public override void OnEnter()
    {
        animator.SetTrigger("Run");
        direction = new Vector3(1, 1, 1);
        //direction = new Vector3(parameter.input_x > 0 ? 1 : -1, 0 , 0);
        //方向由input_x决定
        //playerTransform.localScale = new Vector3(Mathf.Sign(parameter.input_x), 1, 1);

    }

    public override void OnLogic()
    {
        //更新水平轴的输入
        //parameter.input_x = Input.GetAxis("Horizontal");
        //if (parameter.input_x == 0)
        //{
        //    fsm.StateCanExit();
        //}
        //更新玩家位置
        playerTransform.Translate(direction * (Time.deltaTime * 10));
    }

    public override void OnExit()
    {
        //onInputChanged?.Invoke(parameter.input_x);
    }
}