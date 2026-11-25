using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuPrincipalController : MonoBehaviour
{
    [Header("Escena Inicial del Juego")]
    public string nombreEscenaPrimerNivel = "Nivel1";

    [Header("Botones Principales")]
    public Button botonJugar;
    public Button botonOpciones;
    public Button botonSalir;

    [Header("Paneles")]
    public GameObject panelMenuPrincipal;
    public GameObject panelOpciones;
    public Button botonCerrarOpciones;

    [Header("Ajustes Dentro de Opciones")]
    public Slider sliderVolumenGeneral;
    public Toggle toggleSilenciarTodo;
    public Toggle togglePantallaCompleta;
    public TextMeshProUGUI textoPorcentajeVolumen;

    [Header("Audio de Fondo del Menú (Opcional)")]
    public AudioSource audioSourceMenu;
    public AudioClip musicaMenu;
    public bool repetirMusica = true;
    [Range(0f, 1f)]
    public float volumenBaseMusica = 0.6f;

    [Header("Aviso en Pantalla")]
    public TextMeshProUGUI textoEstadoUI;

    private const string KEY_VOLUMEN = "VolumenGeneral";
    private const string KEY_MUTE = "AudioSilenciado";

    private void Awake()
    {
        Time.timeScale = 1f;

        float volumenGuardado = PlayerPrefs.GetFloat(KEY_VOLUMEN, 1f);
        bool estaSilenciado = PlayerPrefs.GetInt(KEY_MUTE, 0) == 1;

        AudioListener.volume = estaSilenciado ? 0f : volumenGuardado;

        if (audioSourceMenu != null && musicaMenu != null)
        {
            audioSourceMenu.playOnAwake = false;
            audioSourceMenu.loop = repetirMusica;
            audioSourceMenu.volume = volumenBaseMusica;
            audioSourceMenu.clip = musicaMenu;
            audioSourceMenu.Play();
        }
    }

    private void Start()
    {
        MostrarMenuPrincipal();
        ConfigurarControlesOpciones();

        if (botonJugar != null)
        {
            botonJugar.onClick.RemoveAllListeners();
            botonJugar.onClick.AddListener(IniciarJuego);
        }

        if (botonOpciones != null)
        {
            botonOpciones.onClick.RemoveAllListeners();
            botonOpciones.onClick.AddListener(AbrirOpciones);
        }

        if (botonSalir != null)
        {
            botonSalir.onClick.RemoveAllListeners();
            botonSalir.onClick.AddListener(SalirDelJuego);
        }

        if (botonCerrarOpciones != null)
        {
            botonCerrarOpciones.onClick.RemoveAllListeners();
            botonCerrarOpciones.onClick.AddListener(MostrarMenuPrincipal);
        }
    }

    private void ConfigurarControlesOpciones()
    {
        float volumenGuardado = PlayerPrefs.GetFloat(KEY_VOLUMEN, 1f);
        bool estaSilenciado = PlayerPrefs.GetInt(KEY_MUTE, 0) == 1;

        if (sliderVolumenGeneral != null)
        {
            sliderVolumenGeneral.minValue = 0f;
            sliderVolumenGeneral.maxValue = 1f;
            sliderVolumenGeneral.value = volumenGuardado;
            sliderVolumenGeneral.onValueChanged.RemoveAllListeners();
            sliderVolumenGeneral.onValueChanged.AddListener(CambiarVolumenGeneral);
        }

        if (toggleSilenciarTodo != null)
        {
            toggleSilenciarTodo.isOn = estaSilenciado;
            toggleSilenciarTodo.onValueChanged.RemoveAllListeners();
            toggleSilenciarTodo.onValueChanged.AddListener(CambiarSilencio);
        }

        if (togglePantallaCompleta != null)
        {
            togglePantallaCompleta.isOn = Screen.fullScreen;
            togglePantallaCompleta.onValueChanged.RemoveAllListeners();
            togglePantallaCompleta.onValueChanged.AddListener(CambiarPantallaCompleta);
        }

        ActualizarTextoVolumen(volumenGuardado);
    }

    public void MostrarMenuPrincipal()
    {
        if (panelMenuPrincipal != null) panelMenuPrincipal.SetActive(true);
        if (panelOpciones != null) panelOpciones.SetActive(false);
    }

    public void AbrirOpciones()
    {
        if (panelMenuPrincipal != null) panelMenuPrincipal.SetActive(false);
        if (panelOpciones != null) panelOpciones.SetActive(true);
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

    public void CambiarPantallaCompleta(bool pantallaCompleta)
    {
        Screen.fullScreen = pantallaCompleta;
    }

    private void ActualizarTextoVolumen(float valor)
    {
        if (textoPorcentajeVolumen != null)
        {
            textoPorcentajeVolumen.text = Mathf.RoundToInt(valor * 100f) + "%";
        }
    }

    public void IniciarJuego()
    {
        Time.timeScale = 1f;

        if (string.IsNullOrEmpty(nombreEscenaPrimerNivel))
        {
            MostrarAviso("Configura el nombre de la escena del primer nivel.");
            return;
        }

        if (Application.CanStreamedLevelBeLoaded(nombreEscenaPrimerNivel))
        {
            if (audioSourceMenu != null && audioSourceMenu.isPlaying)
            {
                audioSourceMenu.Stop();
            }
            SceneManager.LoadScene(nombreEscenaPrimerNivel);
        }
        else
        {
            string msg = $"La escena '{nombreEscenaPrimerNivel}' no está en Build Settings o aún no existe.";
            Debug.LogWarning(msg);
            MostrarAviso(msg);
        }
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo de Tlaxcal-TL...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void MostrarAviso(string mensaje)
    {
        if (textoEstadoUI != null)
        {
            textoEstadoUI.text = mensaje;
        }
    }
}