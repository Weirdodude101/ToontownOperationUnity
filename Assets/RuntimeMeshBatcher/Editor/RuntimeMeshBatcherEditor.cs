using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RuntimeMeshBatcherController))]
public class RuntimeMeshBatcherEditor : Editor
{
	private Texture2D _logo;
    private SerializedProperty _processObjectsByTag;
    private SerializedProperty _rmbTag;
    private SerializedProperty _processObjectsByLayer;
    private SerializedProperty _rmbLayer;
	private SerializedProperty _destroyOriginalObjects;
	private SerializedProperty _keepOriginalObjectReferences;
	private SerializedProperty _combineByGrid;
	private SerializedProperty _gridType;
	private SerializedProperty _gridSize;
	private SerializedProperty _autoRun;

	protected void OnEnable()
	{
		_logo = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/RuntimeMeshBatcher/Editor/RMBLogo.psd", typeof(Texture2D));
	    _processObjectsByTag = serializedObject.FindProperty("processObjectsByTag");
	    _rmbTag = serializedObject.FindProperty("rmbTag");
	    _processObjectsByLayer = serializedObject.FindProperty("processObjectsByLayer");
	    _rmbLayer = serializedObject.FindProperty("rmbLayer");
		_destroyOriginalObjects = serializedObject.FindProperty("destroyOriginalObjects");
		_keepOriginalObjectReferences = serializedObject.FindProperty("keepOriginalObjectReferences");
		_combineByGrid = serializedObject.FindProperty("combineByGrid");
		_gridType = serializedObject.FindProperty("gridType");
		_gridSize = serializedObject.FindProperty("gridSize");
		_autoRun = serializedObject.FindProperty("autoRun");
	}


	public override void OnInspectorGUI()
	{
	    serializedObject.Update();

		GUILayout.Label(_logo);

		EditorGUILayout.PropertyField(_processObjectsByTag, new GUIContent("Batch by tag", "Enables automatic batching by tag"));

		if (_processObjectsByTag.boolValue)
		{
			EditorGUILayout.BeginHorizontal();
		    EditorGUILayout.PropertyField(_rmbTag, new GUIContent("  Tag", "Tag used to identify game objects to be batched"));
			EditorGUILayout.EndHorizontal();
		}

        EditorGUILayout.PropertyField(_processObjectsByLayer, new GUIContent("Batch by layer", "Enables automatic batching by layer"));

		if (_processObjectsByLayer.boolValue)
		{
			EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(_rmbLayer, new GUIContent("  Layer", "Layer used to identify game objects to be batched"));
            EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.PropertyField(_destroyOriginalObjects, new GUIContent("Destroy objects", "Destroy original objects"));
		if (_keepOriginalObjectReferences.boolValue && _destroyOriginalObjects.boolValue) _keepOriginalObjectReferences.boolValue = false;

		EditorGUILayout.PropertyField(_keepOriginalObjectReferences, new GUIContent("Keep references", "Keep original object references"));
		if (_keepOriginalObjectReferences.boolValue && _destroyOriginalObjects.boolValue) _destroyOriginalObjects.boolValue = false;

		EditorGUILayout.PropertyField(_combineByGrid, new GUIContent("Combine by grid", "Combine objects following a 2D or 3D grid"));
		if (_combineByGrid.boolValue)
		{
			EditorGUILayout.BeginVertical();
			EditorGUILayout.PropertyField(_gridType, new GUIContent("  Grid type", "2D or 3D grid"));
			EditorGUILayout.PropertyField(_gridSize, new GUIContent("  Grid size", "Size of the grid"));

			EditorGUILayout.EndVertical();
		}

		EditorGUILayout.PropertyField(_autoRun, new GUIContent("AutoRun", "Automatically combines meshes on startup"));

	    serializedObject.ApplyModifiedProperties();

	}
}