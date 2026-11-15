
using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class IntroTransition : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup introBackground;
    [SerializeField] private CanvasGroup introTextGroup;
    [SerializeField] private TMP_Text introText;

    [Header("Transition Timing")]
    [SerializeField] private float fadeDuration = 0.8f;
    [SerializeField] private float readingDuration = 10f;

    [Header("Introduction Message")]
    [SerializeField, TextArea(5, 12)]
    private string introMessage =
        "CUBA - PRESENT DAY\n\n" +
        "Since 1962, the United States has maintained " +
        "broad trade restrictions on Cuba. These " +
        "restrictions, alongside domestic economic " +
        "challenges and fuel shortages, have placed " +
        "pressure on everyday life.\n\n" +
        "You are responsible for a family living " +
        "in Havana. Manage food, fuel, medicine, " +
        "money, and health while navigating " +
        "shortages and difficult decisions.\n\n" +
        "YOUR OBJECTIVE: SURVIVE SEVEN DAYS.";

    private bool isPlaying;

    void Awake()
    {
        if (introBackground != null)
        {
            introBackground.alpha = 0f;
            introBackground.blocksRaycasts = false;
            introBackground.gameObject.SetActive(false);
        }
    }

    public void PlayIntro(Action startGameplay)
    {
        if (isPlaying)
            return;

        if (introBackground == null ||
            introTextGroup == null ||
            introText == null)
        {
            Debug.LogError(
                "IntroTransition has missing UI references!");

            startGameplay?.Invoke();
            return;
        }

        StartCoroutine(IntroSequence(startGameplay));
    }

    IEnumerator IntroSequence(Action startGameplay)
    {
        isPlaying = true;

        // Activate the introduction overlay.
        introBackground.gameObject.SetActive(true);
        introBackground.transform.SetAsLastSibling();

        introBackground.alpha = 0f;
        introTextGroup.alpha = 0f;

        introBackground.blocksRaycasts = true;

        introText.text = introMessage;

        // Step 1: Fade menu to black.
        yield return FadeGroup(
            introBackground, 0f, 1f);

        // Step 2: Fade introduction text in.
        yield return FadeGroup(
            introTextGroup, 0f, 1f);

        // Step 3: Keep the text visible.
        yield return new WaitForSecondsRealtime(
            readingDuration);

        // Step 4: Fade introduction text out.
        yield return FadeGroup(
            introTextGroup, 1f, 0f);

        // Start gameplay while the screen is black.
        startGameplay?.Invoke();

        // Step 5: Reveal the gameplay screen.
        yield return FadeGroup(
            introBackground, 1f, 0f);

        // Clean up the overlay.
        introBackground.blocksRaycasts = false;
        introBackground.gameObject.SetActive(false);

        isPlaying = false;
    }

    IEnumerator FadeGroup(
        CanvasGroup group,
        float startAlpha,
        float endAlpha)
    {
        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, fadeDuration);

        group.alpha = startAlpha;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(
                elapsed / duration);

            group.alpha = Mathf.Lerp(
                startAlpha,
                endAlpha,
                progress);

            yield return null;
        }

        group.alpha = endAlpha;
    }
}