using UnityEngine;
using TMPro;
using System.Collections;

public class GameManagerNivel3 : MonoBehaviour
{
    public static GameManagerNivel3 Instance;

    [Header("Contador Global (8 Palabras Cotidianas)")]
    public int totalPalabrasObjetivo = 8;
    private int palabrasRecolectadas = 0;
    public TextMeshProUGUI textoContadorUI; // Ejemplo: "0/8"

    [Header("Aviso Global de Interacción")]
    public GameObject panelAvisoQGlobal; // Panel opcional "Pulsa Q" en el Canvas

    [Header("Mensaje Temporal en Pantalla")]
    public GameObject panelMensajeTemporal;
    public TextMeshProUGUI textoMensajeUI;
    public float duracionMensaje = 3.0f;
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
            panelMensajeTemporal.SetActive(false);

        if (panelAvisoQGlobal != null)
            panelAvisoQGlobal.SetActive(false);
    }

    // Registra una palabra de posición 1 a 8 con su equivalente en español y náhuatl
    public void RegistrarPalabraRecolectada(int posicionFija, string palabraEspanol, string palabraNahuatl)
    {
        palabrasRecolectadas++;
        ActualizarContadorUI();

        string mensaje = $"¡Nueva palabra descubierta!\n<b>{palabraEspanol}</b> — <i>{palabraNahuatl}</i>";

        if (palabrasRecolectadas >= totalPalabrasObjetivo)
        {
            StartCoroutine(SecuenciaCompleta(mensaje));
        }
        else
        {
            MostrarMensajeEnPantalla(mensaje);
        }

        if (TabletNivel3Controller.Instance != null)
        {
            TabletNivel3Controller.Instance.RegistrarPalabraPorPosicion(posicionFija);
        }
    }

    IEnumerator SecuenciaCompleta(string mensajeUltimaPalabra)
    {
        MostrarMensajeEnPantalla(mensajeUltimaPalabra);
        yield return new WaitForSeconds(duracionMensaje);
        MostrarMensajeEnPantalla("¡Has reunido las 8 palabras cotidianas en la Galería!\n¡Ve a la prueba final!");
    }

    void ActualizarContadorUI()
    {
        if (textoContadorUI != null)
        {
            textoContadorUI.text = palabrasRecolectadas + "/" + totalPalabrasObjetivo;
        }
    }

    public int GetPalabrasRecolectadas()
    {
        return palabrasRecolectadas;
    }

    public void MostrarMensajeEnPantalla(string mensaje)
    {
        if (panelMensajeTemporal != null && textoMensajeUI != null)
        {
            textoMensajeUI.text = mensaje;
            panelMensajeTemporal.SetActive(true);

            if (rutinaMensaje != null) StopCoroutine(rutinaMensaje);
            rutinaMensaje = StartCoroutine(OcultarMensajeRutina());
        }
    }

    IEnumerator OcultarMensajeRutina()
    {
        yield return new WaitForSeconds(duracionMensaje);
        panelMensajeTemporal.SetActive(false);
    }
}