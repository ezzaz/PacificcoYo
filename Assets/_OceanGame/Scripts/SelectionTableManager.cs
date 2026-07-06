using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionTableManager : MonoBehaviour
{
    public static SelectionTableManager Instance;

    public enum ItemType
    {
        AdultFish,    // Correct: Guardar (Cesta)
        BabyFish,     // Correct: Liberar (Mar)
        ProtectedFish,// Correct: Liberar (Mar)
        Trash         // Correct: Guardar (Reciclar)
    }

    [System.Serializable]
    public class InspectableItem
    {
        public string name;
        public string description;
        public string comment;
        public string sizeText;
        public ItemType type;
        public Color modelColor;
        public Vector3 spawnOffset;
        public Vector3 modelScale;

        [HideInInspector] public GameObject spawnedObject;
        [HideInInspector] public bool isSorted = false;
    }

    [Header("Inspectable Items Database")]
    public List<InspectableItem> items = new List<InspectableItem>();

    [Header("Table Configuration")]
    [SerializeField] private Vector3 tablePositionOffset = new Vector3(0f, -0.6f, 1.8f); // relative to camera
    [SerializeField] private Vector3 tableScale = new Vector3(1.6f, 0.7f, 0.9f);

    private GameObject tableObject;
    private Camera mainCamera;
    private InspectableItem selectedItem = null;
    private bool isMiniGameActive = false;
    private int sortedCount = 0;

    // HUD Text styles
    private string feedbackMessage = "";
    private float feedbackTimer = 0f;
    private Texture2D darkBlueTexture;
    private Texture2D skyBlueBorderTexture;
    private Texture2D buttonNormalTex;
    private Texture2D buttonHoverTex;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindFirstObjectByType<Camera>();
        }

        // Setup custom styles
        darkBlueTexture = CreateColorTexture(new Color(0.02f, 0.08f, 0.18f, 0.9f));
        skyBlueBorderTexture = CreateColorTexture(new Color(0.2f, 0.6f, 0.9f, 0.8f));
        buttonNormalTex = CreateColorTexture(new Color(0.1f, 0.25f, 0.45f, 1.0f));
        buttonHoverTex = CreateColorTexture(new Color(0.15f, 0.4f, 0.7f, 1.0f));

        // Create default list of items if empty (protects scene setup)
        if (items.Count == 0)
        {
            InitializeDefaultItems();
        }

        // Spawn Table and items in front of the player
        StartCoroutine(SetupTableRoutine());
    }

    private Texture2D CreateColorTexture(Color col)
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, col);
        tex.Apply();
        return tex;
    }

    private void InitializeDefaultItems()
    {
        items.Add(new InspectableItem()
        {
            name = "Salmón del Alba Serena",
            description = "Un pez vigoroso y sano con escamas rojizas brillantes.",
            comment = "Tiene un tamaño excelente para el almuerzo de hoy.",
            sizeText = "35 cm (Mínimo legal: 22 cm)",
            type = ItemType.AdultFish,
            modelColor = new Color(0.9f, 0.4f, 0.4f),
            spawnOffset = new Vector3(-0.5f, 0.4f, -0.2f),
            modelScale = new Vector3(0.15f, 0.15f, 0.45f)
        });

        items.Add(new InspectableItem()
        {
            name = "Pez de Colores Fantasía (Cría)",
            description = "Una cría pequeña y vivaz que brilla con destellos rosados.",
            comment = "Aún es muy joven... debe regresar al océano para crecer fuerte.",
            sizeText = "8 cm (Mínimo legal: 15 cm)",
            type = ItemType.BabyFish,
            modelColor = new Color(1f, 0.6f, 0.8f),
            spawnOffset = new Vector3(-0.2f, 0.4f, 0.2f),
            modelScale = new Vector3(0.09f, 0.09f, 0.22f)
        });

        items.Add(new InspectableItem()
        {
            name = "Botella de Plástico Sucia",
            description = "Un residuo plástico marino que flotaba cerca de la orilla.",
            comment = "El plástico daña el ecosistema. Lo reciclaré de forma segura en tierra.",
            sizeText = "Objeto Inorgánico (Residuo)",
            type = ItemType.Trash,
            modelColor = new Color(0.3f, 0.7f, 1f, 0.5f), // transparent blue
            spawnOffset = new Vector3(0.1f, 0.4f, -0.2f),
            modelScale = new Vector3(0.1f, 0.3f, 0.1f)
        });

        items.Add(new InspectableItem()
        {
            name = "Trucha Plateada del Atardecer",
            description = "Un ejemplar maduro y sano que habita en aguas medias.",
            comment = "Desarrollada perfectamente y de tamaño reglamentario.",
            sizeText = "28 cm (Mínimo legal: 20 cm)",
            type = ItemType.AdultFish,
            modelColor = new Color(0.7f, 0.7f, 0.8f),
            spawnOffset = new Vector3(0.4f, 0.4f, 0.1f),
            modelScale = new Vector3(0.14f, 0.14f, 0.38f)
        });

        items.Add(new InspectableItem()
        {
            name = "Estrella de Mar Mística",
            description = "Una estrella de cinco puntas de color dorado brillante.",
            comment = "Las estrellas de mar son especies protegidas en esta bahía. Debe ser liberada.",
            sizeText = "Especie Protegida (Conservación)",
            type = ItemType.ProtectedFish,
            modelColor = new Color(1f, 0.7f, 0.1f),
            spawnOffset = new Vector3(-0.4f, 0.4f, 0.15f),
            modelScale = new Vector3(0.18f, 0.18f, 0.06f)
        });

        items.Add(new InspectableItem()
        {
            name = "Lata de Aluminio Oxidada",
            description = "Una lata metálica de refresco que contamina el muelle.",
            comment = "Basura que daña las branquias de los peces. Me la llevaré para reciclar.",
            sizeText = "Objeto Inorgánico (Metal)",
            type = ItemType.Trash,
            modelColor = new Color(0.6f, 0.4f, 0.3f),
            spawnOffset = new Vector3(0.3f, 0.4f, -0.15f),
            modelScale = new Vector3(0.12f, 0.18f, 0.12f)
        });
    }

    private IEnumerator SetupTableRoutine()
    {
        // Wait briefly for cinematic controllers to initialize
        yield return new WaitForSeconds(0.2f);

        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null) yield break;

        // Position table in front of camera
        Vector3 tablePos = mainCamera.transform.position + mainCamera.transform.forward * tablePositionOffset.z;
        tablePos.y += tablePositionOffset.y;

        // Create the wooden table block
        tableObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tableObject.name = "Selection_Table";
        tableObject.transform.position = tablePos;
        tableObject.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
        tableObject.transform.localScale = tableScale;

        // Give table a nice wooden color
        Material woodMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        woodMat.color = new Color(0.38f, 0.23f, 0.12f); // wood brown
        woodMat.SetFloat("_Smoothness", 0.1f);
        tableObject.GetComponent<Renderer>().material = woodMat;

        // Spawn items on top of the table
        for (int i = 0; i < items.Count; i++)
        {
            InspectableItem item = items[i];
            
            // Create item base model (Cylinder represents a fish shape nicely, spheres for star/bottle)
            GameObject itemObj;
            if (item.type == ItemType.ProtectedFish)
            {
                itemObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            }
            else if (item.type == ItemType.Trash)
            {
                itemObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            }
            else
            {
                itemObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                // Rotate cylinder so it lies flat like a fish
                itemObj.transform.rotation = Quaternion.Euler(0, 0, 90f);
            }

            itemObj.name = "TableItem_" + i;
            
            // Absolute spawn position combining table top center with the offset
            Vector3 worldSpawnPos = tableObject.transform.TransformPoint(item.spawnOffset);
            itemObj.transform.position = worldSpawnPos;
            itemObj.transform.localScale = item.modelScale;

            // Material
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = item.modelColor;
            
            // Enable transparency for plastic bottle
            if (item.name.Contains("Botella"))
            {
                mat.SetFloat("_Surface", 1); // Transparent
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }

            itemObj.GetComponent<Renderer>().material = mat;

            // Associate the GameObject and a tag/component for Raycasting
            item.spawnedObject = itemObj;
        }

        isMiniGameActive = true;
    }

    private void Update()
    {
        if (!isMiniGameActive) return;

        // Handle feedback timer
        if (feedbackTimer > 0f)
        {
            feedbackTimer -= Time.deltaTime;
            if (feedbackTimer <= 0f)
            {
                feedbackMessage = "";
            }
        }

        // Point and click: Mouse left click detection (New Input System)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            // Do not raycast if clicking over GUI controls
            if (mousePosition.x > Screen.width - 420f && selectedItem != null)
            {
                return; 
            }

            Ray ray = mainCamera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Find if we hit one of our inspectable items
                foreach (var item in items)
                {
                    if (!item.isSorted && item.spawnedObject == hit.collider.gameObject)
                    {
                        selectedItem = item;
                        ShowFeedback($"Inspeccionando: {item.name.ToUpper()}", 1.5f);
                        break;
                    }
                }
            }
        }
    }

    public void CategorizeSelection(bool saveInBasket)
    {
        if (selectedItem == null) return;

        bool isCorrect = false;

        switch (selectedItem.type)
        {
            case ItemType.AdultFish:
                if (saveInBasket) isCorrect = true; // correct: save
                break;

            case ItemType.Trash:
                if (saveInBasket) isCorrect = true; // correct: save to recycle
                break;

            case ItemType.BabyFish:
                if (!saveInBasket) isCorrect = true; // correct: release
                break;

            case ItemType.ProtectedFish:
                if (!saveInBasket) isCorrect = true; // correct: release
                break;
        }

        if (isCorrect)
        {
            // Successful categorization
            selectedItem.isSorted = true;
            sortedCount++;

            string actionMsg = saveInBasket ? "guardado en la cesta." : "devuelto al mar.";
            if (selectedItem.type == ItemType.Trash)
            {
                ShowFeedback($"¡Correcto! {selectedItem.name} clasificado para reciclaje.", 3f);
            }
            else
            {
                ShowFeedback($"¡Correcto! {selectedItem.name} {actionMsg}", 3f);
            }

            // Animate object disappearing (fly effect)
            StartCoroutine(AnimateItemOutCoroutine(selectedItem.spawnedObject, saveInBasket));

            selectedItem = null;

            // Check if all items are sorted
            if (sortedCount >= items.Count)
            {
                StartCoroutine(CompleteLevelCoroutine());
            }
        }
        else
        {
            // Incorrect choice: Carlos provides gentle advice
            if (selectedItem.type == ItemType.BabyFish || selectedItem.type == ItemType.ProtectedFish)
            {
                ShowFeedback($"Carlos: \"Mmm... creo que {selectedItem.name} debería ser liberado al océano para cuidar el ecosistema.\"", 4f);
            }
            else if (selectedItem.type == ItemType.AdultFish)
            {
                ShowFeedback($"Carlos: \"Este pez es adulto y de buen tamaño. Deberíamos guardarlo en la cesta para aprovechar el alimento.\"", 4f);
            }
            else if (selectedItem.type == ItemType.Trash)
            {
                ShowFeedback($"Carlos: \"¡No podemos tirar plástico de vuelta al mar! Debería guardarlo en la cesta de reciclaje.\"", 4f);
            }
        }
    }

    private IEnumerator AnimateItemOutCoroutine(GameObject itemObj, bool towardsBasket)
    {
        if (itemObj == null) yield break;

        // Simple smooth slide off the table
        Vector3 startPos = itemObj.transform.position;
        Vector3 targetPos = startPos;
        if (towardsBasket)
        {
            targetPos += mainCamera.transform.right * 2.5f - mainCamera.transform.up * 1f; // slide right/down (basket)
        }
        else
        {
            targetPos += -mainCamera.transform.right * 2.5f + mainCamera.transform.up * 1.5f; // slide left/up (sea)
        }

        float elapsed = 0f;
        float duration = 0.6f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            if (itemObj != null)
            {
                itemObj.transform.position = Vector3.Lerp(startPos, targetPos, t);
                itemObj.transform.localScale = Vector3.Lerp(itemObj.transform.localScale, Vector3.zero, t);
            }
            yield return null;
        }

        Destroy(itemObj);
    }

    private IEnumerator CompleteLevelCoroutine()
    {
        isMiniGameActive = false;
        yield return new WaitForSeconds(1.5f);

        // Notify GameplayCinematicController that Minijuego 4 is complete!
        if (GameplayCinematicController.Instance != null)
        {
            GameplayCinematicController.Instance.NotifyMinijuego4Complete();
        }
        else
        {
            Debug.LogWarning("GameplayCinematicController instance not found. Completion skipped.");
        }
    }

    private void ShowFeedback(string message, float duration)
    {
        feedbackMessage = message;
        feedbackTimer = duration;
    }

    private void OnGUI()
    {
        if (!isMiniGameActive) return;

        // Force Unlock Cursor for Point & Click
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 1. Draw top progress and instructions
        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontSize = 20;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.normal.textColor = Color.yellow;

        Rect titleRect = new Rect(0, 30, Screen.width, 35);
        string progressText = $"Mesa de Conservación: Clasificados {sortedCount} de {items.Count} objetos";
        GUI.Label(new Rect(titleRect.x + 2, titleRect.y + 2, titleRect.width, titleRect.height), progressText, new GUIStyle(titleStyle) { normal = { textColor = Color.black } });
        GUI.Label(titleRect, progressText, titleStyle);

        GUIStyle subStyle = new GUIStyle(GUI.skin.label);
        subStyle.alignment = TextAnchor.MiddleCenter;
        subStyle.fontSize = 14;
        subStyle.normal.textColor = Color.white;
        Rect subRect = new Rect(0, 65, Screen.width, 25);
        GUI.Label(subRect, "Haz clic en cada objeto sobre la mesa para inspeccionarlo", subStyle);

        // 2. Draw Carlos's live feedback dialog/bubble (bottom center)
        if (!string.IsNullOrEmpty(feedbackMessage))
        {
            float boxWidth = 560f;
            float boxHeight = 110f;
            float xPos = (Screen.width - boxWidth) / 2f;
            float yPos = Screen.height - boxHeight - 40f;

            Rect boxRect = new Rect(xPos, yPos, boxWidth, boxHeight);
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.normal.background = darkBlueTexture;
            GUI.Box(boxRect, GUIContent.none, boxStyle);

            // Sky-blue borders
            GUI.Box(new Rect(boxRect.x, boxRect.y, 3, boxRect.height), GUIContent.none, new GUIStyle() { normal = { background = skyBlueBorderTexture } });
            GUI.Box(new Rect(boxRect.x + boxRect.width - 3, boxRect.y, 3, boxRect.height), GUIContent.none, new GUIStyle() { normal = { background = skyBlueBorderTexture } });
            GUI.Box(new Rect(boxRect.x, boxRect.y, boxRect.width, 3), GUIContent.none, new GUIStyle() { normal = { background = skyBlueBorderTexture } });
            GUI.Box(new Rect(boxRect.x, boxRect.y + boxRect.height - 3, boxRect.width, 3), GUIContent.none, new GUIStyle() { normal = { background = skyBlueBorderTexture } });

            GUIStyle fbStyle = new GUIStyle(GUI.skin.label);
            fbStyle.alignment = TextAnchor.MiddleCenter;
            fbStyle.fontSize = 16;
            fbStyle.wordWrap = true;
            fbStyle.normal.textColor = Color.white;

            GUI.Label(new Rect(boxRect.x + 20, boxRect.y + 15, boxRect.width - 40, boxRect.height - 30), feedbackMessage, fbStyle);
        }

        // 3. Draw Papers Please Inspection Card (Right panel)
        if (selectedItem != null)
        {
            float cardWidth = 380f;
            float cardHeight = Screen.height - 180f;
            float cardX = Screen.width - cardWidth - 30f;
            float cardY = 90f;

            Rect cardRect = new Rect(cardX, cardY, cardWidth, cardHeight);
            GUIStyle cardStyle = new GUIStyle(GUI.skin.box);
            cardStyle.normal.background = darkBlueTexture;
            GUI.Box(cardRect, GUIContent.none, cardStyle);

            // Sky-blue border
            GUI.Box(new Rect(cardRect.x, cardRect.y, 3, cardRect.height), GUIContent.none, new GUIStyle() { normal = { background = skyBlueBorderTexture } });
            GUI.Box(new Rect(cardRect.x + cardRect.width - 3, cardRect.y, 3, cardRect.height), GUIContent.none, new GUIStyle() { normal = { background = skyBlueBorderTexture } });
            GUI.Box(new Rect(cardRect.x, cardRect.y, cardRect.width, 3), GUIContent.none, new GUIStyle() { normal = { background = skyBlueBorderTexture } });
            GUI.Box(new Rect(cardRect.x, cardRect.y + cardRect.height - 3, cardRect.width, 3), GUIContent.none, new GUIStyle() { normal = { background = skyBlueBorderTexture } });

            // Title
            GUIStyle inspectTitleStyle = new GUIStyle(GUI.skin.label);
            inspectTitleStyle.alignment = TextAnchor.MiddleCenter;
            inspectTitleStyle.fontSize = 18;
            inspectTitleStyle.fontStyle = FontStyle.Bold;
            inspectTitleStyle.normal.textColor = Color.yellow;
            GUI.Label(new Rect(cardRect.x + 10, cardRect.y + 20, cardWidth - 20, 30), "TARJETA DE SELECCIÓN", inspectTitleStyle);

            // Divider Line
            GUI.Box(new Rect(cardRect.x + 20, cardRect.y + 55, cardWidth - 40, 2), GUIContent.none, new GUIStyle() { normal = { background = skyBlueBorderTexture } });

            // Name
            GUIStyle nameStyle = new GUIStyle(GUI.skin.label);
            nameStyle.fontSize = 17;
            nameStyle.fontStyle = FontStyle.Bold;
            nameStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(cardRect.x + 25, cardRect.y + 70, cardWidth - 50, 30), $"Nombre: {selectedItem.name}", nameStyle);

            // Size / Specs
            GUIStyle specStyle = new GUIStyle(GUI.skin.label);
            specStyle.fontSize = 14;
            specStyle.fontStyle = FontStyle.Italic;
            specStyle.normal.textColor = new Color(0.2f, 0.8f, 1f);
            GUI.Label(new Rect(cardRect.x + 25, cardRect.y + 110, cardWidth - 50, 25), $"Especificación: {selectedItem.sizeText}", specStyle);

            // Description Box
            GUIStyle descStyle = new GUIStyle(GUI.skin.label);
            descStyle.fontSize = 13;
            descStyle.wordWrap = true;
            descStyle.normal.textColor = new Color(0.85f, 0.85f, 0.85f);
            GUI.Label(new Rect(cardRect.x + 25, cardRect.y + 145, cardWidth - 50, 60), $"Descripción:\n{selectedItem.description}", descStyle);

            // Carlos's reflection
            GUIStyle reflectStyle = new GUIStyle(GUI.skin.label);
            reflectStyle.fontSize = 13;
            reflectStyle.wordWrap = true;
            reflectStyle.fontStyle = FontStyle.Italic;
            reflectStyle.normal.textColor = Color.yellow;
            GUI.Label(new Rect(cardRect.x + 25, cardRect.y + 215, cardWidth - 50, 70), $"Carlos:\n\"{selectedItem.comment}\"", reflectStyle);

            // Categorization Buttons (At the bottom of the card)
            float btnWidth = cardWidth - 60f;
            float btnHeight = 45f;
            float btnX = cardRect.x + 30f;

            GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
            btnStyle.normal.background = buttonNormalTex;
            btnStyle.hover.background = buttonHoverTex;
            btnStyle.normal.textColor = Color.white;
            btnStyle.fontSize = 13;
            btnStyle.fontStyle = FontStyle.Bold;
            btnStyle.alignment = TextAnchor.MiddleCenter;

            // 1st Button: Liberar
            string releaseLabel = (selectedItem.type == ItemType.Trash) ? "[ RECHAZAR AL MAR ]" : "[ 🌊 DEVOLVER AL MAR ]";
            if (GUI.Button(new Rect(btnX, cardRect.yMax - 130f, btnWidth, btnHeight), releaseLabel, btnStyle))
            {
                CategorizeSelection(false); // false = Release
            }

            // 2nd Button: Guardar
            string saveLabel = (selectedItem.type == ItemType.Trash) ? "[ ♻️ CLASIFICAR PARA RECICLAR ]" : "[ 🧺 GUARDAR EN CESTA ]";
            if (GUI.Button(new Rect(btnX, cardRect.yMax - 70f, btnWidth, btnHeight), saveLabel, btnStyle))
            {
                CategorizeSelection(true); // true = Save
            }
        }
    }
}