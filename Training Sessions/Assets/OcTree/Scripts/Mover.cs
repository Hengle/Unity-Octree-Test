using UnityEngine;

public class Mover : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.Translate (Input.GetAxis ("Horizontal") * Time.deltaTime * 15f, 0f, Input.GetAxis ("Vertical") * Time.deltaTime * 15f, Space.Self);
		transform.Rotate (0f, Input.GetAxis ("Mouse X") * Time.deltaTime * 60f, 0f, Space.World);
		transform.Rotate (-Input.GetAxis ("Mouse Y") * Time.deltaTime * 60f, 0f, 0f, Space.Self);

		if (Input.GetKeyDown (KeyCode.Mouse0)) 
		{
			//Agregar pooling
			GameObject newCube = GameObject.Instantiate(Resources.Load("Cube"));
		}
	}
}
