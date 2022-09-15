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
        //更新子弹位置
        transform.Translate(Mathf.Sign(transform.localScale.x) * Vector3.right * Time.deltaTime * 5.5f);
    }

    public void Flip()
    {
        //获取scale
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
                    //如果在被击中状态直接返回，被击中无敌时间，直到播放完动画
                    return;
                }
                //状态转换为被击中，使用状态机来实现不需要考虑 anyState-> Atk, 必然要执行完一次
                other.gameObject.GetComponent<player.MyFSM>().isHit = true;
                //判断子弹与玩家的位置
                if (transform.position.x < other.gameObject.transform.position.x)
                {
                    other.gameObject.GetComponent<player.MyFSM>().GetHit(Vector2.right);
                }
                else
                {
                    other.gameObject.GetComponent<player.MyFSM>().GetHit(Vector2.left);
                }
                //销毁
                Destroy(gameObject);
                
            }
        }*/
        if (other != null)
        {
            if (other.CompareTag("Player"))
            {
                other.gameObject.GetComponent<player.MyFSM>().isHit = true;
                //还需要传递一个方向给MyFSM
                Destroy(gameObject);
            }
        }
        
    }
}
