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
//Two files required for testing purposes.  Should be one for the final version?
//File access for different platforms
//Player enters name for each song.  Button that says save name for all?
//Preprocessor directives for platform specific code?
//UpdateScores overload for all?
//Fix/rework updating scores
//Sort all leaderboards
//Change to work with one save file
//Do scores with duplicate names matter?
//Redo remove blank lines

public class LeaderboardManager : MonoBehaviour 
{
	public string filepath;
	public string filepath2;
    private string projectName;
	string filename;
	string filename2;
	public Text testText;
	public int numTopScores = 10;
    //public string[] delimiters = { @":" };
    char[] charSeparators = new char[] {':'};
	StreamReader infile;

    public class PlayerInfo
    {
        public string playerName = "??";
        public int playerScore = 0;
    }

    public class LeaderboardInfo
    {
        public string songName = "";
        public PlayerInfo[] playersInfo;   
    }

    public LeaderboardInfo[] leaderboardArray;

    public PlayerInfo tempInfo = new PlayerInfo();
    
	// Use this for initialization
	void Start () 
	{
        projectName = "LeaderboardTester2";
		filename = "test1"; //read
		filename2 = "test2"; //write
		if (Application.platform == RuntimePlatform.WindowsEditor)
		{
            filepath = "../" + projectName + "/Assets/" + filename + ".txt";
            filepath2 = "../" + projectName + "/Assets/" + filename2 + ".txt";
		}

		else if(Application.platform == RuntimePlatform.Android)
		{
			filepath = Application.persistentDataPath + "/" + filename;
            filepath2 = Application.persistentDataPath + "/" + filename2;
		}

		//infile = new StreamReader (filepath);

        string tempSong = "song4";
        tempInfo.playerName = "temp";
        tempInfo.playerScore = 305;	
        LoadScores();

        if(CheckIfExists("song4") == false)
        {
            WriteNewEntry("song4");
        }

        //UpdateScores(tempSong, tempInfo, ref leaderboardArray);
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void LoadScores()
	{
        infile = new StreamReader (filepath);
        string[] fileInfo = File.ReadAllLines(filepath);
        //print(fileInfo.Length);
        List<string> tempList;

        tempList = fileInfo.ToList();
        print(fileInfo.Length);
        if(tempList.Count > 0)
        {
            leaderboardArray = new LeaderboardInfo[(fileInfo.Length / (numTopScores + 1))];
            //Debug.Log("Length" + leaderboardArray.Length);

            RemoveBlankLines(ref tempList);

            /*if(tempList.Count % (numTopScores + 1) == 0)
            {
                tempList.RemoveRange(((numTopScores + 1) * leaderboardArray.Length), 
                                    (tempList.Count -((numTopScores + 1) * leaderboardArray.Length) ));
            }*/

            fileInfo = tempList.ToArray();                       
            print(fileInfo.Length);
                                    
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
                    //print("i " + i);
                    if(i == 0)
                    {
                        leaderboardArray[j].songName = fileInfo[(numTopScores + 1) * j];
                        //print(leaderboardArray[j].songName);
                    }

                    else
                    {
                         //string[] s = x[i].Split(delimiters, System.StringSplitOptions.None); use for splitting by string(s)
                        string[] tempStringArray = fileInfo[i + ((numTopScores + 1) * j)].Split(charSeparators, System.StringSplitOptions.None); //splitting by char(s)
                        //Debug.Log(tempStringArray.Length);
                        //Debug.Log("Split: " + tempStringArray[0] + " " + tempStringArray[1]);

                        //print(i + ((numTopScores + 1) * j));

                        leaderboardArray[j].playersInfo[i - 1].playerName = tempStringArray[0];
                        leaderboardArray[j].playersInfo[i - 1].playerScore = int.Parse (tempStringArray[1]);
                        //Debug.Log("PlayerName: " + leaderboardArray[j].playersInfo[i - 1].playerName);
                        //Debug.Log("PlayerScore: " + leaderboardArray[j].playersInfo[i - 1].playerScore);
                    }
                }
            }

    		infile.Close ();
            SortAllScores(ref leaderboardArray);
            WriteScores(leaderboardArray);
        }

