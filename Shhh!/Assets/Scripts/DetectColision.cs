using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectColision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ye");
        if (other.gameObject.CompareTag("Objeto"))
        {
            Debug.Log("ye2");
            //GameManager.Instance.LoadNextScene();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
