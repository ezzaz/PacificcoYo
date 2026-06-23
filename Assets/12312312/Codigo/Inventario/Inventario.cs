using UnityEngine;

public class Inventario : MonoBehaviour
{
    [SerializeField] KeyCode togglekey = KeyCode.Q;
    [SerializeField] GameObject uiContainer = null;

    void Start()
    {
        uiContainer.SetActive(false);
    }


    void Update()
    {
        if (Input.GetKeyDown(togglekey))
        {
            uiContainer.SetActive(!uiContainer.activeSelf);
        }
    }
}
