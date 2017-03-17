using UnityEngine;
using System.Collections;

public class Drag : MonoBehaviour {
	float distance = 10;
	Rigidbody2D rb;
	[SerializeField]float speed = 10.0f;
	[SerializeField]float rotSpeed = 25.0f;
	[SerializeField]float flashTime = 1.5f;
	[SerializeField]int numOfFlashes = 3;

	//angles for look at 2d
	protected Vector3 normTarget;
	protected float angle;
	protected Quaternion rot;

    [SerializeField]
    bool hasEndedGame = false;
	[SerializeField]
	bool waitForAWhile = false;

	private GameObject netSprites;

	void Start()
	{
		rb = this.GetComponent<Rigidbody2D>();
		netSprites = GameObject.Find("Net Sprites");

		GetComponentInChildren<SkinnedMeshRenderer>().sortingLayerName = "Bug";

        
	}

	void Update ()
	{
		if (Input.touchCount > 0) {

            if (!waitForAWhile)
            {
                Vector3 touchPosition = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0.0f);
                Vector3 objPosition = Camera.main.ScreenToWorldPoint(touchPosition);

                //Debug.Log(Vector3.Distance(objPosition, this.transform.position));

                //speed = Mathf.Lerp(speed, Mathf.Clamp((Vector3.Distance(objPosition, this.transform.position) - 5.0f), 5.0f, 50.0f), 25.0f * Time.deltaTime);

                normTarget = (objPosition - this.transform.position).normalized;

                angle = Mathf.Atan2(normTarget.y, normTarget.x) * Mathf.Rad2Deg;

                rot = new Quaternion();
                rot.eulerAngles = new Vector3(0, 0, angle - 90);

                this.transform.rotation = Quaternion.Slerp(this.transform.localRotation, rot, Time.deltaTime * rotSpeed);

                //rb.MovePosition(this.transform.localPosition + this.transform.up * Time.deltaTime * speed);

                rb.AddForce(normTarget * speed, ForceMode2D.Force);

                rb.velocity = Vector2.ClampMagnitude(rb.velocity, 8.0f);

                //Debug.Log(rb.velocity);
            }

            if (hasEndedGame)
            {
                Debug.Log("Touched at end");
                GameObject.FindGameObjectWithTag("UIController").GetComponent<UI>().SetHasTouchedAtEnd(true);
            }
		}

	}

    public void SetEndGame(bool val)
    {
		waitForAWhile = val;

		if (waitForAWhile == true)
        {
			StartCoroutine ("Wait4ScoreCount");
		}
    }

	IEnumerator Wait4ScoreCount()
	{
		yield return new WaitForSecondsRealtime(3.0f);
		hasEndedGame = true;
	}


	public IEnumerator FlashJar()
    {
    	for(int i = 0; i< numOfFlashes; i++)
    	{
			netSprites.SetActive(false);	
			yield return new WaitForSecondsRealtime(flashTime/2.0f);
       	 	netSprites.SetActive(true);
			yield return new WaitForSecondsRealtime(flashTime);
    	}
    }
}