using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EfectosMenu : MonoBehaviour
{
    [Header("Efecto de Fogata (Fondo)")]
    public Image panelFondo;
    public float velocidadLatidoFondo = 2f;
    public float oscuridadMinima = 0.8f;

    [Header("Movimiento de Árboles (Deformación)")]
    public DeformacionViento scriptViento; // Arrastra aquí el objeto que tiene el script de deformación
    public float velocidadViento = 1f;
    public float fuerzaVientoMax = 15f; 
    
    [Header("Efecto AK-47 (Movimiento y Latido)")]
    public RectTransform ak47;
    public float fuerzaRetroceso = 10f;
    public float velocidadLatidoAK = 5f;
    
    [Header("Efecto Destello AK-47")]
    public Image destelloArma;
    public float tiempoEntreDisparosMin = 0.1f;
    public float tiempoEntreDisparosMax = 2.5f;

    void Start()
    {
        if (destelloArma != null)
        {
            // Asegurarnos de que el fuego del arma empiece apagado
            destelloArma.enabled = false;
            StartCoroutine(RutinaDestello());
        }
    }

    void Update()
    {
        // Declaramos el tiempo para que el código sepa en qué segundo estamos
        float tiempo = Time.time;
        
        // 1. Efecto de latido de luz suave (Fogata)
        if (panelFondo != null)
        {
            // Usamos la variable correcta: velocidadLatidoFondo
            float ruido = Mathf.PerlinNoise(tiempo * velocidadLatidoFondo, 0f);
            float brillo = Mathf.Lerp(oscuridadMinima, 1f, ruido);
            panelFondo.color = new Color(brillo, brillo, brillo, 1f);
        }

        // 2. Control del viento (Deformamos la punta de los árboles)
        if (scriptViento != null)
        {
            // Usamos Sin para que vaya de izquierda a derecha suavemente
            scriptViento.intensidadViento = Mathf.Sin(tiempo * velocidadViento) * fuerzaVientoMax;
        }

        // 3. Movimiento de la AK-47 (Respiración/Latido de arriba a abajo)
        if (ak47 != null)
        {
            float movimientoY = Mathf.Sin(tiempo * velocidadLatidoAK) * 5f;
            float escalaLatido = 1f + (Mathf.Sin(tiempo * velocidadLatidoAK) * 0.02f);
            ak47.anchoredPosition = new Vector2(ak47.anchoredPosition.x, movimientoY);
            ak47.localScale = new Vector3(escalaLatido, escalaLatido, 1f);
        }
    }

    IEnumerator RutinaDestello()
    {
        while (true)
        {
            // Espera aleatoria antes de empezar a disparar
            yield return new WaitForSeconds(Random.Range(0.5f, 1f));

            // Ráfaga rápida (parpadea más veces)
            int disparos = Random.Range(6, 10);
            for (int i = 0; i < disparos; i++)
            {
                destelloArma.enabled = true;
                // Rotación loca para cada chispazo
                destelloArma.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
                
                yield return new WaitForSeconds(0.04f); // Más rápido
                destelloArma.enabled = false;
                yield return new WaitForSeconds(0.04f);
            }

            // El truco del reloj: Mientras espera el siguiente grupo de disparos, gira suavemente
            float tiempoEsperaCarga = Random.Range(0.5f, 1f);
            float t = 0;
            while (t < tiempoEsperaCarga)
            {
                t += Time.deltaTime;
                // Gira constantemente en el eje Z
                destelloArma.transform.Rotate(0, 0, 180 * Time.deltaTime); 
                yield return null;
            }
        }
    }
}