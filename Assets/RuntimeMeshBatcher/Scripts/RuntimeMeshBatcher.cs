using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public enum MeshBatcherGridType
{
	Grid2D,
	Grid3D,
}

public static class RuntimeMeshBatcher
{
	public static List<GameObject> rmbParents;
	public static Dictionary<GameObject, GameObject[]> originalObjetReferences;
	public static GameObject rmbContainerGameObject;

	public static GameObject CombineMeshes(
		bool processObjectsByTag = false, 
		string rmbTag = null, 
		bool processObjectsByLayer = false, 
		int rmbLayer = 0,
		bool destroyOriginalObjects = false, 
		bool keepOriginalObjectReferences = false,
		bool combineByGrid = false,
		MeshBatcherGridType gridType = MeshBatcherGridType.Grid2D,
		float gridSize = 0)
	{

		// Build Objects List
		var objectsToBatch = new List<GameObject>();

		// Add objects by tag
		if (processObjectsByTag && rmbTag != null) objectsToBatch.AddRange(GameObject.FindGameObjectsWithTag(rmbTag));

		// Add objects by layer
		if (processObjectsByLayer)
		{
			GameObject[] allGameObjects = Object.FindObjectsOfType<GameObject>();
			for (int i = 0; i < allGameObjects.Length; i++)
			{
				GameObject go = allGameObjects[i];
				if (go.layer == rmbLayer) objectsToBatch.Add(go);
			}
		}

		return CombineMeshes(objectsToBatch.ToArray(), destroyOriginalObjects, keepOriginalObjectReferences, combineByGrid, gridType, gridSize);
	}

	public static GameObject CombineMeshes(
		GameObject[] objectsToBatch, bool destroyOriginalObjects = false, bool keepOriginalObjectReferences = false, bool combineByGrid = false,
		MeshBatcherGridType gridType = MeshBatcherGridType.Grid2D,
		float gridSize = 0)
	{
		if (objectsToBatch == null || objectsToBatch.Length == 0)
		{
			Debug.LogWarning("Runtime Mesh Batcher warning: no objects found to be combined.");
			return null;
		}
		GameObject combinedMeshParent;

		if (combineByGrid && gridSize <= 0)
		{
			Debug.LogWarning("Runtime Mesh Batcher warning: Grid Size must be superior to 0. Continuing batching without grid.");
			combineByGrid = false;
		}


		if (combineByGrid)
		{
			var objectsByCell = new Dictionary<string, List<GameObject>>();
			switch (gridType)
			{
				case MeshBatcherGridType.Grid2D:
					foreach (var gameObject in objectsToBatch)
					{
						var position = gameObject.transform.position;
						var xIndex = GetGridIndex(position.x, gridSize);
						var zIndex = GetGridIndex(position.z, gridSize);
						string key = xIndex + "_" + zIndex;
						if (!objectsByCell.ContainsKey(key))
						{
							objectsByCell[key] = new List<GameObject>();
						}
						objectsByCell[key].Add(gameObject);
					}
					break;
				case MeshBatcherGridType.Grid3D:
					foreach (var gameObject in objectsToBatch)
					{
						var position = gameObject.transform.position;
						var xIndex = GetGridIndex(position.x, gridSize);
						var yIndex = GetGridIndex(position.y, gridSize);
						var zIndex = GetGridIndex(position.z, gridSize);
						string key = xIndex + "_" + yIndex + "_" + zIndex;
						if (!objectsByCell.ContainsKey(key))
						{
							objectsByCell[key] = new List<GameObject>();
						}
						objectsByCell[key].Add(gameObject);
					}
					break;
			}
			combinedMeshParent = CreateParent();
			foreach (var gameObjects in objectsByCell.Values)
			{
				var combinedMesh = CreateStaticMesh(gameObjects.ToArray(), destroyOriginalObjects, true);
				SetParent(combinedMesh, combinedMeshParent);
			}
		}
		else
		{
			combinedMeshParent = CreateStaticMesh(objectsToBatch, destroyOriginalObjects);
		}
		if (keepOriginalObjectReferences)
			{
				if (originalObjetReferences == null) originalObjetReferences = new Dictionary<GameObject, GameObject[]>();
				originalObjetReferences[combinedMeshParent] = objectsToBatch;
			}

		return combinedMeshParent;
	}

