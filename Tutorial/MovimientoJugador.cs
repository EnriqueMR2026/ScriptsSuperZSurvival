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

        // ¡NUEVO! Lógica para PC (Ratón oculto y bloqueado, FPS Clásico)
        if (Mouse.current != null && Cursor.lockState == CursorLockMode.Locked)
        {
            // Multiplicamos por 0.1f para que no gire rapidísimo en la compu
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

        // 4. ATAJOS DE TECLADO PARA PRUEBAS (Espacio, E, Q, R)
        if (Keyboard.current != null)
        {
            // Sacamos el script de InteraccionJugador
            InteraccionJugador interaccion = GetComponent<InteraccionJugador>();

            // BOTÓN ACCIÓN (Espacio) - Simula dejar el dedo en la pantalla y soltarlo
            if (Keyboard.current.spaceKey.wasPressedThisFrame && interaccion != null)
            {
                interaccion.PresionarBotonAccion();
            }
            if (Keyboard.current.spaceKey.wasReleasedThisFrame && interaccion != null)
            {
                interaccion.SoltarBotonAccion();
            }

            // BOTÓN INTERACTUAR (E) - La manita
            if (Keyboard.current.eKey.wasPressedThisFrame && interaccion != null)
            {
                interaccion.PresionarBotonInteractuar();
            }

            // BOTÓN MISIONES (Q) - Abrir/Cerrar librito
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                PanelObjetivos panel = FindFirstObjectByType<PanelObjetivos>();
                if (panel != null)
                {
                    // Si el panel grande está encendido, lo cerramos
                    if (panel.panelPrincipal.gameObject.activeSelf)
                    {
                        panel.BotonCerrarPresionado();
                    }
                    else
                    {
                        panel.BotonLibritoPresionado();
                    }
                }
            }

            // ¡NUEVO! BOTÓN RECARGAR (R)
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                // Buscamos si el jugador tiene un arma en las manos y la recargamos
                ControladorArmas armaActiva = GetComponentInChildren<ControladorArmas>();
                if (armaActiva != null)
                {
                    armaActiva.IniciarRecarga();
                }
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