using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//TODO
//Everytime the song changes, record the song name and final score

public class Player : MonoBehaviour 
{
    public static Player instance = null;
    [SerializeField]
    private InputField playerNameText = null;

    [SerializeField]
    private Text scoreText = null;

    public int score = 0; //for testing purposes

    public class PlayerInfo
    {
        public string playerName = "??";
        public int playerScore = 0;
    }

    public PlayerInfo info = new PlayerInfo();

    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
        {
            //if not, set instance to this
            instance = this;
        }        

        //If instance already exists and it's not this:
        else if (instance != this)
        {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance.
            Destroy(gameObject); 
        }
               
        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

	// Use this for initialization
	void Start () 
    {
        info.playerScore = score; //Get score from other script
        scoreText.text = score.ToString();
	}
	
	// Update is called once per frame
	void Update () 
    {

	}

    public void GetName()
    {
        info.playerName = playerNameText.text;
        SceneManager.LoadScene("LeaderboardTester");
    }
}
