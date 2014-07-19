using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class House : FContainer
{
	public static House instance;
	public static FPWorld world;

	public HouseOutline houseOutline;

	public House ()
	{
		instance = this;

		world = FPWorld.Create(80.0f);

		houseOutline = HouseOutline.Create();
		AddChild(houseOutline.holder);


	}

	public void Destroy ()
	{
		instance = null;

		UnityEngine.Object.Destroy(world.gameObject);
		world = null;
	}
}

public class HouseOutline : MonoBehaviour
{
	public FContainer holder;
	public FPNodeLink link;

	public static HouseOutline Create()
	{
		var go = new GameObject();
		go.transform.parent = House.world.transform;
		var ho = go.AddComponent<HouseOutline>();

		ho.holder = new FContainer();
		ho.holder.AddChild(new FSprite("HouseOutline"));

		ho.link = go.AddComponent<FPNodeLink>();
		ho.link.Init (ho.holder,false);

		return ho;
	}
}

