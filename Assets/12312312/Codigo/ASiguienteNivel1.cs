using UnityEngine;
using UnityEngine.SceneManagement;

public class ASiguienteNivel1 : MonoBehaviour
{
    public Dialogos dialogo1, dialogo2;
    public ASiguienteNivel siguiente;
    bool yaApareci1, yaApareci2;

    private void Start()
    {
        yaApareci1 = false;
        yaApareci2 = false;
    }

    private void Update()
    {
        if (siguiente.objetosRecogidos == 1 && !yaApareci1)
        {
            dialogo1.StartDialogue(); yaApareci1 = true;
        }
        if (siguiente.objetosRecogidos >= siguiente.objetosARecojer.Length && !yaApareci2)
        {
            dialogo2.StartDialogue(); yaApareci2 = true;
        }
    }
}
