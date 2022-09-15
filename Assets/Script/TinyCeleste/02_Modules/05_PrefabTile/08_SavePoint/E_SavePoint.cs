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
        

        public C_ColliderChecker colliderChecker;           //碰撞列表检测
        private ColliderCheckerItem playerChecker;

        private void Awake()
        {
            playerChecker = GetComponentNotNull<C_ColliderChecker>().GetChecker("Player Checker");
            colliderChecker = GetComponent<C_ColliderChecker>();
        }


        // Update is called once per frame
        void Update()
        {
            //检测是否与玩家进行碰撞，结果存放在 colliderChecker.checker.colliderTag
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

