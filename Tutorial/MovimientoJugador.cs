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
        Vector2 inputMovimiento = Vector2.zero;
        if (Gamepad.current != null)
        {
            inputMovimiento = Gamepad.current.leftStick.ReadValue();
        }

        Vector3 movimiento = transform.right * inputMovimiento.x + transform.forward * inputMovimiento.y;
        controller.Move(movimiento * velocidad * Time.deltaTime);

        controller.Move(new Vector3(0, -9.81f * Time.deltaTime, 0));

        Vector2 inputVista = Vector2.zero;
        if (Pointer.current != null && Pointer.current.press.isPressed)
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
    }
}