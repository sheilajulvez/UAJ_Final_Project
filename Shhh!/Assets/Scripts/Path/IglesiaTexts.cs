using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IglesiaTexts : MonoBehaviour
{
    public static IglesiaTexts Instance;

    private void Start()
    {
        Instance = this;
        StartTyping();
    }

    private string[] m_Texts =
    {
        "Bienvenidos a la iglesia.\nNecesitais recoger todas las monedas\nPuedes decir 'go next' para ir a la siguiente ubicacion.",
        "Si encuentras la moneda di el objeta que tiene a su lado para recogerla\nPuedes decir 'go next' para ir a la siguiente ubicacion o 'go back' para ir a la anterior.",
        "Si encuentras la moneda di el objeta que tiene a su lado para recogerla\nPuedes decir 'go next' para ir a la siguiente ubicacion o 'go back' para ir a la anterior.",
        "Si encuentras la moneda di el objeta que tiene a su lado para recogerla\nPuedes decir 'go next' para ir a la siguiente ubicacion o 'go back' para ir a la anterior.",
        "Si encuentras la moneda di el objeta que tiene a su lado para recogerla\nPuedes decir 'go back' para ir a la anterior."
    };

    public TMP_Text uiText;
    public float typingSpeed = 0.05f;
    private int i = 0;

    public void StartTyping()
    {
        StopAllCoroutines();
        StartCoroutine(TypeText(m_Texts[i]));

        if (i < m_Texts.Length - 1) i++;
    }

    public void StartTypingR()
    {
        if (i > 0) i--;
        StopAllCoroutines();
        StartCoroutine(TypeText(m_Texts[i]));
    }

    private IEnumerator TypeText(string message)
    {
        uiText.text = "";
        foreach (char letter in message)
        {
            uiText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
