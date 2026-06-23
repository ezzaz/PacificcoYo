using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TableroFunctionality : MonoBehaviour
{
    public float scaleSpeed;
    public float scaleLimit;

    Image image;
    public Color[] colors;



    public void Hover(Transform TR)
    {
        StopCoroutine(ScaleDown(TR));
        image = TR.GetComponent<Image>();
        image.color = colors[1];
        StartCoroutine(ScaleUp(TR));
    }
    public void HoverExit(Transform TR)
    {
        StopCoroutine(ScaleUp(TR));
        image = TR.GetComponent<Image>();
        image.color = colors[0];
        StartCoroutine(ScaleDown(TR));
    }
    public void Click(string sceneName)
    {
        image.color = colors[0];
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator ScaleUp(Transform TR)
    {
        do
        {
            TR.localScale = new Vector3(TR.localScale.x + scaleSpeed, TR.localScale.y + scaleSpeed, TR.localScale.z + scaleSpeed);
            yield return new WaitForEndOfFrame();
        }while (TR.localScale.x <= scaleLimit);
        TR.localScale = Vector3.one * scaleLimit;
    }
    IEnumerator ScaleDown(Transform TR)
    {
        do
        {
            TR.localScale = new Vector3(TR.localScale.x - scaleSpeed, TR.localScale.y - scaleSpeed, TR.localScale.z - scaleSpeed);
            yield return new WaitForEndOfFrame();
        } while (TR.localScale.x >= 1);
        TR.localScale = Vector3.one;
    }
}