using UnityEngine;

public class CinematicaLines : MonoBehaviour
{
    public Dialogos dialogoScript;
    public float numActu, numMax;
    public GameObject botonContinuar;

    private void Start()
    {
        numMax = dialogoScript.dialogueLines.Count;
        botonContinuar.SetActive(false);
    }

    private void Update()
    {
        if (dialogoScript.lineIndex == numMax) 
        {
            botonContinuar.SetActive(true);
        }
    }
}
