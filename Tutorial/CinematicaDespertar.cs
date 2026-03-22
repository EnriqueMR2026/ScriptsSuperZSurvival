using UnityEngine;
using System.Collections;

public class CinematicaDespertar : MonoBehaviour
{
    [Header("Cámaras y Puntos")]
    public Camera camaraCinematica;
    public Transform puntoAcostado;
    public Transform puntoSentado;
    public Transform puntoDePie;

    [Header("Efecto de Párpados (UI)")]
    public RectTransform parpadoSuperior;
    public RectTransform parpadoInferior;
    public float velocidadOjos = 1.5f;

    [Header("El Jugador Real")]
    public GameObject jugadorReal; // Tu personaje con controles
    public GameObject canvasJuego; // Tu UI de vida, hambre, etc.

    [Header("Tiempos de Animación")]
    public float tiempoSentarse = 2.5f;
    public float tiempoLevantarse = 2f;

    void Start()
    {
        // 1. Asegurarnos de que el jugador real esté apagado
        if (jugadorReal != null) jugadorReal.SetActive(false);
        if (canvasJuego != null) canvasJuego.SetActive(false);

        // 2. Colocar la cámara en la almohada
        camaraCinematica.transform.position = puntoAcostado.position;
        camaraCinematica.transform.rotation = puntoAcostado.rotation;

        // 3. ¡Acción!
        StartCoroutine(SecuenciaDespertar());
    }

    IEnumerator SecuenciaDespertar()
    {
        // Esperamos un segundito en total oscuridad (Tensión)
        yield return new WaitForSeconds(1f);

        // --- FASE 1: EL DESPERTAR CANSADO (PARPADEO DOBLE) ---
        // Primer parpadeo (abre muy poquito y cierra rápido)
        yield return StartCoroutine(Parpadear(0.3f)); 
        yield return new WaitForSeconds(0.4f); // Pausa más larga para simular cansancio

        // Segundo parpadeo (abre un poco más y cierra)
        yield return StartCoroutine(Parpadear(0.6f));
        yield return new WaitForSeconds(0.5f); 

        // Apertura definitiva de los ojos
        yield return StartCoroutine(AbrirOjosTotalmente());
        yield return new WaitForSeconds(1.5f); // Se queda viendo el techo un rato recuperando el aliento

        // --- FASE 2: SENTARSE EN LA CAMA ---
        yield return StartCoroutine(MoverCamara(puntoAcostado, puntoSentado, tiempoSentarse));
        yield return new WaitForSeconds(0.5f); // Pausa para tomar aire

        // --- FASE 3: PONERSE DE PIE ---
        yield return StartCoroutine(MoverCamara(puntoSentado, puntoDePie, tiempoLevantarse));
        yield return new WaitForSeconds(0.2f); 

        // --- FASE 4: TRANSICIÓN FINAL SUAVE ---
        // 1. Cerramos los ojos suavemente para el cambio de cámara
        yield return StartCoroutine(CerrarOjosMágicamente());

        // 2. Hacemos el cambio de cámaras y activamos al jugador en la oscuridad
        camaraCinematica.gameObject.SetActive(false); 
        if (jugadorReal != null) jugadorReal.SetActive(true); 

        // 3. Abrimos los ojos lentamente ya con la cámara del jugador activa
        yield return StartCoroutine(AbrirOjosMágicamente());

        // 4. Activamos el canvas del juego y apagamos el de la cinemática
        if (canvasJuego != null) canvasJuego.SetActive(true); 

        // 5. Le avisamos al tutorial que la cinemática ya terminó
        ManejadorTutorial tutorial = FindFirstObjectByType<ManejadorTutorial>();
        if (tutorial != null)
        {
            tutorial.IniciarPrimeraMisionConRetraso();
        }

        gameObject.SetActive(false); 
    }

    // --- CORRUTINAS DE PARPADEO Y TRANSICIÓN --- //

    // Corrutina para un parpadeo rápido (abre un poco y cierra)
    IEnumerator Parpadear(float porcentajeApertura)
    {
        float alturaInicialSup = parpadoSuperior.rect.height;
        float alturaInicialInf = parpadoInferior.rect.height;
        float alturaObjetivoSup = alturaInicialSup * (1f - porcentajeApertura);
        float alturaObjetivoInf = alturaInicialInf * (1f - porcentajeApertura);

        // Abre los ojos
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * (velocidadOjos * 2f); // Doble velocidad para parpadeo
            parpadoSuperior.sizeDelta = new Vector2(parpadoSuperior.sizeDelta.x, Mathf.Lerp(alturaInicialSup, alturaObjetivoSup, t));
            parpadoInferior.sizeDelta = new Vector2(parpadoInferior.sizeDelta.x, Mathf.Lerp(alturaInicialInf, alturaObjetivoInf, t));
            yield return null;
        }

