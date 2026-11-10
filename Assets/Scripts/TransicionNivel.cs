using UnityEngine;
using TMPro;
using System.Collections;

public class TransicionNivel : MonoBehaviour
{
    [Header("Referencias para la Transición")]
    public GameObject panelTransicionNegro; // Arrastra aquí tu PanelTransicion
    public SpriteRenderer spriteTlali;      // Arrastra aquí el SpriteRenderer de Tlali (el personaje)
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
        // Si Tlali toca este colider y la transición no ha empezado
        if (collision.CompareTag(tagDelJugador) && !transicionIniciada)
        {
            transicionIniciada = true;
            StartCoroutine(EfectoTransicionSuave(collision.gameObject));
        }
    }

    IEnumerator EfectoTransicionSuave(GameObject jugador)
    {
        // 1. Opcional: Desactivar el movimiento de Tlali para que no se mueva mientras se desvanece
        // (Si tiene un script de movimiento, puedes desactivarlo aquí)

        // 2. Desvanecer a Tlali poco a poco
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

        // 3. Activar la pantalla negra con el mensaje tierno
        if (panelTransicionNegro != null)
        {
            panelTransicionNegro.SetActive(true);
        }

        // 4. Esperar unos segundos en la pantalla negra (por ejemplo, 3 segundos)
        yield return new WaitForSeconds(3.0f);

        // AQUÍ ES DONDE EN EL FUTURO CAMBIARÁS DE ESCENA:
        UnityEngine.SceneManagement.SceneManager.LoadScene("Nivel2");

        Debug.Log("Transición completada. ¡Listo para el Nivel 2 cuando lo crees!");
    }
}