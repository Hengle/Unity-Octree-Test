using UnityEngine;

public class Mover : MonoBehaviour 
{
	private GameObject child;
	private Material recentCubeMaterial;

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
			//Agregar pooling***
			GameObject newCube = GameObject.Instantiate(Resources.Load("Cube")) as GameObject;
			newCube.transform.position = transform.position + 10 * transform.forward;
		}

		RaycastHit hit = new RaycastHit ();
		if (Physics.Raycast (transform.position, transform.forward, out hit, 100f)) 
		{
			if (hit.collider.gameObject.tag == "OctCube") 
			{
				if (recentCubeMaterial)
					recentCubeMaterial.color = Color.white;
				
				GameObject seen = hit.collider.gameObject;

				recentCubeMaterial = seen.GetComponent<MeshRenderer> ().material;
				recentCubeMaterial.color = Color.cyan;

				//INPUT-CODE-------------------------------------------------------***
				if (Input.GetKeyDown (KeyCode.Mouse1)) 
				{
					seen.transform.SetParent (transform);
					child = seen;
					child.GetComponent<Rigidbody> ().isKinematic = true;
				} 

				if (Input.GetKeyUp (KeyCode.Mouse1)) 
				{
					if (child != null) 
					{
						child.transform.SetParent (null);
						child.GetComponent<Rigidbody> ().isKinematic = false;
						child = null;
					}
				}

				if (Input.GetKeyUp (KeyCode.E)) 
				{
					Destroy (seen);
				}

				if (Input.GetKeyUp (KeyCode.Return)) 
				{
					seen.GetComponent<Rigidbody> ().AddForce (transform.forward * 50f);
				}
			}
		} 
		else 
		{
			if (recentCubeMaterial)
				recentCubeMaterial.color = Color.white;
		}

		if (Input.GetKeyDown (KeyCode.Q)) 
		{
			Application.Quit ();
		}
	}
}
