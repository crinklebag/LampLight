using UnityEngine;
using System.Collections;

public class Jar : MonoBehaviour {

    [SerializeField] float boostForce;
    [SerializeField] float rotationSpeed = 5;
    [SerializeField] float flashSpeed = 0.5f;
    [SerializeField] int numOfFlashes = 3;

    [SerializeField] SpriteRenderer netHoop;
    [SerializeField] SkinnedMeshRenderer netMesh;

    public Sprite[] jarImages; // 0 = not cracked, 1 = a little crack, 2 = halfway, 3 = broken

    bool boosting;

	// Use this for initialization
	void Start () {
        boosting = false;
	}
	
	// Update is called once per frame
	void Update () {
        // Rotate Counter Clockwise
        if (Input.GetKey(KeyCode.A)) {
            this.transform.Rotate(Vector3.forward * rotationSpeed);
        }
        // Roate Clockwise
        if (Input.GetKey(KeyCode.D)){
            this.transform.Rotate(-Vector3.forward * rotationSpeed);
        }

        if (Input.GetKeyDown(KeyCode.W)) {
            this.GetComponent<Rigidbody2D>().AddForce(transform.up * boostForce, ForceMode2D.Impulse);
        }

        // Debug.Log("Velocity: " + this.GetComponent<Rigidbody2D>().velocity);
        boosting = this.GetComponent<Rigidbody2D>().velocity.y > 0.01f ? true : false;
    }

    public bool Boosting() {
        if (this.GetComponent<Rigidbody2D>().velocity.y > 0.01f)
            return true;
        else return false;
    }
    /*
    public void ChangeSprite(int val) {
        jarImage.GetComponent<SpriteRenderer>().sprite = jarImages[val];
        jarImage.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        jarImage.transform.localRotation = Quaternion.identity;
    }
    */

    /*public void startFlashJar()
    {
    	StartCoroutine(FlashJar());
    }
    */
    /*
    IEnumerator FlashJar ()
	{
		Debug.Log("Start Flash");
		Color tempCol = netHoop.GetComponent<SpriteRenderer>().color;

		Color tempMatCol = netHoop.material.color;

		for (int i = 0; i < numOfFlashes; i++) 
		{
			yield return StartCoroutine(alphaDown(tempCol, tempMatCol));	
			yield return StartCoroutine(alphaUp(tempCol, tempMatCol));

			tempCol.a = 1.0f;
			netHoop.GetComponent<SpriteRenderer>().color = tempCol;

			tempMatCol.a = 1.0f;
			netMesh.material.color = tempMatCol;

			yield return null;	
		}

    	yield return null;


        jarImage.GetComponent<SpriteRenderer>().color = Color.clear;
        yield return new WaitForSecondsRealtime(0.3f);
        jarImage.GetComponent<SpriteRenderer>().color = Color.white;
        yield return new WaitForSecondsRealtime(0.3f);
        jarImage.GetComponent<SpriteRenderer>().color = Color.clear;
        yield return new WaitForSecondsRealtime(0.3f);
        jarImage.GetComponent<SpriteRenderer>().color = Color.white;
        yield return new WaitForSecondsRealtime(0.3f);
        jarImage.GetComponent<SpriteRenderer>().color = Color.clear;
        yield return new WaitForSecondsRealtime(0.3f);
        jarImage.GetComponent<SpriteRenderer>().color = Color.white;
        yield return new WaitForSecondsRealtime(0.3f);

    }
    */

    IEnumerator alphaDown (Color temp, Color tempMatCol)
	{

		while(temp.a > 0.05f)
    	{
    		temp.a = Mathf.MoveTowards(temp.a, 0.0f, Time.deltaTime * flashSpeed);
    		netHoop.GetComponent<SpriteRenderer>().color = temp;

			tempMatCol.a = Mathf.MoveTowards(tempMatCol.a, 0.0f, Time.deltaTime * flashSpeed);
			netMesh.material.color = new Color(tempMatCol.r, tempMatCol.g, tempMatCol.b, tempMatCol.a);

    		yield return null;
    	}
    	yield return null;
	}

	IEnumerator alphaUp(Color temp, Color tempMatCol)
	{
		while(temp.a < 0.95f)
    	{
    		temp.a = Mathf.MoveTowards(temp.a, 1.0f, Time.deltaTime * flashSpeed);
    		netHoop.GetComponent<SpriteRenderer>().color = temp;

			tempMatCol.a = Mathf.MoveTowards(tempMatCol.a, 1.0f, Time.deltaTime * flashSpeed);
			netMesh.material.color = new Color(tempMatCol.r, tempMatCol.g, tempMatCol.b, tempMatCol.a);

    		yield return null;
    	}
    	yield return null;
	}
}
