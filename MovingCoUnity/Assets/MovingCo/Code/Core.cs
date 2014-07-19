using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Core : FContainer
{
	public static Core instance;
	
	public Section currentSection = null;

	public FSprite background;

	public Core ()
	{
		instance = this;

		background = new FSprite("PaperBG");
		AddChild(background);
//
//		var box = new FSprite("Box");
//		AddChild(box);
//
//		var label = new FLabel(Fonts.Medium,"Hello World!");
//		AddChild(label);

		Futile.instance.StartDelayedCallback(Start,0.1f);
	}

	public float force = 9.8f;
	
	void FixedUpdate ()
	{
		Vector2 dir = new Vector2(0,0);
		dir.x = Input.acceleration.x;
		dir.y = Input.acceleration.y;
		Physics.gravity = dir * force;
	}

	void Start ()
	{
		//SetSection(new StartSection());
		SetSection(new GameSection());
		StartCurrentSection();
	}

	public void SetSection(Section section)
	{
		if(currentSection != null)
		{
			currentSection.BuildOut();
			currentSection = null;
		}

		if(section != null)
		{
			currentSection = section;
			AddChild(currentSection);
		}
	}

	public void StartCurrentSection()
	{
		currentSection.Start();
	}

	void HandleUpdate ()
	{
		if(Input.GetKeyDown(KeyCode.R))
		{
		
		}
	}
}

