/*  DiamondSquareAlgorithmGenerator
	It is used for generating the heighmap of the Terrain. All the parameters(magic numbers) that
	have been used in this script came from the DiamondSquareParameter.cs

	The Diamond Square Algorithm is implemented in this script.

	author: Xixiang Wu    studentID: 731942
*/

using UnityEngine;
using System.Collections;
using System;


public class DiamondSquareAlgorithmGenerator {

	// Terrain parameters
	private DiamondSquareParameters _parameters;

	// Terrain height array
	private float?[,] heightArray;

	// generate 4 random seeds for Square step
	private int[] seeds = new int[4]; 
	private float[] _seedsNoise = new float[4]; 

	// DSAParameters
	private int _heightMapHoriMax;
	private int _heightMapVertMax;
	private int _heightMax;
	private int _xBoundaryMin;
	private int _xBoundaryMax;
	private int _yBoundaryMin;
	private int _yBoundaryMax;
	private float _smoothness;
	private float _variation;
	private float _noiseReduce;

	// iteration times of DSA
	private int nIterations;

	// a max parameter for DSA
	private int max;


	public DiamondSquareAlgorithmGenerator(DiamondSquareParameters parameters) {
		
		// INIT: parametes
		_parameters = parameters;
		_heightMax = _parameters.heightMax;
		_heightMapHoriMax = _parameters.heightMapWidthX - 1;
		_heightMapVertMax = _parameters.heightMapWidthY - 1;
		_smoothness = _parameters.smoothness;
		_variation = _parameters.variation;
		_seedsNoise = _parameters.seeds;
		_noiseReduce = _parameters._noiseReduce;

		// INIT: DSA
		heightArray = new float?[_heightMapHoriMax+1, _heightMapVertMax+1];
		nIterations = getIterationTimes ();
		max = _heightMapHoriMax;
		initBoundary ();


		// INIT: 2d array
		performDSA(); // performDiamondSquareAlgorithm()

	}

	// Generate 4 random numbers for the height of corners.
	public void getSeeds()
	{
		System.Random rd = new System.Random();

		for (int i = 0; i < 4; i++) {
			seeds [i] = rd.Next (100000) % _heightMax + 1;
		}
	}

	// The size of the map is 2^nIterations+1
	private int getIterationTimes() {
		return (int)Mathf.Log(_heightMapHoriMax,2);
	}

	// get height array from other classes
	public float?[,] getHeightArray() {
		if (heightArray != null) {
			return heightArray;
		}
		return null;
	}

	private void performDSA() {

		// 4 random seeds for corners;
		getSeeds ();
		heightArray [0, 0] = _seedsNoise[0] * seeds [0] / (float)_heightMax;
		heightArray [0,_heightMapVertMax] = _seedsNoise[1] * seeds[1] / (float)_heightMax;
		heightArray [_heightMapHoriMax, 0] = _seedsNoise[2] * seeds[2] / (float)_heightMax;
		heightArray [_heightMapHoriMax, _heightMapVertMax] = _seedsNoise[3] * seeds[3] / (float)_heightMax;

		int stackX;
		int stackY;
		int diffTemp;

		// Iterate n times to fill in every cell
		for (int i = 0; i < nIterations; i++) {

			// prepare the paramenters
			// x: the horizontal start position 
			// y: the vertical start position
			// diffTemp: the temporary difference between each node in matrix in this stage
			stackX = getHalf (max);
			stackY = getHalf (max);
			diffTemp = getHalf (max);


			// Diamond
			diamond(stackX,stackY,diffTemp);

			// Square
			square(stackX,stackY,diffTemp);

			// Random Number Reduced by a coefficient
			reduceNoise();

			// half the integer
			max = getHalf (max);
		}
	}


	// diamond step of DSA
	private void diamond(int stackX, int stackY, int diffTemp) {

		stackX = getHalf(max);
		stackY = getHalf(max);

		float? heightTemp = 0.0f;

		while (stackY < _heightMapHoriMax) {

			while (stackX < _heightMapVertMax) {

				heightTemp += heightArray [stackX - diffTemp, stackY - diffTemp]; // top-left
				heightTemp += heightArray [stackX - diffTemp, stackY + diffTemp]; // top-right
				heightTemp += heightArray [stackX + diffTemp, stackY - diffTemp]; // bottom-left
				heightTemp += heightArray [stackX + diffTemp, stackY + diffTemp]; // bottom-right

				heightArray [stackX, stackY] = heightTemp/4.0f + Noise;
				heightTemp = 0;

				stackX += max;
			}

			stackX = getHalf(max);
			stackY += max;
		}
	}

	// square step of DSA
	private void square(int stackX, int stackY, int diffTemp) {

		while (stackY < _heightMapHoriMax) {

			while (stackX < _heightMapVertMax) {

				// top [x,y-diff]
				if (!heightArray [stackX, stackY-diffTemp].HasValue) {
					heightArray [stackX, stackY-diffTemp] = getValueInSquareStep (stackX, stackY-diffTemp, diffTemp)+ Noise;
				}
				// right [x+diff,y]
				if (!heightArray [stackX+diffTemp, stackY].HasValue) {
					heightArray [stackX+diffTemp, stackY] = getValueInSquareStep (stackX+diffTemp, stackY, diffTemp)+ Noise;
				}
				// bottom [x,y+diff]
				if (!heightArray [stackX, stackY+diffTemp].HasValue) {
					heightArray [stackX, stackY+diffTemp] = getValueInSquareStep (stackX, stackY+diffTemp, diffTemp)+ Noise;
				}
				// left [x-diff,y]
				if (!heightArray [stackX-diffTemp, stackY].HasValue) {
					heightArray [stackX-diffTemp, stackY] = getValueInSquareStep (stackX-diffTemp, stackY, diffTemp)+ Noise;
				}


				stackX += max;
			}

			stackX = getHalf(max);
			stackY += max;
		}
	}

	// To avoid array index out of range and null element in array, use this function
	// to do the value addition
	private float getValueInSquareStep(int x, int y, int diff) {

		float heightTemp = 0.0f;
		int counter = 0;

		if ((x-diff) >= _xBoundaryMin) {
			heightTemp += heightArray [x - diff, y].Value;
			counter++;
		}
		if ((x + diff) <= _xBoundaryMax) {
			heightTemp += heightArray [x + diff, y].Value;
			counter++;
		}
		if ((y - diff) >= _yBoundaryMin) {
			heightTemp += heightArray [x, y - diff].Value;
			counter++;
		}
		if ((y + diff) <= _yBoundaryMax) {
			heightTemp += heightArray [x, y + diff].Value;
			counter++;
		}
			
		return heightTemp/counter;
	}

	// return half of the given number (cast to int)
	private int getHalf(int x) {
		return (int)(x / 2);
	}

	// boundary detection to prevent index out of range
	private void initBoundary() {
		_xBoundaryMin = _parameters.mapStartPointX;
		_xBoundaryMax = _heightMapHoriMax;
		_yBoundaryMin = _parameters.mapStartPointY;
		_yBoundaryMax = _heightMapVertMax;
	}

	// DSA noise
	private float Noise {
		get { return UnityEngine.Random.Range (-_variation, _variation) / _noiseReduce; }
	}

	// noise decrease rate
	private void reduceNoise() {
		_variation *= Mathf.Pow(2, -_smoothness);
	}


}
