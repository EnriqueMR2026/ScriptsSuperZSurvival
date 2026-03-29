using UnityEngine;
using System.Collections;
using TMPro;

public class ControladorArmas : MonoBehaviour
{
    [Header("Configuración del Arma")]
    public string nombreArma = "AK-47";
    public float cadenciaDisparo = 0.15f; // Qué tan rápido salen las balas

    [Header("Identificación Visual (El DNI)")]
    public Sprite iconoCinturon;      // Arrastra aquí la imagen del arma para el slot
    public Sprite iconoBotonAccion;   // Arrastra aquí la imagen para el botón de atacar (ej. una bala)
    public bool usaBalas = true;      // Si está encendido, el juego sabrá que debe mostrar el engranaje de recargar

    [Header("Posicionamiento Individual")]
    public Vector3 posEspera;         
    public Vector3 rotEspera;         
    public Vector3 posDisparo;        
    public Vector3 rotDisparo;        

    [Header("Munición")]
    public int municionActual = 30;
    public int capacidadCargador = 30;
    public int municionReserva = 90;
    public TextMeshProUGUI textoMunicion; // Tu texto "30/30"

    [Header("Disparo y Física")]
    public GameObject prefabBala;
    public Transform puntoDisparo;
    public float fuerzaBala = 50f;

    [Header("Efectos y Sonido")]
    public ParticleSystem efectoFogonazo; 
    public AudioSource audioSource;
    public AudioClip sonidoDisparo;
    public AudioClip sonidoRecarga;
    public AudioClip sonidoSinBalas;

    [Header("Efecto de Recarga")]
    public GameObject cartuchoModelo; 

    private float proximoDisparo = 0f;
    private bool recargando = false;

    void Start()
    {
        ActualizarHUD();
    }

    // Unity llama a OnEnable automáticamente cuando sacas el arma
    private void OnEnable()
    {
        if (textoMunicion != null) 
        {
            textoMunicion.gameObject.SetActive(true);
            ActualizarHUD(); // Actualiza el número por si recargaste antes de guardarla
        }
    }

    // Unity llama a OnDisable automáticamente cuando guardas el arma en el cinturón
    private void OnDisable()
    {
        if (textoMunicion != null) 
        {
            textoMunicion.gameObject.SetActive(false); // Esconde el "30/30"
        }
    }

    // Esta función la llamaremos cuando el jugador mantenga presionado el botón de disparo
    public void IntentarDisparar()
    {
        if (recargando) return; // Si estamos recargando, se bloquea el disparo

        if (municionActual > 0 && Time.time >= proximoDisparo)
        {
            proximoDisparo = Time.time + cadenciaDisparo;
            EjecutarDisparo();
        }
        else if (municionActual <= 0 && Time.time >= proximoDisparo)
        {
            // ¡NUEVO! Sonido de "Click" de arma vacía
            if (audioSource != null && sonidoSinBalas != null)
            {
                audioSource.PlayOneShot(sonidoSinBalas);
            }
            
            proximoDisparo = Time.time + 0.3f; // Pausa para no saturar el click
        }
    }

    void EjecutarDisparo()
    {
        municionActual--;
        ActualizarHUD();

        // 1. Sonido y Fogonazo
        if (audioSource != null && sonidoDisparo != null) audioSource.PlayOneShot(sonidoDisparo);
        if (efectoFogonazo != null) efectoFogonazo.Play();

        // 2. Disparar tu Proyectil
        if (prefabBala != null && puntoDisparo != null)
        {
            GameObject bala = Instantiate(prefabBala, puntoDisparo.position, puntoDisparo.rotation);
            
            // --- ¡MAGIA MEJORADA! Ignoramos TODOS los colisionadores del jugador ---
            Collider[] colisionadoresJugador = GetComponentsInParent<Collider>();
            Collider colliderBala = bala.GetComponent<Collider>();
            
            if (colliderBala != null)
            {
                foreach (Collider colJugador in colisionadoresJugador)
                {
                    Physics.IgnoreCollision(colliderBala, colJugador);
                }
            }

            Rigidbody rbBala = bala.GetComponent<Rigidbody>();
            if (rbBala != null)
            {
                rbBala.AddForce(puntoDisparo.forward * fuerzaBala, ForceMode.Impulse);
            }
        }

        // 3. ¡EL RETROCESO DE CÁMARA Y EL RELOJITO OSCURO!
        // Buscamos el script del jugador y le damos el golpe y el tiempo de recarga
        InteraccionJugador jugador = GetComponentInParent<InteraccionJugador>();
        if (jugador != null)
        {
            jugador.AplicarGolpeRetroceso();
            jugador.AplicarCooldown(cadenciaDisparo); // ¡AQUÍ ACTIVAMOS EL RELOJITO!
        }
    }

    // Esta función la conectaremos a tu nuevo botón de Recargar en la pantalla
    public void IniciarRecarga()
    {
        // Solo recarga si le faltan balas y tiene reservas
        if (recargando || municionActual == capacidadCargador || municionReserva <= 0) return;
        
        StartCoroutine(RutinaRecarga());
    }

    IEnumerator RutinaRecarga()
    {
        recargando = true;

        // 1. Iniciamos el sonido
        if (audioSource != null && sonidoRecarga != null) audioSource.PlayOneShot(sonidoRecarga);

        // 2. Magia visual: Apagamos el cartucho para simular que lo quitó
        if (cartuchoModelo != null) cartuchoModelo.SetActive(false);

        // 3. Esperamos el tiempo exacto que dure tu clip de audio de recarga
        float tiempoEspera = (sonidoRecarga != null) ? sonidoRecarga.length : 2f;

        // ¡NUEVO! Le avisamos al jugador cuánto va a durar la recarga para que el relojito gire lento
        InteraccionJugador jugador = GetComponentInParent<InteraccionJugador>();
        if (jugador != null)
        {
            jugador.AplicarCooldown(tiempoEspera);
        }

        yield return new WaitForSeconds(tiempoEspera);

        // 4. Magia visual: Encendemos el cartucho nuevo
        if (cartuchoModelo != null) cartuchoModelo.SetActive(true);

        // 5. Matemáticas de las balas
        int balasFaltantes = capacidadCargador - municionActual;
        int balasARecargar = Mathf.Min(balasFaltantes, municionReserva);

        municionActual += balasARecargar;
        municionReserva -= balasARecargar;

        ActualizarHUD();
        recargando = false;
    }

    public void ActualizarHUD()
    {
        if (textoMunicion != null)
        {
            textoMunicion.text = municionActual + "/" + municionReserva;
            textoMunicion.color = (municionActual <= 0) ? Color.red : Color.white;
        }
    }

    // ¡NUEVA FUNCIÓN VISUALIZADORA! Solo se ve en el Editor de Unity
    private void OnDrawGizmos()
    {
        // ¡CORRECCIÓN! Usamos el puntoDisparo (la punta del cañón) en lugar del posDisparo
        if (puntoDisparo != null)
        {
            // Dibujamos una esfera roja brillante exactamente donde nacen las balas
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(puntoDisparo.position, 0.03f);
            
            // Dibujamos una línea roja que te muestra hacia dónde van a salir disparadas
            Gizmos.DrawRay(puntoDisparo.position, puntoDisparo.forward * 2f);
        }
    }
}