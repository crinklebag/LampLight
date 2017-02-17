using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ObjectKeeper : MonoBehaviour
{
    public GameObject[] songNameList;
    public GameObject[] songLengthList;

    [SerializeField]
    private AudioClip[] songList;
    [SerializeField]
    private AudioClip chosenSong;
    [SerializeField]
    private int chosenSongNum;

    [SerializeField]
    private Sprite[] BGList;
    [SerializeField]
    private Sprite chosenBG;

    [SerializeField]
    bool setGame = false;
    [SerializeField]
    bool setBGMenuItems = false;
    [SerializeField]
    bool setSongSelectItems = false;

    [SerializeField]
    GameObject[] me;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);

        //Debug.Log(this.gameObject.GetInstanceID());

        me = GameObject.FindGameObjectsWithTag("ObjectKeeper");

        if (me.Length < 2)
        {
            this.name = "ObjectKeeper ORIGINAL";
            return;
        }

        for (int i = 0; i < me.Length; i++)
        {
            //if (this.gameObject.GetInstanceID() > me[i].GetInstanceID())
            if (me[i].gameObject.name == "ObjectKeeper")
            {
                Destroy(me[i].gameObject);
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Main_Mobile":
                {
                    if (setGame)
                    {
                        return;
                    }

                    if (!setGame)
                    {
                        GameObject.Find("WhatTree").GetComponent<Image>().sprite = chosenBG;

                        //Debug.Log("Finding AudioPeer");
                        GameObject.Find("AudioManager").gameObject.GetComponent<AudioSource>().Stop();
                        //GameObject.Find("AudioManager").gameObject.GetComponent<AudioSource>().clip = chosenSong;
                        StartCoroutine(GameObject.Find("AudioManager").gameObject.GetComponent<AudioManager>().StartAudio(chosenSongNum));
                        //GameObject.Find("BG").gameObject.GetComponent<BackgroundScroller>().Reset(GameObject.Find("AudioPeer").gameObject.GetComponent<AudioSource>().clip.length);
                        //GameObject.Find("AudioManager").gameObject.GetComponent<AudioSource>().Play();
                        GameObject.Find("GameController").gameObject.GetComponent<GameController>().SetStartGame(true);
                        setGame = true;
                    }
                }
                break;
            case "BGSelect":
                {
                    setSongSelectItems = false;
                    setGame = false;

                    if (setBGMenuItems == false)
                    {
                        SetUpScene(2);
                        setBGMenuItems = true;
                    }
                }
                break;
            case "SongSelect":
                {
                    setBGMenuItems = false;
                    setGame = false;

                    if (setSongSelectItems == false)
                    {
                        SetUpScene(1);
                        setSongSelectItems = true;
                    }
                }
                break;
            case "MainMenu_Mobile":
                {
                    setSongSelectItems = false;
                    setBGMenuItems = false;
                    setGame = false;
                    songNameList = new GameObject[10];
                    songLengthList = new GameObject[10];
                }
                break;
            default:
                break;
        }

    }

    public void SelectThisSong()
    {
        for (int i = 0; i < songList.Length; i++)
        {
            if (EventSystem.current.currentSelectedGameObject.GetComponent<Text>().text == songList[i].name)
            {
                chosenSong = songList[i];
                chosenSongNum = i;
                SceneManager.LoadScene("BGSelect");
            }
        }
    }

    public void SelectThisBG()
    {
        chosenBG = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite;

        SceneManager.LoadScene("Main_Mobile");
    }

    public void SetUpScene(int whatScene)
    {
        switch (whatScene)
        {
            case 1: // SongSelect
                {
                    songNameList = new GameObject[6];
                    songLengthList = new GameObject[6];

                    for (int i = 0; i < songNameList.Length; i++)
                    {
                        songNameList[i] = GameObject.Find("Song Name " + i.ToString());
                        songLengthList[i] = GameObject.Find("Song Length " + i.ToString());
                        songNameList[i].gameObject.GetComponent<Button>().onClick.AddListener(delegate { SelectThisSong(); });
                    }
                    


                    for (int i = 0; i < songNameList.Length; i++)
                    {
                        songNameList[i].GetComponent<Text>().text = songList[i].name;

                        TimeSpan ts = TimeSpan.FromSeconds(songList[i].length);

                        songLengthList[i].GetComponent<Text>().text = string.Format("{0:D2}:{1:D2}", ts.Minutes, ts.Seconds);
                    }
                }
                break;
            case 2: // BGSelect
                {
                    for (int i = 0; i < BGList.Length; i++)
                    {
                        GameObject.Find("Background " + i.ToString()).GetComponent<Image>().sprite = BGList[i];
                        GameObject.Find("Background " + i.ToString()).GetComponent<Button>().onClick.AddListener(delegate { SelectThisBG(); });
                    }
                }
                break;
            default:
                break;
        }
    }
}
