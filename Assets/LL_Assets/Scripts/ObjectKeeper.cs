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

    [SerializeField]
    private string[] sceneNames;

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
        else
        {
            for (int i = 0; i < me.Length; i++)
            {
                //if (this.gameObject.GetInstanceID() > me[i].GetInstanceID())
                if (me[i].gameObject.name == "ObjectKeeper")
                {
                    Destroy(me[i].gameObject);
                }
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        sceneNames = new string[4];
        sceneNames[0] = "MainMenu_Mobile";
        sceneNames[1] = "SongSelect";
        sceneNames[2] = "BGSelect";
        sceneNames[3] = "Main_Mobile";

        if (Application.platform == RuntimePlatform.XboxOne /*|| Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor*/)
        {
            sceneNames[0] = "MainMenu_Kinect";
            sceneNames[3] = "Main_Kinect";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == sceneNames[0])
        {
            setSongSelectItems = false;
            setBGMenuItems = false;
            setGame = false;
        }
        else if (SceneManager.GetActiveScene().name == sceneNames[1])
        {
            setBGMenuItems = false;
            setGame = false;

            if (setSongSelectItems == false)
            {
                SetUpScene(1);
                setSongSelectItems = true;
            }
        }
        else if (SceneManager.GetActiveScene().name == sceneNames[2])
        {
            setSongSelectItems = false;
            setGame = false;

            if (setBGMenuItems == false)
            {
                SetUpScene(2);
                setBGMenuItems = true;
            }
        }
        else if (SceneManager.GetActiveScene().name == sceneNames[3])
        {
            if (setGame)
            {
                return;
            }

            if (!setGame)
            {
                StartCoroutine("Delay");
                setGame = true;
            }
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
                SceneManager.LoadScene(sceneNames[2]);
            }
        }
    }

    public void SelectThisBG()
    {
        chosenBG = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite;

        SceneManager.LoadScene(sceneNames[3]);
        //StartCoroutine("Delay");
    }

    public void SetUpScene(int whatScene)
    {
        switch (whatScene)
        {
            case 1: // SongSelect
                {
                    for (int i = 0; i < songList.Length; i++)
                    {
                        GameObject.Find("Song Name " + i.ToString()).GetComponent<Button>().onClick.AddListener(delegate { SelectThisSong(); });
                        GameObject.Find("Song Name " + i.ToString()).GetComponent<Text>().text = songList[i].name;

                        TimeSpan ts = TimeSpan.FromSeconds(songList[i].length);

                        GameObject.Find("Song Length " + i.ToString()).GetComponent<Text>().text = string.Format("{0:D2}:{1:D2}", ts.Minutes, ts.Seconds);
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

    IEnumerator Delay()
    {
        yield return new WaitForSecondsRealtime(3.0f);

        GameObject.Find("WhatTree").GetComponent<Image>().sprite = chosenBG;
        GameObject.Find("AudioManager").gameObject.GetComponent<AudioSource>().Stop();
        StartCoroutine(GameObject.Find("AudioManager").gameObject.GetComponent<AudioManager>().StartAudio(chosenSongNum));
        GameObject.Find("Directional light").GetComponent<LightController>().SetGame();
        GameObject.Find("GameController").gameObject.GetComponent<GameController>().SetStartGame(true);
    }
}
