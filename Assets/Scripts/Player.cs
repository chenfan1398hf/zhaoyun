using System.Collections;
using UnityEngine;

/// <summary>
/// �߼���ҽ�ɫ������
/// ���ܰ����������ƶ�����Ծ�����ܡ��ӽǿ���
/// </summary>
[RequireComponent(typeof(Rigidbody))] // ǿ��Ҫ��Rigidbody���
public class AdvancedPlayerController : MonoBehaviour
{
    // region�������۵����飨��Ҫ֧��C# 10.0+��
    #region �����ò���

    [Header("�ƶ�����")]
    [Tooltip("�����ƶ��ٶȣ���/�룩")]
    [SerializeField] private float moveSpeed = 6f;

    [Tooltip("�����ƶ��ٶȳ�����0-1��")]
    [SerializeField] private float airMultiplier = 0.4f;

    [Tooltip("��Ծ����")]
    [SerializeField] private float jumpForce = 12f;

    [Tooltip("���ܳ������")]
    [SerializeField] private float dodgeForce = 20f;

    [Tooltip("������ȴʱ�䣨�룩")]
    [SerializeField] private float dodgeCooldown = 1.5f;

    [Tooltip("����������뾶")]
    [SerializeField] private float groundCheckRadius = 0.3f;

    [Tooltip("����㼶����")]
    [SerializeField] private LayerMask groundMask;

    [Header("�ӽǿ���")]
    [Tooltip("���������")]
    [SerializeField] private float mouseSensitivity = 100f;

    [Tooltip("�������ת֧��")]
    [SerializeField] private Transform cameraPivot;

    [Tooltip("�������")]
    [SerializeField] private float maxLookAngle = 80f;

    [Tooltip("��󸩽�")]
    [SerializeField] private float minLookAngle = -80f;
    private enum MoveAnimationState
    {
        Idle,
        Forward,
        ForwardLeft,
        ForwardRight,
        Backward,
        Left,
        Right
    }
    [SerializeField] 
    private Animator characterAnimator;
    private MoveAnimationState currentAnimState;
    private MoveAnimationState previousAnimState;
    [SerializeField] 
    private float animationTransitionSpeed = 0.1f;
    #endregion

    #region ˽�б���

    private Rigidbody rb;            // �����������s
    private Vector3 moveDirection;   // �ƶ���������
    private float verticalRotation;  // �������ֱ��ת�Ƕ�
    private bool isGrounded;         // �Ƿ��ڵ���
    private float lastDodgeTime;     // �ϴ�����ʱ��
    private bool isDodging;          // �Ƿ�������״̬

    #endregion

    #region �������ڷ���

    /// <summary>
    /// ��ʼ�����
    /// </summary>
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // ����������ת

        // ������굽��Ļ����
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        characterAnimator = this.transform.Find("Player").GetComponent<Animator>();
    }

    /// <summary>
    /// ÿ֡�����߼��������⣩
    /// </summary>
    void Update()
    {
        GroundCheck();       // ������
        GetInput();          // ��ȡ����
        HandleJump();       // ������Ծ
        HandleDodge();       // ��������
        HandleCameraRotation(); // �����ӽ�
        HandleAnimation(); // ������������
    }

    /// <summary>
    /// �������ѭ���������ƶ���
    /// </summary>
    void FixedUpdate()
    {
        MovePlayer();
    }

    private void HandleAnimation()
    {
        // ���ȼ����ϰ���
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
        // ����״̬�仯ʱ����
        if (currentAnimState != previousAnimState)
        {
            float number = (float)currentAnimState / 10;
            characterAnimator.SetFloat("Blend", number);
            Debug.Log("currentAnimState=" + number);

            // ǿ�������л�����
            //characterAnimator.Update(0);
            previousAnimState = currentAnimState;
        }
      

    }

    #endregion

    #region �������ƶ�

    /// <summary>
    /// ��ȡ�������
    /// ������ת��Ϊ��ά�ƶ�����
    /// </summary>
    void GetInput()
    {
        // ��ȡԭʼ����ֵ��-1��1��
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // ������ת��Ϊ����ڽ�ɫ����ķ�������
        // forward��Ӧ��ֱ���루W/S����right��Ӧˮƽ���루A/D��
        moveDirection = (transform.forward * vertical +
                        transform.right * horizontal).normalized;
    }

    /// <summary>
    /// ִ������ƶ�
    /// ���ݵ���״̬Ӧ�ò�ͬ�ٶ�
    /// </summary>
    void MovePlayer()
    {
        if (isDodging) return; // ����ʱ���ó����ƶ�

        // ���㵱ǰ��Ч�ƶ��ٶ�
        float currentSpeed = isGrounded ? moveSpeed : moveSpeed * airMultiplier;

        // ����Y���ٶȲ��䣨�������ã�
        Vector3 targetVelocity = moveDirection * currentSpeed;
        rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);
    }

    #endregion

    #region ��Ծϵͳ

    /// <summary>
    /// ������Ծ����
    /// ���ڽӵ�ʱ������Ծ
    /// </summary>
    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // ʹ�ó����ģʽʩ��˲ʱ��
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    #endregion

    #region ����ϵͳ

    /// <summary>
    /// ������������
    /// �����ȴʱ���Ƿ����
    /// </summary>
    void HandleDodge()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) &&
            Time.time - lastDodgeTime >= dodgeCooldown)
        {
            // ����ʹ�����뷽�򣬷���ʹ���泯����
            Vector3 dodgeDir = (moveDirection != Vector3.zero) ?
                moveDirection.normalized : transform.forward;

            StartCoroutine(PerformDodge(dodgeDir));
        }
    }

    /// <summary>
    /// ִ������Э��
    /// </summary>
    /// <param name="direction">���ܷ���</param>
    IEnumerator PerformDodge(Vector3 direction)
    {
        isDodging = true;

        // ʩ�ӳ����
        rb.AddForce(direction * dodgeForce, ForceMode.Impulse);
        lastDodgeTime = Time.time; // ��¼����ʱ��

        // ��������״̬0.3���ָ�
        yield return new WaitForSeconds(0.3f);
        isDodging = false;
    }

    #endregion

    #region �ӽǿ���

    /// <summary>
    /// ��������ӽ���ת
    /// ���ڰ�ס�Ҽ�ʱ��Ч
    /// </summary>
    void HandleCameraRotation()
    {
        if (!Input.GetMouseButton(1)) return;

        // ��ȡ�����������
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // ˮƽ��ת����Y����ת��ɫ��
        transform.Rotate(Vector3.up * mouseX);

        //// ��ֱ��ת����X����ת�������
        //verticalRotation -= mouseY; // ʹ�ü���ʹ����ƶ��������ֱ��
        //verticalRotation = Mathf.Clamp(verticalRotation, minLookAngle, maxLookAngle);
        //cameraPivot.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    #endregion

    #region ������

    /// <summary>
    /// ִ�е�����
    /// ʹ�����μ���ж��Ƿ�ӵ�
    /// </summary>
    void GroundCheck()
    {
        // ��������0.1�ױ�������洩��
        Vector3 checkPosition = transform.position + Vector3.down * 0.1f;
        isGrounded = Physics.CheckSphere(checkPosition, groundCheckRadius, groundMask);
    }

    /// <summary>
    /// �ڱ༭�����Ƽ�ⷶΧ
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 checkPosition = transform.position + Vector3.down * 0.1f;
        Gizmos.DrawWireSphere(checkPosition, groundCheckRadius);
    }

    #endregion
}