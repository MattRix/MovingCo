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
	public FContainer itemShadowHolder;

	public FContainer debugHolder;

	public Vector2 firstPoint;
	public bool hasFirstPoint = false;

	public Vector3 currentAccel;

	public House ()
	{
		instance = this;

		AddChild(itemShadowHolder = new FContainer());
		AddChild(itemHolder = new FContainer());

		itemShadowHolder.y -= 5.0f;

		debugHolder = new FContainer();

		world = FPWorld.Create(80.0f);

		houseOutline = HouseOutline.Create();
		AddChild(houseOutline.holder);

		AddChild(debugHolder);

		CreateItems();

		ListenForUpdate(HandleUpdate);
	}

	void HandleUpdate ()
	{
		#if UNITY_EDITOR
		if(Input.GetMouseButtonDown(0))
		{
			Vector2 point = this.GetLocalMousePosition();
			if(!hasFirstPoint)
			{
				firstPoint = point;
				hasFirstPoint = true;
			}
			else 
			{
				Rect rect = new Rect(firstPoint.x, firstPoint.y, point.x-firstPoint.x, point.y-firstPoint.y);
				string message = "";
				message += rect.x.ToString("#") + ",";
				message += rect.y.ToString("#")+ ",";
				message += rect.width.ToString("#")+ ",";
				message += rect.height.ToString("#")+ "";
				message = "AddWall(new Rect("+message+"),0);\n";
				MagicWriter.Append(message);
				MagicWriter.Write();
				hasFirstPoint = false;
			}
		}
		#endif
	}

	void CreateItems()
	{
		Vector2 cursor = new Vector2(0,50);

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

	void CreateWalls()
	{
		AddWall(new Rect(-600,-340,1200,50),1.0f);//ground below house
		AddWall(new Rect(-400,-305,800,25),3.0f);//floor of house

		//stairs
		AddWall(new Rect(-401,-306,260,38),0);
		AddWall(new Rect(-405,-275,236,38),0);
		AddWall(new Rect(-405,-237,206,36),0);
		AddWall(new Rect(-406,-223,178,58),0);
		AddWall(new Rect(-412,-191+5,156,54),0);
		AddWall(new Rect(-410,-153+5,122,52),0);
		AddWall(new Rect(-406,-117+5,86,44),0);
		AddWall(new Rect(-404,-81+5,50,36),0);

		AddWall(new Rect(-405,-48,18,262),0); //left wall
		AddWall(new Rect(-403,210+5,764,28),0); //ceiling
		AddWall(new Rect(353,-22,14,258),0); //top right wall
		AddWall(new Rect(-161,-40,530,24),1); //top floor

		//roof
		AddWall(new Rect(-475,208+5,908,30),0);
		AddWall(new Rect(-447,236,838,42),0);
		AddWall(new Rect(-401,280,756,36),0);
	}

	HouseWall AddWall (Rect rect, float angle)
	{
		var wall = HouseWall.Create(this,rect, angle);
		walls.Add(wall);
		return wall;
	}
}

public class HouseWall : MonoBehaviour
{
	public HouseOutline outline;
	public BoxCollider coll;

	public static HouseWall Create(HouseOutline outline, Rect rect, float angle)
	{
		var go = new GameObject("HouseWall");
		go.transform.parent = outline.transform;
		go.transform.localEulerAngles = new Vector3(0,0,angle);

		var wall = go.AddComponent<HouseWall>();
		wall.outline = outline;

		var coll = go.AddComponent<BoxCollider>();
		coll.size = new Vector3(rect.width*FPhysics.POINTS_TO_METERS, rect.height*FPhysics.POINTS_TO_METERS,FPhysics.DEFAULT_Z_THICKNESS);
		go.transform.localPosition = new Vector3(rect.center.x*FPhysics.POINTS_TO_METERS,rect.center.y*FPhysics.POINTS_TO_METERS,0);

		FPDebugRenderer.Create(go,House.instance.debugHolder,0x0000FF,false);

//		PhysicMaterial mat = new PhysicMaterial();
//		mat.bounciness = 0.5f;
//		mat.dynamicFriction = 0.5f;
//		mat.staticFriction = 0.5f;
//		wall.coll.material = mat;

		return wall;
	}
}

