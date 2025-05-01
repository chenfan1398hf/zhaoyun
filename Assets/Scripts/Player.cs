using Cinemachine;
using System.Collections;
using UnityEngine;

/// <summary>
/// 高级玩家角色控制器
/// 功能包含：物理移动、跳跃、闪避、视角控制
/// </summary>
[RequireComponent(typeof(Rigidbody))] // 强制要求Rigidbody组件
public class AdvancedPlayerController : MonoBehaviour
{
    // region将代码折叠分组（需要支持C# 10.0+）
    #region 可配置参数

    [Header("移动参数")]
    [Tooltip("基础移动速度（米/秒）")]
    [SerializeField] private float moveSpeed = 6f;
    private float oldMoveSpeed = 0f;

    [Tooltip("空中移动速度乘数（0-1）")]
    [SerializeField] private float airMultiplier = 0.4f;

    [Tooltip("跳跃力度")]
    [SerializeField] private float jumpForce = 12f;

    [Tooltip("闪避冲刺力度")]
    [SerializeField] private float dodgeForce = 20f;

    [Tooltip("闪避冷却时间（秒）")]
    [SerializeField] private float dodgeCooldown = 1.5f;

    [Tooltip("地面检测球体半径")]
    [SerializeField] private float groundCheckRadius = 0.3f;

    [Tooltip("地面层级掩码")]
    [SerializeField] private LayerMask groundMask;

    [Header("视角控制")]
    [Tooltip("鼠标灵敏度")]
    [SerializeField] private float mouseSensitivity = 100f;

    [Tooltip("摄像机旋转支点")]
    [SerializeField] private Transform cameraPivot;

    [Tooltip("最大仰角")]
    [SerializeField] private float maxLookAngle = 80f;

    [Tooltip("最大俯角")]
    [SerializeField] private float minLookAngle = -80f;
    private enum MoveAnimationState
    {
        Idle,
        Forward,
        ForwardLeft,
        ForwardRight,
        Backward,
        Left,
        Right,
        Jump,
        Attack = 30
    }
    [SerializeField] 
    private Animator characterAnimator;
    private MoveAnimationState currentAnimState;
    private MoveAnimationState previousAnimState;
    [SerializeField] 
    private float animationTransitionSpeed = 0.1f;
    #endregion

    #region 私有变量

    private Rigidbody rb;            // 刚体组件引用s
    private Vector3 moveDirection;   // 移动方向向量
    private float verticalRotation;  // 摄像机垂直旋转角度
    private bool isGrounded;         // 是否在地面
    private float lastDodgeTime;     // 上次闪避时间
    private bool isDodging;          // 是否处于闪避状态

    #endregion

    #region 生命周期方法

    /// <summary>
    /// 初始化组件
    /// </summary>
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // 锁定物理旋转

