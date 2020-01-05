using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public int id;
    public void OnButtonClick(){
        SceneManager.LoadScene(id, LoadSceneMode.Single);
    }
}