        // Cierra los ojos
        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * (velocidadOjos * 2f); 
            parpadoSuperior.sizeDelta = new Vector2(parpadoSuperior.sizeDelta.x, Mathf.Lerp(alturaObjetivoSup, alturaInicialSup, t));
            parpadoInferior.sizeDelta = new Vector2(parpadoInferior.sizeDelta.x, Mathf.Lerp(alturaObjetivoInf, alturaInicialInf, t));
            yield return null;
        }
    }

    // Apertura definitiva de los ojos
    IEnumerator AbrirOjosTotalmente()
    {
        float alturaInicialSup = parpadoSuperior.rect.height;
        float alturaInicialInf = parpadoInferior.rect.height;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * velocidadOjos;
            parpadoSuperior.sizeDelta = new Vector2(parpadoSuperior.sizeDelta.x, Mathf.Lerp(alturaInicialSup, 0, t));
            parpadoInferior.sizeDelta = new Vector2(parpadoInferior.sizeDelta.x, Mathf.Lerp(alturaInicialInf, 0, t));
            yield return null;
        }
    }

    // Corrutina para cerrar los ojos suavemente
    IEnumerator CerrarOjosMágicamente()
    {
        float t = 0;
        float alturaInicialSup = parpadoSuperior.rect.height;
        float alturaInicialInf = parpadoInferior.rect.height;
        
        // Calculamos la mitad de la pantalla para asegurar que se cubra por completo
        float alturaObjetivo = Screen.height + 600f; 

        while (t < 1f)
        {
            t += Time.deltaTime * (velocidadOjos * 0.8f); // Un poco más lento para suavidad
            float curva = Mathf.SmoothStep(0f, 1f, t);

            parpadoSuperior.sizeDelta = new Vector2(parpadoSuperior.sizeDelta.x, Mathf.Lerp(alturaInicialSup, alturaObjetivo, curva));
            parpadoInferior.sizeDelta = new Vector2(parpadoInferior.sizeDelta.x, Mathf.Lerp(alturaInicialInf, alturaObjetivo, curva));
            
            yield return null;
        }

        // Aseguramos que queden cerrados
        parpadoSuperior.sizeDelta = new Vector2(parpadoSuperior.sizeDelta.x, alturaObjetivo);
        parpadoInferior.sizeDelta = new Vector2(parpadoInferior.sizeDelta.x, alturaObjetivo);

        // Esperamos a que Unity termine de renderizar el frame en negro
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.2f); 
    }

    // Corrutina para abrir los ojos suavemente una vez que el jugador ya está activo
    IEnumerator AbrirOjosMágicamente()
    {
        float t = 0;
        float alturaInicialSup = parpadoSuperior.rect.height;
        float alturaInicialInf = parpadoInferior.rect.height;

        while (t < 1f)
        {
            t += Time.deltaTime * (velocidadOjos * 0.5f);
            float curva = Mathf.SmoothStep(0f, 1f, t);

            parpadoSuperior.sizeDelta = new Vector2(parpadoSuperior.sizeDelta.x, Mathf.Lerp(alturaInicialSup, 0, curva));
            parpadoInferior.sizeDelta = new Vector2(parpadoInferior.sizeDelta.x, Mathf.Lerp(alturaInicialInf, 0, curva));
            
            yield return null;
        }

        // Aseguramos que queden abiertos al 100%
        parpadoSuperior.sizeDelta = new Vector2(parpadoSuperior.sizeDelta.x, 0);
        parpadoInferior.sizeDelta = new Vector2(parpadoInferior.sizeDelta.x, 0);
    }

    // Corrutina matemática para mover y rotar la cámara suavemente de un punto a otro
    IEnumerator MoverCamara(Transform inicio, Transform destino, float duracion)
    {
        float t = 0;
        while (t < 1f)
        {
            // Usamos SmoothStep para que acelere y desacelere como un movimiento humano real
            t += Time.deltaTime / duracion;
            float curvaSmooth = Mathf.SmoothStep(0f, 1f, t);

            camaraCinematica.transform.position = Vector3.Lerp(inicio.position, destino.position, curvaSmooth);
            camaraCinematica.transform.rotation = Quaternion.Slerp(inicio.rotation, destino.rotation, curvaSmooth);
            
            yield return null;
        }
    }
}