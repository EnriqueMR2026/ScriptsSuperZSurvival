using UnityEngine;

public class ObjetoRecogible : MonoBehaviour
{
    [Header("Configuración del Objeto")]
    [Tooltip("¿Qué herramienta te va a dar al recogerlo?")]
    public InteraccionJugador.TipoHerramienta herramientaADar;

    [Header("Conexión con la Historia")]
    [Tooltip("¿Recoger este objeto avanza la misión del tutorial?")]
    public bool avanzaTutorial = false; 

    // Esta función la vamos a llamar cuando toques el botón de la manita en la pantalla
    public void Interactuar(GameObject jugador)
    {
        // 1. Buscamos el script de tu jugador para desbloquearle el arma
        InteraccionJugador interaccion = jugador.GetComponent<InteraccionJugador>();
        if (interaccion != null)
        {
            interaccion.DesbloquearHerramienta(herramientaADar);
        }

        // 2. Si este objeto es clave para la historia (como tu hacha inicial), avanzamos el tutorial
        if (avanzaTutorial)
        {
            // ¡CORRECCIÓN! Buscamos el script por su nombre real: ManejadorTutorial
            ManejadorTutorial tutorial = FindFirstObjectByType<ManejadorTutorial>();
            if (tutorial != null)
            {
                tutorial.AvanzarTutorial();
            }
        }

        // 3. Destruimos el objeto de la mesa porque ¡ya lo tienes en las manos!
        Destroy(gameObject);
    }
}