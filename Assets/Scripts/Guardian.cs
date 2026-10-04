using UnityEngine;
using TMPro; 

public class GuardianRango : MonoBehaviour
{
    [Header("Referencias")]
    private Animator animator;          
    public GameObject canvasPrueba;     

    [Header("Configuración")]
    public string tagDelJugador = "Player"; 

    private void Start()
    {
        
        animator = GetComponentInParent<Animator>();

        if (animator == null)
        {
            Debug.LogError("No se encontró ningún componente Animator en los objetos padres.", this);
        }

      
        if (canvasPrueba != null)
        {
            canvasPrueba.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag(tagDelJugador))
        {
           
            if (animator != null)
            {
                animator.SetBool("Voltear", true);
            }

          
            if (canvasPrueba != null)
            {
                canvasPrueba.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
       
        if (collision.CompareTag(tagDelJugador))
        {
            
            if (animator != null)
            {
                animator.SetBool("Voltear", false);
            }

           
            if (canvasPrueba != null)
            {
                canvasPrueba.SetActive(false);
            }
        }
    }
}