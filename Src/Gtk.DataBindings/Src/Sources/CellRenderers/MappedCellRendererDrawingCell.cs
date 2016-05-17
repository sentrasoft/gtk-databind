
using System;
using System.Data.Bindings;
using GLib;
using Gdk;
using Gtk;
using Gtk.ExtraWidgets;
using System.ComponentModel;

namespace Gtk.DataBindings
{
	/// <summary>
	/// Provides custom cell rendered CellRenderer
	/// </summary>
	public class MappedCellRendererDrawingCell : MappedCellRenderer, CellEditable
	{
		private DrawingCellBox box = null;
		/// <value>
		/// Provides access to master cell
		/// </value>
		[Browsable (false)]
		public DrawingCellBox MainBox {
			get { return (box); }
		}
		
		/// <value>
		/// Access to cells
		/// </value>
		[Browsable (false)]
		public DrawingCellCollection Cells {
			get { return (box.Cells); }
		}

		private bool doubleBuffered = false;
		/// <value>
		/// Spceifies if cell should be drawn by double buffering or not
		/// </value>
		public bool DoubleBuffered {
			get { return (doubleBuffered); }
			set {
				if (doubleBuffered == value)
					return;
				doubleBuffered = value;
			}
		}

		/// <value>
		/// Spacing between cells
		/// </value>
		[Browsable (false)]
		public int Padding {
			get { return (System.Convert.ToInt32 (box.Padding)); }
			set { 
				if (box.Padding == value)
					return;
				box.Padding = value; 
//				QueueDraw();
			}
		}

		private Gtk.Widget masterWidget = null;
		/// <value>
		/// Widget responsible for drawing
		/// </value>
		public Gtk.Widget MasterWidget {
			get { return (masterWidget); }
		}
		
		static GType type;
		
		/// <value>
		/// Cell count
		/// </value>
		[Browsable (false)]
		public int Count {
			get { return (box.Count); }
		}

		/// <value>
		/// Specifies pango context which is used to draw text for cell
		/// </value>
		public Pango.Context PangoContext {
			get { 
				if (MasterWidget == null)
					return (null);
				return (MasterWidget.PangoContext); 
			}
		}
		
		/// <summary>
		/// Resolves size of cell renderer
		/// </summary>
		/// <param name="widget">
		/// Widget <see cref="Widget"/>
		/// </param>
		/// <param name="cell_area">
		/// Area <see cref="Gdk.Rectangle"/>
		/// </param>
		/// <param name="x_offset">
		/// X offset <see cref="System.Int32"/>
		/// </param>
		/// <param name="y_offset">
		/// Y offset <see cref="System.Int32"/>
		/// </param>
		/// <param name="width">
		/// Width <see cref="System.Int32"/>
		/// </param>
		/// <param name="height">
		/// Height <see cref="System.Int32"/>
		/// </param>
		public override void GetSize (Widget widget, ref Gdk.Rectangle cell_area, out int x_offset, out int y_offset, out int width, out int height)
		{
			x_offset = 0;
			y_offset = 0;
			double w,h=0;
			masterWidget = widget;
			MainBox.Arguments.Start (CellAction.GetSize, new CellExposeEventArgs(null, null, widget.GdkWindow, null, null));
			MainBox.Arguments.PassedArguments.Widget = MasterWidget;
			MainBox.Arguments.PassedArguments.Renderer = this;
			MainBox.GetCellSize (out w, out h);
			MainBox.Arguments.Stop();
			masterWidget = null;
			width = System.Convert.ToInt32 (w + (Padding * 2)) + 4;
			height = System.Convert.ToInt32 (h + (Padding * 2));
		}

		/// <summary>
		/// Method which paints cells
		/// </summary>
		/// <param name="aDrawable">
		/// Window drawable <see cref="Gdk.Drawable"/>
		/// </param>
		/// <param name="aContext">
		/// Cairo context <see cref="Cairo.Context"/>
		/// </param>
		/// <param name="aArea">
		/// Area <see cref="CellRectangle"/>
		/// </param>
		/// <param name="aFlags">
		/// Flags <see cref="CellRendererState"/>
		/// </param>
		protected virtual void PaintCells (Gdk.Drawable aDrawable, Cairo.Context aContext, CellRectangle aArea, 
		                                   CellRendererState aFlags)
		{
			if (box.IsVisible == false)
				return;
			box.Area.Clip (aContext);
			CellRectangle cliprect = aArea.Copy();
			CellExposeEventArgs args = new CellExposeEventArgs (null, aContext, aDrawable, cliprect, box.Area);
			args.Widget = MasterWidget;
			args.Renderer = this;
			args.Flags = aFlags;
			args.ForceRecalculation = true;
			MainBox.Arguments.Start (CellAction.Paint, args);
			box.Paint (args);
			MainBox.Arguments.Stop();
			args.Disconnect();
			args = null;
			aContext.ResetClip();
		}

