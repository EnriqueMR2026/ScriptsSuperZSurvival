using UnityEngine;
using UnityEngine.UI;

public class SelectorModos : MonoBehaviour
{
    [Header("Paneles de Contenido (Lado Derecho)")]
    public GameObject panelTutorial;
    public GameObject panelArcade;
    public GameObject panelPuntuacion;

    void Start()
    {
        // Al entrar a esta pantalla, mostramos el Tutorial por defecto
        MostrarCategoria(0);
    }

    // Esta función la conectaremos a tus 3 botones izquierdos
    // 0 = Tutorial, 1 = Arcade, 2 = Puntuación
    public void MostrarCategoria(int indice)
    {
        // 1. Apagamos todos los paneles de la derecha para limpiar la pantalla
        panelTutorial.SetActive(false);
        panelArcade.SetActive(false);
        panelPuntuacion.SetActive(false);

        // 2. Encendemos únicamente el que el jugador seleccionó
        if (indice == 0) panelTutorial.SetActive(true);
        if (indice == 1) panelArcade.SetActive(true);
        if (indice == 2) panelPuntuacion.SetActive(true);
    }
}