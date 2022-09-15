using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;
using UnityEngine.Tilemaps;
using TinyCeleste._01_Framework;
using TinyCeleste._02_Modules._03_Player._01_Action._03_Death;
using Cinemachine;
using DG.Tweening;

namespace Gamekit2D
{
    [RequireComponent(typeof(CharacterController2D))]
    [RequireComponent(typeof(Animator))]
    public class PlayerCharacter : Entity
    {
        static protected PlayerCharacter s_PlayerInstance;  //静态类
        static public PlayerCharacter PlayerInstance { get { return s_PlayerInstance; } } //静态属性

        /*        public InventoryController inventoryController   //库存控制器属性
                {
                    get { return m_InventoryController; }
                }*/

        public SpriteRenderer spriteRenderer;   //精灵渲染器
        public Damageable damageable;           //
        public Damager meleeDamager;
        public Transform facingLeftBulletSpawnPoint;
        public Transform facingRightBulletSpawnPoint;
        public Transform cameraFollowTarget;
        public Transform rebirthPosition;       //玩家重生位置

        public float maxSpeed = 10f;
        public float groundAcceleration = 100f;
        public float groundDeceleration = 100f;
        [Range(0f, 1f)] public float pushingSpeedProportion;

        [Range(0f, 1f)] public float airborneAccelProportion;
        [Range(0f, 1f)] public float airborneDecelProportion;
        public float gravity = 50f;
        public float jumpSpeed = 20f;
        public float jumpAbortSpeedReduction = 100f;
        public float fallMultiplier = 3.5f;             //下落参数
        public float lowJumpMultiplier = 2f;            //上升参数

        

        [Range(k_MinHurtJumpAngle, k_MaxHurtJumpAngle)] public float hurtJumpAngle = 45f;
        public float hurtJumpSpeed = 5f;
        public float flickeringDuration = 0.1f;

        [Header("OnWall")]
        private float slideSpeed = 5;
        private float climbSpeed = 5;


        private int m_ComboStep = 0;                //连击数
        private float interval = 2f;                //连击计时
        private float comboTimer;                   //连击计时器
        private bool isAttack = false;
        public float atkSpeed = 3f;                 //攻击和下劈过程中的补偿速度
        public float meleeAttackDashSpeed = 5f;      
        public bool dashWhileAirborne = false;

        public float dashSpeed = 10f;               //冲刺速度
        public float dashInterval = 0.1f;
        public float dashTimer = 0f;
        public int maxDashCount = 2;               //最大冲刺次数
        public int curDashCount;                    //当前冲刺次数
        public float dashStrength = 1;          //冲刺相机晃动幅度
        public float dashShakeSpeed = 1;          //冲刺相机晃动速度

        public bool isDead;                         //是否死亡
        

        //public RandomAudioPlayer footstepAudioPlayer;
        //public RandomAudioPlayer landingAudioPlayer;
        //public RandomAudioPlayer hurtAudioPlayer;
        //public RandomAudioPlayer meleeAttackAudioPlayer;
        //public RandomAudioPlayer rangedAttackAudioPlayer;

        public float shotsPerSecond = 1f;
        public float bulletSpeed = 5f;
        public float holdingGunTimeoutDuration = 10f;
        public bool rightBulletSpawnPointAnimated = true;

        public float cameraHorizontalFacingOffset;
        public float cameraHorizontalSpeedOffset;
        public float cameraVerticalInputOffset;
        public float maxHorizontalDeltaDampTime;
        public float maxVerticalDeltaDampTime;
        public float verticalCameraOffsetDelay;

        public bool spriteOriginallyFacesLeft;

        public ParticleSystem dashParticle;
        protected CinemachineCollisionImpulseSource m_Inpulse;
        protected Death m_Death;                                                             //用于控制死亡的逻辑的组件
        protected CharacterController2D m_CharacterController2D;
        protected Animator m_Animator;
        protected CapsuleCollider2D m_Capsule;
        protected Transform m_Transform;
        protected Vector2 m_MoveVector;
        protected List<Pushable> m_CurrentPushables = new List<Pushable>(4);
        protected Pushable m_CurrentPushable;
        protected float m_TanHurtJumpAngle;
        protected WaitForSeconds m_FlickeringWait;
        protected Coroutine m_FlickerCoroutine;
        protected Transform m_CurrentBulletSpawnPoint;
        protected float m_ShotSpawnGap;
        protected WaitForSeconds m_ShotSpawnWait;
        protected Coroutine m_ShootingCoroutine;
        protected float m_NextShotTime;
        protected bool m_IsFiring;
        protected float m_ShotTimer;
        protected float m_HoldingGunTimeRemaining;
        protected TileBase m_CurrentSurface;
        protected float m_CamFollowHorizontalSpeed;
        protected float m_CamFollowVerticalSpeed;
        protected float m_VerticalCameraOffsetTimer;
        //protected InventoryController m_InventoryController;

        //protected Checkpoint m_LastCheckpoint = null;
        protected Vector2 m_StartingPosition = Vector2.zero;
        protected bool m_StartingFacingLeft = false;

        protected bool m_InPause = false;

