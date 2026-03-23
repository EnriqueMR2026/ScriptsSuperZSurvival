using UnityEngine;
using TMPro;
using System.Collections;

public class ControladorConsumibles : MonoBehaviour
{
    [Header("Configuración del Consumible")]
    public string nombreConsumible = "Manzana";
    public int cantidadActual = 5;
    public float tiempoConsumo = 1.5f; // Lo que tarda en masticar (Cooldown)
    public float recuperacionHambre = 20f;
    public float recuperacionSalud = 10f;

    [Header("Identificación Visual (El DNI)")]
    public Sprite iconoCinturon;      // Arrastra aquí la foto de tu Manzana/Plátano
    public Sprite iconoBotonAccion;   // Arrastra aquí la foto para el botón (ej. una boca comiendo)
    public bool usaBalas = false;     // ¡APAGADO! No recargamos manzanas jajaja

    [Header("Posicionamiento Individual")]
    public Vector3 posEspera;         
    public Vector3 rotEspera;         
    public Vector3 posAccion;        
    public Vector3 rotAccion;  

    [Header("Efectos y Sonido")]
    public AudioSource audioSource;
    public AudioClip sonidoComer; // Tu sonido de masticar

    [Header("UI")]
    public TextMeshProUGUI textoCantidad; // El texto para mostrar "5" en el Canvas

    private bool consumiendo = false;
    private SupervivenciaJugador supervivencia;

    void Start()
    {
        // Buscamos el script de supervivencia en el jugador (padre) para curarlo
        supervivencia = GetComponentInParent<SupervivenciaJugador>();
        ActualizarHUD();
    }

    // Unity llama a OnEnable automáticamente cuando sacas la comida del cinturón
    private void OnEnable()
    {
        if (textoCantidad != null) 
        {
            textoCantidad.gameObject.SetActive(true);
            ActualizarHUD(); 
        }
    }

    // Unity llama a OnDisable automáticamente cuando guardas la comida
    private void OnDisable()
    {
        if (textoCantidad != null) 
        {
            textoCantidad.gameObject.SetActive(false); 
        }
    }

    // Esta función la llamará tu InteraccionJugador cuando mantengas presionado el botón
    public void IntentarConsumir()
    {
        if (consumiendo || cantidadActual <= 0) return;
        StartCoroutine(RutinaConsumir());
    }

    IEnumerator RutinaConsumir()
    {
        consumiendo = true;

        // 1. Sonido de masticar
        if (audioSource != null && sonidoComer != null) 
        {
            audioSource.PlayOneShot(sonidoComer);
        }

        // 2. Le avisamos al jugador cuánto va a durar para que el relojito gire
        InteraccionJugador jugador = GetComponentInParent<InteraccionJugador>();
        if (jugador != null)
        {
            jugador.AplicarCooldown(tiempoConsumo);
        }

        // 3. Esperamos a que termine de comer
        yield return new WaitForSeconds(tiempoConsumo);

        // 4. Descontamos la comida y curamos a nuestro jugador
        cantidadActual--;
        ActualizarHUD();

        if (supervivencia != null)
        {
            // Curamos y evitamos que pase de 100 usando Clamp
            supervivencia.hambreActual = Mathf.Clamp(supervivencia.hambreActual + recuperacionHambre, 0, 100);
            supervivencia.saludActual = Mathf.Clamp(supervivencia.saludActual + recuperacionSalud, 0, 100);
        }

        consumiendo = false;
    }

    public void ActualizarHUD()
    {
        if (textoCantidad != null)
        {
            textoCantidad.text = cantidadActual.ToString();
            // Se pone rojo si ya no te queda comida
            textoCantidad.color = (cantidadActual <= 0) ? Color.red : Color.white; 
        }
    }
}