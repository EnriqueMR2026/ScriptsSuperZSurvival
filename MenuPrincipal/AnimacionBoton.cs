using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class AnimacionBoton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 escalaOriginal;
    
    [Header("Configuración del Botón")]
    public float factorEscala = 0.9f; 
    public float velocidadAnimacion = 15f; 

    [Header("Efecto de Sonido")]
    public AudioClip sonidoClic; // Aquí arrastrarás tu archivo de audio personalizado
    private AudioSource reproductor;

    private Coroutine rutinaAnimacion;

    void Start()
    {
        escalaOriginal = transform.localScale;

        // Creamos un reproductor de audio invisible en el botón automáticamente
        reproductor = gameObject.AddComponent<AudioSource>();
        reproductor.playOnAwake = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Reproducimos tu sonido personalizado al instante de tocar
        if (sonidoClic != null && reproductor != null)
        {
            reproductor.PlayOneShot(sonidoClic);
        }

        if (rutinaAnimacion != null) StopCoroutine(rutinaAnimacion);
        rutinaAnimacion = StartCoroutine(AnimarEscala(escalaOriginal * factorEscala));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (rutinaAnimacion != null) StopCoroutine(rutinaAnimacion);
        rutinaAnimacion = StartCoroutine(AnimarEscala(escalaOriginal));
    }

    IEnumerator AnimarEscala(Vector3 escalaDestino)
    {
        while (Vector3.Distance(transform.localScale, escalaDestino) > 0.01f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, escalaDestino, Time.deltaTime * velocidadAnimacion);
            yield return null;
        }
        transform.localScale = escalaDestino;
    }
}