        else
        {
            print("No data");
            //If no data exists, a new entry should be created for each song
        }

        //Checking contents
        /*for (int x = 0; x < leaderboardArray.Length; x++)
        {
            print(leaderboardArray[x].songName);
            for (int y = 0; y < leaderboardArray[x].playersInfo.Length; y++)
            {
                print(leaderboardArray[x].playersInfo[y].playerName + " " + leaderboardArray[x].playersInfo[y].playerScore);
            }
        }*/
	}

	public void WriteScores(LeaderboardInfo[] list)
	{
		StreamWriter outfile = new StreamWriter (filepath);
        //StreamWriter outfile = new StreamWriter (filepath2, true); // this is for appending
		//outfile.Write(""); for erasing contents

		for (int i = 0; i < list.Length; i++) 
		{
		    outfile.WriteLine (list[i].songName);
            for (int j = 0; j < list[i].playersInfo.Length; j++) 
            {
                //Debug.Log("PlayerName: " + list[i].playersInfo[j].playerName);
                //Debug.Log("PlayerScore: " + list[i].playersInfo[j].playerScore);
                outfile.WriteLine(list[i].playersInfo[j].playerName + charSeparators[0] + list[i].playersInfo[j].playerScore);
            }
		}

		outfile.Close ();
	}

    public bool CheckIfExists(string songName)
    {
       bool exists = true;
       for (int i = 0; i < leaderboardArray.Length; i++)
       {
          if(leaderboardArray[i].songName == songName)
          {
               exists = true;
          }

          else
          {
               exists = false;
          }
       }

       return exists;
    }

    public void WriteNewEntry(string songName)
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

    /*public void UpdateScores(PlayerInfo currentInfo, ref LeaderboardInfo current) //Update one
    {
       //Cast array to list, sort, cast back and then write to file

       List<PlayerInfo> tempList;
       tempList = current.playersInfo.ToList();

       tempList.Add(currentInfo);
       SortScores(tempList);
       tempList.RemoveAt(tempList.Count - 1);

       current.playersInfo = tempList.ToArray();
       LoadScores();

    }*/

    /*public void UpdateScores(string[] songs, PlayerInfo[] currentInfo, LeaderboardInfo[] current) //Update all
    {
        
    }*/

    public void UpdateScores(string song, PlayerInfo currentInfo, ref LeaderboardInfo[] current)
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

                for (int j = 0; j < current[i].playersInfo.Length; j++)
                {
                    print(current[i].playersInfo[j].playerName);
                }
            }

            else
            {
                print("not found");
            }
        }

       WriteScores(current);
       //LoadScores();
    }

    public void SortAllScores(ref LeaderboardInfo[] leaderboards)
    {
        foreach (LeaderboardInfo leaderboard in leaderboards)
        {
            List<PlayerInfo> temp = leaderboard.playersInfo.ToList();
            SortScores(temp);
            leaderboard.playersInfo = temp.ToArray();
        }
    }

    public void SortScores(List<PlayerInfo> current) //Custom list sort method
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

    public void RemoveBlankLines(ref List<string> list)
    {
        int num = 0;
       /* for (int i = 0; i < list.Count; i++)
        {
            if(string.IsNullOrEmpty(list[i]))
            {
                //print("Null");
                print("count " + list.Count);
                num++;
                list.RemoveAt(i);
                print(list[i] + " " + num);

            }
        }*/

        foreach(string line in list)
        {
            if(string.IsNullOrEmpty(line))
            {
                print("Null");
                //print("count " + list.Count);
                list.Remove(line);
                //print(list[i] + " " + num);

            }
        }
    }
}