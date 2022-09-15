using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyCeleste._06_Plugins._01_PrefabTileMap;
using TinyCeleste._02_Modules._07_Physics._04_ColliderChecker;

namespace TinyCeleste._02_Modules._05_PrefabTile._03_Spring
{
    public class E_Spring : E_PrefabTile
    {
        public C_ColliderChecker colliderChecker;           //碰撞列表检测
        public C_EjectPlayer ejectPlayer;                   //
        //public C_SpringAnimator springAnimator;

        
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //检测是否与玩家进行碰撞，结果存放在 colliderChecker.checker.colliderTag
            colliderChecker.ColliderCheckerSystem();
            //
            ejectPlayer.EjectPlayerSystem();
            //
            //Debug.Log("播放弹簧弹起动画。");

        }
    }
}

