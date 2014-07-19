using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PostGameSection : Section
{
	public FLabel gameOverLabel;
	public FButton playAgainButton;
	
	public PostGameSection ()
	{
		
	}
	
	override public void Start()
	{
		AddChild(gameOverLabel = new FLabel(Fonts.Hand,"GAME OVER!"));
		
		playAgainButton = new FButton("UI/PlayAgainButton","UI/PlayAgainButton");
		playAgainButton.SetColors(new Color(0.9f,0.9f,0.9f),Color.white);
		playAgainButton.y = -Futile.screen.halfHeight + 100.0f;
		AddChild(playAgainButton);
		
		gameOverLabel.scale = 0.0f;
		Go.to (gameOverLabel, 0.4f, new TweenConfig().scaleXY(1.0f).backOut());

		playAgainButton.scale = 0.0f;
		Go.to (playAgainButton, 0.4f, new TweenConfig().scaleXY(1.0f).setDelay(0.1f).backOut());
		
		playAgainButton.SignalRelease += (FButton button) => {HandlePlayAgainTap();};
	}
	
	void HandlePlayAgainTap ()
	{
		Core.instance.SetSection(new StartSection());
	}
	
	override public void BuildOut()
	{
		Go.to (gameOverLabel, 0.4f, new TweenConfig().scaleXY(0.0f).setDelay(0.1f).backIn().onComplete(HandleBuildOutComplete));

		Go.to (playAgainButton, 0.4f, new TweenConfig().scaleXY(0.0f).backIn());
	}
	
	void HandleBuildOutComplete (AbstractTween t)
	{
		this.RemoveFromContainer();
		Core.instance.StartCurrentSection();
	}
}

