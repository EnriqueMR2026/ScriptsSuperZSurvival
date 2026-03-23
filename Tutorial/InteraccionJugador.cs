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

    public void CambiarHerramienta(int indiceSlot)
    {
        // 0. ¡CANDADOS DE DESBLOQUEO! Verificamos si tenemos la herramienta antes de intentar sacarla
        if (indiceSlot == 1 && !tieneHacha) return;
        if (indiceSlot == 2 && !tienePico) return;
        if (indiceSlot == 3)
        {
            if (armaEnSlotPrincipal == TipoHerramienta.ArmaFuego && !tieneArmaFuego) return;
            if (armaEnSlotPrincipal == TipoHerramienta.CuerpoACuerpo && !tieneCuchillo) return;
        }

        // 1. Asignamos la herramienta
        if (indiceSlot == -1) herramientaActual = TipoHerramienta.Ninguno; 
        else if (indiceSlot == 0) herramientaActual = TipoHerramienta.Comida;
        else if (indiceSlot == 1) herramientaActual = TipoHerramienta.Hacha;
        else if (indiceSlot == 2) herramientaActual = TipoHerramienta.Pico;
        else if (indiceSlot == 3) herramientaActual = armaEnSlotPrincipal;
        else if (indiceSlot == 4) return; 

        // 2. Activamos el modelo 3D correcto (¡Lo movemos arriba para poder leer su DNI!)
        if (objetoComida != null) objetoComida.SetActive(herramientaActual == TipoHerramienta.Comida);
        if (objetoHacha != null) objetoHacha.SetActive(herramientaActual == TipoHerramienta.Hacha);
        if (objetoPico != null) objetoPico.SetActive(herramientaActual == TipoHerramienta.Pico);
        if (objetoArma != null) objetoArma.SetActive(herramientaActual == TipoHerramienta.ArmaFuego);
        if (objetoCuerpoACuerpo != null) objetoCuerpoACuerpo.SetActive(herramientaActual == TipoHerramienta.CuerpoACuerpo);

        if (herramientaActual == TipoHerramienta.Comida) modeloActivo = objetoComida;
        else if (herramientaActual == TipoHerramienta.Hacha) modeloActivo = objetoHacha;
        else if (herramientaActual == TipoHerramienta.Pico) modeloActivo = objetoPico;
        else if (herramientaActual == TipoHerramienta.ArmaFuego) modeloActivo = objetoArma;
        else if (herramientaActual == TipoHerramienta.CuerpoACuerpo) modeloActivo = objetoCuerpoACuerpo;
        else if (herramientaActual == TipoHerramienta.Ninguno) modeloActivo = null;

        // 3. ¡MAGIA DEL DNI! Leemos los datos del modelo que acabamos de encender
        if (iconoBotonAccion != null)
        {
            // Ocultamos el botón de acción por completo si tenemos las manos vacías (¡Soluciona tu problema del engranaje flotante!)
            // Nota: Para que el engranaje desaparezca, asegúrate de que 'iconoBotonAccion' en Unity sea el objeto PADRE que tiene todo el botón.
            iconoBotonAccion.transform.parent.gameObject.SetActive(herramientaActual != TipoHerramienta.Ninguno);
            
            Sprite fotoAccion = spriteMano; // Por defecto
            bool requiereBalas = false;

            if (modeloActivo != null)
            {
                // Buscamos cuál de los 4 scripts tiene puesto para robarle su DNI
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
            
            // ¡El DNI decide si aparece el botón de recargar completo! (Asegúrate de arrastrar el PADRE a botonRecargarObj)
            if (botonRecargarObj != null) botonRecargarObj.SetActive(requiereBalas);
        }

        // 4. Resaltamos visualmente el slot
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
            armaEnSlotSecundario = TipoHerramienta.CuerpoACuerpo; // ¡CORRECCIÓN! Va a tu slot secundario (4)
            
            // Buscamos tu cuchillo en la primera posición de tu mochila de armas secundarias para robarle su DNI
            if (listaArmasSecundarias.Length > 0 && listaArmasSecundarias[0] != null)
            {
                ControladorCuerpoACuerpo scriptCuchillo = listaArmasSecundarias[0].GetComponent<ControladorCuerpoACuerpo>();
                if (scriptCuchillo != null && slotsCinturon.Length > 4 && slotsCinturon[4] != null)
                {
                    slotsCinturon[4].sprite = scriptCuchillo.iconoCinturon;
                    slotsCinturon[4].gameObject.SetActive(true);
                }
            }
            
            // Forzamos que lo equipes simulando que tocaste el slot 4 de tu pantalla
            TocarBotonCinturonNormal(4);
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
        // Si tocaste el slot de las armas (3 o 4), empezamos a contar el tiempo
        if (indiceBoton == 3 || indiceBoton == 4)
        {
            slotMantenido = indiceBoton;
            persianaAbierta = false;
            
            if (rutinaCronometro != null) StopCoroutine(rutinaCronometro);
            rutinaCronometro = StartCoroutine(CronometroPersiana(indiceBoton));
        }
        else 
        {
            // Si tocaste comida, hacha o pico (0, 1, 2), se equipan rapidísimo
            TocarBotonCinturonNormal(indiceBoton);
        }
    }

    public void SoltarSlotCinturon(int indiceBoton)
    {
        if (slotMantenido == indiceBoton)
        {
            // Apagamos el cronómetro porque ya quitaste el dedo de la pantalla
            if (rutinaCronometro != null) StopCoroutine(rutinaCronometro);
            
            // Si soltaste el dedo rápido (antes de que se abriera la persiana), cuenta como toque normal
            if (!persianaAbierta)
            {
                TocarBotonCinturonNormal(indiceBoton);
            }
            
            slotMantenido = -1; // Reiniciamos el sensor
        }
    }

    private IEnumerator CronometroPersiana(int indice)
    {
        // El juego espera pacientemente los 0.4 segundos que le configuramos
        yield return new WaitForSeconds(tiempoParaDesplegar);
        
        // Si no has soltado el dedo, ¡se abre la magia visual!
        persianaAbierta = true;
        AbrirPersiana(indice);
    }

    private void AbrirPersiana(int indiceSlot)
    {
        // 1. Elegimos qué panel y qué lista de tu mochila vamos a usar
        GameObject panelAUsar = (indiceSlot == 3) ? panelPersianaPrincipal : panelPersianaSecundaria;
        Image[] botonesAUsar = (indiceSlot == 3) ? botonesPersianaPrincipal : botonesPersianaSecundaria;
        GameObject[] listaAUsar = (indiceSlot == 3) ? listaArmasPrincipales : listaArmasSecundarias;

        // 2. Apagamos ambos por precaución, y encendemos solo el que pediste
        if (panelPersianaPrincipal != null) panelPersianaPrincipal.SetActive(false);
        if (panelPersianaSecundaria != null) panelPersianaSecundaria.SetActive(false);
        
        if (panelAUsar != null) panelAUsar.SetActive(true);
        
        // 3. ¡LA MAGIA DEL DNI! Llenamos los botoncitos de ese panel
        for (int i = 0; i < botonesAUsar.Length; i++)
        {
            if (i < listaAUsar.Length && listaAUsar[i] != null)
            {
                botonesAUsar[i].gameObject.SetActive(true);
                
                ControladorArmas armaFuego = listaAUsar[i].GetComponent<ControladorArmas>();
                if (armaFuego != null) botonesAUsar[i].sprite = armaFuego.iconoCinturon;
                
                ControladorCuerpoACuerpo armaMelee = listaAUsar[i].GetComponent<ControladorCuerpoACuerpo>();
                if (armaMelee != null) botonesAUsar[i].sprite = armaMelee.iconoCinturon;
            }
            else
            {
                // Escondemos los botoncitos que sobren para que se vea limpio
                if (botonesAUsar[i] != null) botonesAUsar[i].gameObject.SetActive(false); 
            }
        }
    }

    // Esta es tu lógica original intacta, solo le cambiamos el nombre
    public void TocarBotonCinturonNormal(int indiceBoton)
    {
        if (indiceBoton == 4)
        {
            TipoHerramienta temporal = armaEnSlotPrincipal;
            armaEnSlotPrincipal = armaEnSlotSecundario;
            armaEnSlotSecundario = temporal;

            if (slotsCinturon.Length > 4 && slotsCinturon[3] != null && slotsCinturon[4] != null)
            {
                Sprite imagenTemporal = slotsCinturon[3].sprite;
                slotsCinturon[3].sprite = slotsCinturon[4].sprite;
                slotsCinturon[4].sprite = imagenTemporal;
            }
            indiceBoton = 3;
        }

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
            // 1. Apagamos todas las armas principales para que no se encimen modelos 3D
            foreach (GameObject arma in listaArmasPrincipales) { if (arma != null) arma.SetActive(false); }

            // 2. Asignamos la nueva arma como tu modelo activo
            objetoArma = listaArmasPrincipales[indiceArma];
            armaEnSlotPrincipal = TipoHerramienta.ArmaFuego;

            // 3. Leemos su DNI y actualizamos la foto del cinturón
            ControladorArmas script = objetoArma.GetComponent<ControladorArmas>();
            if (script != null && slotsCinturon.Length > 3 && slotsCinturon[3] != null) 
            {
                slotsCinturon[3].sprite = script.iconoCinturon;
            }

            // 4. Cerramos la persiana visualmente
            if (panelPersianaPrincipal != null) panelPersianaPrincipal.SetActive(false);
            persianaAbierta = false;
            
            // 5. Refrescamos tus manos para que aparezca el arma nueva de inmediato
            CambiarHerramienta(3); 
        }
    }

    public void ElegirArmaSecundaria(int indiceArma)
    {
        if (indiceArma < listaArmasSecundarias.Length && listaArmasSecundarias[indiceArma] != null)
        {
            // 1. Apagamos todas las armas secundarias
            foreach (GameObject arma in listaArmasSecundarias) { if (arma != null) arma.SetActive(false); }

            GameObject armaElegida = listaArmasSecundarias[indiceArma];

            // 2. Leemos su DNI para saber si es pistola o arma blanca y la asignamos
            if (armaElegida.GetComponent<ControladorArmas>() != null)
            {
                armaEnSlotSecundario = TipoHerramienta.ArmaFuego;
                objetoArma = armaElegida; 
                if (slotsCinturon.Length > 4 && slotsCinturon[4] != null) 
                    slotsCinturon[4].sprite = armaElegida.GetComponent<ControladorArmas>().iconoCinturon;
            }
            else if (armaElegida.GetComponent<ControladorCuerpoACuerpo>() != null)
            {
                armaEnSlotSecundario = TipoHerramienta.CuerpoACuerpo;
                objetoCuerpoACuerpo = armaElegida;
                if (slotsCinturon.Length > 4 && slotsCinturon[4] != null) 
                    slotsCinturon[4].sprite = armaElegida.GetComponent<ControladorCuerpoACuerpo>().iconoCinturon;
            }

            // 3. Cerramos la persiana
            if (panelPersianaSecundaria != null) panelPersianaSecundaria.SetActive(false);
            persianaAbierta = false;

            // 4. Forzamos el intercambio de armas para que se te ponga en las manos
            TocarBotonCinturonNormal(4);
        }
    }
}