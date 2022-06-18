using Microsoft.Xna.Framework;
using Vector2 = System.Numerics.Vector2;

using CnV.Draw;

//using Windows.UI.Core;
namespace CnV;
using static CnV.GameClient;
public class TextLayout
	{
		public   TextFormat format;
		internal Vector2    normalizedSpan;
		private  string     text;
		public   Vector2    ScaledSpan(float scale) => normalizedSpan * scale;

		public TextLayout(string text, TextFormat format)
		{
			this.text   = text;
			this.format = format;
			var size = bfont.MeasureFont(text);

			normalizedSpan.X = size.Width;  // *0.5f;
			normalizedSpan.Y = size.Height; // *0.5f;
		}

		internal void Draw(Vector2 c, float scale, Color color, int layer, float z)
		{
		// convert screen from pixels to world units
			

			var span = ScaledSpan(scale);
			if (format.horizontalAlignment == TextFormat.HorizontalAlignment.center)
				c.X -= span.X * 0.5f;
			else if (format.horizontalAlignment == TextFormat.HorizontalAlignment.right)
				c.X -= span.X;
			if (format.verticalAlignment == TextFormat.VerticalAlignment.center)
				c.Y -= span.Y * 0.5f;
			else if (format.verticalAlignment == TextFormat.VerticalAlignment.bottom)
				c.Y -= span.Y;


			draw.DrawString(bfont, text, c, scale, color, layer, z
								); // (c+span*0.5f).CToDepth() );
		}

		//internal void Draw(Vector2 c, Color color,  int layer = Layer.labelText)
		//{
		//	if (format.horizontalAlignment == TextFormat.HorizontalAlignment.center)
		//		c.X -= span.X * 0.5f;
		//	else if (format.horizontalAlignment == TextFormat.HorizontalAlignment.right)
		//		c.X -= span.X;
		//	else if (format.verticalAlignment == TextFormat.VerticalAlignment.center)
		//		c.Y -= span.Y * 0.5f;
		//	else if (format.verticalAlignment == TextFormat.VerticalAlignment.bottom)
		//		c.Y -= span.Y;


		//	AGame.draw.DrawString(AGame.font, text, c, color, layer,  (c+span*0.5f).CToDepth() );
		//}
	}


