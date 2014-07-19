using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//from http://wiki.unity3d.com/index.php/MaxRectsBinPack
//note: this could be cleaned up and optimized a LOT
//best packing: ShortSideFit is best, follwed by Contact Point Rule, and Bottom Left Rule
//perf: some benchmarking is needed to figure out what is fastest
//I switched Rect for IntRect and increased performance 10%
//I then made IntRect a class, which increased performance to 50% faster

public class PRPacker
{
	public int binWidth = 0;
	public int binHeight = 0;
	public bool allowRotations;
	
	public List<PRRect> usedRectangles = new List<PRRect>(100);
	public List<PRRect> freeRectangles = new List<PRRect>(500);
	
	public enum ChoiceHeuristic 
	{
		ShortSideFit, //< -BSSF: Positions the rectangle against the short side of a free rectangle into which it fits the best.
		LongSideFit, //< -BLSF: Positions the rectangle against the long side of a free rectangle into which it fits the best.
		AreaFit, //< -BAF: Positions the rectangle into the smallest free rect into which it fits.
		BottomLeftRule, //< -BL: Does the Tetris placement.
		ContactPointRule //< -CP: Choosest the placement where the rectangle touches other rects as much as possible.
	};
	
	public PRPacker(int width, int height) 
	{
		Init(width, height);
	}
	
	public void Init(int width, int height) 
	{
		binWidth = width;
		binHeight = height;
		allowRotations = false;
		
		PRRect n = new PRRect();
		n.x = 0;
		n.y = 0;
		n.width = width;
		n.height = height;
		
		usedRectangles.Clear();
		
		freeRectangles.Clear();
		freeRectangles.Add(n);
	}
	
	public PRRect Insert(int width, int height, ChoiceHeuristic method) 
	{
		PRRect newNode = new PRRect();
		int score1 = 0; // Unused in this function. We don't need to know the score after finding the position.
		int score2 = 0;

		switch(method) 
		{
			case ChoiceHeuristic.ShortSideFit: newNode = FindPositionForNewNodeBestShortSideFit(width, height, ref score1, ref score2); break;
			case ChoiceHeuristic.BottomLeftRule: newNode = FindPositionForNewNodeBottomLeft(width, height, ref score1, ref score2); break;
			case ChoiceHeuristic.ContactPointRule: newNode = FindPositionForNewNodeContactPoint(width, height, ref score1); break;
			case ChoiceHeuristic.LongSideFit: newNode = FindPositionForNewNodeBestLongSideFit(width, height, ref score2, ref score1); break;
			case ChoiceHeuristic.AreaFit: newNode = FindPositionForNewNodeBestAreaFit(width, height, ref score1, ref score2); break;
		}
		
		if (newNode.height == 0) return newNode;

		int numRectanglesToProcess = freeRectangles.Count;
		for(int i = 0; i < numRectanglesToProcess; ++i) 
		{
			if (SplitFreeNode(freeRectangles[i], ref newNode)) 
			{
				freeRectangles.RemoveAt(i);
				--i;
				--numRectanglesToProcess;
			}
		}
		
		PruneFreeList();
		
		usedRectangles.Add(newNode);
		return newNode;
	}
	
	public void Insert(List<PRRect> rects, List<PRRect> dst, ChoiceHeuristic method) 
	{
		dst.Clear();
		
		while(rects.Count > 0)
		{
			int bestScore1 = int.MaxValue;
			int bestScore2 = int.MaxValue;
			int bestRectIndex = -1;
			PRRect bestNode = new PRRect();
			
			for(int i = 0; i < rects.Count; ++i) 
			{
				int score1 = 0;
				int score2 = 0;
				PRRect newNode = ScoreRect(rects[i].width, rects[i].height, method, ref score1, ref score2);
				
				if (score1 < bestScore1 || (score1 == bestScore1 && score2 < bestScore2)) 
				{
					bestScore1 = score1;
					bestScore2 = score2;
					bestNode = newNode;
					bestRectIndex = i;
				}
			}
			
			if (bestRectIndex == -1)
				return;
			
			PlaceRect(bestNode);
			rects.RemoveAt(bestRectIndex);
		}
	}
	
