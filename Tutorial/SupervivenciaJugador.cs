using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Header("Efectos de Daño")]
    public Image pantallaRoja; // Crea una imagen roja en tu Canvas, ponle transparencia 0, y arrástrala aquí
    public float velocidadDesvanecimientoDaño = 5f;
    public Color colorDaño = new Color(1f, 0f, 0f, 0.5f); // Qué tan intenso será el parpadeo rojo

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

            // ¡NUEVO! Latido rojo de alerta por inanición (hambre)
            if (pantallaRoja != null)
            {
                // Usamos matemáticas (Seno) para hacer un efecto de latido continuo
                float alfaLatido = Mathf.Abs(Mathf.Sin(Time.time * 3f)) * colorDaño.a;
                pantallaRoja.color = new Color(colorDaño.r, colorDaño.g, colorDaño.b, alfaLatido);
            }
        }

        // Evitamos que los valores se salgan de 0 a 100
        saludActual = Mathf.Clamp(saludActual, 0, 100);
        hambreActual = Mathf.Clamp(hambreActual, 0, 100);

        ActualizarUI();

        // Efecto visual de desvanecer la pantalla roja poco a poco (Solo si NO nos estamos muriendo de hambre)
        if (hambreActual > 0 && pantallaRoja != null && pantallaRoja.color.a > 0)
        {
            pantallaRoja.color = Color.Lerp(pantallaRoja.color, Color.clear, velocidadDesvanecimientoDaño * Time.deltaTime);
        }

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

    public void RecibirDaño(float cantidad)
    {
        saludActual -= cantidad;
        saludActual = Mathf.Clamp(saludActual, 0, 100);

        // Activamos el pantallazo rojo
        if (pantallaRoja != null)
        {
            pantallaRoja.color = colorDaño;
        }

        ActualizarUI();
    }
}