        protected readonly int m_HashHorizontalSpeedPara = Animator.StringToHash("HorizontalSpeed");
        protected readonly int m_HashVerticalSpeedPara = Animator.StringToHash("VerticalSpeed");
        protected readonly int m_HashGroundedPara = Animator.StringToHash("Grounded");
        protected readonly int m_HashCrouchingPara = Animator.StringToHash("Crouching");
        protected readonly int m_HashPushingPara = Animator.StringToHash("Pushing");
        protected readonly int m_HashTimeoutPara = Animator.StringToHash("Timeout");
        protected readonly int m_HashRespawnPara = Animator.StringToHash("Respawn");
        protected readonly int m_HashDeadPara = Animator.StringToHash("Dead");
        protected readonly int m_HashHurtPara = Animator.StringToHash("Hurt");
        protected readonly int m_HashForcedRespawnPara = Animator.StringToHash("ForcedRespawn");
        protected readonly int m_HashMeleeAttackPara = Animator.StringToHash("MeleeAttack");
        protected readonly int m_HashComboStepPara = Animator.StringToHash("comboStep");
        protected readonly int m_HashHoldingGunPara = Animator.StringToHash("HoldingGun");
        protected readonly int m_HashOnWallPara = Animator.StringToHash("OnWall");
        protected readonly int m_HashGrabPara = Animator.StringToHash("Grab");
        protected readonly int m_HashSlidePara = Animator.StringToHash("Slide");
        protected readonly int m_HashClimbPara = Animator.StringToHash("Climb");
        protected readonly int m_HashcanMovePara = Animator.StringToHash("canMove");
        protected readonly int m_HashDashPara = Animator.StringToHash("Dash");
        protected readonly int m_HashisDashPara = Animator.StringToHash("hasDash");

        protected const float k_MinHurtJumpAngle = 0.001f;
        protected const float k_MaxHurtJumpAngle = 89.999f;
        protected const float k_GroundedStickingVelocityMultiplier = 3f;    // This is to help the character stick to vertically moving platforms.

        //used in non alloc version of physic function
        protected ContactPoint2D[] m_ContactsBuffer = new ContactPoint2D[16];

        // MonoBehaviour Messages - called by Unity internally.
        void Awake()
        {
            s_PlayerInstance = this;

            m_CharacterController2D = GetComponent<CharacterController2D>();
            m_Animator = GetComponent<Animator>();
            m_Capsule = GetComponent<CapsuleCollider2D>();
            m_Death = GetComponent<Death>();
            m_Inpulse = GetComponent<CinemachineCollisionImpulseSource>();
            m_Transform = transform;
            //m_InventoryController = GetComponent<InventoryController>();

            m_CurrentBulletSpawnPoint = spriteOriginallyFacesLeft ? facingLeftBulletSpawnPoint : facingRightBulletSpawnPoint;
        }

        void Start()
        {
            UpdateFacing(false);
            hurtJumpAngle = Mathf.Clamp(hurtJumpAngle, k_MinHurtJumpAngle, k_MaxHurtJumpAngle);
            m_TanHurtJumpAngle = Mathf.Tan(Mathf.Deg2Rad * hurtJumpAngle);
            m_FlickeringWait = new WaitForSeconds(flickeringDuration);

            meleeDamager.DisableDamage();

            m_ShotSpawnGap = 1f / shotsPerSecond;
            m_NextShotTime = Time.time;
            m_ShotSpawnWait = new WaitForSeconds(m_ShotSpawnGap);

            if (!Mathf.Approximately(maxHorizontalDeltaDampTime, 0f))
            {
                float maxHorizontalDelta = maxSpeed * cameraHorizontalSpeedOffset + cameraHorizontalFacingOffset;
                m_CamFollowHorizontalSpeed = maxHorizontalDelta / maxHorizontalDeltaDampTime;
            }

            if (!Mathf.Approximately(maxVerticalDeltaDampTime, 0f))
            {
                float maxVerticalDelta = cameraVerticalInputOffset;
                m_CamFollowVerticalSpeed = maxVerticalDelta / maxVerticalDeltaDampTime;
            }

            SceneLinkedSMB<PlayerCharacter>.Initialise(m_Animator, this);

            m_StartingPosition = transform.position;
            m_StartingFacingLeft = GetFacing() < 0.0f;
        }

        //如果发生碰撞将被推物添加到list中，为什么是个list
        void OnTriggerEnter2D(Collider2D other)
        {
            Pushable pushable = other.GetComponent<Pushable>();
            if (pushable != null)
            {
                m_CurrentPushables.Add(pushable);
            }
        }
        //移除被推物品
        void OnTriggerExit2D(Collider2D other)
        {
            Pushable pushable = other.GetComponent<Pushable>();
            if (pushable != null)
            {
                if (m_CurrentPushables.Contains(pushable))
                    m_CurrentPushables.Remove(pushable);
            }
        }

        void Update()
        {
            ////暂停键 Update只负责处理暂停逻辑
            //if (PlayerInput.Instance.Pause.Down)
            //{
            //    if (!m_InPause)
            //    {
            //        if (ScreenFader.IsFading)
            //            return;

            //        PlayerInput.Instance.ReleaseControl(false);
            //        PlayerInput.Instance.Pause.GainControl();
            //        m_InPause = true;
            //        Time.timeScale = 0;
            //        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("UIMenus", UnityEngine.SceneManagement.LoadSceneMode.Additive);
            //    }
            //    else
            //    {
            //        Unpause();
            //    }
            //}
        }

