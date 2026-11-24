using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

public class GuardianNivel3 : MonoBehaviour
{
    [Header("Meta de Palabras Cotidianas")]
    public int totalPalabrasNecesarias = 8;

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

    [Header("Indicador y Entorno (Puerta Opcional)")]
    public GameObject indicadorPulsaQUI;
    public GameObject objetoPuertaCerrada;
    public GameObject objetoPuertaAbierta;
    public Collider2D colisionadorPuerta;

    [Header("Evento Especial al Superar el Nivel 3")]
    public UnityEvent alGanarPruebaFinal;

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
        new Pregunta { enunciado = "¿Cómo se dice 'Luna' en náhuatl?", opciones = new string[] { "Metztli", "Xochitl", "Atl" }, indiceCorrecta = 0 },
        new Pregunta { enunciado = "¿Cuál es la palabra en náhuatl para 'Flor'?", opciones = new string[] { "Miztli", "Xochitl", "Tlaxcalli" }, indiceCorrecta = 1 },
        new Pregunta { enunciado = "¿Cómo se dice 'Gato' en náhuatl?", opciones = new string[] { "Itzcuintli", "Cihuatl", "Miztli" }, indiceCorrecta = 2 },
        new Pregunta { enunciado = "¿Qué palabra corresponde a 'Agua' en náhuatl?", opciones = new string[] { "Atl", "Oquichtli", "Metztli" }, indiceCorrecta = 0 },
        new Pregunta { enunciado = "¿Cómo se dice 'Perro' en náhuatl?", opciones = new string[] { "Xochitl", "Itzcuintli", "Atl" }, indiceCorrecta = 1 },
        new Pregunta { enunciado = "¿Cuál es el nombre en náhuatl de 'Tortilla'?", opciones = new string[] { "Cihuatl", "Miztli", "Tlaxcalli" }, indiceCorrecta = 2 },
        new Pregunta { enunciado = "¿Cómo se dice 'Mujer' en náhuatl?", opciones = new string[] { "Cihuatl", "Oquichtli", "Itzcuintli" }, indiceCorrecta = 0 },
        new Pregunta { enunciado = "¿Qué palabra en náhuatl significa 'Hombre'?", opciones = new string[] { "Tlaxcalli", "Oquichtli", "Metztli" }, indiceCorrecta = 1 }
    };

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
                textoDialogoUI.text = "<b>Guardián de la Galería:</b>\n\n¿Estás listo para demostrar que dominas las 8 palabras cotidianas en náhuatl?";
            }

            if (botonSi != null) botonSi.gameObject.SetActive(true);
            if (botonNo != null) botonNo.gameObject.SetActive(true);
        }
    }

    void OnClicSi()
    {
        int palabrasActuales = 0;
        if (GameManagerNivel3.Instance != null)
        {
            palabrasActuales = GameManagerNivel3.Instance.GetPalabrasRecolectadas();
        }

        if (palabrasActuales < totalPalabrasNecesarias)
        {
            if (textoDialogoUI != null)
            {
                textoDialogoUI.text = $"<b>Guardián de la Galería:</b>\n\n¡Aún no tienes todas las palabras! Habla con los NPCs de la tienda ({palabrasActuales}/{totalPalabrasNecesarias}).\n\n[Presiona Enter para continuar]";

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
            textoDialogoUI.text = "<b>Guardián de la Galería:</b>\n\nPuedes repasar las palabras y escuchar su pronunciación en tu tableta con E. Te esperaré aquí.\n\n[Presiona Enter para continuar]";

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

        if (GameManagerNivel3.Instance != null)
        {
            GameManagerNivel3.Instance.MostrarMensajeEnPantalla("¡Has superado la Prueba Final de la Galería!");
        }

        // Ejecuta el evento especial configurado en el Inspector al ganar
        alGanarPruebaFinal?.Invoke();

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