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
    public Button[] botonesPlayPorColor;            // Arrastra aquí los 10 botones Play en orden (0 = color 1 ... 9 = color 10)
    public AudioClip[] audiosPronunciacion;         // Los 10 clips de audio en el mismo orden (0 = color 1 ... 9 = color 10)
    public bool ocultarBotonSiNoDescubierto = false; // Opcional: true para ocultarlo hasta desbloquear, false para solo deshabilitarlo

    [Header("Audio Source de la Tablet")]
    public AudioSource audioSourceTablet;

    private bool tabletAbierta = false;

    // Los 10 colores oficiales en náhuatl en su orden fijo (del 1 al 10)
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

    // Arreglo booleano de tamaño 11 para rastrear posiciones del 1 al 10
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

        // Permite escuchar el audio aunque el juego esté pausado al abrir la tablet
        audioSourceTablet.ignoreListenerPause = true;
        audioSourceTablet.playOnAwake = false;
    }

    private void Start()
    {
        if (panelTabletDiccionario != null) panelTabletDiccionario.SetActive(false);
        ConfigurarBotonesPlay();
        ActualizarTextoTablet();
    }

    private void Update()
    {
        // Abrir/Cerrar tablet con Tab o E
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
        if (posicionColor < 1 || posicionColor > 10 || !descubiertos[posicionColor]) return;

        if (audiosPronunciacion != null && indiceArray >= 0 && indiceArray < audiosPronunciacion.Length)
        {
            AudioClip clip = audiosPronunciacion[indiceArray];
            if (clip != null && audioSourceTablet != null)
            {
                audioSourceTablet.Stop();
                audioSourceTablet.clip = clip;
                audioSourceTablet.Play();
            }
        }
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

    // Método para desbloquear una posición fija del 1 al 10
    public void RegistrarColorPorPosicion(int posicionFija)
    {
        if (posicionFija >= 1 && posicionFija <= 10)
        {
            descubiertos[posicionFija] = true;
            ActualizarTextoTablet();
        }
    }

    // Compatibilidad con el nombre anterior
    public void RegistrarLetraPorPosicion(int posicionFija)
    {
        RegistrarColorPorPosicion(posicionFija);
    }

    void ActualizarTextoTablet()
    {
        string contenido = "<b>COLORES EN NÁHUATL</b>\n\n";

        for (int i = 0; i < coloresNahuatl.Length; i++)
        {
            int numeroPosicion = i + 1; // Posición del 1 al 10
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

            // Actualizar el estado del botón Play correspondiente a esta fila
            if (botonesPlayPorColor != null && i < botonesPlayPorColor.Length && botonesPlayPorColor[i] != null)
            {
                botonesPlayPorColor[i].interactable = estaDescubierto;

                if (ocultarBotonSiNoDescubierto)
                {
                    botonesPlayPorColor[i].gameObject.SetActive(estaDescubierto);
                }
            }
        }

        if (textoListaDiccionario != null)
        {
            textoListaDiccionario.text = contenido;
        }
    }
}