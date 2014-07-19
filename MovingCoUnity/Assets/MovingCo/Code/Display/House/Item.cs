using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Item : MonoBehaviour
{
	public FContainer holder;
	public FPNodeLink link;
	public bool canBeDamaged = true;
	public float redness = 0.0f;
	public FSprite sprite;
	
	public static Item Create(string name, Vector2 pos)
	{
		var go = new GameObject(name);
		go.transform.parent = House.instance.houseOutline.gameObject.transform;

		go.transform.localPosition = new Vector3(pos.x*FPhysics.POINTS_TO_METERS,pos.y*FPhysics.POINTS_TO_METERS,0);

		var item = go.AddComponent<Item>();
		
		item.holder = new FContainer();
		
		item.link = go.AddComponent<FPNodeLink>();
		item.link.Init (item.holder,true);

		var rb = go.AddComponent<Rigidbody>();
		rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

		if(House.instance != null)
		{
			House.instance.itemHolder.AddChild(item.holder);
		}
		
		return item;
	}

	public void Update()
	{
		redness = Mathf.Max(0.0f,redness-0.01f);

		if(sprite != null)
		{
			sprite.color = new Color(1.0f,1.0f-redness,1.0f-redness);
		}
	}

	public void OnCollisionEnter (Collision colli) 
	{
		if(!canBeDamaged) return;

		if(colli.relativeVelocity.magnitude > 3)
		{
			TakeDamage();
		}
	}

	void TakeDamage ()
	{
		redness = 1.0f;

		if(Core.instance.score > 0)
		{
			Core.instance.score -= 1;
		}
	}

	public FSprite AddSprite(string name)
	{
		sprite = new FSprite("Items/"+name+"_front");
		holder.AddChild(sprite);

		return sprite;
	}

	public SphereCollider AddCircleCollider(float radius, float offsetX, float offsetY)
	{
		//var go = new GameObject("CircleColl");
		//go.transform.parent = gameObject.transform;
		//go.transform.localPosition = new Vector3(offsetX*FPhysics.POINTS_TO_METERS, offsetY*FPhysics.POINTS_TO_METERS,0);

		SphereCollider coll = gameObject.AddComponent<SphereCollider>();
		coll.radius = radius * FPhysics.POINTS_TO_METERS;
		coll.center = new Vector3(offsetX*FPhysics.POINTS_TO_METERS, offsetY*FPhysics.POINTS_TO_METERS,0);
		return coll;
	}

	public BoxCollider AddBoxCollider(Rect rect, float angle)
	{
		//var go = new GameObject("BoxColl");
		//go.transform.parent = gameObject.transform;
		//go.transform.localPosition = new Vector3(rect.center.x*FPhysics.POINTS_TO_METERS,rect.center.y*FPhysics.POINTS_TO_METERS,0);
		//go.transform.localEulerAngles = new Vector3(0,0,angle);

		var coll = gameObject.AddComponent<BoxCollider>();
		coll.size = new Vector3(rect.width*FPhysics.POINTS_TO_METERS, rect.height*FPhysics.POINTS_TO_METERS,FPhysics.DEFAULT_Z_THICKNESS);
		coll.center = new Vector3(rect.center.x*FPhysics.POINTS_TO_METERS,rect.center.y*FPhysics.POINTS_TO_METERS,0);
		return coll;
	}

	public void StartDebug(uint color)
	{
		FPDebugRenderer.Create(gameObject,House.instance.debugHolder,color,false);
	}

	public void RemoveFromWorld ()
	{
		holder.RemoveFromContainer();
		UnityEngine.Object.Destroy(gameObject);
	}
}

