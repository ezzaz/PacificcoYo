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
        MiniGame,
        Reeling
    }

    [Header("Fishing Settings")]
    [SerializeField] private float minWaitTime = 2f;
    [SerializeField] private float maxWaitTime = 5f;
    [SerializeField] private float biteDuration = 2.5f;
    [SerializeField] private float castDistance = 14f;

    [Header("Visual Prefabs (Optional)")]
    [SerializeField] private GameObject customBobberPrefab;

    // State
    private FishingState currentState = FishingState.Idle;
    private float stateTimer = 0f;
    private GameObject activeBobber;
    private Waves oceanWaves;

    public bool IsFishingActive()
    {
        return currentState != FishingState.Idle;
    }
    private Transform playerCameraTransform;

    // Fishing Zones
    private FishingZone currentZone = null;

    // Stardew Valley Minigame State Variables
    private float barY = 0.1f; // 0 (bottom) to 1 (top)
    private float barVelocity = 0f;
    private float barHeight = 0.5f; // height of green catcher bar (normalized)
    private float fishY = 0.4f; // 0 to 1
    private float fishTargetY = 0.5f;
    private float fishTimer = 0f;
    private float catchProgress = 0.35f; // starts with a little progress, 0 to 1
    private FishType currentActiveFish;
    private float fishDifficultyFactor = 1.0f;

    // Contemplative text messages
    private string hudMessage = "";
    private string statusMessage = "";
    private float messageDisplayTimer = 0f;

    // Texture Styles for OnGUI
    private Texture2D greenTexture;
    private Texture2D redTexture;
    private Texture2D yellowTexture;
    private Texture2D darkBlueTexture;
    private Texture2D grayTexture;

    // Fish database with unique behaviors and difficulty factors
    private struct FishType
    {
        public string Name;
        public string Description;
        public float Points;
        public Color TextColor;
        public float Difficulty; // 0.1 (very easy) to 2.0 (super wild)

        public FishType(string name, string desc, float points, Color color, float difficulty)
        {
            Name = name;
            Description = desc;
            Points = points;
            TextColor = color;
            Difficulty = difficulty;
        }
    }

    private readonly FishType[] fishDatabase = new FishType[]
    {
        new FishType("Trucha Plateada del Atardecer", "Un pez pacífico que brilla con los últimos rayos de sol.", 15f, new Color(0.9f, 0.9f, 1f), 0.8f),
        new FishType("Salmón del Alba Serena", "Nada con mucha fuerza contra la corriente del océano.", 20f, new Color(1f, 0.8f, 0.8f), 1.5f),
        new FishType("Pez de Colores Fantasía", "Pequeño y vivaz, parece sacado de un sueño.", 10f, new Color(1f, 0.9f, 0.6f), 0.7f),
        new FishType("Cangrejo Esmeralda Brillante", "Camina muy despacio por la arena del fondo marino.", 8f, new Color(0.7f, 1f, 0.7f), 0.4f),
        new FishType("Estrella de Mar Mística", "Descansa inmóvil en las profundidades doradas del océano.", 12f, new Color(0.9f, 0.7f, 1f), 0.3f),
        new FishType("Caracola del Eco Eterno", "Si te la acercas al oído, puedes oír el canto de las ballenas.", 5f, new Color(1f, 1f, 0.8f), 0.2f),
        new FishType("Bota de Cuero Vieja", "Alguien la perdió hace mucho tiempo. Tiene algas pegadas.", 3f, new Color(0.7f, 0.6f, 0.5f), 0.1f)
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

        // Initialize colors for custom UI styling
        greenTexture = CreateColorTexture(new Color(0f, 0.8f, 0.1f, 0.6f));
        redTexture = CreateColorTexture(new Color(0.9f, 0.1f, 0.1f, 0.8f));
        yellowTexture = CreateColorTexture(new Color(0.9f, 0.8f, 0f, 0.8f));
        darkBlueTexture = CreateColorTexture(new Color(0f, 0.05f, 0.1f, 0.85f));
        grayTexture = CreateColorTexture(new Color(0.15f, 0.15f, 0.15f, 0.9f));
    }

    private Texture2D CreateColorTexture(Color col)
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, col);
        tex.Apply();
        return tex;
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

        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            HandleFishingAction();
        }

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
                float vibrate = Mathf.Sin(Time.time * 60f) * 0.06f;
                UpdateBobberBuoyancy(-0.9f + vibrate);
                
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    LoseFish();
                }
                break;

            case FishingState.MiniGame:
                UpdateBobberBuoyancy(-1.2f); 
                UpdateStardewPhysics();
                break;

            case FishingState.Idle:
                hudMessage = "Presiona [F] para lanzar el anzuelo en una Zona de Pesca";
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
                ShowStatus("Recogiste el sedal antes de tiempo... El pez huyó.", 3f);
                ResetToIdle();
                break;

            case FishingState.BiteActive:
                StartMiniGameStruggle();
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
            activeBobber.transform.localScale = new Vector3(0.28f, 0.28f, 0.28f);
            activeBobber.GetComponent<Renderer>().material.color = Color.red;

            // Small white antenna
            GameObject antenna = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Destroy(antenna.GetComponent<Collider>());
            antenna.transform.parent = activeBobber.transform;
            antenna.transform.localPosition = new Vector3(0, 1.2f, 0);
            antenna.transform.localScale = new Vector3(0.2f, 0.8f, 0.2f);
            antenna.GetComponent<Renderer>().material.color = Color.white;
        }

        // Remove active physics collider
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
            targetPos.y = 0.5f;
        }

        // Check if target is inside a valid FishingZone
        FishingZone[] zones = FindObjectsByType<FishingZone>(FindObjectsSortMode.None);
        FishingZone hitZone = null;
        foreach (var zone in zones)
        {
            float dist = Vector3.Distance(new Vector3(targetPos.x, zone.transform.position.y, targetPos.z), zone.transform.position);
            if (dist <= zone.radius)
            {
                hitZone = zone;
                break;
            }
        }

        // Animate flying parabola
        float duration = 1.3f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
            currentPos.y += Mathf.Sin(t * Mathf.PI) * 5f; // Parabolic arc
            
            if (activeBobber != null)
            {
                activeBobber.transform.position = currentPos;
            }
            yield return null;
        }

        if (hitZone != null)
        {
            // Successfully landed in a fishing zone
            currentZone = hitZone;
            currentState = FishingState.WaitingForBite;
            stateTimer = Random.Range(minWaitTime, maxWaitTime);
            hudMessage = $"Anzuelo en: {currentZone.zoneName.ToUpper()}";
            ShowStatus($"¡Excelente lanzamiento! Los peces están activos aquí...", 3f);
        }
        else
        {
            // Missed the zone! Carlos rewinds the rod
            ShowStatus("No hay peces por aquí...\nDebes lanzar el anzuelo dentro de los círculos activos de agua (Zonas de Pesca).", 4f);
            if (activeBobber != null)
            {
                Destroy(activeBobber);
                activeBobber = null;
            }
            ResetToIdle();
        }
    }

    private void UpdateBobberBuoyancy(float verticalOffset)
    {
        if (activeBobber == null) return;

        Vector3 pos = activeBobber.transform.position;
        float waveHeight = 0.5f;
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
        hudMessage = "¡HA PICADO! ¡Presiona [F] para luchar!";
    }

    private void StartMiniGameStruggle()
    {
        currentState = FishingState.MiniGame;
        
        // Choose random fish from database
        int fishIdx = Random.Range(0, fishDatabase.Length);
        currentActiveFish = fishDatabase[fishIdx];
        fishDifficultyFactor = currentActiveFish.Difficulty;

        // Initialize Stardew Valley variables
        barY = 0.1f;
        barVelocity = 0f;
        fishY = 0.3f;
        fishTargetY = 0.4f;
        fishTimer = 0f;
        catchProgress = 0.35f;

        hudMessage = "¡MANTÉN AL PEZ DENTRO DE LA BARRA VERDE!";
    }

    private void UpdateStardewPhysics()
    {
        // 1. Catcher Bar Physics (Gravity and Thrust Input)
        float gravity = 3.5f;   // pulling down
        float thrust = 4.8f;    // pushing up when [F] is held
        float maxSpeed = 2.2f;

        bool isHoldingF = Keyboard.current != null && Keyboard.current.fKey.isPressed;

        if (isHoldingF)
        {
            barVelocity += (thrust - gravity) * Time.deltaTime;
        }
        else
        {
            barVelocity -= gravity * Time.deltaTime;
        }

        // Clamp speed
        barVelocity = Mathf.Clamp(barVelocity, -maxSpeed, maxSpeed);
        barY += barVelocity * Time.deltaTime;

        // Bounce/Clamp at boundaries
        if (barY <= barHeight / 2f)
        {
            barY = barHeight / 2f;
            barVelocity = -barVelocity * 0.15f; // Soft bounce
        }
        else if (barY >= 1f - barHeight / 2f)
        {
            barY = 1f - barHeight / 2f;
            barVelocity = -barVelocity * 0.15f; // Soft bounce
        }

        // 2. Fish AI Movement Simulation (Stardew Style)
        fishTimer -= Time.deltaTime;
        if (fishTimer <= 0f)
        {
            // Choose next position randomly weighted by difficulty
            fishTargetY = Random.Range(0.05f, 0.95f);
            fishTimer = Random.Range(0.4f / fishDifficultyFactor, 1.2f / fishDifficultyFactor);
        }

        // Smoothly approach target Y with some organic noise
        float activeSpeed = 3.0f * fishDifficultyFactor;
        fishY = Mathf.MoveTowards(fishY, fishTargetY, activeSpeed * Time.deltaTime);

        // Add small jitter noise for wild fish
        if (fishDifficultyFactor > 1.0f)
        {
            fishY += Mathf.Sin(Time.time * 25f) * 0.004f;
        }
        fishY = Mathf.Clamp(fishY, 0.02f, 0.98f);

        // 3. Catch Progress Rules
        float halfBar = barHeight / 2f;
        bool isFishInside = (fishY >= barY - halfBar) && (fishY <= barY + halfBar);

        if (isFishInside)
        {
            catchProgress += 0.22f * Time.deltaTime; // speed to win
        }
        else
        {
            catchProgress -= 0.15f * Time.deltaTime; // speed to lose
        }

        catchProgress = Mathf.Clamp01(catchProgress);

        // Win or Lose checks
        if (catchProgress >= 1f)
        {
            ReelInCatch();
        }
        else if (catchProgress <= 0f)
        {
            LoseFish();
        }
    }

    private void ReelInCatch()
    {
        currentState = FishingState.Reeling;
        hudMessage = "";

        int caughtSoFar = 1;
        int required = 12;
        if (GameplayCinematicController.Instance != null)
        {
            caughtSoFar = GameplayCinematicController.Instance.GetFishCaughtCount() + 1;
            required = GameplayCinematicController.Instance.fishRequiredToCatch;
        }

        string successMsg = $"¡Has pescado un {currentActiveFish.Name.ToUpper()}!\n\n\"{currentActiveFish.Description}\"\n\n(+{currentActiveFish.Points} puntos)\n\nProgreso: {caughtSoFar} de {required} peces.";

        // Handle depletion of the fishing zone after 4 fishes are caught in it
        if (currentZone != null)
        {
            bool isDepleted = currentZone.RegisterFishCaught();
            if (isDepleted)
            {
                successMsg += $"\n\n<color=yellow>¡La ({currentZone.zoneName.ToUpper()}) se ha agotado! Busca otra zona activa.</color>";
                Destroy(currentZone.gameObject);
                currentZone = null;
            }
        }

        ShowStatus(successMsg, 5f);

        // Award score
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(currentActiveFish.Points);
        }

        if (GameplayCinematicController.Instance != null)
        {
            GameplayCinematicController.Instance.NotifyFishCaught();
        }

        StartCoroutine(CatchEffectsCoroutine());
    }

    private IEnumerator CatchEffectsCoroutine()
    {
        if (activeBobber != null)
        {
            var ren = activeBobber.GetComponent<Renderer>();
            if (ren != null) ren.material.color = Color.green;
            yield return new WaitForSeconds(0.4f);
            Destroy(activeBobber);
            activeBobber = null;
        }
        ResetToIdle();
    }

    private void LoseFish()
    {
        ShowStatus("¡Se escapó! El pez luchó con demasiada fuerza.", 3f);
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
        currentZone = null;
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

    private void OnGUI()
    {
        // 1. Draw top instruction/HUD message
        if (!string.IsNullOrEmpty(hudMessage))
        {
            GUIStyle hudStyle = new GUIStyle(GUI.skin.label);
            hudStyle.alignment = TextAnchor.MiddleCenter;
            hudStyle.fontSize = 20;
            hudStyle.fontStyle = FontStyle.Bold;
            hudStyle.normal.textColor = Color.yellow;

            Rect hudRect = new Rect(0, Screen.height - 180, Screen.width, 40);
            // Shadow effect
            GUI.Label(new Rect(hudRect.x + 2, hudRect.y + 2, hudRect.width, hudRect.height), hudMessage, new GUIStyle(hudStyle) { normal = { textColor = Color.black } });
            GUI.Label(hudRect, hudMessage, hudStyle);
        }

        // 3. Draw Stardew Valley Fishing Minigame Interface
        if (currentState == FishingState.MiniGame)
        {
            // Container Panel background
            float panelWidth = 140f;
            float panelHeight = 360f;
            float panelX = Screen.width - panelWidth - 40f;
            float panelY = (Screen.height - panelHeight) / 2f;

            Rect panelRect = new Rect(panelX, panelY, panelWidth, panelHeight);
            GUIStyle panelStyle = new GUIStyle(GUI.skin.box);
            panelStyle.normal.background = darkBlueTexture;
            GUI.Box(panelRect, GUIContent.none, panelStyle);

            // Title label
            GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.fontSize = 13;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.normal.textColor = Color.yellow;
            GUI.Label(new Rect(panelRect.x, panelRect.y + 10, panelWidth, 20), "PESCANDO...", titleStyle);

            // Sub-instruction
            GUIStyle keyStyle = new GUIStyle(GUI.skin.label);
            keyStyle.alignment = TextAnchor.MiddleCenter;
            keyStyle.fontSize = 10;
            keyStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(panelRect.x + 5, panelRect.y + 30, panelWidth - 10, 30), "Mantén [F] sube\nSuelta [F] baja", keyStyle);

            // The main vertical tube/slider area
            Rect tubeRect = new Rect(panelRect.x + 25, panelRect.y + 70, 35, 260);
            GUIStyle tubeStyle = new GUIStyle(GUI.skin.box);
            tubeStyle.normal.background = grayTexture;
            GUI.Box(tubeRect, GUIContent.none, tubeStyle);

            // Catcher Bar (Green Box)
            float normCatcherHeight = barHeight * tubeRect.height;
            float normCatcherY = tubeRect.y + (tubeRect.height - (barY * tubeRect.height) - normCatcherHeight / 2f);
            
            // Soft limits clamping for display
            normCatcherY = Mathf.Clamp(normCatcherY, tubeRect.y, tubeRect.y + tubeRect.height - normCatcherHeight);
            
            Rect catcherRect = new Rect(tubeRect.x, normCatcherY, tubeRect.width, normCatcherHeight);
            GUIStyle catcherStyle = new GUIStyle(GUI.skin.box);
            catcherStyle.normal.background = greenTexture;
            GUI.Box(catcherRect, GUIContent.none, catcherStyle);

            // Fish Marker (Orange/Gold square inside the tube)
            float normFishY = tubeRect.y + (tubeRect.height - (fishY * tubeRect.height) - 10f);
            normFishY = Mathf.Clamp(normFishY, tubeRect.y, tubeRect.y + tubeRect.height - 20f);
            
            Rect fishRect = new Rect(tubeRect.x + 6, normFishY, tubeRect.width - 12, 20);
            GUIStyle fishVisualStyle = new GUIStyle(GUI.skin.box);
            fishVisualStyle.normal.background = yellowTexture;
            fishVisualStyle.alignment = TextAnchor.MiddleCenter;
            fishVisualStyle.fontSize = 12;
            GUI.Box(fishRect, "🐟", fishVisualStyle);

            // Progress Bar (Right side of the tube)
            Rect progBackgroundRect = new Rect(tubeRect.xMax + 12, tubeRect.y, 10, tubeRect.height);
            GUI.Box(progBackgroundRect, GUIContent.none, tubeStyle);

            float progFillHeight = catchProgress * tubeRect.height;
            float progFillY = progBackgroundRect.y + (progBackgroundRect.height - progFillHeight);
            Rect progFillRect = new Rect(progBackgroundRect.x, progFillY, progBackgroundRect.width, progFillHeight);

            // Choose color of progress dynamically
            Texture2D progressColorTex = redTexture;
            if (catchProgress > 0.65f) progressColorTex = greenTexture;
            else if (catchProgress > 0.3f) progressColorTex = yellowTexture;

            GUIStyle progressStyle = new GUIStyle(GUI.skin.box);
            progressStyle.normal.background = progressColorTex;
            GUI.Box(progFillRect, GUIContent.none, progressStyle);
        }
    }
}