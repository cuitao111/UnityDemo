using UnityEngine;

namespace Gamekit2D
{
    public class DashSMB : SceneLinkedSMB<PlayerCharacter>
    {
        //����attack
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.InitDashTimer();
        }

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.UpdateDashTimer();
            //m_MonoBehaviour.CheckForOnWallState();
            //ʵ������Ч��
            //����Ƿ�����˲�ǽ��
            if (m_MonoBehaviour.CheckForJumpInput())
                m_MonoBehaviour.WallJump();
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //�˳�ʱ��������Ϊ0
            m_MonoBehaviour.ResetGravity();
        }
    }
}