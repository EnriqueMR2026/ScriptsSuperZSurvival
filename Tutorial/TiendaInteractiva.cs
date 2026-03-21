using UnityEngine;

public class TiendaInteractiva : MonoBehaviour
{
    [Header("UI de la Tienda")]
    public GameObject panelTienda; // Arrastra aquí el panel de tu Canvas
    
    [Header("Referencias")]
    public InventarioJugador inventario;
    public InteraccionJugador interaccion;

    [Header("Ciclo de Día y Noche")]
    public Light luzDireccional; // Arrastra tu "Directional Light" (El Sol de tu escena) aquí

    void Update()
    {
        // Si la tienda está abierta, hacemos que anochezca suavemente aunque el juego esté en pausa
        if (panelTienda != null && panelTienda.activeSelf && luzDireccional != null)
        {
            // Oscurecemos la luz y la volvemos de un tono azul medianoche usando tiempo real (unscaledDeltaTime)
            luzDireccional.intensity = Mathf.Lerp(luzDireccional.intensity, 0.05f, Time.unscaledDeltaTime * 0.5f);
            luzDireccional.color = Color.Lerp(luzDireccional.color, new Color(0.1f, 0.15f, 0.3f), Time.unscaledDeltaTime * 0.5f);
        }
    }

    public void AbrirTienda()
    {
        if (panelTienda != null)
        {
            panelTienda.SetActive(true);
            Time.timeScale = 0f; // Pausamos el mundo 3D para que compres tranquilo
        }
    }

    public void CerrarTienda()
    {
        if (panelTienda != null)
        {
            panelTienda.SetActive(false);
            Time.timeScale = 1f; // El mundo vuelve a la normalidad
            
            // Magia del tutorial: Si estamos en la misión de la tienda (Paso 2) y ya tienes tu cuchillo
            ManejadorTutorial tutorial = FindFirstObjectByType<ManejadorTutorial>();
            if (tutorial != null && tutorial.pasoActual == 2 && interaccion.tieneCuchillo)
            {
                tutorial.AvanzarTutorial();
            }
        }
    }

    // --- FUNCIONES PARA LOS BOTONES DEL CANVAS DE LA TIENDA ---

    public void VenderMaderaTutorial()
    {
        // El tutorial pide cambiar 20 de madera. Digamos que te dan 50 de oro por ella.
        if (inventario.madera >= 20)
        {
            inventario.AgregarRecurso("Madera", -20);
            inventario.AgregarRecurso("Gold", 50);
        }
    }

    public void ComprarCuchillo()
    {
        // El cuchillo cuesta 30 de oro
        if (inventario.gold >= 30 && !interaccion.tieneCuchillo)
        {
            inventario.AgregarRecurso("Gold", -30);
            interaccion.DesbloquearHerramienta(InteraccionJugador.TipoHerramienta.CuerpoACuerpo);
        }
    }

    public void ComprarManzanas()
    {
        // 5 Manzanas cuestan 20 de oro
        if (inventario.gold >= 20)
        {
            inventario.AgregarRecurso("Gold", -20);
            
            // Desbloqueamos el slot de comida en el cinturón
            interaccion.DesbloquearHerramienta(InteraccionJugador.TipoHerramienta.Comida);
            
            // Le sumamos las 5 manzanas directamente a tu mano
            ControladorConsumibles comida = interaccion.objetoComida.GetComponent<ControladorConsumibles>();
            if (comida != null)
            {
                comida.cantidadActual += 5;
                comida.ActualizarHUD();
            }
        }
    }
}