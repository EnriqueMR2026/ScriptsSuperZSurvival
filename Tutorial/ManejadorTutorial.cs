using UnityEngine;
using TMPro;

public class ManejadorTutorial : MonoBehaviour
{
    [Header("UI del Tutorial")]
    public TextMeshProUGUI textoMision; // Tu texto en el Canvas que dirá qué hacer
    public GameObject panelMision;      // El fondito negro o ventanita detrás del texto (opcional)

    [Header("Progreso")]
    public int pasoActual = 0;

    void Start()
    {
        // Al iniciar, mostramos la primera instrucción
        ActualizarMision(0);
    }

    // Esta función la mandaremos llamar desde la mesa, la tienda, los zombis, etc.
    public void AvanzarTutorial()
    {
        pasoActual++;
        ActualizarMision(pasoActual);
    }

    void ActualizarMision(int paso)
    {
        if (panelMision != null && !panelMision.activeSelf) 
        {
            panelMision.SetActive(true);
        }

        switch (paso)
        {
            case 0:
                textoMision.text = "NUEVO OBJETIVO: Ve al comedor de la taberna y toma el hacha de la mesa.";
                break;
            case 1:
                textoMision.text = "NUEVO OBJETIVO: Sal al refugio, busca el Árbol Celestial y recolecta madera.";
                break;
            case 2:
                textoMision.text = "NUEVO OBJETIVO: Ve a la tienda, intercambia la madera por oro y compra un cuchillo y 5 manzanas.";
                break;
            case 3:
                textoMision.text = "NUEVO OBJETIVO: Prende la fogata en el centro del refugio para pasar la noche.";
                break;
            case 4:
                textoMision.text = "¡ALERTA!: ¡Sobrevive a la oleada de zombis usando tu cuchillo!";
                break;
            case 5:
                textoMision.text = "NUEVO OBJETIVO: La zona está limpia. Sube a tu habitación en la taberna y ve a dormir.";
                break;
            default:
                // Si ya acabamos el tutorial, apagamos el panel
                if (panelMision != null) panelMision.SetActive(false);
                break;
        }
    }
}