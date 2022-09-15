using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //不能省略掉z轴的赋值
        //transform.position = target.position;
        transform.position = new Vector3(target.position.x, target.position.y, -10.0f);
    }
}
