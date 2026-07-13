using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class FloatingText3D : MonoBehaviour
{
    [Header("Text Settings")]
    public string textToShow = "";
    public float typingSpeed = 0.04f;
    public float displayDuration = 4.0f;



    [Header("Ghostly Breeze Effect")]
    public bool dissolveOnCollision = true;
    public float dissolveSpeed = 2f;

    private TextMeshPro tmp;
    private bool isDissolving = false;
    private Vector3 currentVelocity = Vector3.zero;

    private void Awake()
    {
        tmp = GetComponent<TextMeshPro>();
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 5f;
        tmp.color = new Color(1f, 0.95f, 0.7f, 0f);
        tmp.outlineWidth = 0.15f;
        tmp.outlineColor = new Color(0f, 0f, 0f, 0.8f);

        // Setup trigger for collision dissolve
        SphereCollider sc = gameObject.AddComponent<SphereCollider>();
        sc.isTrigger = true;
        sc.radius = 5.5f; 
    }

    private void Start()
    {
        StartCoroutine(AnimateTextRoutine());
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entro algo");
        if (dissolveOnCollision && other.CompareTag("Player"))
        {
            Debug.Log("Entro");
            StartCoroutine(GhostlyBreezeDissolve());
        }
    }

    private IEnumerator GhostlyBreezeDissolve()
    {
        isDissolving = true;
        StopCoroutine(nameof(AnimateTextRoutine));

        float fadeDuration = 0.3f;
        float elapsedDissolve = 0f;
        Vector3 breezeDir = Vector3.back;

        tmp.ForceMeshUpdate();

        Color32 baseColor = tmp.color;

        while (elapsedDissolve < fadeDuration)
        {
            elapsedDissolve += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedDissolve / fadeDuration);
            transform.position += breezeDir * (Time.deltaTime * dissolveSpeed);
            int totalChars = tmp.textInfo.characterCount;

            for (int i = 0; i < totalChars; i++)
            {
                var charInfo = tmp.textInfo.characterInfo[i];

                if (!charInfo.isVisible) continue;

                float charDelay = (float)(totalChars - i) / totalChars * 0.3f;
                float charAlpha = Mathf.Lerp(1f, 0f, Mathf.Clamp01((t - charDelay) / (1f - charDelay)));

                Color32 newColor = new Color32(baseColor.r, baseColor.g, baseColor.b, (byte)(charAlpha * 255));

                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;
                Color32[] vertexColors = tmp.textInfo.meshInfo[materialIndex].colors32;

                vertexColors[vertexIndex + 0] = newColor;
                vertexColors[vertexIndex + 1] = newColor;
                vertexColors[vertexIndex + 2] = newColor;
                vertexColors[vertexIndex + 3] = newColor;
            }

            tmp.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            yield return null;
        }

        Destroy(gameObject);
    }

    private IEnumerator AnimateTextRoutine()
    {
        tmp.text = "";
        string currentText = "";
        foreach (char ch in textToShow)
        {
            currentText += ch;
            tmp.text = currentText;
            tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, Mathf.Min(tmp.color.a + 0.2f, 1f));
            yield return new WaitForSeconds(typingSpeed);
        }

        tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, 1f);
        yield return new WaitForSeconds(displayDuration);

        if (!isDissolving)
        {
            StartCoroutine(GhostlyBreezeDissolve());
        }
    }
}