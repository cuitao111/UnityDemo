using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyCeleste._02_Modules._07_Physics._04_ColliderChecker;
using TinyCeleste._06_Plugins._01_PrefabTileMap;

namespace TinyCeleste._02_Modules._05_PrefabTile._02_Balloon
{
    public class E_Balloon : E_PrefabTile
    {
        // 当玩家的冲刺次数小于最大冲刺次数时
        // 与气球碰撞后可重新恢复到最大冲刺速度，同时气球摧毁
        public C_ColliderChecker colliderChecker;
        public C_Balloon_ResumeDashCount resumeDashCount;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // 更新碰撞相关的状态变量
            colliderChecker.ColliderCheckerSystem();

            // 恢复玩家冲刺次数系统 E_Balloon : E_PrefabTile  C_Balloon_ResumeDashCount：EntityComponent
            resumeDashCount.ResumePlayerDashCountSystem();
        }
    }

}