        void FixedUpdate()
        {
            m_CharacterController2D.Move(m_MoveVector * Time.deltaTime);
            m_Animator.SetFloat(m_HashHorizontalSpeedPara, m_MoveVector.x);
            m_Animator.SetFloat(m_HashVerticalSpeedPara, m_MoveVector.y);
            UpdateComboTimer();                 //更新连击计时器
            //UpdateBulletSpawnPointPositions();
            //UpdateCameraFollowTargetPosition();
        }
        //根据spriteRenderer.flipX 如果flipX为true return -1
        public float GetFacing()
        {
            return spriteRenderer.flipX != spriteOriginallyFacesLeft ? -1f : 1f;
        }

        public void TeleportToColliderBottom()
        {
            //获得脚底的Vector
            Vector2 colliderBottom = m_CharacterController2D.Rigidbody2D.position + m_Capsule.offset + Vector2.down * m_Capsule.size.y * 0.5f;
            m_CharacterController2D.Teleport(colliderBottom);
        }

        public void UpdateFacing()
        {
            bool faceLeft = PlayerInput.Instance.Horizontal.Value < 0f;
            bool faceRight = PlayerInput.Instance.Horizontal.Value > 0f;

            if (faceLeft)
            {
                spriteRenderer.flipX = !spriteOriginallyFacesLeft;
                m_CurrentBulletSpawnPoint = facingLeftBulletSpawnPoint;
            }
            else if (faceRight)
            {
                
                spriteRenderer.flipX = spriteOriginallyFacesLeft;
                m_CurrentBulletSpawnPoint = facingRightBulletSpawnPoint;
            }
        }
        //通过更改m_MoveVector,再通过CharacterController2D.Move移动
        public void GroundedHorizontalMovement(bool useInput, float speedScale = 1f)
        {
            float desiredSpeed = useInput ? PlayerInput.Instance.Horizontal.Value * maxSpeed * speedScale : 0f;
            float acceleration = useInput && PlayerInput.Instance.Horizontal.ReceivingInput ? groundAcceleration : groundDeceleration;
            m_MoveVector.x = Mathf.MoveTowards(m_MoveVector.x, desiredSpeed, acceleration * Time.deltaTime);
        }

        public void GroundedVerticalMovement()
        {
            m_MoveVector.y -= gravity * Time.deltaTime;

            if (m_MoveVector.y < -gravity * Time.deltaTime * k_GroundedStickingVelocityMultiplier)
            {
                m_MoveVector.y = -gravity * Time.deltaTime * k_GroundedStickingVelocityMultiplier;
            }
        }
        //根据输入设置蹲下的Trigger
        public void CheckForCrouching()
        {
            m_Animator.SetBool(m_HashCrouchingPara, PlayerInput.Instance.Vertical.Value < 0f);
        }

        //判断是否着地
        public bool CheckForGrounded()
        {
            //获取animator的ground变量
            bool wasGrounded = m_Animator.GetBool(m_HashGroundedPara);
            //获取角色是否在地上，CharacterController2D在FixedUpdate中更新该状态
            bool grounded = m_CharacterController2D.IsGrounded;
            //找出当前踩的TileBase,播放不同的声音
            /*if (grounded)
            {
                FindCurrentSurface();

                if (!wasGrounded && m_MoveVector.y < -1.0f)
                {//only play the landing sound if falling "fast" enough (avoid small bump playing the landing sound)
                    landingAudioPlayer.PlayRandomSound(m_CurrentSurface);
                }
            }
            else
                m_CurrentSurface = null;*/

            m_Animator.SetBool(m_HashGroundedPara, grounded);

            if (grounded)
            {
                curDashCount = maxDashCount;                //着地恢复冲刺次数
            }

            return grounded;
        }
        
        //判断跳跃键
        public bool CheckForJumpInput()
        {
            return PlayerInput.Instance.Jump.Down;
        }

        //更改竖直方向的移动
        public void SetVerticalMovement(float newVerticalMovement)
        {
            m_MoveVector.y = newVerticalMovement;
        }

        public bool CheckForMeleeAttackInput()
        {
            return PlayerInput.Instance.MeleeAttack.Down;
        }
        //idle jmp run调用此函数 
        public void MeleeAttack(bool isGround = true)
        {
            if (!isGround)
            {
                m_Animator.SetTrigger(m_HashMeleeAttackPara);
                return;
            }
            
            //如果已经在攻击态
            if (isAttack)
                return;
            //如果被AtkOver提前结束
            m_ComboStep++;
            if (m_ComboStep > 3)
            {
                m_ComboStep = 1;
            }
            comboTimer = interval;
            isAttack = true;
            m_Animator.SetInteger(m_HashComboStepPara, m_ComboStep);
            m_Animator.SetTrigger(m_HashMeleeAttackPara);
        }

        public void AttackOver()
        {
            isAttack = false;
        }

        public void UpdateFacing(bool faceLeft)
        {
            if (faceLeft)
            {
                spriteRenderer.flipX = !spriteOriginallyFacesLeft;
                m_CurrentBulletSpawnPoint = facingLeftBulletSpawnPoint;
            }
            else
            {
                spriteRenderer.flipX = spriteOriginallyFacesLeft;
                m_CurrentBulletSpawnPoint = facingRightBulletSpawnPoint;
            }
        }

        //更新跳跃状态
        public void UpdateJump()
        {
            //如果上升过程中一直按住跳跃键
            if (!PlayerInput.Instance.Jump.Held && m_MoveVector.y > 0.0f)
            {
                m_MoveVector.y -= jumpAbortSpeedReduction * Time.deltaTime;
            }
        }

