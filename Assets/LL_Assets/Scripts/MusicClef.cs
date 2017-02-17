using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicClef : MonoBehaviour {

    public GameObject[] lines;

    public float GetYPos(int val)
    {
        return lines[val].gameObject.transform.position.y;
    }

    public int GetLinesLength()
    {
        return lines.Length;
    }
}
