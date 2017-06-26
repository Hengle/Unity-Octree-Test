//using System.Collections.Generic;
//using System.Collections;
using UnityEngine;

public class Mover : MonoBehaviour 
{
	private GameObject child;
	private Material recentCubeMaterial;


	// Update is called once per frame
	void Update () 
	{
		transform.Translate (Input.GetAxis ("Horizontal") * Time.deltaTime * 15f, 0f, Input.GetAxis ("Vertical") * Time.deltaTime * 15f, Space.Self);

		if (Input.GetKey (KeyCode.Mouse0)) 
		{
			transform.Rotate (0f, -Input.GetAxis ("Mouse X") * Time.deltaTime * 100f, 0f, Space.World);
			transform.Rotate (Input.GetAxis ("Mouse Y") * Time.deltaTime * 100f, 0f, 0f, Space.Self);
		}

		//Create a new cube item for the octree node to evaluate:
		if (Input.GetKeyDown (KeyCode.Tab)) 
		{
			GameObject newCube = GameObject.Instantiate(Resources.Load("Cube")) as GameObject;
			newCube.transform.position = transform.position + 10 * transform.forward;
			newCube.GetComponent<octreeItem> ().addToRoot ();
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
				if (Input.GetKeyDown (KeyCode.Q)) 
				{
					seen.transform.SetParent (transform);
					child = seen;
					child.GetComponent<Rigidbody> ().isKinematic = true; //DECOMMENT

					Debug.Log(	"Item's name is : " 	+ child.GetComponent<octreeItem> ().name +
								"\nOwner's name is : " 	+ child.GetComponent<octreeItem> ().ownerNodes[0].name +
								"\nOwner has: " 		+ child.GetComponent<octreeItem> ().ownerNodes[0].nodeElements.Count + " elements." + 
								"\nRoot has children: " + (!ReferenceEquals(octreeNode.root.children[0], null)).ToString());
				} 

				if (Input.GetKeyUp (KeyCode.Q)) 
				{
					if (child != null) 
					{
						child.transform.SetParent (null);
						child.GetComponent<Rigidbody> ().isKinematic = false; //DECOMMENT
						child = null;
					}
				}

				if (Input.GetKeyUp (KeyCode.Mouse1)) 
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

		if (Input.GetKeyDown (KeyCode.Escape)) 
		{
			Application.Quit ();
		}
	}
}
