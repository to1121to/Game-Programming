using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/*
    特殊動作的原型，有需要特殊動作時參考
*/
public class DoorController : MonoBehaviour
{
    public string nextScene;
    public float nextx;
    public int neededEvent = -1;
    bool trigger;
    GameController Game;
    private void Start()
    {
        Game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        trigger = false;
    }
    private void Update()
    {
        if (Input.GetButtonDown("Interact") && trigger)
        {
            if (Game.EventExecuted(neededEvent))
            {
                //do the thing if the event has been triggered
                Game.nextx = nextx;
                Game.ChangeScene(nextScene);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            trigger = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            trigger = false;
        }
    }
}
