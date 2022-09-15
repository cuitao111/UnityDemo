using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Gamekit2D
{
    public enum OnWallState
    {
        GRAB,
        SLIDE,
        CLIMB
    }

    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class CharacterController2D : MonoBehaviour
    {
        [Tooltip("The Layers which represent gameobjects that the Character Controller can be grounded on.")]
        public LayerMask groundedLayerMask;         //地面所在层
        [Tooltip("The distance down to check for ground.")]
        public float groundedRaycastDistance = 0.1f;

        public LayerMask sharppointLayerMask;       //刺所在的层


        Rigidbody2D m_Rigidbody2D;
        CapsuleCollider2D m_Capsule;
        Vector2 m_PreviousPosition;
        Vector2 m_CurrentPosition;
        Vector2 m_NextMovement;
        ContactFilter2D m_ContactFilter;
        RaycastHit2D[] m_HitBuffer = new RaycastHit2D[5];           //设置接受返回结果最大为5个
        RaycastHit2D[] m_FoundHits = new RaycastHit2D[3];
        Collider2D[] m_GroundColliders = new Collider2D[3];
        Vector2[] m_RaycastPositions = new Vector2[3];

        public bool onRightWall;  //右墙
        public bool onLeftWall;  //左墙
        public int wallSide;      //与墙体的位置关系

        [Header("Collision")]
        public Transform groundTF;
        //用于判断左右两侧是否接触墙体
        public Vector3 wallOffset;
        public float collisionRadius = 0.25f; //判断距离

        public bool onWall { get; protected set; }  //是否在墙上
        public bool IsGrounded { get; protected set; }
        public bool IsCeilinged { get; protected set; }
        public bool cornerCorrect { get; protected set; }       //跳跃修正
        public Vector2 Velocity { get; protected set; }
        public Rigidbody2D Rigidbody2D { get { return m_Rigidbody2D; } }
        public Collider2D[] GroundColliders { get { return m_GroundColliders; } }
        public ContactFilter2D ContactFilter { get { return m_ContactFilter; } }

        public float raycastLength;             //射线长度
        public Vector3 cornerRaycastPos;        //外测射线
        public Vector3 innerRaycastPost;        //内测
        //public bool cornerCorrect;              //是否进行修正

        void Awake()
        {
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_Capsule = GetComponent<CapsuleCollider2D>();

            m_CurrentPosition = m_Rigidbody2D.position;
            m_PreviousPosition = m_Rigidbody2D.position;

            m_ContactFilter.layerMask = groundedLayerMask;
            m_ContactFilter.useLayerMask = true;                //使用layer过滤
            m_ContactFilter.useTriggers = false;

            Physics2D.queriesStartInColliders = false;
        }

        void FixedUpdate()
        {
            m_PreviousPosition = m_Rigidbody2D.position;
            m_CurrentPosition = m_PreviousPosition + m_NextMovement;
            Velocity = (m_CurrentPosition - m_PreviousPosition) / Time.deltaTime;

            m_Rigidbody2D.MovePosition(m_CurrentPosition);
            m_NextMovement = Vector2.zero;

            //更新IsGrounded状态 默认为脚底检测，false为头顶检测，先注释掉头顶
            CheckCapsuleEndCollisions();
            //CheckCapsuleEndCollisions(false);
            CheckOnWall();          //判断左右墙体

            CheckOnShapPoint();
            RaycastCollision();
            if (cornerCorrect)
            {
                CornerCorrect(m_Rigidbody2D.velocity.y);
            }
        }

        /// <summary>
        /// This moves a rigidbody and so should only be called from FixedUpdate or other Physics messages.
        /// </summary>
        /// <param name="movement">The amount moved in global coordinates relative to the rigidbody2D's position.</param>
        public void Move(Vector2 movement)
        {
            m_NextMovement += movement;
        }

        /// <summary>
        /// This moves the character without any implied velocity.
        /// </summary>
        /// <param name="position">The new position of the character in global space.</param>
        public void Teleport(Vector2 position)
        {
            Vector2 delta = position - m_CurrentPosition;
            m_PreviousPosition += delta;
            m_CurrentPosition = position;
            m_Rigidbody2D.MovePosition(position);
        }

        /// <summary>
        /// This updates the state of IsGrounded.  It is called automatically in FixedUpdate but can be called more frequently if higher accurracy is required.
        /// </summary>
        /// 
        
        public void CheckCapsuleEndCollisions(bool bottom = true)
        {
            Vector2 raycastDirection;
            Vector2 raycastStart;
            float raycastDistance;

            if (m_Capsule == null)
            {
                //没有碰撞体
                raycastStart = m_Rigidbody2D.position + Vector2.up;         //rb上方
                raycastDistance = 1f + groundedRaycastDistance;             //1.1

                if (bottom)
                {
                    //更新raycastDirection和m_RaycastPositions的值
                    raycastDirection = Vector2.down;

                    m_RaycastPositions[0] = raycastStart + Vector2.left * 0.4f;
                    m_RaycastPositions[1] = raycastStart;
                    m_RaycastPositions[2] = raycastStart + Vector2.right * 0.4f;
                }
                else
                {
                    raycastDirection = Vector2.up;

                    m_RaycastPositions[0] = raycastStart + Vector2.left * 0.4f;
                    m_RaycastPositions[1] = raycastStart;
                    m_RaycastPositions[2] = raycastStart + Vector2.right * 0.4f;
                }
            }
            else
            {
                //更新raycastStart和raycastDistance
                raycastStart = m_Rigidbody2D.position + m_Capsule.offset;
                raycastDistance = m_Capsule.size.x * 0.5f + groundedRaycastDistance * 2f;

                if (bottom)
                {
                    raycastDirection = Vector2.down;
                    Vector2 raycastStartBottomCentre = raycastStart + Vector2.down * (m_Capsule.size.y * 0.5f - m_Capsule.size.x * 0.5f);

                    m_RaycastPositions[0] = raycastStartBottomCentre + Vector2.left * m_Capsule.size.x * 0.5f;
                    m_RaycastPositions[1] = raycastStartBottomCentre;
                    m_RaycastPositions[2] = raycastStartBottomCentre + Vector2.right * m_Capsule.size.x * 0.5f;
                }
                else
                {
                    raycastDirection = Vector2.up;
                    Vector2 raycastStartTopCentre = raycastStart + Vector2.up * (m_Capsule.size.y * 0.5f - m_Capsule.size.x * 0.5f);

                    m_RaycastPositions[0] = raycastStartTopCentre + Vector2.left * m_Capsule.size.x * 0.5f;
                    m_RaycastPositions[1] = raycastStartTopCentre;
                    m_RaycastPositions[2] = raycastStartTopCentre + Vector2.right * m_Capsule.size.x * 0.5f;
                }
            }

            for (int i = 0; i < m_RaycastPositions.Length; i++)
            {
                //在顶部和脚部各三个点发出射线，头顶向上， 脚底向下
                int count = Physics2D.Raycast(m_RaycastPositions[i], raycastDirection, m_ContactFilter, m_HitBuffer, raycastDistance);

                if (bottom)
                {
                    //m_FoundHits三个位置全部设置为 m_HitBuffer[0]
                    m_FoundHits[i] = count > 0 ? m_HitBuffer[0] : new RaycastHit2D();
                    m_GroundColliders[i] = m_FoundHits[i].collider;      //设置对应的collider
                }
                //else
                //{
                //    IsCeilinged = false;

                //    for (int j = 0; j < m_HitBuffer.Length; j++)
                //    {
                //        if (m_HitBuffer[j].collider != null)
                //        {
                //            if (!PhysicsHelper.ColliderHasPlatformEffector(m_HitBuffer[j].collider))
                //            {
                //                IsCeilinged = true;
                //            }
                //        }
                //    }
                //}
            }

            if (bottom)
            {
                Vector2 groundNormal = Vector2.zero;
                int hitCount = 0;

                for (int i = 0; i < m_FoundHits.Length; i++)
                {
                    if (m_FoundHits[i].collider != null)
                    {
                        groundNormal += m_FoundHits[i].normal;
                        hitCount++;
                    }
                }

                if (hitCount > 0)
                {
                    groundNormal.Normalize();
                }

                //踩到移动平台
                Vector2 relativeVelocity = Velocity;
                /*
                for (int i = 0; i < m_GroundColliders.Length; i++)
                {
                    if (m_GroundColliders[i] == null)
                        continue;

                    MovingPlatform movingPlatform;

                    if (PhysicsHelper.TryGetMovingPlatform(m_GroundColliders[i], out movingPlatform))
                    {
                        relativeVelocity -= movingPlatform.Velocity / Time.deltaTime;
                        break;
                    }
                }*/

                if (Mathf.Approximately(groundNormal.x, 0f) && Mathf.Approximately(groundNormal.y, 0f))
                {
                    IsGrounded = false;
                }
                else
                {
                    IsGrounded = relativeVelocity.y <= 0f;

                    if (m_Capsule != null)
                    {
                        if (m_GroundColliders[1] != null)
                        {
                            float capsuleBottomHeight = m_Rigidbody2D.position.y + m_Capsule.offset.y - m_Capsule.size.y * 0.5f;
                            float middleHitHeight = m_FoundHits[1].point.y;
                            IsGrounded &= middleHitHeight < capsuleBottomHeight + groundedRaycastDistance;
                        }
                    }
                }
            }

            for (int i = 0; i < m_HitBuffer.Length; i++)
            {
                m_HitBuffer[i] = new RaycastHit2D();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void CheckOnWall()
        {
            //更新与墙体的关系
            onLeftWall = Physics2D.OverlapCircle(transform.position - wallOffset, collisionRadius, groundedLayerMask);
            onRightWall = Physics2D.OverlapCircle(transform.position + wallOffset, collisionRadius, groundedLayerMask);
            onWall = onLeftWall || onRightWall;

            wallSide = onRightWall ? 1 : -1;
        }

        //检测是否与刺碰撞
        public void CheckOnShapPoint()
        {

        }

        //更新跳跃修正状态
        public void RaycastCollision()
        {
            cornerCorrect = Physics2D.Raycast(transform.position + cornerRaycastPos, Vector2.up, raycastLength, groundedLayerMask) &&
                    !Physics2D.Raycast(transform.position + innerRaycastPost, Vector2.up,groundedLayerMask) ||
                    Physics2D.Raycast(transform.position - cornerRaycastPos, Vector2.up, raycastLength, groundedLayerMask) &&
                    !Physics2D.Raycast(transform.position - innerRaycastPost, Vector2.up, groundedLayerMask);
        }

        //头部跳跃修正
        public void CornerCorrect(float Yvelocity)
        {
            //左侧发出射线
            RaycastHit2D hit = Physics2D.Raycast(transform.position - innerRaycastPost + Vector3.up * raycastLength,
                Vector3.left, raycastLength, groundedLayerMask);
            if(hit.collider != null)
            {
                float newPos = hit.point.x - (transform.position.x - cornerRaycastPos.x);
                transform.position = new Vector3(transform.position.x + newPos, transform.position.y, 0);
                m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, Yvelocity);
                return;
            }

            //右侧发出射线进行计算偏移
            hit = Physics2D.Raycast(transform.position + innerRaycastPost + Vector3.up * raycastLength,
                Vector3.left, raycastLength, groundedLayerMask);
            if (hit.collider != null)
            {
                float newPos = hit.point.x - (transform.position.x - cornerRaycastPos.x);
                transform.position = new Vector3(transform.position.x + newPos, transform.position.y, 0);
                m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, Yvelocity);
                return;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other != null)
            {
                Debug.Log(other.gameObject);
                //如果获取到了敌人
                if (other.CompareTag("Enemy"))
                {

                    CameraShake.Instance.DoShake(0.06f, 0.35f);
                    AttackScene.Instance.HitPause(2);

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
                    CameraShake.Instance.DoShake(0.06f, 0.35f);
                    AttackScene.Instance.HitPause(2);
                    //bullet转向
                    other.GetComponent<Bullet>().Flip();
                    //添加时停效果
                    SlowTime(0.15f);
                }
            }
        }

        private void SlowTime(float timer)
        {
            StopAllCoroutines();
            StartCoroutine(SlowTimeCo(timer));
        }

        public IEnumerator SlowTimeCo(float timer)
        {
            Time.timeScale = 0.25f;//修改时间
            while (timer >= 0)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    break;
                }
                yield return null;
            }
            Time.timeScale = 1;
        }

        //在窗口中绘制攻击范围圆心，与左右触墙判定
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(m_RaycastPositions[0], collisionRadius);
            //Gizmos.DrawWireSphere(celling.position, 1f);// MyFSM中用于判定是否可以起身的TF
            Gizmos.DrawWireSphere(m_RaycastPositions[1], collisionRadius);
            Gizmos.DrawWireSphere(m_RaycastPositions[2], collisionRadius);
            Gizmos.DrawWireSphere(transform.position - wallOffset, collisionRadius);
            Gizmos.DrawWireSphere(transform.position + wallOffset, collisionRadius);
            Gizmos.DrawLine(transform.position + cornerRaycastPos, transform.position + cornerRaycastPos + Vector3.up * raycastLength);
            Gizmos.DrawLine(transform.position - cornerRaycastPos, transform.position - cornerRaycastPos + Vector3.up * raycastLength);
            Gizmos.DrawLine(transform.position + innerRaycastPost, transform.position + innerRaycastPost + Vector3.up * raycastLength);
            Gizmos.DrawLine(transform.position - innerRaycastPost, transform.position - innerRaycastPost + Vector3.up * raycastLength);
        }
    }
}