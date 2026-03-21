using UnityEngine;
using TMPro;
using System.Collections;

public class InventarioJugador : MonoBehaviour
{
    [Header("Cantidades (Logica)")]
    public int madera = 0;
    public int piedra = 0;
    public int gold = 0;
    public int fragmentos = 0;
    
    [Header("UI Text Individuales")]
    public TextMeshProUGUI textoMadera;
    public TextMeshProUGUI textoPiedra;
    public TextMeshProUGUI textoGold;
    public TextMeshProUGUI textoFragmentos;

    [Header("Iconos para Animación")]
    public RectTransform iconoMadera;
    public RectTransform iconoPiedra;
    public RectTransform iconoGold;
    public RectTransform iconoFragmentos;

    public float escalaPunch = 1.2f; // Qué tanto se agranda (1.2 = 20% más)
    public float duracionPunch = 0.1f; // Qué tan rápido hace el efecto

    void Start()
    {
        ActualizarTextoUI();
    }

    public void AgregarRecurso(string tipo, int cantidad)
    {
        RectTransform iconoAAnimar = null;

        if (tipo == "Madera")
        {
            madera += cantidad;
            iconoAAnimar = iconoMadera;
        }
        else if (tipo == "Piedra")
        {
            piedra += cantidad;
            iconoAAnimar = iconoPiedra;
        }
        else if (tipo == "Gold")
        {
            gold += cantidad;
            iconoAAnimar = iconoGold;
        }
        else if (tipo == "Fragmentos")
        {
            fragmentos += cantidad;
            iconoAAnimar = iconoFragmentos;
        }

        ActualizarTextoUI();

        // Si encontramos el icono, lanzamos la animación de agrandado
        if (iconoAAnimar != null)
        {
            StopCoroutine("EfectoPunch"); // Detenemos si ya se estaba moviendo
            StartCoroutine(EfectoPunch(iconoAAnimar));
        }
    }
    IEnumerator EfectoPunch(RectTransform target)
    {
        Vector3 escalaOriginal = Vector3.one;
        Vector3 escalaDestino = Vector3.one * escalaPunch;

        float tiempo = 0;
        while (tiempo < duracionPunch)
        {
            target.localScale = Vector3.Lerp(escalaOriginal, escalaDestino, tiempo / duracionPunch);
            tiempo += Time.deltaTime;
            yield return null;
        }

        tiempo = 0;
        while (tiempo < duracionPunch)
        {
            target.localScale = Vector3.Lerp(escalaDestino, escalaOriginal, tiempo / duracionPunch);
            tiempo += Time.deltaTime;
            yield return null;
        }

        target.localScale = escalaOriginal;
    }
        void ActualizarTextoUI()
    {
        if (textoMadera != null) textoMadera.text = madera.ToString();
        if (textoPiedra != null) textoPiedra.text = piedra.ToString();
        if (textoGold != null) textoGold.text = gold.ToString();
        if (textoFragmentos != null) textoFragmentos.text = fragmentos.ToString();
    }
}