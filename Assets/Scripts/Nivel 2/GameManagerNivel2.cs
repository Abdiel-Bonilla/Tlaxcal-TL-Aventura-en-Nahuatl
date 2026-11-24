using UnityEngine;
using TMPro;
using System.Collections;

public class GameManagerNivel2 : MonoBehaviour
{
    public static GameManagerNivel2 Instance;

    [Header("Contador Global (10 Colores en Náhuatl)")]
    public int totalColoresObjetivo = 10;
    private int coloresRecolectados = 0;
    public TextMeshProUGUI textoContadorUI; // Ejemplo: "0/10"

    [Header("Aviso Global de Interacción")]
    public GameObject panelAvisoQGlobal; // Panel que dice "Pulsa Q" en el Canvas

    [Header("Mensaje Temporal en Pantalla")]
    public GameObject panelMensajeTemporal; // Recuadro para mensajes temporales
    public TextMeshProUGUI textoMensajeUI;  // Texto dentro del recuadro
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

    // Método principal para recolectar cada uno de los 10 colores
    public void RegistrarColorRecolectado(string nombreNahuatl, int indiceColor = -1, string colorEspañol = "")
    {
        coloresRecolectados++;
        ActualizarContadorUI();

        string detalleColor = string.IsNullOrEmpty(colorEspañol)
            ? nombreNahuatl
            : $"{nombreNahuatl} ({colorEspañol})";

        string mensaje = $"¡Color descubierto!\n{detalleColor}";

        if (coloresRecolectados >= totalColoresObjetivo)
        {
            StartCoroutine(SecuenciaFinal(mensaje));
        }
        else
        {
            MostrarMensajeEnPantalla(mensaje);
        }

        // Si tu tableta tiene un método para registrar color o posición, puedes conectarlo aquí
        if (TabletNivel2Controller.Instance != null && indiceColor >= 0)
        {
            TabletNivel2Controller.Instance.RegistrarLetraPorPosicion(indiceColor);
        }
    }

    // Sobrecarga/compatibilidad por si algún script llama con un solo string
    public void RegistrarColorRecolectado(string nombreNahuatl)
    {
        RegistrarColorRecolectado(nombreNahuatl, -1, "");
    }

    // Compatibilidad por si algún script antiguo aún llama RecolectarLetra
    public void RecolectarLetra(int posicionFija, string grafiaNahuatl)
    {
        RegistrarColorRecolectado(grafiaNahuatl, posicionFija, "");
    }

    IEnumerator SecuenciaFinal(string mensajeUltimoColor)
    {
        MostrarMensajeEnPantalla(mensajeUltimoColor);
        yield return new WaitForSeconds(duracionMensaje);
        MostrarMensajeEnPantalla("¡Has reunido todos los colores en náhuatl del taller!\n¡Ve a la prueba del Guardián del Conocimiento!");
    }

    void ActualizarContadorUI()
    {
        if (textoContadorUI != null)
        {
            textoContadorUI.text = coloresRecolectados + "/" + totalColoresObjetivo;
        }
    }

    // Método que usa GuardianNivel2 para validar si ya juntó los 10 colores
    public int GetColoresRecolectados()
    {
        return coloresRecolectados;
    }

    // Compatibilidad con el nombre anterior
    public int GetLetrasRecolectadas()
    {
        return coloresRecolectados;
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