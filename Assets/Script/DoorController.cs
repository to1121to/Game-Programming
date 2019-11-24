using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour
{
    public string nextScene;
    public float nextx;
    private void Start()
    {
        if (this.gameObject.name == "DoorToAlpha")
        {
            nextScene = "Alpha";
        }
        else if (this.gameObject.name == "DoorToBeta")
        {
            nextScene = "Beta";
        }
    }
}
