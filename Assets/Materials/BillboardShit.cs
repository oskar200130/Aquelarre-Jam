using UnityEngine;

public class BillboardShit : MonoBehaviour
{
    Transform cam;
    void Start()
    {
        cam = Camera.main.transform;
        //transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cam.position+Vector3.back*20);     //La suma es para que no se giren las palabras
    }
}
