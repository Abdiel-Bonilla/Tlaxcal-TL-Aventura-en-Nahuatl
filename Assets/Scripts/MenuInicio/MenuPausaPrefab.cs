using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class MenuPausaPrefab : MonoBehaviour
{
    [Header("Teclas de Pausa")]
    public KeyCode teclaPausaPrincipal = KeyCode.Escape;
    public KeyCode teclaPausaAlternativa = KeyCode.P;

    [Header("Paneles Internos del Prefab (Hermanos)")]
    public GameObject panelPausaPrincipal;
    public GameObject panelOpcionesPausa;

    [Header("Botones del Menú de Pausa")]
    public Button botonReanudar;
    public Button botonCerrarMenuPausa; // Botón Cerrar dentro de PanelPausaPrincipal
    public Button botonOpciones;
    public Button botonReiniciarNivel;
    public Button botonMenuInicio;

    [Header("Botones de Opciones")]
    public Button botonCerrarOpciones;  // Botón Cerrar dentro de PanelOpcionesPausa

    [Header("Opciones de Audio (Sincronizadas con Menú Principal)")]
    public Slider sliderVolumenGeneral;
    public Toggle toggleSilenciarTodo;
    public TextMeshProUGUI textoPorcentajeVolumen;

    [Header("Escena del Menú Principal")]
    public string nombreEscenaMenu = "MenuInicio";

    private const string KEY_VOLUMEN = "VolumenGeneral";
    private const string KEY_MUTE = "AudioSilenciado";

    private bool estaPausado = false;

    private void Awake()
    {
        float volumenGuardado = PlayerPrefs.GetFloat(KEY_VOLUMEN, 1f);
        bool estaSilenciado = PlayerPrefs.GetInt(KEY_MUTE, 0) == 1;
        AudioListener.volume = estaSilenciado ? 0f : volumenGuardado;
    }

    private void Start()
    {
        estaPausado = false;
        Time.timeScale = 1f;

        if (panelPausaPrincipal != null) panelPausaPrincipal.SetActive(false);
        if (panelOpcionesPausa != null) panelOpcionesPausa.SetActive(false);

        ConfigurarBotones();
        ConfigurarListenersOpciones();
        SincronizarValoresUI();
    }

    private void Update()
    {
        if (EstaEscribiendoEnCampoTexto()) return;

        if (Input.GetKeyDown(teclaPausaPrincipal) || Input.GetKeyDown(teclaPausaAlternativa))
        {
            if (estaPausado)
            {
                ReanudarJuego();
            }
            else
            {
                PausarJuego();
            }
        }
    }

    private bool EstaEscribiendoEnCampoTexto()
    {
        if (EventSystem.current == null || EventSystem.current.currentSelectedGameObject == null)
            return false;

        GameObject seleccionado = EventSystem.current.currentSelectedGameObject;
        return seleccionado.GetComponent<TMP_InputField>() != null ||
               seleccionado.GetComponent<InputField>() != null;
    }

    private void ConfigurarBotones()
    {
        if (botonReanudar != null)
        {
            botonReanudar.onClick.RemoveAllListeners();
            botonReanudar.onClick.AddListener(ReanudarJuego);
        }

        if (botonCerrarMenuPausa != null)
        {
            botonCerrarMenuPausa.onClick.RemoveAllListeners();
            botonCerrarMenuPausa.onClick.AddListener(ReanudarJuego);
        }

        if (botonOpciones != null)
        {
            botonOpciones.onClick.RemoveAllListeners();
            botonOpciones.onClick.AddListener(AbrirOpciones);
        }

        if (botonCerrarOpciones != null)
        {
            botonCerrarOpciones.onClick.RemoveAllListeners();
            botonCerrarOpciones.onClick.AddListener(VolverAPausaPrincipal);
        }

        if (botonReiniciarNivel != null)
        {
            botonReiniciarNivel.onClick.RemoveAllListeners();
            botonReiniciarNivel.onClick.AddListener(ReiniciarNivelActual);
        }

        if (botonMenuInicio != null)
        {
            botonMenuInicio.onClick.RemoveAllListeners();
            botonMenuInicio.onClick.AddListener(IrAlMenuInicio);
        }
    }

    private void ConfigurarListenersOpciones()
    {
        if (sliderVolumenGeneral != null)
        {
            sliderVolumenGeneral.minValue = 0f;
            sliderVolumenGeneral.maxValue = 1f;
            sliderVolumenGeneral.onValueChanged.RemoveAllListeners();
            sliderVolumenGeneral.onValueChanged.AddListener(CambiarVolumenGeneral);
        }

        if (toggleSilenciarTodo != null)
        {
            toggleSilenciarTodo.onValueChanged.RemoveAllListeners();
            toggleSilenciarTodo.onValueChanged.AddListener(CambiarSilencio);
        }
    }

    private void SincronizarValoresUI()
    {
        float volumenGuardado = PlayerPrefs.GetFloat(KEY_VOLUMEN, 1f);
        bool estaSilenciado = PlayerPrefs.GetInt(KEY_MUTE, 0) == 1;

        if (sliderVolumenGeneral != null)
        {
            sliderVolumenGeneral.SetValueWithoutNotify(volumenGuardado);
        }

        if (toggleSilenciarTodo != null)
        {
            toggleSilenciarTodo.SetIsOnWithoutNotify(estaSilenciado);
        }

        ActualizarTextoVolumen(volumenGuardado);
    }

    public void PausarJuego()
    {
        if (estaPausado) return;

        estaPausado = true;
        Time.timeScale = 0f;
        SincronizarValoresUI();

        if (panelPausaPrincipal != null) panelPausaPrincipal.SetActive(true);
        if (panelOpcionesPausa != null) panelOpcionesPausa.SetActive(false);
    }

    public void ReanudarJuego()
    {
        estaPausado = false;
        Time.timeScale = 1f;

        if (panelPausaPrincipal != null) panelPausaPrincipal.SetActive(false);
        if (panelOpcionesPausa != null) panelOpcionesPausa.SetActive(false);
    }

    public void AbrirOpciones()
    {
        SincronizarValoresUI();
        if (panelPausaPrincipal != null) panelPausaPrincipal.SetActive(false);
        if (panelOpcionesPausa != null) panelOpcionesPausa.SetActive(true);
    }

    public void VolverAPausaPrincipal()
    {
        if (panelOpcionesPausa != null) panelOpcionesPausa.SetActive(false);
        if (panelPausaPrincipal != null) panelPausaPrincipal.SetActive(true);
    }

    public void CambiarVolumenGeneral(float valor)
    {
        PlayerPrefs.SetFloat(KEY_VOLUMEN, valor);
        bool estaSilenciado = toggleSilenciarTodo != null && toggleSilenciarTodo.isOn;
        AudioListener.volume = estaSilenciado ? 0f : valor;
        ActualizarTextoVolumen(valor);
    }

    public void CambiarSilencio(bool silenciar)
    {
        PlayerPrefs.SetInt(KEY_MUTE, silenciar ? 1 : 0);
        float volumenActual = sliderVolumenGeneral != null ? sliderVolumenGeneral.value : PlayerPrefs.GetFloat(KEY_VOLUMEN, 1f);
        AudioListener.volume = silenciar ? 0f : volumenActual;
    }

    private void ActualizarTextoVolumen(float valor)
    {
        if (textoPorcentajeVolumen != null)
        {
            textoPorcentajeVolumen.text = Mathf.RoundToInt(valor * 100f) + "%";
        }
    }

    public void ReiniciarNivelActual()
    {
        Time.timeScale = 1f;
        Scene escenaActual = SceneManager.GetActiveScene();
        SceneManager.LoadScene(escenaActual.name);
    }

    public void IrAlMenuInicio()
    {
        Time.timeScale = 1f;

        if (string.IsNullOrEmpty(nombreEscenaMenu))
        {
            Debug.LogWarning("Configura 'Nombre Escena Menu' en MenuPausaPrefab.");
            return;
        }

        if (Application.CanStreamedLevelBeLoaded(nombreEscenaMenu))
        {
            SceneManager.LoadScene(nombreEscenaMenu);
        }
        else
        {
            Debug.LogWarning($"La escena '{nombreEscenaMenu}' no está en Build Settings o aún no existe.");
        }
    }
}