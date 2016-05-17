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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Bindings;

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// Base for DrawingArea packed with cells
	/// </summary>	
	[ToolboxItem (false)]
	public class CellDrawingArea : Gtk.DrawingArea, INotifyPropertyChanged
	{
		private bool isCellRenderer = false;
		/// <value>
		/// Specifies widget as cell renderer
		/// </value>
		internal bool IsCellRenderer {
			get { return (isCellRenderer); }
			set {
				if (isCellRenderer == value)
					return;
				isCellRenderer = value;
			}
		}
		
		private Gdk.Window cellRendererWindow = null;
		public Gdk.Window CellRendererWindow {
			get { return (cellRendererWindow); }
			set { cellRendererWindow = value; }
		}
	
		/// <summary>
		/// PropertyChanged delegate as specified in INotifyPropertyChanged
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Method calls PropertyChanged if it is not null, but it allows external
		/// objects to access this one for convinience
		/// </summary>
		/// <param name="aPropertyName">
		/// Name of the property which changed <see cref="System.String"/>
		/// </param>
		public virtual void OnPropertyChanged (string aPropertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged (this, new PropertyChangedEventArgs(aPropertyName));
		}

		private bool handlesPrelight = false;
		/// <value>
		/// If true then EnterNotifyEvent and LeaveNotifyEvent handle widget Prelight state
		/// </value>
		[Browsable (false)]
		public bool HandlesPrelight {
			get { return (handlesPrelight); }
			set {
				if (handlesPrelight == value)
					return;
				handlesPrelight = value;
				OnPropertyChanged ("HandlesPrelight");
			}
		}
		
		private DrawingCellBox box = null;
		/// <value>
		/// Provides access to master cell
		/// </value>
		[Browsable (false)]
		public DrawingCellBox MainBox {
			get { return (box); }
		}
		
		private IDrawingCell mouseOverCell = null;
		[Browsable (false)]
		public IDrawingCell MouseOverCell {
			get { return (mouseOverCell); }
			protected set {
				if (mouseOverCell == value)
					return;
				if (value != null) {
					if (value != mouseOverCell) {
						if (mouseOverCell != null)
							mouseOverCell.SetGtkState (StateType.Normal);
						mouseOverCell = value;
						mouseOverCell.SetGtkState (StateType.Prelight);
						QueueDraw();
					}
				}
				else if (mouseOverCell != null) {
					mouseOverCell.SetGtkState (StateType.Normal);
					mouseOverCell = null;
					QueueDraw();
				}
			}
		}

		/// <value>
		/// Spacing between cells
		/// </value>
		[Browsable (false)]
		public int Spacing {
			get { return (box.Spacing); }
			set { 
				if (box.Spacing == value)
					return;
				box.Spacing = value; 
				QueueDraw();
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
				QueueDraw();
			}
		}

		/// <value>
		/// Specifies if contents should be expanded or not
		/// </value>
		[Browsable (false)]
		public bool ExpandContents {
			get { return (box.Expanded); }
			set {
				if (box.Expanded == value)
					return;
				box.Expanded = value;
				QueueDraw();
			}
		}
		
		/// <value>
		/// Cell count
		/// </value>
		[Browsable (false)]
		public int Count {
			get { return (box.Count); }
		}
		
		/// <value>
		/// Access to cells
		/// </value>
		[Browsable (false)]
		public DrawingCellCollection Cells {
			get { return (box.Cells); }
		}

		/// <summary>
		/// Method which paints cells
		/// </summary>
		/// <param name="evnt">
		/// Expose event parameters <see cref="Gdk.EventExpose"/>
		/// </param>
		/// <param name="aContext">
		/// Cairo context <see cref="Cairo.Context"/>
		/// </param>
		protected virtual void PaintCells (Gdk.EventExpose evnt, Gdk.Drawable aDrawable, Cairo.Context aContext, CellRectangle aArea)
		{
			if (box.IsVisible == false)
				return;
			System.Console.WriteLine("Paint");
			box.Area.Clip (aContext);
//			aContext.Rectangle (box.Area);
//			aContext.Clip();
//			Cairo.Rectangle cliprect = new Cairo.Rectangle (0, 0, Allocation.Width, Allocation.Height);
			CellRectangle cliprect = new CellRectangle (evnt.Area.X, evnt.Area.Y, evnt.Area.Width, evnt.Area.Height);
//			box.Paint (evnt, aContext, cliprect, box.Area);
//			box.Paint (new CellExposeEventArgs (evnt, aContext, evnt.Window, cliprect, box.Area));
			CellExposeEventArgs args = new CellExposeEventArgs (evnt, aContext, aDrawable, cliprect, box.Area);
			args.WidgetInRenderer = IsCellRenderer;
			args.Widget = this;
			args.ForceRecalculation = true;
			box.Arguments.Start (CellAction.Paint, args);
			box.Paint (args);
			box.Arguments.Stop();
			args.Disconnect();
			args = null;
			aContext.ResetClip();
		}

		protected virtual void CellsChanged()
		{
			Requisition req = new Requisition();
			OnSizeRequested (ref req);
			int w = Allocation.Width;
			int h = Allocation.Height;
			if (req.Width > w)
				w = req.Width;
			if (req.Height > h)
				h = req.Height;
			if (w != Allocation.Width)
				WidthRequest = w;
			if (h != Allocation.Height)
				HeightRequest = h;
		}
		
		/// <summary>
		/// Method which paints background
		/// </summary>
		/// <param name="evnt">
		/// Expose event parameters <see cref="Gdk.EventExpose"/>
		/// </param>
		/// <param name="aContext">
		/// Cairo context <see cref="Cairo.Context"/>
		/// </param>
		protected virtual void PaintBackground (Gdk.EventExpose evnt, Cairo.Context aContext, Cairo.Rectangle aArea)
		{
		}

		/// <summary>
		/// Expose event handler, calls PaintBackground and then PaintCells
		/// </summary>
		/// <param name="evnt">
		/// Arguments <see cref="Gdk.EventExpose"/>
		/// </param>
		/// <returns>
		/// true if successful, false if not <see cref="System.Boolean"/>
		/// </returns>
		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			base.OnExposeEvent (evnt);
			int x,y,w,h,d = 0;
			GdkWindow.GetGeometry (out x, out y, out w, out h, out d);
			CellRectangle r = new CellRectangle (0, 0, w, h);
			
			System.Console.WriteLine("Using cellrendererwindow=" + (IsCellRenderer == true));
			Gdk.Window masterDrawable = (IsCellRenderer == true) ? cellRendererWindow : evnt.Window;
			Gdk.Drawable buffer = null;
			Cairo.Context context = null;
			if (IsDoubleBuffered == true) {
				System.Console.WriteLine("DoubleBuffered");
				buffer = new Gdk.Pixmap (masterDrawable, Allocation.Width, Allocation.Height, 24);			
				context = Gdk.CairoHelper.Create (buffer);
			}
			else {
				masterDrawable.BeginPaintRect (evnt.Area);
				context = Gdk.CairoHelper.Create (masterDrawable);
			}

			Gdk.Color clr = Style.Backgrounds[(int) StateType.Normal];
//			context.Color = new Cairo.Color (0, 0, 1);
			context.Color = clr.GetCairoColor();
			r.DrawPath(context);
			context.Fill();
//			if ((BackgroundPainted == true) && (Sensitive == true))
//				PaintBackground (evnt, g, rect);
//			Cairo.Rectangle r = rect.CopyAndShrink (Padding);
			r.Shrink (Padding);

			CalculateCellAreas (r);
			if (IsDoubleBuffered == true) {
				PaintCells (evnt, buffer, context, r);
				evnt.Window.DrawDrawable (Style.BlackGC, buffer, evnt.Area.X, evnt.Area.Y, evnt.Area.X, evnt.Area.Y, evnt.Area.Width, evnt.Area.Height);
				buffer.Dispose();
			}
			else {
				PaintCells (evnt, masterDrawable, context, r);
				masterDrawable.EndPaint();
			}

			((IDisposable) context.Target).Dispose ();                               
			((IDisposable) context).Dispose ();
			context = null;
			return (true);
		}

		/// <summary>
		/// Returns cell which takes place on specified coordinates
		/// </summary>
		/// <param name="aX">
		/// X <see cref="System.Int32"/>
		/// </param>
		/// <param name="aY">
		/// Y <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// Cell or null <see cref="IDrawingCell"/>
		/// </returns>
		/// <remarks>
		/// This method is usefull for mouse resolving
		/// </remarks>
		public virtual IDrawingCell CellAtCoordinates (int aX, int aY)
		{
			return (box.CellAtCoordinates (aX, aY));
		}
		
		/// <summary>
		/// Calculates cell areas
		/// </summary>
		/// <param name="aRect">
		/// Area occupied by widget <see cref="CellRectangle"/>
		/// </param>
		protected virtual void CalculateCellAreas (CellRectangle aRect)
		{
//			Cairo.Rectangle rect = aRect.CopyAndShrink (Padding);
			CellRectangle rect = aRect.Copy();
			rect.Shrink (Padding);
			box.DoCalculateCellAreas (rect);
			rect = null;
		}
		
		/// <summary>
		/// Calculates requisition
		/// </summary>
		/// <param name="requisition">
		/// Requisition <see cref="Requisition"/>
		/// </param>
		/// <remarks>
		/// Must be overriden
		/// </remarks>
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			double w,h=0;
			System.Console.WriteLine("GetSize");
			Gdk.Drawable drawable;
			if (IsCellRenderer == false)
				drawable = this.GdkWindow;
			else
				drawable = cellRendererWindow;
			box.Arguments.Start (CellAction.GetSize, new CellExposeEventArgs (null, null, drawable, null, null));
			box.Arguments.PassedArguments.Widget = this;
			box.GetCellSize (out w, out h);
			box.Arguments.Stop();
			w = w + (Padding * 2);
			h = h + (Padding * 2);
			requisition.Width = System.Convert.ToInt32 (w);
			requisition.Height = System.Convert.ToInt32 (h);
			System.Console.WriteLine("EndSize");
		}

		/// <summary>
		/// Packs cell as first
		/// </summary>
		/// <param name="aCell">
		/// Cell <see cref="IDrawingCell"/>
		/// </param>
		/// <param name="aExpanded">
		/// Cell is expanded if true, only one expanded cell is supported <see cref="System.Boolean"/>
		/// </param>
		public void PackStart (IDrawingCell aCell, bool aExpanded)
		{
			if (aCell == null)
				return;
			box.PackStart (aCell, aExpanded);
		}

		/// <summary>
		/// Packs cell as last
		/// </summary>
		/// <param name="aCell">
		/// Cell <see cref="IDrawingCell"/>
		/// </param>
		/// <param name="aExpanded">
		/// Cell is expanded if true, only one expanded cell is supported <see cref="System.Boolean"/>
		/// </param>
		public void PackEnd (IDrawingCell aCell, bool aExpanded)
		{
			if (aCell == null)
				return;
			box.PackEnd (aCell, aExpanded);
		}

		protected override bool OnButtonPressEvent (Gdk.EventButton evnt)
		{
			if (TypeValidator.IsCompatible(this.GetType(), typeof(IEditable)) == true)
				if ((this as IEditable).Editable == false)
					return (base.OnButtonPressEvent (evnt));
			if (CanFocus == false)
				return (base.OnButtonPressEvent (evnt));
			if (HasFocus == false) {
				GrabFocus();
				if (box != null) {
					IDrawingCell icell = box.CellAtCoordinates (System.Convert.ToInt32(evnt.X), System.Convert.ToInt32(evnt.Y));
					if (icell != null)
						icell.Activate();
				}
				return (true);
			}
			if (box == null)
				return (base.OnButtonPressEvent (evnt));
			IDrawingCell cell = box.CellAtCoordinates (System.Convert.ToInt32(evnt.X), System.Convert.ToInt32(evnt.Y));
			if (cell == null)
				return (base.OnButtonPressEvent (evnt));
			if (cell.Activate() == true)
				return (true);
			return (base.OnButtonPressEvent (evnt));
		}

		protected override bool OnMotionNotifyEvent (Gdk.EventMotion evnt)
		{
			if (TypeValidator.IsCompatible(this.GetType(), typeof(IEditable)) == true)
				if ((this as IEditable).Editable == false)
					return (base.OnMotionNotifyEvent (evnt));
			MouseOverCell = box.CellAtCoordinates (System.Convert.ToInt32(evnt.X), System.Convert.ToInt32(evnt.Y));
			return (base.OnMotionNotifyEvent (evnt));
		}

		protected override bool OnEnterNotifyEvent (Gdk.EventCrossing evnt)
		{
			if ((HandlesPrelight == true) && (this.IsSensitive() == true))
				State = StateType.Prelight;
			if (TypeValidator.IsCompatible(this.GetType(), typeof(IEditable)) == true)
				if ((this as IEditable).Editable == false)
					return (base.OnEnterNotifyEvent (evnt));
			MouseOverCell = box.CellAtCoordinates (System.Convert.ToInt32(evnt.X), System.Convert.ToInt32(evnt.Y));
			return (base.OnEnterNotifyEvent (evnt));
		}

		protected override bool OnLeaveNotifyEvent (Gdk.EventCrossing evnt)
		{
			if ((HandlesPrelight == true) && (this.IsSensitive() == true))
				State = StateType.Normal;
			if (TypeValidator.IsCompatible(this.GetType(), typeof(IEditable)) == true)
				if ((this as IEditable).Editable == false)
					return (base.OnLeaveNotifyEvent (evnt));
			MouseOverCell = null;
			return (base.OnLeaveNotifyEvent (evnt));
		}

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

		public virtual Gdk.Pixbuf GetDragPixbuf()
		{
			return (MainBox.GetAsPixbuf());
		}
		
		protected virtual void InitArea()
		{
			DoubleBuffered = true;
			Events |= Gdk.EventMask.PointerMotionMask | Gdk.EventMask.EnterNotifyMask | 
				      Gdk.EventMask.LeaveNotifyMask | Gdk.EventMask.KeyPressMask |
					  Gdk.EventMask.KeyReleaseMask | Gdk.EventMask.FocusChangeMask |
					  Gdk.EventMask.Button1MotionMask | Gdk.EventMask.Button2MotionMask |
					  Gdk.EventMask.Button3MotionMask | Gdk.EventMask.ButtonPressMask |
					  Gdk.EventMask.ButtonReleaseMask;
			box.Owner = this;
		}

		public CellDrawingArea()
			: this (null)
		{
		}
		
		public CellDrawingArea (DrawingCellBox aBox)
		{
			if (aBox == null)
				box = CreateBox();
			else
				box = aBox;
			InitArea();
		}
	}
}
