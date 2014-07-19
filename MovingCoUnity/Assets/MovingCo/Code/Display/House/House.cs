using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class House : FContainer
{
	public FSprite houseOutline;

	public House ()
	{
		AddChild(houseOutline = new FSprite("HouseOutline"));

	}

	public void Destroy ()
	{

	}
}

