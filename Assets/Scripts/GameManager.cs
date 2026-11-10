using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Contador Global")]
    public int totalCarretesObjetivo = 10;
    private int carretesRecolectados = 0;
    public TextMeshProUGUI textoContadorUI;

    [Header("Mensaje Temporal en Pantalla")]
    public GameObject panelMensajeTemporal;
    public TextMeshProUGUI textoMensajeUI;
    public float duracionMensaje = 2.5f;
    private Coroutine rutinaMensaje;

    [Header("Efectos de Sonido (Náhuatl)")]
    public AudioSource audioSource;
    [Tooltip("Coloca aquí los 10 audios en orden: Element 0 = Ce (1), Element 1 = Ome (2), etc.")]
    public AudioClip[] audiosNahuatl; // Arrastrarás los 10 audios aquí en el Inspector

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ActualizarContadorUI();
        if (panelMensajeTemporal != null)
        {
            panelMensajeTemporal.SetActive(false);
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void RecolectarCarrete(int numero, string nombreNahuatl)
    {
        carretesRecolectados++;
        ActualizarContadorUI();

        // Reproducir el audio náhuatl correspondiente al número del carrete
        int indiceAudio = numero - 1; // Restamos 1 porque los arreglos empiezan en 0
        if (audioSource != null && audiosNahuatl != null && indiceAudio >= 0 && indiceAudio < audiosNahuatl.Length)
        {
            if (audiosNahuatl[indiceAudio] != null)
            {
                audioSource.PlayOneShot(audiosNahuatl[indiceAudio]);
            }
        }

        // Si es el último carrete, encadenamos los mensajes para que no se encimen
        if (carretesRecolectados >= totalCarretesObjetivo)
        {
            StartCoroutine(SecuenciaMensajesFinales(numero + " - " + nombreNahuatl));
        }
        else
        {
            MostrarMensajeEnPantalla(numero + " - " + nombreNahuatl);
        }

        if (TabletController.Instance != null)
        {
            TabletController.Instance.RegistrarNumero(numero);
        }
    }

    // Corrutina para mostrar primero el carrete final y después el aviso del Guardián
    IEnumerator SecuenciaMensajesFinales(string mensajeUltimoCarrete)
    {
        // 1. Mostrar el mensaje del último carrete recolectado
        MostrarMensajeEnPantalla(mensajeUltimoCarrete);

        // 2. Esperar el tiempo exacto que dura el mensaje en pantalla
        yield return new WaitForSeconds(duracionMensaje);

        // 3. Mostrar inmediatamente el mensaje de que ya está listo para el Guardián
        MostrarMensajeEnPantalla("¡Has reunido todos los carretes!\n¡Ve a la Prueba del Guardián!");
    }

    public int GetCarretesRecolectados()
    {
        return carretesRecolectados;
    }

    void ActualizarContadorUI()
    {
        if (textoContadorUI != null)
        {
            textoContadorUI.text = carretesRecolectados + "/" + totalCarretesObjetivo;
        }
    }

    public void MostrarMensajeEnPantalla(string mensaje)
    {
        if (panelMensajeTemporal != null && textoMensajeUI != null)
        {
            textoMensajeUI.text = mensaje;
            panelMensajeTemporal.SetActive(true);

            if (rutinaMensaje != null)
            {
                StopCoroutine(rutinaMensaje);
            }
            rutinaMensaje = StartCoroutine(OcultarMensajeRutina());
        }
    }

    IEnumerator OcultarMensajeRutina()
    {
        yield return new WaitForSeconds(duracionMensaje);
        panelMensajeTemporal.SetActive(false);
    }
}