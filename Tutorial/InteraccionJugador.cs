using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro; 

public class InteraccionJugador : MonoBehaviour
{
    [Header("Sprites de Iconos")]
    public Sprite spriteBala;
    public Sprite spritePico;
    public Sprite spriteHacha;
    public Sprite spriteManzana; // Para la comida
    public Sprite spriteMano;

    [Header("Referencias UI")]
    public Image iconoBotonAccion;
    public Image iconoBotonInteractuar;
    public Image imagenCooldown;

    [Header("Configuración de Distancia")]
    public float distanciaInteraccion = 5f;
    public LayerMask capaInteractuable;

    // 4 Slots fijos (¡Añadimos 'Ninguno' para las manos vacías!)
    public enum TipoHerramienta { Ninguno, Comida, Hacha, Pico, ArmaFuego, CuerpoACuerpo }

    [Header("Desbloqueos (Tutorial)")]
    public bool tieneHacha = false;
    public bool tienePico = false;
    public bool tieneCuchillo = false;
    public bool tieneArmaFuego = false;
    
    [Header("Cinturón de Herramientas")]
    public TipoHerramienta herramientaActual = TipoHerramienta.ArmaFuego;

    [Header("UI del Cinturón")]
    public Image[] slotsCinturon; // Ahora arrastrarás 4 slots aquí
    public Color colorSeleccionado = Color.yellow;
    public Color colorNormal = Color.white;
    public float escalaSeleccionado = 1.2f;

    [Header("Configuración de Cooldowns")]
    public float cooldownRecursos = 0.9f; 
    public float cooldownArma = 0.3f; 
    
    private float tiempoSiguienteAccion = 0f;
    private float cooldownActualUsado = 0.1f; 
    private InventarioJugador inventario;

    [Header("Efecto de Retroceso (Recoil) y Suavizado")]
    public Transform camaraTransform;   
    public float intensidadRetroceso = 0.05f; 
    public float velocidadRecuperacion = 5f;
    public float velocidadSuavizadoArma = 10f;
    private Vector3 posicionOriginalCamara;

    // ¡NUEVA VARIABLE! Para saber si tenemos el dedo pegado a la pantalla
    private bool manteniendoBotonAccion = false;

    [Header("Modelos 3D de Herramientas")]
    public GameObject objetoComida;   // El modelo 3D del pan o carne
    public GameObject objetoHacha;    
    public GameObject objetoPico;     
    public GameObject objetoArma;     // El modelo 3D del arma en las manos
    public GameObject objetoCuerpoACuerpo;

    [Header("Posiciones Hacha")]
    public Vector3 posEsperaHacha; public Vector3 rotEsperaHacha;
    public Vector3 posAccionHacha; public Vector3 rotAccionHacha;

    [Header("Posiciones Pico")]
    public Vector3 posEsperaPico; public Vector3 rotEsperaPico;
    public Vector3 posAccionPico; public Vector3 rotAccionPico;
    
    private GameObject modeloActivo;

    void Start()
    {
        inventario = GetComponentInParent<InventarioJugador>();
        
        // Forzamos a que inicies sin nada equipado
        herramientaActual = TipoHerramienta.Ninguno;

        // Apagamos visualmente todos los botones del cinturón (slots) al iniciar el juego
        for (int i = 0; i < slotsCinturon.Length; i++)
        {
            if (slotsCinturon[i] != null)
            {
                slotsCinturon[i].gameObject.SetActive(false);
            }
        }

        CambiarHerramienta(-1); // Un índice negativo asegura que todos los modelos 3D se apaguen al inicio

        if (camaraTransform != null)
        {
            posicionOriginalCamara = camaraTransform.localPosition;
        }
    }

    // Memoria de armas para el intercambio táctil
    private TipoHerramienta armaEnSlotPrincipal = TipoHerramienta.ArmaFuego;
    private TipoHerramienta armaEnSlotSecundario = TipoHerramienta.CuerpoACuerpo;

