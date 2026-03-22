using UnityEngine;

public class CamaTutorial : MonoBehaviour
{
    public void Dormir()
    {
        ManejadorTutorial tutorial = FindFirstObjectByType<ManejadorTutorial>();
        
        // Solo te deja dormir si ya mataste a los zombis
        if (tutorial != null && tutorial.pasoActual >= 4)
        {
            tutorial.AvanzarTutorial();
            
            // ¡EL TRUCO NUEVO! Buscamos el script INCLUSO si el Canvas está apagado
            CinematicaDormir cinematica = FindFirstObjectByType<CinematicaDormir>(FindObjectsInactive.Include);
            if (cinematica != null)
            {
                // Primero prendemos el Canvas para que se vea la magia
                cinematica.gameObject.SetActive(true);
                // Luego iniciamos la cinemática
                cinematica.IniciarDormir();
            }
        }
    }
}