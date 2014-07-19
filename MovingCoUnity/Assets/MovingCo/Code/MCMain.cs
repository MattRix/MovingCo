//#define ALLOW_HALF_SCALE_RESOLUTION

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MCMain : MonoBehaviour
{	
	static public MCMain instance = null;
	
	private void Start()
	{
		instance = this;

		Go.defaultEaseType = EaseType.Linear;
		Go.duplicatePropertyRule = DuplicatePropertyRuleType.RemoveRunningProperty;
		
		//Uncomment if you need to delete bad save data on startup
		//PlayerPrefs.DeleteAll();

		bool isIPad = SystemInfo.deviceModel.Contains("iPad");

		FutileParams fparams = new FutileParams(true,true,false,false);
		
		fparams.backgroundColor = RXUtils.GetColorFromHex(0x011020); //RXUtils.GetColorFromHex(0xEEEEEE); 
		fparams.shouldLerpToNearestResolutionLevel = true;

		fparams.resolutionLevelPickDimension = FResolutionLevelPickDimension.Longest;
		fparams.resolutionLevelPickMode = FResolutionLevelPickMode.Downwards;

		fparams.AddResolutionLevel(512.0f,		0.5f,	2.0f,	""); //Sim iPad
		fparams.AddResolutionLevel(1024.0f,		1.0f,	2.0f,	""); //iPad Normal
		fparams.AddResolutionLevel(2048.0f,		2.0f,	2.0f,	""); //iPad Retina

		fparams.origin = new Vector2(0.5f,0.5f);
		
		Futile.instance.Init (fparams);
		// Enable this to get ghosting in the framebuffer
		// NOTE: Only shows up on device, NOT in the editor
//		Futile.instance.camera.clearFlags = CameraClearFlags.Nothing;

//		FFacetType.Quad.maxEmptyAmount = 100;
//		FFacetType.Quad.expansionAmount = 100;
//		FFacetType.Quad.initialAmount = 100;

		FAtlas gameAtlas = Futile.atlasManager.LoadAtlas("Atlases/GameAtlas");

		FAtlas backgroundAtlas = Futile.atlasManager.LoadAtlas("Atlases/BackgroundAtlas");

		Fonts.Load();
		
		Futile.stage.AddChild(new Core());
	}

	public float force = 9.8f;
	
	void FixedUpdate ()
	{
		Vector2 dir = new Vector2(0,0);
		dir.x = Input.acceleration.x;
		dir.y = Input.acceleration.y;
		Physics.gravity = dir * force;
	}

	void OnApplicationQuit()
	{
//		if(Core.instance != null)
//		{
//			Core.instance.SaveAllData();
//		}
	}

	void OnApplicationPause(bool isPaused)
	{
//		if(isPaused && Core.instance != null)
//		{
//			Core.instance.HandleApplicationPause();
//		}
//		else if (!isPaused && Core.instance != null)
//		{
//			Core.instance.HandleApplicationResume();
//		}
	}
}