    public void CambiarHerramienta(int indiceSlot)
    {
        // 1. Asignamos la herramienta dependiendo de qué botón del cinturón tocaste
        if (indiceSlot == 0) herramientaActual = TipoHerramienta.Comida;
        else if (indiceSlot == 1) herramientaActual = TipoHerramienta.Hacha;
        else if (indiceSlot == 2) herramientaActual = TipoHerramienta.Pico;
        else if (indiceSlot == 3) herramientaActual = armaEnSlotPrincipal;
        else if (indiceSlot == 4) return; // El slot 4 (arriba) no se puede "equipar" directo, solo se intercambia

        // 2. Cambiamos el icono del botón de acción (Blindado)
        if (iconoBotonAccion != null)
        {
            if (herramientaActual == TipoHerramienta.Comida) iconoBotonAccion.sprite = spriteManzana;
            else if (herramientaActual == TipoHerramienta.Hacha) iconoBotonAccion.sprite = spriteHacha;
            else if (herramientaActual == TipoHerramienta.Pico) iconoBotonAccion.sprite = spritePico;
            else if (herramientaActual == TipoHerramienta.ArmaFuego) iconoBotonAccion.sprite = spriteBala;
            else if (herramientaActual == TipoHerramienta.CuerpoACuerpo) iconoBotonAccion.sprite = spriteMano; 
        }

        // 3. Resaltamos visualmente el slot
        for (int i = 0; i < slotsCinturon.Length; i++)
        {
            if (slotsCinturon[i] == null) continue; 

            if (i == indiceSlot)
            {
                slotsCinturon[i].color = colorSeleccionado;
                slotsCinturon[i].rectTransform.localScale = Vector3.one * escalaSeleccionado;
            }
            else
            {
                slotsCinturon[i].color = colorNormal;
                slotsCinturon[i].rectTransform.localScale = Vector3.one;
            }
        }

        // 4. Activamos el modelo 3D correcto
        if (objetoComida != null) objetoComida.SetActive(herramientaActual == TipoHerramienta.Comida);
        if (objetoHacha != null) objetoHacha.SetActive(herramientaActual == TipoHerramienta.Hacha);
        if (objetoPico != null) objetoPico.SetActive(herramientaActual == TipoHerramienta.Pico);
        if (objetoArma != null) objetoArma.SetActive(herramientaActual == TipoHerramienta.ArmaFuego);
        if (objetoCuerpoACuerpo != null) objetoCuerpoACuerpo.SetActive(herramientaActual == TipoHerramienta.CuerpoACuerpo);

        // Guardamos el modelo activo
        if (herramientaActual == TipoHerramienta.Comida) modeloActivo = objetoComida;
        else if (herramientaActual == TipoHerramienta.Hacha) modeloActivo = objetoHacha;
        else if (herramientaActual == TipoHerramienta.Pico) modeloActivo = objetoPico;
        else if (herramientaActual == TipoHerramienta.ArmaFuego) modeloActivo = objetoArma;
        else if (herramientaActual == TipoHerramienta.CuerpoACuerpo) modeloActivo = objetoCuerpoACuerpo;
    }

    public void TocarBotonCinturon(int indiceBoton)
    {
        // Si tocamos el arma secundaria (botón 5, índice 4)
        if (indiceBoton == 4)
        {
            // 1. Intercambiamos la memoria interna de las armas
            TipoHerramienta temporal = armaEnSlotPrincipal;
            armaEnSlotPrincipal = armaEnSlotSecundario;
            armaEnSlotSecundario = temporal;

            // 2. Intercambiamos las imágenes para el efecto visual
            if (slotsCinturon.Length > 4 && slotsCinturon[3] != null && slotsCinturon[4] != null)
            {
                Sprite imagenTemporal = slotsCinturon[3].sprite;
                slotsCinturon[3].sprite = slotsCinturon[4].sprite;
                slotsCinturon[4].sprite = imagenTemporal;
            }
            
            // 3. Forzamos a que equipe lo que quedó en el slot principal
            indiceBoton = 3;
        }

        CambiarHerramienta(indiceBoton);
    }

