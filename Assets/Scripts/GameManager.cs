using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    /////////////////////VARIABLES/////////////////////

    //////////UI OBJECTS//////////
    public GameObject MainMenu;
    public GameObject SettingMenu; //too lazy to change this, this is the singleplayer menu
    public GameObject GameUI;
    public GameObject Timer;
    public GameObject GameOver;
    public GameObject GameOverText;


    //////////GAME MANAGEMENT VARS//////////
    static bool isMultiplayer;
    bool gameStarted;
    static float timeSurvived;
    public float multiplyInterval = 5; 
    float ballTime;
    public GameObject ball;
    public GameObject player;
    public static Vector2 topRight;
    public static Vector2 bottomLeft;
    float minutes;
    float seconds;
    
   

    /////////////////////GAME MANAGEMENT/////////////////////

    void Start()
    {
        Time.timeScale = 0;
        bottomLeft = Camera.main.ScreenToWorldPoint (new Vector2 (0, 0));
        topRight = Camera.main.ScreenToWorldPoint (new Vector2 (Screen.width, Screen.height));
    }
    void Update()
    {
        if (gameStarted) {
            //Multiplying balls
            ballTime -= Time.deltaTime;
            if (ballTime < 0){
                Instantiate(ball);
                ballTime = multiplyInterval;
            }
            //Handling game timer
            timeSurvived += Time.deltaTime;
            minutes = Mathf.FloorToInt(timeSurvived / 60);
            seconds = Mathf.FloorToInt(timeSurvived % 60);
            if (seconds < 10){
                Timer.GetComponent<Text>().text = minutes.ToString() + ":0" + seconds.ToString(); 
            }
            else {
                Timer.GetComponent<Text>().text = minutes.ToString() + ":" + seconds.ToString(); 
            }
        }

        if (Input.GetKeyUp(KeyCode.Escape)){
            BackToMainMenu();
        }
    }

    


    /////////////////////UI MANAGEMENT/////////////////////

    public void StartSingleGame() {
        MainMenu.SetActive(false);
        SettingMenu.SetActive(true);
    }

    public void StartMultiGame() {
        MainMenu.SetActive(false);
        SettingMenu.SetActive(true);
    }

    public void SetMultiplyInterval(GameObject toggle) {
        string intervalString = toggle.name.ToString();
        multiplyInterval = float.Parse(intervalString);
    }

    public void BackToMainMenu() {
        SettingMenu.SetActive(false);
        GameUI.SetActive(false);
        GameOver.SetActive(false);
        MainMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void StartGame(GameObject side) {
        SettingMenu.SetActive(false);
        GameUI.SetActive(true);
        if (side.name == "Left") {
            isMultiplayer = false;
            GameObject player1 = Instantiate(player) as GameObject;
            player1.GetComponent<PlayerController>().Init(true, true);
        }
        else if (side.name == "Right") {
            isMultiplayer = false;
            GameObject player1 = Instantiate(player) as GameObject;
            player1.GetComponent<PlayerController>().Init(false, true);
        }
        else if (side.name == "Multiplayer") {
            isMultiplayer = true;
            GameObject player1 = Instantiate(player) as GameObject;
            GameObject player2 = Instantiate(player) as GameObject;
            player1.GetComponent<PlayerController>().Init(true, false);
            player2.GetComponent<PlayerController>().Init(false, false);
        }
        Time.timeScale = 1;
        timeSurvived = 0;
        ballTime = multiplyInterval;
        gameStarted = true;
        Instantiate(ball);
    }
    
    public void GameEnd(bool isLeft) {
        GameUI.SetActive(false);
        GameOver.SetActive(true);
        Time.timeScale = 0;
        if (isMultiplayer) {
            if(isLeft){
                Debug.Log("Multiplayer P1 lose");
            }
            else{
                Debug.Log("Multiplayer P2 lose");
            }
        }
        else {
            if (seconds < 10)
                GameOverText.GetComponent<Text>().text = "Game Over! \nYou survived for " + minutes + ":0" + seconds;
            else {
                GameOverText.GetComponent<Text>().text = "Game Over! \nYou survived for " + minutes + ":" + seconds;
            }
            
        }
    }

    public void ShutDown() {
        Application.Quit();
    }

}
