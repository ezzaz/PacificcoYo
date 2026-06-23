using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiaryManager : MonoBehaviour
{
    public static DiaryManager Instance { get; private set; }

    [Header("References")]
    public Transform tabsContainer;    // contenedor de tabs (ej. HorizontalLayoutGroup)
    public Transform pagesContainer;   // contenedor de páginas (un panel donde instancias las páginas)

    [Header("Prefabs")]
    public GameObject tabPrefab;
    public GameObject pagePrefab;

    public List<TabButton> tabs = new List<TabButton>();
    public List<Page> pages = new List<Page>();
    public Dictionary<string, int> indexByName = new Dictionary<string, int>(); // evita duplicados por nombre

    private int currentIndex = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddPage(ItemData data)
    {
        if (data == null)
        {
            Debug.LogWarning("DiaryManager.AddPage recibió ItemData null.");
            return;
        }

        // Si ya existe una página para este item, simplemente selecciónala.
        if (indexByName.TryGetValue(data.itemName, out int existingIndex))
        {
            SelectTab(existingIndex);
            return;
        }

        // Crear pestaña (tab)
        GameObject newTabObj = Instantiate(tabPrefab, tabsContainer);
        TabButton newTab = newTabObj.GetComponent<TabButton>();
        int newIndex = tabs.Count; // índice para la nueva pestaña
        newTab.Setup(this, newIndex, data.itemName);
        tabs.Add(newTab);

        // Crear página
        GameObject newPageObj = Instantiate(pagePrefab, pagesContainer);
        Page page = newPageObj.GetComponent<Page>();
        page.SetInfo(data);
        pages.Add(page);
        newPageObj.SetActive(false);

        indexByName[data.itemName] = newIndex;

        // Forzar rebuild del layout para que el UI actualice tamaños/integridad inmediatamente
        if (tabsContainer is RectTransform rt)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        }

        // Seleccionar la nueva pestaña
        SelectTab(newIndex);
    }

    public void SelectTab(int index)
    {
        if (index < 0 || index >= pages.Count)
            return;

        currentIndex = index;

        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].gameObject.SetActive(i == index);
        }

        for (int i = 0; i < tabs.Count; i++)
        {
            tabs[i].SetActive(i == index);
        }
    }
}