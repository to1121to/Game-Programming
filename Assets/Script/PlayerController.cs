using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float maxSpeed = 10.0f;
    public float jumpForce = 800.0f;
    
    private float movingSpeed;
    private bool jump;
    //private Collision coll;

    bool facingLeft;
    bool onGround;
    bool onDoor;
    bool onItem;

    GameObject CurrentItem;
    public int[] GotItem;
    Animator anim;

    Scene currentScene;

    string sceneName;
    public float nextx;

    bool SceneChangeFlag;
    GameController Game;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        //coll = GetComponent<Collision>();
        currentScene = SceneManager.GetActiveScene ();
        facingLeft = true;
        onGround = false;
        onDoor = false;
        onItem = false;
        GotItem = new int[8];
        for(int i = 0; i < 8; i++)
        {
            GotItem[i] = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetButtonDown("Jump")) jump = true;
        if(Input.GetKeyDown(KeyCode.W) && onDoor)
        {
            Game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
            Game.ChangeScene(sceneName);
            DontDestroyOnLoad(gameObject);
        }
        else if (Input.GetKeyDown(KeyCode.W) && onItem)
        {
            GotItem[CurrentItem.gameObject.GetComponent<ItemController>().ItemID]++;
            Destroy(CurrentItem);
        }
        
    }

    void FixedUpdate()
    {
        movingSpeed = Input.GetAxis("Horizontal");
        Move(movingSpeed, jump);
        jump = false;
        onDoor = false;
    }

    void Move(float MovingSpeed, bool jump)
    {
        if(/*onGround &&*/ jump)
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0.0f, jumpForce));
        }
        else
        {
            anim.SetFloat("Speed", Mathf.Abs(movingSpeed));

            GetComponent<Rigidbody2D>().velocity = new Vector2(movingSpeed*maxSpeed, GetComponent<Rigidbody2D>().velocity.y);

            if (movingSpeed > 0 && facingLeft || movingSpeed < 0 && !facingLeft) Flip ();
        }
    }

    void Flip()
    {
        facingLeft = !facingLeft;

		Vector3 characterScale = transform.localScale;
		characterScale.x *= -1;
		transform.localScale = characterScale;
    }

    private void OnTriggerStay2D(Collider2D col)
	{
        if(col.tag == "Door")
        {
            onDoor = true;
            sceneName = col.GetComponent<DoorController>().nextScene;
            nextx = col.GetComponent<DoorController>().nextx;
        }
        else if(col.tag == "Item")
        {
            CurrentItem = col.gameObject;
            onItem = true;
        }
	}
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Item")
        {
            onItem = false;
        }
    }
}
