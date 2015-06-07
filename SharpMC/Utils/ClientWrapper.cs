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

namespace SharpMC.Utils
{
	using System.Collections.Generic;
	using System.Net.Sockets;
	using System.Security.Cryptography;
	using System.Text;
	using System.Threading;
	using System.Timers;

	using SharpMC.Entity;
	using SharpMC.Enums;
	using SharpMC.Networking.Packages;

	using Timer = System.Timers.Timer;

	public class ClientWrapper
	{
		private readonly Queue<byte[]> _commands = new Queue<byte[]>();

		private readonly Timer _kTimer = new Timer();

		private readonly AutoResetEvent _resume = new AutoResetEvent(false);

		private readonly Timer _tickTimer = new Timer();

		internal bool EncryptionEnabled = false;

		public PacketMode PacketMode = PacketMode.Ping;

		public Player Player;

		public TcpClient TcpClient;

		public MyThreadPool ThreadPool;

		public ClientWrapper(TcpClient client)
		{
			this.TcpClient = client;
			if (client != null)
			{
				this.ThreadPool = new MyThreadPool();
				this.ThreadPool.LaunchThread(this.ThreadRun);

				var bytes = new byte[8];
				Globals.Rand.NextBytes(bytes);
				this.ConnectionId = Encoding.ASCII.GetString(bytes).Replace("-", string.Empty);
			}
		}

		internal byte[] SharedKey { get; set; }

		internal ICryptoTransform Encrypter { get; set; }

		internal ICryptoTransform Decrypter { get; set; }

		internal string ConnectionId { get; set; }

		internal string Username { get; set; }

		public void AddToQuee(byte[] data, bool quee = false)
		{
			if (this.TcpClient != null)
			{
				if (quee)
				{
					lock (this._commands)
					{
						this._commands.Enqueue(data);
					}

					this._resume.Set();
				}
				else
				{
					this.SendData(data);
				}
			}
		}

		private void ThreadRun()
		{
			while (this._resume.WaitOne())
			{
				byte[] command;
				lock (this._commands)
				{
					command = this._commands.Dequeue();
				}

				this.SendData(command);
			}
		}

		public void SendData(byte[] data)
		{
			if (this.TcpClient != null)
			{
				try
				{
					if (this.Encrypter != null)
					{
						var toEncrypt = data;
						data = new byte[toEncrypt.Length];
						this.Encrypter.TransformBlock(toEncrypt, 0, toEncrypt.Length, data, 0);

						var a = this.TcpClient.GetStream();
						a.Write(data, 0, data.Length);
						a.Flush();
					}
					
					/*	if (EncryptionEnabled)
				{
					AesStream aes = new AesStream(TcpClient.GetStream(), (byte[])SharedKey.Clone());
					aes.Write(data, 0, data.Length);
					aes.Flush();
				}*/
					else
					{
						var a = this.TcpClient.GetStream();
						a.Write(data, 0, data.Length);
						a.Flush();
					}
				}
				catch
				{
					ConsoleFunctions.WriteErrorLine("Failed to send a packet!");
				}
			}
		}

		public void StartKeepAliveTimer()
		{
			this._kTimer.Elapsed += this.DisplayTimeEvent;
			this._kTimer.Interval = 5000;
			this._kTimer.Start();

			this._tickTimer.Elapsed += this.DoTick;
			this._tickTimer.Interval = 50;

			// _tickTimer.Start();
		}

		public void StopKeepAliveTimer()
		{
			this._kTimer.Stop();
		}

		public void DisplayTimeEvent(object source, ElapsedEventArgs e)
		{
			new KeepAlive(this.Player.Wrapper).Write();
		}

		public void DoTick(object source, ElapsedEventArgs e)
		{
			// if (Player != null)
			// {
			// 	Player.OnTick();
			// }
		}
	}
}