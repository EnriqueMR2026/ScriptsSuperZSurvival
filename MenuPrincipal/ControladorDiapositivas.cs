using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class ControladorDiapositivas : MonoBehaviour
{
    [Header("Configuración de Diapositivas")]
    public GameObject panelDiapositivas; // El panel contenedor de todo el tutorial
    public GameObject[] diapositivas; // Las imágenes/paneles que irán pasando
    private int indiceActual = 0;

    [Header("Transición de Despertar")]
    public CanvasGroup cortinaNegra; // Reutilizamos tu cortina negra
    public float velocidadTransicion = 2f;
    public TMP_Text textoDiaHora; // El texto que dirá "DÍA 1 - 9 AM"

    void Start()
    {
        // Nos aseguramos de que el texto de inicio esté apagado
        if (textoDiaHora != null) textoDiaHora.gameObject.SetActive(false);
    }

    // Esta función es para el botón JUGAR del tutorial
    public void IniciarTutorial()
    {
        panelDiapositivas.SetActive(true);
        indiceActual = 0;

        // Apagamos todas las diapositivas y encendemos solo la primera
        for (int i = 0; i < diapositivas.Length; i++)
        {
            diapositivas[i].SetActive(false);
        }
        
        if (diapositivas.Length > 0)
        {
            diapositivas[0].SetActive(true);
        }
    }

    // Esta función es para el botón SIGUIENTE de cada diapositiva
    public void SiguienteDiapositiva()
    {
        if (indiceActual < diapositivas.Length)
        {
            diapositivas[indiceActual].SetActive(false); // Apagamos la que estábamos viendo
        }

        indiceActual++;

        if (indiceActual < diapositivas.Length)
        {
            diapositivas[indiceActual].SetActive(true); // Mostramos la nueva
        }
        else
        {
            // Si ya no hay más, iniciamos la transición para despertar en el juego
            StartCoroutine(RutinaDespertar());
        }
    }

    IEnumerator RutinaDespertar()
    {
        // 1. Pantalla a negro totalmente
        cortinaNegra.blocksRaycasts = true;
        while (cortinaNegra.alpha < 1f)
        {
            cortinaNegra.alpha += Time.deltaTime * velocidadTransicion;
            yield return null;
        }

        // 2. Apagamos el panel de diapositivas para limpiar la pantalla
        panelDiapositivas.SetActive(false);

        // 3. Mostramos el texto épico de inicio
        if (textoDiaHora != null)
        {
            textoDiaHora.gameObject.SetActive(true);
            textoDiaHora.text = "DÍA 1\n09:00 AM - TABERNA";
            
            // Esperamos unos segundos para que el jugador lo lea con tensión
            yield return new WaitForSeconds(3f);
            
            textoDiaHora.gameObject.SetActive(false);
        }

        // 4. ¡Cargar la escena del Tutorial! (Aún con la pantalla en negro)
        // Asegúrate de que tu escena se llame exactamente "Tutorial"
        SceneManager.LoadScene("Tutorial");
    }
}