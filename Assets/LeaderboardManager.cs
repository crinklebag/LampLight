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
//Sort all leaderboards?
//Add a bool to leaderboard called Updated to make use of sorting leaderboards?
//Do scores with duplicate names matter?
//Display all leaderboards
//Code clean up
//Should leaderboard display setup be programatic?
//Add playerInfo text templates to leaderboard.  Instantiate according to numTopScores and height of text.
//Future development: creating leaderboards programatically according to numTopScores
//Get text fields

public class LeaderboardManager : MonoBehaviour 
{
    [SerializeField]
	private string filepath;

    [SerializeField]
    private string projectName;

	string filename;
	//public Text testText; //use for testing file paths
	private int numTopScores = 5;
    //public string[] delimiters = { @":" };
    char[] charSeparators = new char[] {':'};
	StreamReader infile;

    private class PlayerInfo
    {
        public string playerName = "??";
        public int playerScore = 0;
    }

    private class LeaderboardInfo
    {
        public string songName = "";
        public PlayerInfo[] playersInfo;   
    }

    private LeaderboardInfo[] leaderboardArray;
    private List<LeaderboardInfo> leaderboardList = new List<LeaderboardInfo>();

    private Player.SessionInfo info = new Player.SessionInfo();
    
	// Use this for initialization
	void Start () 
	{
        projectName = "Lamplight";
		filename = "test1";

		if (Application.platform == RuntimePlatform.WindowsEditor)
		{
            filepath = "../" + projectName + "/Assets/" + filename + ".txt";
		}

		else if(Application.platform == RuntimePlatform.Android)
		{
			filepath = Application.persistentDataPath + "/" + filename;
		}

        info = Player.instance.GetSessionInfo();  

        LoadScores();    
        UpdateAllScores(info.playerName, info, ref leaderboardList);
        WriteScores(ref leaderboardList);
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

        if(tempList.Count > 0)
        {
            leaderboardArray = new LeaderboardInfo[(fileInfo.Length / (numTopScores + 1))];

            RemoveBlankLines(ref tempList);

            fileInfo = tempList.ToArray();                       

            for (int i = 0; i < leaderboardArray.Length; i++)
            {
                leaderboardArray[i] = new LeaderboardInfo();
                leaderboardArray[i].playersInfo = new PlayerInfo[numTopScores];

                for (int j = 0; j < numTopScores; j++) 
                {
                    leaderboardArray[i].playersInfo[j] = new PlayerInfo();
                }
            }

            for (int j = 0; j < leaderboardArray.Length; j++)
            {

                for (int i = 0; i < numTopScores + 1; i++)
                {
                    if(i == 0)
                    {
                        leaderboardArray[j].songName = fileInfo[(numTopScores + 1) * j];
                    }

                    else
                    {
                         //string[] s = x[i].Split(delimiters, System.StringSplitOptions.None); use for splitting by string(s)
                        string[] tempStringArray = fileInfo[i + ((numTopScores + 1) * j)].Split(charSeparators, System.StringSplitOptions.None); //splitting by char(s)

                        leaderboardArray[j].playersInfo[i - 1].playerName = tempStringArray[0];
                        leaderboardArray[j].playersInfo[i - 1].playerScore = int.Parse (tempStringArray[1]);
                    }
                }
            }

    		infile.Close ();

            leaderboardList = leaderboardArray.ToList();

        }

        else
        {
            print("No data");
            //If no data exists, a new file should be created and a new entry should be created for each song
        }

        infile.Close ();
	}

    private void WriteScores(ref List<LeaderboardInfo> lbList)
    {
        LeaderboardInfo[] temp = lbList.ToArray();
        WriteScores(ref temp);
    }

	private void WriteScores(ref LeaderboardInfo[] lbArray)
	{
		StreamWriter outfile = new StreamWriter (filepath);
        //StreamWriter outfile = new StreamWriter (filepath2, true); // this is for appending
		//outfile.Write(""); for erasing contents

		for (int i = 0; i < lbArray.Length; i++) 
		{
		    outfile.WriteLine (lbArray[i].songName);
            for (int j = 0; j < lbArray[i].playersInfo.Length; j++) 
            {
                outfile.WriteLine(lbArray[i].playersInfo[j].playerName + charSeparators[0] + lbArray[i].playersInfo[j].playerScore);
            }
		}

		outfile.Close ();
	}

