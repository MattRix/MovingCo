using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSection : Section
{
	public FButton closeButton;
	public House house;

	public GameSection ()
	{

	}
	
	override public void Start()
	{
		closeButton = new FButton("UI/CloseButton","UI/CloseButton");
		closeButton.SetColors(new Color(0.9f,0.9f,0.9f),Color.white);
		closeButton.x = -Futile.screen.halfWidth + 54.0f;
		closeButton.y = Futile.screen.halfHeight - 52.0f;
		AddChild(closeButton);	
		
		closeButton.SignalRelease += (FButton button) => {HandleCloseTap();};

		closeButton.scale = 0.0f;
		Go.to (closeButton, 0.4f, new TweenConfig().scaleXY(1.0f).setDelay(0.1f).backOut());

		house = new House();
		house.SetPosition(25.0f,-12.0f);
		AddChild(house);

		house.scale = 0.0f;
		Go.to (house, 0.4f, new TweenConfig().scaleXY(1.0f).backOut());
	}

	void HandleCloseTap ()
	{
		Core.instance.SetSection(new PostGameSection());
	}
	
	override public void BuildOut()
	{
		Go.to (closeButton, 0.4f, new TweenConfig().scaleXY(0.0f).backIn());
		Go.to (house, 0.4f, new TweenConfig().scaleXY(0.0f).setDelay(0.1f).backIn().onComplete(HandleBuildOutComplete));
	}

	void HandleBuildOutComplete (AbstractTween t)
	{
		house.Destroy();
		this.RemoveFromContainer();
		Core.instance.StartCurrentSection();
	}
}

