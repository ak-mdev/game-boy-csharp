using System;
using OpenTK.Graphics.OpenGL;
using System.IO;

namespace GameBoy.Graphics
{
	public static class Shader
	{
		public static int Load (string vertexFileName, string fragmentFileName)
		{
			// Create the shaders
			var vertexShader = GL.CreateShader (ShaderType.VertexShader);
			var fragmentShader = GL.CreateShader (ShaderType.FragmentShader);

			// load vertex
			var vertexShaderSource = File.ReadAllText (vertexFileName);
			// load fragment
			var fragmentShaderSource = File.ReadAllText (fragmentFileName);

			Debug.WriteLine ("Compiling shader: {0}", vertexFileName);
			GL.ShaderSource (vertexShader, vertexShaderSource);
			GL.CompileShader (vertexShader);
			Debug.WriteLine (GL.GetShaderInfoLog (vertexShader));

			Debug.WriteLine ("Compiling shader: {0}", fragmentFileName);
			GL.ShaderSource (fragmentShader, fragmentShaderSource);
			GL.CompileShader (fragmentShader);
			Debug.WriteLine (GL.GetShaderInfoLog (fragmentShader));

			Debug.WriteLine ("Linking program");
			var program = GL.CreateProgram ();
			GL.AttachShader (program, vertexShader);
			GL.AttachShader (program, fragmentShader);
			GL.LinkProgram (program);
			Debug.WriteLine (GL.GetProgramInfoLog (program));
			GL.UseProgram (program);

			GL.DetachShader (program, vertexShader);
			GL.DetachShader (program, fragmentShader);

			GL.DeleteShader (vertexShader);
			GL.DeleteShader (fragmentShader);

			Debug.WriteLine ("Done loading shaders into program {0}", program);

			return program;
		}
	}
}

