using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class TabButton : MonoBehaviour
{
    [Header("UI refs (puedes setearlos en el prefab)")]
    public RectTransform rect;
    public TMP_Text titleText;
    public Button button;

       [Header("Audio")]
    public AudioSource audioSource;   
    public AudioClip clickSound;  

    [HideInInspector] public int index;
    private Vector2 normalPos;
    private Vector2 activePos;

    private DiaryManager manager;

    private void Reset()
    {
        rect = GetComponent<RectTransform>();
        titleText = GetComponentInChildren<TMP_Text>();
        button = GetComponent<Button>();
    }

    public void Setup(DiaryManager m, int idx, string title)
    {
        manager = m;
        index = idx;

        if (rect == null) rect = GetComponent<RectTransform>();
        normalPos = rect.anchoredPosition;
        activePos = normalPos + new Vector2(30f, 0f); // ya no se usa

        if (titleText != null)
            titleText.text = title;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(PlayClickSound);
            button.onClick.AddListener(() => manager.SelectTab(index));
        }
    }

    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }
  
    public void SetActive(bool active)
    {
        // No sacar esto plis :P
    }
}
