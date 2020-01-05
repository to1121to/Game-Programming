using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCoverEffect : MonoBehaviour
{
    GameController Game;
    public bool unlock;
    public int lockEvent;
    
    void Start()
    {
        Game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        gameObject.SetActive(!unlock);
    }

    void Update()
    {
        if(!unlock) {
            unlock = Game.EventExecuted(lockEvent);
            gameObject.SetActive(!unlock);
        }
    }
}
