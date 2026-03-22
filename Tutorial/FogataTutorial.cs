using UnityEngine;

public class FogataTutorial : MonoBehaviour
{
    [Header("Zombis de la Horda")]
    public GameObject[] zombis; // Arrastra a los 3 zombis aquí desde tu jerarquía

    [Header("Efectos Visuales")]
    public GameObject fuegoParticulas; // Las partículas de fuego (opcional)
    public Light luzFogata;            // La luz naranja de la fogata (opcional)

    private bool hordaActiva = false;
    private ManejadorTutorial tutorial;

    void Start()
    {
        tutorial = FindFirstObjectByType<ManejadorTutorial>();
        
        // Nos aseguramos de que el fuego y los zombis estén apagados al iniciar el juego
        if (fuegoParticulas != null) fuegoParticulas.SetActive(false);
        
        // ¡CORRECCIÓN! Usamos .enabled en lugar de .SetActive para los componentes de Luz
        if (luzFogata != null) luzFogata.enabled = false; 

        foreach(GameObject z in zombis)
        {
            if (z != null) z.SetActive(false);
        }
    }

    public void Encender()
    {
        // Solo la podemos encender si estamos en el paso exacto de la fogata (Paso 3)
        if (tutorial != null && tutorial.pasoActual == 3)
        {
            // Prendemos el fuego
            if (fuegoParticulas != null) fuegoParticulas.SetActive(true);
            
            // ¡CORRECCIÓN! Usamos .enabled aquí también
            if (luzFogata != null) luzFogata.enabled = true; 

            // ¡MAGIA DE TERROR! Cambiamos la niebla a un rojo oscuro y denso
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.3f, 0f, 0f); // Rojo oscuro
            RenderSettings.fogDensity = 0.08f;

            // Despertamos a la horda
            foreach(GameObject z in zombis)
            {
                if (z != null) z.SetActive(true);
            }

            hordaActiva = true;
            tutorial.AvanzarTutorial(); // Pasa al Paso 4: ¡Sobrevive a la oleada!
        }
    }

    void Update()
    {
        if (hordaActiva)
        {
            bool todosMuertos = true;

            // Revisamos uno por uno si los zombis siguen vivos
            foreach(GameObject z in zombis)
            {
                if (z != null)
                {
                    // Sacamos su script para ver si la variable "estaMuerto" ya es verdadera
                    EnemigoZombi scriptZombi = z.GetComponent<EnemigoZombi>();
                    if (scriptZombi != null && !scriptZombi.estaMuerto)
                    {
                        todosMuertos = false; // ¡Aún queda uno vivo!
                        break;
                    }
                }
            }

            // Si el ciclo terminó y todosMuertos sigue siendo true, ¡ganaste!
            if (todosMuertos)
            {
                hordaActiva = false;

                // ¡NUEVO! Apagamos la fogata y su luz porque ya estás a salvo
                if (fuegoParticulas != null) fuegoParticulas.SetActive(false);
                if (luzFogata != null) luzFogata.enabled = false;
                
                // Quitamos la niebla roja y ponemos una de noche normal
                RenderSettings.fogColor = new Color(0.1f, 0.1f, 0.15f); 
                RenderSettings.fogDensity = 0.02f;

                // Avanzamos al Paso 5: Ve a dormir
                if (tutorial != null) tutorial.AvanzarTutorial();
            }
        }
    }
}