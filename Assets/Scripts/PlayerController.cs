using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    float speed;
    float height;
    string input;

    


    void Start()
    {
        height = transform.localScale.y;
    }

    public void Init(bool isLeft, bool isSingle) {
        Vector2 pos = Vector2.zero;

        if (isLeft) {
            pos = new Vector2(GameManager.bottomLeft.x, 0);
            pos += Vector2.right * 2;

            input = "Player1";
        }
        else {
            pos = new Vector2(GameManager.topRight.x, 0);
            pos -= Vector2.right * 2;
            if (isSingle){
                input = "Player1";
            }
            else {
                input = "Player2";
            } 
        }

        transform.position = pos;

        gameObject.name = input;
    }

    void Update() {
        float move = Input.GetAxis(input) * Time.deltaTime * speed;

        if (transform.position.y < GameManager.bottomLeft.y + height / 2 && move < 0){
            move = 0;
        }
        if (transform.position.y > GameManager.topRight.y - height / 2 && move > 0){
            move = 0;
        }

        if (Time.timeScale == 0) {
            Destroy(gameObject);
        }

        transform.Translate(move * Vector2.up);  
    }
}
