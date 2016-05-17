
using System;
using System.Data.Bindings;

namespace Gtk.DataBindings
{
	/// <summary>
	/// Provides methods which extend CellRenderers
	/// </summary>
	public static class CellRendererExtensionMethods
	{
		public static string ResolveDataProperty (this CellRenderer aCell)
		{
			if (TypeValidator.IsCompatible(aCell.GetType(), typeof(MappedCellRenderer)) == true)
				return ((aCell as MappedCellRenderer).GetDataProperty());
			if (TypeValidator.IsCompatible(aCell.GetType(), typeof(CellRendererToggle)) == true)
				return ("active");
			if (TypeValidator.IsCompatible(aCell.GetType(), typeof(CellRendererText)) == true)
				return ("text");
			if (TypeValidator.IsCompatible(aCell.GetType(), typeof(CellRendererPixbuf)) == true)
				return ("pixbuf");
			if (TypeValidator.IsCompatible(aCell.GetType(), typeof(CellRendererProgress)) == true)
				return ("value");
			if (TypeValidator.IsCompatible(aCell.GetType(), typeof(CellRendererSpin)) == true)
				return ("text");
			if (TypeValidator.IsCompatible(aCell.GetType(), typeof(CellRendererCombo)) == true)
				return ("text");
			throw new NotImplementedException ("CellRenderer of type {0} is not specified in list for data property");
		}
	}
}
