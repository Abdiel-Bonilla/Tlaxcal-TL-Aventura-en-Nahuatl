using UnityEngine;
using TMPro; // Necesario para manejar textos de TextMeshPro

public class PantallasIntroNivel2 : MonoBehaviour
{
    [Header("Paneles de Introducción del Nivel 2")]
    public GameObject pantalla1Bienvenida;
    public GameObject pantalla2Controles;
    public GameObject pantalla3Nivel;

    [Header("Textos de las Pantallas")]
    public TextMeshProUGUI textoBienvenida;
    public TextMeshProUGUI textoControles;
    public TextMeshProUGUI textoNivel3;

    [Header("Canvas del Nivel 2 Real (HUD y Herramientas)")]
    public GameObject canvasJuegoHUD;
    public GameObject canvasTapetes;

    private int pasoActual = 1;

    private void Start()
    {
        // Asignar los textos automáticamente al iniciar
        if (textoBienvenida != null)
        {
            textoBienvenida.text = "<b>¡Bienvenido al Nivel 2 del Taller Textil!</b>\n\n" +
                                   "Has llegado al corazón de Contla para descubrir los secretos del abecedario náhuatl.\n\n" +
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

        if (textoNivel3 != null)
        {
            textoNivel3.text = "<b>¡Reto Nivel 2!</b>\n\n" +
                               "Explora el taller, recolecta las letras escondidas en los telares y prepárate para superar la prueba del Guardián del Conocimiento.\n\n" +
                               "[ Presiona ENTER para comenzar el juego ]";
        }

        // Mostrar la primera pantalla y apagar el resto al iniciar la escena
        if (pantalla1Bienvenida != null) pantalla1Bienvenida.SetActive(true);
        if (pantalla2Controles != null) pantalla2Controles.SetActive(false);
        if (pantalla3Nivel != null) pantalla3Nivel.SetActive(false);

        // Ocultar el juego principal mientras pasa la intro
        if (canvasJuegoHUD != null) canvasJuegoHUD.SetActive(false);
        if (canvasTapetes != null) canvasTapetes.SetActive(false);

        // Congelar el tiempo para que Tlali no se mueva durante la intro
        Time.timeScale = 0f;
    }

    private void Update()
    {
        // Al presionar ENTER avanzamos entre pantallas o iniciamos el nivel
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
            // Ocultar la última pantalla de intro
            if (pantalla3Nivel != null) pantalla3Nivel.SetActive(false);

            // Activar los elementos reales del Nivel 2
            if (canvasJuegoHUD != null) canvasJuegoHUD.SetActive(true);
            if (canvasTapetes != null) canvasTapetes.SetActive(true);

            // Reanudar el tiempo del juego
            Time.timeScale = 1f;

            // Desactivar este script para que no siga consumiendo recursos
            gameObject.SetActive(false);
        }
    }
}