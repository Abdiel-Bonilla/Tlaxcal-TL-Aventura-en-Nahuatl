using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Text;

public class PantallaVictoriaCreditosNivel3 : MonoBehaviour
{
    [Header("Paneles Principales")]
    public GameObject panelVictoriaCreditos;
    public GameObject panelGameOver;
    public TextMeshProUGUI textoGraciasPorJugar;

    [Header("Audio de Fondo (Créditos y Game Over)")]
    public AudioSource audioFondoFinal;
    public AudioClip clipFondoCreditosYGameOver;
    public bool repetirAudioFondo = true;
    [Range(0f, 1f)]
    public float volumenAudioFondo = 0.7f;

    [Header("Personaje Saltando con Manos Levantadas")]
    public Transform personajeCelebrando; // Acepta tanto UI (RectTransform) como objeto 2D
    public Animator animatorCelebracion;
    public string triggerSaltoBrazos = "Celebrar";
    public float alturaSalto = 45f;
    public float velocidadSalto = 8f;
    public bool agregarSquashYStretch = true;

    [Header("Créditos Ascendentes desde Código")]
    public RectTransform rectContenedorCreditos; // Arrastra aquí TextoCreditosAscendentes
    public TextMeshProUGUI textoCreditosUI;    // Arrastra aquí el mismo TextoCreditosAscendentes
    public float posicionYInicial = -350f;
    public float posicionYFinal = 520f;
    public float velocidadSubidaCreditos = 65f;
    public float pausaAlTerminarCreditos = 1.2f;

    [Header("Botón Menú de Inicio (Game Over)")]
    public Button botonMenuInicio;
    public string nombreEscenaMenu = "MenuInicio";

    private Vector3 posicionBaseMundo;
    private Vector2 posicionBaseUI;
    private Vector3 escalaBase;
    private RectTransform rectCelebracion;
    private bool animandoCelebracion = false;
    private Coroutine rutinaPrincipal;

    private void Awake()
    {
        ConfigurarAudioFondo();
    }

    private void Start()
    {
        if (panelVictoriaCreditos != null) panelVictoriaCreditos.SetActive(false);
        if (panelGameOver != null) panelGameOver.SetActive(false);

        ConfigurarReferenciaPersonaje();
        CargarCreditosDesdeCodigo();

        if (botonMenuInicio != null)
        {
            botonMenuInicio.onClick.RemoveAllListeners();
            botonMenuInicio.onClick.AddListener(IrAlMenuInicio);
        }
    }

    private void ConfigurarAudioFondo()
    {
        if (audioFondoFinal == null)
        {
            audioFondoFinal = GetComponent<AudioSource>();
        }

        if (audioFondoFinal == null)
        {
            audioFondoFinal = gameObject.AddComponent<AudioSource>();
        }

        audioFondoFinal.playOnAwake = false;
        audioFondoFinal.ignoreListenerPause = true;
        audioFondoFinal.loop = repetirAudioFondo;
        audioFondoFinal.volume = volumenAudioFondo;
    }

    private void IniciarAudioFondoFinal()
    {
        if (audioFondoFinal == null || clipFondoCreditosYGameOver == null) return;

        audioFondoFinal.Stop();
        audioFondoFinal.clip = clipFondoCreditosYGameOver;
        audioFondoFinal.loop = repetirAudioFondo;
        audioFondoFinal.volume = volumenAudioFondo;
        audioFondoFinal.Play();
    }

    private void DetenerAudioFondoFinal()
    {
        if (audioFondoFinal == null) return;

        audioFondoFinal.Stop();
        audioFondoFinal.clip = null;
    }

    private void ConfigurarReferenciaPersonaje()
    {
        if (personajeCelebrando == null) return;

        escalaBase = personajeCelebrando.localScale;
        rectCelebracion = personajeCelebrando as RectTransform;

        if (rectCelebracion != null)
        {
            posicionBaseUI = rectCelebracion.anchoredPosition;
        }
        else
        {
            posicionBaseMundo = personajeCelebrando.position;
        }
    }

    private void Update()
    {
        if (!animandoCelebracion || personajeCelebrando == null) return;

        float fase = Mathf.Abs(Mathf.Sin(Time.unscaledTime * velocidadSalto));
        float offsetSalto = fase * alturaSalto;

        if (rectCelebracion != null)
        {
            rectCelebracion.anchoredPosition = posicionBaseUI + new Vector2(0f, offsetSalto);
        }
        else
        {
            personajeCelebrando.position = posicionBaseMundo + new Vector3(0f, offsetSalto * 0.01f, 0f);
        }

        if (agregarSquashYStretch)
        {
            float estirarY = 1f + (fase * 0.08f);
            float encogerX = 1f - (fase * 0.05f);
            personajeCelebrando.localScale = new Vector3(
                escalaBase.x * encogerX,
                escalaBase.y * estirarY,
                escalaBase.z
            );
        }
    }

