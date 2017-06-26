using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class octreeItem : MonoBehaviour 
{

	public List<octreeNode> ownerNodes = new List<octreeNode> ();
	private Vector3 prevPos;

	//Borrar despues, solo para efectos de debugging:
	public static int id = 1;
	public string name;

	// Use this for initialization
	void Start () 
	{
		prevPos = transform.position;

		name = "Item " + id++;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if (transform.position != prevPos) 
		{
			addToRoot (); //Later-

			prevPos = transform.position;
		}
	}

	public void addToRoot()
	{
		octreeNode.root.processItem (this);
	}
}
