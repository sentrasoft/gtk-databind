
using System;

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// Same as cairo rectangle but less consuming on allocation since
	/// values can be modified
	/// </summary>
	public class CellRectangle
	{
		private double x = 0.0;
		public double X {
			get { return (x); }
			set {
				if (x == value)
					return;
				x = value;
			}
		}
		
		private double y = 0.0;
		public double Y {
			get { return (y); }
			set {
				if (y == value)
					return;
				y = value;
			}
		}
		
		private double width = -1;
		public double Width {
			get { return (width); }
			set {
				if (width == value)
					return;
				width = value;
			}
		}
		
		private double height = -1;
		public double Height {
			get { return (height); }
			set {
				if (height == value)
					return;
				height = value;
			}
		}
		
		public double Left {
			get { return (x); }
		}
		
		public double Top {
			get { return (y); }
		}
		
		public double Right {
			get { return (x+width); }
		}
		
		public double Bottom {
			get { return (y+height); }
		}
		
		public bool IsEmpty {
			get { return ((width <= 0) || (height <= 0)); }
		}
		
	    public void DrawRoundedRectangle (Cairo.Context aContext, double aRadius)
	    {
			if ((aRadius > height / 2) || (aRadius > width / 2))
			    aRadius = Math.Min (height / 2, width / 2);
			
			aContext.MoveTo (x, y + aRadius);
			aContext.Arc (x + aRadius, y + aRadius, aRadius, Math.PI, -Math.PI / 2);
			aContext.LineTo (x + width - aRadius, y);
			aContext.Arc (x + width - aRadius, y + aRadius, aRadius, -Math.PI / 2, 0);
			aContext.LineTo (x + width, y + height - aRadius);
			aContext.Arc (x + width - aRadius, y + height - aRadius, aRadius, 0, Math.PI / 2);
			aContext.LineTo (x + aRadius, y + height);
			aContext.Arc (x + aRadius, y + height - aRadius, aRadius, Math.PI / 2, Math.PI);
			aContext.ClosePath ();
	    }
    
		public void DrawPath (Cairo.Context aContext)
		{
			aContext.MoveTo (Left, Top);
			aContext.LineTo (Left, Bottom);
			aContext.LineTo (Right, Bottom);
			aContext.LineTo (Right, Top);
			aContext.LineTo (Left, Top);
			aContext.ClosePath();
		}
		
		public void Clip (Cairo.Context aContext)
		{
			DrawPath (aContext);
			aContext.Clip();
		}
		
		/// <summary>
		/// Checks if rectangle is inside of clipping area
		/// </summary>
		/// <param name="aClippingArea">
		/// Cllipping rectangle <see cref="CellRectangle"/>
		/// </param>
		/// <returns>
		/// true if area intersects with clipping area, false if not <see cref="System.Boolean"/>
		/// </returns>
		public bool IsInsideArea (CellRectangle aClippingArea)
		{
			if (Left > aClippingArea.Right)
				return (false);
			if (Right < aClippingArea.Left)
				return (false);
			if (Top > aClippingArea.Bottom)
				return (false);
			if (Bottom < aClippingArea.Top)
				return (false);
			return (true);
		}
		
		public void CopyFrom (CellRectangle aRect)
		{
			x = aRect.X;
			y = aRect.Y;
			width = aRect.Width;
			height = aRect.Height;
		}
		
		public double[] Store()
		{
			double[] res = new double[4] { x, y, width, height };
			return (res);
		}
		
		public void Restore (double[] aBuffer)
		{
			if (aBuffer == null)
				return;
			x = aBuffer[0];
			y = aBuffer[1];
			width = aBuffer[2];
			height = aBuffer[3];
		}
		
		/// <summary>
		/// Grows it by specified number
		/// </summary>
		/// <param name="aGrowFor">
		/// Growth size <see cref="System.Double"/>
		/// </param>
		/// <returns>
		/// New rectangle <see cref="Cairo.Rectangle"/>
		/// </returns>
		public void Grow (double aGrowFor)
		{
			x -= aGrowFor;
			y -= aGrowFor;
			width += (aGrowFor*2);
			height += (aGrowFor*2);
		}
		
		public void Shrink (double aShrinkFor)
		{
			Grow (-aShrinkFor);
		}
		
		public CellRectangle Copy()
		{
			return (new CellRectangle (x, y, width, height));
		}
		
		public void Set (double aX, double aY, double aWidth, double aHeight)
		{
			x = aX;
			y = aY;
			width = aWidth;
			height = aHeight;
		}

		public Gdk.Rectangle CopyToGdkRectangle()
		{
			return (new Gdk.Rectangle (System.Convert.ToInt32 (x),
			                           System.Convert.ToInt32 (y),
			                           System.Convert.ToInt32 (width),
			                           System.Convert.ToInt32 (height)));
		}

		public override string ToString ()
		{
			return string.Format("[CellRectangle: X={0}, Y={1}, Width={2}, Height={3}, Left={4}, Top={5}, Right={6}, Bottom={7}, IsEmpty={8}]", X, Y, Width, Height, Left, Top, Right, Bottom, IsEmpty);
		}

		public CellRectangle()
		{
		}
		
		public CellRectangle (double aX, double aY, double aWidth, double aHeight)
		{
			X = aX;
			Y = aY;
			Width = aWidth;
			Height = aHeight;
		}
	}
}
