using UnityEngine;
using TMPro;

public class TabletNivel2Controller : MonoBehaviour
{
    public static TabletNivel2Controller Instance;

    [Header("UI de la Tablet del Abecedario")]
    public GameObject panelTabletDiccionario;       // El panel general de la tablet
    public TextMeshProUGUI textoListaDiccionario;   // El texto con la lista ordenada del 1 al 17

    private bool tabletAbierta = false;

    // Las 17 grafías oficiales en su orden fijo (del 1 al 17)
    private string[] grafiasNahuatl = {
        "A", "CH", "E", "I", "K", "KW", "L", "M", "N", "O", "P", "S", "T", "TZ", "W", "X", "Y"
    };

    // Arreglo booleano de tamaño 18 para rastrear del 1 al 17
    private bool[] descubiertas = new bool[18];

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (panelTabletDiccionario != null) panelTabletDiccionario.SetActive(false);
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

    public void AlternarTablet()
    {
        tabletAbierta = !tabletAbierta;
        if (panelTabletDiccionario != null)
        {
            panelTabletDiccionario.SetActive(tabletAbierta);
        }
        Time.timeScale = tabletAbierta ? 0f : 1f; // Pausa el juego opcionalmente
    }

    // Método para desbloquear una posición fija del 1 al 17
    public void RegistrarLetraPorPosicion(int posicionFija)
    {
        if (posicionFija >= 1 && posicionFija <= 17)
        {
            descubiertas[posicionFija] = true;
            ActualizarTextoTablet();
        }
    }

    void ActualizarTextoTablet()
    {
        if (textoListaDiccionario == null) return;

        string contenido = "<b>ABECEDARIO NÁHUATL </b>\n\n";

        // Se dibuja la lista siempre ordenada del 1 al 17
        for (int i = 0; i < grafiasNahuatl.Length; i++)
        {
            int numeroPosicion = i + 1; // Posición del 1 al 17
            string grafia = grafiasNahuatl[i];

            if (descubiertas[numeroPosicion])
            {
                // Si ya fue descubierta, se muestra su número fijo y su grafía real
                contenido += $"[OK] {numeroPosicion}. <b>{grafia}</b>\n";
            }
            else
            {
                // Si aún no se encuentra, se muestra bloqueada en su lugar fijo
                contenido += $"[X] {numeroPosicion}. ???\n";
            }
        }

        textoListaDiccionario.text = contenido;
    }
}