using UnityEngine;
using System.Collections;

public class Jar : MonoBehaviour {

    [SerializeField] float boostForce;
    [SerializeField] float rotationSpeed = 5;

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
}
