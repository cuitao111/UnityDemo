using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyCeleste._06_Plugins._01_PrefabTileMap;
using TinyCeleste._02_Modules._07_Physics._04_ColliderChecker;
using TinyCeleste._01_Framework;
using Gamekit2D;

namespace TinyCeleste._02_Modules._05_PrefabTile._08_SavePoint
{
    public class E_SavePoint : EntityComponent
    {
        

        public C_ColliderChecker colliderChecker;           //��ײ�б���
        private ColliderCheckerItem playerChecker;

        private void Awake()
        {
            playerChecker = GetComponentNotNull<C_ColliderChecker>().GetChecker("Player Checker");
            colliderChecker = GetComponent<C_ColliderChecker>();
        }


        // Update is called once per frame
        void Update()
        {
            //����Ƿ�����ҽ�����ײ���������� colliderChecker.checker.colliderTag
            colliderChecker.ColliderCheckerSystem();
            //
            if (playerChecker.isHit)
            {
                foreach (var tagContainer in playerChecker.targetList)
                {
                    var player = (PlayerCharacter)tagContainer.GetEntityObject();
                    player.ResetRebirthPos(transform);
                }
            }
        }
    }
}

