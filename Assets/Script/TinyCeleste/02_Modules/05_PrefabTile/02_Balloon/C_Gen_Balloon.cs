using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Gen_Balloon : MonoBehaviour
{
    //重生计时器
    public float resumeInterval = 5f;
    public float Timer = 0;
    

    private void Awake()
    {
        //开始在transfrom位置生成一个Balloon
        //使用Instantiate初始化bullet
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
    //重生Rallboon
    public void ResumeBallon()
    {
        Timer = resumeInterval;
    }
}
