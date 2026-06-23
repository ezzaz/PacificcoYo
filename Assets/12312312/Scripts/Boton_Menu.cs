using System.Runtime.Serialization;
using UnityEngine;

public class Boton_Menu : Botones
{
    [SerializeField] bool activo;
    [SerializeField] GameObject menuPausa;

    private void Update()
    {
        if (respuesta_si)
        { activo = true; }
    }
}
