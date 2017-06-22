using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class octreeNode
{
	//Variables-------------------------------------------------------------------------------
	public static int maxNumberOfElements = 1;
	public static float minimumSize = 0.5f; //Find out how to do it for polygons-*
	private static octreeNode _root;

	private List<octreeItem> nodeElements = new List<octreeItem>();
	private octreeNode[] _children = new octreeNode[8];
	private float halfSpaceLength;
	private Vector3 centerSpace;
	private octreeNode parent;

	//PROPERTIES------------------------------------------------------------------------------
	public float HalfSpaceLength { get { return halfSpaceLength; } }
	public Vector3 CenterSpace { get { return centerSpace; } }
	public octreeNode[] children {get { return _children; } }
	public static octreeNode root
	{
		get
		{
			if (_root == null) 
			{
				//Mas adelante el octree podria tener un centro distinto de origen:
				_root = new octreeNode (null, new List<octreeItem>(), 15f, Vector3.zero);
			}
			return _root;
		}
	}


	//Methods---------------------------------------------------------------------------------

	//Runtime method has to be static since it is called before start (no instances exist yet)-
	[RuntimeInitializeOnLoadMethod]
	static bool init()
	{
		return root == null;
	}


	//Constructor-
	public octreeNode(octreeNode parent, List<octreeItem> potentialItems, float halfLength, Vector3 centerPos)
	{
		this.parent = parent;
		this.centerSpace = centerPos;
		this.halfSpaceLength = halfLength;

		for (int i  = 0; i < potentialItems.Count; i++) 
		{
			processItem (potentialItems[i]);
		}

		//Graphic representation of the space-
		drawNodeSpace();
	}


	//Method that evaluates if the item belongs to this node, or if it belongs to a child-
	public bool processItem(octreeItem item)
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

		//If it does not belong to the octree, it gets deleted.
		//GameObject.Destroy (item.gameObject); //ERROR-*
		return false;
	}


	//Pushes an item into the node. If the node reaches the limit amount, then it splits and reassigns the elements-
	private void push(octreeItem item)
	{
		if (!nodeElements.Contains (item)) 
		{
			nodeElements.Add (item);
		}

		if (nodeElements.Count > maxNumberOfElements) 
		{
			split ();

			nodeElements.Clear ();
		}
	}


	//Method that creates the 8 children of this node. After creating them, it empties its own List of items-
	private void split()
	{
		float half = halfSpaceLength / 2f;

		if (half >= minimumSize) 
		{
			for (int y = 0; y < 2; y++) 
			{
				for (int z = 0; z < 2; z++) 
				{
					for (int x = 0; x < 2; x++) 
					{
						Vector3 childCenter = new Vector3 (centerSpace.x + half * Mathf.Pow (-1f, x), 
										                      centerSpace.y + half * Mathf.Pow (-1f, y), 
										                      centerSpace.z + half * Mathf.Pow (-1f, z));
						_children [4 * x + 2 * y + z] = new octreeNode (this, nodeElements, half, childCenter);
					}
				}
			}
		} 
		else 
		{
			Debug.LogWarning ("Reached minimum node size in octree!");
		}
	}


	//Returns true if the vector pos is inside the bounds of this node-
	private bool containsItem(Vector3 pos)
	{
		return 	(pos.x < (centerSpace.x + halfSpaceLength) && pos.x > (centerSpace.x - halfSpaceLength)) && 
				(pos.y < (centerSpace.y + halfSpaceLength) && pos.y > (centerSpace.y - halfSpaceLength)) && 
				(pos.z < (centerSpace.z + halfSpaceLength) && pos.z > (centerSpace.z - halfSpaceLength));
	}


	//Debug method for visualizing-
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
	}
}
