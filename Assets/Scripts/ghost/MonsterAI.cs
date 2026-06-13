using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    public Transform player;
    public Transform[] patrolPoints;

    public float chaseDistance = 8f;
    public float attackDistance = 2.2f;

    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    public int damage = 40;
    public float attackCooldown = 1.2f;

    public bool isStunned = false;
    public float stunTimer = 0f;

    private NavMeshAgent agent;
    private Animation anim;
    private PlayerHealth playerHealth;

    private int currentPoint = 0;
    private float lastAttackTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animation>();
        playerHealth = FindObjectOfType<PlayerHealth>();

        agent.speed = patrolSpeed;

        if (patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[0].position);

        PlayAnim("Walk");
    }

    void Update()
    {
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            agent.isStopped = true;
            PlayAnim("Idle");

            if (stunTimer <= 0)
            {
                isStunned = false;
                agent.isStopped = false;
                PlayAnim("Walk");
            }

            return;
        }

        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackDistance)
            Attack();
        else if (distance <= chaseDistance)
            Chase();
        else
            Patrol();
    }

    void Patrol()
    {
        agent.isStopped = false;
        agent.speed = patrolSpeed;
        PlayAnim("Walk");

        if (patrolPoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance <= 0.6f)
        {
            currentPoint++;

            if (currentPoint >= patrolPoints.Length)
                currentPoint = 0;

            agent.SetDestination(patrolPoints[currentPoint].position);
        }
    }

    void Chase()
    {
        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
        PlayAnim("Run");
    }

    void Attack()
    {
        agent.isStopped = true;
        PlayAnim("Attack2");

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            if (playerHealth == null)
                playerHealth = FindObjectOfType<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Ghoul damaged player: " + damage);
            }

            lastAttackTime = Time.time;
        }
    }

    public void StunMonster(float duration)
    {
        isStunned = true;
        stunTimer = duration;
        agent.isStopped = true;
        PlayAnim("Idle");

        Debug.Log("GHOUL STUNNED");
    }

    void PlayAnim(string animName)
    {
        if (anim != null && !anim.IsPlaying(animName))
            anim.Play(animName);
    }
}