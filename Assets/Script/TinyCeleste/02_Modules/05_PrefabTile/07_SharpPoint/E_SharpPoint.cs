using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyCeleste._06_Plugins._01_PrefabTileMap;
using TinyCeleste._02_Modules._07_Physics._04_ColliderChecker;
using Gamekit2D;

namespace TinyCeleste._02_Modules._05_PrefabTile._07_SharpPoint
{
    public class E_SharpPoint : E_PrefabTile
    {

        public C_ColliderChecker colliderChecker;
        public C_DamagePlayer damagePlayer;
        //public PlayerCharacter character;
        // Start is called before the first frame update
        void Awake()
        {
            damagePlayer = gameObject.GetComponent<C_DamagePlayer>();
            colliderChecker = gameObject.GetComponent<C_ColliderChecker>();
        }

        // Update is called once per frame
        void Update()
        {
            //¼ì²âÅö×²
            colliderChecker.ColliderCheckerSystem();
            //Íæ¼ÒÊÜ»÷·´À¡
            damagePlayer.DamagePlayerSystem();
        }
    }

}