	void PlaceRect(PRRect node) 
	{
		int numRectanglesToProcess = freeRectangles.Count;

		for(int i = 0; i < numRectanglesToProcess; ++i) 
		{
			if (SplitFreeNode(freeRectangles[i], ref node)) 
			{
				freeRectangles.RemoveAt(i);
				--i;
				--numRectanglesToProcess;
			}
		}
		
		PruneFreeList();
		
		usedRectangles.Add(node);
	}
	
	PRRect ScoreRect(int width, int height, ChoiceHeuristic method, ref int score1, ref int score2) 
	{
		PRRect newNode = new PRRect();
		score1 = int.MaxValue;
		score2 = int.MaxValue;

		switch(method) 
		{
			case ChoiceHeuristic.ShortSideFit: newNode = FindPositionForNewNodeBestShortSideFit(width, height, ref score1, ref score2); break;
			case ChoiceHeuristic.BottomLeftRule: newNode = FindPositionForNewNodeBottomLeft(width, height, ref score1, ref score2); break;
			case ChoiceHeuristic.ContactPointRule: newNode = FindPositionForNewNodeContactPoint(width, height, ref score1); 
			score1 = -score1; // Reverse since we are minimizing, but for contact point score bigger is better.
			break;
			case ChoiceHeuristic.LongSideFit: newNode = FindPositionForNewNodeBestLongSideFit(width, height, ref score2, ref score1); break;
			case ChoiceHeuristic.AreaFit: newNode = FindPositionForNewNodeBestAreaFit(width, height, ref score1, ref score2); break;
		}
		
		// Cannot fit the current rectangle.
		if (newNode.height == 0) 
		{
			score1 = int.MaxValue;
			score2 = int.MaxValue;
		}
		
		return newNode;
	}
	
	/// Computes the ratio of used surface area.
	public float Occupancy() 
	{
		ulong usedSurfaceArea = 0;
		for(int i = 0; i < usedRectangles.Count; ++i)
		{
			usedSurfaceArea += (uint)usedRectangles[i].width * (uint)usedRectangles[i].height;
		}
		
		return (float)usedSurfaceArea / (binWidth * binHeight);
	}
	
	PRRect FindPositionForNewNodeBottomLeft(int width, int height, ref int bestY, ref int bestX) 
	{
		PRRect bestNode = new PRRect();
		//memset(bestNode, 0, sizeof(Rect));
		
		bestY = int.MaxValue;
		
		for(int i = 0; i < freeRectangles.Count; ++i) 
		{
			// Try to place the rectangle in upright (non-flipped) orientation.
			if (freeRectangles[i].width >= width && freeRectangles[i].height >= height)
			{
				int topSideY = freeRectangles[i].y + height;
				if (topSideY < bestY || (topSideY == bestY && freeRectangles[i].x < bestX)) 
				{
					bestNode.x = freeRectangles[i].x;
					bestNode.y = freeRectangles[i].y;
					bestNode.width = width;
					bestNode.height = height;
					bestY = topSideY;
					bestX = freeRectangles[i].x;
				}
			}

			if (allowRotations && freeRectangles[i].width >= height && freeRectangles[i].height >= width)
			{
				int topSideY = freeRectangles[i].y + width;
				if (topSideY < bestY || (topSideY == bestY && freeRectangles[i].x < bestX)) 
				{
					bestNode.x = freeRectangles[i].x;
					bestNode.y = freeRectangles[i].y;
					bestNode.width = height;
					bestNode.height = width;
					bestY = topSideY;
					bestX = freeRectangles[i].x;
				}
			}
		}
		return bestNode;
	}
	
