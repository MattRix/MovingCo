using System;
using UnityEngine;

public class Core : FContainer
{

	public Core ()
	{
		var bg = new FSprite("PaperBG");
		AddChild(bg);

		var box = new FSprite("Box");
		AddChild(box);

		var label = new FLabel(Fonts.Medium,"Hello World!");
		AddChild(label);
	}

	void HandleUpdate ()
	{
		if(Input.GetKeyDown(KeyCode.R))
		{
		
		}
	}
}

