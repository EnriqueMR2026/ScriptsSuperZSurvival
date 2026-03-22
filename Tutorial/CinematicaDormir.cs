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
        // Apagamos el UI del juego (el Canvas) para que parezca de película
        if (canvasJuego != null) canvasJuego.SetActive(false);

        // Cerramos los ojos rápido simulando un parpadeo de transición
        yield return StartCoroutine(CerrarOjosMagicamente(velocidadOjos * 1.5f));

        // Hacemos el cambio mágico: Apagamos al jugador real y prendemos la cámara de cine "De Pie"
        if (jugadorReal != null) jugadorReal.SetActive(false);
        camaraCinematica.gameObject.SetActive(true);
        camaraCinematica.transform.position = puntoDePie.position;
        camaraCinematica.transform.rotation = puntoDePie.rotation;

        // Abrimos los ojos para ver desde la nueva cámara
        yield return StartCoroutine(AbrirOjosMagicamente(velocidadOjos * 1.5f));
        yield return new WaitForSeconds(0.5f);

        // --- FASE 2: IR A LA CAMA ---
        // De pie a sentado
        yield return StartCoroutine(MoverCamara(puntoDePie, puntoSentado, tiempoSentarse));
        yield return new WaitForSeconds(0.5f);

        // De sentado a acostado
        yield return StartCoroutine(MoverCamara(puntoSentado, puntoAcostado, tiempoAcostarse));
        yield return new WaitForSeconds(1f);

        // --- FASE 3: QUEDARSE DORMIDO (PARPADEOS LENTOS) ---
        // Primer parpadeo de sueño (lento)
        yield return StartCoroutine(Parpadear(0.5f, velocidadOjos * 0.7f));
        yield return new WaitForSeconds(1f);

        // Segundo parpadeo más pesado
        yield return StartCoroutine(Parpadear(0.8f, velocidadOjos * 0.5f));
        yield return new WaitForSeconds(1f);

        // Cerrar los ojos definitivamente por el cansancio
        yield return StartCoroutine(CerrarOjosMagicamente(velocidadOjos * 0.4f));

        // --- FASE 4: TRANSICIÓN AL DÍA 2 ---
        yield return new WaitForSeconds(2f); // Tiempo en negro total para descansar

        if (textoDiaHora != null)
        {
            textoDiaHora.gameObject.SetActive(true);
            textoDiaHora.text = "DÍA 2\n09:00 AM"; // ¡El nuevo día!
            yield return new WaitForSeconds(4f); // Tiempo para leer con tensión
            textoDiaHora.gameObject.SetActive(false);
        }

        // --- FASE 5: DESPERTAR AL DÍA SIGUIENTE ---
        yield return new WaitForSeconds(1f);

        // Parpadeo corto para despertar
        yield return StartCoroutine(Parpadear(0.4f, velocidadOjos));
        yield return new WaitForSeconds(0.5f);

        // Abre los ojos totalmente
        yield return StartCoroutine(AbrirOjosMagicamente(velocidadOjos));
        yield return new WaitForSeconds(1f);

        // Se sienta en la cama
        yield return StartCoroutine(MoverCamara(puntoAcostado, puntoSentado, tiempoSentarse));
        yield return new WaitForSeconds(0.5f);

        // Se levanta de la cama
        yield return StartCoroutine(MoverCamara(puntoSentado, puntoDePie, tiempoAcostarse));
        yield return new WaitForSeconds(0.5f);

        // --- FASE 6: DEVOLVER EL CONTROL AL JUGADOR ---
        // Parpadeo de transición final
        yield return StartCoroutine(CerrarOjosMagicamente(velocidadOjos * 1.5f));

        // Apagamos la cámara de cine y encendemos al jugador real
        camaraCinematica.gameObject.SetActive(false);
        if (jugadorReal != null) jugadorReal.SetActive(true);

        yield return StartCoroutine(AbrirOjosMagicamente(velocidadOjos * 1.5f));

        // Devolvemos la UI a la normalidad
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