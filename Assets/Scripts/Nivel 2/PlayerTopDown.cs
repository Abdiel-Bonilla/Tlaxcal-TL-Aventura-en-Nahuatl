using UnityEngine;

public class PlayerTopDown : MonoBehaviour
{
    [Header("Velocidad de Movimiento")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Asegurarnos de apagar la gravedad para el RPG cenital
        rb.gravityScale = 0f;
    }

    void Update()
    {
        // Leer las teclas WASD o las flechas
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Comprobar si el jugador se está moviendo
        bool isWalking = movement.sqrMagnitude > 0.1f;

        // Enviar la señal al Animator
        animator.SetBool("isMoving", isWalking);

        if (isWalking)
        {
            // Actualizar coordenadas para el Blend Tree de movimiento
            animator.SetFloat("InputX", movement.x);
            animator.SetFloat("InputY", movement.y);
            animator.SetFloat("LastInputX", movement.x);
            animator.SetFloat("LastInputY", movement.y);

            // Voltear sprite si va hacia los lados
            if (movement.x > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (movement.x < 0)
            {
                spriteRenderer.flipX = true;
            }
        }
    }

    void FixedUpdate()
    {
        // Mover al personaje fluidamente en el plano 2D
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }
}