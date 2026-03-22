using UnityEngine;

public class CamaTutorial : MonoBehaviour
{
    public void Dormir()
    {
        ManejadorTutorial tutorial = FindFirstObjectByType<ManejadorTutorial>();
        
        // ¡CORRECCIÓN! El paso correcto para ir a dormir después de limpiar la zona es el 5
        if (tutorial != null && tutorial.pasoActual == 5)
        {
            // Avanza al paso 6 para que el texto de la misión desaparezca y bloquee la cama
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