        // 锁定鼠标到屏幕中心
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        characterAnimator = this.transform.Find("Player").GetComponent<Animator>();
        composer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
    }

    /// <summary>
    /// 每帧更新逻辑（输入检测）
    /// </summary>
    void Update()
    {
        if (GameManager.instance.isBeginLevel)
        {
            GroundCheck();       // 地面检测
            GetInput();          // 获取输入
            HandleJump();       // 处理跳跃
            HandleDodge();       // 处理闪避
            HandleCameraRotation(); // 控制视角
            HandleAnimation(); // 新增动画处理
        }
    }

    /// <summary>
    /// 物理更新循环（处理移动）
    /// </summary>
    void FixedUpdate()
    {
        MovePlayer();
    }
    private bool isAttack = false;
    private void HandleAnimation()
    {
        if (isAttack)
        {
            return;
        }
        if (GameManager.instance.isBeginLevel == false)
        {
            return;
        }
        if (Input.GetMouseButton(0)) // 按住左键持续触发攻击
        {
            SetAnimationState(MoveAnimationState.Attack);
        }
        else if (Input.GetKey(KeyCode.Escape))
        {
            GameManager.instance.OpenEndPanel();
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            SetAnimationState(MoveAnimationState.Jump);
        }
        else
        {
            // 优先检测组合按键
            if (Input.GetKey(KeyCode.W))
            {
                if (Input.GetKey(KeyCode.A))
                {
                    SetAnimationState(MoveAnimationState.ForwardLeft);
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    SetAnimationState(MoveAnimationState.ForwardRight);
                }
                else
                {
                    SetAnimationState(MoveAnimationState.Forward);
                }
            }
            else if (Input.GetKey(KeyCode.S))
            {
                SetAnimationState(MoveAnimationState.Backward);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                SetAnimationState(MoveAnimationState.Left);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                SetAnimationState(MoveAnimationState.Right);
            }
            else
            {
                SetAnimationState(MoveAnimationState.Idle);
            }
        }
        

        UpdateAnimatorParameters();
    }
    private void SetAnimationState(MoveAnimationState newState)
    {
        if (currentAnimState != newState)
        {
            currentAnimState = newState;
        }
    }
    private void UpdateAnimatorParameters()
    {
        // 仅在状态变化时更新
        if (currentAnimState != previousAnimState)
        {
            if (previousAnimState >= MoveAnimationState.Attack)
            {
                isAttack = true;
                oldMoveSpeed = moveSpeed;
                moveSpeed = 0;
                rb.velocity = Vector3.zero;
                this.Invoke("IsAttack", 1.2f);
            }
            else if (previousAnimState == MoveAnimationState.Jump)
            {
                characterAnimator.SetBool("Jumo",true);
                this.Invoke("JumpEnd", 0.5f);
                previousAnimState = currentAnimState;
                return;
            }
            float number = (float)currentAnimState / 10;
            characterAnimator.SetFloat("Blend", number);
            Debug.Log("currentAnimState=" + number);

            // 强制立即切换动画
            //characterAnimator.Update(0);
            previousAnimState = currentAnimState;
        }
    }
    public void IsAttack()
    {
        isAttack = false;
        moveSpeed = oldMoveSpeed;
    }
    public void JumpEnd()
    {
        characterAnimator.SetBool("Jumo", false);
    }

    #endregion

    #region 输入与移动

    /// <summary>
    /// 获取玩家输入
    /// 将输入转换为三维移动方向
    /// </summary>
    void GetInput()
    {
       
        // 获取原始输入值（-1到1）
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // 将输入转换为相对于角色朝向的方向向量
        // forward对应垂直输入（W/S），right对应水平输入（A/D）
        moveDirection = (transform.forward * vertical +
                        transform.right * horizontal).normalized;
    }

    /// <summary>
    /// 执行玩家移动
    /// 根据地面状态应用不同速度
    /// </summary>
    void MovePlayer()
    {
        if (isAttack)
        {
            return;
        }
        if (isDodging) return; // 闪避时禁用常规移动

        // 计算当前有效移动速度
        float currentSpeed = isGrounded ? moveSpeed : moveSpeed * airMultiplier;

        // 保持Y轴速度不变（重力作用）
        Vector3 targetVelocity = moveDirection * currentSpeed;
        rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);
    }

    #endregion

    #region 跳跃系统

    /// <summary>
    /// 处理跳跃输入
    /// 仅在接地时允许跳跃
    /// </summary>
    void HandleJump()
    {
        //if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        //{
        //    // 使用冲击力模式施加瞬时力
        //    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        //}
    }

    #endregion

    #region 闪避系统

    /// <summary>
    /// 处理闪避输入
    /// 检查冷却时间是否结束
    /// </summary>
    void HandleDodge()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) &&
            Time.time - lastDodgeTime >= dodgeCooldown)
        {
            // 优先使用输入方向，否则使用面朝方向
            Vector3 dodgeDir = (moveDirection != Vector3.zero) ?
                moveDirection.normalized : transform.forward;

            StartCoroutine(PerformDodge(dodgeDir));
        }
    }

    /// <summary>
    /// 执行闪避协程
    /// </summary>
    /// <param name="direction">闪避方向</param>
    IEnumerator PerformDodge(Vector3 direction)
    {
        isDodging = true;

        // 施加冲刺力
        rb.AddForce(direction * dodgeForce, ForceMode.Impulse);
        lastDodgeTime = Time.time; // 记录闪避时间

        // 保持闪避状态0.3秒后恢复
        yield return new WaitForSeconds(0.3f);
        isDodging = false;
    }

    #endregion

    #region 视角控制
    public CinemachineVirtualCamera virtualCamera;
    private CinemachineComposer composer;
    /// <summary>
    /// 处理鼠标视角旋转
    /// 仅在按住右键时生效
    /// </summary>
    void HandleCameraRotation()
    {
        if (!Input.GetMouseButton(1)) return;

        // 获取鼠标增量输入
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * 1 * Time.deltaTime;

        // 水平旋转（绕Y轴旋转角色）
        transform.Rotate(Vector3.up * mouseX);

        float newScreenY = composer.m_ScreenY + mouseY;

        // 限制在0.4到0.7之间
        newScreenY = Mathf.Clamp(newScreenY, 0.4f, 0.7f);

        // 应用限制后的值
        composer.m_ScreenY = newScreenY;
        //// 垂直旋转（绕X轴旋转摄像机）
        //verticalRotation -= mouseY; // 使用减号使鼠标移动方向符合直觉
        //verticalRotation = Mathf.Clamp(verticalRotation, minLookAngle, maxLookAngle);
        //cameraPivot.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    #endregion

    #region 地面检测

    /// <summary>
    /// 执行地面检测
    /// 使用球形检测判断是否接地
    /// </summary>
    void GroundCheck()
    {
        // 检测点下移0.1米避免与地面穿插
        Vector3 checkPosition = transform.position + Vector3.down * 0.1f;
        isGrounded = Physics.CheckSphere(checkPosition, groundCheckRadius, groundMask);
    }

    /// <summary>
    /// 在编辑器绘制检测范围
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 checkPosition = transform.position + Vector3.down * 0.1f;
        Gizmos.DrawWireSphere(checkPosition, groundCheckRadius);
    }

    #endregion
}