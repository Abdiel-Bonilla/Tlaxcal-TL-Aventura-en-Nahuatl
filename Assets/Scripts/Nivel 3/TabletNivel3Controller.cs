using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabletNivel3Controller : MonoBehaviour
{
    public static TabletNivel3Controller Instance;

    [Header("UI de la Tablet del Nivel 3")]
    public GameObject panelTabletDiccionario;
    public TextMeshProUGUI textoListaDiccionario;

    [Header("Botones Play Enfrente de Cada Fila (1 al 8)")]
    public Button[] botonesPlayPalabras;
    public AudioClip[] audiosPronunciacion;
    public bool ocultarBotonSiNoDescubierto = false;

    [Header("Audio Source de la Tablet")]
    public AudioSource audioSourceTablet;

    // Las 8 palabras cotidianas en español en orden (1 al 8)
    private string[] palabrasEspanol = {
        "Luna",
        "Flor",
        "Gato",
        "Agua",
        "Perro",
        "Tortilla",
        "Mujer",
        "Hombre"
    };

    // Palabras en náhuatl correspondientes (puedes ajustarlas en el código o mantenerlas aquí)
    private string[] palabrasNahuatl = {
        "Metztli",
        "Xochitl",
        "Miztli",
        "Atl",
        "Itzcuintli",
        "Tlaxcalli",
        "Cihuatl",
        "Oquichtli"
    };

    private bool tabletAbierta = false;
    private bool[] descubiertos = new bool[9]; // Índices 1..8

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
    }

    private void Start()
    {
        if (panelTabletDiccionario != null)
        {
            panelTabletDiccionario.SetActive(false);
        }

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
        if (botonesPlayPalabras == null) return;

        for (int i = 0; i < botonesPlayPalabras.Length; i++)
        {
            if (botonesPlayPalabras[i] == null) continue;

            int indiceAudio = i;
            int posicionPalabra = i + 1;

            botonesPlayPalabras[i].onClick.RemoveAllListeners();
            botonesPlayPalabras[i].onClick.AddListener(() => ReproducirAudioPalabra(indiceAudio, posicionPalabra));
        }
    }

    public void ReproducirAudioPalabra(int indiceArray, int posicionPalabra)
    {
        if (posicionPalabra < 1 || posicionPalabra > 8 || !descubiertos[posicionPalabra]) return;

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

    public void RegistrarPalabraPorPosicion(int posicionFija)
    {
        if (posicionFija >= 1 && posicionFija <= 8)
        {
            descubiertos[posicionFija] = true;
            ActualizarTextoTablet();
        }
    }

    void ActualizarTextoTablet()
    {
        string contenido = "<b>GALERÍA DE PALABRAS </b>\n\n";

        for (int i = 0; i < palabrasEspanol.Length; i++)
        {
            int numeroPosicion = i + 1;
            string esp = palabrasEspanol[i];
            string nah = palabrasNahuatl[i];
            bool estaDescubierto = descubiertos[numeroPosicion];

            if (estaDescubierto)
            {
                contenido += $"[OK] {numeroPosicion}. <b>{esp}</b> — <i>{nah}</i>\n";
            }
            else
            {
                contenido += $"[X] {numeroPosicion}. ???\n";
            }

            if (botonesPlayPalabras != null && i < botonesPlayPalabras.Length && botonesPlayPalabras[i] != null)
            {
                botonesPlayPalabras[i].interactable = estaDescubierto;

                if (ocultarBotonSiNoDescubierto)
                {
                    botonesPlayPalabras[i].gameObject.SetActive(estaDescubierto);
                }
            }
        }

        if (textoListaDiccionario != null)
        {
            textoListaDiccionario.text = contenido;
        }
    }
}