using UnityEngine;

public class PlayerTopDown : MonoBehaviour
{
    [Header("Velocidad de Movimiento")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 movement;

    [Header("Efectos de Sonido (Audio)")]
    public AudioSource audioSourceTlali;          // Para los pasos
    public AudioSource audioSourceRespiracion;    // Para la respiración en bucle
    public AudioClip sonidoCorrer;                // Sonido de pasos caminando
    public AudioClip sonidoRespirar;              // Sonido de respiración en pausa

    [Header("Configuración de Pasos")]
    private float contadorPasos = 0f;
    public float intervaloPasos = 0.35f;          // Tiempo entre cada paso

    [Header("Configuración de Respiración")]
    private float tiempoQuieto = 0f;
    public float tiempoParaComenzarRespirar = 1.5f; // Segundos quieto antes de respirar

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Asegurarnos de apagar la gravedad para el RPG cenital
        rb.gravityScale = 0f;

        if (audioSourceTlali == null)
        {
            audioSourceTlali = GetComponent<AudioSource>();
        }
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

            // Lógica de Sonido de Pasos al caminar
            contadorPasos += Time.deltaTime;
            if (contadorPasos >= intervaloPasos)
            {
                if (audioSourceTlali != null && sonidoCorrer != null)
                {
                    audioSourceTlali.PlayOneShot(sonidoCorrer);
                }
                contadorPasos = 0f;
            }

            // Si se mueve, detenemos la respiración y reiniciamos el cronómetro
            DetenerRespiracion();
            tiempoQuieto = 0f;
        }
        else
        {
            // Si está completamente quieto en el Nivel 2
            contadorPasos = intervaloPasos;
            tiempoQuieto += Time.deltaTime;

            if (tiempoQuieto >= tiempoParaComenzarRespirar)
            {
                if (audioSourceRespiracion != null && sonidoRespirar != null)
                {
                    if (!audioSourceRespiracion.isPlaying)
                    {
                        audioSourceRespiracion.clip = sonidoRespirar;
                        audioSourceRespiracion.loop = true;
                        audioSourceRespiracion.Play();
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        // Mover al personaje fluidamente en el plano 2D
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    void DetenerRespiracion()
    {
        if (audioSourceRespiracion != null && audioSourceRespiracion.isPlaying)
        {
            audioSourceRespiracion.Stop();
            audioSourceRespiracion.loop = false;
        }
    }
}