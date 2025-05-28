using UnityEngine;
using System.Collections; // ���� ������ ������������ �������� ��� ����-����

public class UrkAI : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 2f;
    public int health = 50;
    public int attackDamage = 10;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    private float lastAttackTime;

    [Header("Detection")]
    public float detectionRadius = 5f;
    public LayerMask playerLayer;

    [Header("References")]
    private Transform playerTransform;
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Death Settings")]
    public float deathAnimationDuration = 1.5f;

    private enum UrkState { Idle, Chase, Attack, Hurt, Die }
    private UrkState currentState;

    private Vector2 lastMovementDirection = Vector2.down; // ��� Idle � ����������� ����������� Hurt/Attack
    private Vector2 currentMovementInput = Vector2.zero; // ��� Walk Blend Tree

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("UrkAI (" + gameObject.name + "): Player not found! Ensure player has 'Player' tag.");
        }
    }

    void Start()
    {
        currentState = UrkState.Idle;
        lastAttackTime = -attackCooldown;
        UpdateAnimatorParameters(false); // ���������� �� ����
    }

    void Update()
    {
        if (currentState == UrkState.Die || currentState == UrkState.Hurt || playerTransform == null)
        {
            // ���� ��� �������, �������� ���� ��� ����� �� ������, ���������� �������� ������ ��������/�����.
            // �������� Hurt/Die ���� ������ ��� � Idle ��� ���������.
            // ���� ����� ������, � ��� �� ������� � �� �����, �� ������ ��������� � Idle.
            if (playerTransform == null && currentState != UrkState.Die && currentState != UrkState.Hurt)
            {
                currentState = UrkState.Idle;
                rb.velocity = Vector2.zero;
                UpdateAnimatorParameters(false);
            }
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        bool isCurrentlyWalking = false;

        switch (currentState)
        {
            case UrkState.Idle:
                rb.velocity = Vector2.zero;
                if (CanSeePlayer(distanceToPlayer))
                {
                    currentState = UrkState.Chase;
                }
                break;

            case UrkState.Chase:
                isCurrentlyWalking = true;
                if (distanceToPlayer <= attackRange)
                {
                    currentState = UrkState.Attack;
                    isCurrentlyWalking = false;
                    rb.velocity = Vector2.zero;
                }
                else if (!CanSeePlayer(distanceToPlayer))
                {
                    currentState = UrkState.Idle;
                    isCurrentlyWalking = false;
                }
                else
                {
                    Vector2 direction = (playerTransform.position - transform.position).normalized;
                    rb.velocity = direction * moveSpeed;
                    currentMovementInput = direction;
                    if (direction.sqrMagnitude > 0.01f)
                    {
                        lastMovementDirection = direction;
                    }
                }
                break;

            case UrkState.Attack:
                rb.velocity = Vector2.zero; // ��������, ��� �����
                isCurrentlyWalking = false;

                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    PerformAttack(); // � PerformAttack ����� ���������� AttackTrigger
                    lastAttackTime = Time.time;
                    // ����� ������ PerformAttack, ��� �������� � ���� �� Attack_Direction ���������,
                    // � ����� Animator ������ ��� � Idle.
                    // �� ����� ����� ���������� currentState � Idle, ����� ������ Update �� �������� ��������� ����� �����.
                    // ��� �����, ����� ��������� Attack �������, ���� �������� �� ����������,
                    // �� ��� ��� � ��� ����� �� Exit Time, �� ����� ����� �� �������� � Idle � ��� �����.
                    // ���� ������� ���, ��� �� ����� ������ ��������� �����, ���� ����� ��� ��� � ����.
                }

                // ��������, �� ������ �� ����� ����� ������� ����� ��� �� ����� ��������
                if (distanceToPlayer > attackRange && Time.time < lastAttackTime + attackCooldown) // ���� ����� ������ �� ����� ��������
                {
                    currentState = UrkState.Chase; // �������� ������������
                }
                else if (!CanSeePlayer(distanceToPlayer)) // ����� ������ ������
                {
                    currentState = UrkState.Idle;
                }
                break;
        }
        UpdateAnimatorParameters(isCurrentlyWalking);
    }

    bool CanSeePlayer(float distance)
    {
        return distance <= detectionRadius;
    }

    void UpdateAnimatorParameters(bool isWalking)
    {
        if (animator == null) return;

        animator.SetBool("isWalking", isWalking);

        if (isWalking)
        {
            animator.SetFloat("InputX", currentMovementInput.x);
            animator.SetFloat("InputY", currentMovementInput.y);
            animator.SetFloat("lastInputX", currentMovementInput.x); // ��������� � lastInput ��� ������� ���������
            animator.SetFloat("lastInputY", currentMovementInput.y);
        }
        else
        {
            animator.SetFloat("InputX", 0);
            animator.SetFloat("InputY", 0);
            animator.SetFloat("lastInputX", lastMovementDirection.x);
            animator.SetFloat("lastInputY", lastMovementDirection.y);
        }
    }

    void PerformAttack()
    {
        if (playerTransform == null || animator == null || currentState == UrkState.Die || currentState == UrkState.Hurt) return;

        // ������������� ����������� ��� ������ ���������� �������� �����
        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
        if (directionToPlayer.sqrMagnitude > 0.01f)
        {
            animator.SetFloat("lastInputX", directionToPlayer.x);
            animator.SetFloat("lastInputY", directionToPlayer.y);
            lastMovementDirection = directionToPlayer; // ��������� ��� Idle ����� �����
        }
        animator.SetBool("isWalking", false); // ��������, ��� ����������� �� �� ����

        animator.SetTrigger("AttackTrigger");
        Debug.Log("Urk (" + gameObject.name + ") AttackTrigger set. Attacking towards (" + animator.GetFloat("lastInputX").ToString("F2") + ", " + animator.GetFloat("lastInputY").ToString("F2") + ")");


        // ��������� ����� ����� ������ ����� Animation Event � ����� ��������, ��� ���������� ����.
        // ��� �������� ���� ������� ���� �����.
        PlayerStats playerStats = playerTransform.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.TakeDamage(attackDamage);
            // Debug.Log("Urk (" + gameObject.name + ") dealt " + attackDamage + " damage.");
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentState == UrkState.Die) return; // ���� ��� �������, ������ �� ������

        health -= damage;
        Debug.Log("Urk (" + gameObject.name + ") took " + damage + " damage. Current health: " + health);

        currentState = UrkState.Hurt; // ��������� � ��������� Hurt, ����� �������� ��������

        if (animator != null)
        {
            // ������������� ��������� ��� ������������ �������� Hurt
            animator.SetFloat("lastInputX", lastMovementDirection.x);
            animator.SetFloat("lastInputY", lastMovementDirection.y);
            animator.SetBool("isWalking", false);

            animator.SetTrigger("HurtTrigger");
            // Debug.Log("Urk (" + gameObject.name + ") HurtTrigger set.");
        }

        if (health <= 0)
        {
            Die(); // ���� �������� �����������, �������� ������
        }
        else
        {
            // ���� �� ����, ��������� �������� ��� �������� �� ��������� Hurt
            StopAllCoroutines(); // ������������� ���������� �������� ResetStateAfterHurt, ���� ����
            StartCoroutine(ResetStateAfterHurt());
        }
    }

    IEnumerator ResetStateAfterHurt() // �������, ��� ��� �������� ����
    {
        yield return new WaitForSeconds(0.5f); // ������� ����� ��� �������� Hurt

        if (currentState == UrkState.Hurt) 
        {
            if (playerTransform != null && CanSeePlayer(Vector2.Distance(transform.position, playerTransform.position)))
            {
                currentState = UrkState.Chase;
            }
            else
            {
                currentState = UrkState.Idle;
            }
        }
    }



    void Die()
    {
        if (currentState == UrkState.Die) return; // ������������� ������������ �����

        TaskManager.Instance.ReportProgress("beat_orc");

        Debug.Log("Urk (" + gameObject.name + ") is dying. Health: " + health);
        currentState = UrkState.Die; // ������������� ���������� ��������� �������

        if (rb != null)
        {
            rb.velocity = Vector2.zero;   // ������������� ����� ��������
                                          // rb.isKinematic = true; // �����������: ������� ���� ��������������� � ������
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false; // ��������� ���������
        }

        if (animator != null)
        {
            animator.SetBool("isWalking", false); // ��������, ��� �� � �������� ������
            animator.ResetTrigger("AttackTrigger"); // ���������� ������� �����
            animator.ResetTrigger("HurtTrigger");  // ���������� ������� ��������� �����
            animator.SetTrigger("DieTrigger");    // ��������� ������� ������
            Debug.Log("Urk (" + gameObject.name + ") DieTrigger set. Will be destroyed in " + deathAnimationDuration + " seconds.");
        }
        else // ���� ��������� ��� (�� ������ ������)
        {
            Debug.LogWarning("Urk (" + gameObject.name + ") has no Animator. Destroying immediately or with minimal delay.");
            Destroy(gameObject, 0.1f);
            return;
        }

        // ���������� ������� ������ ����� ������������ �������� ������, ��������� ������������� ������������
        Destroy(gameObject, deathAnimationDuration);
    }

    public void InitializeStats(int dayNumber)
    {
        health = 50 + (dayNumber - 1) * 20;
        attackDamage = 10 + (dayNumber - 1) * 5;
        health = Mathf.Clamp(health, 50, 150);
        attackDamage = Mathf.Clamp(attackDamage, 10, 40);
        // Debug.Log("Urk (" + gameObject.name + ") initialized for day " + dayNumber + ". HP: " + health + ", DMG: " + attackDamage);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}