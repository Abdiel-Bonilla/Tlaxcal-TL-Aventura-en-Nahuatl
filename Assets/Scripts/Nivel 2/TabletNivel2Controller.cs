using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabletNivel2Controller : MonoBehaviour
{
    public static TabletNivel2Controller Instance;

    [Header("UI de la Tablet de Colores (Tu diseño actual)")]
    public GameObject panelTabletDiccionario;       // El panel general de la tablet
    public TextMeshProUGUI textoListaDiccionario;   // Tu texto con la lista ordenada del 1 al 10

    [Header("Botones Play Enfrente de Cada Fila (1 al 10)")]
    public Button[] botonesPlayPorColor;            // 10 botones Play en orden (0 = color 1 ... 9 = color 10)
    public AudioClip[] audiosPronunciacion;         // 10 clips de audio en el mismo orden (0 = color 1 ... 9 = color 10)
    public bool ocultarBotonSiNoDescubierto = false; // true para ocultar hasta desbloquear, false para deshabilitar
    public bool permitirAudioSinDescubrir = false;  // Déjalo en false para juego normal; true solo para probar audios

    [Header("Audio Source de la Tablet")]
    public AudioSource audioSourceTablet;
    [Range(0f, 1f)]
    public float volumenAudioTablet = 1f;

    private bool tabletAbierta = false;

    private string[] coloresNahuatl = {
        "Chichiltik",
        "Chilkostik",
        "Kostik",
        "Xoxouik",
        "Atenextik",
        "Kamojti",
        "Xochipaltik",
        "Tlikostik",
        "Yayauik",
        "Chipauak"
    };

    private string[] coloresEspanol = {
        "Rojo",
        "Naranja",
        "Amarillo",
        "Verde",
        "Azul",
        "Morado",
        "Rosa",
        "Café",
        "Negro / Oscuro",
        "Blanco"
    };

    private bool[] descubiertos = new bool[11];

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (audioSourceTablet == null)
        {
            audioSourceTablet = GetComponent<AudioSource>();
        }

        if (audioSourceTablet == null)
        {
            audioSourceTablet = gameObject.AddComponent<AudioSource>();
        }

        audioSourceTablet.ignoreListenerPause = true;
        audioSourceTablet.playOnAwake = false;
        audioSourceTablet.mute = false;
        audioSourceTablet.volume = volumenAudioTablet;
    }

    private void Start()
    {
        if (panelTabletDiccionario != null) panelTabletDiccionario.SetActive(false);
        ConfigurarBotonesPlay();
        ActualizarTextoTablet();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.E))
        {
            AlternarTablet();
        }
    }

    private void ConfigurarBotonesPlay()
    {
        if (botonesPlayPorColor == null) return;

        for (int i = 0; i < botonesPlayPorColor.Length; i++)
        {
            if (botonesPlayPorColor[i] == null) continue;

            int indiceAudio = i;
            int posicionColor = i + 1;

            botonesPlayPorColor[i].onClick.RemoveAllListeners();
            botonesPlayPorColor[i].onClick.AddListener(() => ReproducirAudioColor(indiceAudio, posicionColor));
        }
    }

    public void ReproducirAudioColor(int indiceArray, int posicionColor)
    {
        if (posicionColor < 1 || posicionColor > 10) return;

        if (!permitirAudioSinDescubrir && !descubiertos[posicionColor])
        {
            Debug.LogWarning($"El color en posición {posicionColor} aún no está descubierto.");
            return;
        }

        if (audiosPronunciacion == null || indiceArray < 0 || indiceArray >= audiosPronunciacion.Length)
        {
            Debug.LogWarning($"No hay clip de audio asignado en el índice {indiceArray}.");
            return;
        }

        AudioClip clip = audiosPronunciacion[indiceArray];
        if (clip == null)
        {
            Debug.LogWarning($"El AudioClip en el índice {indiceArray} está vacío (None).");
            return;
        }

        if (audioSourceTablet == null)
        {
            Debug.LogWarning("No hay AudioSource asignado en TabletNivel2Controller.");
            return;
        }

        audioSourceTablet.mute = false;
        audioSourceTablet.volume = volumenAudioTablet;
        audioSourceTablet.Stop();
        audioSourceTablet.clip = clip;
        audioSourceTablet.Play();
    }

    public void AlternarTablet()
    {
        tabletAbierta = !tabletAbierta;
        if (panelTabletDiccionario != null)
        {
            panelTabletDiccionario.SetActive(tabletAbierta);
        }

        if (!tabletAbierta && audioSourceTablet != null && audioSourceTablet.isPlaying)
        {
            audioSourceTablet.Stop();
        }

        Time.timeScale = tabletAbierta ? 0f : 1f;
    }

    public void RegistrarColorPorPosicion(int posicionFija)
    {
        if (posicionFija >= 1 && posicionFija <= 10)
        {
            descubiertos[posicionFija] = true;
            ActualizarTextoTablet();
        }
    }

    public void RegistrarLetraPorPosicion(int posicionFija)
    {
        RegistrarColorPorPosicion(posicionFija);
    }

    void ActualizarTextoTablet()
    {
        string contenido = "<b>COLORES EN NÁHUATL</b>\n\n";

        for (int i = 0; i < coloresNahuatl.Length; i++)
        {
            int numeroPosicion = i + 1;
            string colorNah = coloresNahuatl[i];
            string colorEsp = coloresEspanol[i];
            bool estaDescubierto = descubiertos[numeroPosicion];

            if (estaDescubierto)
            {
                contenido += $"[OK] {numeroPosicion}. <b>{colorNah}</b> ({colorEsp})\n";
            }
            else
            {
                contenido += $"[X] {numeroPosicion}. ???\n";
            }

            if (botonesPlayPorColor != null && i < botonesPlayPorColor.Length && botonesPlayPorColor[i] != null)
            {
                bool habilitado = permitirAudioSinDescubrir || estaDescubierto;
                botonesPlayPorColor[i].interactable = habilitado;

                if (ocultarBotonSiNoDescubierto)
                {
                    botonesPlayPorColor[i].gameObject.SetActive(habilitado);
                }
            }
        }

        if (textoListaDiccionario != null)
        {
            textoListaDiccionario.text = contenido;
        }
    }
}