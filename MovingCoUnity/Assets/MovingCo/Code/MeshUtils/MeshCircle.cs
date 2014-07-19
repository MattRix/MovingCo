using System;
using System.Collections;
using UnityEngine;

public class MeshCircle : FMeshNode
{
	private float _radius = 1.0f;
	private int _numSides = 16;
	private bool _needsRealloc = true;
	private Color _innerColor = Color.white;
	private Color _outerColor = Color.white;

	public MeshCircle() : this(1.0f, 16, Color.white, Color.white)
	{
	}

	public MeshCircle(float radius, int sides) : this(radius, sides, Color.white, Color.white)
	{
	}

	public MeshCircle(float radius, int sides, Color color) : this(radius, sides, color, color)
	{
	}

	public MeshCircle(float radius, int sides, Color innerColor, Color outerColor) : base()
	{
		_radius = radius;
		_numSides = sides;
		_innerColor = innerColor;
		_outerColor = outerColor;

		FMeshData meshData = GenerateMesh();
		Init(meshData, Futile.atlasManager.GetElementWithName("Box"));
	}

	private FMeshData GenerateMesh()
	{
		FMeshData meshData;
		if (_needsRealloc)
		{
			meshData = new FMeshData(FFacetType.Triangle);
		}
		else
		{
			// reuse the existing data
			meshData = _meshData;
		}
		float curAngle = 0.0f;
		float increment = 360.0f / _numSides;
		Vector2 uv = new Vector2(0, 0);
		Vector2 origin = new Vector2(0, 0);
		Vector2 prevVert = new Vector2(_radius, 0.0f);
		Vector2 curVert = new Vector2();
		int curTriIndex = 0;
		for (int v = 1; v <= _numSides; v++)
		{
			if (v < _numSides)
			{
				curAngle += increment;
			}
			else
			{
				curAngle = 0.0f;
			}
			float angleRad = curAngle * RXMath.DTOR;
			curVert.x = Mathf.Cos(angleRad) * _radius;
			curVert.y = Mathf.Sin(angleRad) * _radius;
			if (_needsRealloc)
			{
				FMeshVertex v0, v1, v2;
				v0 = new FMeshVertex(origin, uv);
				v1 = new FMeshVertex(prevVert, uv);
				v2 = new FMeshVertex(curVert, uv);
				v0.color = _innerColor;
				v1.color = v2.color = _outerColor;
				meshData.AddTriangle(v0, v1, v2);
			}
			else
			{
				FMeshTriangle curTri = meshData.GetTriangle(curTriIndex);
				curTri.SetVertexPos(0, origin).SetVertexColor(0, _innerColor);
				curTri.SetVertexPos(1, prevVert).SetVertexColor(1, _outerColor);
				curTri.SetVertexPos(2, curVert).SetVertexColor(2, _outerColor);
			}
			prevVert = curVert;
			++curTriIndex;
		}
		_needsRealloc = false;
		return meshData;
	}

	void RegenerateMesh()
	{
		_meshData = GenerateMesh();
		_meshData.MarkChanged();
	}

	public int numSides
	{
		get { return _numSides; }
		set
		{
			if (value != _numSides && value >= 3)
			{
				_numSides = value;
				_needsRealloc = true;
				RegenerateMesh();
			}
		}
	}

	public float radius
	{
		get { return _radius; }
		set
		{
			if (value > 0)
			{
				_radius = value;
				RegenerateMesh();
			}
		}
	}

	override public Color color
	{
		set
		{
			if (value != _innerColor || value != _outerColor)
			{
				_innerColor = value;
				_outerColor = value;
				RegenerateMesh();
			}
		}
	}

	public Color innerColor
	{
		get { return _innerColor; }
		set
		{
			if (value != _innerColor)
			{
				_innerColor = value;
				RegenerateMesh();
			}
		}
	}

	public Color outerColor
	{
		get { return _outerColor; }
		set
		{
			if (value != _outerColor)
			{
				_outerColor = value;
				RegenerateMesh();
			}
		}
	}
}

