using UnityEngine;
using TMPro;

public class TabletController : MonoBehaviour
{
    public static TabletController Instance;

    [Header("UI de la Tablet")]
    public GameObject panelTabletDiccionario;
    public TextMeshProUGUI textoListaDiccionario;

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
        if (panelTabletDiccionario != null)
        {
            panelTabletDiccionario.SetActive(tabletAbierta);
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
        if (textoListaDiccionario == null) return;

        string contenido = "<b>DICCIONARIO MÁGICO (Náhuatl)</b>\n\n";

        // Iteramos del número 1 al 10
        for (int num = 1; num <= 10; num++)
        {
            // El índice en nombresNahuatl va de 0 a 9 (num - 1)
            string palabra = nombresNahuatl[num - 1];

            if (descubiertos[num])
            {
                contenido += $"[OK] {num}. {NombresMayus(palabra)}\n";
            }
            else
            {
                contenido += $"[X] {num}. ???\n";
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