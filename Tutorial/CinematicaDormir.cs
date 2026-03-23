using UnityEngine;
using System.Collections;
using TMPro; // Para el texto de Día 2

public class CinematicaDormir : MonoBehaviour
{
    [Header("Cámaras y Puntos")]
    public Camera camaraCinematica;
    public Transform puntoAcostado;
    public Transform puntoSentado;
    public Transform puntoDePie;

    [Header("Efectos UI")]
    public RectTransform parpadoSuperior;
    public RectTransform parpadoInferior;
    public TextMeshProUGUI textoDiaHora; // Para mostrar DÍA 2
    public float velocidadOjos = 1.5f;

    [Header("El Jugador Real")]
    public GameObject jugadorReal; 
    public GameObject canvasJuego; 

    [Header("Tiempos de Animación")]
    public float tiempoSentarse = 2f;
    public float tiempoAcostarse = 2.5f;

    // Esta función la llamará tu CamaTutorial al acostarte
    public void IniciarDormir()
    {
        StartCoroutine(SecuenciaDormirYDespertar());
    }

    IEnumerator SecuenciaDormirYDespertar()
    {
        // --- FASE 1: PREPARACIÓN Y TRANSICIÓN A CINEMÁTICA ---
        if (canvasJuego != null) canvasJuego.SetActive(false);

        yield return StartCoroutine(CerrarOjosMagicamente(velocidadOjos * 1.5f));

        Camera camaraTusOjos = Camera.main; 
        
        if (jugadorReal != null) jugadorReal.SetActive(false);
        camaraCinematica.gameObject.SetActive(true);

        if (camaraTusOjos != null)
        {
            camaraCinematica.transform.position = camaraTusOjos.transform.position;
            camaraCinematica.transform.rotation = camaraTusOjos.transform.rotation;
        }
        else 
        {
            camaraCinematica.transform.position = puntoDePie.position;
            camaraCinematica.transform.rotation = puntoDePie.rotation;
        }

        yield return StartCoroutine(AbrirOjosMagicamente(velocidadOjos * 1.5f));
        yield return new WaitForSeconds(0.2f); 

        // --- FASE 2: IR A LA CAMA (FLUIDO) ---
        yield return StartCoroutine(MoverCamaraDesdeActual(puntoSentado, tiempoSentarse));
        yield return new WaitForSeconds(0.2f);

        yield return StartCoroutine(MoverCamara(puntoSentado, puntoAcostado, tiempoAcostarse));

        // --- FASE 3: QUEDARSE DORMIDO ---
        yield return StartCoroutine(Parpadear(0.5f, velocidadOjos * 0.7f));
        yield return StartCoroutine(Parpadear(0.8f, velocidadOjos * 0.5f));
        yield return StartCoroutine(CerrarOjosMagicamente(velocidadOjos * 0.4f));

        // --- FASE 4: TRANSICIÓN AL DÍA 2 Y RESETEO DEL MUNDO ---
        
        // ¡MAGIA DE AMANECER! Mientras tienes los ojos cerrados, reseteamos el set de grabación
        
        // 1. Buscamos el Sol y le ponemos los valores exactos de tu foto
        Light[] luces = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
        foreach (Light luz in luces)
        {
            if (luz.type == LightType.Directional)
            {
                // ¡CORRECCIÓN! Intensidad a 1.5 como pediste para que brille precioso
                luz.intensity = 1.5f;
                // Transformamos tu color Hexadecimal a formato de Unity
                Color colorDia;
                if (ColorUtility.TryParseHtmlString("#FFF3CD", out colorDia))
                {
                    luz.color = colorDia;
                }
                // Tu rotación exacta
                luz.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
                break;
            }
        }

        // 2. Buscamos la fogata solita y apagamos sus efectos
        FogataTutorial fogata = Object.FindFirstObjectByType<FogataTutorial>();
        if (fogata != null)
        {
            if (fogata.fuegoParticulas != null) fogata.fuegoParticulas.SetActive(false);
            if (fogata.luzFogata != null) fogata.luzFogata.enabled = false;
        }
        
        // 3. Apagamos la niebla roja de terror para que el cielo se vea clarito
        RenderSettings.fog = false;

        yield return new WaitForSeconds(2f); 

        if (textoDiaHora != null)
        {
            textoDiaHora.gameObject.SetActive(true);
            textoDiaHora.text = "DÍA 2\n09:00 AM"; 
            yield return new WaitForSeconds(4f); 
            textoDiaHora.gameObject.SetActive(false);
        }

        // --- FASE 5: DESPERTAR AL DÍA SIGUIENTE ---
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(Parpadear(0.4f, velocidadOjos));
        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(AbrirOjosMagicamente(velocidadOjos));
        yield return new WaitForSeconds(0.5f);

        // Se sienta
        yield return StartCoroutine(MoverCamara(puntoAcostado, puntoSentado, tiempoSentarse));
        yield return new WaitForSeconds(0.2f);

        // Se levanta
        yield return StartCoroutine(MoverCamara(puntoSentado, puntoDePie, tiempoAcostarse));
        yield return new WaitForSeconds(0.5f);

        // --- FASE 6: DEVOLVER EL CONTROL ---
        yield return StartCoroutine(CerrarOjosMagicamente(velocidadOjos * 1.5f));

        camaraCinematica.gameObject.SetActive(false);
        
        if (jugadorReal != null) 
        {
            jugadorReal.transform.position = puntoDePie.position;
            
            MovimientoJugador mov = jugadorReal.GetComponent<MovimientoJugador>();
            if (mov != null)
            {
                mov.ForzarRotacion(90f, 0f);
            }
            
            jugadorReal.SetActive(true);
        }

        yield return StartCoroutine(AbrirOjosMagicamente(velocidadOjos * 1.5f));

        if (canvasJuego != null) canvasJuego.SetActive(true);
    }

