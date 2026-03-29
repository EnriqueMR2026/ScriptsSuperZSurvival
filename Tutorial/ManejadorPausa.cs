using UnityEngine;
using UnityEngine.SceneManagement;

public class ManejadorPausa : MonoBehaviour
{
    [Header("Paneles de UI")]
    public GameObject panelPausa;
    public GameObject panelConfiguracion;

    private bool estaPausado = false;

    public void Pausar()
    {
        estaPausado = true;
        Time.timeScale = 0f;
        panelPausa.SetActive(true);

        // Liberamos el ratón para poder dar clic en la interfaz
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Reanudar()
    {
        estaPausado = false;
        Time.timeScale = 1f;
        panelPausa.SetActive(false);
        panelConfiguracion.SetActive(false);

        // Bloqueamos el ratón de nuevo para seguir disparando y moviendo la cámara
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