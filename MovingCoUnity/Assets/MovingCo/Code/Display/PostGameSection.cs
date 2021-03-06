using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PostGameSection : Section
{
	public FLabel gameOverLabel;
	public FButton playAgainButton;
	public FLabel scoreLabel;
	
	public PostGameSection ()
	{
		
	}
	
	override public void Start()
	{
		bool didWin = Core.instance.didWin;

		AddChild(gameOverLabel = new FLabel(Fonts.Hand,didWin?"YOU WON!":"GAME OVER!"));
		
		playAgainButton = new FButton("UI/PlayAgainButton","UI/PlayAgainButton");
		playAgainButton.SetColors(new Color(0.9f,0.9f,0.9f),Color.white);
		playAgainButton.y = -Futile.screen.halfHeight + 100.0f;
		AddChild(playAgainButton);
		
		gameOverLabel.scale = 0.0f;
		Go.to (gameOverLabel, 0.4f, new TweenConfig().scaleXY(1.0f).backOut());

		playAgainButton.scale = 0.0f;
		Go.to (playAgainButton, 0.4f, new TweenConfig().scaleXY(1.0f).setDelay(0.1f).backOut());

		if(didWin)
		{
			AddChild(scoreLabel = new FLabel(Fonts.Hand,"$"+Core.instance.score));
			scoreLabel.color = new Color(0.85f,1.0f,0.85f);
			scoreLabel.y = gameOverLabel.y - 50.0f;
			gameOverLabel.y += 50.0f;
		}
		
		playAgainButton.SignalRelease += (FButton button) => {HandlePlayAgainTap();};
	}
	
	void HandlePlayAgainTap ()
	{
		Core.instance.SetSection(new StartSection());
	}
	
	override public void BuildOut()
	{
		Go.to (gameOverLabel, 0.4f, new TweenConfig().scaleXY(0.0f).setDelay(0.1f).backIn().onComplete(HandleBuildOutComplete));
		if(scoreLabel != null) Go.to (scoreLabel, 0.4f, new TweenConfig().scaleXY(0.0f).backIn());
		Go.to (playAgainButton, 0.4f, new TweenConfig().scaleXY(0.0f).backIn());
	}
	
	void HandleBuildOutComplete (AbstractTween t)
	{
		this.RemoveFromContainer();
		Core.instance.StartCurrentSection();
	}
}

