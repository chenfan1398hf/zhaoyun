using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour
{
    public enum BossState { Idle, Chase, Attack, Return }

    [Header("Settings")]
    public float attackRange = 3f;      // ������Χ
    public float chaseRange = 10f;     // ׷����Χ
    public float chaseSpeed = 4f;      // ׷���ٶ�
    public float patrolSpeed = 2f;     // �����ٶ�
    public float attackCooldown = 2f;  // �������
    public float giveUpDistance = 15f; // ����׷������

    [Header("References")]
    public Transform target;           // Ŀ�����
    public Animator animator;

    private NavMeshAgent agent;
    private BossState currentState;
    private Vector3 homePosition;      // ��ʼλ��
    private float lastAttackTime;      // �ϴι���ʱ��
    private bool isDead;               // �Ƿ�����

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        homePosition = transform.position;
        currentState = BossState.Idle;
        agent.speed = patrolSpeed;
    }

    void Update()
    {
        if (isDead) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        float distanceToHome = Vector3.Distance(transform.position, homePosition);

        switch (currentState)
        {
            case BossState.Idle:
                // �����ҽ���׷����Χ
                if (distanceToTarget <= chaseRange)
                {
                    ChangeState(BossState.Chase);
                }
                break;

            case BossState.Chase:
                agent.SetDestination(target.position);

                // ���빥����Χ
                if (distanceToTarget <= attackRange)
                {
                    ChangeState(BossState.Attack);
                }
                // Ŀ�곬����������
                else if (distanceToTarget > giveUpDistance)
                {
                    ChangeState(BossState.Return);
                }
                break;

            case BossState.Attack:
                // ʼ������Ŀ��
                FaceTarget();

                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    // ִ�й���
                    PerformAttack();
                    lastAttackTime = Time.time;
                }

                // Ŀ���뿪������Χ
                if (distanceToTarget > attackRange)
                {
                    ChangeState(BossState.Chase);
                }
                break;

            case BossState.Return:
                // ������ʼλ��
                agent.SetDestination(homePosition);

                // �ص�ԭλ��������״̬
                if (distanceToHome < 1f)
                {
                    ChangeState(BossState.Idle);
                }
                // ����;�з���Ŀ��
                else if (distanceToTarget <= chaseRange)
                {
                    ChangeState(BossState.Chase);
                }
                break;
        }

        UpdateAnimations();
    }

    void ChangeState(BossState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case BossState.Chase:
                agent.speed = chaseSpeed;
                agent.isStopped = false;
                break;

            case BossState.Attack:
                agent.isStopped = true;
                break;

            case BossState.Return:
                agent.speed = patrolSpeed;
                agent.isStopped = false;
                break;

            case BossState.Idle:
                agent.isStopped = true;
                break;
        }
    }

    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void PerformAttack()
    {
        // ���ﴥ������������ʵ�ʹ����߼�
        animator.SetTrigger("Attack");
        // ʵ���˺��߼���Ҫ���������Ϸʵ��
        Debug.Log("BOSS����������");
    }

    void UpdateAnimations()
    {
        //animator.SetBool("IsMoving", agent.velocity.magnitude > 0.1f);
        animator.SetFloat("Blend", agent.velocity.magnitude / chaseSpeed);
    }

    // �ڱ༭������ʾ��ⷶΧ
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }

    // ��������
    public void Die()
    {
        isDead = true;
        agent.isStopped = true;
        animator.SetTrigger("Die");
        // ���������߼�...
    }
}