	PRRect FindPositionForNewNodeBestShortSideFit(int width, int height, ref int bestShortSideFit, ref int bestLongSideFit)  
	{
		PRRect bestNode = new PRRect();
		//memset(&bestNode, 0, sizeof(Rect));
		
		bestShortSideFit = int.MaxValue;
		
		for(int i = 0; i < freeRectangles.Count; ++i) 
		{
			// Try to place the rectangle in upright (non-flipped) orientation.
			if (freeRectangles[i].width >= width && freeRectangles[i].height >= height) 
			{
				int leftoverHoriz = Mathf.Abs(freeRectangles[i].width - width);
				int leftoverVert = Mathf.Abs(freeRectangles[i].height - height);
				int shortSideFit = Mathf.Min(leftoverHoriz, leftoverVert);
				int longSideFit = Mathf.Max(leftoverHoriz, leftoverVert);
				
				if (shortSideFit < bestShortSideFit || (shortSideFit == bestShortSideFit && longSideFit < bestLongSideFit))
				{
					bestNode.x = freeRectangles[i].x;
					bestNode.y = freeRectangles[i].y;
					bestNode.width = width;
					bestNode.height = height;
					bestShortSideFit = shortSideFit;
					bestLongSideFit = longSideFit;
				}
			}
			
			if (allowRotations && freeRectangles[i].width >= height && freeRectangles[i].height >= width) 
			{
				int flippedLeftoverHoriz = Mathf.Abs(freeRectangles[i].width - height);
				int flippedLeftoverVert = Mathf.Abs(freeRectangles[i].height - width);
				int flippedShortSideFit = Mathf.Min(flippedLeftoverHoriz, flippedLeftoverVert);
				int flippedLongSideFit = Mathf.Max(flippedLeftoverHoriz, flippedLeftoverVert);
				
				if (flippedShortSideFit < bestShortSideFit || (flippedShortSideFit == bestShortSideFit && flippedLongSideFit < bestLongSideFit)) 
				{
					bestNode.x = freeRectangles[i].x;
					bestNode.y = freeRectangles[i].y;
					bestNode.width = height;
					bestNode.height = width;
					bestShortSideFit = flippedShortSideFit;
					bestLongSideFit = flippedLongSideFit;
				}
			}
		}
		return bestNode;
	}
	
	PRRect FindPositionForNewNodeBestLongSideFit(int width, int height, ref int bestShortSideFit, ref int bestLongSideFit) 
	{
		PRRect bestNode = new PRRect();
		//memset(&bestNode, 0, sizeof(Rect));
		
		bestLongSideFit = int.MaxValue;
		
		for(int i = 0; i < freeRectangles.Count; ++i) 
		{
			// Try to place the rectangle in upright (non-flipped) orientation.
			if (freeRectangles[i].width >= width && freeRectangles[i].height >= height)
			{
				int leftoverHoriz = Mathf.Abs(freeRectangles[i].width - width);
				int leftoverVert = Mathf.Abs(freeRectangles[i].height - height);
				int shortSideFit = Mathf.Min(leftoverHoriz, leftoverVert);
				int longSideFit = Mathf.Max(leftoverHoriz, leftoverVert);
				
				if (longSideFit < bestLongSideFit || (longSideFit == bestLongSideFit && shortSideFit < bestShortSideFit)) 
				{
					bestNode.x = freeRectangles[i].x;
					bestNode.y = freeRectangles[i].y;
					bestNode.width = width;
					bestNode.height = height;
					bestShortSideFit = shortSideFit;
					bestLongSideFit = longSideFit;
				}
			}
			
			if (allowRotations && freeRectangles[i].width >= height && freeRectangles[i].height >= width) 
			{
				int leftoverHoriz = Mathf.Abs(freeRectangles[i].width - height);
				int leftoverVert = Mathf.Abs(freeRectangles[i].height - width);
				int shortSideFit = Mathf.Min(leftoverHoriz, leftoverVert);
				int longSideFit = Mathf.Max(leftoverHoriz, leftoverVert);
				
				if (longSideFit < bestLongSideFit || (longSideFit == bestLongSideFit && shortSideFit < bestShortSideFit)) 
				{
					bestNode.x = freeRectangles[i].x;
					bestNode.y = freeRectangles[i].y;
					bestNode.width = height;
					bestNode.height = width;
					bestShortSideFit = shortSideFit;
					bestLongSideFit = longSideFit;
				}
			}
		}
		return bestNode;
	}
	