    // --- CORRUTINAS MATEMÁTICAS --- //

    IEnumerator Parpadear(float porcentajeApertura, float velocidad)
    {
        float alturaInicialSup = parpadoSuperior.rect.height;
        float alturaInicialInf = parpadoInferior.rect.height;
        float alturaObjetivoSup = alturaInicialSup * (1f - porcentajeApertura);
        float alturaObjetivoInf = alturaInicialInf * (1f - porcentajeApertura);

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * velocidad;
            parpadoSuperior.sizeDelta = new Vector2(parpadoSuperior.sizeDelta.x, Mathf.Lerp(alturaInicialSup, alturaObjetivoSup, t));
            parpadoInferior.sizeDelta = new Vector2(parpadoInferior.sizeDelta.x, Mathf.Lerp(alturaInicialInf, alturaObjetivoInf, t));
            yield return null;
        }

        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * velocidad;
            parpadoSuperior.sizeDelta = new Vector2(parpadoSuperior.sizeDelta.x, Mathf.Lerp(alturaObjetivoSup, alturaInicialSup, t));
            parpadoInferior.sizeDelta = new Vector2(parpadoInferior.sizeDelta.x, Mathf.Lerp(alturaObjetivoInf, alturaInicialInf, t));
            yield return null;
        }
    }

    IEnumerator CerrarOjosMagicamente(float velocidad)
    {
        float t = 0;
        float alturaInicialSup = parpadoSuperior.rect.height;
        float alturaInicialInf = parpadoInferior.rect.height;
        float alturaObjetivo = Screen.height + 600f; 

        while (t < 1f)
        {
            t += Time.deltaTime * velocidad;
            float curva = Mathf.SmoothStep(0f, 1f, t);
            parpadoSuperior.sizeDelta = new Vector2(parpadoSuperior.sizeDelta.x, Mathf.Lerp(alturaInicialSup, alturaObjetivo, curva));
            parpadoInferior.sizeDelta = new Vector2(parpadoInferior.sizeDelta.x, Mathf.Lerp(alturaInicialInf, alturaObjetivo, curva));
            yield return null;
        }

        parpadoSuperior.sizeDelta = new Vector2(parpadoSuperior.sizeDelta.x, alturaObjetivo);
        parpadoInferior.sizeDelta = new Vector2(parpadoInferior.sizeDelta.x, alturaObjetivo);
        yield return new WaitForEndOfFrame();
    }

    IEnumerator AbrirOjosMagicamente(float velocidad)
    {
        float t = 0;
        float alturaInicialSup = parpadoSuperior.rect.height;
        float alturaInicialInf = parpadoInferior.rect.height;

        while (t < 1f)
        {
            t += Time.deltaTime * velocidad;
            float curva = Mathf.SmoothStep(0f, 1f, t);
            parpadoSuperior.sizeDelta = new Vector2(parpadoSuperior.sizeDelta.x, Mathf.Lerp(alturaInicialSup, 0, curva));
            parpadoInferior.sizeDelta = new Vector2(parpadoInferior.sizeDelta.x, Mathf.Lerp(alturaInicialInf, 0, curva));
            yield return null;
        }

        parpadoSuperior.sizeDelta = new Vector2(parpadoSuperior.sizeDelta.x, 0);
        parpadoInferior.sizeDelta = new Vector2(parpadoInferior.sizeDelta.x, 0);
    }

    // ¡NUEVA CORRUTINA! Para calcular el movimiento desde tu posición actual exacta
    IEnumerator MoverCamaraDesdeActual(Transform destino, float duracion)
    {
        Vector3 posicionInicial = camaraCinematica.transform.position;
        Quaternion rotacionInicial = camaraCinematica.transform.rotation;
        float t = 0;
        
        while (t < 1f)
        {
            t += Time.deltaTime / duracion;
            float curvaSmooth = Mathf.SmoothStep(0f, 1f, t);
            camaraCinematica.transform.position = Vector3.Lerp(posicionInicial, destino.position, curvaSmooth);
            camaraCinematica.transform.rotation = Quaternion.Slerp(rotacionInicial, destino.rotation, curvaSmooth);
            yield return null;
        }
    }

    IEnumerator MoverCamara(Transform inicio, Transform destino, float duracion)
    {
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / duracion;
            float curvaSmooth = Mathf.SmoothStep(0f, 1f, t);
            camaraCinematica.transform.position = Vector3.Lerp(inicio.position, destino.position, curvaSmooth);
            camaraCinematica.transform.rotation = Quaternion.Slerp(inicio.rotation, destino.rotation, curvaSmooth);
            yield return null;
        }
    }
}