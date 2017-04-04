using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Text;

//TODO
//Need Gameobject with script that stores songs played and scores for those songs; access existing scripts?
//Build path test
//File access for different platforms
//Preprocessor directives for platform specific code?
//Add a bool to playerInfo that was added called Updated to make use of showing recent addition?
//Do scores with duplicate names matter?
//Code clean up
//Leaderboard offset variable?

public class LeaderboardManager : MonoBehaviour 
{
    [SerializeField]
    private string filepath;

    [SerializeField]
    private string projectName;

    string filename;
    //public Text testText; //use for testing file paths
    private int numTopScores = 5;
    private int numMaxScores = 100;
    //public string[] delimiters = { @":" };
    char[] charSeparators = new char[] {':'};
    StreamReader infile;

    [SerializeField] private Transform startPoint = null;
    public Vector2 leaderboardSize = Vector2.zero;
    public GameObject leaderboardPrefab = null;
    public Bounds leaderboardPrefabBounds;
    private int numLeaderboards = 0;
    public Canvas canvas = null;

    public GameObject[] leaderboards = null;

    public class PlayerInfo
    {
        public string playerName = "??";
        public int playerScore = 0;
        public bool wasAdded = false;
        public int rank = 0;
    }

    public class LeaderboardInfo
    {
        //public string songName = "";
        public PlayerInfo[] playersInfo;   
    }

    private LeaderboardInfo total = new LeaderboardInfo();
    private LeaderboardInfo topFive = new LeaderboardInfo();
    private LeaderboardInfo playerRanking = new LeaderboardInfo();
    public List<LeaderboardInfo> leaderboardList = new List<LeaderboardInfo>();

    private PlayerInfo info = new PlayerInfo();
    
    // Use this for initialization
    void Start () 
    {
        projectName = "Lamplight";
        filename = "test1";

        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            filepath = "../" + projectName + "/Assets/" + filename + ".txt";
        }

        else if(Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.WindowsPlayer ||
                Application.platform == RuntimePlatform.IPhonePlayer)
        {
            filepath = Application.persistentDataPath + "/" + filename;
        }

        info.playerName = Player.instance.info.playerName;
        info.playerScore = Player.instance.info.playerScore;
        info.wasAdded = true;  

