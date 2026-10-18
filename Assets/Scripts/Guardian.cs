using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GuardianRango : MonoBehaviour
{
    [Header("Referencias UI del Reto")]
    private Animator animator;
    public GameObject canvasPrueba;
    public GameObject panelConfirmacion;
    public GameObject panelPreguntaNahuatl;
    public TextMeshProUGUI textoPregunta;

    [Header("Botones de Opciones (UI)")]
    public Button botonOpcion1;
    public Button botonOpcion2;
    public Button botonOpcion3;

    [Header("Mecanismo de la Puerta y Guardián")]
    public SpriteRenderer spritePuerta;
    public Sprite spritePuertaAbierta;
    public Collider2D colliderPuerta;
    public SpriteRenderer spriteGuardian;

    [Header("Configuración del Reto")]
    public string tagDelJugador = "Player";
    public int carretesTotalesRequeridos = 10;

    [System.Serializable]
    public struct PreguntaNahuatl
    {
        public int numero;
        public string nombreNahuatl;
    }

    private List<PreguntaNahuatl> listaBasePreguntas = new List<PreguntaNahuatl>()
    {
        new PreguntaNahuatl { numero = 1, nombreNahuatl = "Ce" },
        new PreguntaNahuatl { numero = 2, nombreNahuatl = "Ome" },
        new PreguntaNahuatl { numero = 3, nombreNahuatl = "Eyi" },
        new PreguntaNahuatl { numero = 4, nombreNahuatl = "Nahui" },
        new PreguntaNahuatl { numero = 5, nombreNahuatl = "Macuilli" },
        new PreguntaNahuatl { numero = 6, nombreNahuatl = "Chicuace" },
        new PreguntaNahuatl { numero = 7, nombreNahuatl = "Chicome" },
        new PreguntaNahuatl { numero = 8, nombreNahuatl = "Chicueyi" },
        new PreguntaNahuatl { numero = 9, nombreNahuatl = "Chicnahui" },
        new PreguntaNahuatl { numero = 10, nombreNahuatl = "Mahtlactli" }
    };

    private List<PreguntaNahuatl> preguntasActualesRonda = new List<PreguntaNahuatl>();
    private int rondaActual = 1;
    private PreguntaNahuatl preguntaEnJuego;
    private bool retoCompletado = false;
    private bool procesandoRespuesta = false;

    private void Start()
    {
        animator = GetComponentInParent<Animator>();

        if (animator == null)
        {
            Debug.LogError("No se encontró ningún componente Animator en los objetos padres.", this);
        }

        if (canvasPrueba != null) canvasPrueba.SetActive(false);
        if (panelPreguntaNahuatl != null) panelPreguntaNahuatl.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(tagDelJugador) && !retoCompletado)
        {
            if (animator != null) animator.SetBool("Voltear", true);

            int carretesActuales = 0;
            if (GameManager.Instance != null)
            {
                carretesActuales = GameManager.Instance.GetCarretesRecolectados();
            }

            if (carretesActuales < carretesTotalesRequeridos)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.MostrarMensajeEnPantalla($"Faltan carretes: {carretesActuales}/{carretesTotalesRequeridos}. ¡Busca todos!");
                }
            }
            else
            {
                if (canvasPrueba != null)
                {
                    canvasPrueba.SetActive(true);
                    if (panelConfirmacion != null) panelConfirmacion.SetActive(true);
                    if (panelPreguntaNahuatl != null) panelPreguntaNahuatl.SetActive(false);

                    if (textoPregunta != null)
                    {
                        textoPregunta.text = "<b>¡Listo para la Prueba!</b>\n\n¿Estás preparado para demostrar tu conocimiento ante el Guardián?";
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(tagDelJugador) && !retoCompletado)
        {
            if (animator != null) animator.SetBool("Voltear", false);

            if (canvasPrueba != null) canvasPrueba.SetActive(false);
        }
    }

    public void BotonSi_AceptarPrueba()
    {
        if (panelConfirmacion != null) panelConfirmacion.SetActive(false);
        if (panelPreguntaNahuatl != null) panelPreguntaNahuatl.SetActive(true);

        rondaActual = 1;
        procesandoRespuesta = false;
        PrepararPreguntasAleatorias();
        SiguientePregunta();
    }

    public void BotonNo_CancelarPrueba()
    {
        if (canvasPrueba != null) canvasPrueba.SetActive(false);
        if (animator != null) animator.SetBool("Voltear", false);
    }

    void PrepararPreguntasAleatorias()
    {
        List<PreguntaNahuatl> copia = new List<PreguntaNahuatl>(listaBasePreguntas);
        preguntasActualesRonda.Clear();

        for (int i = 0; i < 3; i++)
        {
            int indexAleatorio = Random.Range(0, copia.Count);
            preguntasActualesRonda.Add(copia[indexAleatorio]);
            copia.RemoveAt(indexAleatorio);
        }
    }

    void SiguientePregunta()
    {
        if (rondaActual <= 3)
        {
            SetBotonesVisibles(true);

            preguntaEnJuego = preguntasActualesRonda[rondaActual - 1];

            if (textoPregunta != null)
            {
                textoPregunta.text = $"<b>Ronda {rondaActual} de 3</b>\n¿Cuál es el nombre en náhuatl del número <b>{preguntaEnJuego.numero}</b>?";
            }

            ConfigurarBotonesRespuestas();
        }
    }

    void ConfigurarBotonesRespuestas()
    {
        List<PreguntaNahuatl> opciones = new List<PreguntaNahuatl>();
        opciones.Add(preguntaEnJuego);

        List<PreguntaNahuatl> posiblesDistracciones = new List<PreguntaNahuatl>(listaBasePreguntas);
        posiblesDistracciones.RemoveAll(p => p.numero == preguntaEnJuego.numero);

        for (int i = 0; i < 2; i++)
        {
            int rndIndex = Random.Range(0, posiblesDistracciones.Count);
            opciones.Add(posiblesDistracciones[rndIndex]);
            posiblesDistracciones.RemoveAt(rndIndex);
        }

        for (int i = 0; i < opciones.Count; i++)
        {
            PreguntaNahuatl temp = opciones[i];
            int randomIndex = Random.Range(i, opciones.Count);
            opciones[i] = opciones[randomIndex];
            opciones[randomIndex] = temp;
        }

        AsignarBoton(botonOpcion1, opciones[0]);
        AsignarBoton(botonOpcion2, opciones[1]);
        AsignarBoton(botonOpcion3, opciones[2]);
    }

    void AsignarBoton(Button boton, PreguntaNahuatl opcion)
    {
        if (boton == null) return;

        TextMeshProUGUI textoBoton = boton.GetComponentInChildren<TextMeshProUGUI>();
        if (textoBoton != null)
        {
            textoBoton.text = $"{opcion.nombreNahuatl} ({opcion.numero})";
        }

        int valorOpcion = opcion.numero;

        boton.onClick.RemoveAllListeners();
        boton.onClick.AddListener(() =>
        {
            if (!procesandoRespuesta)
            {
                ValidarRespuesta(valorOpcion);
            }
        });
    }

    public void ValidarRespuesta(int numeroSeleccionado)
    {
        if (numeroSeleccionado == preguntaEnJuego.numero)
        {
            rondaActual++;

            if (rondaActual > 3)
            {
                retoCompletado = true;
                StartCoroutine(SecuenciaVictoriaEpica());
            }
            else
            {
                SiguientePregunta();
            }
        }
        else
        {
            StartCoroutine(SecuenciaError());
        }
    }

    IEnumerator SecuenciaError()
    {
        procesandoRespuesta = true;
        SetBotonesVisibles(false);

        if (textoPregunta != null)
        {
            textoPregunta.text = "<b>[Respuesta Incorrecta]</b>\n\nEl Guardián te pide reiniciar el reto desde la Ronda 1...";
        }

        yield return new WaitForSeconds(2f);

        rondaActual = 1;
        PrepararPreguntasAleatorias();
        procesandoRespuesta = false;
        SiguientePregunta();
    }

    IEnumerator SecuenciaVictoriaEpica()
    {
        procesandoRespuesta = true;
        SetBotonesVisibles(false);

        if (textoPregunta != null)
        {
            textoPregunta.text = "<b>¡PRUEBA DEL GUARDIÁN SUPERADA!</b>\n\n<i>\"Has demostrado sabiduría en los números sagrados... El camino está abierto.\"</i>";
        }

        yield return new WaitForSeconds(3.5f);

        if (spritePuerta != null && spritePuertaAbierta != null)
        {
            spritePuerta.sprite = spritePuertaAbierta;
        }
        if (colliderPuerta != null)
        {
            colliderPuerta.enabled = false;
        }

        if (spriteGuardian != null)
        {
            float tiempo = 1.5f;
            float transcurrido = 0f;
            Color colorOriginal = spriteGuardian.color;

            while (transcurrido < tiempo)
            {
                transcurrido += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, transcurrido / tiempo);
                spriteGuardian.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, alpha);
                yield return null;
            }
        }

        if (canvasPrueba != null)
        {
            canvasPrueba.SetActive(false);
        }

        gameObject.SetActive(false);
    }

    void SetBotonesVisibles(bool estado)
    {
        if (botonOpcion1 != null) botonOpcion1.gameObject.SetActive(estado);
        if (botonOpcion2 != null) botonOpcion2.gameObject.SetActive(estado);
        if (botonOpcion3 != null) botonOpcion3.gameObject.SetActive(estado);
    }
}