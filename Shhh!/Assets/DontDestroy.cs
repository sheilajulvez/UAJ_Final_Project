using UnityEngine;

public class PersistentPlayer : MonoBehaviour
{
    private static PersistentPlayer instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // No destruir al cargar otra escena
        }
        else
        {
            Destroy(gameObject); // Evita duplicados si vuelves a esta escena
        }
    }
}
