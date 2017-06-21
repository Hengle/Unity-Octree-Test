using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class octreeNode
{
	public static int maxNumberOfElements = 1;
	private static octreeNode _root;
	public static octreeNode root
	{
		get
		{
			if (_root == null) 
			{
				_root = new octreeNode (null, new List<octreeItem>(), 15f, Vector3.zero);
			}
			return _root;
		}
	}

	private octreeNode parent;
	private List<octreeItem> nodeElements = new List<octreeItem>();

	public float halfSpaceLength;
	public Vector3 centerSpace;

	private octreeNode[] _children = new octreeNode[8];
	public octreeNode[] children
	{
		get { return _children; }
	}


	[RuntimeInitializeOnLoadMethod]
	static bool init()
	{
		return root == null;
	}


	//Bob el contructor:
	public octreeNode(octreeNode parent, List<octreeItem> potentialItems, float halfLength, Vector3 centerPos)
	{
		this.parent = parent;

		for (int i  = 0; i < potentialItems.Count; i++) 
		{
			processItem (potentialItems[i]);
		}

		this.centerSpace = centerPos;
		this.halfSpaceLength = halfLength;

		//Graphic representation of the space:
		drawNodeSpace();
	}


	//TODO : Metodo que evalua si el item corresponde que pertenezca o no a este nodo.
	private bool processItem(octreeItem item)
	{
		if (containsItem (item.transform.position)) 
		{
			if (ReferenceEquals (_children[0], null)) 
			{
				push (item);

				return true;
			} 
			else 
			{
				for (int i = 0; i < 8; i++) 
				{
					if (_children [i].processItem (item))
						return true;
				}
			}
		}

		return false;
	}


	private void push(octreeItem item)
	{
		if (!nodeElements.Contains (item)) 
		{
			nodeElements.Add (item);
		}

		if (nodeElements.Count >= maxNumberOfElements) 
		{
			split ();
		}
	}


	private void split()
	{
		float half = halfSpaceLength / 2f;

		for(int y = 0; y < 2; y++)
		{
			for(int z = 0; z < 2; z++)
			{
				for(int x = 0; x < 2; x++)
				{
					Vector3 childCenter = new Vector3 (	centerSpace.x + half * Mathf.Pow(-1f, x), 
														centerSpace.y + half * Mathf.Pow(-1f, y), 
														centerSpace.z + half * Mathf.Pow(-1f, z));
					_children[4 * x + 2 * y + z] = new octreeNode (this, containsItem, half, childCenter);
				}
			}
		}
	}


	private bool containsItem(Vector3 pos)
	{
		return 	(pos.x < (centerSpace.x + halfSpaceLength) || pos.x > (centerSpace.x - halfSpaceLength)) && 
				(pos.y < (centerSpace.y + halfSpaceLength) || pos.y > (centerSpace.y - halfSpaceLength)) && 
				(pos.z < (centerSpace.z + halfSpaceLength) || pos.z > (centerSpace.z - halfSpaceLength));
	}


	//Debug method for visualizing:
	private void drawNodeSpace()
	{
		GameObject obj = new GameObject ();
		obj.hideFlags = HideFlags.HideInHierarchy;
		LineRenderer line = obj.AddComponent<LineRenderer> ();
		line.useWorldSpace = true;
		line.SetWidth (0.2f, 0.2f);
		line.SetVertexCount (16);

		Vector3[] vertices = new Vector3[8];

		for(int y = 0; y < 2; y++)
		{
			for(int z = 0; z < 2; z++)
			{
				for(int x = 0; x < 2; x++)
				{
					Vector3 gen = new Vector3 (	centerSpace.x + halfSpaceLength * Mathf.Pow(-1f, x), 
												centerSpace.y + halfSpaceLength * Mathf.Pow(-1f, y), 
												centerSpace.z + halfSpaceLength * Mathf.Pow(-1f, z));
					vertices[4 * x + 2 * y + z] = gen;
				}
			}
		}

		line.SetPosition (0, vertices[0]);
		line.SetPosition (1, vertices[1]);
		line.SetPosition (2, vertices[3]);
		line.SetPosition (3, vertices[2]);
		line.SetPosition (4, vertices[0]);
		line.SetPosition (5, vertices[4]);
		line.SetPosition (6, vertices[5]);
		line.SetPosition (7, vertices[1]);
		line.SetPosition (8, vertices[5]);
		line.SetPosition (9, vertices[7]);
		line.SetPosition (10, vertices[3]);
		line.SetPosition (11, vertices[7]);
		line.SetPosition (12, vertices[6]);
		line.SetPosition (13, vertices[2]);
		line.SetPosition (14, vertices[6]);
		line.SetPosition (15, vertices[4]);

		Debug.Log ("All Drawn");
	}
}
