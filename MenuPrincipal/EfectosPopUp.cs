using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EfectosPopUp : MonoBehaviour
{
    [Header("Efectos del Título")]
    public RectTransform tituloTransform;
    public Image imagenTitulo;
    public float velocidadFlote = 1.5f;
    public float alturaFlote = 10f;
    public float velocidadBrillo = 2f;
    public float alphaMinimoTitulo = 0.5f;

    [Header("Efectos del Foco (Baño Apocalíptico)")]
    public Image luzFondo;
    public float tiempoMinFallo = 2f;
    public float tiempoMaxFallo = 6f;

    private Vector2 posicionInicialTitulo;

    void Start()
    {
        if (tituloTransform != null)
        {
            // Guardamos dónde pusiste el título originalmente para que flote desde ahí
            posicionInicialTitulo = tituloTransform.anchoredPosition;
        }

        if (luzFondo != null)
        {
            // Iniciamos el cortocircuito del foco
            StartCoroutine(RutinaParpadeoFoco());
        }
    }

    void Update()
    {
        if (tituloTransform != null)
        {
            // Flote suave de arriba a abajo usando Seno
            float nuevaY = posicionInicialTitulo.y + Mathf.Sin(Time.time * velocidadFlote) * alturaFlote;
            tituloTransform.anchoredPosition = new Vector2(posicionInicialTitulo.x, nuevaY);
        }

        if (imagenTitulo != null)
        {
            // Brillo que sube y baja (cambiando la transparencia o Alpha)
            float brillo = Mathf.Lerp(alphaMinimoTitulo, 1f, (Mathf.Sin(Time.time * velocidadBrillo) + 1f) / 2f);
            Color colorTitulo = imagenTitulo.color;
            colorTitulo.a = brillo;
            imagenTitulo.color = colorTitulo;
        }
    }

    IEnumerator RutinaParpadeoFoco()
    {
        while (true)
        {
            // Espera normal con la luz encendida, simulando que el foco funciona bien un rato
            yield return new WaitForSeconds(Random.Range(tiempoMinFallo, tiempoMaxFallo));

            // Falla del foco (Parpadeo aleatorio entre 1 y 3 veces seguidas)
            int cantidadParpadeos = Random.Range(1, 4);
            
            for (int i = 0; i < cantidadParpadeos; i++)
            {
                // Oscurece el fondo de tu baño (Bajando los valores RGB, no el Alpha)
                float oscuridad = Random.Range(0.2f, 0.5f); // 0.2 es muy oscuro, 0.5 es a la mitad
                luzFondo.color = new Color(oscuridad, oscuridad, oscuridad, 1f);
                
                // Apagado por un instante súper rápido
                yield return new WaitForSeconds(Random.Range(0.05f, 0.15f)); 
                
                // Enciende de nuevo la luz al 100% (Blanco puro para que tu imagen se vea normal)
                luzFondo.color = new Color(1f, 1f, 1f, 1f);
                
                // Encendido por un instante antes del siguiente parpadeo (si hay más de uno)
                yield return new WaitForSeconds(Random.Range(0.05f, 0.2f)); 
            }
        }
    }
}