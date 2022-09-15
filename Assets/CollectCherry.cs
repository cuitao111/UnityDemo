using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectCherry : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision != null)
        {
            if (collision.CompareTag("Player"))
            {
                //调用player获取cherry方法
                if (collision.gameObject.GetComponent<player.MyFSM>().CollectCherry())
                {
                    //销毁当前对象
                    Destroy(gameObject);
                }
            }
        }
    }
}
