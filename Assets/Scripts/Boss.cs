using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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
    public float attackRadius = 0.5f; // 攻击范围半径
    public int LevelNumber = 1;

    [Header("References")]
    public Transform target;           // 目标对象
    public Animator animator;

    private NavMeshAgent agent;
    private BossState currentState;
    private Vector3 homePosition;      // 初始位置
    private float lastAttackTime;      // 上次攻击时间
    private bool isDead;               // 是否死亡

    [SerializeField]
    public GameData BossGameData = new GameData();

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        homePosition = transform.position;
        currentState = BossState.Idle;
        agent.speed = patrolSpeed;
        InitGameData();
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
                    StartCoroutine(PerformAttack());
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

    IEnumerator PerformAttack()
    {
        // 这里触发攻击动画和实际攻击逻辑
        animator.SetInteger("Attack",1);
        yield return new WaitForSeconds(0.5f);

        Collider[] hitColliders = Physics.OverlapSphere(transform.Find("attackTag").position, attackRadius);
        bool hitPlayer = false;

        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                AdvancedPlayerController playerHealth = collider.GetComponent<AdvancedPlayerController>();
                if (playerHealth != null)
                {
                    playerHealth.heroGameData.AddHp(-BossGameData.Attack);
                    GameManager.instance.UpdateHeroHp(playerHealth.heroGameData);
                    playerHealth.HitAni();
                    if (playerHealth.heroGameData.Live == false)
                    {
                        playerHealth.Dead();
                    }
                    Debug.Log("击中玩家！"+ BossGameData.Attack);
                }
            }
        }
        yield return new WaitForSeconds(0.3f);
        animator.SetInteger("Attack", 0);
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
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.Find("attackTag").position, attackRadius);
    }

    // 死亡处理
    public void Die()
    {
        isDead = true;
        agent.isStopped = true;
        animator.SetTrigger("Die");
        // 其他死亡逻辑...
    }
    //初始化数据
    public void InitGameData()
    {
        BossGameData.Hp = 1000;
        BossGameData.MaxHp = 1000;
        BossGameData.Attack = 100;
        BossGameData.Type = 2;
    }
    public void HitAni()
    {
        animator.Play("hit_back");
        BossDead();
    }
    public void UpdateHp()
    {
        this.transform.Find("CanvasHp/Image (1)").GetComponent<Image>().fillAmount = (float)BossGameData.Hp / BossGameData.MaxHp;
    }
    public void BossDead()
    {
        if (BossGameData.Live == false)
        {
            //Destroy(this.gameObject);
            this.gameObject.SetActive(false);
            bool isBool = GameManager.instance.CheckBossDeath();
            if (isBool)
            {
                Debug.Log("通关");
                GameManager.instance.nextPanel.SetActive(true);
            }
            else
            {
                Debug.Log("为通关");
            }
        }
    }

}