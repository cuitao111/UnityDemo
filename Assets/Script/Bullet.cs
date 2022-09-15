using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float timer;
    // Start is called before the first frame update
    void Start()
    {
        timer = 5.0f;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if(timer < 0)
        {
            Destroy(gameObject);
        }
        //�����ӵ�λ��
        transform.Translate(Mathf.Sign(transform.localScale.x) * Vector3.right * Time.deltaTime * 5.5f);
    }

    public void Flip()
    {
        //��ȡscale
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        /*if (other != null)
        {
            //Debug.Log($"Bullet OnTriggerEnter {other.transform.name}");
            if (other.CompareTag("Player"))
            {
                if (other.gameObject.GetComponent<player.MyFSM>().isHit)
                {
                    //����ڱ�����״ֱ̬�ӷ��أ��������޵�ʱ�䣬ֱ�������궯��
                    return;
                }
                //״̬ת��Ϊ�����У�ʹ��״̬����ʵ�ֲ���Ҫ���� anyState-> Atk, ��ȻҪִ����һ��
                other.gameObject.GetComponent<player.MyFSM>().isHit = true;
                //�ж��ӵ�����ҵ�λ��
                if (transform.position.x < other.gameObject.transform.position.x)
                {
                    other.gameObject.GetComponent<player.MyFSM>().GetHit(Vector2.right);
                }
                else
                {
                    other.gameObject.GetComponent<player.MyFSM>().GetHit(Vector2.left);
                }
                //����
                Destroy(gameObject);
                
            }
        }*/
        if (other != null)
        {
            if (other.CompareTag("Player"))
            {
                other.gameObject.GetComponent<player.MyFSM>().isHit = true;
                //����Ҫ����һ�������MyFSM
                Destroy(gameObject);
            }
        }
        
    }
}
