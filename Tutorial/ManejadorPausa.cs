using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ManejadorPausa : MonoBehaviour
{
    [Header("Paneles de UI")]
    public GameObject panelPausa;
    public GameObject panelConfiguracion;
    public GameObject canvasControlesMoviles;

    [Header("Textos")]
    public TextMeshProUGUI textoEstadisticas;

    private bool estaPausado = false;

    // Variables globales para llevar el conteo desde cualquier otro script
    public static int zombisEliminados = 0;
    public static float danoCausado = 0f;

    public void Pausar()
    {
        estaPausado = true;
        Time.timeScale = 0f;
        panelPausa.SetActive(true);

        // Actualizamos el texto de estadísticas justo al pausar
        if (textoEstadisticas != null)
        {
            float tiempoJugado = Time.timeSinceLevelLoad;
            int minutos = Mathf.FloorToInt(tiempoJugado / 60);
            int segundos = Mathf.FloorToInt(tiempoJugado % 60);
            
            textoEstadisticas.text = "TIEMPO DE SUPERVIVENCIA: " + minutos.ToString("00") + ":" + segundos.ToString("00") + "\n" +
                                     "ZOMBIS ELIMINADOS: " + zombisEliminados.ToString() + "\n" +
                                     "DAÑO CAUSADO: " + Mathf.RoundToInt(danoCausado).ToString();
        }

        // Apagamos los controles táctiles para que no haya accidentes
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