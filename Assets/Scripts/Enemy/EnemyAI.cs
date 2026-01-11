using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    private enum State { Idle, Follow, Attack, Dead }

    [Header("Stats")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float timeBetweenAttacks = 1f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float rotationOffset = 0f;

    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Effects")]
    [SerializeField] private GameObject deathEffectPrefab; 

    private State currentState;
    private Transform player;
    private Rigidbody2D rb;
    private Collider2D col; 
    private Health health;
    private float attackTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        health = GetComponent<Health>();

        health.OnDeath += HandleDeath;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        currentState = State.Idle;

        attackTimer = timeBetweenAttacks;
    }

    private void FixedUpdate()
    {
        if (player == null || currentState == State.Dead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Idle:
                rb.linearVelocity = Vector2.zero;
                if (distance < 10f) currentState = State.Follow;
                break;

            case State.Follow:
                MoveTowardsPlayer();
                if (distance <= attackRange) currentState = State.Attack;
                break;

            case State.Attack:
                rb.linearVelocity = Vector2.zero; 
                AttackLogic();
                if (distance > attackRange) currentState = State.Follow;
                break;
        }

        UpdateVisuals();
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
    }

    private void UpdateVisuals()
    {
        if (player != null)
        {
            Vector2 direction = player.position - transform.position;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            rb.rotation = angle + rotationOffset;
        }

        bool isMoving = currentState == State.Follow;
        animator.SetBool("IsMoving", isMoving);
    }

    private void AttackLogic()
    {
        attackTimer += Time.fixedDeltaTime;
        if (attackTimer >= timeBetweenAttacks)
        {
            animator.SetTrigger("Attack");
            if (player.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damage);
            }
            attackTimer = 0;
        }
    }

    private void HandleDeath()
    {
        if (currentState == State.Dead) return;

        currentState = State.Dead;

        rb.linearVelocity = Vector2.zero;
        if (col != null) col.enabled = false;

        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(10);
        }
        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        float duration = 0.5f;
        float currentTime = 0f;
        Color startColor = spriteRenderer.color;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, currentTime / duration);
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }

    public void Initialize(float newSpeed, float newDamage, float newHealth)
    {
        speed = newSpeed;
        damage = newDamage;
    }
}