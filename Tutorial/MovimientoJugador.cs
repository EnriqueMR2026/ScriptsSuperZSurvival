using UnityEngine;
using UnityEngine.InputSystem;

public class MovimientoJugador : MonoBehaviour
{
    private CharacterController controller;
    public Transform camaraJugador;
    
    public float velocidad = 5f;
    public float sensibilidadVista = 75f;
    
    private float rotacionX = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 1. LECTURA DE MOVIMIENTO (Joystick y Teclado)
        Vector2 inputMovimiento = Vector2.zero;
        
        if (Gamepad.current != null)
        {
            inputMovimiento = Gamepad.current.leftStick.ReadValue();
        }

        // Teclado (AWSD) para probar en tu laptop
        if (Keyboard.current != null)
        {
            float tecladoX = 0f;
            float tecladoY = 0f;
            if (Keyboard.current.wKey.isPressed) tecladoY += 1f;
            if (Keyboard.current.sKey.isPressed) tecladoY -= 1f;
            if (Keyboard.current.dKey.isPressed) tecladoX += 1f;
            if (Keyboard.current.aKey.isPressed) tecladoX -= 1f;

            // Si tocaste el teclado, sobrescribe al joystick
            if (tecladoX != 0 || tecladoY != 0)
            {
                inputMovimiento = new Vector2(tecladoX, tecladoY).normalized;
            }
        }

        // 2. APLICAR MOVIMIENTO (Cuerpo físico)
        Vector3 movimiento = transform.right * inputMovimiento.x + transform.forward * inputMovimiento.y;
        controller.Move(movimiento * velocidad * Time.deltaTime);
        controller.Move(new Vector3(0, -9.81f * Time.deltaTime, 0));

        // 3. LECTURA DE VISTA (Pantalla táctil o Mouse PC)
        Vector2 inputVista = Vector2.zero;

        // Lógica para PC (Ratón oculto y bloqueado, FPS Clásico)
        if (Mouse.current != null && Cursor.lockState == CursorLockMode.Locked)
        {
            inputVista = Mouse.current.delta.ReadValue() * 0.1f; 
        }
        // Lógica para Celular (Tocar y arrastrar lado derecho)
        else if (Pointer.current != null && Pointer.current.press.isPressed)
        {
            if (Pointer.current.position.ReadValue().x > Screen.width / 2f)
            {
                inputVista = Pointer.current.delta.ReadValue();
            }
        }

        rotacionX -= inputVista.y * sensibilidadVista * Time.deltaTime;
        rotacionX = Mathf.Clamp(rotacionX, -90f, 90f);
        camaraJugador.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);

        transform.Rotate(Vector3.up * inputVista.x * sensibilidadVista * Time.deltaTime);

        // 4. ATAJOS DE TECLADO PARA PRUEBAS
        InteraccionJugador interaccion = GetComponent<InteraccionJugador>();

        if (Keyboard.current != null)
        {
            // BOTÓN ACCIÓN (Espacio)
            if (Keyboard.current.spaceKey.wasPressedThisFrame && interaccion != null) interaccion.PresionarBotonAccion();
            if (Keyboard.current.spaceKey.wasReleasedThisFrame && interaccion != null) interaccion.SoltarBotonAccion();

            // BOTÓN INTERACTUAR (E)
            if (Keyboard.current.eKey.wasPressedThisFrame && interaccion != null) interaccion.PresionarBotonInteractuar();

            // BOTÓN MISIONES (Q)
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                PanelObjetivos panel = FindFirstObjectByType<PanelObjetivos>();
                if (panel != null)
                {
                    if (panel.panelPrincipal.gameObject.activeSelf) panel.BotonCerrarPresionado();
                    else panel.BotonLibritoPresionado();
                }
            }

            // BOTÓN RECARGAR (R)
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                ControladorArmas armaActiva = GetComponentInChildren<ControladorArmas>();
                if (armaActiva != null) armaActiva.IniciarRecarga();
            }

            // ¡NUEVO! BOTÓN PERSIANA (Shift)
            if (Keyboard.current.leftShiftKey.wasPressedThisFrame && interaccion != null)
            {
                // Le mandamos el aviso seguro para alternar las opciones del cinturón
                interaccion.SendMessage("AlternarOpcionesPersiana", SendMessageOptions.DontRequireReceiver);
            }
        }

        // ¡NUEVO! CONTROLES DEL RATÓN (Clic Izquierdo y Rueda)
        if (Mouse.current != null && Cursor.lockState == CursorLockMode.Locked && interaccion != null)
        {
            // CLIC IZQUIERDO (Atacar / Disparar y ocultar persiana)
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                interaccion.PresionarBotonAccion();
                // Ocultamos la persiana al realizar una acción
                interaccion.SendMessage("OcultarPersiana", SendMessageOptions.DontRequireReceiver);
            }
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                interaccion.SoltarBotonAccion();
            }

            // RUEDA DEL RATÓN (Cambiar Slot del Cinturón)
            float scroll = Mouse.current.scroll.y.ReadValue();
            if (scroll != 0)
            {
                int nuevoSlot = interaccion.slotActual;
                
                // Si la rueda va hacia arriba, restamos un índice; si va hacia abajo, lo sumamos
                if (scroll > 0) nuevoSlot--; 
                if (scroll < 0) nuevoSlot++; 

                // Matemáticas para que el cinturón dé la vuelta infinita (0 a 4)
                if (nuevoSlot > 4) nuevoSlot = 0;
                if (nuevoSlot < 0) nuevoSlot = 4;

                interaccion.CambiarHerramienta(nuevoSlot);
            }
        }
    }

    // ¡NUEVA FUNCIÓN MÁGICA! Para forzar la vista sin que el Update nos pelee
    public void ForzarRotacion(float rotacionYDelCuerpo, float rotacionXDeLaCamara)
    {
        // 1. Giramos el cuerpo entero (Y)
        transform.rotation = Quaternion.Euler(0f, rotacionYDelCuerpo, 0f);

        // 2. Reseteamos la memoria interna de la cámara para que no dé saltos
        rotacionX = rotacionXDeLaCamara;

        // 3. Forzamos la cámara a mirar exactamente a donde le decimos (X)
        if (camaraJugador != null)
        {
            camaraJugador.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);
        }
    }
}