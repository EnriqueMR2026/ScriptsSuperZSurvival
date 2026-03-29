using UnityEngine;
using UnityEngine.SceneManagement; // Para poder saltar al Game Over o Menú

public class ManejadorPausa : MonoBehaviour
{
    [Header("Paneles del Menú")]
    public GameObject panelPausa;
    public GameObject panelConfiguracion;

    [Header("Estado del Juego")]
    public bool estaPausado = false;

    void Update()
    {
        // Si el jugador presiona la tecla de escape (o la que tú decidas), se pausa
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (estaPausado) Reanudar();
            else Pausar();
        }
    }

    public void Pausar()
    {
        estaPausado = true;
        panelPausa.SetActive(true);
        Time.timeScale = 0f; // ¡MÁGICA! Detiene todo el movimiento en el juego
        
        // Aquí bloqueamos el cursor para que el jugador pueda usar el ratón en el menú
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Reanudar()
    {
        estaPausado = false;
        panelPausa.SetActive(false);
        panelConfiguracion.SetActive(false);
        Time.timeScale = 1f; // El tiempo vuelve a correr normal
        
        // Volvemos a ocultar el cursor para seguir disparando
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void IrAlResumen()
    {
        Time.timeScale = 1f; // ¡Ojo! Siempre hay que devolver el tiempo a 1 antes de cambiar de escena
        SceneManager.LoadScene("EscenaGameOver"); // Asegúrate de que tu escena se llame así exactito
    }
}