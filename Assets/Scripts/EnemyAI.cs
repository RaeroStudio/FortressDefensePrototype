using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour 
{
    [Header("Настройки")]
    public float detectionRadius = 10f;
    public float attackRadius = 2f;
    public float settlerDamage = 15f;
    public float citadelDamage = 30f;
    public float attackCooldown = 1f;
    public float maxHealth = 100f;
    public float MoveSpeed = 5f;

    [Header("Ссылки")]
    public Citadel citadel;
    public GoldManager GoldManager;

    private NavMeshAgent agent;
    private float currentHealth;
    private float lastAttackTime;
    private Transform currentTarget;
    private float targetUpdateInterval = 0.5f;
    private float lastTargetUpdateTime;
    private void SetDestinationToCitadel()
    {
        if (agent.isOnNavMesh)
            agent.SetDestination(citadel.transform.position); // Добавляем .transform
    }
    void Start()
{
    agent = GetComponent<NavMeshAgent>();
    currentHealth = maxHealth;

    // Автопоиск цитадели с использованием нового API
    if(citadel == null)
    {
        citadel = FindAnyObjectByType<Citadel>(FindObjectsInactive.Include);
        if(citadel == null) 
            Debug.LogError("Citadel not found on scene!");
    }

    // Автопоиск GoldManager с новым методом
    if(GoldManager == null)
    {
        GoldManager = FindAnyObjectByType<GoldManager>(FindObjectsInactive.Include);
        if(GoldManager == null) 
            Debug.LogError("GoldManager not found!");
    }

    SetDestinationToCitadel();
}

    void Update()
    {
        if (Time.time - lastTargetUpdateTime >= targetUpdateInterval)
        {
            FindTarget();
            lastTargetUpdateTime = Time.time;
        }

        if (currentTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

            if (distanceToTarget <= attackRadius)
            {
                AttackTarget();
            }
            else
            {
                UpdateMovement();
            }

            RotateTowardsTarget();
        }
        else
        {
            MoveToCitadel();
        }
    }

   private void FindTarget()
{
    Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
    Transform closestSettler = null;
    float closestDistance = Mathf.Infinity;

    foreach (var hitCollider in hitColliders)
    {
        if (hitCollider.CompareTag("Settler"))
        {
            float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestSettler = hitCollider.transform;
            }
        }
    }

    // Исправленная строка:
    currentTarget = closestSettler != null ? closestSettler : citadel.transform; // Явное обращение к transform
}

    private void AttackTarget()
    {
        if (agent.isOnNavMesh) agent.isStopped = true;

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            if (currentTarget.CompareTag("Settler"))
            {
                SettlerAI settler = currentTarget.GetComponent<SettlerAI>();
                if (settler != null) settler.TakeDamage(settlerDamage);
            }
            else if (currentTarget.CompareTag("Citadel"))
            {
                Citadel citadel = currentTarget.GetComponent<Citadel>();
                if (citadel != null) citadel.TakeDamage(citadelDamage);
            }
            lastAttackTime = Time.time;
        }
    }

    private void UpdateMovement()
    {
        if (agent.isOnNavMesh && !agent.pathPending)
        {
            agent.isStopped = false;
            agent.SetDestination(currentTarget.position);
        }
    }

    private void RotateTowardsTarget()
    {
         if(currentTarget == null) return;

    Vector3 direction = currentTarget.position - transform.position;
    
    // Проверка на нулевой вектор и минимальную дистанцию
    if(direction.sqrMagnitude < 0.001f) return;

    direction.y = 0; // Игнорируем вертикальную составляющую
    Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);
    transform.rotation = Quaternion.Slerp(
        transform.rotation, 
        lookRotation, 
        Time.deltaTime * agent.angularSpeed
    ); }

    private void MoveToCitadel()
    {
        if (agent.isOnNavMesh && !agent.pathPending)
        {
            agent.SetDestination(citadel.transform.position);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0) Die();
    }

    public event System.Action OnEnemyDeath;

    private void Die()
    {
    OnEnemyDeath?.Invoke();
    
    // Добавляем проверку существования объекта
    if(this == null || gameObject == null) return;

    if(GoldManager != null)
    {
        GoldManager.AddGold(10);
    }
    else
    {
        Debug.LogWarning("GoldManager not found!");
    }

    Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
    void OnDestroy()
{
    // Гарантированно отписываемся от событий
    OnEnemyDeath = null;
}
}