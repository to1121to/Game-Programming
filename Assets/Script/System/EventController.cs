using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    用來放"拿道具"、"對話"、"查看"、"開門"等等動作 
*/
public class EventController : MonoBehaviour
{
    public int EventID;
    bool Interactable;
    bool Trigger;
    public bool delect;
    public int neededItem = -1;
    GameController Game;
    private void Start()
    {
        Game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        Trigger = false;
    }
    private void Update()
    {
        Interactable = Game.Interactable(EventID);
        if (Input.GetButtonDown("Interact") && Trigger)
        {
            if (Interactable)
            {
                if (Game.HadItem(neededItem))
                {
                    //Debug.Log(EventID);
                    int nextEvent = Game.EventTrigger(EventID, true);
                    if (nextEvent != -1)
                    {
                        EventID = nextEvent;
                    }
                    if (delect) Destroy(gameObject);

                }
                else
                {
                    Game.EventTrigger(EventID, false);
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Trigger = true;
            /*if (!collision.gameObject.GetComponent<PlayerController>().Event && Interactable)
            {
                Trigger = true;
                collision.gameObject.GetComponent<PlayerController>().Event = true;
            }*/
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Trigger = true;
            /*if (!collision.gameObject.GetComponent<PlayerController>().Event && Interactable)
            {
                Trigger = true;
                collision.gameObject.GetComponent<PlayerController>().Event = true;
            }*/
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Trigger = false;
            /*if (collision.gameObject.GetComponent<PlayerController>().Event && Interactable)
            {
                collision.gameObject.GetComponent<PlayerController>().Event = false;
            }*/
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Trigger = false;
            /*if (collision.gameObject.GetComponent<PlayerController>().Event && Interactable)
            {
                collision.gameObject.GetComponent<PlayerController>().Event = false;
            }*/
        }
    }
}