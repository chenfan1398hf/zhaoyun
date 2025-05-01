using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour
{
    public enum BossState { Idle, Chase, Attack, Return }

    [Header("Settings")]
    public float attackRange = 3f;      // 攻击范围
    public float chaseRange = 10f;     // 追击范围
    public float chaseSpeed = 4f;      // 追击速度
    public float patrolSpeed = 2f;     // 返回速度
    public float attackCooldown = 2f;  // 攻击间隔
    public float giveUpDistance = 15f; // 放弃追击距离

    [Header("References")]
    public Transform target;           // 目标对象
    public Animator animator;

    private NavMeshAgent agent;
    private BossState currentState;
    private Vector3 homePosition;      // 初始位置
    private float lastAttackTime;      // 上次攻击时间
    private bool isDead;               // 是否死亡

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
                // 检测玩家进入追击范围
                if (distanceToTarget <= chaseRange)
                {
                    ChangeState(BossState.Chase);
                }
                break;

            case BossState.Chase:
                agent.SetDestination(target.position);

                // 进入攻击范围
                if (distanceToTarget <= attackRange)
                {
                    ChangeState(BossState.Attack);
                }
                // 目标超出放弃距离
                else if (distanceToTarget > giveUpDistance)
                {
                    ChangeState(BossState.Return);
                }
                break;

            case BossState.Attack:
                // 始终面向目标
                FaceTarget();

                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    // 执行攻击
                    PerformAttack();
                    lastAttackTime = Time.time;
                }

                // 目标离开攻击范围
                if (distanceToTarget > attackRange)
                {
                    ChangeState(BossState.Chase);
                }
                break;

            case BossState.Return:
                // 返回起始位置
                agent.SetDestination(homePosition);

                // 回到原位后进入待机状态
                if (distanceToHome < 1f)
                {
                    ChangeState(BossState.Idle);
                }
                // 返回途中发现目标
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
        // 这里触发攻击动画和实际攻击逻辑
        animator.SetTrigger("Attack");
        // 实际伤害逻辑需要根据你的游戏实现
        Debug.Log("BOSS发动攻击！");
    }

    void UpdateAnimations()
    {
        //animator.SetBool("IsMoving", agent.velocity.magnitude > 0.1f);
        animator.SetFloat("Blend", agent.velocity.magnitude / chaseSpeed);
    }

    // 在编辑器中显示检测范围
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }

    // 死亡处理
    public void Die()
    {
        isDead = true;
        agent.isStopped = true;
        animator.SetTrigger("Die");
        // 其他死亡逻辑...
    }
}