using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    private Rigidbody2D rb2D;
    private Vector2 dir;
    void Start () {
        rb2D = GetComponent<Rigidbody2D> ();
        dir = new Vector2 (-1, 0);
        InvokeRepeating ("ChangeDireciton", 2, Random.Range(3, 5));
    }
    // Update is called once per frame
    private void ChangeDireciton () {
        dir *= -1;
        gameObject.transform.localScale = new Vector3 (gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
        rb2D.velocity = dir * Random.Range(3.0f, 5.0f);
    }
}