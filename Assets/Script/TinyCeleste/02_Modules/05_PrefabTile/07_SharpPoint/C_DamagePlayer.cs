using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyCeleste._01_Framework;
using TinyCeleste._02_Modules._07_Physics._04_ColliderChecker;
using Gamekit2D;

namespace TinyCeleste._02_Modules._05_PrefabTile._07_SharpPoint
{
    public class C_DamagePlayer : EntityComponent
    {

        private ColliderCheckerItem playerChecker;
        // Start is called before the first frame update
        private void Awake()
        {
            playerChecker = GetComponentNotNull<C_ColliderChecker>().GetChecker("Player Checker");
        }

        //
        public void DamagePlayerSystem()
        {
            if (playerChecker.isHit)
            {
                //bool isEventHappend = false;
                foreach (var tagContainer in playerChecker.targetList)
                {
                    var player = (PlayerCharacter)tagContainer.GetEntityObject();
                    //isEventHappend = player.DamageSystem() || isEventHappend;
                    player.DamageSystem();
                }

            }
        }
    }
}

