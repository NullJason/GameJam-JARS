using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SpeechHandler : MonoBehaviour
{
    public static SpeechHandler Instance { get; private set; } // Singleton instance
    public Transform DialogueBar; // The rectangular Speech bubble.
    public Transform BarContainer;
    public Transform SpeakerNameTextContainer; // Contains TMP_Text component.
    public Transform SpeechTextContainer; // Contains TMP_Text component.
    public float TextDisplayDelay = 0.1f; // Delay for text display.
    public bool IsPlayingAnim = false; // Detect if animation/flavor is active.

    private Queue<Dialogue> dialogueQueue = new Queue<Dialogue>(); // Queue for dialogues.
    private Text speakerNameText;
    private Text speechText;
    private CanvasGroup dialogueBarCanvasGroup;
    private bool isAutoPlaying = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
            return;
        }
        Instance = this;
        speakerNameText = SpeakerNameTextContainer.GetComponent<Text>();
        speechText = SpeechTextContainer.GetComponent<Text>();
        dialogueBarCanvasGroup = DialogueBar.GetComponent<CanvasGroup>();

        if (dialogueBarCanvasGroup == null)
        {
            dialogueBarCanvasGroup = DialogueBar.gameObject.AddComponent<CanvasGroup>();
        }

        DialogueBar.gameObject.SetActive(false);
    }

    public class Dialogue
    {
        public float MinUiAlpha = 0.5f;
        public float MinTextAlpha = 0.65f;
        private string text;
        private float textSize;
        private string speaker;
        private Color barColor;
        private Color textColor;
        private int flavor;
        private float autoPlay;
        private float animDelay;
        public Dialogue(string speaker, string text, float? textSize, Color? barColor, Color? textColor, int? flavor, float? autoPlay, float? animDelay)
        {
            this.speaker = speaker;
            this.text = text;
            this.textSize = textSize ?? 20;
            this.barColor = new Color(0, 0, 0, 85f/255f); // Transparent by default.
            this.textColor = Color.white; // white by default.
            this.flavor = flavor ?? 0;
            this.autoPlay = autoPlay ?? 0;
            this.animDelay = animDelay ?? 0; if(flavor == 0) this.animDelay = 0; else if(flavor == 1 && animDelay == 0) this.animDelay = 0.1f;
            if(this.barColor.a < MinUiAlpha) this.barColor = new Color(this.barColor.r, this.barColor.g, this.barColor.b, MinUiAlpha);
            if(this.textColor.a < MinTextAlpha) this.textColor = new Color(this.textColor.r, this.textColor.g, this.textColor.b, MinTextAlpha);
        }

        public string GetText() => text;
        public float GetTextSize() => textSize;
        public string GetSpeaker() => speaker;
        public Color GetBarColor() => barColor;
        public Color GetTextColor() => textColor;
        public int GetFlavor() => flavor;
        public float GetAutoPlay() => autoPlay;
        public float GetTextDisplayDelay() => animDelay;
    }

    public void AcceptNew(string speaker, string text, float? textSize = null, Color? barColor = null, Color? textColor = null, int? flavor = null, float? autoPlay = null, float? animDelay = null)
    {
        Debug.Log("Accepted new dialogue."+text + barColor.ToString() + textColor.ToString());
        Dialogue newDialogue = new Dialogue(speaker, text, textSize, barColor, textColor, flavor, autoPlay, animDelay);
        dialogueQueue.Enqueue(newDialogue);
    }
    public bool Stopped(){return dialogueQueue.Count == 0 && !IsPlayingAnim && !isAutoPlaying;}
    public void PlayNext()
    {
        if(isAutoPlaying) return;
        if (IsPlayingAnim) // If animation is in progress.
        {
            IsPlayingAnim = false;
            TextDisplayDelay = 0; // Instantly finish the animation.
            return;
        }

        if (dialogueQueue.Count == 0) // If no dialogues remain.
        {
            if (DialogueBar.gameObject.activeSelf) StartCoroutine(FadeOutBar());
            return;
        }

        Dialogue currentDialogue = dialogueQueue.Dequeue();
        TextDisplayDelay = currentDialogue.GetTextDisplayDelay();
        StartCoroutine(FadeInBar(currentDialogue));
        Debug.Log("Playing dialogue: "+currentDialogue.GetText());
    }

    private IEnumerator FadeInBar(Dialogue dialogue)
    {
        if(!DialogueBar.gameObject.activeSelf) {
            DialogueBar.gameObject.SetActive(true);

            float fadeDuration = 0.1f;
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                dialogueBarCanvasGroup.alpha = t / fadeDuration;
                yield return null;
            }

            dialogueBarCanvasGroup.alpha = 1;
        }

        // Set UI properties.
        BarContainer.GetComponent<UnityEngine.UI.Image>().color = dialogue.GetBarColor();
        speakerNameText.text = dialogue.GetSpeaker();
        speakerNameText.color = dialogue.GetTextColor();
        speechText.color = dialogue.GetTextColor();
        //speechText.font = dialogue.GetTextSize();

        if (dialogue.GetFlavor() == 0) // Instantly display text.
        {
            speechText.text = dialogue.GetText();
        }
        else if (dialogue.GetFlavor() == 1) // Delayed text animation.
        {
            StartDelayedTextInput(dialogue.GetText());
        }
        else if (dialogue.GetFlavor() == 2) // Instant text with shaking animation.
        {
            speechText.text = dialogue.GetText();
            StartCoroutine(PlayShakingAnimation());
        }
        float autoPlayDelay = dialogue.GetAutoPlay();
        if (autoPlayDelay > 0)
        {
            isAutoPlaying = true;
            if (dialogue.GetFlavor() != 2) yield return new WaitForSeconds(autoPlayDelay + (TextDisplayDelay * dialogue.GetText().Length));
            else{yield return new WaitForSeconds(autoPlayDelay + TextDisplayDelay );}
            isAutoPlaying = false; IsPlayingAnim = false; PlayNext();
        }
    }
    private IEnumerator PlayShakingAnimation()
    {
        // Define shake duration and intensity.
        IsPlayingAnim = true;
        float shakeDuration = TextDisplayDelay;
        float shakeIntensity = 5f;

        Vector3 originalTextPosition = speechText.rectTransform.localPosition;
        Vector3 originalDialogueBarPosition = DialogueBar.localPosition;

        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            float offsetX = Random.Range(-shakeIntensity, shakeIntensity);
            float offsetY = Random.Range(-shakeIntensity, shakeIntensity);

            // Apply shake to text and dialogue bar.
            speechText.rectTransform.localPosition = originalTextPosition + new Vector3(offsetX, offsetY, 0);
            DialogueBar.localPosition = originalDialogueBarPosition + new Vector3(offsetX * 0.5f, offsetY * 0.5f, 0); // Less intense screen shake.

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset positions.
        speechText.rectTransform.localPosition = originalTextPosition;
        DialogueBar.localPosition = originalDialogueBarPosition;
        IsPlayingAnim = false;
    }
    private IEnumerator FadeOutBar()
    {
        float fadeDuration = 0.1f;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            dialogueBarCanvasGroup.alpha = 1 - (t / fadeDuration);
            yield return null;
        }

        dialogueBarCanvasGroup.alpha = 0;
        DialogueBar.gameObject.SetActive(false);
    }

    public void StartDelayedTextInput(string message)
    {
        StartCoroutine(DoDelayedTextInput(message));
    }

    private IEnumerator DoDelayedTextInput(string message)
    {
        IsPlayingAnim = true;
        speechText.text = "";

        foreach (char c in message)
        {
            speechText.text += c;
            yield return new WaitForSeconds(TextDisplayDelay);
        }
        IsPlayingAnim = false;
    }
    private IEnumerator ApplyScreenShakeEffect()
    {
        // TODO: must anchor player.
        Vector3 originalPosition = Camera.main.transform.localPosition;
        float duration = 0.5f;
        float magnitude = 0.1f;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            Camera.main.transform.localPosition = new Vector3(x, y, originalPosition.z);
            yield return null;
        }

        Camera.main.transform.localPosition = originalPosition;
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Return)){PlayNext();}
    }
}