using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class octreeNode
{
	//Variables-------------------------------------------------------------------------------
	public static int maxNumberOfElements = 1;
	public static float minimumSize = 1f; //Find out how to do it for polygons-*
	private static octreeNode _root;

	public List<octreeItem> nodeElements = new List<octreeItem>();
	private octreeNode[] _children = new octreeNode[8];
	private float halfSpaceLength;
	private Vector3 centerSpace;
	private octreeNode parent;

	//Borrar despues, solo para efectos de debugging:
	public static int id = 0;
	public string name;

	//Visual.
	private LineRenderer line;
	private GameObject obj = new GameObject ();

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
				_root = new octreeNode (null, new List<octreeItem>(), 16f, Vector3.zero, "Root");
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
	public octreeNode(octreeNode parent, List<octreeItem> potentialItems, float halfLength, Vector3 centerPos, string name)
	{
		this.parent = parent;
		this.centerSpace = centerPos;
		this.halfSpaceLength = halfLength;
		this.name = name + "(" + id++ + ")"; //Only for debugging-

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
				pushItem (item);

				return true;
			} 
			else 
			{
				for (int i = 0; i < 8; i++) 
				{
					if (_children [i].processItem (item))//***************************************************************************************************************************
						return true;
				}
			}
		}

		//If it does not belong to the octree, it gets deleted.
		//GameObject.Destroy (item.gameObject); //ERROR-*
		return false;
	}


	//Alternative to push method proposed below:
	private void pushItem(octreeItem item)
	{
		//If the item belongs to this node's elements, then it will not be evaluated:
		if (!nodeElements.Contains (item))
		{
			//If the item has an owner (meaning he is being modified, not created):
			if (item.ownerNodes.Count > 0)
			{
				//If the item's owner is different from this node (caused by items moved or node split)
				if (!ReferenceEquals (this, item.ownerNodes [0]))
				{
					//Debug.LogWarning ("CAMBIO DE DUEÑO");

					//You get the previous owner reference:
					octreeNode prevOwner = item.ownerNodes [0];
					prevOwner.nodeElements.Remove (item);
					item.ownerNodes [0] = this;
					this.nodeElements.Add (item);

					Debug.Log ("Stats when changing ownership;" +
						"\nprevOwnerNodeElementsCount = " + prevOwner.nodeElements.Count +
						"\nThisOwnerNodeElementsCount = " + nodeElements.Count);

					//If the previous owner was not root, then you have to check wether his parent should merge his children back together:
					if (prevOwner.parent != null)
					{
						//We get the parent node of the previous owner, which might get terminated:
						octreeNode prevOwnerParent = prevOwner.parent;

						List<octreeItem> tempElementsList = new List<octreeItem> ();
						int totalCount = 0;
						for (int i = 0; i < 8; i++) 
						{
							octreeNode child = prevOwnerParent.children [i];

							if (child == null)
							{
								Debug.LogError ("ERROR; CHILD ES NULL. INFORMACION: " +
									"\nprevOwner name = " + prevOwner.name + 
									"\nPrevOwner parent's name = " + prevOwnerParent.name + 
									"\nIndex of null children of said parent = " + i );
							}

							Debug.LogWarning ("Informacion de cada child de el padre del antiguo owner al cambiar de Owner: " +
												"\nchild name = " + child.name +
												"\nchild's number of elements = " + child.nodeElements.Count + 
												"\nchild's father = " + prevOwnerParent.name);

							totalCount += child.nodeElements.Count;//**********************************************************************************************************************
							tempElementsList.AddRange (child.nodeElements); 
						}

						/*
						if (totalCount <= maxNumberOfElements)
						{

							//*
							if (prevOwnerParent.name == "Root(0)")
								Debug.LogError ("ESTA BORRANDO LOS HIJOS DE ROOT!!!" +
									"\nTotal count = " + totalCount +
									"\nMaxNumberOfElements = " + maxNumberOfElements);
							//*

							for (int i = 0; i < 8; i++)
							{
								GameObject.Destroy (prevOwnerParent.children [i].obj);//**********************************########################3
								prevOwnerParent.children [i] = null;
							}

							//*
							//Every item which pointed to these destroyed child nodes must now point to their parent (new owner):
							for (int i = 0; i < tempElementsList.Count; i++)
							{
								tempElementsList [i].ownerNodes [0] = prevOwnerParent;
								prevOwnerParent.nodeElements.Add (tempElementsList[i]);
							}
							//*/

							//prevOwnerParent.nodeElements.AddRange (tempElementsList);
						//}
						//*/
					} 
				}
			}
			//If the item doesn't have an owner, he must be entering the octree for the first time:
			else
			{
				item.ownerNodes.Add (this);
				this.nodeElements.Add (item);
			}
		}

		//If after adding the element the node is overloaded, it has to split and then empty his list:
		if (nodeElements.Count > maxNumberOfElements) 
		{

			if (this.name == "node(1)")
				Debug.LogWarning ("Node 1 is being split" + 
					"It contains " + nodeElements.Count + " elements." + 
					"\nBefore the split, root has children: "  + (!ReferenceEquals(root.children[0], null)).ToString() );

			split ();

			nodeElements.Clear ();

			if (this.children [0] != null)
				Debug.LogWarning ("It created the 8 children and referenced them allright" + 
					"\nAfter the split, root has children: "  + (!ReferenceEquals(root.children[0], null)).ToString() );
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
						
						//_children [4 * x + 2 * y + z] = new octreeNode (this, nodeElements, half, childCenter, "node");
						_children [4 * y + 2 * z + x] = new octreeNode (this, nodeElements, half, childCenter, "node");
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
		return 	(pos.x < (centerSpace.x + halfSpaceLength) && pos.x >= (centerSpace.x - halfSpaceLength)) && 
				(pos.y < (centerSpace.y + halfSpaceLength) && pos.y >= (centerSpace.y - halfSpaceLength)) && 
				(pos.z < (centerSpace.z + halfSpaceLength) && pos.z >= (centerSpace.z - halfSpaceLength));
	}


	//Debug method for visualizing-
	private void drawNodeSpace()
	{
		obj.hideFlags = HideFlags.HideInHierarchy;
		line = obj.AddComponent<LineRenderer> ();
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
