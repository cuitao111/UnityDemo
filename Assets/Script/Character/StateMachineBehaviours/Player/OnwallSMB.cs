using UnityEngine;

namespace Gamekit2D
{
    public class OnwallSMB : SceneLinkedSMB<PlayerCharacter>
    {
        //类似attack
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //设置三种状态的共同点
            //先判断三种状态的哪一种，进入条件是在墙上，且浮空
            m_MonoBehaviour.CheckForOnWallState();
        }

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.GroundedHorizontalMovement(false); 
            m_MonoBehaviour.UpdateOnWallFacing();         //更新面朝方向
            m_MonoBehaviour.CheckForOnWallState();
            m_MonoBehaviour.CheckForGrounded();
            m_MonoBehaviour.CheckForWall();
            if (m_MonoBehaviour.CheckForJumpInput())
                m_MonoBehaviour.WallJump();
            else if (m_MonoBehaviour.CheckForDashInput())
                m_MonoBehaviour.Dash();

            //实现粒子效果
            //检查是否出现了蹬墙跳和冲刺逻辑
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //退出时设置重力为0
            m_MonoBehaviour.ResetGravity();
            m_MonoBehaviour.SetDashState(false);
        }
    }
}