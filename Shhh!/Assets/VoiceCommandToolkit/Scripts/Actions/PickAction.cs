using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickAction : IVoiceAction
{
    public void Execute()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "Retiro" || currentScene == "Cine" || currentScene == "Iglesia" || currentScene == "Mina")
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            GameObject starCoin = GameObject.Find("StarCoin");

            if (player != null && starCoin != null)
            {
                AudioSource[] sources = player.GetComponents<AudioSource>();
                Collider triggerCollider = starCoin.GetComponent<Collider>();

                if (triggerCollider != null && triggerCollider.isTrigger)
                {
                    if (triggerCollider.bounds.Contains(player.transform.position))
                    {
                        foreach (AudioSource source in sources)
                        {
                            if (source.clip != null && source.clip.name == "coin")
                            {
                                source.Play();
                                break;
                            }
                        }

                        GameManager.Instance.SetCoinsCollected(GameManager.Instance.GetCoinsCollected() + 1);

                        if (GameManager.Instance.GetCoinsCollected() < 4)
                        {
                            switch (currentScene)
                            {
                                case "Retiro":
                                    GameManager.Instance.LoadScene("Iglesia");
                                    break;
                                case "Iglesia":
                                    GameManager.Instance.LoadScene("Cine");
                                    break;
                                case "Mina":
                                    GameManager.Instance.LoadScene("Retiro");
                                    break;
                                case "Cine":
                                    GameManager.Instance.LoadScene("Mina");
                                    break;
                                default:
                                    Debug.LogWarning("Escena no contemplada para la transición.");
                                    break;
                            }
                            Debug.Log("nextscene");
                        }
                    }
                    else
                    {
                        foreach (AudioSource source in sources)
                        {
                            if (source.clip != null && source.clip.name == "empty")
                            {
                                source.Play();
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("El collider de StarCoin no está marcado como Trigger.");
                }
            }
            else
            {
                Debug.LogError("No se encontró el jugador o el objeto StarCoin.");
            }
        }
    }
}
