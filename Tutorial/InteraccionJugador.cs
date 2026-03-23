using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class InteraccionJugador : MonoBehaviour
{
    [Header("Sprites por Defecto")]
    public Sprite spriteMano; 

    [Header("Referencias UI")]
    public Image iconoBotonAccion;
    public Image iconoBotonInteractuar;
    public Image imagenCooldown;

    [Header("Configuración de Distancia")]
    public float distanciaInteraccion = 5f;
    public LayerMask capaInteractuable;

    public enum TipoHerramienta { Ninguno, Comida, Hacha, Pico, ArmaFuego, CuerpoACuerpo }

    [Header("Desbloqueos (Tutorial)")]
    public bool tieneHacha = false;
    public bool tienePico = false;
    public bool tieneCuchillo = false;
    public bool tieneArmaFuego = false;
    
    [Header("Cinturón de Herramientas")]
    public TipoHerramienta herramientaActual = TipoHerramienta.ArmaFuego;

    [Header("UI del Cinturón")]
    public Image[] slotsCinturon; 
    public Color colorSeleccionado = Color.yellow;
    public Color colorNormal = Color.white;
    public float escalaSeleccionado = 1.2f;

    [Header("Persianas Desplegables (CORREGIDO)")]
    public GameObject panelPersianaPrincipal; // Arriba del Slot 3
    public Image[] botonesPersianaPrincipal; 
    
    public GameObject panelPersianaSecundaria; // Arriba del Slot 4
    public Image[] botonesPersianaSecundaria; 
    
    public float tiempoParaDesplegar = 0.4f;
    private int slotMantenido = -1;
    private float tiempoPresionando = 0f;
    private bool persianaAbierta = false;

    [Header("Botones Extras UI")]
    public GameObject botonRecargarObj;
    
    // Variables internas de control de tiempo
    private float tiempoSiguienteAccion = 0f;
    private float cooldownActualUsado = 0.1f; 
    private InventarioJugador inventario;

    [Header("Efecto de Retroceso (Recoil) y Suavizado")]
    public Transform camaraTransform;   
    public float intensidadRetroceso = 0.05f; 
    public float velocidadRecuperacion = 5f;
    public float velocidadSuavizadoArma = 10f;
    private Vector3 posicionOriginalCamara;

    // Para saber si tenemos el dedo pegado a la pantalla
    private bool manteniendoBotonAccion = false;

    [Header("Modelos 3D de Herramientas")]
    public GameObject objetoComida;   
    public GameObject objetoHacha;    
    public GameObject objetoPico;     
    public GameObject objetoArma;     
    public GameObject objetoCuerpoACuerpo;
    
    [Header("Arsenal de la Persiana")]
    public GameObject[] listaArmasPrincipales; // Aquí arrastrarás tu AK47, Uzi, M590
    public GameObject[] listaArmasSecundarias; // Aquí arrastrarás tu Cuchillo, M500, etc.

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
    
    // ¡NUEVO! Memoria visual para que nunca se confundan los modelos 3D
    private GameObject modeloEnSlot3;
    private GameObject modeloEnSlot4;
    private bool slotsInicializados = false;

    public void CambiarHerramienta(int indiceSlot)
    {
        // 0. Inicialización de seguridad (solo corre la primera vez)
        if (!slotsInicializados)
        {
            modeloEnSlot3 = objetoArma; 
            modeloEnSlot4 = objetoCuerpoACuerpo; 
            slotsInicializados = true;
        }

        // 1. CANDADOS DE DESBLOQUEO
        if (indiceSlot == 1 && !tieneHacha) return;
        if (indiceSlot == 2 && !tienePico) return;
        if (indiceSlot == 3)
        {
            if (armaEnSlotPrincipal == TipoHerramienta.ArmaFuego && !tieneArmaFuego) return;
            if (armaEnSlotPrincipal == TipoHerramienta.CuerpoACuerpo && !tieneCuchillo) return;
        }
        if (indiceSlot == 4)
        {
            if (armaEnSlotSecundario == TipoHerramienta.ArmaFuego && !tieneArmaFuego) return;
            if (armaEnSlotSecundario == TipoHerramienta.CuerpoACuerpo && !tieneCuchillo) return;
        }

        // 2. Asignamos la herramienta (¡YA NO IGNORAMOS EL 4!)
        if (indiceSlot == -1) herramientaActual = TipoHerramienta.Ninguno; 
        else if (indiceSlot == 0) herramientaActual = TipoHerramienta.Comida;
        else if (indiceSlot == 1) herramientaActual = TipoHerramienta.Hacha;
        else if (indiceSlot == 2) herramientaActual = TipoHerramienta.Pico;
        else if (indiceSlot == 3) herramientaActual = armaEnSlotPrincipal;
        else if (indiceSlot == 4) herramientaActual = armaEnSlotSecundario; 

        // 3. Activamos el modelo 3D correcto
        if (objetoComida != null) objetoComida.SetActive(herramientaActual == TipoHerramienta.Comida);
        if (objetoHacha != null) objetoHacha.SetActive(herramientaActual == TipoHerramienta.Hacha);
        if (objetoPico != null) objetoPico.SetActive(herramientaActual == TipoHerramienta.Pico);
        
        // Apagamos las armas actuales para evitar empalmes feos
        if (modeloEnSlot3 != null) modeloEnSlot3.SetActive(false);
        if (modeloEnSlot4 != null) modeloEnSlot4.SetActive(false);

        // Encendemos solo la que tocaste
        if (indiceSlot == 3 && modeloEnSlot3 != null) modeloEnSlot3.SetActive(true);
        if (indiceSlot == 4 && modeloEnSlot4 != null) modeloEnSlot4.SetActive(true);

        // Le decimos al código quién es el modelo activo para que lo mueva
        if (herramientaActual == TipoHerramienta.Comida) modeloActivo = objetoComida;
        else if (herramientaActual == TipoHerramienta.Hacha) modeloActivo = objetoHacha;
        else if (herramientaActual == TipoHerramienta.Pico) modeloActivo = objetoPico;
        else if (indiceSlot == 3) modeloActivo = modeloEnSlot3;
        else if (indiceSlot == 4) modeloActivo = modeloEnSlot4;
        else if (herramientaActual == TipoHerramienta.Ninguno) modeloActivo = null;

        // 4. MAGIA DEL DNI
        if (iconoBotonAccion != null)
        {
            iconoBotonAccion.transform.parent.gameObject.SetActive(herramientaActual != TipoHerramienta.Ninguno);
            Sprite fotoAccion = spriteMano; 
            bool requiereBalas = false;

            if (modeloActivo != null)
            {
                if (modeloActivo.GetComponent<ControladorArmas>() != null) {
                    fotoAccion = modeloActivo.GetComponent<ControladorArmas>().iconoBotonAccion;
                    requiereBalas = modeloActivo.GetComponent<ControladorArmas>().usaBalas;
                }
                else if (modeloActivo.GetComponent<ControladorCuerpoACuerpo>() != null) {
                    fotoAccion = modeloActivo.GetComponent<ControladorCuerpoACuerpo>().iconoBotonAccion;
                    requiereBalas = modeloActivo.GetComponent<ControladorCuerpoACuerpo>().usaBalas;
                }
                else if (modeloActivo.GetComponent<ControladorConsumibles>() != null) {
                    fotoAccion = modeloActivo.GetComponent<ControladorConsumibles>().iconoBotonAccion;
                    requiereBalas = modeloActivo.GetComponent<ControladorConsumibles>().usaBalas;
                }
                else if (modeloActivo.GetComponent<ControladorHerramientas>() != null) {
                    fotoAccion = modeloActivo.GetComponent<ControladorHerramientas>().iconoBotonAccion;
                    requiereBalas = modeloActivo.GetComponent<ControladorHerramientas>().usaBalas;
                }
            }

            iconoBotonAccion.sprite = fotoAccion;
            if (botonRecargarObj != null) botonRecargarObj.SetActive(requiereBalas);
        }

        // 5. Resaltamos visualmente el slot
        for (int i = 0; i < slotsCinturon.Length; i++)
        {
            if (slotsCinturon[i] == null) continue; 
            if (i == indiceSlot && indiceSlot != -1)
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
    }

    // ¡NUEVA FUNCIÓN! Para que la tienda o los objetos del suelo la llamen
    public void DesbloquearHerramienta(TipoHerramienta herramientaNueva)
    {
        if (herramientaNueva == TipoHerramienta.Hacha)
        {
            tieneHacha = true;
            if (slotsCinturon.Length > 1 && slotsCinturon[1] != null) slotsCinturon[1].gameObject.SetActive(true);
            CambiarHerramienta(1); // La equipamos de inmediato
        }
        else if (herramientaNueva == TipoHerramienta.CuerpoACuerpo)
        {
            tieneCuchillo = true;
            armaEnSlotSecundario = TipoHerramienta.CuerpoACuerpo; 
            
            // ¡MAGIA NUEVA! Guardamos tu cuchillo directo en la memoria del Slot 4
            if (listaArmasSecundarias.Length > 0 && listaArmasSecundarias[0] != null)
            {
                modeloEnSlot4 = listaArmasSecundarias[0]; 
                
                ControladorCuerpoACuerpo scriptCuchillo = listaArmasSecundarias[0].GetComponent<ControladorCuerpoACuerpo>();
                if (scriptCuchillo != null && slotsCinturon.Length > 4 && slotsCinturon[4] != null)
                {
                    slotsCinturon[4].sprite = scriptCuchillo.iconoCinturon;
                    slotsCinturon[4].gameObject.SetActive(true);
                }
            }
            
            // Forzamos el cambio limpio
            CambiarHerramienta(4);
        }
        else if (herramientaNueva == TipoHerramienta.Comida)
        {
            if (slotsCinturon.Length > 0 && slotsCinturon[0] != null) slotsCinturon[0].gameObject.SetActive(true);
            CambiarHerramienta(0);
        }
    }

    private Coroutine rutinaCronometro; // El motorcito de nuestro reloj táctil

    public void ApretarSlotCinturon(int indiceBoton)
    {
        if (indiceBoton == 3 || indiceBoton == 4)
        {
            // ¡NUEVO! Si la persiana ya está abierta, la cerramos y abortamos (para que te puedas arrepentir)
            if (persianaAbierta)
            {
                CerrarPersianas();
                return; 
            }

            slotMantenido = indiceBoton;
            persianaAbierta = false;
            
            if (rutinaCronometro != null) StopCoroutine(rutinaCronometro);
            rutinaCronometro = StartCoroutine(CronometroPersiana(indiceBoton));
        }
        else 
        {
            CerrarPersianas(); // Si tocas otra cosa, cerramos por limpieza
            TocarBotonCinturonNormal(indiceBoton);
        }
    }

    public void SoltarSlotCinturon(int indiceBoton)
    {
        if (slotMantenido == indiceBoton)
        {
            if (rutinaCronometro != null) StopCoroutine(rutinaCronometro);
            
            if (!persianaAbierta)
            {
                TocarBotonCinturonNormal(indiceBoton);
            }
            
            slotMantenido = -1; 
        }
    }

    private IEnumerator CronometroPersiana(int indice)
    {
        yield return new WaitForSeconds(tiempoParaDesplegar);
        persianaAbierta = true;
        AbrirPersiana(indice);
    }

    public void CerrarPersianas()
    {
        if (panelPersianaPrincipal != null) panelPersianaPrincipal.SetActive(false);
        if (panelPersianaSecundaria != null) panelPersianaSecundaria.SetActive(false);
        persianaAbierta = false;
    }

    private void AbrirPersiana(int indiceSlot)
    {
        GameObject panelAUsar = (indiceSlot == 3) ? panelPersianaPrincipal : panelPersianaSecundaria;
        Image[] botonesAUsar = (indiceSlot == 3) ? botonesPersianaPrincipal : botonesPersianaSecundaria;
        GameObject[] listaAUsar = (indiceSlot == 3) ? listaArmasPrincipales : listaArmasSecundarias;

        CerrarPersianas(); // Limpiamos antes de abrir
        if (panelAUsar != null) panelAUsar.SetActive(true);
        persianaAbierta = true;
        
        int botonActual = 0; // Para ir llenando los botones disponibles de abajo hacia arriba

        for (int i = 0; i < listaAUsar.Length; i++)
        {
            if (listaAUsar[i] != null)
            {
                // 1. Verificamos si esta arma es la que ya tienes equipada
                bool esArmaEquipada = false;
                if (indiceSlot == 3 && listaAUsar[i] == objetoArma && armaEnSlotPrincipal == TipoHerramienta.ArmaFuego) esArmaEquipada = true;
                if (indiceSlot == 4)
                {
                    if (armaEnSlotSecundario == TipoHerramienta.ArmaFuego && listaAUsar[i] == objetoArma) esArmaEquipada = true;
                    if (armaEnSlotSecundario == TipoHerramienta.CuerpoACuerpo && listaAUsar[i] == objetoCuerpoACuerpo) esArmaEquipada = true;
                }

                // 2. Si NO la traes puesta, la ponemos en la persiana
                if (!esArmaEquipada && botonActual < botonesAUsar.Length)
                {
                    botonesAUsar[botonActual].gameObject.SetActive(true);
                    
                    ControladorArmas armaFuego = listaAUsar[i].GetComponent<ControladorArmas>();
                    if (armaFuego != null) botonesAUsar[botonActual].sprite = armaFuego.iconoCinturon;
                    
                    ControladorCuerpoACuerpo armaMelee = listaAUsar[i].GetComponent<ControladorCuerpoACuerpo>();
                    if (armaMelee != null) botonesAUsar[botonActual].sprite = armaMelee.iconoCinturon;

                    // ¡MAGIA PURA! Le enseñamos al botón a qué arma llamar desde el código
                    int indiceCapturado = i; 
                    Button btn = botonesAUsar[botonActual].GetComponent<Button>();
                    if (btn != null)
                    {
                        btn.onClick.RemoveAllListeners(); // Borramos funciones viejas
                        if (indiceSlot == 3) btn.onClick.AddListener(() => ElegirArmaPrincipal(indiceCapturado));
                        else btn.onClick.AddListener(() => ElegirArmaSecundaria(indiceCapturado));
                    }

                    botonActual++; // Pasamos al siguiente botón visual
                }
            }
        }

        // 3. Escondemos los botones que sobren en blanco (si solo tenías 2 armas y equipaste 1, sobra 1 botón)
        for (int i = botonActual; i < botonesAUsar.Length; i++)
        {
            if (botonesAUsar[i] != null) botonesAUsar[i].gameObject.SetActive(false);
        }
    }

    // Esta es tu lógica original intacta, solo le cambiamos el nombre
    public void TocarBotonCinturonNormal(int indiceBoton)
    {
        // ¡ADIÓS AL CÓDIGO VIEJO DE INTERCAMBIO QUE ARRUINABA TODO!
        // Ahora el slot que tocas, es el slot que se equipa directo en la memoria nueva.
        CambiarHerramienta(indiceBoton);
    }

    void Update()
    {
        // 1. Rayo invisible para detectar qué estamos mirando
        RaycastHit hitUpdate;
        bool estaApuntando = Physics.Raycast(camaraTransform.position, camaraTransform.forward, out hitUpdate, distanciaInteraccion, capaInteractuable);

        if (iconoBotonInteractuar != null)
        {
            bool viendoInteractuable = estaApuntando && hitUpdate.collider.CompareTag("Interactuable");
            // ¡CORRECCIÓN! Apagamos al PADRE de la imagen (el engranaje) para que desaparezca todo
            iconoBotonInteractuar.transform.parent.gameObject.SetActive(viendoInteractuable);
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

            if (herramientaActual == TipoHerramienta.ArmaFuego)
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
            else if (herramientaActual == TipoHerramienta.Comida)
            {
                ControladorConsumibles comidaActiva = modeloActivo.GetComponent<ControladorConsumibles>();
                if (comidaActiva != null)
                {
                    bool enConsumo = Time.time < tiempoSiguienteAccion;
                    destinoPos = enConsumo ? comidaActiva.posAccion : comidaActiva.posEspera;
                    destinoRot = enConsumo ? comidaActiva.rotAccion : comidaActiva.rotEspera;
                    velocidadActual = 10f / comidaActiva.tiempoConsumo;
                }
            }
            // ¡EL CEREBRO NUEVO PARA TUS HERRAMIENTAS (HACHA/PICO)!
            else if (herramientaActual == TipoHerramienta.Hacha || herramientaActual == TipoHerramienta.Pico)
            {
                ControladorHerramientas herramienta = modeloActivo.GetComponent<ControladorHerramientas>();
                if (herramienta != null)
                {
                    bool enGolpe = Time.time < (tiempoSiguienteAccion - (herramienta.tiempoUso / 2f));
                    destinoPos = enGolpe ? herramienta.posAccion : herramienta.posEspera;
                    destinoRot = enGolpe ? herramienta.rotAccion : herramienta.rotEspera;
                    velocidadActual = 10f / herramienta.tiempoUso;
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
            else if (herramientaActual == TipoHerramienta.Comida && modeloActivo != null)
            {
                ControladorConsumibles comidaActiva = modeloActivo.GetComponent<ControladorConsumibles>();
                if (comidaActiva != null)
                {
                    comidaActiva.IntentarConsumir();
                }
            }
            // ¡LA CONEXIÓN LIMPIA PARA EL HACHA Y EL PICO!
            else if ((herramientaActual == TipoHerramienta.Hacha || herramientaActual == TipoHerramienta.Pico) && modeloActivo != null)
            {
                ControladorHerramientas herramienta = modeloActivo.GetComponent<ControladorHerramientas>();
                if (herramienta != null && Time.time >= tiempoSiguienteAccion)
                {
                    herramienta.IntentarUsar(capaInteractuable, camaraTransform, distanciaInteraccion, inventario);
                    AplicarCooldown(herramienta.tiempoUso);
                }
            }
        }
    }

    // --- NUEVAS FUNCIONES PARA LA PANTALLA TÁCTIL ---

    public void PresionarBotonAccion()
    {
        manteniendoBotonAccion = true;
    }

    public void SoltarBotonAccion()
    {
        manteniendoBotonAccion = false;
    }

    // ¡NUEVA FUNCIÓN! Para el botón táctil de la manita
    public void PresionarBotonInteractuar()
    {
        RaycastHit hit;
        // Disparamos desde la cámara
        if (Physics.Raycast(camaraTransform.position, camaraTransform.forward, out hit, distanciaInteraccion, capaInteractuable))
        {
            if (hit.collider.CompareTag("Interactuable"))
            {
                // 1. Intentamos sacar el script de ObjetoRecogible (El hacha)
                ObjetoRecogible objeto = hit.collider.GetComponent<ObjetoRecogible>();
                if (objeto != null)
                {
                    objeto.Interactuar(gameObject);
                }

                // 2. Intentamos abrir la tienda interactiva
                TiendaInteractiva tienda = hit.collider.GetComponent<TiendaInteractiva>();
                if (tienda != null)
                {
                    tienda.AbrirTienda();
                }

                // 3. Intentamos encender la fogata para la invasión
                FogataTutorial fogata = hit.collider.GetComponent<FogataTutorial>();
                if (fogata != null)
                {
                    fogata.Encender();
                }

                // ¡EL GRAN FINAL! Intentamos irnos a dormir
                CamaTutorial cama = hit.collider.GetComponent<CamaTutorial>();
                if (cama != null)
                {
                    cama.Dormir();
                }
            }
        }
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

    // --- NUEVAS FUNCIONES PARA LOS BOTONES DE LAS PERSIANAS ---

    public void ElegirArmaPrincipal(int indiceArma)
    {
        if (indiceArma < listaArmasPrincipales.Length && listaArmasPrincipales[indiceArma] != null)
        {
            foreach (GameObject arma in listaArmasPrincipales) { if (arma != null) arma.SetActive(false); }

            // ¡MAGIA NUEVA! Lo guardamos en la memoria exclusiva del Slot 3
            modeloEnSlot3 = listaArmasPrincipales[indiceArma];
            armaEnSlotPrincipal = TipoHerramienta.ArmaFuego;

            ControladorArmas script = modeloEnSlot3.GetComponent<ControladorArmas>();
            if (script != null && slotsCinturon.Length > 3 && slotsCinturon[3] != null) 
            {
                slotsCinturon[3].sprite = script.iconoCinturon;
            }

            CerrarPersianas();
            CambiarHerramienta(3); 
        }
    }

    public void ElegirArmaSecundaria(int indiceArma)
    {
        if (indiceArma < listaArmasSecundarias.Length && listaArmasSecundarias[indiceArma] != null)
        {
            foreach (GameObject arma in listaArmasSecundarias) { if (arma != null) arma.SetActive(false); }

            GameObject armaElegida = listaArmasSecundarias[indiceArma];

            // ¡MAGIA NUEVA! Lo guardamos en la memoria exclusiva del Slot 4
            modeloEnSlot4 = armaElegida;

            if (armaElegida.GetComponent<ControladorArmas>() != null)
            {
                armaEnSlotSecundario = TipoHerramienta.ArmaFuego;
                if (slotsCinturon.Length > 4 && slotsCinturon[4] != null) 
                    slotsCinturon[4].sprite = armaElegida.GetComponent<ControladorArmas>().iconoCinturon;
            }
            else if (armaElegida.GetComponent<ControladorCuerpoACuerpo>() != null)
            {
                armaEnSlotSecundario = TipoHerramienta.CuerpoACuerpo;
                if (slotsCinturon.Length > 4 && slotsCinturon[4] != null) 
                    slotsCinturon[4].sprite = armaElegida.GetComponent<ControladorCuerpoACuerpo>().iconoCinturon;
            }

            CerrarPersianas();
            CambiarHerramienta(4);
        }
    }
}