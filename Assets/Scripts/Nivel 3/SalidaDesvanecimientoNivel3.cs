using UnityEngine;
using System.Collections;

public class SalidaDesvanecimientoNivel3 : MonoBehaviour
{
    [Header("Referencias del Jugador y Animación")]
    public GameObject personaje;
    public Animator animatorPersonaje;
    public string triggerDesvanecimiento = "Desvanecer"; // Opcional: nombre de trigger en el Animator
    public float duracionDesvanecimiento = 2.2f;       // Un poco larga como pediste

    [Header("Transición y Pantalla de Victoria")]
    public GameObject panelTransicion;
    public float esperaExtraTransicion = 0.6f;
    public GameObject panelVictoriaCreditos;             // Si está en la misma escena
    public PantallaVictoriaCreditosNivel3 controladorVictoria;
    public string nombreEscenaVictoria = "";           // Déjalo vacío si usas panel en la misma escena

    private bool activado = false;

    private void Start()
    {
        if (panelTransicion != null) panelTransicion.SetActive(false);
        if (panelVictoriaCreditos != null) panelVictoriaCreditos.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activado) return;

        if (collision.CompareTag("Player"))
        {
            activado = true;
            if (personaje == null) personaje = collision.gameObject;
            if (animatorPersonaje == null && personaje != null)
            {
                animatorPersonaje = personaje.GetComponentInChildren<Animator>();
            }

            StartCoroutine(SecuenciaSalida());
        }
    }

    private IEnumerator SecuenciaSalida()
    {
        Time.timeScale = 1f;

        // Desactivar scripts de movimiento del jugador para que no camine mientras se desvanece
        if (personaje != null)
        {
            MonoBehaviour[] scriptsJugador = personaje.GetComponents<MonoBehaviour>();
            foreach (var s in scriptsJugador)
            {
                if (s != null) s.enabled = false;
            }

            if (animatorPersonaje != null && !string.IsNullOrEmpty(triggerDesvanecimiento))
            {
                animatorPersonaje.SetTrigger(triggerDesvanecimiento);
            }
        }

        // Desvanecimiento gradual por alpha en SpriteRenderers como respaldo visual seguro
        float tiempo = 0f;
        SpriteRenderer[] sprites = personaje != null
            ? personaje.GetComponentsInChildren<SpriteRenderer>()
            : new SpriteRenderer[0];

        Color[] coloresIniciales = new Color[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
        {
            coloresIniciales[i] = sprites[i].color;
        }

        while (tiempo < duracionDesvanecimiento)
        {
            tiempo += Time.deltaTime;
            float progreso = Mathf.Clamp01(tiempo / duracionDesvanecimiento);

            for (int i = 0; i < sprites.Length; i++)
            {
                if (sprites[i] != null)
                {
                    Color c = coloresIniciales[i];
                    c.a = Mathf.Lerp(coloresIniciales[i].a, 0f, progreso);
                    sprites[i].color = c;
                }
            }

            yield return null;
        }

        if (personaje != null)
        {
            personaje.SetActive(false);
        }

        // Activar transición
        if (panelTransicion != null)
        {
            panelTransicion.SetActive(true);
        }

        yield return new WaitForSeconds(esperaExtraTransicion);

        if (!string.IsNullOrEmpty(nombreEscenaVictoria))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nombreEscenaVictoria);
        }
        else
        {
            if (panelTransicion != null) panelTransicion.SetActive(false);
            if (panelVictoriaCreditos != null) panelVictoriaCreditos.SetActive(true);

            if (controladorVictoria != null)
            {
                controladorVictoria.IniciarSecuenciaVictoriaYCreditos();
            }
        }
    }
}