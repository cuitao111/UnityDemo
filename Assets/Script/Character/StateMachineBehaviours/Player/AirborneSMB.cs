using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit2D
{
    //跳跃状态
    public class AirborneSMB : SceneLinkedSMB<PlayerCharacter>
    {
        public override void OnSLStateNoTransitionUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.UpdateFacing();
            m_MonoBehaviour.UpdateJump();
            m_MonoBehaviour.AirborneHorizontalMovement();
            m_MonoBehaviour.AirborneVerticalMovement();
            m_MonoBehaviour.CheckForGrounded();
            //m_MonoBehaviour.CheckForHoldingGun();
            //m_MonoBehaviour.CheckAndFireGun ();
            m_MonoBehaviour.CheckForCrouching ();
            m_MonoBehaviour.CheckForWall();
            if (m_MonoBehaviour.CheckForMeleeAttackInput())
                m_MonoBehaviour.MeleeAttack(false);
            else if (m_MonoBehaviour.CheckForDashInput())
                m_MonoBehaviour.Dash();
            //else if (m_MonoBehaviour.CheckFoGrabInput())
            //    m_MonoBehaviour.WallGrab();
        }
    }
}