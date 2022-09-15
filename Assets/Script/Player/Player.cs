using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("补偿速度")]
    public float lightSpeed;
    [Header("打击感")]
    public float shakeTime;
    public int lightPause;
    public float lightStrength;
    [Space]

    //输入水平轴的值 作为成员 后面使用
    private float input_x;
    //获取刚体
    Rigidbody2D rigidbody2D;
    //速度
    public float speed = 3.0f;
    //跳跃速度
    public float JumpSpeed = 10.0f;
    //获取动画对象
    Animator animator;
    //是否在攻击状态
    bool isAttack = false;
    //获取两个子对象， 地面与攻击
    Transform groundTF;
    Transform attackTF;
    //声明当前连击数
    private int comboStep;
    //允许combo的时间，在此时间内按下键才可以连击
    private float interval = 1.0f;
    //计时器
    private float timer;
    //区分轻重攻击
    private string attackType;
    [SerializeField] private LayerMask layer;
    //功能和groundTf相同，判断是否在地面
    [SerializeField] private Vector3 check;
    //HP
    public int health;

    void Start()
    {
        //获取刚体
        rigidbody2D = GetComponent<Rigidbody2D>();
        //初始化两个子对象
        groundTF = transform.Find("ground");
        attackTF = transform.Find("attack");
        //获取动画器
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
        UpdateAttack();
    }

    //攻击动作判断
    public void UpdateAttack()
    {
        //已经在攻击中，直接返回，这样写无法连击
        //但是没有这一段，会让第二段攻击覆盖第一段
        if (isAttack)
        {
            return;
        }

        //如果按下攻击键且未在攻击状态
        if (Input.GetKeyDown(KeyCode.J) && !isAttack)
        {
            //设置攻击状态 类型 ++comboStep
            isAttack = true;
            attackType = "Light";
            comboStep++;
            //循环三次攻击
            if(comboStep > 3)
            {
                comboStep = 1;
            }
            //设置interval给timer
            timer = interval;
            switch (comboStep)
            {
                case 1:
                    animator.Play("player_atk1");
                    break;
                case 2:
                    animator.Play("player_atk2");
                    break;
                case 3:
                    animator.Play("player_atk3");
                    break;
            }

            //攻击完毕后返回AnyState 在攻击合适位置调用AttackEnd， 通常不会放在最后一帧，
            // 提高combo的连贯性
            Invoke("AttackEnd", 0.65f);
        }

        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                //重置comboStep和timer
                timer = 0;
                comboStep = 0;
            }
        }
    }

    //攻击检测
    void CheckAttackBox()
    {
        float width = 1.5f;
        float height = 2;
        //右上
        Vector2 pos1 = attackTF.position + attackTF.right * width * 0.5f + attackTF.up * height * 0.5f;
        //右下
        Vector2 pos2 = attackTF.position + attackTF.right * width * 0.5f - attackTF.up * height * 0.5f;
        //左上
        Vector2 pos3 = attackTF.position - attackTF.right * width * 0.5f + attackTF.up * height * 0.5f;
        //左下
        Vector2 pos4 = attackTF.position - attackTF.right * width * 0.5f - attackTF.up * height * 0.5f;
        Debug.DrawLine(pos1, pos2, Color.red, 0.25f);
        Debug.DrawLine(pos2, pos4, Color.red, 0.25f);
        Debug.DrawLine(pos4, pos3, Color.red, 0.25f);
        Debug.DrawLine(pos3, pos1, Color.red, 0.25f);

        //box盒子碰撞检测
        Collider2D col = Physics2D.OverlapBox(attackTF.position, 
            new Vector2(width, height), 0,LayerMask.GetMask("bullet"));
        //如果检测到bullet
        if(col != null && transform.position.x > col.transform.position.x)
        {
            //抖动摄像机
            CameraShake.Instance.DoShake(0.06f, 0.35f);
            //bullet转向
            col.GetComponent<Bullet>().Flip();
            //添加时停效果
            SlowTime(0.15f);
            
        }

    }

    void AttackEnd()
    {
        isAttack = false;
    }

    //添加动作
    public void LateUpdate()
    {
        if (isAttack)
        {
            return;
        }

        if(isInground() == true)
        {
            //在地上有idel和move两种状态
            if (rigidbody2D.velocity.x == 0)
            {
                animator.Play("Idle");
            }
            else
            {
                animator.Play("Run");
            }
        }
        else
        {
            if (rigidbody2D.velocity.y > 0)
            {
                animator.Play("Jump");
            }
            else
            {
                animator.Play("Fall");
            }
        }
        
    }

    public void UpdatePosition()
    {
        //获取输入的水平轴方向的大小
        input_x = Input.GetAxis("Horizontal");

        if(input_x != 0)
        {
            //改缩放比例， 镜像 转向
            transform.localScale = new Vector3(Mathf.Sign(input_x), 1, 1);
        }

        //用户按下空格时，改变对象的y值
        if (Input.GetKeyDown(KeyCode.Space)&& isInground()==true)
        {
            //x轴不变，y轴改变
            //Debug.Log("get key down!");
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, JumpSpeed);
        }

        //更新刚体的水平位置
        if (!isAttack)
        {
            //如果不在攻击状态，正常移动
            rigidbody2D.velocity = new Vector2(input_x * speed, rigidbody2D.velocity.y);
        }
        else
        {
            // 如果是轻攻击添加移动补偿,方向由当前面朝的方向决定，而不是输入
            if (attackType == "Light")
            {
                rigidbody2D.velocity = new Vector2(transform.localScale.x * lightSpeed, rigidbody2D.velocity.y);
            }
        }
        
    }

    //判断是否在地面上
    public bool isInground()
    {
        return Physics2D.OverlapCircle(groundTF.position, 0.02f, LayerMask.GetMask("ground"));
    }

    private void SlowTime(float timer)
    {
        StopAllCoroutines();
        StartCoroutine(SlowTimeCo(timer));
    }

    public IEnumerator SlowTimeCo(float timer)
    {
        Time.timeScale = 0.25f;//修改时间
        while(timer >= 0)
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                break;
            }
            yield return null;
        }
        Time.timeScale = 1;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null)
        {
            Debug.Log(other.gameObject);
            //如果获取到了敌人
            if (other.CompareTag("Enemy"))
            {
                if(attackType == "Light")
                {
                    CameraShake.Instance.DoShake(lightStrength, lightSpeed);
                    AttackScene.Instance.HitPause(lightPause);
                }
                //CameraShake.Instance.DoShake(0.06f, 0.35f);
                
                //传递玩家的朝向，调用敌人被击中的逻辑
                if (transform.localScale.x > 0)
                {
                    //玩家朝右侧击打
                    other.GetComponent<enemy.MyFSM>().GetHit(Vector2.right);
                }
                else if (transform.localScale.x < 0)
                {
                    other.GetComponent<enemy.MyFSM>().GetHit(Vector2.left);
                }

            }
            //检测到bullet逻辑
            if (other.CompareTag("Bullet") &&
                transform.position.x > other.transform.position.x)
            {
                //抖动摄像机
                //CameraShake.Instance.DoShake(0.06f, 0.35f);
                CameraShake.Instance.DoShake(lightStrength, lightSpeed);
                AttackScene.Instance.HitPause(lightPause);
                //bullet转向
                other.GetComponent<Bullet>().Flip();
                //添加时停效果
                SlowTime(0.15f);
            }
        }
    }

    //获取cherry
    public void CollectCherry()
    {
        health++;
        //TODO: 播放音效
    }
}
