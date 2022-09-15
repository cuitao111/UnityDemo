using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("�����ٶ�")]
    public float lightSpeed;
    [Header("�����")]
    public float shakeTime;
    public int lightPause;
    public float lightStrength;
    [Space]

    //����ˮƽ���ֵ ��Ϊ��Ա ����ʹ��
    private float input_x;
    //��ȡ����
    Rigidbody2D rigidbody2D;
    //�ٶ�
    public float speed = 3.0f;
    //��Ծ�ٶ�
    public float JumpSpeed = 10.0f;
    //��ȡ��������
    Animator animator;
    //�Ƿ��ڹ���״̬
    bool isAttack = false;
    //��ȡ�����Ӷ��� �����빥��
    Transform groundTF;
    Transform attackTF;
    //������ǰ������
    private int comboStep;
    //����combo��ʱ�䣬�ڴ�ʱ���ڰ��¼��ſ�������
    private float interval = 1.0f;
    //��ʱ��
    private float timer;
    //�������ع���
    private string attackType;
    [SerializeField] private LayerMask layer;
    //���ܺ�groundTf��ͬ���ж��Ƿ��ڵ���
    [SerializeField] private Vector3 check;
    //HP
    public int health;

    void Start()
    {
        //��ȡ����
        rigidbody2D = GetComponent<Rigidbody2D>();
        //��ʼ�������Ӷ���
        groundTF = transform.Find("ground");
        attackTF = transform.Find("attack");
        //��ȡ������
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
        UpdateAttack();
    }

    //���������ж�
    public void UpdateAttack()
    {
        //�Ѿ��ڹ����У�ֱ�ӷ��أ�����д�޷�����
        //����û����һ�Σ����õڶ��ι������ǵ�һ��
        if (isAttack)
        {
            return;
        }

        //������¹�������δ�ڹ���״̬
        if (Input.GetKeyDown(KeyCode.J) && !isAttack)
        {
            //���ù���״̬ ���� ++comboStep
            isAttack = true;
            attackType = "Light";
            comboStep++;
            //ѭ�����ι���
            if(comboStep > 3)
            {
                comboStep = 1;
            }
            //����interval��timer
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

            //������Ϻ󷵻�AnyState �ڹ�������λ�õ���AttackEnd�� ͨ������������һ֡��
            // ���combo��������
            Invoke("AttackEnd", 0.65f);
        }

        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                //����comboStep��timer
                timer = 0;
                comboStep = 0;
            }
        }
    }

    //�������
    void CheckAttackBox()
    {
        float width = 1.5f;
        float height = 2;
        //����
        Vector2 pos1 = attackTF.position + attackTF.right * width * 0.5f + attackTF.up * height * 0.5f;
        //����
        Vector2 pos2 = attackTF.position + attackTF.right * width * 0.5f - attackTF.up * height * 0.5f;
        //����
        Vector2 pos3 = attackTF.position - attackTF.right * width * 0.5f + attackTF.up * height * 0.5f;
        //����
        Vector2 pos4 = attackTF.position - attackTF.right * width * 0.5f - attackTF.up * height * 0.5f;
        Debug.DrawLine(pos1, pos2, Color.red, 0.25f);
        Debug.DrawLine(pos2, pos4, Color.red, 0.25f);
        Debug.DrawLine(pos4, pos3, Color.red, 0.25f);
        Debug.DrawLine(pos3, pos1, Color.red, 0.25f);

        //box������ײ���
        Collider2D col = Physics2D.OverlapBox(attackTF.position, 
            new Vector2(width, height), 0,LayerMask.GetMask("bullet"));
        //�����⵽bullet
        if(col != null && transform.position.x > col.transform.position.x)
        {
            //���������
            CameraShake.Instance.DoShake(0.06f, 0.35f);
            //bulletת��
            col.GetComponent<Bullet>().Flip();
            //���ʱͣЧ��
            SlowTime(0.15f);
            
        }

    }

    void AttackEnd()
    {
        isAttack = false;
    }

    //��Ӷ���
    public void LateUpdate()
    {
        if (isAttack)
        {
            return;
        }

        if(isInground() == true)
        {
            //�ڵ�����idel��move����״̬
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
        //��ȡ�����ˮƽ�᷽��Ĵ�С
        input_x = Input.GetAxis("Horizontal");

        if(input_x != 0)
        {
            //�����ű����� ���� ת��
            transform.localScale = new Vector3(Mathf.Sign(input_x), 1, 1);
        }

        //�û����¿ո�ʱ���ı�����yֵ
        if (Input.GetKeyDown(KeyCode.Space)&& isInground()==true)
        {
            //x�᲻�䣬y��ı�
            //Debug.Log("get key down!");
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, JumpSpeed);
        }

        //���¸����ˮƽλ��
        if (!isAttack)
        {
            //������ڹ���״̬�������ƶ�
            rigidbody2D.velocity = new Vector2(input_x * speed, rigidbody2D.velocity.y);
        }
        else
        {
            // ������ṥ������ƶ�����,�����ɵ�ǰ�泯�ķ������������������
            if (attackType == "Light")
            {
                rigidbody2D.velocity = new Vector2(transform.localScale.x * lightSpeed, rigidbody2D.velocity.y);
            }
        }
        
    }

    //�ж��Ƿ��ڵ�����
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
        Time.timeScale = 0.25f;//�޸�ʱ��
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
            //�����ȡ���˵���
            if (other.CompareTag("Enemy"))
            {
                if(attackType == "Light")
                {
                    CameraShake.Instance.DoShake(lightStrength, lightSpeed);
                    AttackScene.Instance.HitPause(lightPause);
                }
                //CameraShake.Instance.DoShake(0.06f, 0.35f);
                
                //������ҵĳ��򣬵��õ��˱����е��߼�
                if (transform.localScale.x > 0)
                {
                    //��ҳ��Ҳ����
                    other.GetComponent<enemy.MyFSM>().GetHit(Vector2.right);
                }
                else if (transform.localScale.x < 0)
                {
                    other.GetComponent<enemy.MyFSM>().GetHit(Vector2.left);
                }

            }
            //��⵽bullet�߼�
            if (other.CompareTag("Bullet") &&
                transform.position.x > other.transform.position.x)
            {
                //���������
                //CameraShake.Instance.DoShake(0.06f, 0.35f);
                CameraShake.Instance.DoShake(lightStrength, lightSpeed);
                AttackScene.Instance.HitPause(lightPause);
                //bulletת��
                other.GetComponent<Bullet>().Flip();
                //���ʱͣЧ��
                SlowTime(0.15f);
            }
        }
    }

    //��ȡcherry
    public void CollectCherry()
    {
        health++;
        //TODO: ������Ч
    }
}