    private bool CheckIfExists(string songName, ref List<LeaderboardInfo> lb)
    {
       bool exists = false;
       for (int i = 0; i < lb.Count; i++)
       {
          if(lb[i].songName == songName)
          {
               exists = true;
          }
       }

       return exists;
    }

    private void WriteNewEntry(string songName)
    {     
       StreamWriter outfile = new StreamWriter (filepath, true);

       LeaderboardInfo temp = new LeaderboardInfo();
       temp.playersInfo = new PlayerInfo[numTopScores];
       temp.songName = songName;
       outfile.WriteLine("\n");
       outfile.WriteLine(temp.songName);

       for (int j = 0; j < temp.playersInfo.Length; j++) 
       {
           temp.playersInfo[j] = new PlayerInfo();
           outfile.WriteLine(temp.playersInfo[j].playerName + charSeparators[0] + temp.playersInfo[j].playerScore);
       }

       outfile.Close();
    }

    private void AddNewEntry(string songName, ref List<LeaderboardInfo> leaderboards)
    {
        LeaderboardInfo temp = new LeaderboardInfo();
        temp.playersInfo = new PlayerInfo[numTopScores];
        temp.songName = songName;

       for (int j = 0; j < temp.playersInfo.Length; j++) 
       {
           temp.playersInfo[j] = new PlayerInfo();
       }

       leaderboards.Add(temp);
    }

    private void UpdateAllScores(string playerName, Player.SessionInfo info, ref List<LeaderboardInfo> current) //Update all
    {
        for (int j = 0; j < info.playerInfo.Count; j++)
        {
            if(CheckIfExists(info.playerInfo[j].songName, ref current) == false)
            {
                AddNewEntry(info.playerInfo[j].songName, ref current);
            }
        }

        List<PlayerInfo> tempList;
        for (int i = 0; i < current.Count; i++)
        {
            for (int j = 0; j < info.playerInfo.Count; j++) 
            {
                if (current[i].songName == info.playerInfo[j].songName)
                {
                    tempList = current[i].playersInfo.ToList();
                    PlayerInfo tempInfo = new PlayerInfo();
                    tempInfo.playerName = playerName;
                    tempInfo.playerScore = info.playerInfo[j].playerScore;
                    tempList.Add(tempInfo);
                    SortScores(tempList);
                    tempList.RemoveAt(tempList.Count - 1);

                    current[i].playersInfo = tempList.ToArray();
                }

            }
        }

        SortAllScores(ref current);
    }

    private void UpdateScores(string song, PlayerInfo currentInfo, ref LeaderboardInfo[] current) //Update one
    {
        List<PlayerInfo> tempList;
        for (int i = 0; i < current.Length; i++)
        {
            if(current[i].songName == song)
            {
                print("found");
                tempList = current[i].playersInfo.ToList();
                tempList.Add(currentInfo);
                SortScores(tempList);
                tempList.RemoveAt(tempList.Count - 1);

                current[i].playersInfo = tempList.ToArray();
            }
        }
    }

    private void SortAllScores(ref List<LeaderboardInfo> leaderboards)
    {
        foreach (LeaderboardInfo leaderboard in leaderboards)
        {
            List<PlayerInfo> temp = leaderboard.playersInfo.ToList();
            SortScores(temp);
            leaderboard.playersInfo = temp.ToArray();
        }
    }

    private void SortScores(List<PlayerInfo> current) //Custom list sort method
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

    private void PrintLeaderboardContents(ref LeaderboardInfo[] lbArray)
    {
        //Checking contents
        for (int x = 0; x < lbArray.Length; x++)
        {
            print(lbArray[x].songName);
            for (int y = 0; y < lbArray[x].playersInfo.Length; y++)
            {
                print(lbArray[x].playersInfo[y].playerName + " " + lbArray[x].playersInfo[y].playerScore);
            }
        }
    }

    private void EraseContents()
    {
        StreamWriter outfile = new StreamWriter (filepath);
        outfile.Write("");
        outfile.Close();
    }
}