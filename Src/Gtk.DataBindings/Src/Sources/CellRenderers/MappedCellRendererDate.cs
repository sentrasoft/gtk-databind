//  
//  Copyright (C) 2009 matooo
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System;
using System.Data.Bindings;
using Gtk.ExtraWidgets;
using GLib;
using System.Globalization;

namespace Gtk.DataBindings
{
	/// <summary>
	/// Specifies cell renderer which handles date
	/// </summary>
	[GtkCellFactoryProvider ("date", "DefaultFactoryCreate")]
	[GtkCellFactoryProvider ("longdate", "DefaultFactoryCreate")]
	[GtkTypeCellFactoryProvider ("datehamdler", "DefaultFactoryCreate", typeof(DateTime))]
	public class MappedCellRendererDate : MappedCellRendererDrawingCell
	{
		private CellEditableAdapter adapter = null;
		
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
			MappedCellRendererDate wdg = new MappedCellRendererDate();
			if (aArgs.InvokationHandler == "longdate")
				wdg.LongDate = true;
			wdg.Editable = (aArgs.State == PropertyDefinition.ReadOnly);
			wdg.MappedTo = aArgs.PropertyName;
			return (wdg);
		}
		
		private DrawingCellEditText datetext = new DrawingCellEditText();
		
		private bool longDate = false;
		/// <value>
		/// Defines if text is important (bold) or not
		/// </value>
		public bool LongDate {
			get { return (longDate); }
			set {
				if (longDate == value)
					return;
				longDate = value;
				if (LongDate == false)
					datetext.SizeText = (isImportant == true) ? "<b>88/88/8888</b>" : "88/88/8888";
				else
					datetext.SizeText = "";
			}
		}
		
		private bool isImportant = false;
		/// <value>
		/// Defines if text is important (bold) or not
		/// </value>
		public bool IsImportant {
			get { return (isImportant); }
			set {
				if (isImportant == value)
					return;
				isImportant = value;
				if (LongDate == false)
					datetext.SizeText = (isImportant == true) ? "<b>88/88/8888</b>" : "88/88/8888";
				else
					datetext.SizeText = "";
			}
		}
		
		private DateTime date = DateTime.Now;
		/// <value>
		/// Specifies property which is used to assign value
		/// </value>
		[Property ("date")]
		public DateTime Date {
			get { return (date); }
			set {
				if (date.Equals(value) == true)
					return;
				date = value;
				if (LongDate == true)
					datetext.Text = date.ToLongDateString();
				else
					datetext.Text = date.ToShortDateString();
			}
		}

		public override void GetSize (Widget widget, ref Gdk.Rectangle cell_area, out int x_offset, out int y_offset, out int width, out int height)
		{
			base.GetSize (widget, ref cell_area, out x_offset, out y_offset, out width, out height);
//			int w, h;
//			(adapter.Implementor as DateEntry).GetSizeRequest (out w, out h);
			Requisition req = (adapter.Implementor as DateEntry).SizeRequest();
			System.Console.WriteLine("width={0}, height={1}, w={2}, h={3}", width, height, req.Width, req.Height);
			if (req.Width > width)
				width = req.Width;
			if (req.Height > height)
				height = req.Height;
		}

		/// <summary>
		/// Returns default data property
		/// </summary>
		/// <returns>
		/// Property name <see cref="System.String"/>
		/// </returns>
		public override string GetDataProperty ()
		{
			return ("date");
		}

		public override CellEditable StartEditing (Gdk.Event evnt, Widget widget, string path, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, CellRendererState flags)
		{
			System.Console.WriteLine("CR.StartEditing(Editable={0})", Editable);
			if (Editable == false)
				return (null);
//			return base.StartEditing (evnt, widget, path, background_area, cell_area, flags);
			CellEditableImplementor wdg = (CellEditableImplementor) adapter.Implementor;
//			(wdg as DateEntry).DoubleBuffered = true;
			(wdg as DateEntry).CellRendererWindow = widget.GdkWindow;
			System.Console.WriteLine("TYpe={0}", (wdg as DateEntry).CellRendererWindow.GetType());
//			CellEditEntry text = new CellEditEntry();
//			text.EditingDone += OnEditDone;
//			text.Text = source.Name;
//			text.path = path;
			if (TypeValidator.IsCompatible(wdg.GetType(), typeof(Gtk.Widget)) == true)
				(wdg as Gtk.Widget).Show();
			
//			view.EditingRow = true;
            
            return (adapter);
		}

		/// <summary>
		/// Assigns value to this column
		/// </summary>
		/// <param name="aValue">
		/// Value to be assigned <see cref="System.Object"/>
		/// </param>
		public void AssignValue (object aValue)
		{
			if (aValue != null)
				Date = (DateTime) aValue;
		}

		public MappedCellRendererDate()
			: base (new DrawingCellHBox())
		{
			this.Mode = CellRendererMode.Editable;
			datetext.SizeText = "88/88/8888";
			MainBox.PackEnd (new DrawingCellNull(), true);
			MainBox.PackEnd (datetext, false);
			DateEntry de = new DateEntry();
			de.IsCellRenderer = true;
			de.HasCalculator = false;
			de.HasClearButton = false;
			de.HasDropDown = false;
			
			adapter = new CellEditableAdapter (de);
		}
	}
}
