using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Управляет движением персонажа Илурина в игре "Гном-пивовар" с использованием физики Rigidbody2D.
/// </summary>

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Скорость перемещения персонажа в единицах за секунду.
    /// </summary>
    [SerializeField] private float moveSpeed = 5f;

    /// <summary>
    /// Компонент Rigidbody2D для управления физикой движения.
    /// </summary>
    private Rigidbody2D rb;

    /// <summary>
    /// Вектор ввода от игрока, определяющий направление движения.
    /// </summary>
    private Vector2 moveInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() 
    {
        MoveCharacter();
    }

    /// <summary>
    /// Применяет скорость к Rigidbody2D на основе вектора ввода.
    /// </summary>
    /// <remarks>
    /// Вектор ввода нормализуется, чтобы обеспечить равномерную скорость в любом направлении.
    /// </remarks>
    public void MoveCharacter()
    {
        rb.velocity = moveInput.normalized * moveSpeed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
}
