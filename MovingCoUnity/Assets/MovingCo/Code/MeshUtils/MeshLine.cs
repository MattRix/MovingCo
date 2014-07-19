using System;
using System.Collections;
using UnityEngine;

public class MeshLine : FMeshNode
{
	private float _lineWidth = 1.0f;
	private Vector2 _p0;
	private Vector2 _p1;
	private bool _needsRealloc = true;
	private Color _innerColor = Color.white;
	private Color _outerColor = Color.white;
	
	public MeshLine() : this(Vector2.zero, new Vector2(1.0f, 0.0f), 1.0f, Color.white)
	{
	}

	public MeshLine(Vector2 p0, Vector2 p1, float lineWidth) : this(p0, p1, lineWidth, Color.white)
	{
	}

	public MeshLine(Vector2 p0, Vector2 p1, float lineWidth, Color color) : base()
	{
		_p0 = p0;
		_p1 = p1;
		_lineWidth = lineWidth;
		_innerColor = color;
		_outerColor = color;

		FMeshData meshData = GenerateMesh();
		Init(meshData, Futile.atlasManager.GetElementWithName("Box"));
	}
	
	private FMeshData GenerateMesh()
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

		Vector2 lineVect = _p1 - _p0;
		Vector2 orthoVect = new Vector2(-lineVect.y, lineVect.x);
		orthoVect.Normalize();
		Vector2 halfWidthVect = orthoVect * 0.5f * _lineWidth;

		Vector2 va = _p0 + halfWidthVect;
		Vector2 vb = _p0 - halfWidthVect;
		Vector2 vc = _p1 - halfWidthVect;
		Vector2 vd = _p1 + halfWidthVect;

		Vector2 uv = new Vector2(0, 0);
		if (_needsRealloc)
		{
			meshData.AddQuad(new FMeshVertex(va, uv), new FMeshVertex(vb, uv), new FMeshVertex(vc, uv), new FMeshVertex(vd, uv));
		}
		else
		{
			FMeshQuad curQuad = meshData.GetQuad(0);
			curQuad.SetVertexPos(0, va).SetVertexColor(0, _innerColor);
			curQuad.SetVertexPos(1, vb).SetVertexColor(1, _innerColor);
			curQuad.SetVertexPos(2, vc).SetVertexColor(2, _outerColor);
			curQuad.SetVertexPos(3, vd).SetVertexColor(3, _outerColor);
		}
		_needsRealloc = false;
		return meshData;
	}

	void RegenerateMesh()
	{
		_meshData = GenerateMesh();
		HandleMeshDataChanged();
	}

	public void SetPoints(Vector2 p0, Vector2 p1)
	{
		_p0 = p0;
		_p1 = p1;
		RegenerateMesh();
	}
	
	public Vector2 p0
	{
		get { return _p0; }
		set
		{
			_p0 = value;
			RegenerateMesh();
		}
	}

	public Vector2 p1
	{
		get { return _p1; }
		set
		{
			_p1 = value;
			RegenerateMesh();
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

	override public Color color
	{
		get { return _innerColor; }
		set
		{
			if (value != _innerColor)
			{
				_innerColor = value;
				_outerColor = value;
				RegenerateMesh();
			}
		}
	}
}

