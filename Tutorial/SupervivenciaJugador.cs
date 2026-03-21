using UnityEngine;
using UnityEngine.UI;

public class SupervivenciaJugador : MonoBehaviour
{
    [Header("Valores Actuales")]
    public float saludActual = 100f;
    public float hambreActual = 100f;

    [Header("Configuracion")]
    public float hambrePorSegundo = 0.5f; // Cuánto hambre pierdes
    public float dañoPorHambre = 1.0f;    // Cuánta vida pierdes al estar en 0 hambre

    [Header("UI")]
    public Image barraSalud;
    public Image barraHambre;

    void Start()
    {
        saludActual = 100f;
        hambreActual = 100f;
    }

    void Update()
    {
        // El hambre baja con el tiempo
        if (hambreActual > 0)
        {
            hambreActual -= hambrePorSegundo * Time.deltaTime;
        }
        else
        {
            // Si no hay hambre, perdemos salud
            saludActual -= dañoPorHambre * Time.deltaTime;
        }

        // Evitamos que los valores se salgan de 0 a 100
        saludActual = Mathf.Clamp(saludActual, 0, 100);
        hambreActual = Mathf.Clamp(hambreActual, 0, 100);

        ActualizarUI();

        if (saludActual <= 0)
        {
            Debug.Log("Has muerto, mi rey.");
            // Aquí luego pondremos una pantalla de Game Over
        }
    }

    void ActualizarUI()
    {
        if (barraSalud != null) barraSalud.fillAmount = saludActual / 100f;
        if (barraHambre != null) barraHambre.fillAmount = hambreActual / 100f;
    }
}