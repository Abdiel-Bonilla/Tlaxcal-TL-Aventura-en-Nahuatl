using UnityEngine;

public class CarreteColeccionable : MonoBehaviour
{
    [Header("Configuración Náhuatl")]
    public int numeroValor = 1;         
    public string nombreNahuatl = "Ce"; 
    private bool fueRecolectado = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !fueRecolectado)
        {
            fueRecolectado = true;


            if (GameManager.Instance != null)
            {
                GameManager.Instance.RecolectarCarrete(numeroValor, nombreNahuatl);
            }


            Destroy(gameObject);
        }
    }
}