using UnityEngine;
using TMPro;
using System.Collections;

public class MovimientoPersonaje : MonoBehaviour
{
    public float Speed;
    public float JumpForce;

    private Rigidbody2D Rigidbody2D;
    private Animator Animator;
    private float Horizontal;
    private bool Grounded;

    [SerializeField] private float alturaPies = 0.1258188f;

    [Header("Efectos de Sonido (Audio)")]
    public AudioSource audioSourceTlali; // AudioSource principal para pasos y saltos
    public AudioSource audioSourceRespiracion; // Un segundo AudioSource exclusivo para la respiración (¡Recomendado!)
    public AudioClip sonidoSalto;
    public AudioClip sonidoCorrer;
    public AudioClip sonidoRespirar;

    private float contadorPasos = 0f;
    public float intervaloPasos = 0.35f;

    private float tiempoQuieto = 0f;
    public float tiempoParaComenzarRespirar = 1.5f; // Tiempo quieto antes de empezar a respirar

    void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();

        if (audioSourceTlali == null)
        {
            audioSourceTlali = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        Horizontal = Input.GetAxisRaw("Horizontal");

        if (Horizontal < 0.0f) transform.localScale = new Vector3(-3.0f, 1.67f, 1.0f);
        else if (Horizontal > 0.0f) transform.localScale = new Vector3(3.0f, 1.67f, 1.0f);

        Animator.SetBool("runing", Horizontal != 0.0f);

        Vector2 posicionPies = new Vector2(transform.position.x, transform.position.y - alturaPies);

        Debug.DrawRay(posicionPies, Vector3.down * 0.2f, Color.red);
        if (Physics2D.Raycast(posicionPies, Vector3.down, 0.2f))
        {
            Grounded = true;
        }
        else
        {
            Grounded = false;
        }

        // Lógica de Salto
        if (Input.GetKeyDown(KeyCode.W) && Grounded)
        {
            Jump();

            if (audioSourceTlali != null && sonidoSalto != null)
            {
                audioSourceTlali.PlayOneShot(sonidoSalto);
            }

            DetenerRespiracion();
            tiempoQuieto = 0f;
        }

        // Lógica de Correr (Pasos)
        if (Horizontal != 0.0f && Grounded)
        {
            contadorPasos += Time.deltaTime;
            if (contadorPasos >= intervaloPasos)
            {
                if (audioSourceTlali != null && sonidoCorrer != null)
                {
                    audioSourceTlali.PlayOneShot(sonidoCorrer);
                }
                contadorPasos = 0f;
            }

            DetenerRespiracion();
            tiempoQuieto = 0f;
        }
        else if (Horizontal == 0.0f && Grounded)
        {
            contadorPasos = intervaloPasos;
            tiempoQuieto += Time.deltaTime;

            // Si pasa el tiempo de quietud y no está sonando la respiración, la iniciamos en bucle
            if (tiempoQuieto >= tiempoParaComenzarRespirar)
            {
                if (audioSourceRespiracion != null && sonidoRespirar != null)
                {
                    if (!audioSourceRespiracion.isPlaying)
                    {
                        audioSourceRespiracion.clip = sonidoRespirar;
                        audioSourceRespiracion.loop = true; // Activa el bucle automático
                        audioSourceRespiracion.Play();
                    }
                }
            }
        }
        else
        {
            // Si está en el aire (saltando/cayendo)
            DetenerRespiracion();
            tiempoQuieto = 0f;
        }

        Animator.SetFloat("VerticalVelocity", Rigidbody2D.linearVelocity.y);
        Animator.SetBool("isGrounded", Grounded);
    }

    void DetenerRespiracion()
    {
        if (audioSourceRespiracion != null && audioSourceRespiracion.isPlaying)
        {
            audioSourceRespiracion.Stop();
            audioSourceRespiracion.loop = false;
        }
    }

    private void Jump()
    {
        Rigidbody2D.AddForce(Vector2.up * JumpForce);
    }

    private void FixedUpdate()
    {
        Rigidbody2D.linearVelocity = new Vector2(Horizontal * Speed, Rigidbody2D.linearVelocity.y);
    }
}