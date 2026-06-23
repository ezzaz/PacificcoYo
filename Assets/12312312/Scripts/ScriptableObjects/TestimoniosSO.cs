
using UnityEngine;
[CreateAssetMenu(fileName = "Testimonio", menuName = "Testimonio")]

public class TestimoniosSO : ScriptableObject
{
    public string pregunta;
    public bool esVerdadera;
    public bool esSeleccionMultiple;
    public bool esLafinal;
    public string respuesta1, respuesta2, respuesta3, respuesta4;

    public string pensamiento1,pensamiento2,pensamiento3;

}