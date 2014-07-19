using System;
using UnityEngine;

public class Fonts
{
	public static string Medium = "DinCondBold_Medium";

	public static void Load()
	{
		LoadFont(Medium, 0, 0, 0, 0);
	}

	public static void LoadFont(string fontName, float offsetX, float offsetY,float lineHeightOffset, float kerningOffset)
	{
		offsetY -= 0f;
		lineHeightOffset -= 0;
		
		FTextParams textParams;
		
		textParams = new FTextParams();
		textParams.kerningOffset = kerningOffset;
		textParams.lineHeightOffset = lineHeightOffset;
		
		//DIRECT FONT LOADING
		//		Futile.atlasManager.LoadImage("Atlases/Fonts/"+fontName);
		//		Futile.atlasManager.LoadFont(fontName,"Atlases/Fonts/"+fontName, "Atlases/Fonts/"+fontName, offsetX,offsetY,textParams);
		
		//ATLAS FONT LOADING
		Futile.atlasManager.LoadFont(fontName,"Fonts/"+fontName, "Atlases/Fonts/"+fontName, offsetX,offsetY,textParams);
	}
}

