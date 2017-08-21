﻿//
//  Buffer.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2016 Jarl Gullberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace Everlook.Viewport.Rendering.Core
{
	/// <summary>
	/// Represents a native OpenGL data buffer.
	/// </summary>
	/// <typeparam name="T">Any structure.</typeparam>
	public sealed class Buffer<T> : IDisposable where T : struct
	{
		private readonly int NativeBufferID;
		private int DataSize;

		/// <summary>
		/// Gets the intended use of the buffer.
		/// </summary>
		public BufferTarget Target { get; }

		/// <summary>
		/// Gets a hinting value as to how the buffer's data might be read or written.
		/// </summary>
		public BufferUsageHint UsageHint { get; }

		/// <summary>
		/// Gets or sets the data contained in the buffer. This is an expensive operation.
		/// </summary>
		public T[] Data
		{
			get
			{
				Bind();

				T[] bufferData = new T[this.DataSize / Marshal.SizeOf<T>()];
				GL.GetBufferSubData(this.Target, IntPtr.Zero, this.DataSize, bufferData);

				return bufferData;
			}

			set
			{
				Bind();

				this.DataSize = value.Length * Marshal.SizeOf<T>();
				GL.BufferData(this.Target, this.DataSize, value, this.UsageHint);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Buffer{T}"/> class.
		/// </summary>
		/// <param name="target">The intended use of the buffer.</param>
		/// <param name="usageHint">A hint as to how the buffer's data might be read or written.</param>
		public Buffer(BufferTarget target, BufferUsageHint usageHint)
		{
			this.Target = target;
			this.UsageHint = usageHint;

			this.NativeBufferID = GL.GenBuffer();
		}

		/// <summary>
		/// Binds the buffer as the current OpenGL object, making it available for use.
		/// </summary>
		public void Bind()
		{
			GL.BindBuffer(this.Target, this.NativeBufferID);
		}

		/// <summary>
		/// Disposes the buffer, deleting the underlying OpenGL object.
		/// </summary>
		public void Dispose()
		{
			GL.DeleteBuffer(this.NativeBufferID);
		}
	}
}