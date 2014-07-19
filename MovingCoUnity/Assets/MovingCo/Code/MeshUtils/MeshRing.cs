using System;
using System.Collections;
using UnityEngine;

public class MeshRing : MeshArcLine
{
	public MeshRing() : this(10.0f, 20.0f, 16, Color.white, Color.white)
	{
	}
	
	public MeshRing(float innerRadius, float outerRadius, int sides) 
		: this(innerRadius, outerRadius, sides, Color.white, Color.white)
	{
	}

	public MeshRing(float innerRadius, float outerRadius, int sides, Color innerColor, Color outerColor) 
		: base(0, 360, (outerRadius-innerRadius)*0.5f+innerRadius, outerRadius-innerRadius, sides, innerColor, outerColor)
	{
	}
	
	public int numSides
	{
		get { return _numSidesInCircle; }
		set
		{
			if (value != _numSidesInCircle && value >= 3)
			{
				_numSidesInCircle = value;
				RegenerateMesh();
			}
		}
	}

	public float innerRadius
	{
		get { return _radius - (_lineWidth * 0.5f); }
		set
		{
			if (value >= 0 && value <= outerRadius)
			{
				float curInnerRad = _radius - (_lineWidth * 0.5f);
				float innerRadDiff = value - curInnerRad;
				_lineWidth = _lineWidth - innerRadDiff;
				_radius = _radius + (innerRadDiff * 0.5f);
				if (_lineWidth < 0) _lineWidth = 0;
				if (_radius < 0) _radius = 0;
				RegenerateMesh();
			}
		}
	}

	public float outerRadius
	{
		get { return _radius + (_lineWidth * 0.5f); }
		set
		{
			if (value >= 0 && value >= innerRadius)
			{
				float curOuterRad = _radius + (_lineWidth * 0.5f);
				float outerRadDiff = value - curOuterRad;
				_lineWidth = _lineWidth + outerRadDiff;
				_radius = _radius + (outerRadDiff * 0.5f);
				if (_lineWidth < 0) _lineWidth = 0;
				if (_radius < 0) _radius = 0;
				RegenerateMesh();
			}
		}
	}
}

