using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DeformacionViento : BaseMeshEffect
{
    [Range(-15f, 15f)]
    public float intensidadViento = 0f; // Esto es cuánto se "estira" la punta

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive()) return;

        UIVertex vertex = new UIVertex();
        for (int i = 0; i < vh.currentVertCount; i++)
        {
            vh.PopulateUIVertex(ref vertex, i);
            
            // Si el pivote de tu imagen está en la base (Y=0), 
            // los puntos de arriba tienen una posición Y mayor a 0.
            if (vertex.position.y > 0.1f)
            {
                vertex.position.x += intensidadViento;
            }
            
            vh.SetUIVertex(vertex, i);
        }
    }

    // Agregamos esto para que la imagen se refresque constantemente y veamos el movimiento
    void Update()
    {
        if (graphic != null)
        {
            graphic.SetVerticesDirty();
        }
    }
}