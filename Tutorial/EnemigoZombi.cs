using UnityEngine;
using UnityEngine.AI;

public class EnemigoZombi : MonoBehaviour
{
    // Variables del zombi
    public Transform objetivoJugador; 
    private NavMeshAgent agente;
    private Animator anim;

    [Header("Configuración de Ataque")]
    public float cooldownAtaque = 2f; // Tiempo en segundos entre cada golpe
    private float tiempoSiguienteAtaque = 0f;

    [Header("Daño al Jugador")]
    public float dañoAtaque = 15f;

    [Header("Salud y Estado")]
    public int vidaMaxima = 100;
    private int vidaActual;
    public bool estaMuerto = false;

    void Start()
    {
        // Conectamos el motor de movimiento y el de animaciones
        agente = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        // Inicializamos la vida del zombi al máximo
        vidaActual = vidaMaxima;
    }

    void Update()
    {
        // Si el zombi ya está muerto, cortamos la función aquí para que no haga nada más
        if (estaMuerto) return;

        // Si no hay jugador asignado o el agente no funciona, no hacemos nada
        if (objetivoJugador == null || agente == null) return;

        // Calculamos a qué distancia está el jugador
        float distancia = Vector3.Distance(transform.position, objetivoJugador.position);

        // Si el zombi está más lejos que su distancia de frenado, persigue al jugador
        if (distancia > agente.stoppingDistance)
        {
            agente.SetDestination(objetivoJugador.position);
            
            // Le mandamos la señal al Animator de que debe mover las piernas
            anim.SetBool("Caminando", true);
        }
        else
        {
            // Si ya está lo suficientemente cerca, se prepara para atacar
            // Hacemos que gire suavemente para no dejar de mirar al jugador
            Vector3 direccion = (objetivoJugador.position - transform.position).normalized;
            Quaternion rotacionMirar = Quaternion.LookRotation(new Vector3(direccion.x, 0, direccion.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionMirar, Time.deltaTime * 5f);
            
            // Le mandamos la señal de que se detenga
            anim.SetBool("Caminando", false);

            // Verificamos si el reloj ya nos permite tirar otro golpe
            if (Time.time >= tiempoSiguienteAtaque)
            {
                anim.SetTrigger("Atacar");

                // ¡NUEVO! Conectamos el golpe con la salud del jugador
                SupervivenciaJugador jugador = objetivoJugador.GetComponent<SupervivenciaJugador>();
                if (jugador != null)
                {
                    jugador.RecibirDaño(dañoAtaque);
                }
                
                // Reiniciamos el reloj sumándole el cooldown al tiempo actual
                tiempoSiguienteAtaque = Time.time + cooldownAtaque;
            }
        }
    }

    public void RecibirDano(int cantidad)
    {
        // Si ya está muerto, ignoramos el daño extra para no repetir la animación
        if (estaMuerto) return;

        vidaActual -= cantidad;

        // Verificamos si la vida se acabó
        if (vidaActual <= 0)
        {
            estaMuerto = true;
            
            // Apagamos su motor para que deje de perseguirte
            agente.enabled = false;
            
            // Le mandamos la señal matemática para que caiga al piso
            anim.SetTrigger("Morir");
            
            // Hacemos que el cuerpo desaparezca después de 5 segundos para no llenar la aldea de cadáveres
            Destroy(gameObject, 5f);
        }
    }
}