        LoadScores();
        GetLeaderboards();
        //PrintLeaderboardContents(ref total);    
        DisplayLeaderboards();
    }
    
    // Update is called once per frame
    void Update () 
    {

    }

    private void LoadScores()
    {
        infile = new StreamReader (filepath);
        string[] fileInfo = File.ReadAllLines(filepath);
        List<string> tempList;

        tempList = fileInfo.ToList();
        print("t:" + tempList.Count);
        if(tempList.Count > 0)
        {
            RemoveBlankLines(ref tempList);

            fileInfo = tempList.ToArray();

            total.playersInfo = new PlayerInfo[fileInfo.Length];

            print(total.playersInfo.Length);

            for (int i = 0; i < fileInfo.Length; i++)
            {
                total.playersInfo[i] = new PlayerInfo(); 
                 //string[] s = x[i].Split(delimiters, System.StringSplitOptions.None); use for splitting by string(s)
                string[] tempStringArray = fileInfo[i].Split(charSeparators, System.StringSplitOptions.None); //splitting by char(s)

                total.playersInfo[i].playerName = tempStringArray[0];
                total.playersInfo[i].playerScore = int.Parse (tempStringArray[1]);
                total.playersInfo[i].wasAdded = false;
            } 

            infile.Close ();  
            UpdateScores(info, ref total);
            WriteScores(ref total);                    

        }

        else
        {
            infile.Close();
            print("No data");
            //WriteToFile(info);

            total.playersInfo = new PlayerInfo[numTopScores];

            for (int i = 0; i < numTopScores; i++)
            {
                total.playersInfo[i] = new PlayerInfo();
            }

            UpdateScores(info, ref total);
            WriteScores(ref total);  
        }
    }

    private void WriteScores(ref LeaderboardInfo leaderboard)
    {
        StreamWriter outfile = new StreamWriter (filepath);
        //StreamWriter outfile = new StreamWriter (filepath2, true); // this is for appending
        //outfile.Write(""); for erasing contents

        for (int i = 0; i < leaderboard.playersInfo.Length; i++) 
        {
            outfile.WriteLine(leaderboard.playersInfo[i].playerName + charSeparators[0] + leaderboard.playersInfo[i].playerScore);
        }

        outfile.Close ();
    }

    private void WriteToFile(PlayerInfo info)
    {
        StreamWriter outfile = new StreamWriter (filepath);
        outfile.WriteLine(info.playerName + charSeparators[0] + info.playerScore);
        outfile.Close();
    }

    private void UpdateScores(PlayerInfo currentInfo, ref LeaderboardInfo leaderboard)
    {
        List<PlayerInfo> tempList = leaderboard.playersInfo.ToList();
        tempList.Add(currentInfo);

        SortScores(ref tempList);

        if(tempList.Count > numMaxScores)
        {
            int range = tempList.Count - numMaxScores;
            tempList.RemoveRange(numMaxScores, range);
        }

        leaderboard.playersInfo = tempList.ToArray();

        for (int i = 0; i < total.playersInfo.Length; i++)
        {
            total.playersInfo[i].rank = i + 1;
        }
    }

    private void SortScores(ref List<PlayerInfo> current) //Custom list sort method
    {
        current.Sort((PlayerInfo x, PlayerInfo y)=>
        {
            if(x.playerScore > y.playerScore)
            {
                return -1;
            }

            if(x.playerScore < y.playerScore)
            {
                return 1;
            }

            else
            {
                return 0;
            }
        });
    }

    private void RemoveBlankLines(ref List<string> list)
    {
        List<string> tempList = new List<string>();

       for (int i = 0; i < list.Count; i++)
       {
          if(!string.IsNullOrEmpty(list[i]))
          {
              tempList.Add(list[i]);
          }
       }

       list = tempList;

    }

    private void PrintLeaderboardContents(ref LeaderboardInfo leaderboard)
    {
        //Checking contents
        for (int x = 0; x < leaderboard.playersInfo.Length; x++)
        {
            print(leaderboard.playersInfo[x].playerName + " " + leaderboard.playersInfo[x].playerScore);
        }
    }

    private void EraseContents()
    {
        StreamWriter outfile = new StreamWriter (filepath);
        outfile.Write("");
        outfile.Close();
    }

    public void GetLeaderboards()
    {
        int lowerBound = 0;
        int upperBound = 0;
        int position = 0;
        playerRanking.playersInfo = new PlayerInfo[numTopScores];
        PlayerInfo tempInfo = new PlayerInfo();

        //print("Total " + total.playersInfo.Length);

        for (int i = 0; i < numTopScores; i++)
        {
            playerRanking.playersInfo[i] = new PlayerInfo();
        }

        foreach (PlayerInfo temp in total.playersInfo)
        {
            if (temp.wasAdded)
            {
                tempInfo = temp;
            }
        }

        //print(tempInfo.playerName + " " + tempInfo.playerScore);

        for (int j = 0; j < total.playersInfo.Length; j++)
        {
            if(total.playersInfo[j].wasAdded)
            {
                tempInfo = total.playersInfo[j];
                position = j + 1;
                break;
            }
        }

        topFive.playersInfo = new PlayerInfo[numTopScores];

        for (int i = 0; i < numTopScores; i++)
        {
            topFive.playersInfo[i] = new PlayerInfo();
            topFive.playersInfo[i] = total.playersInfo[i];
        }

        //print("pos " + position);

        if(position > numTopScores)
        {
            int lowerOffset = (position - 1) - numTopScores;
            //print("lOff: " + lowerOffset);
            if(lowerOffset < 2)
            {
                lowerBound = numTopScores - 1;
                upperBound = position + numTopScores - 1;
            }

            else
            {
                if(position == total.playersInfo.Length)
                {
                    upperBound = 0;
                    lowerBound = position - numTopScores;
                    //print("Equal");
                }

                else if (position == (total.playersInfo.Length - 1))
                {
                    upperBound = position + 1 ;
                    lowerBound = position - (numTopScores - 2);
                }

                else
                {
                    upperBound = position + 2;
                    lowerBound = position - 2;
                }
            }

            leaderboardList.Add(topFive);
        }

        else
        {
            leaderboardList.Add(topFive);
            return;
        }

        /*print("lBound: " + lowerBound);
        print("uBound: " + upperBound);*/

        List<PlayerInfo> tempList = new List<PlayerInfo>();

        for (int k = lowerBound; k <= upperBound; k++)
        {
            tempList.Add(total.playersInfo[k - 1]);
        }

        PlayerInfo[] tempArray = tempList.ToArray();
        //print(tempArray.Length);

        playerRanking.playersInfo = tempList.ToArray();
        leaderboardList.Add(playerRanking);
    }

    private void DisplayLeaderboards()
    {
        startPoint.position = Camera.main.ViewportToWorldPoint(new Vector3(0,0,0));
        numLeaderboards = leaderboardList.Count;
        print(numLeaderboards);

        GameObject tempBoard = Instantiate(leaderboardPrefab) as GameObject;
        leaderboardPrefabBounds = tempBoard.GetComponent<BoxCollider2D>().bounds;
        DestroyImmediate(tempBoard);
        print(leaderboardPrefabBounds.size);

        for (int x = 0; x < numLeaderboards; x++) 
        {
            GameObject board = Instantiate(leaderboardPrefab) as GameObject;
            board.transform.parent = canvas.transform;

            if(numLeaderboards > 1)
            {
                board.transform.localPosition = new Vector2((x * leaderboardPrefabBounds.size.x) + startPoint.position.x, 0);
            }

            else
            {
                board.transform.localPosition = Vector2.zero;
            }
        }

        leaderboards = GameObject.FindGameObjectsWithTag("Leaderboard");
        print(leaderboards.Length);
        //print(leaderboards[0].transform.childCount);

        for (int i = 0; i < leaderboards.Length; i++)
        {
            for (int j = 0; j < leaderboards[i].transform.childCount; j++) 
            {
                Transform child = leaderboards[i].transform.GetChild(j);
                if(child.gameObject.tag == "PlayerInfo")
                {
                    child.gameObject.GetComponent<Text>().text = 
                    leaderboardList[i].playersInfo[j].rank + " " +
                    leaderboardList[i].playersInfo[j].playerName + " " + 
                    leaderboardList[i].playersInfo[j].playerScore; 
                }
            }
        }
    }
}