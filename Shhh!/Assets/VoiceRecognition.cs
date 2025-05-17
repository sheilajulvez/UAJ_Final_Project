using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceRecognition : MonoBehaviour
{
    KeywordRecognizer keywordRecognizer;

    Dictionary<string, Action> wordToAction;

    void Start()
    {
        wordToAction = new Dictionary<string, Action>
        {
            { "retiro", () => CambioDeEscena("Retiro") },
            { "mina", () => CambioDeEscena("Mina") },
            { "cine", () => CambioDeEscena("Cine") },
            { "iglesia", () => CambioDeEscena("Iglesia") }
        };


        keywordRecognizer = new KeywordRecognizer(wordToAction.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += WordRecognized;
        keywordRecognizer.Start();
    }

    private void WordRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log(args.text);
        wordToAction[args.text].Invoke();
    }

    private void CambioDeEscena(string scene)
    {
        GameManager.Instance.LoadScene(scene);
    }
}
