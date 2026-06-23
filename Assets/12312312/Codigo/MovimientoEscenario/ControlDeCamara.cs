using UnityEngine;

public class ControlDeCamara : MonoBehaviour
{
    public GameObject camara;
    public Vector3 posicionCamara;
    public GameObject[] posicionVacios;
    public float velocidadLerp;
    public int camaraActual;
    void Start()
    {
        camaraActual = 0;
    }

    // Update is called once per frame
    void Update()
    {
        posicionCamara = posicionVacios[camaraActual].transform.position;
    }

    private void FixedUpdate()
    {
        camara.transform.position = Vector3.Lerp(camara.transform.position,posicionCamara, velocidadLerp * Time.fixedDeltaTime);
    }
}
