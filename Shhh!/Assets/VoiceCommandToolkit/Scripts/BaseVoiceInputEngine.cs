using TMPro;
using UnityEngine;
using UnityEngine.UI;
using AudioDetection.Interfaces;

public abstract class BaseVoiceInputEngine : MonoBehaviour, IVoiceInputEngine
{
    protected TextMeshProUGUI hypothesisText;
    protected Image panelImage;
    protected Sprite valid;
    protected Sprite invalid;

    public abstract event System.Action<string, object[]> OnCommandRecognized;
    public abstract void Initialize(string[] commands);

    protected virtual void Start()
    {
        hypothesisText = GameObject.Find("MessageText")?.GetComponent<TextMeshProUGUI>();
        panelImage = GameObject.Find("UnrecognizedCommandPanel")?.GetComponent<Image>();

        // Carga desde la carpeta Resources
        valid = Resources.Load<Sprite>("Sprites/valid");
        invalid = Resources.Load<Sprite>("Sprites/invalid");
    }
}
