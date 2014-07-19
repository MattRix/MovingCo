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
		item.AddCircleCollider(18.0f,0.0f,0.0f);

		item.StartDebug(0xFF0000);
		
		return item;
	}
}

