using TMPro;
using UnityEngine;
using UnityEngine.UI;
using AudioDetection.Interfaces;

public abstract class BaseVoiceInputEngine : MonoBehaviour, IVoiceInputEngine
{
    [SerializeField]
    protected TextMeshProUGUI hypothesisText;

    [SerializeField]
    protected Image panelImage;

    [SerializeField]
    protected Sprite valid;

    [SerializeField]
    protected Sprite invalid;

    public abstract event System.Action<string, object[]> OnCommandRecognized;
    public abstract void Initialize(string[] commands);

    protected virtual void Awake()
    {
        EnsureFeedbackReferences();
        SetFeedbackState(false);
    }

    protected virtual void Start()
    {
        EnsureFeedbackReferences();
    }

    protected void EnsureFeedbackReferences()
    {
        if (hypothesisText == null)
        {
            hypothesisText = GameObject.Find("MessageText")?.GetComponent<TextMeshProUGUI>();
        }

        if (panelImage == null)
        {
            panelImage = GameObject.Find("UnrecognizedCommandPanel")?.GetComponent<Image>();
        }

        if (valid == null)
        {
            valid = Resources.Load<Sprite>("Sprites/valid");
        }

        if (invalid == null)
        {
            invalid = Resources.Load<Sprite>("Sprites/invalid");
        }
    }

    protected void SetFeedbackState(bool recognized)
    {
        EnsureFeedbackReferences();

        if (panelImage == null)
        {
            return;
        }

        Sprite targetSprite = recognized ? valid : invalid;
        if (targetSprite != null)
        {
            panelImage.sprite = targetSprite;
        }
    }
}
