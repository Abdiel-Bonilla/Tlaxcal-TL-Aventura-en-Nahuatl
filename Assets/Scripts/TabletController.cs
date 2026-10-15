using UnityEngine;
using TMPro;

public class TabletController : MonoBehaviour
{
    public static TabletController Instance;

    [Header("UI de la Tablet")]
    public GameObject panelTabletDiccionario;
    public TextMeshProUGUI textoListaDiccionario;

    private bool tabletAbierta = false;

    private string[] nombresNahuatl = {
        "Ce", "Ome", "Eyi", "Nahui",
        "Macuilli", "Chicuace", "Chicome", "Chicueyi", "Chicnahui", "Mahtlactli"
    };

    private bool[] descubiertos = new bool[11];

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (panelTabletDiccionario != null)
        {
            panelTabletDiccionario.SetActive(false);
        }
        ActualizarTextoTablet();
    }

    private void Update()
    {
        // Abrir o cerrar la tablet presionando la tecla Tab o E
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.E))
        {
            AlternarTablet();
        }
    }

    public void AlternarTablet()
    {
        tabletAbierta = !tabletAbierta;
        if (panelTabletDiccionario != null) // o panelTabletDiccionario != null
        {
            panelTabletDiccionario.SetActive(tabletAbierta);
        }

        // Pausar el juego mientras revisa la tablet
        Time.timeScale = tabletAbierta ? 0f : 1f;
    }

    public void RegistrarNumero(int numero)
    {
        if (numero >= 0 && numero < descubiertos.Length)
        {
            descubiertos[numero] = true;
            ActualizarTextoTablet();
        }
    }

    void ActualizarTextoTablet()
    {
        if (textoListaDiccionario == null) return;

        string contenido = "<b>DICCIONARIO MÁGICO (Náhuatl)</b>\n\n";

        for (int i = 0; i < nombresNahuatl.Length; i++)
        {
            int numeroMostrado = i + 1; // Inicia la numeración en 1

            if (descubiertos[i])
            {
                contenido += $"[OK] {numeroMostrado}. {NombresMayus(nombresNahuatl[i])}\n";
            }
            else
            {
                contenido += $"[X] {numeroMostrado}. ???\n";
            }
        }

        textoListaDiccionario.text = contenido;
    }

    string NombresMayus(string texto)
    {
        if (string.IsNullOrEmpty(texto)) return texto;
        return char.ToUpper(texto[0]) + texto.Substring(1);
    }
}