using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemMaker : MonoBehaviour
{
	public static Item Create_BBall(Vector2 pos)
	{
		var item = Item.Create("BBall",pos);

		item.canBeDamaged = false;

		item.AddSprite("BBall");
		var coll = item.AddCircleCollider(22.0f,0.0f,0.0f);

		item.StartDebug(0xFF0000);

		PhysicMaterial mat = new PhysicMaterial();
		mat.bounciness = 0.8f;
		mat.dynamicFriction = 0.5f;
		mat.staticFriction = 0.5f;
		mat.frictionCombine = PhysicMaterialCombine.Maximum;
		mat.bounceCombine = PhysicMaterialCombine.Maximum;
		coll.material = mat;
		
		return item;
	}

	public static Item Create_DrawerSmall(Vector2 pos)
	{
		var item = Item.Create("DrawerSmall",pos);
		
		item.AddSprite("DrawerSmall");
		item.AddBoxCollider(new Rect(-47,-49,100,94),0);
		
		item.StartDebug(0xFF0000);
		
		return item;
	}

	public static Item Create_DeskLamp(Vector2 pos)
	{
		var item = Item.Create("DeskLamp",pos);
		
		item.AddSprite("DeskLamp");
		item.AddBoxCollider(new Rect(-20,-35,40,70),0);
		
		item.StartDebug(0xFF0000);
		
		return item;
	}

	public static Item Create_Couch(Vector2 pos)
	{
		var item = Item.Create("Couch",pos);
		
		item.AddSprite("Couch");
		item.AddBoxCollider(new Rect(-120,-50,240,110),0);
		
		item.StartDebug(0xFF0000);
		
		return item;
	}

}

