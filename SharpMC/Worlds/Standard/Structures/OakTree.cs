﻿#region Header

// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// ©Copyright Kenny van Vulpen - 2015
#endregion

namespace SharpMC.Worlds.Standard.Structures
{
	using SharpMC.Blocks;
	using SharpMC.Utils;

	internal class OakTree : Structure
	{
		private readonly int LeafRadius = 2;

		public override string Name
		{
			get
			{
				return "OakTree";
			}
		}

		public override int Height
		{
			get
			{
				return 10;
			}
		}

		// Enable to use vanilla tree's
		/*
		public override void Create(ChunkColumn chunk, int x, int y, int z)
		{
			var location = new Vector3(x, y, z);
			if (!ValidLocation(new Vector3(x, y, z))) return;

			Random R = new Random();
			int Height = R.Next(4, 5);
			GenerateColumn(chunk, location, Height, new Block(17));
			Vector3 LeafLocation = location + new Vector3(0, Height, 0);
			GenerateVanillaLeaves(chunk, LeafLocation, LeafRadius, new Block(18));
			
		}*/
		public override Block[] Blocks
		{
			get
			{
				return new[]
					       {
						       new Block(17) { Coordinates = new Vector3(0, 0, 0) }, new Block(17) { Coordinates = new Vector3(0, 1, 0) }, 
						       new Block(17) { Coordinates = new Vector3(0, 2, 0) }, new Block(17) { Coordinates = new Vector3(0, 3, 0) }, 
						       new Block(17) { Coordinates = new Vector3(0, 4, 0) }, new Block(17) { Coordinates = new Vector3(0, 5, 0) }, 
						       new Block(18) { Coordinates = new Vector3(-1, 4, 1) }, 
						       new Block(18) { Coordinates = new Vector3(1, 4, -1) }, 
						       new Block(18) { Coordinates = new Vector3(-1, 4, -1) }, 
						       new Block(18) { Coordinates = new Vector3(1, 4, 1) }, 
						       new Block(18) { Coordinates = new Vector3(-1, 4, 0) }, 
						       new Block(18) { Coordinates = new Vector3(1, 4, 0) }, 
						       new Block(18) { Coordinates = new Vector3(0, 4, -1) }, 
						       new Block(18) { Coordinates = new Vector3(0, 4, 1) }, 
						       new Block(18) { Coordinates = new Vector3(-1, 5, 0) }, 
						       new Block(18) { Coordinates = new Vector3(1, 5, 0) }, 
						       new Block(18) { Coordinates = new Vector3(0, 5, -1) }, 
						       new Block(18) { Coordinates = new Vector3(0, 5, 1) }, new Block(18) { Coordinates = new Vector3(0, 6, 0) }
					       };
			}
		}

		public bool ValidLocation(Vector3 location)
		{
			if (location.X - this.LeafRadius < 0 || location.X + this.LeafRadius >= 16 || location.Z - this.LeafRadius < 0
			    || location.Z + this.LeafRadius >= 256)
			{
				return false;
			}

			return true;
		}
	}
}