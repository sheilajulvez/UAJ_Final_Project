using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para SceneManager
using UnityEngine.UI; // Necesario para SceneManager
public class ExitAction : IVoiceAction {
    public void Execute() {
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