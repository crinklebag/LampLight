using UnityEngine;
using System.Collections;

public class Jar : MonoBehaviour {

    [SerializeField] float boostForce;
    [SerializeField] float rotationSpeed = 5;
    [SerializeField] GameObject jarImage;

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

    public void ChangeSprite(int val) {
        jarImage.GetComponent<SpriteRenderer>().sprite = jarImages[val];
        jarImage.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        jarImage.transform.localRotation = Quaternion.identity;
    }

    public IEnumerator FlashJar()
    {
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
}