		/// <summary>
		/// Calculates cell areas
		/// </summary>
		/// <param name="aRect">
		/// Area occupied by widget <see cref="CellRectangle"/>
		/// </param>
		protected virtual void CalculateCellAreas (CellRectangle aRect)
		{
			CellRectangle rect = aRect.Copy();
			rect.Shrink (Padding);
			box.DoCalculateCellAreas (rect);
			rect = null;
		}
		
		/// <summary>
		/// Renders cell on provided space
		/// </summary>
		/// <param name="window">
		/// Drawable <see cref="Gdk.Drawable"/>
		/// </param>
		/// <param name="widget">
		/// Widget <see cref="Widget"/>
		/// </param>
		/// <param name="background_area">
		/// Area for background <see cref="Gdk.Rectangle"/>
		/// </param>
		/// <param name="cell_area">
		/// Cell area <see cref="Gdk.Rectangle"/>
		/// </param>
		/// <param name="expose_area">
		/// Expose area <see cref="Gdk.Rectangle"/>
		/// </param>
		/// <param name="aFlags">
		/// State flags <see cref="CellRendererState"/>
		/// </param>
		protected override void Render (Gdk.Drawable window, Widget widget, Gdk.Rectangle background_area, 
		                                Gdk.Rectangle cell_area, Gdk.Rectangle expose_area, 
		                                CellRendererState aFlags)
		{
			masterWidget = widget;

			CellRectangle r = new CellRectangle (cell_area.X, cell_area.Y, cell_area.Width, cell_area.Height);
			
			Gdk.Drawable buffer = null;
			Cairo.Context context = null;
			if (DoubleBuffered == true) {
				buffer = new Gdk.Pixmap (window, cell_area.Width, cell_area.Height);			
				context = Gdk.CairoHelper.Create (buffer);
			}
			else
				context = Gdk.CairoHelper.Create (window);

			Style style = Rc.GetStyle (widget);
			r.Set (expose_area.X, expose_area.Y, expose_area.Width, expose_area.Height);
			CalculateCellAreas (r);
			if (DoubleBuffered == true) {
				PaintCells (buffer, context, r, aFlags);
				widget.GdkWindow.DrawDrawable (style.BlackGC, buffer, cell_area.X, cell_area.Y, cell_area.X, cell_area.Y, cell_area.Width, cell_area.Height);
				buffer.Dispose();
			}
			else
				PaintCells (window, context, r, aFlags);

			style.Dispose();
			((IDisposable) context.Target).Dispose ();                               
			((IDisposable) context).Dispose ();
			context = null;
			masterWidget = null;
		}

		/// <summary>
		/// Initializes area
		/// </summary>
		protected virtual void InitArea()
		{
//			DoubleBuffered = true;
			box.Owner = this;
		}

/*		public virtual CellEditable CreateEditWidget()
		{
			throw new System.NotImplementedException ("CreateEditWidget must be overiden in CellRenderer");
		}*/

/*		public override CellEditable StartEditing (Gdk.Event evnt, Widget widget, string path, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, CellRendererState flags)
		{
//			return base.StartEditing (evnt, widget, path, background_area, cell_area, flags);
			Gtk.CellEditable widget = CreateEditWidget();
			
//			CellEditEntry text = new CellEditEntry();
//			text.EditingDone += OnEditDone;
//			text.Text = source.Name;
//			text.path = path;
			if (TypeValidator.IsCompatible(widget.GetType(), typeof(Gtk.Widget)) == true)
				(widget as Gtk.Widget).Show();
			
//			view.EditingRow = true;
            
            return (widget);
		}*/

		/// <summary>
		/// Creates main box which defines basic layout
		/// </summary>
		/// <returns>
		/// Box <see cref="DrawingCellPaintedBox"/>
		/// </returns>
		protected virtual DrawingCellBox CreateBox()
		{
			throw new NotImplementedException ("CreateBox must be done in derived classes");
		}

//		protected virtual CellEditableImplementor GetImplementor()
//		{
//			return (
//		}
		
		public MappedCellRendererDrawingCell()
			: this (null)
		{
		}
		
		public MappedCellRendererDrawingCell (DrawingCellBox aBox)
			: base()
		{
			
			if (aBox == null)
				box = CreateBox();
			else
				box = aBox;
			InitArea();
		}

		#region CellEditable implementation
		public void StartEditing (Gdk.Event evnt)
		{
			System.Console.WriteLine("StartEditing() " + evnt.Window.GetType());
		}

		private event EventHandler editingDone = null;
		public event EventHandler EditingDone {
			add { editingDone += value; }
			remove { editingDone -= value; }
		}
		
		public void FinishEditing ()
		{
			System.Console.WriteLine("FinishEditing");
//			throw new System.NotImplementedException();
		}
		
		public void RemoveWidget ()
		{
			System.Console.WriteLine("RemoveWidget");
//			throw new System.NotImplementedException();
		}
		
		private event EventHandler widgetRemoved = null;
		public event EventHandler WidgetRemoved {
			add { widgetRemoved += value; }
			remove { widgetRemoved -= value; }
		}
		#endregion

	}
}
