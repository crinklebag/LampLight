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
    public Text[] songNameList;
    public Text[] songLengthList;

    [SerializeField]
    private AudioClip[] songList;
    [SerializeField]
    private AudioClip chosenSong;

    [SerializeField]
    private Sprite[] BGList;
    [SerializeField]
    private Sprite chosenBG;

    [SerializeField]
    bool setGame = false;
    [SerializeField]
    bool setBGMenuItems = false;

    [SerializeField]
    GameObject[] me;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);

        //Debug.Log(this.gameObject.GetInstanceID());

        me = GameObject.FindGameObjectsWithTag("ObjectKeeper");

        if (me.Length < 2)
        {
            return;
        }

        for (int i = 0; i < me.Length; i++)
        {
            if (this.gameObject.GetInstanceID() > me[i].GetInstanceID())
            {
                Destroy(me[i].gameObject);
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        SetUpScene(1);
    }

    // Update is called once per frame
    void Update()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Polly's Scene":
                {
                    if (setGame)
                    {
                        return;
                    }

                    if (!setGame)
                    {
                        GameObject.Find("WhatTree").GetComponent<Image>().sprite = chosenBG;

                        //Debug.Log("Finding AudioPeer");
                        GameObject.Find("AudioPeer").gameObject.GetComponent<AudioSource>().Stop();
                        GameObject.Find("AudioPeer").gameObject.GetComponent<AudioSource>().clip = chosenSong;
                        GameObject.Find("BG").gameObject.GetComponent<BackgroundScroller>().Reset(GameObject.Find("AudioPeer").gameObject.GetComponent<AudioSource>().clip.length);
                        GameObject.Find("AudioPeer").gameObject.GetComponent<AudioSource>().Play();
                        GameObject.Find("GameController").gameObject.GetComponent<GameController>().SetStartGame(true);
                        setGame = true;
                    }
                }
                break;
            case "BGSelect":
                {
                    if (setBGMenuItems == false)
                    {
                        SetUpScene(2);
                        setBGMenuItems = true;
                    }
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
                SceneManager.LoadScene("BGSelect");
            }
        }
    }

    public void SelectThisBG()
    {
        chosenBG = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite;

        SceneManager.LoadScene("Polly's Scene");
    }

    public void SetUpScene(int whatScene)
    {
        switch (whatScene)
        {
            case 1: // SongSelect
                {
                    songNameList = new Text[6];
                    songLengthList = new Text[6];

                    for (int i = 0; i < songNameList.Length; i++)
                    {
                        songNameList[i] = GameObject.Find("Song Name " + i.ToString()).GetComponent<Text>();
                        songLengthList[i] = GameObject.Find("Song Length " + i.ToString()).GetComponent<Text>();
                    }

                    for (int i = 0; i < songNameList.Length; i++)
                    {
                        songNameList[i].text = songList[i].name;

                        TimeSpan ts = TimeSpan.FromSeconds(songList[i].length);

                        songLengthList[i].text = string.Format("{0:D2}:{1:D2}", ts.Minutes, ts.Seconds);
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
