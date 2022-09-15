using TinyCeleste._01_Framework;
//using TinyCeleste._02_Modules._03_Player;
//using TinyCeleste._02_Modules._04_Effect._01_Dust;
using TinyCeleste._02_Modules._07_Physics._04_ColliderChecker;
using Gamekit2D;
using UnityEngine;

namespace TinyCeleste._02_Modules._05_PrefabTile._02_Balloon
{
    public class C_Balloon_ResumeDashCount : EntityComponent
    {
        private ColliderCheckerItem playerChecker;

        public float resumTimer = 3f;

        public float Timer;

        private void Awake()
        {
            playerChecker = GetComponentNotNull<C_ColliderChecker>().GetChecker("Player Checker");
        }

        //���ó�̴���
        public void ResumePlayerDashCountSystem()
        {
            if (playerChecker.isHit)
            {
                bool isEventHappend = false;
                foreach (var tagContainer in playerChecker.targetList)
                {
                    var player = (PlayerCharacter) tagContainer.GetEntityObject();
                    isEventHappend = player.ResumeDashCount() || isEventHappend;
                }

                if (isEventHappend) Boom();
                
            }
            //������ʱ��
            /*if (Timer > 0)
            {
                Timer -= Time.deltaTime;
                if (Timer <= 0)
                {
                    gameObject.SetActive(true);
                    Timer = 0;
                }
            }*/
        }

        private void Boom()
        {
            //��disable����ʱ��������enable
            //gameObject.SetActive(false);
            //Timer = resumTimer;
            C_Gen_Balloon gen = GetComponentInParent<C_Gen_Balloon>();
            gen.ResumeBallon();

            //���ø������C_Gen_Balloon��resume����
            Destroy(gameObject);
            //S_Dust_Factory.Instance.CreateDust(transform.position);
        }
    }
}