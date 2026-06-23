using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Page : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public Image icon;

    public void SetInfo(ItemData data)
    {
        titleText.text = data.itemName;
        descriptionText.text = data.description;
        if (icon != null) icon.sprite = data.icon;
    }
}
