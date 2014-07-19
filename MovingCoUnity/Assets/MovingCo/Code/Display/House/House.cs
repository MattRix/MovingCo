using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class House : FContainer
{
	public static House instance;
	public static FPWorld world;

	public HouseOutline houseOutline;

	public List<Item> items = new List<Item>();

	public FContainer itemHolder;

	public House ()
	{
		instance = this;

		AddChild(itemHolder = new FContainer());

		world = FPWorld.Create(80.0f);

		houseOutline = HouseOutline.Create();
		AddChild(houseOutline.holder);

		CreateItems();
	}

	void CreateItems()
	{
		Vector2 cursor = new Vector2(0,0);

		ItemMaker.Create_BBall(cursor);
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
		var go = new GameObject("HouseOutline");
		go.transform.parent = House.world.transform;
		var ho = go.AddComponent<HouseOutline>();

		ho.holder = new FContainer();
		ho.holder.AddChild(new FSprite("HouseOutline"));

		ho.link = go.AddComponent<FPNodeLink>();
		ho.link.Init (ho.holder,false);

		return ho;
	}
}

