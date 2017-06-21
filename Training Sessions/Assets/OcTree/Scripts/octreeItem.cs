using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class octreeItem : MonoBehaviour {

	public List<octreeNode> ownerNodes = new List<octreeNode> ();
	private Vector3 prevPos;

	// Use this for initialization
	void Start () 
	{
		prevPos = transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if (transform.position != prevPos) 
		{
			//REORDENA EL ARBOL

			prevPos = transform.position;
		}
	}
}
