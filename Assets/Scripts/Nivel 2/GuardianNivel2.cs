using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic; // Necesario para usar listas (List<int>)

public class GuardianNivel2 : MonoBehaviour
{
    [Header("UI del Guardián (Paneles)")]
    public GameObject panelDialogoPrincipal;   // Panel para la pregunta inicial ("¿Estás listo?")
    public GameObject panelPreguntaGuardian;   // Panel para las rondas de trivia
    public TextMeshProUGUI textoDialogoUI;     // Texto para los mensajes del guardián

    [Header("Botones de Inicio (Sí / No)")]
    public Button botonSi;
    public Button botonNo;

    [Header("UI de Trivia (3 Rondas)")]
    public TextMeshProUGUI textoPreguntaUI;    // Texto de la pregunta de trivia
    public Button[] botonesOpciones;           // 3 Botones para las respuestas de la trivia
    public TextMeshProUGUI[] textosBotonesUI;  // Textos dentro de los 3 botones

    [Header("Indicador y Entorno (Puerta)")]
    public GameObject indicadorPulsaQUI;       // Aviso de "Pulsa Q"
    public GameObject objetoPuertaCerrada;     // Objeto hijo o sprite de la puerta cerrada
    public GameObject objetoPuertaAbierta;     // Objeto hijo o sprite de la puerta abierta
    public Collider2D colisionadorPuerta;      // El colisionador físico de la puerta para desactivarlo al abrir

    private bool playerInRange = false;
    private bool pruebaSuperada = false;
    private int rondaActual = 0;               // Control de las 3 rondas (1, 2, 3)
    private bool esperandoEnterParaCerrar = false; // Bandera para saber si estamos esperando la tecla Enter

    // Estructura de preguntas sobre las grafías náhuatl
    private struct Pregunta
    {
        public string enunciado;
        public string[] opciones;
        public int indiceCorrecta;
    }

    private Pregunta[] bancoPreguntas = new Pregunta[]
    {
        new Pregunta { enunciado = "¿Cuál es la grafía náhuatl correcta para el sonido africado 'tz'?", opciones = new string[] { "CH", "TZ", "KW" }, indiceCorrecta = 1 },
        new Pregunta { enunciado = "¿Qué grafía representa la posición 2 del abecedario de Contla?", opciones = new string[] { "CH", "A", "E" }, indiceCorrecta = 0 },
        new Pregunta { enunciado = "¿Cuál grafía se usa para el sonido labial con 'w' (como KW)?", opciones = new string[] { "W", "KW", "K" }, indiceCorrecta = 1 },
        new Pregunta { enunciado = "¿Qué letra sencilla abre el abecedario del taller en la posición 1?", opciones = new string[] { "E", "I", "A" }, indiceCorrecta = 2 },
        new Pregunta { enunciado = "¿Qué grafía se utiliza en la posición 6 del taller de Contla?", opciones = new string[] { "KW", "X", "TZ" }, indiceCorrecta = 0 },
        new Pregunta { enunciado = "¿Cuál es la grafía para la letra equis en este abecedario?", opciones = new string[] { "W", "X", "Y" }, indiceCorrecta = 1 }
    };

    private int preguntaActualIndex = 0;

    // Lista para llevar el registro de las preguntas ya usadas en las 3 rondas actuales
    private List<int> preguntasUsadasEnIntento = new List<int>();

    void Start()
    {
        if (panelDialogoPrincipal != null) panelDialogoPrincipal.SetActive(false);
        if (panelPreguntaGuardian != null) panelPreguntaGuardian.SetActive(false);
        if (indicadorPulsaQUI != null) indicadorPulsaQUI.SetActive(false);

        // Asegurar estado inicial de la puerta
        if (objetoPuertaCerrada != null) objetoPuertaCerrada.SetActive(true);
        if (objetoPuertaAbierta != null) objetoPuertaAbierta.SetActive(false);

        // Configurar los botones de Sí / No iniciales
        if (botonSi != null) botonSi.onClick.AddListener(OnClicSi);
        if (botonNo != null) botonNo.onClick.AddListener(OnClicNo);
    }

