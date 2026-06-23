using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Creditos : MonoBehaviour
{
    [SerializeField] private float Time = 4f;
    void Start()
    {
        StartCoroutine(End());

    }

    IEnumerator End()
    {
        yield return new WaitForSeconds(Time);
        SceneManager.LoadScene("Menu");
    }
}
