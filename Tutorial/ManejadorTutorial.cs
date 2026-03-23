using UnityEngine;
using System.Collections; // Necesario para la pausa de tiempo

public class ManejadorTutorial : MonoBehaviour
{
    [Header("Conexión con el UI")]
    public PanelObjetivos panelObjetivos; // ¡Conectamos tu nuevo panel aquí!

    [Header("Progreso")]
    public int pasoActual = 0;

    void Start()
    {
        // Vaciamos el Start. Ahora la cinemática nos avisará cuándo iniciar.
    }

    // ¡NUEVA FUNCIÓN! La llamará la cinemática al terminar de despertar
    public void IniciarPrimeraMisionConRetraso()
    {
        StartCoroutine(EsperarYMostrarPrimeraMision());
    }

    private IEnumerator EsperarYMostrarPrimeraMision()
    {
        yield return new WaitForSeconds(2f); // Esperamos los 2 segundos que pediste
        ActualizarMision(pasoActual);        // Lanzamos la misión 0
    }

    // Esta función la mandaremos llamar desde la mesa, la tienda, etc.
    public void AvanzarTutorial()
    {
        pasoActual++;
        ActualizarMision(pasoActual);
    }

    void ActualizarMision(int paso)
    {
        string textoMision = "";

        switch (paso)
        {
            case 0:
                textoMision = "NUEVO OBJETIVO: Ve al comedor de la taberna y toma el hacha de la mesa.";
                break;
            case 1:
                textoMision = "NUEVO OBJETIVO: Sal al refugio, busca el Árbol Celestial y recolecta madera.";
                break;
            case 2:
                textoMision = "NUEVO OBJETIVO: Ve a la tienda, intercambia la madera por oro y compra un cuchillo y 5 manzanas.";
                break;
            case 3:
                textoMision = "NUEVO OBJETIVO: Prende la fogata en el centro del refugio para pasar la noche.";
                break;
            case 4:
                textoMision = "¡ALERTA!: ¡Sobrevive a la oleada de zombis usando tu cuchillo!";
                break;
            case 5:
                textoMision = "NUEVO OBJETIVO: La zona está limpia. Sube a tu habitación en la taberna y ve a dormir.";
                break;
            case 6:
                textoMision = "Gracias por jugar la mini DEMO, ahora todas las armas están desbloqueadas. Ve a la fogata y enciéndela.";
                break;
        }

        if (panelObjetivos != null && textoMision != "")
        {
            panelObjetivos.MostrarNuevaMision(textoMision);
        }
    }

    // ¡NUEVA FUNCIÓN!
    public void IniciarDiaDos()
    {
        pasoActual = 6;
        ActualizarMision(pasoActual);

        // Desbloqueamos las armas del jugador
        InteraccionJugador interaccion = FindFirstObjectByType<InteraccionJugador>();
        if (interaccion != null)
        {
            interaccion.DesbloquearArmasModoLibre();
        }
    }
}