    void Update()
    {
        // Detectar si estamos esperando Enter para cerrar el diálogo de advertencia/despedida
        if (esperandoEnterParaCerrar && Input.GetKeyDown(KeyCode.Return))
        {
            CerrarTodo();
            return;
        }

        // Si el jugador está cerca, no ha superado la prueba y presiona Q
        if (playerInRange && !pruebaSuperada && Input.GetKeyDown(KeyCode.Q) && !esperandoEnterParaCerrar)
        {
            AbrirDialogoPrincipal();
        }
    }

    void AbrirDialogoPrincipal()
    {
        if (panelDialogoPrincipal != null)
        {
            if (indicadorPulsaQUI != null) indicadorPulsaQUI.SetActive(false);
            panelDialogoPrincipal.SetActive(true);
            Time.timeScale = 0f; // Pausar juego
            esperandoEnterParaCerrar = false;

            if (textoDialogoUI != null)
            {
                textoDialogoUI.text = "<b>Guardián del Conocimiento:</b>\n\n¿Estás listo para demostrar que dominas el abecedario náhuatl de Contla?";
            }

            // Asegurarnos de que los botones estén activos al abrir el diálogo
            if (botonSi != null) botonSi.gameObject.SetActive(true);
            if (botonNo != null) botonNo.gameObject.SetActive(true);
        }
    }

    void OnClicSi()
    {
        // Validar directamente con el GameManager si ya juntó las 17 letras
        int letrasActuales = 0;
        if (GameManagerNivel2.Instance != null)
        {
            letrasActuales = GameManagerNivel2.Instance.GetLetrasRecolectadas();
        }

        // Si no tiene las 17 letras, se lo negamos y pedimos presionar Enter para cerrar
        if (letrasActuales < 17)
        {
            if (textoDialogoUI != null)
            {
                textoDialogoUI.text = "<b>Guardián del Conocimiento:</b>\n\n¡Aún no tienes todas las letras! Te falta explorar más tapetes en el taller (" + letrasActuales + "/17).\n\n[Presiona Enter para continuar]";

                if (botonSi != null) botonSi.gameObject.SetActive(false);
                if (botonNo != null) botonNo.gameObject.SetActive(false);

                esperandoEnterParaCerrar = true; // Activamos la espera de la tecla Enter
            }
            return;
        }

        // Si sí tiene las 17 letras, cerramos este panel e iniciamos la Ronda 1 de 3
        if (panelDialogoPrincipal != null) panelDialogoPrincipal.SetActive(false);
        rondaActual = 1;

        // Limpiamos el historial al iniciar un nuevo intento de prueba
        preguntasUsadasEnIntento.Clear();
        LanzarPreguntaRonda();
    }

    void OnClicNo()
    {
        if (textoDialogoUI != null)
        {
            textoDialogoUI.text = "<b>Guardián del Conocimiento:</b>\n\nVe a estudiar tu tableta y repasa las grafías. Te esperaré aquí cuando estés listo.\n\n[Presiona Enter para continuar]";

            // Ocultar los botones de Sí / No
            if (botonSi != null) botonSi.gameObject.SetActive(false);
            if (botonNo != null) botonNo.gameObject.SetActive(false);

            esperandoEnterParaCerrar = true; // Activamos la espera de la tecla Enter para salir
        }
    }

    void CerrarTodo()
    {
        esperandoEnterParaCerrar = false;
        Time.timeScale = 1f; // Reanudar el tiempo del juego
        if (panelDialogoPrincipal != null) panelDialogoPrincipal.SetActive(false);
        if (panelPreguntaGuardian != null) panelPreguntaGuardian.SetActive(false);
    }

