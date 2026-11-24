using UnityEngine;

public class ColorNahuatlTapete : MonoBehaviour
{
    [Header("Configuración de este Color")]
    [Range(1, 10)]
    public int posicionColor = 1; // 1 a 10 según la lista de la tablet
    public string nombreNahuatl = "Chichiltik";
    public string colorEspañol = "Rojo";
    public AudioClip audioPronunciacion;

    [Header("Opcional: Si la bolita tiene otro nombre")]
    public GameObject animacionBolita;

    private bool playerInRange = false;
    private bool yaFueInteractuado = false;

    void Update()
    {
        // Si el jugador está cerca, no se ha recolectado y presiona Q
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

        // Reproducir audio de pronunciación del color en náhuatl
        if (audioPronunciacion != null)
        {
            AudioSource.PlayClipAtPoint(audioPronunciacion, transform.position, 1f);
        }

        // Enviar datos al GameManager para registrar el color y actualizar la tablet
        if (GameManagerNivel2.Instance != null)
        {
            GameManagerNivel2.Instance.RegistrarColorRecolectado(nombreNahuatl, posicionColor, colorEspañol);
        }

        // Apagar la animación de la bolita flotante dentro de este objeto sin destruir el objeto
        Transform bolita = transform.Find("AnimacionBolita");
        if (bolita != null)
        {
            bolita.gameObject.SetActive(false);
        }

        if (animacionBolita != null)
        {
            animacionBolita.SetActive(false);
        }

        Debug.Log("Color interactuado: " + nombreNahuatl + " (" + colorEspañol + ")");
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