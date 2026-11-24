using UnityEngine;

public class PantallasIntroController : MonoBehaviour
{
    [Header("Paneles de Introducción")]
    public GameObject pantalla1Bienvenida;
    public GameObject pantalla2Controles;
    public GameObject pantalla3Nivel;

    [Header("Canvas del Juego Real (HUD y Niveles)")]
    public GameObject canvasJuegoHUD;      
    public GameObject canvasHilo;         

    private int pasoActual = 1;

    private void Start()
    {
       
        if (pantalla1Bienvenida != null) pantalla1Bienvenida.SetActive(true);
        if (pantalla2Controles != null) pantalla2Controles.SetActive(false);
        if (pantalla3Nivel != null) pantalla3Nivel.SetActive(false);

        
        if (canvasJuegoHUD != null) canvasJuegoHUD.SetActive(false);
        if (canvasHilo != null) canvasHilo.SetActive(false);

        
        Time.timeScale = 0f;
    }

    private void Update()
    {
        // Al presionar ENTER avanzamos entre pantallas o iniciamos el juego
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

           
            if (canvasJuegoHUD != null) canvasJuegoHUD.SetActive(true);
            if (canvasHilo != null) canvasHilo.SetActive(true);

           
            Time.timeScale = 1f;

           
            gameObject.SetActive(false);
        }
    }
}