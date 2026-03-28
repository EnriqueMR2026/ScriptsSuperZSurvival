using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManejadorConfiguracion : MonoBehaviour
{
    [Header("UI Visual (Textos y Sliders)")]
    public TMP_Text txtVolMaestro;
    public Slider sliderVolMaestro;
    
    public TMP_Text txtVolMusica;
    public Slider sliderVolMusica;
    
    public TMP_Text txtVolEfectos;
    public Slider sliderVolEfectos;

    public TMP_Text txtBrillo;
    public Slider sliderBrillo;

    [Header("🎧 Audio y Sonido")]
    public float volumenMaestro;
    public float volumenMusica;
    public float volumenEfectos;

    [Header("🔋 Gráficos y Rendimiento")]
    public int calidadGrafica = 1; // 1 = Media (Bloqueada por ahora)
    public float nivelBrillo;

    [Header("📱 Controles y HUD")]
    public float sensibilidadCamara;
    public float tamanoBotones;
    public float opacidadUI;
    public int vibracionActivada; // 1 = Sí, 0 = No
    public int joystickDinamico; // 1 = Sí, 0 = No
    public int modoZurdo; // 1 = Sí, 0 = No

    [Header("⚙️ Interfaz y Experiencia")]
    public string idiomaActual = "Español"; // Bloqueado por ahora
    public int hitmarkersActivados; // 1 = Sí, 0 = No
    public int marcadorCombosActivado; // 1 = Sí, 0 = No

    void Start()
    {
        // Apenas el menú carga, leemos la memoria del celular
        CargarOpcionesGuardadas();
    }

    public void CargarOpcionesGuardadas()
    {
        volumenMaestro = PlayerPrefs.GetFloat("VolumenMaestro", 1f); 
        volumenMusica = PlayerPrefs.GetFloat("VolumenMusica", 1f);
        volumenEfectos = PlayerPrefs.GetFloat("VolumenEfectos", 1f);
        nivelBrillo = PlayerPrefs.GetFloat("NivelBrillo", 1f);
        
        sensibilidadCamara = PlayerPrefs.GetFloat("Sensibilidad", 0.5f); 
        tamanoBotones = PlayerPrefs.GetFloat("TamanoBotones", 1f);
        opacidadUI = PlayerPrefs.GetFloat("OpacidadUI", 1f);
        
        vibracionActivada = PlayerPrefs.GetInt("Vibracion", 1); 
        joystickDinamico = PlayerPrefs.GetInt("JoystickDinamico", 0); 
        modoZurdo = PlayerPrefs.GetInt("ModoZurdo", 0);
        hitmarkersActivados = PlayerPrefs.GetInt("Hitmarkers", 1);
        marcadorCombosActivado = PlayerPrefs.GetInt("MarcadorCombos", 1);

        // ¡MAGIA PURA! Movemos las barritas físicas para que coincidan con la memoria guardada
        if (sliderVolMaestro != null) sliderVolMaestro.value = volumenMaestro;
        if (sliderVolMusica != null) sliderVolMusica.value = volumenMusica;
        if (sliderVolEfectos != null) sliderVolEfectos.value = volumenEfectos;
        if (sliderBrillo != null) sliderBrillo.value = nivelBrillo;

        Debug.Log("¡Configuración del jugador cargada con éxito de la memoria del celular!");
    }

    // --- FUNCIONES PARA QUE EL CANVAS (LA INTERFAZ) GUARDE LOS CAMBIOS ---

    public void GuardarVolumenMaestro(float valor)
    {
        volumenMaestro = valor;
        PlayerPrefs.SetFloat("VolumenMaestro", valor);
        AudioListener.volume = valor; 
        PlayerPrefs.Save(); 

        // ¡LA MAGIA DEL NUMERITO! Multiplicamos por 100 y le pegamos el símbolo de porcentaje
        if (txtVolMaestro != null) txtVolMaestro.text = Mathf.RoundToInt(valor * 100f).ToString() + "%";
    }

    public void GuardarVolumenMusica(float valor)
    {
        volumenMusica = valor;
        PlayerPrefs.SetFloat("VolumenMusica", valor);
        PlayerPrefs.Save();

        // ¡LA MAGIA DEL NUMERITO!
        if (txtVolMusica != null) txtVolMusica.text = Mathf.RoundToInt(valor * 100f).ToString() + "%";
    }

    public void GuardarVolumenEfectos(float valor)
    {
        volumenEfectos = valor;
        PlayerPrefs.SetFloat("VolumenEfectos", valor);
        PlayerPrefs.Save();

        if (txtVolEfectos != null) txtVolEfectos.text = Mathf.RoundToInt(valor * 100f).ToString() + "%";
    }

    public void GuardarSensibilidad(float valor)
    {
        sensibilidadCamara = valor;
        PlayerPrefs.SetFloat("Sensibilidad", valor);
        PlayerPrefs.Save();

        if (txtSensibilidad != null) txtSensibilidad.text = Mathf.RoundToInt(valor * 100f).ToString() + "%";
    }

    public void GuardarTamanoBotones(float valor)
    {
        tamanoBotones = valor;
        PlayerPrefs.SetFloat("TamanoBotones", valor);
        PlayerPrefs.Save();
    }

    public void AlternarHitmarkers(bool activado)
    {
        // Convertimos el "True/False" del botón a "1 o 0" para que Android lo entienda
        hitmarkersActivados = activado ? 1 : 0;
        PlayerPrefs.SetInt("Hitmarkers", hitmarkersActivados);
        PlayerPrefs.Save();
    }

    public void AlternarMarcadorCombos(bool activado)
    {
        marcadorCombosActivado = activado ? 1 : 0;
        PlayerPrefs.SetInt("MarcadorCombos", marcadorCombosActivado);
        PlayerPrefs.Save();
    }

    // --- NUEVAS FUNCIONES DE BRILLO Y VIBRACIÓN ---
    
    public void GuardarNivelBrillo(float valor)
    {
        nivelBrillo = valor;
        PlayerPrefs.SetFloat("NivelBrillo", valor);
        
        // ¡Magia exclusiva para celulares! Esto controla el brillo físico de la pantalla
        Screen.brightness = valor; 
        
        PlayerPrefs.Save();

        if (txtBrillo != null) txtBrillo.text = Mathf.RoundToInt(valor * 100f).ToString() + "%";
    }

    public void AlternarVibracion(bool activado)
    {
        vibracionActivada = activado ? 1 : 0;
        PlayerPrefs.SetInt("Vibracion", vibracionActivada);
        PlayerPrefs.Save();
    }

    // --- NUEVAS FUNCIONES DE CONTROLES Y HUD ---

    public void GuardarOpacidadUI(float valor)
    {
        opacidadUI = valor;
        PlayerPrefs.SetFloat("OpacidadUI", valor);
        PlayerPrefs.Save();
    }

    public void AlternarJoystickDinamico(bool activado)
    {
        joystickDinamico = activado ? 1 : 0;
        PlayerPrefs.SetInt("JoystickDinamico", joystickDinamico);
        PlayerPrefs.Save();
    }

    public void AlternarModoZurdo(bool activado)
    {
        modoZurdo = activado ? 1 : 0;
        PlayerPrefs.SetInt("ModoZurdo", modoZurdo);
        PlayerPrefs.Save();
    }
}
