using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabletController : MonoBehaviour
{
    public static TabletController Instance;

    [Header("UI de la Tablet")]
    public GameObject panelTabletDiccionario;
    public TextMeshProUGUI textoListaDiccionario;

    [Header("Botones Play Enfrente de Cada Fila (1 al 10)")]
    public Button[] botonesPlayNumeros;          // 10 botones en orden: índice 0 = 1 (Ce) ... índice 9 = 10 (Mahtlactli)
    public AudioClip[] audiosNumerosNahuatl;     // 10 clips de audio en el mismo orden (0 a 9)
    public bool ocultarBotonSiNoDescubierto = false;

    [Header("Audio Source de la Tablet")]
    public AudioSource audioSourceTablet;

    private bool tabletAbierta = false;

    // Arreglo de palabras en náhuatl del 1 al 10
    private string[] nombresNahuatl = {
        "Ce", "Ome", "Eyi", "Nahui",
        "Macuilli", "Chicuace", "Chicome", "Chicueyi", "Chicnahui", "Mahtlactli"
    };

    // Arreglo del tamaño 11 para poder usar los índices directos del 1 al 10
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

        // Permite reproducir la pronunciación aunque la tablet pause el juego (Time.timeScale = 0)
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
        // Abrir o cerrar la tablet presionando la tecla Tab o E
        if (Input.GetKeyDown(KeyCode.E))
        {
            AlternarTablet();
        }
    }

    private void ConfigurarBotonesPlay()
    {
        if (botonesPlayNumeros == null) return;

        for (int i = 0; i < botonesPlayNumeros.Length; i++)
        {
            if (botonesPlayNumeros[i] == null) continue;

            int indiceAudio = i;
            int numeroReal = i + 1; // 1 al 10

            botonesPlayNumeros[i].onClick.RemoveAllListeners();
            botonesPlayNumeros[i].onClick.AddListener(() => ReproducirAudioNumero(indiceAudio, numeroReal));
        }
    }

    public void ReproducirAudioNumero(int indiceArray, int numero)
    {
        if (numero < 1 || numero > 10 || !descubiertos[numero]) return;

        if (audiosNumerosNahuatl != null && indiceArray >= 0 && indiceArray < audiosNumerosNahuatl.Length)
        {
            AudioClip clip = audiosNumerosNahuatl[indiceArray];
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

        // Pausar el juego mientras revisa la tablet
        Time.timeScale = tabletAbierta ? 0f : 1f;
    }

    public void RegistrarNumero(int numero)
    {
        if (numero >= 1 && numero <= 10)
        {
            descubiertos[numero] = true; // Guarda directamente en el índice del 1 al 10
            ActualizarTextoTablet();
        }
    }

    void ActualizarTextoTablet()
    {
        string contenido = "<b>DICCIONARIO MÁGICO (Náhuatl)</b>\n\n";

        // Iteramos del número 1 al 10
        for (int num = 1; num <= 10; num++)
        {
            int indiceArray = num - 1;
            string palabra = nombresNahuatl[indiceArray];
            bool estaDescubierto = descubiertos[num];

            if (estaDescubierto)
            {
                contenido += $"[OK] {num}. {NombresMayus(palabra)}\n";
            }
            else
            {
                contenido += $"[X] {num}. ???\n";
            }

            if (botonesPlayNumeros != null && indiceArray < botonesPlayNumeros.Length && botonesPlayNumeros[indiceArray] != null)
            {
                botonesPlayNumeros[indiceArray].interactable = estaDescubierto;

                if (ocultarBotonSiNoDescubierto)
                {
                    botonesPlayNumeros[indiceArray].gameObject.SetActive(estaDescubierto);
                }
            }
        }

        if (textoListaDiccionario != null)
        {
            textoListaDiccionario.text = contenido;
        }
    }

    string NombresMayus(string texto)
    {
        if (string.IsNullOrEmpty(texto)) return texto;
        return char.ToUpper(texto[0]) + texto.Substring(1);
    }
}