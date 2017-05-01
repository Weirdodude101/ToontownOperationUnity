using UnityEngine;

public class RuntimeMeshBatcherController : MonoBehaviour
{
	
	public bool processObjectsByLayer;
	public bool processObjectsByTag;
	public int rmbLayer;
	public string rmbTag;

	public bool destroyOriginalObjects;
	public bool keepOriginalObjectReferences;

	public bool combineByGrid;
	public MeshBatcherGridType gridType;
	public float gridSize;

	public bool autoRun;

	public static RuntimeMeshBatcherController instance { get; set; }

	protected void Awake()
	{
		instance = this;
		RuntimeMeshBatcher.rmbContainerGameObject = gameObject;
	}

	protected void Start()
	{
		if (autoRun) CombineMeshes();
	}

	public GameObject CombineMeshes()
	{
		//var startCount = Environment.TickCount;
		return RuntimeMeshBatcher.CombineMeshes(
			processObjectsByTag, rmbTag, processObjectsByLayer, rmbLayer, destroyOriginalObjects, keepOriginalObjectReferences, combineByGrid, gridType, gridSize);
		//var endCount = Environment.TickCount;
		//Debug.Log("Meshes combined in " + (endCount - startCount) + "ms");
	}

	public void UncombineMeshes(GameObject rmbParent)
	{
		RuntimeMeshBatcher.UncombineMeshes(rmbParent);
	}

	public GameObject CombineMeshes (GameObject[] objectsToBatch)
	{
		return RuntimeMeshBatcher.CombineMeshes ( objectsToBatch, 
												 destroyOriginalObjects,
		                                         keepOriginalObjectReferences, 
		                                         combineByGrid,
		                                         gridType,
		                                         gridSize );
	}
}