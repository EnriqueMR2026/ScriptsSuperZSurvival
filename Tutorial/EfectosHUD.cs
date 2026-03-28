using UnityEngine;
using TMPro; 
using System.Collections;

public class EfectosHUD : MonoBehaviour
{
    [Header("Referencias Visuales")]
    public TMP_Text textoCombos;
    public GameObject imagenHitmarker;

    private Vector3 tamanoOriginalTexto;

    // ¡NUEVAS VARIABLES! Para controlar tu racha de combos
    public float tiempoParaPerderCombo = 3f;
    private int contadorHits = 0;
    private float temporizadorCombo = 0f;

    // --- VARIABLES PARA CONTROLAR ANIMACIONES ---
    private Coroutine corrutinaLatido;
    private Coroutine corrutinaHitmarker;

    void Start()
    {
        // Guardamos el tamaño original de tu texto para que el latido sepa a qué tamaño regresar
        if (textoCombos != null)
        {
            tamanoOriginalTexto = textoCombos.transform.localScale;
        }

        // ¡NUEVO! Apagamos los textos al iniciar la partida para que empiecen invisibles
        textoCombos.gameObject.SetActive(false);
        imagenHitmarker.SetActive(false);
    }

    // --- MAGIA DEL LATIDO ---
    public IEnumerator LatidoTexto()
    {
        // El latido: Crece un 150% de golpe
        textoCombos.transform.localScale = tamanoOriginalTexto * 1.5f; 
        
        // Regresa a su tamaño original suavemente en 0.15 segundos para dar ese efecto de "pálpito"
        float tiempo = 0f;
        while (tiempo < 0.15f)
        {
            tiempo += Time.deltaTime;
            textoCombos.transform.localScale = Vector3.Lerp(tamanoOriginalTexto * 1.5f, tamanoOriginalTexto, tiempo / 0.15f);
            yield return null; // Esperamos al siguiente frame
        }
        
        // Aseguramos que al terminar quede exactamente en su tamaño normal
        textoCombos.transform.localScale = tamanoOriginalTexto;
    }

    void Update()
    {
        // Si tenemos al menos 1 hit, el reloj empieza a correr hacia atrás
        if (contadorHits > 0)
        {
            temporizadorCombo -= Time.deltaTime;

            if (temporizadorCombo <= 0f)
            {
                // Se acabó el tiempo, el combo regresa a cero
                PerderCombo();
            }
        }
    }

    // ¡LA FUNCIÓN MAESTRA QUE LLAMAREMOS AL DISPARAR!
    public void RegistrarImpacto()
    {
        contadorHits++;
        temporizadorCombo = tiempoParaPerderCombo; // Reiniciamos el reloj a 3 segundos

        // 1. Encendemos los visuales
        textoCombos.gameObject.SetActive(true);
        imagenHitmarker.SetActive(true);

        // 2. Actualizamos el número en pantalla
        textoCombos.text = "HIT x" + contadorHits + "!";

        // 3. ¡MAGIA DE COLORES! (Blanco -> Amarillo -> Naranja -> Rojo cada 7 hits)
        if (contadorHits < 7)
        {
            textoCombos.color = Color.white;
        }
        else if (contadorHits >= 7 && contadorHits < 14)
        {
            textoCombos.color = Color.yellow;
        }
        else if (contadorHits >= 14 && contadorHits < 21)
        {
            // Unity no tiene un "Color.orange" por defecto que se vea tan bien, así que lo creamos a mano
            textoCombos.color = new Color(1f, 0.5f, 0f); 
        }
        else if (contadorHits >= 21)
        {
            textoCombos.color = Color.red;
        }

        // 4. Hacemos que el texto palpite (Detenemos el latido anterior por si disparas muy rápido)
        if (corrutinaLatido != null) StopCoroutine(corrutinaLatido);
        corrutinaLatido = StartCoroutine(LatidoTexto());

        // 5. El hitmarker debe desaparecer rapidísimo (es solo un destello)
        if (corrutinaHitmarker != null) StopCoroutine(corrutinaHitmarker);
        corrutinaHitmarker = StartCoroutine(ApagarHitmarker());
    }

    private void PerderCombo()
    {
        contadorHits = 0;
        textoCombos.gameObject.SetActive(false);
    }

    private IEnumerator ApagarHitmarker()
    {
        // Esperamos una fracción de segundo y apagamos la tachita
        yield return new WaitForSeconds(0.15f);
        imagenHitmarker.SetActive(false);
    }
}