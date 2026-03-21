using UnityEngine;

public class PuertaControl : MonoBehaviour
{
    public Transform pivotIzq;
    public Transform pivotDer;
    
    public bool abierta = false;
    public float anguloApertura = 90f;
    public float velocidad = 5f;

    private Quaternion rotacionCerradaIzq;
    private Quaternion rotacionAbiertaIzq;
    private Quaternion rotacionCerradaDer;
    private Quaternion rotacionAbiertaDer;

    void Start()
    {
        rotacionCerradaIzq = pivotIzq.localRotation;
        rotacionAbiertaIzq = Quaternion.Euler(0, -anguloApertura, 0);
        
        rotacionCerradaDer = pivotDer.localRotation;
        rotacionAbiertaDer = Quaternion.Euler(0, anguloApertura, 0);
    }

    void Update()
    {
        // Movimiento suave de la puerta izquierda
        Quaternion rotacionMetaIzq = abierta ? rotacionAbiertaIzq : rotacionCerradaIzq;
        pivotIzq.localRotation = Quaternion.Slerp(pivotIzq.localRotation, rotacionMetaIzq, Time.deltaTime * velocidad);

        // Movimiento suave de la puerta derecha
        Quaternion rotacionMetaDer = abierta ? rotacionAbiertaDer : rotacionCerradaDer;
        pivotDer.localRotation = Quaternion.Slerp(pivotDer.localRotation, rotacionMetaDer, Time.deltaTime * velocidad);
    }

    public void AlternarPuerta()
    {
        abierta = !abierta;
    }
}