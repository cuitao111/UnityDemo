using UnityEngine;

namespace Gamekit2D
{
    public class OnwallSMB : SceneLinkedSMB<PlayerCharacter>
    {
        //����attack
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //��������״̬�Ĺ�ͬ��
            //���ж�����״̬����һ�֣�������������ǽ�ϣ��Ҹ���
            m_MonoBehaviour.CheckForOnWallState();
        }

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.GroundedHorizontalMovement(false); 
            m_MonoBehaviour.UpdateOnWallFacing();         //�����泯����
            m_MonoBehaviour.CheckForOnWallState();
            m_MonoBehaviour.CheckForGrounded();
            m_MonoBehaviour.CheckForWall();
            if (m_MonoBehaviour.CheckForJumpInput())
                m_MonoBehaviour.WallJump();
            else if (m_MonoBehaviour.CheckForDashInput())
                m_MonoBehaviour.Dash();

            //ʵ������Ч��
            //����Ƿ�����˵�ǽ���ͳ���߼�
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //�˳�ʱ��������Ϊ0
            m_MonoBehaviour.ResetGravity();
            m_MonoBehaviour.SetDashState(false);
        }
    }
}