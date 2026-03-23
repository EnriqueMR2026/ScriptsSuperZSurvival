using UnityEngine;

public class RotadorUI : MonoBehaviour
{
    [Header("Configuración del Engranaje")]
    [Tooltip("Qué tan rápido da vueltas")]
    public float velocidadRotacion = 50f;
    
    [Tooltip("Palomeado = Gira como reloj (+). Apagado = Gira al revés (-)")]
    public bool girarComoReloj = true; 

    void Update()
    {
        // En la interfaz de Unity (2D), los números negativos giran hacia la derecha (reloj)
        float direccion = girarComoReloj ? -1f : 1f; 
        
        // Hacemos que gire suavemente sin importar si el celular es rápido o lento
        transform.Rotate(0f, 0f, velocidadRotacion * direccion * Time.deltaTime);
    }
}