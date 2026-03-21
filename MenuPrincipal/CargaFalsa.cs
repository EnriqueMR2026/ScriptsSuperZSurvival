using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CargaFalsa : MonoBehaviour
{
    [Header("UI de Progreso")]
    public TMPro.TextMeshProUGUI textoCarga;
    public TMPro.TextMeshProUGUI textoPorcentaje;
    public UnityEngine.UI.Image barraDeCarga;
    
    // Lista de 150 temáticas de Carga
    private string[] frasesFalsas = {
        "Afilando el hacha de tala...",
        "Limpiando el cañón de la AK-47...",
        "Plantando el Roble Celestial...",
        "Generando cerebros para la horda...",
        "Acomodando la madera en el refugio...",
        "Horneando bolillos en el molino...",
        "Pintando la sangre de los zombis...",
        "Escondiendo la piedra en la cantera...",
        "Preparando la fogata...",
        "Haciendo la cama de la taberna...",
        "Cargando polígonos del mapa...",
        "Calculando el peso de los troncos...",
        "Forjando la hoja del pico...",
        "Llenando los cargadores de balas...",
        "Practicando gruñidos de muerto viviente...",
        "Barriendo el polvo del refugio...",
        "Ajustando la mira de las armas...",
        "Encendiendo el sol del Día 1...",
        "Apagando las luces para la noche...",
        "Acomodando los estantes de la tienda...",
        "Calculando la física de las caídas...",
        "Escondiendo Easter Eggs...",
        "Puliendo el cinturón de herramientas...",
        "Renderizando el pasto del molino...",
        "Colocando las puertas de madera...",
        "Enseñando a caminar a los zombis...",
        "Inyectando adrenalina a la horda...",
        "Doblando las sábanas de la cama...",
        "Picando la piedra de la mina...",
        "Calculando el retroceso del arma...",
        "Limpiando la sangre del piso...",
        "Preparando el sistema de misiones...",
        "Afinando los efectos de sonido...",
        "Generando la niebla nocturna...",
        "Aplastando los bugs del mapa...",
        "Contando las monedas de la tienda...",
        "Sembrando árboles normales...",
        "Tallando el letrero de la taberna...",
        "Cargando texturas de supervivencia...",
        "Poniendo el seguro a la AK-47...",
        "Ensamblando el motor de físicas...",
        "Preparando el tutorial...",
        "Despertando al comerciante...",
        "Afilando los dientes de los zombis...",
        "Calculando el daño de los balazos...",
        "Acomodando el inventario...",
        "Revisando las colisiones de las paredes...",
        "Preparando la pantalla de victoria...",
        "Cargando inteligencia artificial...",
        "Engrasando las bisagras de las puertas...",
        "Sacudiendo el polvo de las rocas...",
        "Preparando las animaciones de ataque...",
        "Escondiendo raciones de comida...",
        "Calibrando el daño del hacha...",
        "Calculando la distancia de persecución...",
        "Enfriando el cañón de las armas...",
        "Diseñando la interfaz del jugador...",
        "Recogiendo la basura de la aldea...",
        "Preparando el apocalipsis...",
        "Revisando los puntos de vida...",
        "Limpiando la mira de hierro...",
        "Calculando el impacto de la madera...",
        "Colocando la leña en la fogata...",
        "Alineando los árboles del bosque...",
        "Cargando el motor de renderizado...",
        "Preparando las sombras dinámicas...",
        "Encerando el piso de la taberna...",
        "Calculando la velocidad de los zombis...",
        "Programando el miedo...",
        "Ajustando el brillo de la luna...",
        "Distribuyendo la maleza...",
        "Cargando sonidos de pasos...",
        "Preparando la zona segura...",
        "Revisando la durabilidad del hacha...",
        "Generando partículas de polvo...",
        "Alistando los recursos del mapa...",
        "Cargando el cielo diurno...",
        "Preparando el cielo estrellado...",
        "Revisando las físicas del jugador...",
        "Cargando los menús deslizantes...",
        "Calculando la recolección de piedra...",
        "Ajustando la cámara en primera persona...",
        "Puliendo los modelos 3D...",
        "Escondiendo secretos en el molino...",
        "Preparando el sistema de comercio...",
        "Calculando el precio del pico...",
        "Evaluando el valor de la madera...",
        "Cargando las animaciones de muerte...",
        "Ajustando el tamaño del Roble Celestial...",
        "Distribuyendo rocas en la cantera...",
        "Preparando el evento de la horda...",
        "Sincronizando el reloj interno...",
        "Cargando texturas de alta resolución...",
        "Optimizando el uso de memoria...",
        "Preparando el cinturón del jugador...",
        "Cargando íconos de la interfaz...",
        "Revisando el código de los enemigos...",
        "Afinando el sistema de apuntado...",
        "Calculando físicas de las balas...",
        "Cargando el sistema de guardado falso...",
        "Preparando el terreno de juego...",
        "Ajustando la gravedad del mundo...",
        "Compilando shaders de materiales...",
        "Generando la malla de navegación...",
        "Enseñando a los zombis a esquivar...",
        "Preparando los efectos de sangre...",
        "Ajustando la sensibilidad del control...",
        "Cargando el modelo de la AK-47...",
        "Revisando el peso de la mochila...",
        "Afinando los sonidos de los disparos...",
        "Preparando la música de tensión...",
        "Cargando los efectos ambientales...",
        "Ajustando la iluminación global...",
        "Revisando colisiones del terreno...",
        "Preparando el spawn de enemigos...",
        "Cargando el sistema de oleadas...",
        "Ajustando la dificultad del Día 1...",
        "Preparando los desafíos del Día 2...",
        "Cargando la horda del Día 3...",
        "Revisando la lógica de las misiones...",
        "Afinando el tutorial de supervivencia...",
        "Preparando las notificaciones...",
        "Cargando el apuntador de objetivos...",
        "Revisando el daño por caída...",
        "Ajustando el área de interacción...",
        "Cargando las texturas de la tienda...",
        "Preparando los objetos intercambiables...",
        "Revisando el valor de los bolillos...",
        "Afinando el sistema de recarga...",
        "Cargando animaciones del hacha...",
        "Preparando el golpe del pico...",
        "Revisando la detección de impactos...",
        "Ajustando el retroceso visual...",
        "Cargando la interfaz de salud...",
        "Preparando el Game Over...",
        "Revisando el contador de días...",
        "Afinando las físicas de la madera...",
        "Cargando el sistema de construcción...",
        "Preparando la interacción con la cama...",
        "Revisando el mensaje de sueño...",
        "Ajustando el tiempo de carga...",
        "Cargando la escena principal...",
        "Preparando la experiencia arcade...",
        "Revisando los Easter Eggs de CoD...",
        "Afinando los detalles del mapa...",
        "Cargando los scripts finales...",
        "Preparando al jugador...",
        "¡Todo listo, cargando!"
    };

    void Start()
    {
        // Iniciamos la trampa apenas abre el juego
        StartCoroutine(RutinaDeCarga());
    }

    IEnumerator RutinaDeCarga()
    {
        float tiempoTotal = 10f; // Los 10 segundos de carga
        float tiempoPasado = 0f;

        while (tiempoPasado < tiempoTotal)
        {
            // Ponemos una frase al azar en la pantalla
            textoCarga.text = frasesFalsas[Random.Range(0, frasesFalsas.Length)];
            
            // Calculamos cuánto tiempo vamos a esperar en este ciclo
            float tiempoEspera = Random.Range(0.1f, 0.4f);
            
            // Hacemos una mini-rutina para que la barra y el porcentaje suban suavemente durante ese tiempo de espera
            float tiempoSubida = 0f;
            float porcentajeInicio = tiempoPasado / tiempoTotal;
            float porcentajeFin = Mathf.Clamp01((tiempoPasado + tiempoEspera) / tiempoTotal);

            while (tiempoSubida < tiempoEspera)
            {
                tiempoSubida += Time.deltaTime;
                float porcentajeActual = Mathf.Lerp(porcentajeInicio, porcentajeFin, tiempoSubida / tiempoEspera);
                
                // Llenamos la barra visual
                if (barraDeCarga != null) barraDeCarga.fillAmount = porcentajeActual;
                
                // Actualizamos el texto con 2 decimales
                if (textoPorcentaje != null) textoPorcentaje.text = (porcentajeActual * 100f).ToString("F2") + " %";
                
                yield return null; // Esperamos al siguiente frame de Unity
            }
            
            tiempoPasado += tiempoEspera;
        }

        // Nos aseguramos de que llegue al 100% exacto al terminar
        if (barraDeCarga != null) barraDeCarga.fillAmount = 1f;
        if (textoPorcentaje != null) textoPorcentaje.text = "100.00 %";
        
        // Esperamos medio segundito para que el jugador saboree el 100%
        yield return new WaitForSeconds(0.5f);

        // Apagamos este panel falso
        gameObject.SetActive(false);
    }
}