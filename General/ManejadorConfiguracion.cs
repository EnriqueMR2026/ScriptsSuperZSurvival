using UnityEngine;
using UnityEngine.UI;

public class ManejadorConfiguracion : MonoBehaviour
{
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
        // Si es la primera vez que juegan, Unity pondrá estos valores por defecto
        volumenMaestro = PlayerPrefs.GetFloat("VolumenMaestro", 1f); // 1f = 100%
        volumenMusica = PlayerPrefs.GetFloat("VolumenMusica", 1f);
        volumenEfectos = PlayerPrefs.GetFloat("VolumenEfectos", 1f);

        nivelBrillo = PlayerPrefs.GetFloat("NivelBrillo", 1f);
        
        sensibilidadCamara = PlayerPrefs.GetFloat("Sensibilidad", 0.5f); // 0.5f = Mitad
        tamanoBotones = PlayerPrefs.GetFloat("TamanoBotones", 1f);
        opacidadUI = PlayerPrefs.GetFloat("OpacidadUI", 1f);
        
        vibracionActivada = PlayerPrefs.GetInt("Vibracion", 1); // 1 = Activado por defecto
        joystickDinamico = PlayerPrefs.GetInt("JoystickDinamico", 0); // 0 = Fijo por defecto
        modoZurdo = PlayerPrefs.GetInt("ModoZurdo", 0);

        hitmarkersActivados = PlayerPrefs.GetInt("Hitmarkers", 1);
        marcadorCombosActivado = PlayerPrefs.GetInt("MarcadorCombos", 1);

        // Aquí luego le diremos al juego que aplique estos valores visualmente
        Debug.Log("¡Configuración del jugador cargada con éxito de la memoria del celular!");
    }

    // --- FUNCIONES PARA QUE EL CANVAS (LA INTERFAZ) GUARDE LOS CAMBIOS ---

    public void GuardarVolumenMaestro(float valor)
    {
        volumenMaestro = valor;
        PlayerPrefs.SetFloat("VolumenMaestro", valor);
        AudioListener.volume = valor; // ¡Magia! Esto le baja el volumen a todo Unity al instante
        PlayerPrefs.Save(); // Forzamos el guardado en el disco del celular
    }

    public void GuardarVolumenMusica(float valor)
    {
        volumenMusica = valor;
        PlayerPrefs.SetFloat("VolumenMusica", valor);
        PlayerPrefs.Save();
    }

    public void GuardarVolumenEfectos(float valor)
    {
        volumenEfectos = valor;
        PlayerPrefs.SetFloat("VolumenEfectos", valor);
        PlayerPrefs.Save();
    }

    public void GuardarSensibilidad(float valor)
    {
        sensibilidadCamara = valor;
        PlayerPrefs.SetFloat("Sensibilidad", valor);
        PlayerPrefs.Save();
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
