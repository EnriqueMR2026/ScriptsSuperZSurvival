using UnityEngine;
using UnityEngine.SceneManagement;

public class ManejadorPausa : MonoBehaviour
{
    [Header("Paneles de UI")]
    public GameObject panelPausa;
    public GameObject panelConfiguracion;
    public GameObject canvasControlesMoviles; // ¡NUEVO! Para apagar los joysticks en Android

    private bool estaPausado = false;

    public void Pausar()
    {
        estaPausado = true;
        Time.timeScale = 0f;
        panelPausa.SetActive(true);

        // ¡NUEVO! Apagamos los controles táctiles para que no haya accidentes
        if (canvasControlesMoviles != null) canvasControlesMoviles.SetActive(false);

        // Liberamos el ratón (esto sigue siendo útil cuando haces pruebas en tu computadora)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Reanudar()
    {
        estaPausado = false;
        Time.timeScale = 1f;
        panelPausa.SetActive(false);
        panelConfiguracion.SetActive(false);

        // ¡NUEVO! Encendemos los controles táctiles de nuevo para seguir jugando
        if (canvasControlesMoviles != null) canvasControlesMoviles.SetActive(true);

        // Bloqueamos el ratón de nuevo (para pruebas en computadora)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void AbrirConfiguracion()
    {
        panelPausa.SetActive(false);
        panelConfiguracion.SetActive(true);
    }

    public void IrAlMenuPrincipal()
    {
        // Es súper importante regresar el tiempo a la normalidad antes de cambiar de escena
        Time.timeScale = 1f;

        // Aquí guardamos la variable para que el Menú Principal sepa qué hacer al cargar
        PlayerPrefs.SetInt("MostrarGameOver", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene("MenuPrincipal");
    }
}