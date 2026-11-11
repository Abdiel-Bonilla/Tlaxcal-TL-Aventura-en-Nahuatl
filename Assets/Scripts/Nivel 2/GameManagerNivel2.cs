using UnityEngine;
using TMPro;
using System.Collections;

public class GameManagerNivel2 : MonoBehaviour
{
    public static GameManagerNivel2 Instance;

    [Header("Contador Global (17 Letras del Abecedario)")]
    public int totalLetrasObjetivo = 17;
    private int letrasRecolectadas = 0;
    public TextMeshProUGUI textoContadorUI; // Arrastra aquí tu texto de UI ej. "0/17"

    [Header("Aviso Global de Interacción")]
    public GameObject panelAvisoQGlobal; // Arrastra aquí el Panel que dice "Pulsa Q" desde tu Canvas

    [Header("Mensaje Temporal en Pantalla")]
    public GameObject panelMensajeTemporal; // El recuadro o panel de fondo para "¡Letra encontrada!"
    public TextMeshProUGUI textoMensajeUI;  // El texto dentro del recuadro
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
            panelMensajeTemporal.SetActive(false);

        if (panelAvisoQGlobal != null)
            panelAvisoQGlobal.SetActive(false);
    }

    public void RecolectarLetra(int posicionFija, string grafiaNahuatl)
    {
        letrasRecolectadas++;
        ActualizarContadorUI();

        string mensaje = "¡Letra encontrada!\n Letra " + " - " + grafiaNahuatl;

        if (letrasRecolectadas >= totalLetrasObjetivo)
        {
            StartCoroutine(SecuenciaFinal(mensaje));
        }
        else
        {
            MostrarMensajeEnPantalla(mensaje);
        }

        // Comunicarnos con la Tablet del Nivel 2 usando la posición fija
        if (TabletNivel2Controller.Instance != null)
        {
            TabletNivel2Controller.Instance.RegistrarLetraPorPosicion(posicionFija);
        }
    }

    IEnumerator SecuenciaFinal(string mensajeUltimaLetra)
    {
        MostrarMensajeEnPantalla(mensajeUltimaLetra);
        yield return new WaitForSeconds(duracionMensaje);
        MostrarMensajeEnPantalla("¡Has completado todo el Abecedario Náhuatl de Contla!\n¡Nivel 2 Superado!");
    }

    void ActualizarContadorUI()
    {
        if (textoContadorUI != null)
        {
            textoContadorUI.text = letrasRecolectadas + "/" + totalLetrasObjetivo;
        }
    }

    // Método público necesario para que el Guardián valide cuántas letras lleva
    public int GetLetrasRecolectadas()
    {
        return letrasRecolectadas;
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