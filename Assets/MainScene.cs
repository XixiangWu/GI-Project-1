using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MainScene : MonoBehaviour {

	private Terrain _terrain;  	// Current terrain
	private float?[,] _heightArray;		// nullable array
	private float[,] _heightArraySafe;  // non-nullable array

	// DSA Generator
	private DiamondSquareAlgorithmGenerator DSAGenerator;
	private DiamondSquareParameters DSAParameters;
	public Shader shader;
	public Sun sun;

	private Material material;


	private void Start () {

		// INIT: init Diamond Square Algorithm Generator
		DSAGenerator = new DiamondSquareAlgorithmGenerator (new DiamondSquareParameters());
		DSAParameters = new DiamondSquareParameters ();

		// INIT: connect to current active terrain 
		_terrain = Terrain.activeTerrain;

		// INIT: 2d height map array of Terrain
		initHeightArray();

		// INIT: Start Terrain
		initTerrain();

		// INIT: Shader (not fixed yet)
		// initPhongShader ();

		// INIT: terrain texture
		PaintTerrain (_heightArraySafe);



		// TEST: reset whole terrain
		//printHeight ();
		//resetTerrain ();

	}


	// Update is called once per frame
	private void Update () {

		// Render (not fixed yet)
  		//material.SetColor("_PointLightColor", sun.GetColor());
  		//material.SetVector("_PointLightPosition", sun.GetPosition());
	}

	private void initHeightArray() {
		_heightArray = DSAGenerator.getHeightArray();
		_heightArraySafe = new float[DSAParameters.heightMapWidthX, DSAParameters.heightMapWidthY];
	}

	private void initTerrain() {

		if (getHeightArraySafe()) {
			_terrain.terrainData.SetHeights (0, 0, _heightArraySafe);
		} else {
			_terrain.terrainData.SetHeights (0, 0, _heightArraySafe);
			Debug.Log ("Map loading error :(");
		}	
	}

	private bool getHeightArraySafe() {

		for (int i = 0; i<(DSAParameters.heightMapWidthX-1); i++) {
			for (int j = 0; j < (DSAParameters.heightMapWidthY-1); j++) {
				if (_heightArray [i, j].HasValue) {
					_heightArraySafe [i, j] = _heightArray [i, j].Value;
				} else {
//					return false; // some values in height array has not been assigned
					_heightArraySafe [i, j] = 0.0f;
				}
			}
		}
		return true;
	}


	// TEST:

	private void resetTerrain() { // For testing
		for (int i = 0; i < DSAParameters.heightMapWidthX; i++) {
			for (int j = 0; j < DSAParameters.heightMapWidthY; j++) {
				_heightArraySafe [i, j] = 0.0f;
			}
		}
		_terrain.terrainData.SetHeights (0, 0, _heightArraySafe);
	}

	private void printHeight(int nIteration) {
		for (int i = 0; i<DSAParameters.heightMapWidthX; i++) {
			for (int j = 0; j < DSAParameters.heightMapWidthY; j++) {
				Debug.Log ("x: " + i + " y:" + j + " height: " + _heightArraySafe [i, j]);
			}
		}
	}


	protected void PaintTerrain(float[,] heights)
	{
		// The structure that holds all the splat-related data
		float[, ,] splatmapData = new float[_terrain.terrainData.alphamapWidth, _terrain.terrainData.alphamapHeight, _terrain.terrainData.alphamapLayers];

		// The splat map is always 512 x 512, so we need to scale that to whatever resolution our terrain is
		float xCorrection = (heights.GetLength(0) - 1f) / _terrain.terrainData.alphamapWidth;
		float yCorrection = (heights.GetLength(1) - 1f) / _terrain.terrainData.alphamapHeight;

		// The height interval that each texture represents (alphamapLayers = number of textures)
		float textureInterval = 1f / (_terrain.terrainData.alphamapLayers - 1);

		// A number that represents which textures and which blending weights are present at a particular step
		float textureBlend;

		// The texture ID that corresponds to a particular height
		int textureId;

		// Represents (1 - blending weight of the current texture), also blending weight of the next texture
		float textureWeight;

		// The array that holds the blend weight for each texture
		float[] splat;

		// Iteration variables
		int z, y, x;
		for (y = 0; y < _terrain.terrainData.alphamapHeight; y++)
		{
			for (x = 0; x < _terrain.terrainData.alphamapWidth; x++)
			{
				splat = new float[_terrain.terrainData.alphamapLayers];

				textureBlend = Mathf.Clamp01(heights[(int)(x * xCorrection), (int)(y * yCorrection)]) / textureInterval;
				textureId = (int)textureBlend;
				textureWeight = textureBlend - textureId;

				if (textureWeight < 0.2f)
				{
					textureWeight = 0f;
				}
				else
				{
					textureWeight = Mathf.InverseLerp(0.2f, 1f, textureWeight);
				}

				splat[textureId] = 1.0f - textureWeight;
				if (textureId < _terrain.terrainData.alphamapLayers - 1)
				{
					splat[textureId + 1] = textureWeight;
				}

				// now assign the values to the correct location in the array
				for (z = 0; z < _terrain.terrainData.alphamapLayers; ++z)
				{
					splatmapData[(int)(x), (int)(y), z] = splat[z];
				}
			}
		}

		// Assign the splat map to the terrain data
		_terrain.terrainData.SetAlphamaps(0, 0, splatmapData);
	}

	private void initPhongShader() {
		
		Mesh mesh = CreateTerrainMesh ();
		gameObject.AddComponent<MeshFilter>().mesh = mesh;
			
		this.material = gameObject.AddComponent<MeshRenderer>().material;
		this.material.shader = shader;
	}


	Mesh CreateTerrainMesh() {
		
		Mesh m = new Mesh();
		m.name = "terrain";

		List<Vector3> vertices = new List<Vector3>();
		List<int> triangles = new List<int>();

		// use quads 
		for (int x = 0; x < (DSAParameters.heightMapWidthX-1); x++) {
			for (int z = 0; z < (DSAParameters.heightMapWidthY-1); z++) {
				vertices.Add(new Vector3(x, _heightArraySafe[x,z], z));
				vertices.Add(new Vector3(x + 1, _heightArraySafe[x+1,z], z));
				vertices.Add(new Vector3(x + 1, _heightArraySafe[x+1,z+1], z + 1));
				vertices.Add(new Vector3(x, _heightArraySafe[x,z+1], z + 1));
			}
		}

		m.Clear ();
		m.vertices = vertices.ToArray();
		m.triangles = triangles.ToArray();
		m.Optimize ();
		m.RecalculateNormals();

		return m;
	}


}
