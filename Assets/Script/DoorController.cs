using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour
{
    void onCollisionEnter2D(Collision2D col)
	{
        Debug.Log("ASDA");
        //if (Input.GetKeyDown(KeyCode.W))
        //{
            SceneManager.LoadScene(1, LoadSceneMode.Additive);
        //}
	}
}