	private static int GetGridIndex(float position, float gridSize)
	{
		return (int) Mathf.Floor(position/gridSize);
	}

	private static GameObject CreateParent()
	{
		if (rmbParents == null) rmbParents = new List<GameObject>();
		var rmbParent = new GameObject("RuntimeMeshBatcherParent_" + rmbParents.Count);
		rmbParents.Add(rmbParent);
		if (rmbContainerGameObject == null) rmbContainerGameObject = new GameObject("RuntimeMeshBatcherContainer");
		SetParent(rmbParent, rmbContainerGameObject);
		return rmbParent;
	}

	private static GameObject CreateStaticMesh(
		GameObject[] gameObjects, bool destroyOriginalObjects, bool combineByGrid = false)
	{
		GameObject combinedMeshParent = combineByGrid ? new GameObject("SubParent") : CreateParent();
		Transform combinedMeshParentTransform = combinedMeshParent.transform;
		var originalTransforms = new Dictionary<Transform, Transform>();
		foreach (GameObject gameObject in gameObjects)
		{
			if ( gameObject == null )
				continue;

			var renderers = gameObject.GetComponentsInChildren<Renderer>();
			if (renderers.Length == 0) continue;
			if (gameObject.GetComponentsInChildren<Renderer>()[0].isPartOfStaticBatch) continue;
			//gameObject.transform.SetParent(combinedMeshParent.transform, true);
			Transform goTransform = gameObject.transform;
			// Cache original transforms if objects are not to be destroyed
			if (!destroyOriginalObjects) originalTransforms[goTransform] = goTransform.parent;
			SetParent(goTransform, combinedMeshParentTransform);
		}

		Matrix4x4 myTransform = combinedMeshParentTransform.worldToLocalMatrix;
		var combineInstanceListsPerMaterial = new Dictionary<Material, List<List<CombineInstance>>>();
		MeshRenderer[] meshRenderers = combinedMeshParent.GetComponentsInChildren<MeshRenderer>();

		foreach (MeshRenderer meshRenderer in meshRenderers)
		{
			foreach (Material material in meshRenderer.sharedMaterials)
			{
				if (material != null && !combineInstanceListsPerMaterial.ContainsKey(material))
				{
					combineInstanceListsPerMaterial.Add(material, new List<List<CombineInstance>>());
				}
			}
		}

		var vertexCountPerMaterial = new Dictionary<Material, int>();

		MeshFilter[] meshFilters = combinedMeshParent.GetComponentsInChildren<MeshFilter>();
		foreach (MeshFilter filter in meshFilters)
		{
			if (filter.sharedMesh == null) continue;
			if (filter.sharedMesh.subMeshCount > 1)
			{
				var meshRenderer = filter.GetComponent<MeshRenderer>();
				for (int i = 0; i < meshRenderer.sharedMaterials.Count(); i++)
				{
					var ci = new CombineInstance { mesh = filter.sharedMesh, subMeshIndex = i, transform = myTransform * filter.transform.localToWorldMatrix };
					Material m = meshRenderer.sharedMaterials[i];
					int vertexCount = ci.mesh.vertexCount;
					if (!vertexCountPerMaterial.ContainsKey(m) || vertexCountPerMaterial[m] + vertexCount > 65536)
					{
						vertexCountPerMaterial[m] = 0;
						combineInstanceListsPerMaterial[m].Add(new List<CombineInstance>());
					}
					int ciListCount = combineInstanceListsPerMaterial[m].Count;
					List<CombineInstance> currentCiList = combineInstanceListsPerMaterial[m][ciListCount - 1];
					vertexCountPerMaterial[m] += vertexCount;
					currentCiList.Add(ci);									
				}
			}
			else
			{
				var ci = new CombineInstance { mesh = filter.sharedMesh, transform = myTransform * filter.transform.localToWorldMatrix };
				Material m = filter.GetComponent<Renderer>().sharedMaterial;
				int vertexCount = ci.mesh.vertexCount;
				if (!vertexCountPerMaterial.ContainsKey(m) || vertexCountPerMaterial[m] + vertexCount > 65536)
				{
					vertexCountPerMaterial[m] = 0;
					combineInstanceListsPerMaterial[m].Add(new List<CombineInstance>());
				}
				int ciListCount = combineInstanceListsPerMaterial[m].Count;
				List<CombineInstance> currentCiList = combineInstanceListsPerMaterial[m][ciListCount - 1];
				vertexCountPerMaterial[m] += vertexCount;
				currentCiList.Add(ci);				
			}
			filter.GetComponent<Renderer>().enabled = false;
		}

		foreach (Material m in combineInstanceListsPerMaterial.Keys)
		{
			for (int index = 0; index < combineInstanceListsPerMaterial[m].Count; index++)
			{
				List<CombineInstance> ciList = combineInstanceListsPerMaterial[m][index];
				var staticMesh = new GameObject("CombinedMesh_" + m.name + "_" + index);
				Transform staticMeshTransform = staticMesh.transform;
				SetParent(staticMeshTransform, combinedMeshParent.transform);
				staticMeshTransform.localPosition = Vector3.zero;
				staticMeshTransform.localRotation = Quaternion.identity;
				staticMeshTransform.localScale = Vector3.one;

				var filter = staticMesh.AddComponent<MeshFilter>();
				filter.mesh.CombineMeshes(ciList.ToArray(), true, true);

				var renderer = staticMesh.AddComponent<MeshRenderer>();
				renderer.material = m;
			}
		}

		if (destroyOriginalObjects) for (int i = 0; i < gameObjects.Length; i++) Object.Destroy(gameObjects[i]);
		else
		{
			foreach (MeshRenderer meshRenderer in meshRenderers) meshRenderer.enabled = false;

			foreach (Transform goTransform in gameObjects.Select(gameObject => gameObject.transform).Where(goTransform => originalTransforms.ContainsKey(goTransform))) {
				if ( goTransform != null && originalTransforms[goTransform] != null )
				SetParent(goTransform, originalTransforms[goTransform]);
			}
		}

		return combinedMeshParent;
	}

