using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class GuardianNivel2 : MonoBehaviour
{
    [Header("Meta de Colores")]
    public int totalColoresNecesarios = 10;

    [Header("UI del Guardián (Paneles)")]
    public GameObject panelDialogoPrincipal;
    public GameObject panelPreguntaGuardian;
    public TextMeshProUGUI textoDialogoUI;

    [Header("Botones de Inicio (Sí / No)")]
    public Button botonSi;
    public Button botonNo;

    [Header("UI de Trivia (3 Rondas)")]
    public TextMeshProUGUI textoPreguntaUI;
    public Button[] botonesOpciones;
    public TextMeshProUGUI[] textosBotonesUI;

    [Header("Indicador y Entorno (Puerta)")]
    public GameObject indicadorPulsaQUI;
    public GameObject objetoPuertaCerrada;
    public GameObject objetoPuertaAbierta;
    public Collider2D colisionadorPuerta;

    private bool playerInRange = false;
    private bool pruebaSuperada = false;
    private int rondaActual = 0;
    private bool esperandoEnterParaCerrar = false;

    private struct Pregunta
    {
        public string enunciado;
        public string[] opciones;
        public int indiceCorrecta;
    }

    private Pregunta[] bancoPreguntas = new Pregunta[]
    {
        new Pregunta { enunciado = "¿Cómo se dice 'Rojo' en náhuatl?", opciones = new string[] { "Chichiltik", "Kostik", "Chipauak" }, indiceCorrecta = 0 },
        new Pregunta { enunciado = "¿Cuál es el nombre náhuatl para el color 'Amarillo'?", opciones = new string[] { "Xoxouik", "Kostik", "Tlikostik" }, indiceCorrecta = 1 },
        new Pregunta { enunciado = "¿Qué color en náhuatl corresponde al 'Verde'?", opciones = new string[] { "Atenextik", "Kamojti", "Xoxouik" }, indiceCorrecta = 2 },
        new Pregunta { enunciado = "¿Cómo se llama el color 'Azul' en náhuatl?", opciones = new string[] { "Atenextik", "Chilkostik", "Yayauik" }, indiceCorrecta = 0 },
        new Pregunta { enunciado = "¿Cuál es la palabra en náhuatl para 'Morado'?", opciones = new string[] { "Tlastaloltik", "Kamojti", "Chipauak" }, indiceCorrecta = 1 },
        new Pregunta { enunciado = "¿Cómo se dice 'Naranja' en náhuatl?", opciones = new string[] { "Chichiltik", "Tlikostik", "Chilkostik" }, indiceCorrecta = 2 },
        new Pregunta { enunciado = "¿Qué color representa 'Tlastaloltik'?", opciones = new string[] { "Rosa", "Blanco", "Café" }, indiceCorrecta = 0 },
        new Pregunta { enunciado = "¿Cuál es el color en náhuatl para 'Café'?", opciones = new string[] { "Yayauik", "Tlikostik", "Kostik" }, indiceCorrecta = 1 },
        new Pregunta { enunciado = "¿Cómo se dice 'Blanco' en náhuatl?", opciones = new string[] { "Atenextik", "Kamojti", "Chipauak" }, indiceCorrecta = 2 },
        new Pregunta { enunciado = "¿A qué color corresponde 'Yayauik'?", opciones = new string[] { "Gris / Oscuro", "Rojo", "Verde" }, indiceCorrecta = 0 }
    }; //[cite: 4]

    private int preguntaActualIndex = 0;
    private List<int> preguntasUsadasEnIntento = new List<int>();

    void Start()
    {
        if (panelDialogoPrincipal != null) panelDialogoPrincipal.SetActive(false);
        if (panelPreguntaGuardian != null) panelPreguntaGuardian.SetActive(false);
        if (indicadorPulsaQUI != null) indicadorPulsaQUI.SetActive(false);

        if (objetoPuertaCerrada != null) objetoPuertaCerrada.SetActive(true);
        if (objetoPuertaAbierta != null) objetoPuertaAbierta.SetActive(false);

        if (botonSi != null) botonSi.onClick.AddListener(OnClicSi);
        if (botonNo != null) botonNo.onClick.AddListener(OnClicNo);
    }

    void Update()
    {
        if (esperandoEnterParaCerrar && Input.GetKeyDown(KeyCode.Return))
        {
            CerrarTodo();
            return;
        }

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
            Time.timeScale = 0f;
            esperandoEnterParaCerrar = false;

            if (textoDialogoUI != null)
            {
                textoDialogoUI.text = "<b>Guardián del Conocimiento:</b>\n\n¿Estás listo para demostrar que dominas los colores en náhuatl del taller?";
            }

            if (botonSi != null) botonSi.gameObject.SetActive(true);
            if (botonNo != null) botonNo.gameObject.SetActive(true);
        }
    }

    void OnClicSi()
    {
        int coloresActuales = 0;
        if (GameManagerNivel2.Instance != null)
        {
            coloresActuales = GameManagerNivel2.Instance.GetColoresRecolectados();
        }

        if (coloresActuales < totalColoresNecesarios)
        {
            if (textoDialogoUI != null)
            {
                textoDialogoUI.text = $"<b>Guardián del Conocimiento:</b>\n\n¡Aún no tienes todos los colores! Te falta explorar más en el taller ({coloresActuales}/{totalColoresNecesarios}).\n\n[Presiona Enter para continuar]";

                if (botonSi != null) botonSi.gameObject.SetActive(false);
                if (botonNo != null) botonNo.gameObject.SetActive(false);

                esperandoEnterParaCerrar = true;
            }
            return;
        }

        if (panelDialogoPrincipal != null) panelDialogoPrincipal.SetActive(false);
        rondaActual = 1;
        preguntasUsadasEnIntento.Clear();
        LanzarPreguntaRonda();
    }

    void OnClicNo()
    {
        if (textoDialogoUI != null)
        {
            textoDialogoUI.text = "<b>Guardián del Conocimiento:</b>\n\nVe a escuchar y practicar los colores en tu tableta. Te esperaré aquí cuando estés listo.\n\n[Presiona Enter para continuar]";

            if (botonSi != null) botonSi.gameObject.SetActive(false);
            if (botonNo != null) botonNo.gameObject.SetActive(false);

            esperandoEnterParaCerrar = true;
        }
    }

    void CerrarTodo()
    {
        esperandoEnterParaCerrar = false;
        Time.timeScale = 1f;
        if (panelDialogoPrincipal != null) panelDialogoPrincipal.SetActive(false);
        if (panelPreguntaGuardian != null) panelPreguntaGuardian.SetActive(false);
    }

    void LanzarPreguntaRonda()
    {
        if (panelPreguntaGuardian != null)
        {
            panelPreguntaGuardian.SetActive(true);

            do
            {
                preguntaActualIndex = Random.Range(0, bancoPreguntas.Length);
            }
            while (preguntasUsadasEnIntento.Contains(preguntaActualIndex));

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
            rondaActual++;

            if (rondaActual > 3)
            {
                SuperarPruebaFinal();
            }
            else
            {
                LanzarPreguntaRonda();
            }
        }
        else
        {
            rondaActual = 1;
            preguntasUsadasEnIntento.Clear();

            if (textoPreguntaUI != null)
            {
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
        Time.timeScale = 1f;

        if (panelPreguntaGuardian != null) panelPreguntaGuardian.SetActive(false);

        if (objetoPuertaCerrada != null) objetoPuertaCerrada.SetActive(false);
        if (objetoPuertaAbierta != null) objetoPuertaAbierta.SetActive(true);

        if (colisionadorPuerta != null)
        {
            colisionadorPuerta.enabled = false;
        }

        if (GameManagerNivel2.Instance != null)
        {
            GameManagerNivel2.Instance.MostrarMensajeEnPantalla("¡Has superado la Prueba del Guardián!\n¡La puerta del taller se ha abierto!");
        }

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