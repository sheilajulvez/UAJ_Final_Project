using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MinePath : MonoBehaviour
{
    public static MinePath Instance;

    public Transform[] ruta1;
    public Transform[] ruta2;
    public Transform[] ruta3;

    private MoveAgent player;

    private int currentPath = 0;

    private void Start()
    {
        Instance = this;
        player = GameManager.Instance.GetPlayer();
    }


    public void AvanzaRuta()
    {
        Transform[] path;

        switch (currentPath)
        {
            case 0: path = ruta2; break;
            case 1: path = ruta3; break;
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
            case 1: path = ruta1; break;
            case 2: path = ruta2; break;
            default: return;
        }

        player.IniciarRuta(path);
        if (currentPath > 0) currentPath--;
    }
}
