using System;
using System.Collections;
using UnityEngine;

public class MeshArcLine : FMeshNode
{
	protected float _startAngle = 0.0f;
	protected float _endAngle = 30.0f;
	protected float _radius = 10.0f;
	protected float _lineWidth = 1.0f;
	protected int _numSidesInCircle = 16;
	protected bool _needsRealloc = true;
	protected Color _innerColor = Color.white;
	protected Color _outerColor = Color.white;

	public MeshArcLine() 
		: this(0.0f, 30.0f, 10.0f, 1.0f, 16, Color.white, Color.white)
	{
	}

	public MeshArcLine(float startAngle, float endAngle, float radius, float lineWidth, int numSidesInCircle) 
		: this(startAngle, endAngle, radius, lineWidth, numSidesInCircle, Color.white, Color.white)
	{
	}

	public MeshArcLine(float startAngle, float endAngle, float radius, float lineWidth, int numSidesInCircle, Color innerColor, Color outerColor) 
		: base()
	{
		_startAngle = (startAngle <= endAngle) ? startAngle : endAngle;
		_endAngle = (startAngle <= endAngle) ? endAngle : startAngle;
		_radius = radius;
		_lineWidth = lineWidth;
		_numSidesInCircle = numSidesInCircle;
		_innerColor = innerColor;
		_outerColor = outerColor;

		FMeshData meshData = GenerateMesh();
		Init(meshData, Futile.atlasManager.GetElementWithName("Box"));
	}
	
	protected FMeshData GenerateMesh()
	{
		FMeshData meshData;
		if (_needsRealloc)
		{
			meshData = new FMeshData(FFacetType.Quad);
		}
		else
		{
			// reuse the existing data
			meshData = _meshData;
		}

		float curAngle = _startAngle;
		float increment = 360.0f / _numSidesInCircle;

		float angleRad = curAngle * RXMath.DTOR;
		float angleX = Mathf.Cos(angleRad);
		float angleY = Mathf.Sin(angleRad);
		float innerRadius = _radius - (_lineWidth * 0.5f);
		float outerRadius = _radius + (_lineWidth * 0.5f);

		Vector2 uv = new Vector2(0, 0);
		Vector2 prevInnerVert = new Vector2(angleX * innerRadius, angleY * innerRadius);
		Vector2 prevOuterVert = new Vector2(angleX * outerRadius, angleY * outerRadius);
		Vector2 curInnerVert = new Vector2();
		Vector2 curOuterVert = new Vector2();
		int curQuadIndex = 0;

		do
		{
			curAngle += increment;
			if (curAngle > _endAngle)
			{
				curAngle = _endAngle;
			}
			angleRad = curAngle * RXMath.DTOR;
			angleX = Mathf.Cos(angleRad);
			angleY = Mathf.Sin(angleRad);
			curInnerVert.x = angleX * innerRadius;
			curInnerVert.y = angleY * innerRadius;
			curOuterVert.x = angleX * outerRadius;
			curOuterVert.y = angleY * outerRadius;

			if (_needsRealloc)
			{
				FMeshVertex v0, v1, v2, v3;
				v0 = new FMeshVertex(prevInnerVert, uv);
				v1 = new FMeshVertex(prevOuterVert, uv);
				v2 = new FMeshVertex(curOuterVert, uv);
				v3 = new FMeshVertex(curInnerVert, uv);
				v0.color = v3.color = _innerColor;
				v1.color = v2.color = _outerColor;
				meshData.AddQuad(v0, v1, v2, v3);
			}
			else
			{
				FMeshQuad curQuad = meshData.GetQuad(curQuadIndex);
				curQuad.SetVertexPos(0, prevInnerVert).SetVertexColor(0, _innerColor);
				curQuad.SetVertexPos(1, prevOuterVert).SetVertexColor(1, _outerColor);
				curQuad.SetVertexPos(2, curOuterVert).SetVertexColor(2, _outerColor);
				curQuad.SetVertexPos(3, curInnerVert).SetVertexColor(3, _innerColor);
			}

			prevInnerVert = curInnerVert;
			prevOuterVert = curOuterVert;
			++curQuadIndex;
		} while (curAngle < _endAngle);

		_needsRealloc = false;
		return meshData;
	}
	
	protected void RegenerateMesh()
	{
		_meshData = GenerateMesh();
		HandleMeshDataChanged();
	}

	public float startAngle
	{
		get { return _startAngle; }
		set
		{
			if (value != _startAngle)
			{
				_startAngle = value;
				RegenerateMesh();
			}
		}
	}
	
	public float endAngle
	{
		get { return _endAngle; }
		set
		{
			if (value != _endAngle)
			{
				_endAngle = value;
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

	public float lineWidth
	{
		get { return _lineWidth; }
		set
		{
			_lineWidth = value;
			RegenerateMesh();
		}
	}

	public int numSidesInCircle
	{
		get { return _numSidesInCircle; }
		set
		{
			if (value != _numSidesInCircle && value >= 3)
			{
				_numSidesInCircle = value;
				_needsRealloc = true;
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

