using System.IO;
using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections.Generic;

public class MCWindow : EditorWindow
{
	[MenuItem ("MovingCo/Open Window")]
	static void Init () 
	{
		// Get existing open window or if none, make a new one:
		MCWindow window = (MCWindow)EditorWindow.GetWindow (typeof (MCWindow));
		window.position = new Rect(100,100,300,500);
		window.title = "MCWindow";
		window.Show(); 
	} 
	
	public void OnGUI()
	{
		EditorGUILayout.BeginHorizontal();
//		if(GUILayout.Button("Toggle Hat Tool"))
//		{
//			if(Core.instance != null)
//			{
//				HatTool.Show();
//			}
//		}
		EditorGUILayout.EndHorizontal();
		
	} 

	[MenuItem ("MovingCo/Build for iOS")]
	static void BuildForIOS() 
	{
		BuildOptions options = BuildOptions.None;
		options |= BuildOptions.AcceptExternalModificationsToPlayer;
		options |= BuildOptions.SymlinkLibraries;
		UnityEditor.BuildPipeline.BuildPlayer(new string[] {"Assets/MovingCo.unity"},"Export/MovingCo_iOS",BuildTarget.iPhone,options);
	}
	
	[MenuItem ("MovingCo/Build for Web")]
	static void BuildForWeb() 
	{
		BuildOptions options = BuildOptions.None;
		//options |= BuildOptions.AcceptExternalModificationsToPlayer;
		//options |= BuildOptions.SymlinkLibraries;
		UnityEditor.BuildPipeline.BuildPlayer(new string[] {"Assets/MovingCo.unity"},"Export/MovingCo_Web",BuildTarget.WebPlayer,options);
	}
} 

