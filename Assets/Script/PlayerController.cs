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
    Animator anim;
    Scene currentScene;
    string sceneName;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        //coll = GetComponent<Collision>();
        currentScene = SceneManager.GetActiveScene ();
        sceneName = currentScene.name;
        facingLeft = true;
        onGround = false;
        onDoor = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetButtonDown("Jump")) jump = true;
        if(Input.GetKeyDown(KeyCode.W) && onDoor)
        {
            if(sceneName == "Alpha")
            {
                SceneManager.LoadScene("Beta");
            }
            else if(sceneName == "Beta")
            {
                SceneManager.LoadScene("Alpha");
            }
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
        onDoor = true;
	}
}
