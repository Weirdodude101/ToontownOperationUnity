/****************************************
	Simple Mesh Combine v1.5		
	Copyright 2015 Unluck Software	
 	www.chemicalbliss.com
 	
 	Change Log
 		v1.1
 		Added naming and prefab save option	
 		
 		v1.2
 		Added lightmap support		
 		
 		v1.3
 		Added multiple material support
 			v1.301
 			Fixed compile error trying to unwrap UV in game mode	
 		
 		v1.4
 		Added C# scripts
 		
 		v1.41 - 22.01.2015
 		Changed from using SharedMaterial.Name to SharedMaterial directly to identify different materials
 		Fixed error when combining meshes with more submeshes than materials
 		
 		v1.5 -24.01.2015
 		Improved editor layout, added more info and tips
 		Lightmap option as own function
 		Now sets UV2 to null to reduce mesh size
*****************************************/
import System.IO;
@CustomEditor(SimpleMeshCombine)
public class SimpleMeshCombineEditor extends Editor {
	var tex:Texture = Resources.Load("SMC_Title");
			
    override function OnInspectorGUI () {
    	var infoColor: Color = Color32(216, 255, 0, 255);
		var dColor: Color = Color32(175, 175, 175, 255);
		var aColor: Color = Color.white;	
		var buttonStyle = new GUIStyle(GUI.skin.button);
		var buttonStyle2 = new GUIStyle(GUI.skin.button);
		var titleStyle = new GUIStyle(GUI.skin.label);
    	buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fixedWidth = 150;
		buttonStyle.fixedHeight = 35;
		buttonStyle.fontSize = 15;
		buttonStyle2.fixedWidth = 200;
		buttonStyle2.fixedHeight = 25;
		buttonStyle2.margin = RectOffset((Screen.width-200)*.5, (Screen.width-200)*.5, 0, 0);
		buttonStyle.margin = RectOffset((Screen.width-150)*.5, (Screen.width-150)*.5, 0, 0);
		titleStyle.fixedWidth = 256;
		titleStyle.fixedHeight = 64;
		titleStyle.margin = RectOffset((Screen.width-256)*.5, (Screen.width-256)*.5, 0, 0);  		
   		var infoStyle = new GUIStyle(GUI.skin.label);
   		infoStyle.fontSize = 10;
   		infoStyle.margin.top = 0;
   		infoStyle.margin.bottom = 0;
   		
   		GUILayout.Label(tex,titleStyle);
		
		GUI.color = dColor;
		
		
		/*-------------------------------------
		//	COMBINE MESH AREA
		//-------------------------------------*/
		
		EditorGUILayout.BeginVertical("Box");
		GUI.color = Color.white;

		if(!target.combined){
		
			GUI.color = infoColor;

			if(GUILayout.Button("Combine", buttonStyle)) {
				if(target.transform.childCount > 1) target.CombineMeshes();
			}
			   
			GUI.color = Color.white;

			GUILayout.Label("Vertex count: - / 65536",infoStyle);
			GUILayout.Label("Material count: -",infoStyle);      	
       }else{
       	
			GUI.color = infoColor;

			if(GUILayout.Button("Release", buttonStyle)) {
				target.EnableRenderers(true);
				target._savedPrefab = false;
				if(target.combined)
					DestroyImmediate(target.combined);
			}		
			if(target.combined){
				GUI.color = Color.white;
				GUILayout.Label("Vertex count: " + target.vCount + " / 65536",infoStyle);
        		GUILayout.Label("Material count: " + target.combined.renderer.sharedMaterials.length,infoStyle);
        	}
        }
        
		EditorGUILayout.EndVertical();
        
        if(target.combined && !target._savedPrefab){       	
        	if(!target._canGenerateLightmapUV) {
        		GUILayout.Label("Warning: Mesh has too high vertex count",EditorStyles.boldLabel);
        		GUI.enabled = false;
        		
        	}
        	
			GUI.color = dColor;
	
	
			/*-------------------------------------
			//	LIGHTMAP AREA
			//-------------------------------------*/
	
			EditorGUILayout.BeginVertical("Box");
			GUI.color = Color.white;
			GUI.color = infoColor;

        	if(GUILayout.Button("Generate Lightmap UV's", buttonStyle2)) {
				target.GenerateLightmapUV();
			}
			
			GUI.color = Color.white;
			GUILayout.Label("Warning: Overlapping meshes might freeze Unity", infoStyle);
			EditorGUILayout.EndVertical();		
			GUI.color = dColor;
			
			
			/*-------------------------------------
			//	SAVE MESH AREA
			//-------------------------------------*/
			
			EditorGUILayout.BeginVertical("Box");
			GUI.color = Color.white;	
			GUI.color = infoColor;

        	if(GUILayout.Button("Save Mesh", buttonStyle2)) {
        		var n:String = target.meshName;
        		if(System.IO.Directory.Exists("Assets/Simple Mesh Combine/Saved Meshes/")){
        		if(!System.IO.File.Exists("Assets/Simple Mesh Combine/Saved Meshes/"+target.meshName+".asset")){     	
        			AssetDatabase.CreateAsset(target.combined.GetComponent(MeshFilter).sharedMesh, "Assets/Simple Mesh Combine/Saved Meshes/"+n+".asset");
        			target._savedPrefab = true;
        			Debug.Log("Saved Assets/Simple Mesh Combine/Saved Meshes/"+n+".asset");
        		}else{
        			Debug.Log(target.meshName+".asset" + " already exists, please change the name");
        		}
        		
        		}else{
        			Debug.Log("Missing Folder: Assets/Simple Mesh Combine/Saved Meshes/");
        		}
        	}
	
			GUI.color = Color.white;	

        	target.meshName = GUILayout.TextField(target.meshName);
        	     
			EditorGUILayout.EndVertical();
		
        }
        GUILayout.Label("For best result use as few materials as possible",infoStyle);
        GUILayout.Label("Reduce scene size by saving meshes",infoStyle);
        if (GUI.changed){
                EditorUtility.SetDirty(target);
		}
	}
}