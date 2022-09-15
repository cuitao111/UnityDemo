using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    //����һ��??������������ֹ�����ͬʱ�����������ƫ��
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
        //�����λ
        transform.position = pre_vector;*/

        //��һ��ʵ�ַ�ʽ
        //��ȡ�������Transform��Ϣ
        Transform camera = transform;
        //Transform camera = Camera.main.transform;
        //��¼�µ�ǰ�����λ��
        Vector3 pre_pos = camera.transform.position;
        //��ѭ��ʱ�����0ʱ
        while(timer > 0)
        {
            //���λ������仯������������������仯��range�˴�������ǿ��
            camera.position = Random.insideUnitSphere * range + pre_pos;
            //����ʱ��
            timer -= Time.deltaTime;
            //��ͣһ֡����ѭ��
            yield return null;
        }
        //�����λ
        camera.transform.position = pre_pos;
        isShake = false;
    }
}
