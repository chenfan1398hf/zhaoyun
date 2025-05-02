using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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
    public float attackRadius = 0.5f; // ������Χ�뾶
    public int LevelNumber = 1;

    [Header("References")]
    public Transform target;           // Ŀ�����
    public Animator animator;

    private NavMeshAgent agent;
    private BossState currentState;
    private Vector3 homePosition;      // ��ʼλ��
    private float lastAttackTime;      // �ϴι���ʱ��
    private bool isDead;               // �Ƿ�����

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
                    StartCoroutine(PerformAttack());
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

    IEnumerator PerformAttack()
    {
        // ���ﴥ������������ʵ�ʹ����߼�
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
                    Debug.Log("������ң�"+ BossGameData.Attack);
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

    // �ڱ༭������ʾ��ⷶΧ
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.Find("attackTag").position, attackRadius);
    }

    // ��������
    public void Die()
    {
        isDead = true;
        agent.isStopped = true;
        animator.SetTrigger("Die");
        // ���������߼�...
    }
    //��ʼ������
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
                Debug.Log("ͨ��");
                GameManager.instance.nextPanel.SetActive(true);
            }
            else
            {
                Debug.Log("Ϊͨ��");
            }
        }
    }

}