    void Update()
    {
        // 1. Rayo invisible para detectar qué estamos mirando
        RaycastHit hit;
        bool estaApuntando = Physics.Raycast(transform.position, transform.forward, out hit, distanciaInteraccion, capaInteractuable);

        if (iconoBotonInteractuar != null)
        {
            bool viendoInteractuable = estaApuntando && hit.collider.CompareTag("Interactuable");
            iconoBotonInteractuar.gameObject.SetActive(viendoInteractuable);
        }

        // 2. Suavizado de cámara 
        if (camaraTransform != null)
        {
            camaraTransform.localPosition = Vector3.Lerp(camaraTransform.localPosition, posicionOriginalCamara, Time.deltaTime * velocidadRecuperacion);
        }

        // 3. Posicionamiento dinámico 
        if (modeloActivo != null)
        {
            Vector3 destinoPos = Vector3.zero;
            Vector3 destinoRot = Vector3.zero;
            float velocidadActual = velocidadSuavizadoArma; 

            if (herramientaActual == TipoHerramienta.Hacha)
            {
                bool enGolpe = Time.time < (tiempoSiguienteAccion - (cooldownRecursos / 2f));
                destinoPos = enGolpe ? posAccionHacha : posEsperaHacha;
                destinoRot = enGolpe ? rotAccionHacha : rotEsperaHacha;
                velocidadActual = 10f / cooldownRecursos;
            }
            else if (herramientaActual == TipoHerramienta.Pico)
            {
                bool enGolpe = Time.time < (tiempoSiguienteAccion - (cooldownRecursos / 2f));
                destinoPos = enGolpe ? posAccionPico : posEsperaPico;
                destinoRot = enGolpe ? rotAccionPico : rotEsperaPico;
                velocidadActual = 10f / cooldownRecursos;
            }
            else if (herramientaActual == TipoHerramienta.ArmaFuego)
            {
                ControladorArmas armaActiva = modeloActivo.GetComponent<ControladorArmas>();
                if (armaActiva != null)
                {
                    destinoPos = manteniendoBotonAccion ? armaActiva.posDisparo : armaActiva.posEspera;
                    destinoRot = manteniendoBotonAccion ? armaActiva.rotDisparo : armaActiva.rotEspera;
                    velocidadActual = manteniendoBotonAccion ? (velocidadSuavizadoArma * 3f) : velocidadSuavizadoArma;
                }
            }
            else if (herramientaActual == TipoHerramienta.CuerpoACuerpo)
            {
                ControladorCuerpoACuerpo armaMelee = modeloActivo.GetComponent<ControladorCuerpoACuerpo>();
                if (armaMelee != null)
                {
                    bool enGolpe = Time.time < (tiempoSiguienteAccion - (armaMelee.cadenciaAtaque / 2f));
                    destinoPos = enGolpe ? armaMelee.posAccion : armaMelee.posEspera;
                    destinoRot = enGolpe ? armaMelee.rotAccion : armaMelee.rotEspera;
                    velocidadActual = 10f / armaMelee.cadenciaAtaque; 
                }
            }
            // ¡NUEVO! Posicionamiento de la Comida
            else if (herramientaActual == TipoHerramienta.Comida)
            {
                ControladorConsumibles comidaActiva = modeloActivo.GetComponent<ControladorConsumibles>();
                if (comidaActiva != null)
                {
                    // Si el relojito está girando, significa que la está consumiendo
                    bool enConsumo = Time.time < tiempoSiguienteAccion;
                    destinoPos = enConsumo ? comidaActiva.posAccion : comidaActiva.posEspera;
                    destinoRot = enConsumo ? comidaActiva.rotAccion : comidaActiva.rotEspera;
                    velocidadActual = 10f / comidaActiva.tiempoConsumo;
                }
            }

            modeloActivo.transform.localPosition = Vector3.Lerp(modeloActivo.transform.localPosition, destinoPos, Time.deltaTime * velocidadActual);
            modeloActivo.transform.localRotation = Quaternion.Slerp(modeloActivo.transform.localRotation, Quaternion.Euler(destinoRot), Time.deltaTime * velocidadActual);
        }

        // 4. Animación circular del Cooldown
        if (imagenCooldown != null)
        {
            if (Time.time < tiempoSiguienteAccion)
            {
                float tiempoRestante = tiempoSiguienteAccion - Time.time;
                imagenCooldown.fillAmount = tiempoRestante / cooldownActualUsado;
            }
            else
            {
                imagenCooldown.fillAmount = 0;
            }
        }

        // 5. ¡MAGIA DE DISPARO Y ATAQUE AUTOMÁTICO!
        if (manteniendoBotonAccion)
        {
            if (herramientaActual == TipoHerramienta.ArmaFuego && modeloActivo != null)
            {
                ControladorArmas armaActiva = modeloActivo.GetComponent<ControladorArmas>();
                if (armaActiva != null) armaActiva.IntentarDisparar(); 
            }
            else if (herramientaActual == TipoHerramienta.CuerpoACuerpo && modeloActivo != null)
            {
                ControladorCuerpoACuerpo armaMelee = modeloActivo.GetComponent<ControladorCuerpoACuerpo>();
                if (armaMelee != null)
                {
                    armaMelee.IntentarAtacar(capaInteractuable, camaraTransform);
                    
                    if (Time.time >= tiempoSiguienteAccion)
                    {
                        AplicarCooldown(armaMelee.cadenciaAtaque);
                    }
                }
            }
            // ¡NUEVO! Comer automáticamente si mantienes el botón
            else if (herramientaActual == TipoHerramienta.Comida && modeloActivo != null)
            {
                ControladorConsumibles comidaActiva = modeloActivo.GetComponent<ControladorConsumibles>();
                if (comidaActiva != null)
                {
                    comidaActiva.IntentarConsumir();
                }
            }
        }
    }

