using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Gen_Balloon : MonoBehaviour
{
    //������ʱ��
    public float resumeInterval = 5f;
    public float Timer = 0;
    

    private void Awake()
    {
        //��ʼ��transfromλ������һ��Balloon
        //ʹ��Instantiate��ʼ��bullet
        GameObject obj = Instantiate(Resources.Load("Balloon")) as GameObject;
        obj.transform.position = transform.position;
        obj.transform.parent = transform;
        //obj.AddComponent<Bullet>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Timer > 0)
        {
            Timer -= Time.deltaTime;
            if (Timer <= 0)
            {
                GameObject obj = Instantiate(Resources.Load("Balloon")) as GameObject;
                obj.transform.position = transform.position;
                obj.transform.parent = transform;
            }
        }
    }
    //����Rallboon
    public void ResumeBallon()
    {
        Timer = resumeInterval;
    }
}
