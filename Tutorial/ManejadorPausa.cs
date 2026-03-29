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
}