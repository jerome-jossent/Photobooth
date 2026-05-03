using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Photoing : MonoBehaviour
{

    public RawImage fadeImage;

    public float t_offset = 1.0f;
    public float t_masque = 1.0f;
    float t;

    public void _NewPhoto(float temps_ms)
    {
        Debug.Log("_NewPhoto " + temps_ms);
        t = temps_ms / 1000 - t_offset;
        StartCoroutine(FadeRoutine());
    }

    IEnumerator FadeRoutine()
    {
        fadeImage.gameObject.SetActive(true);
        // Attendre 3 secondes
        yield return new WaitForSeconds(t);

        float duration = 1f;
        float time = 0f;

        Color color = fadeImage.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = time / duration;

            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        // Assurer alpha = 1 à la fin
        fadeImage.color = new Color(color.r, color.g, color.b, 1f);

        // Attendre 1 secondes
        yield return new WaitForSeconds(t_masque);
        fadeImage.color = new Color(color.r, color.g, color.b, 0f);

        fadeImage.gameObject.SetActive(false);

        //Changement de theme
        yield return new WaitForSeconds(2000); // temps visu disponible 2 sec, 10 sec
        SpriteColorLoader._instance.LoadRandomCombination();
    }
}
