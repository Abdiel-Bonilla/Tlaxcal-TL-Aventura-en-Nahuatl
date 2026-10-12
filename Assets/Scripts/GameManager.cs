using UnityEngine;
using TMPro; 
using System.Collections;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    [Header("Contador Global")]
    public int totalCarretesObjetivo = 11; 
    private int carretesRecolectados = 0;
    public TextMeshProUGUI textoContadorUI;

    [Header("Mensaje Temporal en Pantalla")]
    public GameObject panelMensajeTemporal; 
    public TextMeshProUGUI textoMensajeUI;  
    public float duracionMensaje = 2.0f;  
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
        MostrarMensajeEnPantalla(numero + " - " + nombreNahuatl);

        // Notificar a la tablet 
        if (TabletController.Instance != null)
        {
            TabletController.Instance.RegistrarNumero(numero);
        }

        if (carretesRecolectados >= totalCarretesObjetivo)
        {
            Debug.Log("¡Todos los carretes recolectados! Listo para el reto del Guardián.");
        }
    }

    void ActualizarContadorUI()
    {
        if (textoContadorUI != null)
        {

            textoContadorUI.text = carretesRecolectados + "/" + totalCarretesObjetivo;
        }
    }

    void MostrarMensajeEnPantalla(string mensaje)
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