using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using DiscordPresence;

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
    public GameObject HighScoreText;
    public GameObject PlayerName;
    public GameObject secretIDMenu;
    public GameObject PlayerID;

    //////////SERVER VARS//////////

    private string secretKey = "pios345noib09845ujg";
    string uniquePlayerID;
    public string addScoreURL = "http://steirii.xyz/add_score.php?";

    //////////GAME MANAGEMENT VARS//////////
    static bool isMultiplayer;
    bool gameStarted;
    static float timeSurvived;
    public float multiplyInterval; 
    float ballTime;
    public GameObject ball;
    public GameObject player;
    public static Vector2 topRight;
    public static Vector2 bottomLeft;
    float minutes;
    float seconds;
    public float highScoreMultiplyInterval;
    public float highScoreTimeSurvived;

    [Serializable]
    class SaveData {
        public float savedInterval;
        public float savedTime;
        public string savedID;
    }

    
   

    /////////////////////GAME MANAGEMENT/////////////////////

    void Start()
    {
        Time.timeScale = 0;
        bottomLeft = Camera.main.ScreenToWorldPoint (new Vector2 (0, 0));
        topRight = Camera.main.ScreenToWorldPoint (new Vector2 (Screen.width, Screen.height));
        LoadScores();
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
            PresenceManager.UpdatePresence(detail: "In Game", state: "Time: " + Timer.GetComponent<Text>().text + " | Multiply interval: " + multiplyInterval);
        }

        if (Input.GetKeyUp(KeyCode.Escape)){
            BackToMainMenu();
        }
    }

    


    /////////////////////UI MANAGEMENT/////////////////////

    public void StartSingleGame() {
        MainMenu.SetActive(false);
        SettingMenu.SetActive(true);
        if (multiplyInterval == 100) {
            multiplyInterval = 5;
        }      
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

    public void SubmitScoreFromMain() {
        MainMenu.SetActive(false);
        GameOver.SetActive(true);
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
        gameStarted = false;
        GameUI.SetActive(false);
        GameOver.SetActive(true);
        PresenceManager.UpdatePresence(detail: "In Menu", state: "");
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

        //SAVING TIME AND INTERVAL LOCALLY
        if (multiplyInterval < highScoreMultiplyInterval) {
                highScoreMultiplyInterval = multiplyInterval;
                highScoreTimeSurvived = timeSurvived;
                SaveScores();
        }
        else if (multiplyInterval == highScoreMultiplyInterval) {
            if (timeSurvived >= highScoreTimeSurvived) {
                highScoreMultiplyInterval = multiplyInterval;
                highScoreTimeSurvived = timeSurvived;
                SaveScores();       
            }
        }
        
        float min = Mathf.FloorToInt(highScoreTimeSurvived / 60);
        float sec = Mathf.FloorToInt(highScoreTimeSurvived % 60);
        if (sec < 10) {
            HighScoreText.GetComponent<Text>().text = "High Score: " + min + ":0" + sec + " at an interval of " + highScoreMultiplyInterval + " seconds";
        }
        else {
            HighScoreText.GetComponent<Text>().text = "High Score: " + min + ":" + sec + " at an interval of " + highScoreMultiplyInterval + " seconds";  
        }
    }

    public void SubmitTime() {
        if (PlayerName.GetComponent<Text>().text == "") {
        }
        else {
            string namevar = PlayerName.GetComponent<Text>().text;
            float min = Mathf.FloorToInt(highScoreTimeSurvived / 60);
            float sec = Mathf.FloorToInt(highScoreTimeSurvived % 60);
            if (sec < 10) {
                StartCoroutine(PostTime(namevar, (int)highScoreMultiplyInterval, min + ":0" + sec, uniquePlayerID));
            }
            else {
                StartCoroutine(PostTime(namevar, (int)highScoreMultiplyInterval, min + ":" + sec, uniquePlayerID));
            }   
        }  
    }

    public void SavePlayerID() {
        if (PlayerID.GetComponent<Text>().text == "") {
        }
        else {
            string id = PlayerID.GetComponent<Text>().text;
            if (id.Length < 20) {
            }
            else {
                uniquePlayerID = id;
                SaveScores();
                LoadScores();
                secretIDMenu.SetActive(false);
            }
        }  
    }

    public void LoadButton() {
        LoadScores();
    }


    //SAVE AND LOAD SCORES
    void SaveScores() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/SaveData.dat");
        SaveData data = new SaveData();
        data.savedID = uniquePlayerID;
        data.savedTime = timeSurvived;
        data.savedInterval = multiplyInterval;
        bf.Serialize(file, data);
        file.Close();
    }

    void LoadScores() {
        if (File.Exists(Application.persistentDataPath + "/SaveData.dat")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/SaveData.dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();
            uniquePlayerID = data.savedID;
            highScoreMultiplyInterval = data.savedInterval;
            highScoreTimeSurvived = data.savedTime;
            MainMenu.SetActive(true);
        }
        else {
            secretIDMenu.SetActive(true);
        }
    }


    public void ShutDown() {
        Application.Quit();
    }

    /////////////////////SERVER MANAGEMENT/////////////////////

    public string Md5Sum(string strToEncrypt)
{
	System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
	byte[] bytes = ue.GetBytes(strToEncrypt);
 
	// encrypt bytes
	System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
	byte[] hashBytes = md5.ComputeHash(bytes);
 
	// Convert the encrypted bytes back to a string (base 16)
	string hashString = "";
 
	for (int i = 0; i < hashBytes.Length; i++)
	{
		hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
	}
 
	return hashString.PadLeft(32, '0');
}



    IEnumerator PostTime(string Name, int MultiplyInterval, string Time, string UPID) {
        string hash = Md5Sum(Name + MultiplyInterval + Time + UPID + secretKey);

        string post_url = addScoreURL + "name=" + WWW.EscapeURL(Name) + "&multiplyinterval=" + MultiplyInterval + "&time=" + Time + "&upid=" + UPID + "&hash=" + hash;

        WWW hs_post = new WWW(post_url);
        yield return hs_post;

        if (hs_post.error != null) {
            print("There was an error posting your score: " + hs_post.error);
            print(post_url);
        }
        else {
            Debug.Log("Sent");
            Debug.Log(post_url);
        }
    }





}
