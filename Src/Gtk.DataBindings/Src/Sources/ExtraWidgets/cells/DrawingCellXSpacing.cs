
using System;

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// Cell which provides fixed width separator
	/// </summary>
	public class DrawingCellXSpacing : DrawingCellNull
	{
		public DrawingCellXSpacing (double aSpacing)
		{
			MinWidth = aSpacing;
		}
	}
}
