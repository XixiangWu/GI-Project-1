using UnityEngine;
using System.Collections;

public class Sun : MonoBehaviour {

	public Color color;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround (new Vector3 (0, 500, 0), Vector3.right, 5.0f * Time.deltaTime);
	}

	public Vector3 GetPosition()
	{
		return this.transform.position;
	}

	public Color GetColor() {
		return this.color;
	}
}
