using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScene : MonoBehaviour
{
    //����ģʽ
    private static AttackScene instance;
    //�������ԣ����ڷ���
    public static AttackScene Instance
    {
        get
        {
            //���Ϊ�գ���ȡ
            if(instance == null)
            {
                instance = Transform.FindObjectOfType<AttackScene>();
            }
            return instance;
        }
    }
    //������������Э��
    public void HitPause(int frames)
    {
        //����Э��
        StartCoroutine(Pause(frames));
    }

    //ʹ��Э��ʵ��ͣ��, ����Ϊͣ�ٶ���֡
    IEnumerator Pause(int frames)
    {
        //�������ͣ��ʱ��
        float pauseTime = frames / 60f;
        Time.timeScale = 0; //����ͣ�� 0������ȫֹͣ
        yield return new WaitForSecondsRealtime(pauseTime);
        //��������������Ϊ1
        Time.timeScale = 1;
    }


}
