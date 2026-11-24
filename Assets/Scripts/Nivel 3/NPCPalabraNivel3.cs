using UnityEngine;

public class NPCPalabraNivel3 : MonoBehaviour
{
    [Header("Palabra Cotidiana del NPC (1 al 8)")]
    [Range(1, 8)]
    public int posicionPalabra = 1;
    public string palabraEspanol = "Luna";
    public string palabraNahuatl = ""; // Escribe aquí la palabra en náhuatl en el Inspector
    public AudioClip audioPronunciacion; // Opcional

    [Header("Burbuja sobre la Cabeza del NPC")]
    public GameObject burbujaCabeza; // Arrastra aquí la burbuja hija del NPC (o se buscará por nombre "Burbuja")
    public KeyCode teclaInteraccion = KeyCode.Q;

    private bool playerInRange = false;
    private bool yaHablamos = false;

    private void Start()
    {
        if (burbujaCabeza == null)
        {
            Transform hijoBurbuja = transform.Find("Burbuja");
            if (hijoBurbuja != null) burbujaCabeza = hijoBurbuja.gameObject;
        }

        if (burbujaCabeza != null)
        {
            burbujaCabeza.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && !yaHablamos && Input.GetKeyDown(teclaInteraccion))
        {
            InteractuarConNPC();
        }
    }

    void InteractuarConNPC()
    {
        yaHablamos = true;

        // Ocultar burbuja del NPC y aviso global de Q sin destruir el NPC
        if (burbujaCabeza != null)
        {
            burbujaCabeza.SetActive(false);
        }

        if (GameManagerNivel3.Instance != null && GameManagerNivel3.Instance.panelAvisoQGlobal != null)
        {
            GameManagerNivel3.Instance.panelAvisoQGlobal.SetActive(false);
        }

        if (audioPronunciacion != null)
        {
            AudioSource.PlayClipAtPoint(audioPronunciacion, transform.position, 1f);
        }

        if (GameManagerNivel3.Instance != null)
        {
            GameManagerNivel3.Instance.RegistrarPalabraRecolectada(posicionPalabra, palabraEspanol, palabraNahuatl);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (yaHablamos) return;

        if (collision.CompareTag("Player"))
        {
            playerInRange = true;

            if (burbujaCabeza != null)
            {
                burbujaCabeza.SetActive(true);
            }

            if (GameManagerNivel3.Instance != null && GameManagerNivel3.Instance.panelAvisoQGlobal != null)
            {
                GameManagerNivel3.Instance.panelAvisoQGlobal.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;

            if (burbujaCabeza != null)
            {
                burbujaCabeza.SetActive(false);
            }

            if (GameManagerNivel3.Instance != null && GameManagerNivel3.Instance.panelAvisoQGlobal != null)
            {
                GameManagerNivel3.Instance.panelAvisoQGlobal.SetActive(false);
            }
        }
    }
}