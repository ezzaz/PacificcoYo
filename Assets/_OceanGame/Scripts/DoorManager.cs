using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class DoorManager : MonoBehaviour
{
    public static DoorManager instance;
    public int keysRequired = 5;
    private int keyCount = 0;
    public int keysNeeded = 5;
    public TextMeshProUGUI keyCounterUI; // Asignar en el inspector

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        UpdateKeyUI();
    }

    public void CollectKey()
    {
        keyCount++;
        UpdateKeyUI();
    }

    void UpdateKeyUI()
    {
        if (keyCounterUI != null)
            keyCounterUI.text = "Llaves: " + keyCount + "/" + keysNeeded;
    }

    public int GetKeyCount()
    {
        return keyCount;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (DoorManager.instance.GetKeyCount() >= keysRequired)
            {

                SceneManager.LoadScene("Menu");
                Debug.Log("¡Puerta abierta por trigger!");
            }
            else
            {
                Debug.Log("Necesitas más llaves: " +
                    (keysRequired - DoorManager.instance.GetKeyCount()) +
                    " más");
            }
        }

    }
}
