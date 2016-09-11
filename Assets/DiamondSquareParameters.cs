using UnityEngine;
using System.Collections;

public class DiamondSquareParameters
{
	// The parameters are used for generating Terrain
	public float[] seeds = { 0.5f, 0.5f, 0.5f, 0.5f }; // The random scale of height of four corners
	public int heightMax = 300;			// map max height	
	public int heightMapWidthX = 513;	// 0-512  513 pixels
	public int heightMapWidthY = 513;	// 0-512  513 pixels
	public int mapStartPointX = 0;		// 
	public int mapStartPointY = 0;		//
	public float smoothness = 0.85f;	// higher value of smoothness will generate a flatter terrain
	public float variation = 100.0f;	// Noise
	public float _noiseReduce = 500.0f;	// a default value
}
