using UnityEngine;

public class ProyectilBala : MonoBehaviour
{
    [Header("Efectos")]
    public GameObject efectoImpacto; // El prefab de la chispa o polvo que aparecerá al chocar

    [Header("Configuración de Vuelo")]
    public float tiempoDeVida = 2f; // Tiempo en segundos antes de desaparecer en el aire

    void Start()
    {
        // Esta sola línea destruye la bala mágicamente después de X segundos
        // para que no viaje hasta el infinito si disparan al cielo.
        Destroy(gameObject, tiempoDeVida);
    }

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

        // ¡AQUÍ ESTÁ LA MAGIA NUEVA! Le preguntamos al objeto chocado si tiene el código de zombi
        EnemigoZombi zombi = collision.collider.GetComponent<EnemigoZombi>();
        if (zombi != null)
        {
            // Le bajamos 35 puntos de vida (necesitarás 3 tiros para matar a un zombi de 100 de vida)
            zombi.RecibirDano(35);
        }

        // 4. Borramos la bala inmediatamente para que no atraviese ni rebote
        Destroy(gameObject);
    }
}