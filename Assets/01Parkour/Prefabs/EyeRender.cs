using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeRender : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var pupileGameObject = GameObject.Find("Pupile");
        if (pupileGameObject == null) return;
        Renderer renderer = pupileGameObject.GetComponent<Renderer>();
        if (renderer == null) return;
        renderer.material.color = Color.red;
    }
}
