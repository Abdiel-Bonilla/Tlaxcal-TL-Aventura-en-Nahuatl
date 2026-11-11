using UnityEngine;
using TMPro;
using System.Collections;

public class TransicionNivel3 : MonoBehaviour
{
    [Header("Referencias para la Transición")]
    public GameObject panelTransicionNegro;     // Tu PanelTransicion en el Canvas
    public TextMeshProUGUI textoTransicionUI;   // El texto dentro del panel negro
    public SpriteRenderer spriteTlali;          // El SpriteRenderer de Tlali
    public string tagDelJugador = "Player";

    private bool transicionIniciada = false;

    private void Start()
    {
        // Asegurarnos de que el panel negro empiece apagado
        if (panelTransicionNegro != null)
        {
            panelTransicionNegro.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si Tlali toca la puerta y la transición no ha empezado
        if (collision.CompareTag(tagDelJugador) && !transicionIniciada)
        {
            transicionIniciada = true;
            StartCoroutine(EfectoTransicionSuave(collision.gameObject));
        }
    }

    IEnumerator EfectoTransicionSuave(GameObject jugador)
    {
        // 1. Desactivar el movimiento de Tlali
        PlayerTopDown mov = jugador.GetComponent<PlayerTopDown>();
        if (mov != null)
        {
            mov.enabled = false;
        }

        // 2. Desvanecer a Tlali poco a poco (Efecto Fade Out)
        if (spriteTlali != null)
        {
            float tiempo = 1.0f;
            float transcurrido = 0f;
            Color colorOriginal = spriteTlali.color;

            while (transcurrido < tiempo)
            {
                transcurrido += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, transcurrido / tiempo);
                spriteTlali.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, alpha);
                yield return null;
            }
        }

        // 3. Poner el texto de felicitación en la pantalla negra
        if (textoTransicionUI != null)
        {
            textoTransicionUI.text = "¡Nivel 2 Superado!\nAvanzando al Nivel 3___ En Desarrollo...";
        }

        // 4. Activar la pantalla negra
        if (panelTransicionNegro != null)
        {
            panelTransicionNegro.SetActive(true);
        }

        // 5. Esperar 3 segundos
        yield return new WaitForSeconds(3.0f);

        // 6. Cargar el Nivel 3
        UnityEngine.SceneManagement.SceneManager.LoadScene("Nivel3");
    }
}