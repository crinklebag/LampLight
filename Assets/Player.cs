using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//TODO
//Everytime the song changes, record the song name and final score

public class Player : MonoBehaviour 
{
    public static Player instance = null;

    public class PlayerInfo
    {
        public string playerName = "??";
        public int playerScore = 0;
    }

    public class SessionInfo
    {
        public string playerName;
        public List<PlayerInfo> playerInfo = new List<PlayerInfo>();
    }

    //public SessionInfo info = new SessionInfo();

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
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject); 
        }
               
        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

	// Use this for initialization
	void Start () 
    {
		/*info.playerName = playerName;

        AddInfo("song6", 32);
        AddInfo("song3", 9000);
        AddInfo("song1", 3);*/

        info.playerName = "RGD";
        info.playerScore = 20000;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(Input.GetKey(KeyCode.Space) && SceneManager.GetActiveScene().name != "LeaderboardTester")
        {
            //Application.LoadLevel("LeaderboardTester");
            SceneManager.LoadScene("LeaderboardTester");
        }
	}
}