    private void CargarCreditosDesdeCodigo()
    {
        if (textoCreditosUI == null) return;

        textoCreditosUI.textWrappingMode = TextWrappingModes.NoWrap;
        textoCreditosUI.overflowMode = TextOverflowModes.Overflow;
        textoCreditosUI.alignment = TextAlignmentOptions.Center;

        string[] lineasCreditos = new string[]
        {
            "<b>TLAXCAL-TL: AVENTURA EN NÁHUATL</b>",
            "",
            "<b>Desarrollo, Arquitectura y Programación</b>",
            "- Brandon Candaneda González",
            "- Abdiel Isaí Bonilla Romano",
            "",
            "<b>Asesoría Académica y Técnica</b>",
            "- Dra. Sonia López Rodríguez",
            "",
            "<b>Audio y voces en náhuatl</b>",
            "- Dolores Romano Hernandez",
            "",
            "<b>Contexto Académico</b>",
            "- Asignatura: Optativa de Creación de Videojuegos",
            "- Carrera: Ingeniería en Desarrollo y Gestión de Software (IDGS) — 10° A",
            "- Universidad Tecnológica de Tlaxcala (UTT)",
            "",
            "<b>Arquitectura y Motor Gráfico</b>",
            "- Motor de juego: Unity 6.3 LTS (Universal Render Pipeline 2D - URP)",
            "- Lenguaje de programación: C# (.NET)",
            "- Entorno de desarrollo (IDE): Visual Studio Community 2022",
            "- Control de versiones: Git & GitHub",
            "",
            "<b>Sistemas y Mecánicas Implementadas</b>",
            "- Diseño de niveles: Grid & Tilemap System con Tilemap Collider 2D y Composite Collider 2D",
            "- Físicas y movimiento: Rigidbody 2D, Raycast 2D para detección de superficie y Capsule Collider 2D",
            "- Sistema de animación: Animator Controller con máquina de estados y parámetros dinámicos",
            "- Inteligencia y eventos: Detección por áreas Trigger (Circle Collider 2D) para interacción con el Guardián",
            "- Cámara: Script dinámico de seguimiento continuo (CameraFollow)",
            "- Interfaz de usuario (UI): Unity Canvas, TextMeshPro y sistema interactivo de la Tablet Digital de Tlali (Diccionario Mágico)",
            "",
            "<b>Arte, Diseño y Metodología</b>",
            "- Diseño gráfico y animación: Pixel Art en Piskel (sprites, hojas de animación y tilesets)",
            "- Metodología de diseño: Player-Centered Design (PCD) y Teoría de la Autodeterminación (SDT)",
            "- Estándares aplicados: ISO 9241-210 (Diseño Centrado en el Usuario) e ISO/IEC 25010 (Calidad de Software)",
            "- Plataformas de compilación: Windows Standalone (.exe) y WebGL",
            "",
            "<b>Inspiración Cultural y Documentación Lingüística</b>",
            "- Municipio de Contla de Juan Cuamatzi, Tlaxcala — \"Cuna del Sarape\"",
            "- Tradición textil artesanal, telares de pedal y preservación de la variante local del náhuatl",
            "- Fuentes institucionales de referencia: INALI, INAH e INEGI",
            "",
            "<b>¡Gracias por jugar y contribuir a la preservación de la lengua náhuatl y el patrimonio textil de Tlaxcala!</b>"
        };

        StringBuilder sb = new StringBuilder();
        foreach (string linea in lineasCreditos)
        {
            sb.AppendLine(linea);
        }

        textoCreditosUI.text = sb.ToString();
    }

    public void IniciarSecuenciaVictoriaYCreditos()
    {
        if (rutinaPrincipal != null) StopCoroutine(rutinaPrincipal);
        rutinaPrincipal = StartCoroutine(SecuenciaCompleta());
    }

    private IEnumerator SecuenciaCompleta()
    {
        if (panelGameOver != null) panelGameOver.SetActive(false);
        if (panelVictoriaCreditos != null) panelVictoriaCreditos.SetActive(true);

        IniciarAudioFondoFinal();

        // Esperar un frame para que el panel y el Canvas actualicen su layout antes de capturar posición base
        yield return null;

        ConfigurarReferenciaPersonaje();
        CargarCreditosDesdeCodigo();
        animandoCelebracion = true;

        if (animatorCelebracion != null && !string.IsNullOrEmpty(triggerSaltoBrazos))
        {
            animatorCelebracion.SetTrigger(triggerSaltoBrazos);
        }

        if (rectContenedorCreditos != null)
        {
            Vector2 pos = rectContenedorCreditos.anchoredPosition;
            pos.y = posicionYInicial;
            rectContenedorCreditos.anchoredPosition = pos;

            while (rectContenedorCreditos.anchoredPosition.y < posicionYFinal)
            {
                pos = rectContenedorCreditos.anchoredPosition;
                pos.y += velocidadSubidaCreditos * Time.unscaledDeltaTime;
                rectContenedorCreditos.anchoredPosition = pos;
                yield return null;
            }
        }

        yield return new WaitForSecondsRealtime(pausaAlTerminarCreditos);

        animandoCelebracion = false;
        if (personajeCelebrando != null)
        {
            personajeCelebrando.localScale = escalaBase;
            if (rectCelebracion != null) rectCelebracion.anchoredPosition = posicionBaseUI;
        }

        // El audio sigue sonando durante Game Over y se cancela al dar clic en el botón de Menú de Inicio
        if (panelVictoriaCreditos != null) panelVictoriaCreditos.SetActive(false);
        if (panelGameOver != null) panelGameOver.SetActive(true);

        if (textoGraciasPorJugar != null)
        {
            textoGraciasPorJugar.text = "<b>GAME OVER</b>\n\n¡Gracias por jugar Tlaxcal-TL!";
        }
    }

    public void IrAlMenuInicio()
    {
        DetenerAudioFondoFinal();

        if (rutinaPrincipal != null)
        {
            StopCoroutine(rutinaPrincipal);
            rutinaPrincipal = null;
        }

        animandoCelebracion = false;
        Time.timeScale = 1f;

        if (string.IsNullOrEmpty(nombreEscenaMenu))
        {
            Debug.LogWarning("Aún no has configurado el nombre de la escena del Menú de Inicio.");
            return;
        }

        if (Application.CanStreamedLevelBeLoaded(nombreEscenaMenu))
        {
            SceneManager.LoadScene(nombreEscenaMenu);
        }
        else
        {
            Debug.LogWarning($"La escena '{nombreEscenaMenu}' aún no existe o no está agregada en Build Settings.");
        }
    }
}