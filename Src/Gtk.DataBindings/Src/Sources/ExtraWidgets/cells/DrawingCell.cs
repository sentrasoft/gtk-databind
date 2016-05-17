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

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// Base for IDrawingCell implementations
	/// </summary>	
	public class DrawingCell : BaseNotifyPropertyChanged, IDrawingCell
	{
		private double sizeWidth = -1;
		internal double SizeWidth {
			get { return (sizeWidth); }
		}
		
		private double sizeHeight = -1;
		internal double SizeHeight {
			get { return (sizeHeight); }
		}
		
		private bool isFocused = false;

		private CellArguments arguments = new CellArguments();
		/// <value>
		/// Arguments passed on action
		/// </value>
		public CellArguments Arguments {
			get { 
				if (Owner == null)
					return (arguments);
				if (TypeValidator.IsCompatible(Owner.GetType(), typeof(IDrawingCell)) == true)
					return ((Owner as DrawingCell).Arguments);
				return (arguments); 
			}
		}
		
		/// <summary>
		/// Checks if cell is visible or not
		/// </summary>
		public virtual bool IsVisible {
			get { return (Visible == true); }
		}
		
		private bool visible = true;
		/// <value>
		/// Specifies if cell is visible or not
		/// </value>
		public bool Visible {
			get { return (visible); }
			set {
				if (visible == value)
					return;
				visible = value;
				OnPropertyChanged ("Visible");
			}
		}
		
		private CellRectangle area = new CellRectangle();
		/// <value>
		/// Specifies are defined for cell
		/// </value>
		public CellRectangle Area {
			get { return (area); }
//			set { area = value; }
		}

		private double padding = 0;
		/// <value>
		/// Specifies padding border width for cell
		/// </value>
		public double Padding {
			get { return (padding); }
			set {
				if (padding == value)
					return;
				padding = value;
				OnPropertyChanged ("Padding");
			}
		}
		
		private double minWidth = 0;
		/// <value>
		/// Specifies padding border width for cell
		/// </value>
		public double MinWidth {
			get { return (minWidth); }
			set {
				if (minWidth == value)
					return;
				minWidth = value;
				OnPropertyChanged ("MinWidth");
			}
		}
		
		private double minHeight = 0;
		/// <value>
		/// Specifies padding border width for cell
		/// </value>
		public double MinHeight {
			get { return (minHeight); }
			set {
				if (minHeight == value)
					return;
				minHeight = value;
				OnPropertyChanged ("MinHeight");
			}
		}
		
		private WeakReference owner = null;
		/// <value>
		/// Specifies owner
		/// </value>
		public object Owner {
			get {
				if (owner == null)
					return (null);
				return (owner.Target);
			}
			set {
				if (value == Owner)
					return;
				if (value == null) {
					DisconnectFromWidgetFocus();
					owner = null;
				}
				else {
					DisconnectFromWidgetFocus();
					owner = new WeakReference (value);
					ConnectToWidgetFocus();
				}
			}
		}

		public bool MasterIsFocused {
			get {
				if (owner == null)
					return (false);
				if (TypeValidator.IsCompatible(Master.GetType(), typeof(Gtk.Widget)) == false)
					return (false);
				Gtk.Widget wdg = (Gtk.Widget) Master;
				if (wdg.HasFocus == true)
					return (true);
				while (wdg.Parent != null) {
					wdg = wdg.Parent;
					if (wdg.HasFocus == true)
						return (true);
				}
				return (false);
			}
		}
		
		private void HandleFocusOutEvent(object o, FocusOutEventArgs args)
		{
			isFocused = false;
		}

		private void HandleFocusInEvent (object o, FocusInEventArgs args)
		{
			isFocused = true;			
		}
		
		protected void ConnectToWidgetFocus()
		{
			if (Owner == null)
				return;
			if (Owner != Master)
				return;
			if (TypeValidator.IsCompatible(Master.GetType(), typeof(Gtk.Widget)) == true) {
				(Master as Gtk.Widget).FocusInEvent += HandleFocusInEvent;
				(Master as Gtk.Widget).FocusOutEvent += HandleFocusOutEvent;
			}
		}
		
		protected void DisconnectFromWidgetFocus()
		{
			isFocused = false;
			if (Owner == null)
				return;
			if (TypeValidator.IsCompatible(Master.GetType(), typeof(Gtk.Widget)) == true) {
				(Master as Gtk.Widget).FocusInEvent -= HandleFocusInEvent;
				(Master as Gtk.Widget).FocusOutEvent -= HandleFocusOutEvent;
			}
		}

		private WeakReference resolvedMaster = null;
		/// <value>
		/// Returns top most owner of the cells who isn't IDrawingCell
		/// </value>
		public object Master {
			get {
				if (resolvedMaster != null)
					return (resolvedMaster.Target);
				if (Owner == null)
					return (null);
				if (TypeValidator.IsCompatible(Owner.GetType(), typeof(IDrawingCell)) == true)
					if ((Owner as IDrawingCell).Owner != null) {
						object o = (Owner as IDrawingCell).Master;
						if (o != null)
							if (TypeValidator.IsCompatible(o.GetType(), typeof(IDrawingCell)) == false)
								resolvedMaster = new WeakReference(o);
						if (resolvedMaster != null)
							return (resolvedMaster.Target);
						return (null);
					}
				return (Owner);
			}
		}
		
		internal virtual void HierarchyChanged()
		{
			resolvedMaster = null;
		}
		
		private bool expanded = false;
		/// <value>
		/// Speceifies if cell is expanded or not
		/// </value>
		public bool Expanded {
			get { return (expanded); }
			set { 
				if (expanded == value)
					return;
				expanded = value; 
				OnPropertyChanged ("Expanded");
			}
		}
		
		private Pango.Context pangoContext = null;
		public Pango.Context PangoContext {
			get { 
				if (pangoContext != null)
					return (pangoContext);
				if (Owner != null)
					if (TypeValidator.IsCompatible(Owner.GetType(), typeof(DrawingCell)) == true)
						return ((Owner as DrawingCell).PangoContext);
				if (Master == null)
					return (null);
				if (TypeValidator.IsCompatible(Master.GetType(), typeof(Gtk.Widget)) == true)
					pangoContext = (Master as Gtk.Widget).PangoContext;
				if (TypeValidator.IsCompatible(Master.GetType(), typeof(Gtk.DataBindings.MappedCellRendererDrawingCell)) == true)
					pangoContext = (Master as Gtk.DataBindings.MappedCellRendererDrawingCell).PangoContext;
				return (pangoContext);
			}
		}

		private Pango.FontDescription fontDescription = null;
		public Pango.FontDescription FontDescription {
			get { 
				if (Owner != null)
					if (TypeValidator.IsCompatible(Owner.GetType(), typeof(DrawingCell)) == true)
						return ((Owner as DrawingCell).FontDescription);
				if (fontDescription != null)
					return (fontDescription);
//				fontDescription = new PangoContext.FontDescription();
				fontDescription = new Pango.FontDescription();
System.Console.WriteLine("FontDescription=" + fontDescription.GetType());
				return (fontDescription);
			}
		}

		internal virtual void ResetSize()
		{
			sizeHeight = -1;
			sizeWidth = -1;
			if (Owner != null)
				if (TypeValidator.IsCompatible(Owner.GetType(), typeof(DrawingCell)) == true)
					(Owner as DrawingCell).ResetSize();
		}
		
		private Pango.Layout layout = null;
		public Pango.Layout Layout {
			get { 
				if (Owner != null)
					if (TypeValidator.IsCompatible(Owner.GetType(), typeof(DrawingCell)) == true)
						return ((Owner as DrawingCell).Layout);
				if (layout == null)
					layout = CreateLayout();
				return (layout);
			}
		}
		
		private Pango.Layout CreateLayout() 
		{
			if (Master == null)
				return (null);
			if (TypeValidator.IsCompatible(Master.GetType(), typeof(Gtk.Widget)) == true)
				return ((Master as Gtk.Widget).CreatePangoLayout(""));
			if (TypeValidator.IsCompatible(Master.GetType(), typeof(Gtk.DataBindings.MappedCellRendererDrawingCell)) == true) 
				return (((Gtk.Widget) Arguments.PassedArguments.Widget).CreatePangoLayout (""));
			return (ChameleonTemplates.Entry.CreatePangoLayout(""));
		}
		
		protected virtual CellRectangle GetPaintableArea()
		{
			return (new CellRectangle (Area.X+Padding, Area.Y+Padding, Area.Width-(Padding*2), Area.Height-(Padding*2)));
		}
		
		/// <summary>
		/// Paints cell on cairo context
		/// </summary>
		/// <param name="aArgs">
		/// A <see cref="CellExposeEventArgs"/>
		/// </param>
		public virtual void Paint (CellExposeEventArgs aArgs)
		{
			throw new NotImplementedException ("Paint has to be overriden in derived classes");
		}
		
		/// <summary>
		/// Resolves size needed for cell
		/// </summary>
		/// <param name="aWidth">
		/// Cell width <see cref="System.Double"/>
		/// </param>
		/// <param name="aHeight">
		/// Cell height <see cref="System.Double"/>
		/// </param>
		public virtual void GetSize (out double aWidth, out double aHeight)
		{
			throw new NotImplementedException ("GetSize has to be overriden in derived classes");
		}

		/// <summary>
		/// Returns cell which takes place on specified coordinates
		/// </summary>
		/// <param name="aX">
		/// X <see cref="System.Double"/>
		/// </param>
		/// <param name="aY">
		/// Y <see cref="System.Double"/>
		/// </param>
		/// <returns>
		/// Cell or null <see cref="IDrawingCell"/>
		/// </returns>
		/// <remarks>
		/// This method is usefull for mouse resolving
		/// </remarks>
		public virtual IDrawingCell CellAtCoordinates (double aX, double aY)
		{
			if ((aX >= Area.X) && (aX <= (Area.Width+Area.X)))
				if ((aY >= Area.Y) && (aY <= (Area.Height+Area.Y)))
					return (this);
			return (null);
		}
		
		/// <summary>
		/// Resolves size needed for cell
		/// </summary>
		/// <param name="aWidth">
		/// Cell width <see cref="System.Double"/>
		/// </param>
		/// <param name="aHeight">
		/// Cell height <see cref="System.Double"/>
		/// </param>
		public virtual void GetCellSize (out double aWidth, out double aHeight)
		{
			if (IsVisible == false) {
				aWidth = 0;
				aHeight = 0;
				return;
			}
			if ((sizeHeight < 0) || (sizeWidth < 0)) {
				GetSize (out aWidth, out aHeight);
				sizeHeight = aHeight;
				sizeWidth = aWidth;
			}
			else {
				aWidth = sizeWidth;
				aHeight = sizeHeight;
			}
			if (aWidth < MinWidth)
				aWidth = MinWidth;
			if (aHeight < MinHeight)
				aHeight = MinHeight;
			aWidth += (Padding * 2);
			aHeight += (Padding * 2);
		}
		
		protected virtual void InitCell()
		{
		}
		
		public DrawingCell()
		{
			InitCell();
		}
	}
}