        //跳跃过程中的水平移动
        public void AirborneHorizontalMovement()
        {
            float desiredSpeed = PlayerInput.Instance.Horizontal.Value * maxSpeed;
            //Debug.Log(desiredSpeed);
            float acceleration;

            if (PlayerInput.Instance.Horizontal.ReceivingInput)
                acceleration = groundAcceleration * airborneAccelProportion;
            else
                acceleration = groundDeceleration * airborneDecelProportion;
            //将m_MoveVector.x值向desiredSpeed趋近，但是不会超过acceleration * Time.deltaTime
            //Debug.LogFormat("desiredSpeed:{0}", desiredSpeed);
            //Debug.LogFormat("acceleration:{0}", acceleration);
            m_MoveVector.x = Mathf.MoveTowards(m_MoveVector.x, desiredSpeed, acceleration * Time.deltaTime);
            //Debug.Log(m_MoveVector.x);
        }
        

        //跳跃过程竖直移动
        public void AirborneVerticalMovement()
        {
            //如果速度接近0或者触碰到了天花板，则将速度设置为0
            if (Mathf.Approximately(m_MoveVector.y, 0f) || m_CharacterController2D.IsCeilinged && m_MoveVector.y > 0f)
            //if (m_CharacterController2D.IsCeilinged && m_MoveVector.y > 0f)
            {
                m_MoveVector.y = 0f;
            }
            //根据重力更新速度
            //m_MoveVector.y -= gravity * Time.deltaTime;
            if (m_MoveVector.y <= 0)
            {
                //下落
                m_MoveVector.y -= gravity * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (m_MoveVector.y > 0 && PlayerInput.Instance.Jump.Held)
            {
                m_MoveVector.y -= gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }

        public void EnableMeleeAttack()
        {

            meleeDamager.EnableDamage();
            meleeDamager.disableDamageAfterHit = true;
            //meleeAttackAudioPlayer.PlayRandomSound();
        }

        //根据传入的float增加移动速度
        public void SetHorizontalMovement(float newHorizontalMovement)
        {
            m_MoveVector.x = newHorizontalMovement;
        }

        //关闭攻击
        public void DisableMeleeAttack()
        {
            meleeDamager.DisableDamage();
        }

        //攻击时的补偿移动
        public void MeleeAtkHorizontalMovement()
        {
            //int direction = m_MoveVector.x == 0 ? 0 : m_MoveVector.x > 0 ? 1 : -1;
            int direction = spriteOriginallyFacesLeft ^ spriteRenderer.flipX ? -1: 1;
            m_MoveVector.x = direction * atkSpeed;
            
        }

        //更新连击的计时器
        public void UpdateComboTimer()
        {
            if (comboTimer != 0)
            {
                comboTimer -= Time.deltaTime;
                if (comboTimer <= 0)
                {
                    comboTimer = 0;
                    m_ComboStep = 0;
                }
            }
        }

        //判断是否触碰墙体
        public bool CheckForWall()
        {
            m_Animator.SetBool(m_HashOnWallPara, m_CharacterController2D.onWall);
            return m_CharacterController2D.onWall;
        }
        //检测抓墙键的输入
        public bool CheckFoGrabInput()
        {
            return PlayerInput.Instance.Grab.Held;
        }
        //执行
        public void WallGrab()
        {
            m_Animator.SetTrigger(m_HashGrabPara);
        }

        //检测墙上的是哪种状态
        public void CheckForOnWallState()
        {
            if (PlayerInput.Instance.Vertical.Value > 0 && PlayerInput.Instance.Grab.Held)
            {
                //爬墙状态
                m_Animator.SetTrigger(m_HashClimbPara);
                OnWallVerticalMovement();       //根据输入决定竖直方向的移动速度
            }
            else if (PlayerInput.Instance.Grab.Down || PlayerInput.Instance.Grab.Held)
            {
                m_Animator.SetTrigger(m_HashGrabPara);          //设置扒墙
                //更新重力为0
                gravity = 0;
                //如果有水平方向的输入
                if (PlayerInput.Instance.Horizontal.Value > .2f || PlayerInput.Instance.Horizontal.Value < -.2f)
                    m_MoveVector.y = 0;

                float speedModifier = PlayerInput.Instance.Vertical.Value > 0 ? .5f : 1;
                m_MoveVector.y = PlayerInput.Instance.Vertical.Value * (maxSpeed * speedModifier);
            }
            else
            {
                //滑墙状态
                m_Animator.SetTrigger(m_HashSlidePara);
                bool pushingWall = false;
                if ((m_MoveVector.x > 0 && m_CharacterController2D.onRightWall) || (m_MoveVector.x < 0 && m_CharacterController2D.onLeftWall))
                {
                    pushingWall = true;
                }
                float push = pushingWall ? 0 : m_MoveVector.x;

                m_MoveVector = new Vector2(push, -slideSpeed);
            }
        }
        //重置重力用于退出墙上状态
        public void ResetGravity()
        {
            gravity = 50f;
        }

        //墙上的竖直速度更新
        public void OnWallVerticalMovement(float speedScale = 1f)
        {
            float desiredSpeed = PlayerInput.Instance.Vertical.Value * climbSpeed * speedScale;
            float acceleration = PlayerInput.Instance.Vertical.ReceivingInput ? groundAcceleration : groundDeceleration;
            m_MoveVector.y = Mathf.MoveTowards(m_MoveVector.y, desiredSpeed, acceleration * Time.deltaTime);
        }
        //更新在墙上的朝向
        public void UpdateOnWallFacing()
        {
            bool faceLeft = m_CharacterController2D.onLeftWall;

            if (faceLeft)
            {
                spriteRenderer.flipX = !spriteOriginallyFacesLeft;
                m_CurrentBulletSpawnPoint = facingLeftBulletSpawnPoint;
            }
            else
            {

                spriteRenderer.flipX = spriteOriginallyFacesLeft;
                m_CurrentBulletSpawnPoint = facingRightBulletSpawnPoint;
            }
        }
        //蹬墙跳
        public void WallJump()
        {
            //翻转方向
            spriteRenderer.flipX = m_CharacterController2D.onRightWall ? !spriteOriginallyFacesLeft : spriteOriginallyFacesLeft;

            m_Animator.SetBool(m_HashOnWallPara, false);
            StopCoroutine(DisableMovement(0));
            StartCoroutine(DisableMovement(.1f));

            int wallDir = m_CharacterController2D.onRightWall ? -1 :1;

            m_MoveVector.y = jumpSpeed;
            m_MoveVector.x = wallDir * jumpSpeed;

        }
        //
        IEnumerator DisableMovement(float time)
        {
            m_Animator.SetBool(m_HashcanMovePara, false);
            yield return new WaitForSeconds(time);
            m_Animator.SetBool(m_HashcanMovePara, true);
        }
        //检测是否按下冲刺键
        public bool CheckForDashInput()
        {
            return PlayerInput.Instance.Dash.Down;
        }
        //冲刺
        public void Dash()
        {
            //hasDashed = true;
            if (curDashCount == 0) return;          //消耗光冲刺次数
            curDashCount--;

            m_Animator.SetTrigger(m_HashDashPara);
            m_Animator.SetBool(m_HashisDashPara, true);
            m_Inpulse.GenerateImpulse();
            FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));

            float x = PlayerInput.Instance.Horizontal.Value;
            float y = PlayerInput.Instance.Vertical.Value;

            if(x == 0 && y == 0)
            {
                x = GetFacing();
            }

            Vector2 dir = new Vector2(x, y);
            gravity = 0;

            m_MoveVector.x = dir.normalized.x * dashSpeed;
            m_MoveVector.y = dir.normalized.y * dashSpeed;

            
            StartCoroutine(DashWait());
        }

