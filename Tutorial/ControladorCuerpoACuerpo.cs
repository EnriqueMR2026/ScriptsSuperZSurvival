using UnityEngine;

public class ControladorCuerpoACuerpo : MonoBehaviour
{
    [Header("Configuración del Arma")]
    public string nombreArma = "Cuchillo";
    public float cadenciaAtaque = 0.5f; // Qué tan rápido puedes dar tajos
    public int dano = 50; // ¡El cuchillo hace mucho daño de cerca!
    public float distanciaAtaque = 2.5f; // Alcance del brazo

    [Header("Identificación Visual (El DNI)")]
    public Sprite iconoCinturon;      // Arrastra aquí la foto de tu Cuchillo/Hacha/Pico
    public Sprite iconoBotonAccion;   // Arrastra aquí la foto para el botón (ej. un tajo o el mismo cuchillo)
    public bool usaBalas = false;     // ¡APAGADO! Para que el cinturón esconda el botón de recargar

    [Header("Posicionamiento Individual")]
    public Vector3 posEspera;         
    public Vector3 rotEspera;         
    public Vector3 posAccion;        
    public Vector3 rotAccion;  

    [Header("Efectos y Sonido")]
    public AudioSource audioSource;
    public AudioClip sonidoAtaque; // Sonido de cortar el aire "¡Swish!"
    public AudioClip sonidoImpacto; // Sonido al apuñalar carne (opcional)

    private float proximoAtaque = 0f;

    // Esta función la llamará tu InteraccionJugador cuando toques el botón
    public void IntentarAtacar(LayerMask capaInteractuable, Transform origenRayo)
    {
        if (Time.time >= proximoAtaque)
        {
            proximoAtaque = Time.time + cadenciaAtaque;
            EjecutarAtaque(capaInteractuable, origenRayo);
        }
    }

    void EjecutarAtaque(LayerMask capaInteractuable, Transform origenRayo)
    {
        // 1. Sonido del tajo al aire
        if (audioSource != null && sonidoAtaque != null) 
        {
            audioSource.PlayOneShot(sonidoAtaque);
        }

        // 2. Lógica de Daño (Un rayo muy cortito hacia adelante)
        RaycastHit hit;
        if (Physics.Raycast(origenRayo.position, origenRayo.forward, out hit, distanciaAtaque, capaInteractuable))
        {
            // Reproducimos sonido de impacto si le dimos a algo
            if (audioSource != null && sonidoImpacto != null) 
            {
                audioSource.PlayOneShot(sonidoImpacto);
            }

            EnemigoZombi zombi = hit.collider.GetComponentInParent<EnemigoZombi>();
            if (zombi != null)
            {
                zombi.RecibirDano(dano); 
            }
        }
    }
}