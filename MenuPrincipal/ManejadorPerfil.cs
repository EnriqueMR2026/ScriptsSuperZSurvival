using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ManejadorPerfil : MonoBehaviour
{
    [Header("Gestión de Paneles")]
    public GameObject panelPrincipal;
    public GameObject panelSeleccionModo;
    public GameObject panelConfiguracion;
    public GameObject panelGameOver;
    
    [Header("UI Registro")]
    public TMP_InputField inputNombre;
    public Button botonAceptar;
    public GameObject panelPopUp;
    public GameObject[] fotosPerfil; 
    
    [Header("UI Menu Principal")]
    public TMP_Text textoNombreMenu;

    [Header("UI Panel Perfil (Configuración)")]
    public TMP_Text textoNombreConfig;
    public GameObject[] fotosConfig; 

    [Header("Efectos de Transición")]
    public CanvasGroup cortinaNegra; // Arrastra aquí tu CortinaNegra
    public float velocidadTransicion = 3f; // Qué tan rápido se oscurece

    private int iconoSeleccionado = -1;

    void Start()
    {
        // Al iniciar el juego, mostramos el nombre si ya existe
        ActualizarTextoMenu();

        // ¡NUEVO! Revisamos si venimos de la pantalla de pausa/juego
        if (PlayerPrefs.GetInt("MostrarGameOver", 0) == 1)
        {
            // Apagamos el menú principal y encendemos el panel de resultados
            panelPrincipal.SetActive(false);
            if (panelGameOver != null) 
            {
                panelGameOver.SetActive(true);
            }

            // Borramos la nota secreta para que no se abra la próxima vez que entremos de forma normal
            PlayerPrefs.DeleteKey("MostrarGameOver");
        }
    }

    // Esta se la pones al Botón JUGAR del Menú Principal
    public void AbrirSeleccionModo()
    {
        StartCoroutine(RutinaTransicionJugar());
    }

    // Esta se la pones al Botón ATRÁS de la pantalla de Selección de Modo
    public void VolverAlMenuPrincipal()
    {
        StartCoroutine(RutinaTransicionVolver());
    }

    // Función auxiliar para mostrar el nombre en el menú principal Y en la configuración
    void ActualizarTextoMenu()
    {
        if (PlayerPrefs.HasKey("NombreJugador"))
        {
            string nombre = PlayerPrefs.GetString("NombreJugador");
            textoNombreMenu.text = "Soldado: " + nombre;

            // ¡NUEVO! Actualizamos el texto en tu panel de Perfil
            if (textoNombreConfig != null) textoNombreConfig.text = nombre;

            // ¡NUEVO! Encendemos solo la foto que el jugador eligió
            if (fotosConfig != null && fotosConfig.Length > 0)
            {
                int iconoGuardado = PlayerPrefs.GetInt("IconoJugador", 0);
                for (int i = 0; i < fotosConfig.Length; i++)
                {
                    if (fotosConfig[i] != null) fotosConfig[i].SetActive(i == iconoGuardado);
                }
            }
        }
        else
        {
            textoNombreMenu.text = "";
            if (textoNombreConfig != null) textoNombreConfig.text = "Sin perfil";
        }
    }
    
    // Se llama cada vez que escribes en el InputField
    public void ValidarRegistro()
    {
        // Condición: Min 4 letras Y que haya seleccionado un icono
        if (inputNombre.text.Length >= 4 && iconoSeleccionado != -1)
        {
            botonAceptar.interactable = true;
        }
        else
        {
            botonAceptar.interactable = false;
        }
    }

    // Función para los botones del carrusel (Atrás = -1, Adelante = 1)
    public void CambiarFoto(int direccion)
    {
        // Si ya hay una foto mostrándose, la ocultamos primero
        if (iconoSeleccionado != -1)
        {
            fotosPerfil[iconoSeleccionado].SetActive(false);
        }

        // Si es la primera vez que toca un botón (no hay foto visible)
        if (iconoSeleccionado == -1)
        {
            if (direccion == 1) iconoSeleccionado = 0; // Si le da adelante, muestra la Foto (1)
            if (direccion == -1) iconoSeleccionado = fotosPerfil.Length - 1; // Si le da atrás, muestra la Foto (5)
        }
        else
        {
            // Sumamos o restamos la dirección (+1 o -1) al índice actual
            iconoSeleccionado += direccion;

            // Matemáticas para que el carrusel sea infinito (dé la vuelta)
            if (iconoSeleccionado >= fotosPerfil.Length)
            {
                iconoSeleccionado = 0; // Si se pasa de la última, vuelve a la primera
            }
            else if (iconoSeleccionado < 0)
            {
                iconoSeleccionado = fotosPerfil.Length - 1; // Si baja de la primera, va a la última
            }
        }

        // Mostramos la nueva foto seleccionada
        fotosPerfil[iconoSeleccionado].SetActive(true);

        // Validamos si ya puede darle al botón de Aceptar
        ValidarRegistro();
    }

    // Esta se la pones al Botón ACEPTAR del Pop-Up
    public void GuardarPerfil()
    {
        PlayerPrefs.SetString("NombreJugador", inputNombre.text);
        PlayerPrefs.SetInt("IconoJugador", iconoSeleccionado);
        PlayerPrefs.Save();

        ActualizarTextoMenu(); // Actualizamos el nombre en pantalla
        StartCoroutine(RutinaTransicionAceptar());
    }

    // --- CORRUTINAS DE TRANSICIÓN OSCURA --- //

    IEnumerator RutinaTransicionJugar()
    {
        // 1. Oscurecer la pantalla
        cortinaNegra.blocksRaycasts = true; // Bloquea toques accidentales
        while (cortinaNegra.alpha < 1f)
        {
            cortinaNegra.alpha += Time.deltaTime * velocidadTransicion;
            yield return null;
        }

        // 2. Hacer los cambios de paneles en secreto (mientras está oscuro)
        panelPrincipal.SetActive(false);
        panelSeleccionModo.SetActive(true);

        if (!PlayerPrefs.HasKey("NombreJugador"))
        {
            panelPopUp.SetActive(true);
            botonAceptar.interactable = false;
        }
        else
        {
            panelPopUp.SetActive(false);
        }

        // 3. Volver a iluminar la pantalla
        while (cortinaNegra.alpha > 0f)
        {
            cortinaNegra.alpha -= Time.deltaTime * velocidadTransicion;
            yield return null;
        }
        cortinaNegra.blocksRaycasts = false;
    }

    IEnumerator RutinaTransicionAceptar()
    {
        // 1. Oscurecer la pantalla
        cortinaNegra.blocksRaycasts = true;
        while (cortinaNegra.alpha < 1f)
        {
            cortinaNegra.alpha += Time.deltaTime * velocidadTransicion;
            yield return null;
        }

        // 2. Cerrar solo el Pop-Up en secreto
        panelPopUp.SetActive(false);

        // 3. Volver a iluminar
        while (cortinaNegra.alpha > 0f)
        {
            cortinaNegra.alpha -= Time.deltaTime * velocidadTransicion;
            yield return null;
        }
        cortinaNegra.blocksRaycasts = false;
    }

    IEnumerator RutinaTransicionVolver()
    {
        // 1. Oscurecer la pantalla
        cortinaNegra.blocksRaycasts = true; 
        while (cortinaNegra.alpha < 1f)
        {
            cortinaNegra.alpha += Time.deltaTime * velocidadTransicion;
            yield return null;
        }

        // 2. Hacer el cambio de paneles al revés
        panelSeleccionModo.SetActive(false);
        panelPrincipal.SetActive(true);

        // 3. Volver a iluminar la pantalla
        while (cortinaNegra.alpha > 0f)
        {
            cortinaNegra.alpha -= Time.deltaTime * velocidadTransicion;
            yield return null;
        }
        cortinaNegra.blocksRaycasts = false;
    }

    // ¡NUEVA FUNCIÓN! Para el botón "Editar Perfil" en la configuración
    public void EditarPerfil()
    {
        StartCoroutine(RutinaEditarPerfil());
    }

    IEnumerator RutinaEditarPerfil()
    {
        // 1. Bajamos el telón oscuro
        cortinaNegra.blocksRaycasts = true;
        while (cortinaNegra.alpha < 1f)
        {
            cortinaNegra.alpha += Time.deltaTime * velocidadTransicion;
            yield return null;
        }

        // 2. Encendemos el PopUp mágico
        panelPopUp.SetActive(true);

        // 3. Precargamos los datos que ya tenía para que no tenga que escribir todo de nuevo
        if (PlayerPrefs.HasKey("NombreJugador"))
        {
            inputNombre.text = PlayerPrefs.GetString("NombreJugador");
            iconoSeleccionado = PlayerPrefs.GetInt("IconoJugador");

            // Encendemos la foto correcta en el carrusel del PopUp
            for (int i = 0; i < fotosPerfil.Length; i++)
            {
                if (fotosPerfil[i] != null) fotosPerfil[i].SetActive(i == iconoSeleccionado);
            }
            ValidarRegistro(); // Para encender el botón de aceptar
        }

        // 4. Subimos el telón
        while (cortinaNegra.alpha > 0f)
        {
            cortinaNegra.alpha -= Time.deltaTime * velocidadTransicion;
            yield return null;
        }
        cortinaNegra.blocksRaycasts = false;
    }

    // --- ¡NUEVAS FUNCIONES PARA ENTRAR Y SALIR DE LA CONFIGURACIÓN! ---
    
    public void AbrirConfiguracion()
    {
        StartCoroutine(RutinaTransicionConfiguracion(true));
    }

    public void CerrarConfiguracion()
    {
        StartCoroutine(RutinaTransicionConfiguracion(false));
    }

    IEnumerator RutinaTransicionConfiguracion(bool abriendo)
    {
        // 1. Bajamos el telón oscuro
        cortinaNegra.blocksRaycasts = true;
        while (cortinaNegra.alpha < 1f)
        {
            cortinaNegra.alpha += Time.deltaTime * velocidadTransicion;
            yield return null;
        }

        // 2. Hacemos el cambio de paneles según si entramos o salimos
        if (abriendo)
        {
            panelPrincipal.SetActive(false);
            panelConfiguracion.SetActive(true);
        }
        else
        {
            panelConfiguracion.SetActive(false);
            panelPrincipal.SetActive(true);
        }

        // 3. Subimos el telón
        while (cortinaNegra.alpha > 0f)
        {
            cortinaNegra.alpha -= Time.deltaTime * velocidadTransicion;
            yield return null;
        }
        cortinaNegra.blocksRaycasts = false;
    }
}