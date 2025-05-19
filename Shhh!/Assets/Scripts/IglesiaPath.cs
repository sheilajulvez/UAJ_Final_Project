using System.Linq;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.AI;

public class IglesiaPath : MonoBehaviour
{
    public static IglesiaPath Instance;

    private void Start()
    {
        Instance = this;
    }

    public Transform[] ruta1;
    public Transform[] ruta2;
    public Transform[] ruta3;
    public Transform[] ruta4;

    public MoveAgent player;

    private int currentPath = 0;

    public void AvanzaRuta()
    {
        Transform[] path;

        switch (currentPath)
        {
            case 0: path = ruta1; break;
            case 1: path = ruta2; break;
            case 2: path = ruta3; break;
            case 3: path = ruta4; break;
            default: return;
        }

        player.IniciarRuta(path);

        currentPath++;
    }

    public void RetrocedeRuta()
    {

        Transform[] path;

        switch (currentPath)
        {
            case 1: path = ruta1.Reverse().ToArray(); break;
            case 2: path = ruta2.Reverse().ToArray(); break;
            case 3: path = ruta3.Reverse().ToArray(); break;
            case 4: path = ruta4.Reverse().ToArray(); break;
            default: return;
        }

        player.IniciarRuta(path);
        if (currentPath > 0) currentPath--;
    }
}