    void LanzarPreguntaRonda()
    {
        if (panelPreguntaGuardian != null)
        {
            panelPreguntaGuardian.SetActive(true);

            // Buscar una pregunta que NO haya salido en este intento actual
            do
            {
                preguntaActualIndex = Random.Range(0, bancoPreguntas.Length);
            }
            while (preguntasUsadasEnIntento.Contains(preguntaActualIndex));

            // Guardamos esta pregunta en la lista de usadas para que no se repita en las siguientes rondas
            preguntasUsadasEnIntento.Add(preguntaActualIndex);

            Pregunta p = bancoPreguntas[preguntaActualIndex];

            if (textoPreguntaUI != null)
            {
                textoPreguntaUI.text = $"<b>Prueba del Guardián (Ronda {rondaActual} de 3):</b>\n\n{p.enunciado}";
            }

            for (int i = 0; i < botonesOpciones.Length; i++)
            {
                int opcionIndex = i;
                textosBotonesUI[i].text = p.opciones[i];

                botonesOpciones[i].onClick.RemoveAllListeners();
                botonesOpciones[i].onClick.AddListener(() => ValidarRespuestaRonda(opcionIndex, p.indiceCorrecta));
            }
        }
    }

    void ValidarRespuestaRonda(int elegido, int correcta)
    {
        if (elegido == correcta)
        {
            // ¡Acierto en esta ronda!
            rondaActual++;

            if (rondaActual > 3)
            {
                // ¡Superó las 3 rondas con éxito!
                SuperarPruebaFinal();
            }
            else
            {
                // Pasar a la siguiente ronda asegurando una pregunta nueva
                LanzarPreguntaRonda();
            }
        }
        else
        {
            // ¡Se equivocó! Se reinician las rondas a 1 y se limpia el historial para que puedan salir de nuevo
            rondaActual = 1;
            preguntasUsadasEnIntento.Clear();

            if (textoPreguntaUI != null)
            {
                // Al fallar y reiniciar, lanzamos de una vez una nueva pregunta limpia
                preguntaActualIndex = Random.Range(0, bancoPreguntas.Length);
                preguntasUsadasEnIntento.Add(preguntaActualIndex);
                Pregunta p = bancoPreguntas[preguntaActualIndex];

                textoPreguntaUI.text = "<b>¡Respuesta incorrecta!</b>\nEl conocimiento falló. Las 3 rondas se reinician. ¡Vuelve a intentar!\n\n" + p.enunciado;

                for (int i = 0; i < botonesOpciones.Length; i++)
                {
                    int opcionIndex = i;
                    textosBotonesUI[i].text = p.opciones[i];

                    botonesOpciones[i].onClick.RemoveAllListeners();
                    botonesOpciones[i].onClick.AddListener(() => ValidarRespuestaRonda(opcionIndex, p.indiceCorrecta));
                }
            }
        }
    }

    void SuperarPruebaFinal()
    {
        pruebaSuperada = true;
        Time.timeScale = 1f; // Reanudar juego

        if (panelPreguntaGuardian != null) panelPreguntaGuardian.SetActive(false);

        // 1. Intercambiar los objetos de la puerta (apagar cerrada, encender abierta con su fondo oscuro)
        if (objetoPuertaCerrada != null) objetoPuertaCerrada.SetActive(false);
        if (objetoPuertaAbierta != null) objetoPuertaAbierta.SetActive(true);

        // 2. Desactivar el colisionador sólido de la puerta para permitir el paso
        if (colisionadorPuerta != null)
        {
            colisionadorPuerta.enabled = false;
        }

        // 3. Mostrar mensaje de victoria en el GameManager
        if (GameManagerNivel2.Instance != null)
        {
            GameManagerNivel2.Instance.MostrarMensajeEnPantalla("¡Has superado la Prueba del Guardián!\n¡La puerta del taller se ha abierto!");
        }

        Debug.Log("¡Prueba de 3 rondas superada! Puerta abierta y colisionador desactivado.");

        // 4. Desaparecer al Guardián
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !pruebaSuperada)
        {
            playerInRange = true;
            if (indicadorPulsaQUI != null) indicadorPulsaQUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            if (indicadorPulsaQUI != null) indicadorPulsaQUI.SetActive(false);
        }
    }
}