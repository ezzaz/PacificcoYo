using UnityEngine;

public class Completado : MonoBehaviour
{
    public GameObject[] slots;
    public GameObject salida;
    public int numeroMax, numeroActual;

    void Start()
    {
        numeroMax = slots.Length;
        salida.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (numeroActual == numeroMax) 
            salida.SetActive(true);
    }
}