        //负责生成Ghost
        IEnumerator DashWait()
        {
            FindObjectOfType<GhostTrail>().ShowGhost();
            StartCoroutine(GroundDash());
            DOVirtual.Float(14, 0, .8f, RigidbodyDrag);

            dashParticle.Play();
            //gravity = 0;
            //GetComponent<BetterJumping>().enabled = false;
            //wallJumped = true;
            //m_Animator.SetBool(m_HashisDashPara, true);

            yield return new WaitForSeconds(.3f);

            dashParticle.Stop();
            //gravity = 3;
            //GetComponent<BetterJumping>().enabled = true;
            //wallJumped = false;
            //m_Animator.SetBool(m_HashisDashPara, false);
        }

        IEnumerator GroundDash()
        {
            yield return new WaitForSeconds(.15f);
            if (m_CharacterController2D.IsGrounded)
                m_Animator.SetBool(m_HashisDashPara, false);
        }

        void RigidbodyDrag(float x)
        {
            m_CharacterController2D.Rigidbody2D.drag = x;
        }

        public void InitDashTimer()
        {
            dashTimer = dashInterval;
            
        }
        //更新冲刺timer
        public void UpdateDashTimer()
        {
            if(dashTimer >= 0)
            {
                dashTimer -= Time.deltaTime;
                if(dashTimer < 0)
                {
                    if(!CheckForGrounded())
                        m_MoveVector.y = 0.5f;
                    m_MoveVector.x = 0.5f;
                    //AirborneHorizontalMovement();
                    m_Animator.SetBool(m_HashisDashPara, false);
                    
                }
            }
        }
        //更新dash状态
        public void SetDashState(bool isDash)
        {
            m_Animator.SetBool(m_HashisDashPara, isDash);
        }

        //直接赋予某个方向上的速度
        public bool BeEjected(Vector2 velocity)
        {

            //重置状态
            CheckForGrounded();
            //CheckForWall();
            //结束Dash状态
            dashTimer = -1;
            m_Animator.SetBool(m_HashisDashPara, false);
            m_MoveVector.x = velocity.x;
            m_MoveVector.y = velocity.y;
            //恢复冲刺次数
            curDashCount = maxDashCount;
            return true;
        }

        //重置冲刺次数
        public bool ResumeDashCount()
        {
            bool res = curDashCount < maxDashCount;
            curDashCount = maxDashCount;
            return true;
        }

        //玩家受伤判断
        public void DamageSystem()
        {
            //1.获取的到了回车键
            //2.碰到了尖刺
            m_MoveVector = Vector2.zero;
            m_Death.Die();
        }

        //将玩家移动到出生位置
        // 瞬间移动（非物理移动）
        // 物理移动应去改变Rigidbody的velocity
        public void SetRebirthPosition()
        {
            transform.position = rebirthPosition.position;
            curDashCount = maxDashCount;
        }

