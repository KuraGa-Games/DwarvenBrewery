using UnityEngine;
using UnityEngine.InputSystem; // Убедись, что эта строка есть

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 20; // Урон от удара Илурина
    [SerializeField] private float attackRange = 1.0f; // Дистанция атаки
    [SerializeField] private float attackCooldown = 0.5f; // Задержка между атаками
    [SerializeField] private LayerMask enemyLayerMask; // Слой, на котором находятся враги
    [SerializeField] private Transform attackPoint; // Точка, откуда исходит атака (если нужна особая точка)

    private float lastAttackTime;

    [Header("Animation Settings")]
    private Animator animator;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lastMoveDirection;
    private bool isCurrentlyRunning = false;

    private PlayerInput playerInput;
    private InputAction attackAction; // Мы будем хранить ссылку на действие атаки здесь

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();

        if (animator == null)
        {
            Debug.LogError("PlayerController: Компонент Animator не найден!");
        }
        if (playerInput == null)
        {
            Debug.LogError("PlayerController: Компонент PlayerInput не найден!");
        }
        else
        {
            // Используем имя "Fire" (или то, которое у тебя в Input Actions)
            attackAction = playerInput.actions["Fire"];
            if (attackAction == null)
            {
                Debug.LogError("PlayerController: Действие 'Fire' (для атаки) не найдено в Input Actions! Проверь имя.");
            }
        }
        if (attackPoint == null)
        {
            attackPoint = transform;
        }
    }

    private void Start()
    {
        lastMoveDirection = Vector2.down;
        UpdateAnimatorParameters();
        lastAttackTime = -attackCooldown; // Позволяет атаковать сразу
    }

    private void FixedUpdate()
    {
        MoveCharacter();
    }

    private void Update()
    {
        HandleAnimation();
        HandleAttackInput(); // Обрабатываем ввод для атаки
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void MoveCharacter()
    {
        if (moveInput.sqrMagnitude > 0.01f)
        {
            isCurrentlyRunning = true;
            rb.velocity = moveInput.normalized * moveSpeed;
            lastMoveDirection = moveInput.normalized;
        }
        else
        {
            isCurrentlyRunning = false;
            rb.velocity = Vector2.zero;
        }
    }

    private void HandleAnimation()
    {
        if (animator == null) return;
        UpdateAnimatorParameters();
    }

    private void UpdateAnimatorParameters()
    {
        animator.SetBool("isRunning", isCurrentlyRunning);
        if (isCurrentlyRunning)
        {
            animator.SetFloat("InputX", moveInput.x);
            animator.SetFloat("InputY", moveInput.y);
            animator.SetFloat("lastInputX", moveInput.x);
            animator.SetFloat("lastInputY", moveInput.y);
        }
        else
        {
            animator.SetFloat("InputX", 0);
            animator.SetFloat("InputY", 0);
            animator.SetFloat("lastInputX", lastMoveDirection.x);
            animator.SetFloat("lastInputY", lastMoveDirection.y);
        }
    }

    /// <summary>
    /// Обрабатывает ввод для атаки.
    /// </summary>
    private void HandleAttackInput()
    {
        if (animator == null) return;
        if (attackAction == null) // Проверка, если действие атаки не найдено (уже есть в Awake, но для надежности)
        {
            return;
        }

        if (attackAction.WasPerformedThisFrame() && Time.time >= lastAttackTime + attackCooldown)
        {
            PerformPlayerAttack();
            lastAttackTime = Time.time;
        }
    }

    /// <summary>
    /// Выполняет логику атаки игрока.
    /// </summary>
    private void PerformPlayerAttack()
    {
        Debug.Log("Player attacking!");
        animator.SetTrigger("AttackTrigger"); // Запускаем анимацию атаки

        // Определяем направление атаки на основе lastMoveDirection (куда игрок смотрел)
        // или можно использовать текущее направление мыши, если игра это поддерживает.
        // Пока будем использовать lastMoveDirection.

        Vector2 attackDirection = lastMoveDirection;
        if (attackDirection == Vector2.zero) // Если стоит на месте и не двигался, атакует вперед (или по умолчанию вниз)
        {
            // Если спрайт игрока может поворачиваться, можно взять transform.right или transform.up
            // В 2D top-down, если lastMoveDirection (0,0), то нужно дефолтное направление.
            // Предположим, он атакует в последнем направлении, в котором смотрел.
            // Если lastMoveDirection (0,0) (в самом начале игры), атакуем вниз.
            attackDirection = animator.GetFloat("lastInputY") < -0.1f ? Vector2.down :
                              animator.GetFloat("lastInputY") > 0.1f ? Vector2.up :
                              animator.GetFloat("lastInputX") < -0.1f ? Vector2.left :
                              animator.GetFloat("lastInputX") > 0.1f ? Vector2.right :
                              Vector2.down; // крайний дефолт
        }


        // Обнаружение врагов в зоне атаки
        // Используем OverlapCircleAll, чтобы найти всех врагов в радиусе атаки
        // Позиция для OverlapCircle - это attackPoint.position + attackDirection * (attackRange / 2)
        // Это смещает центр круга немного вперед в направлении атаки.
        // Размер круга - attackRange / 2 (или просто attackRange, если attackPoint уже на краю)

        // Более простой вариант: круг с центром в attackPoint и радиусом attackRange
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayerMask);

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            UrkAI urk = enemyCollider.GetComponent<UrkAI>(); // Получаем компонент UrkAI
            if (urk != null) // Если это действительно Урк
            {
                Debug.Log("Player hit Urk: " + enemyCollider.name);
                urk.TakeDamage(attackDamage); // Наносим урон Урку
            }
        }
    }

    // Для отладки можно нарисовать зону атаки в редакторе
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}