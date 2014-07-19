using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StartSection : Section
{
	public FContainer truckHolder;
	public FSprite truck;
	public FSprite truckSign;
	public FButton playAgainButton;

	public StartSection ()
	{

	}

	override public void Start()
	{
		AddChild(truckHolder = new FContainer());
		truckHolder.SetPosition(0,0);

		truckHolder.AddChild(truck = new FSprite("MovingTruck"));
		truckHolder.AddChild(truckSign = new FSprite("TruckSign"));
		truckSign.SetPosition(-170f,40f);	

		playAgainButton = new FButton("UI/StartButton","UI/StartButton");
		playAgainButton.SetColors(new Color(0.9f,0.9f,0.9f),Color.white);
		playAgainButton.y = -Futile.screen.halfHeight + 100.0f;
		AddChild(playAgainButton);

		truckHolder.scale = 0.0f;
		Go.to (truckHolder, 0.4f, new TweenConfig().scaleXY(1.0f).backOut());

		truckSign.scale = 0.0f;
		Go.to (truckSign, 0.4f, new TweenConfig().scaleXY(1.0f).setDelay(0.1f).backOut());

		playAgainButton.scale = 0.0f;
		Go.to (playAgainButton, 0.4f, new TweenConfig().scaleXY(1.0f).backOut());

		playAgainButton.SignalRelease += (FButton button) => {HandlePlayAgainTap();};
	}

	void HandlePlayAgainTap ()
	{
		Core.instance.SetSection(new GameSection());
	}

	override public void BuildOut()
	{
		Go.to (truckHolder, 0.4f, new TweenConfig().scaleXY(0.0f).setDelay(0.1f).backIn().onComplete(HandleBuildOutComplete));
		
		Go.to (truckSign, 0.4f, new TweenConfig().scaleXY(0.0f).backIn());
		
		Go.to (playAgainButton, 0.4f, new TweenConfig().scaleXY(0.0f).backIn());
	}

	void HandleBuildOutComplete (AbstractTween t)
	{
		this.RemoveFromContainer();
		Core.instance.StartCurrentSection();
	}
}

