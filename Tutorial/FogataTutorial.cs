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
        if (tutorial != null)
        {
            // --- FOGATA DÍA 1 ---
            if (tutorial.pasoActual == 3)
            {
                if (fuegoParticulas != null) fuegoParticulas.SetActive(true);
                if (luzFogata != null) luzFogata.enabled = true; 

                RenderSettings.fog = true;
                RenderSettings.fogMode = FogMode.Exponential;
                RenderSettings.fogColor = Color.red; 
                RenderSettings.fogDensity = 0.03f;

                foreach(GameObject z in zombis) { if (z != null) z.SetActive(true); }

                hordaActiva = true;
                tutorial.AvanzarTutorial(); 
            }
            // --- FOGATA DÍA 2 (MODO LIBRE) ---
            else if (tutorial.pasoActual == 6) 
            {
                if (fuegoParticulas != null) fuegoParticulas.SetActive(true);
                if (luzFogata != null) luzFogata.enabled = true; 

                // Reciclamos los zombis escondidos y los revivimos sin cambiar el clima
                foreach(GameObject z in zombis)
                {
                    if (z != null) 
                    {
                        z.SetActive(true);
                        EnemigoZombi scriptZ = z.GetComponent<EnemigoZombi>();
                        if (scriptZ != null) scriptZ.RevivirZombi();
                    }
                }
            }
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

                // Quitamos la niebla roja y ponemos una de noche normal
                RenderSettings.fogColor = new Color(0.1f, 0.1f, 0.15f); 
                RenderSettings.fogDensity = 0.02f;

                // Avanzamos al Paso 5: Ve a dormir
                if (tutorial != null) tutorial.AvanzarTutorial();
            }
        }
    }
}