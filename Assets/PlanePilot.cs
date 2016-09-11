using UnityEngine;
using System.Collections;

public class PlanePilot : MonoBehaviour {

	public float speed;
	Rigidbody rb;

	// Use this for initialization
	void Start () {

		// lock and hide the cursor
		SetCursorState();

		// camera rigidbody
		gameObject.GetComponent<Collider> ();
		rb = GetComponent <Rigidbody>();
	}

	// Hide cursor when locking
	void SetCursorState ()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	// Update is called once per frame
	void Update () {

		// key detection
		if (Input.GetKey ("w")) {
			rb.MovePosition (transform.position + transform.forward * Time.deltaTime * 40.0f);
		}

		if (Input.GetKey ("s")) {
			rb.MovePosition (transform.position + transform.forward * Time.deltaTime * -30.0f);
		}

		if (Input.GetKey ("a")) {
			transform.Rotate (Vector3.back * Time.deltaTime * -35.0f);
		}

		if (Input.GetKey ("d")) {
			transform.Rotate (Vector3.back * Time.deltaTime * 35.0f);
		}

		if (Input.GetKey ("q")) {
			transform.Rotate (Vector3.up * Time.deltaTime * -35.0f);
		}
		if (Input.GetKey ("e")) {
			transform.Rotate (Vector3.up * Time.deltaTime * 35.0f);
		}

		// mouse movement detection
		float h = 0.75f * Input.GetAxis("Mouse X") ;
		float v = -0.75f * Input.GetAxis("Mouse Y");
		transform.Rotate(v, h, 0);


	}
}
