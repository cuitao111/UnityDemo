using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyCeleste._02_Modules._07_Physics._04_ColliderChecker;
using TinyCeleste._06_Plugins._01_PrefabTileMap;

namespace TinyCeleste._02_Modules._05_PrefabTile._02_Balloon
{
    public class E_Balloon : E_PrefabTile
    {
        // ����ҵĳ�̴���С������̴���ʱ
        // ��������ײ������»ָ���������ٶȣ�ͬʱ����ݻ�
        public C_ColliderChecker colliderChecker;
        public C_Balloon_ResumeDashCount resumeDashCount;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // ������ײ��ص�״̬����
            colliderChecker.ColliderCheckerSystem();

            // �ָ���ҳ�̴���ϵͳ E_Balloon : E_PrefabTile  C_Balloon_ResumeDashCount��EntityComponent
            resumeDashCount.ResumePlayerDashCountSystem();
        }
    }

}
