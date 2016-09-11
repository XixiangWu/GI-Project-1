README

This is all written in Mac OSX 10.11.6, Unity 5.40f3, please tell me that if there has any mistake that caused by system.

------------------------------------------------------------------------------
Instruction:

keyboard: W -- Forward  S -- Back  A -- Rotate anti-clockwise  D -- Rotate clockwise
	  	Q -- Rotate around Y axis anti-clockwise
		E -- Rotate around Y axis clockwise

Born Position is fixed in 500 300 500.

------------------------------------------------------------------------------
Features:

The "DiamondSquareAlgorithmParameter.cs" included all the editable parameters.
You can change any numbers you as you wish to change the terrain generator.


-------------------------------------------------------------------------------
BUG:

1.
The shader could not work properly due to the wrongly coding part of finding 
quads. I'm very confused in this part. In my script, no matter how I change 
the vertices and triangles, the shader could not be implement correctly. Therefore,
I've commented most of the shader to ensure that the rest function of my game 
could work.

2. The rigid body property of gameobject "Plane" can work most of the time. However
when you try to use "Plane" to hit the terrain almost "perpendicularly", the plane will go through the terrain rather than bounce away.

-------------------------------------------------------------------------------
Reference:

The "PaintTerrain" method used in "MainScene.cs" is derived from this website:
http://answers.unity3d.com/questions/12835/how-to-automatically-apply-different-textures-on-t.html