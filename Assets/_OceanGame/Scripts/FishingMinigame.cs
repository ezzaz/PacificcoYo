using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingMinigame : MonoBehaviour
{
    private enum FishingState
    {
        Idle,
        Casting,
        WaitingForBite,
        BiteActive,
        Reeling
    }

    [Header("Fishing Settings")]
    [SerializeField] private float minWaitTime = 3f;
    [SerializeField] private float maxWaitTime = 7f;
    [SerializeField] private float biteDuration = 2f;
    [SerializeField] private float castDistance = 12f;

    [Header("Visual Prefabs (Optional)")]
    [SerializeField] private GameObject customBobberPrefab;

    // State
    private FishingState currentState = FishingState.Idle;
    private float stateTimer = 0f;
    private GameObject activeBobber;
    private Waves oceanWaves;
    private Transform playerCameraTransform;

    // Contemplative text messages
    private string hudMessage = "";
    private string statusMessage = "";
    private float messageDisplayTimer = 0f;

    // Fish database for variety
    private struct FishType
    {
        public string Name;
        public string Description;
        public float Points;
        public Color TextColor;

        public FishType(string name, string desc, float points, Color color)
        {
            Name = name;
            Description = desc;
            Points = points;
            TextColor = color;
        }
    }

    private readonly FishType[] fishDatabase = new FishType[]
    {
        new FishType("Trucha Plateada del Atardecer", "Un pez pacífico que brilla con los últimos rayos de sol.", 15f, new Color(0.9f, 0.9f, 1f)),
        new FishType("Salmón del Alba Serena", "Nada con gracia contra la corriente del océano.", 20f, new Color(1f, 0.8f, 0.8f)),
        new FishType("Pez de Colores Fantasía", "Pequeño y vivaz, parece sacado de un sueño.", 10f, new Color(1f, 0.9f, 0.6f)),
        new FishType("Cangrejo Esmeralda Brillante", "Camina despacio por la arena del fondo marino.", 8f, new Color(0.7f, 1f, 0.7f)),
        new FishType("Estrella de Mar Mística", "Descansa inmóvil en las profundidades doradas.", 12f, new Color(0.9f, 0.7f, 1f)),
        new FishType("Caracola del Eco Eterno", "Si te la acercas al oído, puedes oír el canto de las ballenas.", 5f, new Color(1f, 1f, 0.8f)),
        new FishType("Bota de Cuero Vieja", "Alguien la perdió hace mucho tiempo. Tiene algas pegadas.", 3f, new Color(0.7f, 0.6f, 0.5f))
    };

    private void Start()
    {
        oceanWaves = FindFirstObjectByType<Waves>();
        
        // Find camera
        Camera cam = GetComponentInChildren<Camera>();
        if (cam != null)
        {
            playerCameraTransform = cam.transform;
        }
        else if (Camera.main != null)
        {
            playerCameraTransform = Camera.main.transform;
        }
        else
        {
            playerCameraTransform = transform;
        }
    }

    private void Update()
    {
        if (oceanWaves == null)
        {
            oceanWaves = FindFirstObjectByType<Waves>();
        }

        // Fade out status messages over time
        if (messageDisplayTimer > 0f)
        {
            messageDisplayTimer -= Time.deltaTime;
            if (messageDisplayTimer <= 0f)
            {
                statusMessage = "";
            }
        }

        // Check if player is near water (on beach / lower altitude)
        bool isNearWater = transform.position.y < 8.0f;

        // Process inputs
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (isNearWater)
            {
                HandleFishingAction();
            }
            else if (currentState == FishingState.Idle)
            {
                ShowStatus("Debes estar más cerca del mar para pescar...", 3f);
            }
        }

        // Update active fishing states
        switch (currentState)
        {
            case FishingState.WaitingForBite:
                UpdateBobberBuoyancy(0f);
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    TriggerBite();
                }
                break;

            case FishingState.BiteActive:
                // Make the bobber submerge and vibrate
                float vibrate = Mathf.Sin(Time.time * 50f) * 0.05f;
                UpdateBobberBuoyancy(-0.8f + vibrate);
                
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    LoseFish();
                }
                break;

            case FishingState.Idle:
                if (isNearWater && activeBobber == null)
                {
                    hudMessage = "Presiona [F] para lanzar la caña de pescar";
                }
                else
                {
                    hudMessage = "";
                }
                break;
        }
    }

    private void HandleFishingAction()
    {
        switch (currentState)
        {
            case FishingState.Idle:
                StartCoroutine(CastLineCoroutine());
                break;

            case FishingState.WaitingForBite:
                // Pulled too early
                ShowStatus("Recogiste el sedal antes de tiempo... El pez huyó.", 3f);
                ResetToIdle();
                break;

            case FishingState.BiteActive:
                // Successful catch!
                ReelInCatch();
                break;
        }
    }

    private IEnumerator CastLineCoroutine()
    {
        currentState = FishingState.Casting;
        hudMessage = "";
        ShowStatus("Lanzando la caña con paciencia...", 2f);

        // Instantiate bobber
        if (customBobberPrefab != null)
        {
            activeBobber = Instantiate(customBobberPrefab);
        }
        else
        {
            // Build procedural bobber
            activeBobber = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            activeBobber.name = "Fishing_Bobber";
            activeBobber.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            
            // Red top
            activeBobber.GetComponent<Renderer>().material.color = Color.red;

            // Small white antenna
            GameObject antenna = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Destroy(antenna.GetComponent<Collider>());
            antenna.transform.parent = activeBobber.transform;
            antenna.transform.localPosition = new Vector3(0, 1.2f, 0);
            antenna.transform.localScale = new Vector3(0.2f, 0.8f, 0.2f);
            antenna.GetComponent<Renderer>().material.color = Color.white;
        }

        // Remove active physics collider to prevent issues
        if (activeBobber.TryGetComponent<Collider>(out var col))
        {
            Destroy(col);
        }

        Vector3 startPos = playerCameraTransform.position + playerCameraTransform.forward * 1.5f - Vector3.up * 0.5f;
        Vector3 targetPos = playerCameraTransform.position + playerCameraTransform.forward * castDistance;
        
        // Find ocean level at target
        if (oceanWaves != null)
        {
            targetPos.y = oceanWaves.GetHeight(targetPos);
        }
        else
        {
            targetPos.y = 3.5f;
        }

        // Animate flying parabola
        float duration = 1.2f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
            // Parabolic arc
            currentPos.y += Mathf.Sin(t * Mathf.PI) * 4f;
            
            if (activeBobber != null)
            {
                activeBobber.transform.position = currentPos;
            }
            yield return null;
        }

        // Settle on water
        currentState = FishingState.WaitingForBite;
        stateTimer = Random.Range(minWaitTime, maxWaitTime);
        hudMessage = "Espera con paciencia... El sedal flota tranquilo";
        ShowStatus("...Silencio... El agua se mueve pacíficamente...", 4f);
    }

    private void UpdateBobberBuoyancy(float verticalOffset)
    {
        if (activeBobber == null) return;

        Vector3 pos = activeBobber.transform.position;
        float waveHeight = 3.5f;
        if (oceanWaves != null)
        {
            waveHeight = oceanWaves.GetHeight(pos);
        }
        pos.y = waveHeight + verticalOffset;
        activeBobber.transform.position = pos;
    }

    private void TriggerBite()
    {
        currentState = FishingState.BiteActive;
        stateTimer = biteDuration;
        hudMessage = "¡HA PICADO! ¡Presiona [F] ahora!";
        ShowStatus("¡El flotador se hunde! ¡Tira de la caña!", biteDuration);
    }

    private void ReelInCatch()
    {
        currentState = FishingState.Reeling;
        hudMessage = "";

        // Choose random fish
        int fishIdx = Random.Range(0, fishDatabase.Length);
        FishType caught = fishDatabase[fishIdx];

        int caughtSoFar = 1;
        int required = 3;
        if (GameplayCinematicController.Instance != null)
        {
            caughtSoFar = GameplayCinematicController.Instance.GetFishCaughtCount() + 1;
            required = GameplayCinematicController.Instance.fishRequiredToCatch;
        }

        string successMsg = $"¡Has pescado un {caught.Name.ToUpper()}!\n\n\"{caught.Description}\"\n\n(+{caught.Points} puntos)\n\nProgreso: {caughtSoFar} de {required} peces.";
        ShowStatus(successMsg, 6f);

        // Award score
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(caught.Points);
        }

        if (GameplayCinematicController.Instance != null)
        {
            GameplayCinematicController.Instance.NotifyFishCaught();
        }

        // Visual effects (flash bobber green and destroy)
        StartCoroutine(CatchEffectsCoroutine());
    }

    private IEnumerator CatchEffectsCoroutine()
    {
        if (activeBobber != null)
        {
            var ren = activeBobber.GetComponent<Renderer>();
            if (ren != null) ren.material.color = Color.green;
            yield return new WaitForSeconds(0.5f);
            Destroy(activeBobber);
            activeBobber = null;
        }
        ResetToIdle();
    }

    private void LoseFish()
    {
        ShowStatus("Se escapó... El mar vuelve a estar en calma.", 3f);
        ResetToIdle();
    }

    private void ResetToIdle()
    {
        if (activeBobber != null)
        {
            Destroy(activeBobber);
            activeBobber = null;
        }
        currentState = FishingState.Idle;
    }

    private void ShowStatus(string message, float duration)
    {
        statusMessage = message;
        messageDisplayTimer = duration;
    }

    private void OnDestroy()
    {
        if (activeBobber != null)
        {
            Destroy(activeBobber);
        }
    }

    // Modern and elegant screen HUD overlay
    private void OnGUI()
    {
        // 1. Draw top instruction/HUD message
        if (!string.IsNullOrEmpty(hudMessage))
        {
            GUIStyle hudStyle = new GUIStyle(GUI.skin.label);
            hudStyle.alignment = TextAnchor.MiddleCenter;
            hudStyle.fontSize = 22;
            hudStyle.fontStyle = FontStyle.Bold;
            hudStyle.normal.textColor = Color.yellow;

            // Shadow
            Rect hudRect = new Rect(0, Screen.height - 180, Screen.width, 40);
            GUI.Label(new Rect(hudRect.x + 2, hudRect.y + 2, hudRect.width, hudRect.height), hudMessage, new GUIStyle(hudStyle) { normal = { textColor = Color.black } });
            GUI.Label(hudRect, hudMessage, hudStyle);
        }

        // 2. Draw card container for statusMessage
        if (!string.IsNullOrEmpty(statusMessage))
        {
            // Dark elegant background box
            float boxWidth = 550f;
            float boxHeight = 150f;
            float xPos = (Screen.width - boxWidth) / 2f;
            float yPos = (Screen.height - boxHeight) / 2.5f;

            Rect boxRect = new Rect(xPos, yPos, boxWidth, boxHeight);
            
            // Texture style for box
            Texture2D bgTexture = new Texture2D(1, 1);
            bgTexture.SetPixel(0, 0, new Color(0f, 0.05f, 0.1f, 0.85f)); // Deep dark blue-black transparency
            bgTexture.Apply();

            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.normal.background = bgTexture;
            
            // Draw background
            GUI.Box(boxRect, GUIContent.none, boxStyle);

            // Draw border
            Texture2D borderTexture = new Texture2D(1, 1);
            borderTexture.SetPixel(0, 0, new Color(0.2f, 0.6f, 0.9f, 0.8f)); // Sky blue border
            borderTexture.Apply();
            
            GUIStyle borderStyle = new GUIStyle();
            borderStyle.normal.background = borderTexture;
            // Left, Right, Top, Bottom borders
            GUI.Box(new Rect(boxRect.x, boxRect.y, 3, boxRect.height), GUIContent.none, borderStyle);
            GUI.Box(new Rect(boxRect.x + boxRect.width - 3, boxRect.y, 3, boxRect.height), GUIContent.none, borderStyle);
            GUI.Box(new Rect(boxRect.x, boxRect.y, boxRect.width, 3), GUIContent.none, borderStyle);
            GUI.Box(new Rect(boxRect.x, boxRect.y + boxRect.height - 3, boxRect.width, 3), GUIContent.none, borderStyle);

            // Text layout
            GUIStyle textStyle = new GUIStyle(GUI.skin.label);
            textStyle.alignment = TextAnchor.MiddleCenter;
            textStyle.fontSize = 18;
            textStyle.wordWrap = true;
            textStyle.normal.textColor = Color.white;

            Rect textRect = new Rect(boxRect.x + 20, boxRect.y + 15, boxRect.width - 40, boxRect.height - 30);
            GUI.Label(textRect, statusMessage, textStyle);
        }
    }
}