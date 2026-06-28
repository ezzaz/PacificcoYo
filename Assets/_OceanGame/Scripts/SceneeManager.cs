using System.Collections;
using UnityEngine;

public class SceneeManager : MonoBehaviour
{
    [SerializeField] private GameObject FindKeytext;
    private float Wait = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Begin());
        FindKeytext.SetActive(true);
        Time.timeScale = 0f;
    }

    IEnumerator Begin()
    {
        Debug.Log("si es begin");
        yield return new WaitForSecondsRealtime(Wait);
        Time.timeScale = 1f;
        FindKeytext.SetActive(false);
        
    }


}
