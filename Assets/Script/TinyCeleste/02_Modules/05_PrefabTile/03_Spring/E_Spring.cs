using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyCeleste._06_Plugins._01_PrefabTileMap;
using TinyCeleste._02_Modules._07_Physics._04_ColliderChecker;

namespace TinyCeleste._02_Modules._05_PrefabTile._03_Spring
{
    public class E_Spring : E_PrefabTile
    {
        public C_ColliderChecker colliderChecker;           //��ײ�б���
        public C_EjectPlayer ejectPlayer;                   //
        //public C_SpringAnimator springAnimator;

        
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //����Ƿ�����ҽ�����ײ���������� colliderChecker.checker.colliderTag
            colliderChecker.ColliderCheckerSystem();
            //
            ejectPlayer.EjectPlayerSystem();
            //
            //Debug.Log("���ŵ��ɵ��𶯻���");

        }
    }
}

