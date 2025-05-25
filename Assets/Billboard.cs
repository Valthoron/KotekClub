using UnityEngine;

public class Billboard : MonoBehaviour
{
    void Start()
    {
        
    }

    void LateUpdate()
    {
        //transform.LookAt(Camera.main.transform.position);
        transform.rotation = Camera.main.transform.rotation;
    }
}
