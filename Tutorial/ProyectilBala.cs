using UnityEngine;

public class ProyectilBala : MonoBehaviour
{
    [Header("Efectos")]
    public GameObject efectoImpacto; // El prefab de la chispa o polvo que aparecerá al chocar

    [Header("Configuración de Vuelo")]
    public float tiempoDeVida = 2f; // Tiempo en segundos antes de desaparecer en el aire

    void Start()
    {
        // 1. Reloj de autodestrucción: Si no choca con nada, se borra en 5 segundos
        Destroy(gameObject, 5f);
        
        // --- ¡EL BUG ESTABA AQUÍ! BORRAMOS LA SECCIÓN 2 POR COMPLETO ---
    }

    // ¡NUEVO! Agrega esta variable justo arriba, junto a 'tiempoDeVida', para que el arma pueda modificarla
    public float danoBala = 35f;

    private void OnCollisionEnter(Collision collision)
    {
        // 1. Obtenemos el punto exacto donde la bala tocó la superficie
        ContactPoint contacto = collision.contacts[0];
        
        // 2. Orientamos el efecto para que salga "hacia afuera" de la pared
        Quaternion rotacionImpacto = Quaternion.LookRotation(contacto.normal);

        // 3. Si el efecto de impacto está asignado, lo creamos en ese punto
        if (efectoImpacto != null)
        {
            Instantiate(efectoImpacto, contacto.point, rotacionImpacto);
        }

        // 3.5 Le preguntamos al objeto chocado si tiene el código de zombi
        EnemigoZombi zombi = collision.collider.GetComponent<EnemigoZombi>();
        if (zombi != null)
        {
            // ¡ACTUALIZADO! Ahora usamos la variable en lugar del número fijo
            zombi.RecibirDano(Mathf.RoundToInt(danoBala));

            // ¡ACTUALIZADO! Sumamos el daño real al contador global de estadísticas
            ManejadorPausa.danoCausado += danoBala;

            // --- ¡NUEVO! AVISAMOS AL HUD DEL IMPACTO ---
            EfectosHUD efectos = FindFirstObjectByType<EfectosHUD>();
            if (efectos != null)
            {
                efectos.RegistrarImpacto();
            }
        }

        // 4. Borramos la bala inmediatamente para que no atraviese ni rebote
        Destroy(gameObject);
    }
}