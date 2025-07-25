using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AudioDetection.Interfaces;
public class BackAction : IVoiceAction {
    public void Execute(params object[] parameters) {
        if (SceneManager.GetActiveScene().name != "Selection" && SceneManager.GetActiveScene().name != "Victory")
        {
            GameObject back = GameObject.Find("Back");
            if (back != null)
            {
                back.GetComponent<Button>().onClick.Invoke();
                //controlSave.SetActive(true);
            }
            else
            {
                GameObject hide = GameObject.Find("Hide");
                if (hide != null)
                {
                    hide.GetComponent<Button>().onClick.Invoke();
                }
            }
        }
    }
}