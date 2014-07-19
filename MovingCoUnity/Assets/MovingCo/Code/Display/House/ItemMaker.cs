using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemMaker : MonoBehaviour
{
	public static Item Create_BBall(Vector2 pos)
	{
		var item = Item.Create("BBall",pos);

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
}

