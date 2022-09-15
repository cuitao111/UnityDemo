using TinyCeleste._01_Framework;
//using TinyCeleste._02_Modules._03_Player;
using TinyCeleste._02_Modules._07_Physics._04_ColliderChecker;
using TinyCeleste._02_Modules._08_Proxy;
using UnityEngine;
using Gamekit2D;

namespace TinyCeleste._02_Modules._05_PrefabTile._03_Spring
{
    public class C_EjectPlayer : EntityComponent
    {
        private ColliderCheckerItem m_PlayerChecker;
        //private C_SpringAnimator m_SpringAnimator;
        private Transform m_Transform;
        
        public float ejectSpeed = 30f;

        public float ejectAngel = 90;

        public bool isEventHappend;

        private void Awake()
        {
            //初始化 Spring 的ColliderChecker
            m_PlayerChecker = GetComponentNotNull<C_ColliderChecker>().GetChecker("Player Checker");
            //m_SpringAnimator = GetComponentNotNull<C_SpringAnimator>();
            m_Transform = GetComponentNotNull<C_Transform2DProxy>().transform;
        }

        public void EjectPlayerSystem()
        {
            isEventHappend = false;
            // 播放动画时不能弹
            //if (m_SpringAnimator.isEjectAnimPlaying) return;                    //已经在播放弹跳动画
            if (m_PlayerChecker.isHit)                                          //isHit表示触发了检测器
            {
                //从checker中获取碰撞检测的对象
                foreach (var tagContainer in m_PlayerChecker.targetList)
                {
                    //获取 C_ColliderTag并转为Player对象 后面调用 Player的BeEjected
                    var player = (PlayerCharacter) tagContainer.GetEntityObject();
                    //var rad = (m_Transform.eulerAngles.z + 90) * Mathf.Deg2Rad;
                    var rad = ejectAngel * Mathf.Deg2Rad;
                    var dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
                    Vector2 velocity = ejectSpeed * dir;
                    isEventHappend = player.BeEjected(velocity) || isEventHappend;
                }
            }
        }
    }
}