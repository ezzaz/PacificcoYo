using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Unity.Burst.Intrinsics.X86.Avx;

/// <summary>
/// Controla la secuencia de introducción del Minijuego 1:
/// 1) Pantalla en negro. 2) Fundido de negro para revelar el paisaje.
/// 3) Aparece el título del juego. 4) El título se desvanece "fantasmalmente".
/// 5) Se le devuelve el control al jugador.
/// Todo es configurable desde el Inspector.
/// </summary>
public class IntroSequenceController : MonoBehaviour
{
    [Header("Referencias UI (asignadas en el prefab)")]
    [Tooltip("Imagen negra a pantalla completa que hace el fundido.")]
    public Image fadeImage;
    [Tooltip("Texto del título del juego.")]
    public TextMeshProUGUI titleText;
    private TextMeshPro tmp;
    public float dissolveSpeed = 2f;
    [Header("Contenido")]
    [Tooltip("Texto del título que aparece al iniciar.")]
    public string gameTitle = "OCÉANO";

    [Header("Tiempos (segundos) - editables")]
    [Tooltip("Cuánto se mantiene la pantalla en negro antes de revelar.")]
    public float initialBlackHold = 0.8f;
    [Tooltip("Duración del fundido de negro a transparente (revelar paisaje).")]
    public float fadeInDuration = 2.5f;
    [Tooltip("Pausa para contemplar el paisaje antes de mostrar el título.")]
    public float landscapeAdmireTime = 1.2f;
    [Tooltip("Duración de la aparición del título.")]
    public float titleFadeIn = 1.5f;
    [Tooltip("Cuánto tiempo permanece visible el título.")]
    public float titleHold = 2.0f;
    [Tooltip("Duración de la desaparición fantasmal del título.")]
    public float titleGhostFadeOut = 2.2f;

    [Header("Control del jugador")]
    [Tooltip("Se buscan automáticamente en el GameObject 'Player' si se deja vacío.")]
    public MonoBehaviour[] scriptsToDisableDuringIntro;

    private void Awake()
    {
        tmp = GetComponent<TextMeshPro>();
    }
    private void Start()
    {
        // Preparar título
        if (titleText != null)
        {
            titleText.text = gameTitle;
            SetTextAlpha(titleText, 0f);
        }

        // Empezar en negro
        if (fadeImage != null)
        {
            SetImageAlpha(fadeImage, 1f);
            fadeImage.gameObject.SetActive(true);
        }

        // Si no se asignaron scripts, buscar los controles del jugador automáticamente
        if (scriptsToDisableDuringIntro == null || scriptsToDisableDuringIntro.Length == 0)
        {
            AutoFindPlayerControls();
        }

        SetControlsEnabled(false);
        StartCoroutine(PlayIntro());
    }

    private void AutoFindPlayerControls()
    {
        GameObject player = GameObject.Find("Player");
        if (player == null) return;

        var found = new System.Collections.Generic.List<MonoBehaviour>();
        var move = player.GetComponent<PlayerMovement>();
        var look = player.GetComponent<PlayerLook>();
        var pickup = player.GetComponent<PlayerPickUpDrop>();
        if (move != null) found.Add(move);
        if (look != null) found.Add(look);
        if (pickup != null) found.Add(pickup);
        scriptsToDisableDuringIntro = found.ToArray();
    }

    private IEnumerator PlayIntro()
    {
        // 0) Bloquear cursor durante la intro
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 1) Mantener en negro
        yield return new WaitForSeconds(initialBlackHold);

        // 2) Fundido de negro -> revelar paisaje
        yield return StartCoroutine(FadeImage(1f, 0f, fadeInDuration));
        if (fadeImage != null) fadeImage.gameObject.SetActive(false);

        // 3) Contemplar el paisaje
        yield return new WaitForSeconds(landscapeAdmireTime);

        // 4) Aparece el título
        if (titleText != null)
        {
            yield return StartCoroutine(FadeText(titleText, 0f, 1f, titleFadeIn));
            yield return new WaitForSeconds(titleHold);
            // 5) Desaparición fantasmal (sube, se agranda y se desvanece)
            yield return StartCoroutine(GhostlyFadeTitle());
        }

        // 6) Devolver el control al jugador
        SetControlsEnabled(true);
    }

    private IEnumerator GhostlyFadeTitle()
    {
        float t = 0f;
        float titleGhostFadeOut = 1f;

        Vector3 breezeDir = Vector3.left;

        titleText.ForceMeshUpdate();

        Color32 baseColor = titleText.color;
        Vector3 initialPos = titleText.rectTransform.anchoredPosition;

        while (t < titleGhostFadeOut)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / titleGhostFadeOut);
            int totalChars = titleText.textInfo.characterCount;

            for (int i = 0; i < totalChars; i++)
            {
                var charInfo = titleText.textInfo.characterInfo[i];

                if (!charInfo.isVisible) continue;

             
                float charDelay = (float)(totalChars - i) / totalChars * 0.5f;
                float charAlpha = Mathf.Lerp(1f, 0f, Mathf.Clamp01((k - charDelay) / (1f - charDelay)));

                Color32 newColor = new Color32(baseColor.r, baseColor.g, baseColor.b, (byte)(charAlpha * 255));

                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;
                Color32[] vertexColors = titleText.textInfo.meshInfo[materialIndex].colors32;

                vertexColors[vertexIndex + 0] = newColor;
                vertexColors[vertexIndex + 1] = newColor;
                vertexColors[vertexIndex + 2] = newColor;
                vertexColors[vertexIndex + 3] = newColor;
            }
            titleText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            titleText.rectTransform.anchoredPosition = initialPos + (Vector3.left * 100 * t);

            yield return null;
        }
        titleText.gameObject.SetActive(false);
    }


    private IEnumerator FadeImage(float from, float to, float duration)
    {
        if (fadeImage == null) yield break;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            SetImageAlpha(fadeImage, Mathf.Lerp(from, to, Mathf.Clamp01(t / duration)));
            yield return null;
        }
        SetImageAlpha(fadeImage, to);
    }

    private IEnumerator FadeText(TextMeshProUGUI text, float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            SetTextAlpha(text, Mathf.Lerp(from, to, Mathf.Clamp01(t / duration)));
            yield return null;
        }
        SetTextAlpha(text, to);
    }

    private void SetControlsEnabled(bool enabled)
    {
        if (scriptsToDisableDuringIntro == null) return;
        foreach (var s in scriptsToDisableDuringIntro)
        {
            if (s != null) s.enabled = enabled;
        }
    }

    private void SetImageAlpha(Image img, float a)
    {
        Color c = img.color;
        c.a = a;
        img.color = c;
    }

    private void SetTextAlpha(TextMeshProUGUI text, float a)
    {
        Color c = text.color;
        c.a = a;
        text.color = c;
    }
}