	PRRect FindPositionForNewNodeBestAreaFit(int width, int height, ref int bestAreaFit, ref int bestShortSideFit) 
	{
		PRRect bestNode = new PRRect();
		//memset(&bestNode, 0, sizeof(Rect));
		
		bestAreaFit = int.MaxValue;
		
		for(int i = 0; i < freeRectangles.Count; ++i) 
		{
			int areaFit = freeRectangles[i].width * freeRectangles[i].height - width * height;
			
			// Try to place the rectangle in upright (non-flipped) orientation.
			if (freeRectangles[i].width >= width && freeRectangles[i].height >= height) 
			{
				int leftoverHoriz = Mathf.Abs(freeRectangles[i].width - width);
				int leftoverVert = Mathf.Abs(freeRectangles[i].height - height);
				int shortSideFit = Mathf.Min(leftoverHoriz, leftoverVert);
				
				if (areaFit < bestAreaFit || (areaFit == bestAreaFit && shortSideFit < bestShortSideFit))
				{
					bestNode.x = freeRectangles[i].x;
					bestNode.y = freeRectangles[i].y;
					bestNode.width = width;
					bestNode.height = height;
					bestShortSideFit = shortSideFit;
					bestAreaFit = areaFit;
				}
			}
			
			if (allowRotations && freeRectangles[i].width >= height && freeRectangles[i].height >= width) 
			{
				int leftoverHoriz = Mathf.Abs(freeRectangles[i].width - height);
				int leftoverVert = Mathf.Abs(freeRectangles[i].height - width);
				int shortSideFit = Mathf.Min(leftoverHoriz, leftoverVert);
				
				if (areaFit < bestAreaFit || (areaFit == bestAreaFit && shortSideFit < bestShortSideFit)) 
				{
					bestNode.x = freeRectangles[i].x;
					bestNode.y = freeRectangles[i].y;
					bestNode.width = height;
					bestNode.height = width;
					bestShortSideFit = shortSideFit;
					bestAreaFit = areaFit;
				}
			}
		}
		return bestNode;
	}
	
	/// Returns 0 if the two intervals i1 and i2 are disjoint, or the length of their overlap otherwise.
	int CommonIntervalLength(int i1start, int i1end, int i2start, int i2end) 
	{
		if (i1end < i2start || i2end < i1start) return 0;

		return Mathf.Min(i1end, i2end) - Mathf.Max(i1start, i2start);
	}
	
	int ContactPointScoreNode(int x, int y, int width, int height) 
	{
		int score = 0;
		
		if (x == 0 || x + width == binWidth) score += height;

		if (y == 0 || y + height == binHeight) 	score += width;
		
		for(int i = 0; i < usedRectangles.Count; ++i) 
		{
			if (usedRectangles[i].x == x + width || usedRectangles[i].x + usedRectangles[i].width == x)
			{
				score += CommonIntervalLength(usedRectangles[i].y, usedRectangles[i].y + usedRectangles[i].height, y, y + height);
			}

			if (usedRectangles[i].y == y + height || usedRectangles[i].y + usedRectangles[i].height == y)
			{
				score += CommonIntervalLength(usedRectangles[i].x, usedRectangles[i].x + usedRectangles[i].width, x, x + width);
			}
		}
		return score;
	}
	
