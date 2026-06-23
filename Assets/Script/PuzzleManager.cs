using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    public SnapZone[] zonas;

    [Header("Escena siguiente")]
    public string siguienteEscena;

    private void Awake()
    {
        Instance = this;
    }

    public void ComprobarPuzzle()
    {
        foreach (SnapZone zona in zonas)
        {
            if (!zona.ocupado)
                return;
        }

        Debug.Log("Puzzle completado");

        SceneManager.LoadScene(siguienteEscena);
    }
}