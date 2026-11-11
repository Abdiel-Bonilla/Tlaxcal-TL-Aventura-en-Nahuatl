using UnityEngine;

public class TapeteInteractuableNivel2 : MonoBehaviour
{
    [Header("Configuración de este Tapete")]
    [Tooltip("Posición fija en la tabla del 1 al 17 (Ej. 1 para A, 2 para CH, 14 para TZ)")]
    public int posicionFijaTabla = 1;

    [Tooltip("La grafía exacta que se mostrará (Ej. 'A', 'CH', 'TZ')")]
    public string grafiaNahuatl = "A";

    private bool playerInRange = false;
    private bool yaFueInteractuado = false;

    void Update()
    {
        // Si el jugador está cerca y presiona Q
        if (playerInRange && !yaFueInteractuado && Input.GetKeyDown(KeyCode.Q))
        {
            Interactuar();
        }
    }

    void Interactuar()
    {
        yaFueInteractuado = true;

        // Apagar el aviso global de "Pulsa Q" a través del GameManager
        if (GameManagerNivel2.Instance != null && GameManagerNivel2.Instance.panelAvisoQGlobal != null)
        {
            GameManagerNivel2.Instance.panelAvisoQGlobal.SetActive(false);
        }

        // Enviar datos al GameManager para registrar la letra y actualizar la tablet
        if (GameManagerNivel2.Instance != null)
        {
            GameManagerNivel2.Instance.RecolectarLetra(posicionFijaTabla, grafiaNahuatl);
        }

        // Apagar la animación de la bolita flotante dentro de este tapete específico
        Transform bolita = transform.Find("AnimacionBolita");
        if (bolita != null)
        {
            bolita.gameObject.SetActive(false);
        }

        Debug.Log("Tapete interactuado. Letra: " + " -> " + grafiaNahuatl);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detectar si el objeto que entra tiene el tag "Player"
        if (collision.CompareTag("Player") && !yaFueInteractuado)
        {
            playerInRange = true;

            // Encender el panel global de "Pulsa Q"
            if (GameManagerNivel2.Instance != null && GameManagerNivel2.Instance.panelAvisoQGlobal != null)
            {
                GameManagerNivel2.Instance.panelAvisoQGlobal.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;

            // Apagar el panel global cuando el jugador se aleje
            if (GameManagerNivel2.Instance != null && GameManagerNivel2.Instance.panelAvisoQGlobal != null)
            {
                GameManagerNivel2.Instance.panelAvisoQGlobal.SetActive(false);
            }
        }
    }
}