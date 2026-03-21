using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ManejadorPerfil : MonoBehaviour
{
    [Header("Gestión de Paneles")]
    public GameObject panelPrincipal;
    public GameObject panelSeleccionModo;
    
    [Header("UI Registro")]
    public TMP_InputField inputNombre;
    public Button botonAceptar;
    public GameObject panelPopUp;
    public GameObject[] fotosPerfil; 
    
    [Header("UI Menu Principal")]
    public TMP_Text textoNombreMenu;

    [Header("Efectos de Transición")]
    public CanvasGroup cortinaNegra; // Arrastra aquí tu CortinaNegra
    public float velocidadTransicion = 3f; // Qué tan rápido se oscurece

    private int iconoSeleccionado = -1;

    void Start()
    {
        // Al iniciar el juego, mostramos el nombre si ya existe
        ActualizarTextoMenu();
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

    // Función auxiliar para mostrar el nombre en el menú principal
    void ActualizarTextoMenu()
    {
        if (PlayerPrefs.HasKey("NombreJugador"))
        {
            string nombre = PlayerPrefs.GetString("NombreJugador");
            textoNombreMenu.text = "Soldado: " + nombre;
        }
        else
        {
            textoNombreMenu.text = "";
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
}