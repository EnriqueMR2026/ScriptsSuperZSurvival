using UnityEngine;

public class ControladorHerramientas : MonoBehaviour
{
    [Header("Configuración de la Herramienta")]
    public string nombreHerramienta = "Hacha";
    public float tiempoUso = 0.9f; // El cooldown que antes estaba en el jugador
    
    [Header("Lógica de Recolección")]
    public string etiquetaRecurso = "Recurso_Madera"; // A qué le pega (Recurso_Piedra para el pico)
    public string nombreRecursoInventario = "Madera"; // Qué te da al inventario (Piedra para el pico)

    [Header("Identificación Visual (El DNI)")]
    public Sprite iconoCinturon;      // Su foto del cinturón
    public Sprite iconoBotonAccion;   // Su foto de acción (ej. un tajo)
    public bool usaBalas = false;     // ¡APAGADO! No recargamos hachas

    [Header("Posicionamiento Individual")]
    public Vector3 posEspera;         
    public Vector3 rotEspera;         
    public Vector3 posAccion;        
    public Vector3 rotAccion;  

    [Header("Efectos y Sonido")]
    public AudioSource audioSource;
    public AudioClip sonidoUso; // Aquí va el sonidoTalar o sonidoPicar

    // Esta función la llamará el jugador cuando mantengas presionado el botón
    public void IntentarUsar(LayerMask capaInteractuable, Transform origenRayo, float distanciaInteraccion, InventarioJugador inventario)
    {
        RaycastHit hit;
        // Lanzamos el rayo para ver si le damos al árbol o a la piedra
        if (Physics.Raycast(origenRayo.position, origenRayo.forward, out hit, distanciaInteraccion, capaInteractuable))
        {
            // Verificamos si le pegó al recurso correcto para esta herramienta
            if (hit.collider.CompareTag(etiquetaRecurso))
            {
                inventario.AgregarRecurso(nombreRecursoInventario, 1);
                
                if (audioSource != null && sonidoUso != null) 
                {
                    audioSource.PlayOneShot(sonidoUso);
                }
            }
        }
    }
}