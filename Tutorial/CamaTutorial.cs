using UnityEngine;

public class CamaTutorial : MonoBehaviour
{
    public void Dormir()
    {
        ManejadorTutorial tutorial = FindFirstObjectByType<ManejadorTutorial>();
        
        // Solo te deja dormir si ya mataste a los zombis y estás en el último paso (Paso 4 o 5, dependiendo de tu contador)
        if (tutorial != null && tutorial.pasoActual >= 4)
        {
            tutorial.AvanzarTutorial();
            
            // Aquí puedes poner un Debug para confirmar que ganaste, 
            // más adelante lo cambiaremos por una pantalla negra de "Día 1"
            Debug.Log("¡TUTORIAL COMPLETADO! Dulces sueños, mi campeón.");
        }
    }
}