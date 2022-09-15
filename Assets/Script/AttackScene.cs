using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScene : MonoBehaviour
{
    //单例模式
    private static AttackScene instance;
    //共有属性，用于访问
    public static AttackScene Instance
    {
        get
        {
            //如果为空，获取
            if(instance == null)
            {
                instance = Transform.FindObjectOfType<AttackScene>();
            }
            return instance;
        }
    }
    //创建函数调用协程
    public void HitPause(int frames)
    {
        //启用协程
        StartCoroutine(Pause(frames));
    }

    //使用协程实现停顿, 参数为停顿多少帧
    IEnumerator Pause(int frames)
    {
        //计算具体停顿时间
        float pauseTime = frames / 60f;
        Time.timeScale = 0; //设置停顿 0代表完全停止
        yield return new WaitForSecondsRealtime(pauseTime);
        //结束后重新设置为1
        Time.timeScale = 1;
    }


}
