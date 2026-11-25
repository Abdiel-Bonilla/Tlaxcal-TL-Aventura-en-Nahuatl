using UnityEngine;
using TMPro;

public class PantallasIntroNivel3 : MonoBehaviour
{
    [Header("Paneles de Introducción del Nivel 3")]
    public GameObject pantalla1Bienvenida;
    public GameObject pantalla2Controles;
    public GameObject pantalla3Nivel;

    [Header("Textos de las Pantallas")]
    public TextMeshProUGUI textoBienvenida;
    public TextMeshProUGUI textoControles;
    public TextMeshProUGUI textoSobreNivel;

    [Header("Elementos del Nivel 3 Real (HUD y Herramientas)")]
    public GameObject canvasJuegoHUD;       // Tu Canvas o contenedor del HUD del Nivel 3
    public GameObject contenedorNPCs;       // Opcional: NPCs o elementos que quieras mostrar al iniciar

    private int pasoActual = 1;

    private void Start()
    {
        if (textoBienvenida != null)
        {
            textoBienvenida.text = "<b>¡Bienvenido al Nivel 3: Tienda de Exhibición y Galerías!</b>\n\n" +
                                   "Recorre la galería de artesanías y descubre las palabras cotidianas hablando con las personas.\n\n" +
                                   "[ Presiona ENTER para continuar ]";
        }

        if (textoControles != null)
        {
            textoControles.text = "<b>Controles: </b>\n\n" +
                                  "Moverse a los lados: \n" +
                                  "Moverse arriba y abajo: \n" +
                                  "Interactuar: \n" +
                                  "Abrir menú de la tablet: \n\n" +
                                  "[ Presiona ENTER para continuar ]";
        }

        if (textoSobreNivel != null)
        {
            textoSobreNivel.text = "<b>¡Reto Nivel 3!</b>\n\n" +
                                   "Habla con las personas de la tienda para aprender palabras cotidianas, regístralas en tu tableta y supera las 3 rondas del guardián del conocimiento.\n\n" +
                                   "[ Presiona ENTER para comenzar el juego ]";
        }

        // Mostrar solo la primera pantalla al iniciar
        if (pantalla1Bienvenida != null) pantalla1Bienvenida.SetActive(true);
        if (pantalla2Controles != null) pantalla2Controles.SetActive(false);
        if (pantalla3Nivel != null) pantalla3Nivel.SetActive(false);

        // Ocultar temporalmente el HUD o elementos indicados mientras corre la intro
        if (canvasJuegoHUD != null) canvasJuegoHUD.SetActive(false);
        if (contenedorNPCs != null) contenedorNPCs.SetActive(false);

        // Pausar el juego durante la introducción
        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            AvanzarPantalla();
        }
    }

    void AvanzarPantalla()
    {
        pasoActual++;

        if (pasoActual == 2)
        {
            if (pantalla1Bienvenida != null) pantalla1Bienvenida.SetActive(false);
            if (pantalla2Controles != null) pantalla2Controles.SetActive(true);
        }
        else if (pasoActual == 3)
        {
            if (pantalla2Controles != null) pantalla2Controles.SetActive(false);
            if (pantalla3Nivel != null) pantalla3Nivel.SetActive(true);
        }
        else if (pasoActual > 3)
        {
            if (pantalla3Nivel != null) pantalla3Nivel.SetActive(false);

            if (canvasJuegoHUD != null) canvasJuegoHUD.SetActive(true);
            if (contenedorNPCs != null) contenedorNPCs.SetActive(true);

            Time.timeScale = 1f;
            gameObject.SetActive(false);
        }
    }
}