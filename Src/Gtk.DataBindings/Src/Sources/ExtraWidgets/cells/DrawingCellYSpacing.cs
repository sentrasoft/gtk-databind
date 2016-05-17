
using System;

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// Cell which provides fixed height separator
	/// </summary>
	public class DrawingCellYSpacing : DrawingCellNull
	{
		public DrawingCellYSpacing (double aSpacing)
		{
			MinHeight = aSpacing;
		}
	}
}
