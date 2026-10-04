using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MovimientoPersonaje : MonoBehaviour
{
    public float Speed;
    public float JumpForce;

    private Rigidbody2D Rigidbody2D;
    private Animator Animator;
    private float Horizontal;
    private bool Grounded;


    [SerializeField] private float alturaPies = 0.1258188f;

    void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
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

        if (Input.GetKeyDown(KeyCode.W) && Grounded)
        {
            Jump();
        }

        // Agregar animaciones salto y caida
        Animator.SetFloat("VerticalVelocity", Rigidbody2D.linearVelocity.y);
        Animator.SetBool("isGrounded", Grounded);
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