        public void CheckForDeath()
        {

        }

        //重新设置重生点位置
        public void ResetRebirthPos(Transform transform)
        {
            rebirthPosition.position = transform.position;
        }

        //蹭墙跳，设定速度为蹬墙跳速度
        public void JumpInDash()
        {

        }

        /*
         *
        //将持枪状态设置为false
        public void ForceNotHoldingGun()
        {
            m_Animator.SetBool(m_HashHoldingGunPara, false);
        }

        //检查是否在推东西
        public void CheckForPushing()
        {
            bool pushableOnCorrectSide = false;                 //初始化变量
            Pushable previousPushable = m_CurrentPushable;      //之前被推的物品

            m_CurrentPushable = null;

            if (m_CurrentPushables.Count > 0)
            {
                //判读输入左右
                bool movingRight = PlayerInput.Instance.Horizontal.Value > float.Epsilon;
                bool movingLeft = PlayerInput.Instance.Horizontal.Value < -float.Epsilon;
                //逐一判断m_CurrentPushables是否处于正确的位置，找到第一个赋值给CurrentPushable
                for (int i = 0; i < m_CurrentPushables.Count; i++)
                {
                    float pushablePosX = m_CurrentPushables[i].pushablePosition.position.x;
                    float playerPosX = m_Transform.position.x;
                    if (pushablePosX < playerPosX && movingLeft || pushablePosX > playerPosX && movingRight)
                    {
                        pushableOnCorrectSide = true;
                        m_CurrentPushable = m_CurrentPushables[i];
                        break;
                    }
                }
                //如果找到了，更新角色的位置，y值不变
                if (pushableOnCorrectSide)
                {
                    Vector2 moveToPosition = movingRight ? m_CurrentPushable.playerPushingRightPosition.position : m_CurrentPushable.playerPushingLeftPosition.position;
                    moveToPosition.y = m_CharacterController2D.Rigidbody2D.position.y;
                    m_CharacterController2D.Teleport(moveToPosition);
                }
            }
            //如果更换了push物体，将之前的设置为end
            if (previousPushable != null && m_CurrentPushable != previousPushable)
            {//we changed pushable (or don't have one anymore), stop the old one sound
                previousPushable.EndPushing();
            }
            //设置animator的变量
            m_Animator.SetBool(m_HashPushingPara, pushableOnCorrectSide);
        }

        //检测两种条件下启动协程
        public void CheckAndFireGun()
        {
            if (PlayerInput.Instance.RangedAttack.Held && m_Animator.GetBool(m_HashHoldingGunPara))
            {
                if (m_ShootingCoroutine == null)
                    m_ShootingCoroutine = StartCoroutine(Shoot());
            }
            //对应按键弹起
            if ((PlayerInput.Instance.RangedAttack.Up || !m_Animator.GetBool(m_HashHoldingGunPara)) && m_ShootingCoroutine != null)
            {
                StopCoroutine(m_ShootingCoroutine);
                m_ShootingCoroutine = null;
            }
        }

        //检查是否持枪
        public bool CheckForHoldingGun()
        {
            bool holdingGun = false;

            if (PlayerInput.Instance.RangedAttack.Held)
            {
                holdingGun = true;
                m_Animator.SetBool(m_HashHoldingGunPara, true);
                m_HoldingGunTimeRemaining = holdingGunTimeoutDuration;
            }
            else
            {
                m_HoldingGunTimeRemaining -= Time.deltaTime;

                if (m_HoldingGunTimeRemaining <= 0f)
                {
                    m_Animator.SetBool(m_HashHoldingGunPara, false);
                }
            }

            return holdingGun;
        }
        //找到目前踩踏的地板，据此发出不同声音
        public void FindCurrentSurface()
        {
            Collider2D groundCollider = m_CharacterController2D.GroundColliders[0];

            if (groundCollider == null)
                groundCollider = m_CharacterController2D.GroundColliders[1];

            if (groundCollider == null)
                return;

            TileBase b = PhysicsHelper.FindTileForOverride(groundCollider, transform.position, Vector2.down);
            if (b != null)
            {
                m_CurrentSurface = b;
            }
        }

        public void Unpause()
        {
            //if the timescale is already > 0, we 
            if (Time.timeScale > 0)
                return;
            //当timescale is already > 0，不在暂停时间状态，启动协程
            StartCoroutine(UnpauseCoroutine());
        }

        protected IEnumerator UnpauseCoroutine()
        {
            Time.timeScale = 1;
            //关闭 UI
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("UIMenus");
            PlayerInput.Instance.GainControl();
            //we have to wait for a fixed update so the pause button state change, otherwise we can get in case were the update
            //of this script happen BEFORE the input is updated, leading to setting the game in pause once again
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();
            m_InPause = false;
        }





        // Protected functions.
        protected void UpdateBulletSpawnPointPositions()
        {
            if (rightBulletSpawnPointAnimated)
            {
                Vector2 leftPosition = facingRightBulletSpawnPoint.localPosition;
                leftPosition.x *= -1f;
                facingLeftBulletSpawnPoint.localPosition = leftPosition;
            }
            else
            {
                Vector2 rightPosition = facingLeftBulletSpawnPoint.localPosition;
                rightPosition.x *= -1f;
                facingRightBulletSpawnPoint.localPosition = rightPosition;
            }
        }

        protected void UpdateCameraFollowTargetPosition()
        {
            float newLocalPosX;
            float newLocalPosY = 0f;

            float desiredLocalPosX = (spriteOriginallyFacesLeft ^ spriteRenderer.flipX ? -1f : 1f) * cameraHorizontalFacingOffset;
            desiredLocalPosX += m_MoveVector.x * cameraHorizontalSpeedOffset;
            if (Mathf.Approximately(m_CamFollowHorizontalSpeed, 0f))
                newLocalPosX = desiredLocalPosX;
            else
                newLocalPosX = Mathf.Lerp(cameraFollowTarget.localPosition.x, desiredLocalPosX, m_CamFollowHorizontalSpeed * Time.deltaTime);

            bool moveVertically = false;
            if (!Mathf.Approximately(PlayerInput.Instance.Vertical.Value, 0f))
            {
                m_VerticalCameraOffsetTimer += Time.deltaTime;

                if (m_VerticalCameraOffsetTimer >= verticalCameraOffsetDelay)
                    moveVertically = true;
            }
            else
            {
                moveVertically = true;
                m_VerticalCameraOffsetTimer = 0f;
            }

            if (moveVertically)
            {
                float desiredLocalPosY = PlayerInput.Instance.Vertical.Value * cameraVerticalInputOffset;
                if (Mathf.Approximately(m_CamFollowVerticalSpeed, 0f))
                    newLocalPosY = desiredLocalPosY;
                else
                    newLocalPosY = Mathf.MoveTowards(cameraFollowTarget.localPosition.y, desiredLocalPosY, m_CamFollowVerticalSpeed * Time.deltaTime);
            }

            cameraFollowTarget.localPosition = new Vector2(newLocalPosX, newLocalPosY);
        }

        protected IEnumerator Flicker()
        {
            float timer = 0f;

            while (timer < damageable.invulnerabilityDuration)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                yield return m_FlickeringWait;
                timer += flickeringDuration;
            }

            spriteRenderer.enabled = true;
        }

        protected IEnumerator Shoot()
        {
            while (PlayerInput.Instance.RangedAttack.Held)
            {
                if (Time.time >= m_NextShotTime)
                {
                    SpawnBullet();
                    m_NextShotTime = Time.time + m_ShotSpawnGap;
                }
                yield return null;
            }
        }

        protected void SpawnBullet()
        {
            //we check if there is a wall between the player and the bullet spawn position, if there is, we don't spawn a bullet
            //otherwise, the player can "shoot throught wall" because the arm extend to the other side of the wall
            Vector2 testPosition = transform.position;
            testPosition.y = m_CurrentBulletSpawnPoint.position.y;
            Vector2 direction = (Vector2)m_CurrentBulletSpawnPoint.position - testPosition;
            float distance = direction.magnitude;
            direction.Normalize();

            RaycastHit2D[] results = new RaycastHit2D[12];
            if (Physics2D.Raycast(testPosition, direction, m_CharacterController2D.ContactFilter, results, distance) > 0)
                return;

            BulletObject bullet = bulletPool.Pop(m_CurrentBulletSpawnPoint.position);
            bool facingLeft = m_CurrentBulletSpawnPoint == facingLeftBulletSpawnPoint;
            bullet.rigidbody2D.velocity = new Vector2(facingLeft ? -bulletSpeed : bulletSpeed, 0f);
            bullet.spriteRenderer.flipX = facingLeft ^ bullet.bullet.spriteOriginallyFacesLeft;

            rangedAttackAudioPlayer.PlayRandomSound();
        }

        // Public functions - called mostly by StateMachineBehaviours in the character's Animator Controller but also by Events.
        public void SetMoveVector(Vector2 newMoveVector)
        {
            m_MoveVector = newMoveVector;
        }



        public void IncrementMovement(Vector2 additionalMovement)
        {
            m_MoveVector += additionalMovement;
        }

        public void IncrementHorizontalMovement(float additionalHorizontalMovement)
        {
            m_MoveVector.x += additionalHorizontalMovement;
        }

        public void IncrementVerticalMovement(float additionalVerticalMovement)
        {
            m_MoveVector.y += additionalVerticalMovement;
        }


        public Vector2 GetMoveVector()
        {
            return m_MoveVector;
        }

        public bool IsFalling()
        {
            return m_MoveVector.y < 0f && !m_Animator.GetBool(m_HashGroundedPara);
        }
        

        public void MovePushable()
        {
            //we don't push ungrounded pushable, avoid pushing floating pushable or falling pushable.
            if (m_CurrentPushable && m_CurrentPushable.Grounded)
                m_CurrentPushable.Move(m_MoveVector * Time.deltaTime);
        }

        public void StartPushing()
        {
            if (m_CurrentPushable)
                m_CurrentPushable.StartPushing();
        }

        public void StopPushing()
        {
            if(m_CurrentPushable)
                m_CurrentPushable.EndPushing();
        }



        public bool CheckForFallInput()
        {
            return PlayerInput.Instance.Vertical.Value < -float.Epsilon && PlayerInput.Instance.Jump.Down;
        }

        public bool MakePlatformFallthrough()
        {
            int colliderCount = 0;
            int fallthroughColliderCount = 0;
        
            for (int i = 0; i < m_CharacterController2D.GroundColliders.Length; i++)
            {
                Collider2D col = m_CharacterController2D.GroundColliders[i];
                if(col == null)
                    continue;

                colliderCount++;

                if (PhysicsHelper.ColliderHasPlatformEffector (col))
                    fallthroughColliderCount++;
            }

            if (fallthroughColliderCount == colliderCount)
            {
                for (int i = 0; i < m_CharacterController2D.GroundColliders.Length; i++)
                {
                    Collider2D col = m_CharacterController2D.GroundColliders[i];
                    if (col == null)
                        continue;

                    PlatformEffector2D effector;
                    PhysicsHelper.TryGetPlatformEffector (col, out effector);
                    FallthroughReseter reseter = effector.gameObject.AddComponent<FallthroughReseter>();
                    reseter.StartFall(effector);
                    //set invincible for half a second when falling through a platform, as it will make the player "standup"
                    StartCoroutine(FallThroughtInvincibility());
                }
            }

            return fallthroughColliderCount == colliderCount;
        }

        IEnumerator FallThroughtInvincibility()
        {
            damageable.EnableInvulnerability(true);
            yield return new WaitForSeconds(0.5f);
            damageable.DisableInvulnerability();
        }

        public void EnableInvulnerability()
        {
            damageable.EnableInvulnerability();
        }

        public void DisableInvulnerability()
        {
            damageable.DisableInvulnerability();
        }

        public Vector2 GetHurtDirection()
        {
            Vector2 damageDirection = damageable.GetDamageDirection();

            if (damageDirection.y < 0f)
                return new Vector2(Mathf.Sign(damageDirection.x), 0f);

            float y = Mathf.Abs(damageDirection.x) * m_TanHurtJumpAngle;

            return new Vector2(damageDirection.x, y).normalized;
        }

        public void OnHurt(Damager damager, Damageable damageable)
        {
            //if the player don't have control, we shouldn't be able to be hurt as this wouldn't be fair
            if (!PlayerInput.Instance.HaveControl)
                return;

            UpdateFacing(damageable.GetDamageDirection().x > 0f);
            damageable.EnableInvulnerability();

            m_Animator.SetTrigger(m_HashHurtPara);

            //we only force respawn if helath > 0, otherwise both forceRespawn & Death trigger are set in the animator, messing with each other.
            if(damageable.CurrentHealth > 0 && damager.forceRespawn)
                m_Animator.SetTrigger(m_HashForcedRespawnPara);

            m_Animator.SetBool(m_HashGroundedPara, false);
            hurtAudioPlayer.PlayRandomSound();

            //if the health is < 0, mean die callback will take care of respawn
            if(damager.forceRespawn && damageable.CurrentHealth > 0)
            {
                StartCoroutine(DieRespawnCoroutine(false, true));
            }
        }

        public void OnDie()
        {
            m_Animator.SetTrigger(m_HashDeadPara);

            StartCoroutine(DieRespawnCoroutine(true, false));
        }

        IEnumerator DieRespawnCoroutine(bool resetHealth, bool useCheckPoint)
        {
            PlayerInput.Instance.ReleaseControl(true);
            yield return new WaitForSeconds(1.0f); //wait one second before respawing
            yield return StartCoroutine(ScreenFader.FadeSceneOut(useCheckPoint ? ScreenFader.FadeType.Black : ScreenFader.FadeType.GameOver));
            if(!useCheckPoint)
                yield return new WaitForSeconds (2f);
            Respawn(resetHealth, useCheckPoint);
            yield return new WaitForEndOfFrame();
            yield return StartCoroutine(ScreenFader.FadeSceneIn());
            PlayerInput.Instance.GainControl();
        }

        public void StartFlickering()
        {
            m_FlickerCoroutine = StartCoroutine(Flicker());
        }

        public void StopFlickering()
        {
            StopCoroutine(m_FlickerCoroutine);
            spriteRenderer.enabled = true;
        }


        

        public void PlayFootstep()
        {
            footstepAudioPlayer.PlayRandomSound(m_CurrentSurface);
            var footstepPosition = transform.position;
            footstepPosition.z -= 1;
            VFXController.Instance.Trigger("DustPuff", footstepPosition, 0, false, null, m_CurrentSurface);
        }

        public void Respawn(bool resetHealth, bool useCheckpoint)
        {
            if (resetHealth)
                damageable.SetHealth(damageable.startingHealth);

            //we reset the hurt trigger, as we don't want the player to go back to hurt animation once respawned
            m_Animator.ResetTrigger(m_HashHurtPara);
            if (m_FlickerCoroutine != null)
            {//we stop flcikering for the same reason
                StopFlickering();
            }

            m_Animator.SetTrigger(m_HashRespawnPara);

            if (useCheckpoint && m_LastCheckpoint != null)
            {
                UpdateFacing(m_LastCheckpoint.respawnFacingLeft);
                GameObjectTeleporter.Teleport(gameObject, m_LastCheckpoint.transform.position);
            }
            else
            {
                UpdateFacing(m_StartingFacingLeft);
                GameObjectTeleporter.Teleport(gameObject, m_StartingPosition);
            }
        }

        public void SetChekpoint(Checkpoint checkpoint)
        {
            m_LastCheckpoint = checkpoint;
        }

        //This is called by the inventory controller on key grab, so it can update the Key UI.
        public void KeyInventoryEvent()
        {
            if (KeyUI.Instance != null) KeyUI.Instance.ChangeKeyUI(m_InventoryController);
        }*/
    }
}
