using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSection : Section
{
	public static GameSection instance; 

	public FButton closeButton;
	public House house;
	public FLabel scoreLabel;
	public float redness = 0.0f;

	public GameSection ()
	{
		instance = this;
	}
	
	override public void Start()
	{
		Core.instance.didWin = false;
		Core.instance.score = Config.START_SCORE;

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

		scoreLabel = new FLabel(Fonts.Hand, "$");
		AddChild(scoreLabel);
		scoreLabel.color = new Color(0.85f,1.0f,0.85f);
		scoreLabel.SetPosition(house.x + 0, house.y + 270.0f);

		scoreLabel.scale = 0.0f;
		Go.to (scoreLabel, 0.4f, new TweenConfig().scaleXY(1.0f).setDelay(0.2f).backOut());

		ListenForUpdate(HandleUpdate);
	}

	void HandleUpdate ()
	{
		int score = Core.instance.score;

		if(score < 0) score = 0;
		{
			if(Time.frameCount % 60 == 0)
			{
				score -= 1;
			}
		}

		redness = Mathf.Max(0.0f,redness-0.01f);

		scoreLabel.color = Color.Lerp(new Color(0.85f,1.0f,0.85f), Color.red, redness);

		scoreLabel.text = "$"+score;
		Core.instance.score = score;
	}

	void HandleCloseTap ()
	{
		Core.instance.SetSection(new PostGameSection());
	}
	
	override public void BuildOut()
	{
		instance = null;
		Go.to (scoreLabel, 0.4f, new TweenConfig().scaleXY(0.0f).backIn());
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

