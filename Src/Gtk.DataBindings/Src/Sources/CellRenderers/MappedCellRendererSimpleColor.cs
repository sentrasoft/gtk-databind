
using System;
using System.Data.Bindings;
using Gtk.ExtraWidgets;
using GLib;

namespace Gtk.DataBindings
{
	/// <summary>
	/// Provides cell renderer for simple color box
	/// </summary>
	[GtkCellFactoryProvider ("cairocolor", "DefaultFactoryCreate")]	
	[GtkCellFactoryProvider ("simplecairocolor", "DefaultFactoryCreate")]
	[GtkTypeCellFactoryProvider ("cairocolorhandler", "DefaultFactoryCreate", typeof (Cairo.Color))]
	public class MappedCellRendererSimpleColor : MappedCellRendererDrawingCell
	{
		/// <summary>
		/// Registered factory creation method
		/// </summary>
		/// <param name="aArgs">
		/// Arguments <see cref="FactoryInvocationArgs"/>
		/// </param>
		/// <returns>
		/// Result widget <see cref="IMappedColumnItem"/>
		/// </returns>
		public static IMappedColumnItem DefaultFactoryCreate (FactoryInvocationArgs aArgs)
		{
			IMappedColumnItem wdg = new MappedCellRendererSimpleColor();
			wdg.MappedTo = aArgs.PropertyName;
			return (wdg);
		}
		
		private DrawingCellGdkColorFrame clrcell = new DrawingCellGdkColorFrame();
		
		static GType type;

		private Cairo.Color color = new Cairo.Color (0, 0, 0);
		[GLib.Property ("color")]
		public Cairo.Color Color {
			get { return (color); }
			set { 
				if (color.Equals(value) == true)
					return;
				color = value;
				clrcell.FillColor = Color;
			}
		}
		
		public override string GetDataProperty ()
		{
			return ("color");
		}

		public override void AssignValue (object aValue)
		{
			try {
				if (aValue != null)
					Color = (Cairo.Color) aValue;
			}
			catch {
				if (aValue == null)
					System.Console.WriteLine("Null assigned to SimpleCOlorCellRenderer");
				else
					System.Console.WriteLine("Value type {0} assigned to SimpleColorCellRenderer", aValue.GetType());
				Color = new Cairo.Color	(0, 0, 0);
			}
			clrcell.FillColor = Color;
		}

		public MappedCellRendererSimpleColor()
			: base (new DrawingCellBin())
		{
			clrcell.BorderVisible = true;
			clrcell.BorderColor = new Cairo.Color (0, 0, 0);
			clrcell.BorderWidth = 2;
			clrcell.FillColor = Color;
			clrcell.FillVisible = true;
			(MainBox as DrawingCellBin).Pack (clrcell);
		}
	}
}
