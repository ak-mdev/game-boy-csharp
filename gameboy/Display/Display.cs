using System;
using OpenTK;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK.Input;

namespace GameBoy.Graphics
{
	public class Display : GameWindow
	{
		public readonly Color[] PALETTE = new Color[4] {
			new Color (255, 255, 255),
			new Color (192, 192, 192),
			new Color (96, 96, 96),
			new Color (0, 0, 0)
		};

		private uint vao = 0;
		private uint vbo = 0;
		private readonly float[] VERTEX_BUFFER_DATA = new float[] {
			-1.0f, -1.0f, 0.0f,
			1.0f, -1.0f, 0.0f,
			-1.0f,  1.0f, 0.0f,
			-1.0f,  1.0f, 0.0f,
			1.0f, -1.0f, 0.0f,
			1.0f,  1.0f, 0.0f
		};
		private int program = 0;
		private int texture = 0;
		private int time = 0;
		private uint fbo = 0;
		private uint renderedTexture = 0;

		public Color[] FrameBuffer { get; set; }

		public byte Control { get; set; }

		public byte ScrollX { get; set; }

		public byte ScrollY { get; set; }

		public byte Scanline { get; set; }

		public byte[,,] Tiles { get; set; }

		public Color[] BackgroundPalette { get; set; }

		public Color[][] SpritePalette { get; set; }

		public Display ()
		{
			FrameBuffer = new Color[160 * 144];
			Tiles = new byte[384, 8, 8];
			BackgroundPalette = new Color[4];
			SpritePalette = new Color[2][];
			for (int i = 0; i < SpritePalette.Length; i++) {
				SpritePalette [i] = new Color[4];
			}
		}

		public void Reset ()
		{
			Array.Clear (Tiles, 0, Tiles.Length);
			BackgroundPalette [0] = PALETTE [0];
			BackgroundPalette [1] = PALETTE [1];
			BackgroundPalette [2] = PALETTE [2];
			BackgroundPalette [3] = PALETTE [3];

			SpritePalette [0] [0] = PALETTE [0];
			SpritePalette [0] [1] = PALETTE [1];
			SpritePalette [0] [2] = PALETTE [2];
			SpritePalette [0] [3] = PALETTE [3];

			SpritePalette [1] [0] = PALETTE [0];
			SpritePalette [1] [1] = PALETTE [1];
			SpritePalette [1] [2] = PALETTE [2];
			SpritePalette [1] [3] = PALETTE [3];

			Control = 0;
			ScrollX = 0;
			ScrollY = 0;
			Scanline = 0;
		}

		public void RenderScanline (byte[] vram, byte[] oam)
		{
			var mapOffset = ((Control & (byte)GpuControl.Tilemap) != 0) ? 0x1c00 : 0x1800;
			mapOffset += (((Scanline + ScrollY) & 255) >> 3) << 5;

			var lineOffset = ScrollX >> 3;

			var x = ScrollX & 7;
			var y = (Scanline + ScrollY) & 7;

			var pixelOffset = Scanline * 160;

			var tile = (ushort)vram [mapOffset + lineOffset];

			var scanlineRow = new byte[160];

			for (var i = 0; i < 160; i++) {
				var color = Tiles [tile, y, x];
				scanlineRow [i] = color;

				FrameBuffer [pixelOffset].R = BackgroundPalette [color].R;
				FrameBuffer [pixelOffset].G = BackgroundPalette [color].G;
				FrameBuffer [pixelOffset].B = BackgroundPalette [color].B;

				x++;
				if (x == 8) {
					x = 0;
					lineOffset = (lineOffset + 1) & 31;
					tile = vram [mapOffset + lineOffset];
				}
			}

			var sprites = new List<Sprite> (Sprite.FromBytes (oam));

			for (int i = 0; i < 40; i++) {
				var sprite = sprites [i];

				if (sprite == null)
					break;

				int sx = sprite.X - 8;
				int sy = sprite.Y - 16;

				if (sy <= Scanline && (sy + 8) > Scanline) {
					var pal = SpritePalette [sprite.Options.Palette];

					pixelOffset = Scanline * 160 + sx;

					var tileRow = (byte)(sprite.Options.VFlip != 0 ? (7 - (Scanline - sy)) : (Scanline - sy));

					for (x = 0; x < 8; x++) {
						if (sx + x >= 0 && sx + x < 160 && (~sprite.Options.Priority != 0 || scanlineRow [sx + x] == 0)) {
							var color = sprite.Options.HFlip != 0 ? Tiles [sprite.Tile, tileRow, 7 - x] : Tiles [sprite.Tile, tileRow, x];

							if (color != 0) {
								FrameBuffer [pixelOffset].R = pal [color].R;
								FrameBuffer [pixelOffset].G = pal [color].G;
								FrameBuffer [pixelOffset].B = pal [color].B;
							}
							pixelOffset++;
						}
					}
				}
			}
		}

		public void DrawFrameBuffer ()
		{
			GL.Clear (ClearBufferMask.ColorBufferBit);

			GL.BindFramebuffer (FramebufferTarget.Framebuffer, fbo);
			GL.Viewport (0, 0, Width, Height);

			SwapBuffers ();
		}

		protected override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);

			Title = "Tetris";
			WindowBorder = WindowBorder.Fixed;
			GL.ClearColor (System.Drawing.Color.CornflowerBlue);

			GL.GenVertexArrays (1, out vao);
			GL.BindVertexArray (vao);

			GL.GenBuffers (1, out vbo);
			GL.BindBuffer (BufferTarget.ArrayBuffer, vbo);
			GL.BufferData<float>(BufferTarget.ArrayBuffer, new IntPtr(VERTEX_BUFFER_DATA.Length * sizeof(float)), VERTEX_BUFFER_DATA, BufferUsageHint.StaticDraw);

			// Create and compile our GLSL program from the shaders
			program = Shader.Load("vertex.glsl", "fragment.glsl");
			texture = GL.GetUniformLocation(program, "renderedTexture");
			time = GL.GetUniformLocation (program, "time");

			GL.GenFramebuffers (1, out fbo);
			GL.BindFramebuffer (FramebufferTarget.Framebuffer, fbo);
			GL.GenTextures (1, out renderedTexture);

			GL.BindTexture (TextureTarget.Texture2D, renderedTexture);
			GL.TexImage2D (TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, 1024, 768, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
			GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, renderedTexture, 0);

			var drawBuffers = new DrawBuffersEnum[] { DrawBuffersEnum.ColorAttachment0 };
			GL.DrawBuffers (1, drawBuffers);

			var status = GL.CheckFramebufferStatus (FramebufferTarget.Framebuffer);
			if (status != FramebufferErrorCode.FramebufferComplete) {
				Console.WriteLine ("ERROR: Framebuffer creation failed ({0})", status.ToString ());
				Program.Quit ();
			}

		}
	}
}

