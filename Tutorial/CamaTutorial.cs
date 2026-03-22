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
            
            // ¡MAGIA! Buscamos el nuevo script y le decimos que inicie la cinemática
            CinematicaDormir cinematica = FindFirstObjectByType<CinematicaDormir>();
            if (cinematica != null)
            {
                cinematica.IniciarDormir();
            }
        }
    }
}