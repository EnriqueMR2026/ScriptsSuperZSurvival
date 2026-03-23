using UnityEngine;
using UnityEngine.UI;

public class AnimadorGIF : MonoBehaviour
{
    [Header("Pon aquí todas las fotos de tu GIF")]
    public Sprite[] frames;
    
    [Header("¿Qué tan rápido se mueve?")]
    public float cuadrosPorSegundo = 15f;
    
    private Image imagenUI;

    void Start()
    {
        imagenUI = GetComponent<Image>();
    }

    void Update()
    {
        if (frames.Length > 0 && imagenUI != null)
        {
            // Magia matemática que decide qué foto mostrar según el reloj del juego
            int indice = (int)(Time.time * cuadrosPorSegundo) % frames.Length;
            imagenUI.sprite = frames[indice];
        }
    }
}