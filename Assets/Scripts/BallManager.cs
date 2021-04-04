using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BallManager : MonoBehaviour
{
    [SerializeField]
    float speed;
    float radius;
    Vector2 direction;

    GameManager GameManager;
    void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        direction = Random.insideUnitCircle.normalized;
        radius = transform.localScale.x / 2;
        gameObject.name = "Ball";
    }

    void Update()
    {
        transform.Translate (direction * speed * Time.deltaTime);

        if (transform.position.y < GameManager.bottomLeft.y + radius && direction.y < 0){
            direction.y = -direction.y;
        }
        if (transform.position.y > GameManager.topRight.y - radius && direction.y > 0){
            direction.y = -direction.y;
        }
        if (transform.position.x < GameManager.bottomLeft.x + radius && direction.x < 0){
            direction.x = -direction.x;
        }
        if (transform.position.x > GameManager.topRight.x - radius && direction.x > 0){
            direction.x = -direction.x;
        }

        if (Time.timeScale == 0){
            Destroy(gameObject);
        }
    }


    void OnTriggerEnter2D(Collider2D other) {
        if (other.name == "Player1"){
            GameManager.GameEnd(true);
            Destroy(other.gameObject);
            Destroy(GameObject.Find("Player2"));
        }
        else if (other.name == "Player2"){
            GameManager.GameEnd(false);
            Destroy(other.gameObject);
            Destroy(GameObject.Find("Player1"));
        }
        foreach(GameObject ball in GameObject.FindGameObjectsWithTag("Ball")) {
            Destroy(ball.gameObject);
        }
    }

}