	PRRect FindPositionForNewNodeContactPoint(int width, int height, ref int bestContactScore) 
	{
		PRRect bestNode = new PRRect();
		//memset(&bestNode, 0, sizeof(Rect));
		
		bestContactScore = -1;
		
		for(int i = 0; i < freeRectangles.Count; ++i) 
		{
			// Try to place the rectangle in upright (non-flipped) orientation.
			if (freeRectangles[i].width >= width && freeRectangles[i].height >= height) 
			{
				int score = ContactPointScoreNode(freeRectangles[i].x, freeRectangles[i].y, width, height);
				if (score > bestContactScore) 
				{
					bestNode.x = freeRectangles[i].x;
					bestNode.y = freeRectangles[i].y;
					bestNode.width = width;
					bestNode.height = height;
					bestContactScore = score;
				}
			}

			if (allowRotations && freeRectangles[i].width >= height && freeRectangles[i].height >= width) 
			{
				int score = ContactPointScoreNode(freeRectangles[i].x, freeRectangles[i].y, height, width);
				if (score > bestContactScore) 
				{
					bestNode.x = freeRectangles[i].x;
					bestNode.y = freeRectangles[i].y;
					bestNode.width = height;
					bestNode.height = width;
					bestContactScore = score;
				}
			}
		}
		return bestNode;
	}
	
	bool SplitFreeNode(PRRect freeNode, ref PRRect usedNode) 
	{
		// Test with SAT if the rectangles even intersect.
		if (usedNode.x >= freeNode.x + freeNode.width || usedNode.x + usedNode.width <= freeNode.x ||
		    usedNode.y >= freeNode.y + freeNode.height || usedNode.y + usedNode.height <= freeNode.y)
			return false;
		
		if (usedNode.x < freeNode.x + freeNode.width && usedNode.x + usedNode.width > freeNode.x) 
		{
			// New node at the top side of the used node.
			if (usedNode.y > freeNode.y && usedNode.y < freeNode.y + freeNode.height) 
			{

				PRRect newNode = new PRRect();
				newNode.x = freeNode.x;
				newNode.y = freeNode.y;
				newNode.width = freeNode.width;
				newNode.height = usedNode.y - newNode.y;
				freeRectangles.Add(newNode);
			}
			
			// New node at the bottom side of the used node.
			if (usedNode.y + usedNode.height < freeNode.y + freeNode.height)
			{
				PRRect newNode = new PRRect();
				newNode.x = freeNode.x;
				newNode.y = usedNode.y + usedNode.height;
				newNode.width = freeNode.width;
				newNode.height = freeNode.y + freeNode.height - (usedNode.y + usedNode.height);
				freeRectangles.Add(newNode);
			}
		}
		
		if (usedNode.y < freeNode.y + freeNode.height && usedNode.y + usedNode.height > freeNode.y) 
		{
			// New node at the left side of the used node.
			if (usedNode.x > freeNode.x && usedNode.x < freeNode.x + freeNode.width) 
			{
				PRRect newNode = new PRRect();
				newNode.x = freeNode.x;
				newNode.y = freeNode.y;
				newNode.width = usedNode.x - newNode.x;
				newNode.height = freeNode.height;
				freeRectangles.Add(newNode);
			}
			
			// New node at the right side of the used node.
			if (usedNode.x + usedNode.width < freeNode.x + freeNode.width) 
			{
				PRRect newNode = new PRRect();
				newNode.x = usedNode.x + usedNode.width;
				newNode.y = freeNode.y;
				newNode.width = freeNode.x + freeNode.width - (usedNode.x + usedNode.width);
				newNode.height = freeNode.height;
				freeRectangles.Add(newNode);
			}
		}
		
		return true;
	}
	
	void PruneFreeList() 
	{
		for(int i = 0; i < freeRectangles.Count; ++i)
		{
			for(int j = i+1; j < freeRectangles.Count; ++j) 
			{
				if (IsContainedIn(freeRectangles[i], freeRectangles[j])) 
				{
					freeRectangles.RemoveAt(i);
					--i;
					break;
				}

				if (IsContainedIn(freeRectangles[j], freeRectangles[i])) 
				{
					freeRectangles.RemoveAt(j);
					--j;
				}
			}
		}
	}


	
	bool IsContainedIn(PRRect a, PRRect b) 
	{
		return a.x >= b.x && a.y >= b.y 
			&& a.x+a.width <= b.x+b.width 
			&& a.y+a.height <= b.y+b.height;
	}
}
