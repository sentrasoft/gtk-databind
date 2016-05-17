
using System;

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// 
	/// </summary>
	public class DrawingCellComboArrow : DrawingCellArrow
	{
		protected static double MAXARROW = 10;

		public override void MaxArrowSize (out double x, out double y)
		{
			base.MaxArrowSize (out x, out y);
			if (x > MAXARROW)
				x = MAXARROW;
			if (y > MAXARROW)
				y = MAXARROW;
		}
	}
}
