namespace CnV;

using System.Numerics;

public struct TextFormat
{
	public enum VerticalAlignment
	{
		top,
		center,
		bottom,
	}
	public enum HorizontalAlignment
	{
		left,
		center,
		right,
	}
	public HorizontalAlignment horizontalAlignment;
	public VerticalAlignment   verticalAlignment;
	public Vector2             scale;

	public TextFormat(HorizontalAlignment _horizontalAlignment = HorizontalAlignment.left, VerticalAlignment _verticalAlignment = VerticalAlignment.top)
	{
		horizontalAlignment = _horizontalAlignment;
		verticalAlignment   = _verticalAlignment;
		scale               = new Vector2(1, 1);

	}

}