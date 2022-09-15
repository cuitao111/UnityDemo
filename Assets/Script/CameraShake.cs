using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    //创建一个??，震动锁定，防止多个震动同时发生导致相机偏移
    public bool isShake;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }


    public void DoShake(float range, float timer)
    {
        StopAllCoroutines();
        if (!isShake)
        {
            StartCoroutine(Shake(range, timer));
        }
    }

    public IEnumerator Shake(float range, float timer)
    {
        isShake = true;
        /*Vector3 pre_vector = transform.position;
        while(timer > 0)
        {
            timer -= Time.deltaTime;
            if(timer < 0)
            {
                break;
            }
            Vector3 pos = transform.position;
            pos.x += Random.Range(-range, range);
            pos.y += Random.Range(-range, range);
            transform.position = pos;
            yield return null;
        }
        //相机归位
        transform.position = pre_vector;*/

        //另一种实现方式
        //获取主相机的Transform信息
        Transform camera = transform;
        //Transform camera = Camera.main.transform;
        //记录下当前相机的位置
        Vector3 pre_pos = camera.transform.position;
        //当循环时间大于0时
        while(timer > 0)
        {
            //相机位置随机变化，让坐标在球内随机变化，range此处代表震动强度
            camera.position = Random.insideUnitSphere * range + pre_pos;
            //更新时间
            timer -= Time.deltaTime;
            //暂停一帧结束循环
            yield return null;
        }
        //相机归位
        camera.transform.position = pre_pos;
        isShake = false;
    }
}
