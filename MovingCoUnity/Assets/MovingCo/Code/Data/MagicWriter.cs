using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicWriter
{
	public static string magicMessage = "";

	public static void Append(string mess)
	{
		magicMessage += mess;

		Debug.Log ("MAGIC: appending " + mess);
	}

	public static void Write()
	{
		WriteFile(magicMessage);
	}

	public static string WriteFile(string text)
	{
		#if UNITY_EDITOR && !UNITY_WEBPLAYER
		string fullPath = 
			System.IO.Directory.GetCurrentDirectory()+
				System.IO.Path.DirectorySeparatorChar+
				"Assets"+
				System.IO.Path.DirectorySeparatorChar+
				"MagicMessage.txt";
		
		Debug.Log ("writing " + text);
		System.IO.File.WriteAllText(fullPath, text);
		
		//var info:FileInfo = new FileInfo("/Applications/TextEdit.app/Contents/MacOS/TextEdit");
		//System.Diagnostics.Process.Start(info.FullName);
		
		return fullPath;
		#else 
		return "COULD NOT WRITE";
		#endif
	}
}