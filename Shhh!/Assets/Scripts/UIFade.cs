using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIFade : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    public float fadeDuration = 1f;

    public Button myButtonClassic;
    public Button myButtonVoice;
    public Button myButtonQuit;


    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        
        if (gameObject.name == "Fade") StartCoroutine(FadeIn(10f));

        if (myButtonClassic != null) myButtonClassic.onClick.AddListener(() => GameManager.Instance.SetVoiceRecogniser(false));
        if (myButtonVoice != null) myButtonVoice.onClick.AddListener(() => GameManager.Instance.SetVoiceRecogniser(true));
        if (myButtonQuit != null) myButtonQuit.onClick.AddListener(() => GameManager.Instance.QuitGame());
    }

    public IEnumerator FadeIn(float timeWaiting, System.Action onComplete = null)
    {
        yield return new WaitForSeconds(timeWaiting);
        
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;

        onComplete?.Invoke();
    }

    public IEnumerator FadeOut(float timeWaiting, System.Action onComplete = null)
    {

        yield return new WaitForSeconds(timeWaiting);

        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1 - (elapsed / fadeDuration));
            yield return null;
        }

        canvasGroup.alpha = 0f;

        onComplete?.Invoke();
    }

    public void LlamarCorrutinaFadeOut(float timeWaiting)
    {
        StartCoroutine(FadeOut(timeWaiting, () => { gameObject.SetActive(false); }));
    }

    public void LlamarCorrutinaFadeIn(float timeWaiting)
    {
        gameObject.SetActive(true);
        StartCoroutine(FadeIn(timeWaiting));
    }
}