    // --- NUEVAS FUNCIONES PARA LA PANTALLA TÁCTIL ---

    public void PresionarBotonAccion()
    {
        manteniendoBotonAccion = true;

        // Lógica de talar y picar (Solo se ejecuta una vez al tocar)
        if (herramientaActual != TipoHerramienta.ArmaFuego && Time.time >= tiempoSiguienteAccion)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, distanciaInteraccion, capaInteractuable))
            {
                if (herramientaActual == TipoHerramienta.Hacha && hit.collider.CompareTag("Recurso_Madera"))
                {
                    inventario.AgregarRecurso("Madera", 1);
                    AplicarCooldown(cooldownRecursos);
                }
                else if (herramientaActual == TipoHerramienta.Pico && hit.collider.CompareTag("Recurso_Piedra"))
                {
                    inventario.AgregarRecurso("Piedra", 1);
                    AplicarCooldown(cooldownRecursos);
                }
            }
        }
    }

    public void SoltarBotonAccion()
    {
        manteniendoBotonAccion = false;
    }

    // Función auxiliar que ya tenías
    public void AplicarCooldown(float tiempo)
    {
        cooldownActualUsado = tiempo;
        tiempoSiguienteAccion = Time.time + tiempo;
    }

    // --- EFECTOS VISUALES ---
    public void AplicarGolpeRetroceso()
    {
        if (camaraTransform != null)
        {
            // Da un pequeño "golpe" hacia atrás a la cámara simulando la patada del arma
            camaraTransform.localPosition -= Vector3.forward * intensidadRetroceso;
        }
    }
}