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
                //����player��ȡcherry����
                if (collision.gameObject.GetComponent<player.MyFSM>().CollectCherry())
                {
                    //���ٵ�ǰ����
                    Destroy(gameObject);
                }
            }
        }
    }
}
