using UnityEngine;

public class CamaTutorial : MonoBehaviour
{
    public void Dormir()
    {
        ManejadorTutorial tutorial = FindFirstObjectByType<ManejadorTutorial>();
        
        // ¡CANDADO ACTIVO! Solo te deja dormir EXACTAMENTE en el paso 4 (Noche del Día 1). 
        // Si avanzas al paso 5 (Amanecer del Día 2), ya no hará nada.
        if (tutorial != null && tutorial.pasoActual == 4)
        {
            // Avanza al paso 5 (Día 2) para bloquear la cama
            tutorial.AvanzarTutorial();
            
            CinematicaDormir cinematica = FindFirstObjectByType<CinematicaDormir>(FindObjectsInactive.Include);
            if (cinematica != null)
            {
                cinematica.gameObject.SetActive(true);
                cinematica.IniciarDormir();
            }
        }
    }
}