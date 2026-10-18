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
    }

    public void RecolectarCarrete(int numero, string nombreNahuatl)
    {
        carretesRecolectados++;
        ActualizarContadorUI();

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