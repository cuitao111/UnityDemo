using UnityEngine;

namespace Gamekit2D
{
    public class DashSMB : SceneLinkedSMB<PlayerCharacter>
    {
        //类似attack
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.InitDashTimer();
        }

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.UpdateDashTimer();
            //m_MonoBehaviour.CheckForOnWallState();
            //实现粒子效果
            //检查是否出现了蹭墙跳
            if (m_MonoBehaviour.CheckForJumpInput())
                m_MonoBehaviour.WallJump();
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //退出时设置重力为0
            m_MonoBehaviour.ResetGravity();
        }
    }
}