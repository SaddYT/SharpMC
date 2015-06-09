﻿// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// 
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// ©Copyright Kenny van Vulpen - 2015

using SharpMC.Core.Blocks;
using SharpMC.Core.Entity;
using SharpMC.Core.Utils;
using SharpMC.Core.Worlds;

namespace SharpMC.Core.Networking.Packages
{
	internal class BlockChange : Package<BlockChange>
	{
		public int BlockId;
		public Vector3 Location;
		public int MetaData;

		public BlockChange(ClientWrapper client)
			: base(client)
		{
			SendId = 0x23;
		}

		public BlockChange(ClientWrapper client, DataBuffer buffer)
			: base(client, buffer)
		{
			SendId = 0x23;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WritePosition(Location);
				Buffer.WriteVarInt(BlockId << 4 | MetaData);
				Buffer.FlushData();
			}
		}

		public static void Broadcast(Block block, Level level, bool self = true, Player source = null)
		{
			lock (level.OnlinePlayers)
			{
				foreach (var i in level.OnlinePlayers.ToArray())
				{
					if (!self && i == source)
					{
						continue;
					}
					//Client = i.Wrapper;
					//Buffer = new DataBuffer(i.Wrapper);
					//_stream = i.Wrapper.TCPClient.GetStream();
					//Write();
					new BlockChange(i.Wrapper)
					{
						BlockId = block.Id,
						MetaData = block.Metadata,
						Location = block.Coordinates
					}.Write();
				}
			}
		}
	}
}