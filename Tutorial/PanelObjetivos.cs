using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class PanelObjetivos : MonoBehaviour
{
    [Header("Referencias de UI")]
    public RectTransform panelPrincipal; 
    public GameObject iconoLibrito;      
    public TextMeshProUGUI textoMision;

    [Header("Efectos Visuales Mágicos")]
    public RectTransform botonCerrar; // Arrastra tu botón de "X" o "Cerrar" aquí
    public Image imagenFondoPanel;    // Arrastra el fondo rojo aquí
    public float velocidadLatido = 3f;
    public float fuerzaLatido = 0.05f; 

    [Header("Configuración de Animación")]
    public float velocidadAnimacion = 2.5f;
    public Vector2 posicionEscondidaIzquierda = new Vector2(-1200f, 0f);
    public Vector2 posicionNormalPantalla = new Vector2(50f, 0f);

    private Coroutine efectosCorrutina;

    [Header("Sonidos")]
    public AudioSource audioSourcePanel;
    public AudioClip sonidoAbrir;
    public AudioClip sonidoCerrar;

    public void MostrarNuevaMision(string nuevaMision)
    {
        textoMision.text = nuevaMision;
        StartCoroutine(AnimacionDeslizarAdentro());
    }

    private IEnumerator AnimacionDeslizarAdentro()
    {
        if (iconoLibrito != null) iconoLibrito.SetActive(false);
        
        // Reproducir sonido al abrir
        if (audioSourcePanel != null && sonidoAbrir != null) audioSourcePanel.PlayOneShot(sonidoAbrir);
        
        panelPrincipal.gameObject.SetActive(true);
        panelPrincipal.localScale = Vector3.one; 
        panelPrincipal.anchoredPosition = posicionEscondidaIzquierda;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * velocidadAnimacion;
            float curvaSmooth = Mathf.SmoothStep(0f, 1f, t); 
            panelPrincipal.anchoredPosition = Vector2.Lerp(posicionEscondidaIzquierda, posicionNormalPantalla, curvaSmooth);
            yield return null;
        }
        
        panelPrincipal.anchoredPosition = posicionNormalPantalla;

        // Una vez que el panel entró, ¡iniciamos los efectos de terror!
        if (efectosCorrutina != null) StopCoroutine(efectosCorrutina);
        efectosCorrutina = StartCoroutine(EfectosVisualesConstantes());
    }

    // ¡NUEVA MAGIA! Corrutina que maneja el latido y el parpadeo
    private IEnumerator EfectosVisualesConstantes()
    {
        Vector3 escalaOriginalBoton = botonCerrar.localScale;
        Color colorOriginalFondo = imagenFondoPanel.color;
        
        // Decidimos en qué momento del futuro dará el primer chispazo
        float tiempoSiguienteParpadeo = Time.time + Random.Range(1f, 3f);

        while (true) // Se repite infinitamente mientras el panel esté en pantalla
        {
            // 1. Efecto de Latido del Botón (Se infla y desinfla suavecito)
            if (botonCerrar != null)
            {
                float escala = 1f + Mathf.Sin(Time.time * velocidadLatido) * fuerzaLatido;
                botonCerrar.localScale = escalaOriginalBoton * escala;
            }

            // 2. Efecto de Foco Fallando (Parpadeo rápido)
            if (imagenFondoPanel != null && Time.time >= tiempoSiguienteParpadeo)
            {
                // Da un "chispazo" mezclando su color con luz blanca
                imagenFondoPanel.color = Color.Lerp(colorOriginalFondo, Color.white, 0.4f);
                yield return new WaitForSeconds(Random.Range(0.05f, 0.1f)); 
                
                // Se apaga
                imagenFondoPanel.color = colorOriginalFondo;
                yield return new WaitForSeconds(Random.Range(0.02f, 0.08f)); 
                
                // A veces da un segundo chispazo rapidito (para que se sienta orgánico)
                if (Random.value > 0.5f)
                {
                    imagenFondoPanel.color = Color.Lerp(colorOriginalFondo, Color.white, 0.3f);
                    yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
                    imagenFondoPanel.color = colorOriginalFondo;
                }

                // Calculamos para cuándo será el próximo fallo de luz
                tiempoSiguienteParpadeo = Time.time + Random.Range(2f, 6f);
            }

            yield return null; 
        }
    }

    // ¡NUEVA FUNCIÓN! Esta es la que conectaremos al botón
    public void BotonCerrarPresionado()
    {
        StartCoroutine(AnimacionCerrarYConvertirEnLibro());
    }

    private IEnumerator AnimacionCerrarYConvertirEnLibro()
    {
        // 0. Reproducir sonido al cerrar
        if (audioSourcePanel != null && sonidoCerrar != null) audioSourcePanel.PlayOneShot(sonidoCerrar);
        
        // 1. Apagamos los latidos y chispazos para que no den error al encogerse
        if (efectosCorrutina != null) StopCoroutine(efectosCorrutina);
        
        // 2. Nos aseguramos de que el botón no se quede "inflado"
        botonCerrar.localScale = Vector3.one;

        float t = 0;
        float velocidadCierre = velocidadAnimacion * 1.5f; // Se cierra un poquito más rápido de lo que entró
        
        Vector2 posicionInicial = panelPrincipal.anchoredPosition;
        
        // Sacamos las coordenadas de dónde está el librito en la pantalla
        Vector2 posicionDestino = iconoLibrito.GetComponent<RectTransform>().anchoredPosition;

        // 3. Magia de distorsión y movimiento al mismo tiempo
        while (t < 1f)
        {
            t += Time.deltaTime * velocidadCierre;
            float curvaSmooth = Mathf.SmoothStep(0f, 1f, t);

            // Viaja hacia la posición del librito
            panelPrincipal.anchoredPosition = Vector2.Lerp(posicionInicial, posicionDestino, curvaSmooth);
            
            // Se encoje a cero (distorsionándose de derecha a izquierda, de abajo hacia arriba)
            panelPrincipal.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, curvaSmooth);

            yield return null;
        }

        // 4. Apagamos el panel grande por completo
        panelPrincipal.gameObject.SetActive(false);
        
        // 5. ¡Aparece el librito!
        if (iconoLibrito != null)
        {
            iconoLibrito.SetActive(true);
            // Aseguramos que el librito esté en su tamaño normal
            iconoLibrito.transform.localScale = Vector3.one; 
        }
    }

    // ¡NUEVA FUNCIÓN! Esta es la que conectaremos al botón del Librito pequeñito
    public void BotonLibritoPresionado()
    {
        StartCoroutine(AnimacionAbrirDesdeLibro());
    }

    private IEnumerator AnimacionAbrirDesdeLibro()
    {
        // 0. Reproducir sonido al volver a abrir desde el librito
        if (audioSourcePanel != null && sonidoAbrir != null) audioSourcePanel.PlayOneShot(sonidoAbrir);
        
        // 1. Apagamos el librito
        if (iconoLibrito != null) iconoLibrito.SetActive(false);

        // 2. Prendemos el panel grande pero chiquitito (escala cero) y en la esquina
        panelPrincipal.gameObject.SetActive(true);
        panelPrincipal.localScale = Vector3.zero;
        
        Vector2 posicionOrigen = iconoLibrito.GetComponent<RectTransform>().anchoredPosition;
        panelPrincipal.anchoredPosition = posicionOrigen;

        float t = 0;
        float velocidadApertura = velocidadAnimacion * 1.5f;

        // 3. Magia inversa: viaja de regreso al centro mientras se infla
        while (t < 1f)
        {
            t += Time.deltaTime * velocidadApertura;
            float curvaSmooth = Mathf.SmoothStep(0f, 1f, t);

            panelPrincipal.anchoredPosition = Vector2.Lerp(posicionOrigen, posicionNormalPantalla, curvaSmooth);
            panelPrincipal.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, curvaSmooth);

            yield return null;
        }

        // 4. Lo fijamos exactamente en su lugar y tamaño perfecto
        panelPrincipal.anchoredPosition = posicionNormalPantalla;
        panelPrincipal.localScale = Vector3.one;

        // 5. ¡Reiniciamos los efectos de terror (latidos y chispazos)!
        if (efectosCorrutina != null) StopCoroutine(efectosCorrutina);
        efectosCorrutina = StartCoroutine(EfectosVisualesConstantes());
    }
}