	public static void UncombineMeshes(GameObject rmbParent)
	{
		if (rmbParent == null)
		{
			Debug.LogWarning("Runtime Mesh Batcher warning: Calling UncombineMeshes with null parent GameObject.");
			return;
		}
		if (originalObjetReferences == null)
		{
			Debug.LogWarning("Runtime Mesh Batcher warning: Calling UncombineMeshes without having kept object references.");
			return;
		}
		if (!originalObjetReferences.ContainsKey(rmbParent))
		{
			Debug.LogWarning("Runtime Mesh Batcher warning: Calling UncombineMeshes with undefined parent GameObject.");
			return;
		}

		GameObject[] gameObjects = originalObjetReferences[rmbParent];
		Object.Destroy(rmbParent);

		foreach (GameObject gameObject in gameObjects)
		{
			if ( gameObject != null )
			{
			MeshRenderer[] meshRenderersOnObject = gameObject.GetComponents<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in meshRenderersOnObject) meshRenderer.enabled = true;
			MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in meshRenderers) meshRenderer.enabled = true;
		}
	}
	}

	private static void SetParent(GameObject gameObject, GameObject parent)
	{
		SetParent(gameObject.transform, parent.transform);
	}

	private static void SetParent(Transform gameObjectTransform, Transform parentTransform)
	{
#if UNITY_4_5
		gameObjectTransform.parent = parentTransform;
#else
		gameObjectTransform.SetParent(parentTransform, true);
#endif

	}
}