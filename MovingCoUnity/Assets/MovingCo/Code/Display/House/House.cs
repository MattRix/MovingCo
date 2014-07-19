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

	public FContainer debugHolder;

	public House ()
	{
		instance = this;

		AddChild(itemHolder = new FContainer());

		debugHolder = new FContainer();

		world = FPWorld.Create(80.0f);

		houseOutline = HouseOutline.Create();
		AddChild(houseOutline.holder);

		AddChild(debugHolder);

		CreateItems();
	}

	void CreateItems()
	{
		Vector2 cursor = new Vector2(0,0);

		AddItem(ItemMaker.Create_BBall(cursor));
	}

	public Item AddItem(Item item)
	{
		items.Add(item);
		return item;
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
	public List<HouseWall> walls = new List<HouseWall>();

	public static HouseOutline Create()
	{
		var go = new GameObject("HouseOutline");
		go.transform.parent = House.world.transform;
		var ho = go.AddComponent<HouseOutline>();

		ho.holder = new FContainer();
		ho.holder.AddChild(new FSprite("HouseOutline"));

		ho.link = go.AddComponent<FPNodeLink>();
		ho.link.Init (ho.holder,false);

		ho.CreateWalls();

		return ho;
	}

	void CreateWalls ()
	{

	}
}

public class HouseWall
{
	public HouseOutline outline;

	public static HouseWall Create(HouseOutline outline, Rect rect)
	{
		var go = new GameObject("HouseWall");
		go.transform.parent = House.world.transform;
		var wall = go.AddComponent<HouseOutline>();

		wall.outline = outline;
		

		
		return wall;
	}
}

