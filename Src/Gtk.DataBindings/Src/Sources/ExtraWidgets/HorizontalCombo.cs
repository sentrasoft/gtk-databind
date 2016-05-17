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
using Gdk;
using Gtk;
using Gtk.ExtraWidgets;
using GLib;

namespace Gtk.DataBindings
{
	/// <summary>
	/// 
	/// </summary>
	[ToolboxItem (true)]
	[Category ("Extra Gtk Widgets")]
	public class HorizontalCombo : EventBox
	{
		static GType type;

		private HBox mainbox = new HBox();

		private List<HBox> widgets = new List<HBox>();

		/// <value>
		/// Returns count of items
		/// </value>
		public int Count {
			get { return (widgets.Count); }
		}
		
		private bool separatorLines = true;
		/// <value>
		/// Specifies if lines between cell should be drawn or not
		/// </value>
		public bool SeparatorLines {
			get { return (separatorLines); }
			set { 
				if (separatorLines == value)
					return;
				separatorLines = value;
				if (value == true) {
					for (int i=0; i<Count-1; i++) {
						VSeparator vs = new VSeparator();
						vs.Show();
						widgets[i].PackEnd (vs, false, false, 0);
					}
				}
				else {
					for (int i=0; i<Count-1; i++) {
						for (int j=widgets[i].Children.Length-2; j>=0; j--) {
							if (widgets[i].Children[j] is VSeparator) {
								VSeparator vs = (VSeparator) widgets[i].Children[j];
								widgets[i].Remove (vs);
								vs.Hide();
								vs.Destroy();
							}
						}
					}
				}
			}
		}
		
		/// <value>
		/// Specifies homogeneous layout, aka. wheter all cells should be equal sized or not
		/// </value>
		public bool Homogeneous {
			get { return (mainbox.Homogeneous); }
			set { 
				if (mainbox.Homogeneous == value)
					return;
				mainbox.Homogeneous = value; 
				foreach (Widget wdg in mainbox.Children)
					mainbox.SetChildPacking (wdg, mainbox.Homogeneous, mainbox.Homogeneous, 0, PackType.Start);
			}
		}

		private int hovered = -1;
		/// <value>
		/// Index of item when mouse passes over it
		/// </value>
		public int Hovered {
			get { return (hovered); }
			set {
				if (hovered == value)
					return;
				if ((Count > 0) && (hovered != -1) && (hovered != Selected))
					if (widgets[hovered].Children.Length > 0)
						if (TypeValidator.IsCompatible(widgets[hovered].Children[0].GetType(), typeof(HComboCell)) == true)
							(widgets[hovered].Children[0] as HComboCell).ItemState = EnumItemState.Normal;
				hovered = value; 
				if ((Count > 0) && (hovered != -1) && (hovered != Selected))
					if (widgets[hovered].Children.Length > 0)
						if (TypeValidator.IsCompatible(widgets[hovered].Children[0].GetType(), typeof(HComboCell)) == true)
							(widgets[hovered].Children[0] as HComboCell).ItemState = EnumItemState.Preflight;
				QueueDraw();
				OnCellHovered (value);
			}
		}

		private event HorizontalComboBoxEvent cellHovered = null;
		/// <summary>
		/// Event triggered when mouse passes over cell
		/// </summary>
		public event HorizontalComboBoxEvent CellHovered {
			add { cellHovered += value; }
			remove { cellHovered -= value; }
		}
		
		/// <summary>
		/// Calls event handlers which registered CellHovered event
		/// </summary>
		/// <param name="aIndex">
		/// A <see cref="System.Int32"/>
		/// </param>
		protected virtual void OnCellHovered (int aIndex)
		{
			if (cellHovered != null)
				cellHovered (this, new HorizontalComboEventArgs (this, aIndex));
		}
		
		private int selected = -1;
		/// <value>
		/// Specifies index of currntly selected cell
		/// </value>
		public int Selected {
			get { return (selected); }
			set {
				if (selected == value)
					return;
				if ((Count > 0) && (selected != -1))
					if (widgets[hovered].Children.Length > 0)
						if (TypeValidator.IsCompatible(widgets[hovered].Children[0].GetType(), typeof(HComboCell)) == true)
							(widgets[selected].Children[0] as HComboCell).ItemState = EnumItemState.Normal;
				selected = value; 
				if ((Count > 0) && (selected != -1))
					if (widgets[hovered].Children.Length > 0)
						if (TypeValidator.IsCompatible(widgets[hovered].Children[0].GetType(), typeof(HComboCell)) == true)
							(widgets[selected].Children[0] as HComboCell).ItemState = EnumItemState.Selected;
				QueueDraw();
			}
		}

		private event HorizontalComboBoxEvent cellSelected = null;
		/// <summary>
		/// Event triggered when cell is selected
		/// </summary>
		public event HorizontalComboBoxEvent CellSelected {
			add { cellSelected += value; }
			remove { cellSelected -= value; }
		}
		
		/// <summary>
		/// Calls event handlers which registered CellSelected event
		/// </summary>
		/// <param name="aIndex">
		/// A <see cref="System.Int32"/>
		/// </param>
		protected virtual void OnCellSelected (int aIndex)
		{
			if (cellSelected != null)
				cellSelected (this, new HorizontalComboEventArgs (this, aIndex));
		}
		
		private Style buttonStyle = null;
		
		protected virtual void ExposeWidget (bool aFocus, bool aPrelight, Gdk.Rectangle aClipRegion, Gdk.EventExpose evnt)
		{
			Widget wdg = this;
			
			Gdk.Rectangle rect = Allocation.Copy();
			Gdk.Rectangle clip = aClipRegion;
			Gdk.Rectangle borderclip = aClipRegion;
			
			borderclip.X += buttonStyle.XThickness;
			borderclip.Y += buttonStyle.YThickness;
			borderclip.Width -= buttonStyle.XThickness*2;
			borderclip.Height -= buttonStyle.YThickness*2;
			if (wdg.IsSensitive() == false) {
				Style.PaintFlatBox (buttonStyle, GdkWindow, StateType.Insensitive, ShadowType.Out, clip, 
				                wdg, "button", rect.X, rect.Y, rect.Width, rect.Height);
				Style.PaintBox (buttonStyle, GdkWindow, StateType.Insensitive, ShadowType.Out, clip, 
				                wdg, "button", rect.X, rect.Y, rect.Width, rect.Height);
			}
			else {
				if (HasDefault == true)
					Style.PaintBox (buttonStyle, GdkWindow, StateType.Selected, ShadowType.Out, clip, 
					                wdg, "button", rect.X, rect.Y, rect.Width, rect.Height);
				Gtk.StateType st = StateType.Normal;
				if (aFocus == true)
					st = StateType.Selected;
				else if (aPrelight == true)
					st = StateType.Prelight;
				Style.PaintBox (buttonStyle, GdkWindow, st, ShadowType.Out, borderclip, 
				                wdg, "buttondefault", rect.X, rect.Y, rect.Width, rect.Height);
			}
		}
		
		/// <summary>
		/// Draws widget
		/// </summary>
		/// <param name="evnt">
		/// Parameters <see cref="Gdk.EventExpose"/>
		/// </param>
		/// <returns>
		/// returns true <see cref="System.Boolean"/>
		/// </returns>
		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			buttonStyle = Rc.GetStyle (ChameleonTemplates.Button);

			if (widgets.Count > 0) {
				Gtk.Requisition req, mainreq;
				Gdk.Rectangle rect = evnt.Area;
				int x,y,w,h = 0;
				mainreq = SizeRequest();
				evnt.Window.BeginPaintRect (evnt.Area);
				ExposeWidget (false, false, rect, evnt);
				if (Homogeneous == true) {
					int cellWidth = Allocation.Width / Count;
					if (Hovered > -1) {
						rect = new Rectangle (Hovered*cellWidth, 0, cellWidth, Allocation.Height);
						ExposeWidget (false, true, rect, evnt);
					}
					if (Selected > -1) {
						rect = new Rectangle (Selected*cellWidth, 0, cellWidth, Allocation.Height);
						ExposeWidget (true, false, rect, evnt);
					}
				}
				else {
					if (Hovered > -1) {
						if (widgets[Hovered].TranslateCoordinates (this, 0, 0, out x, out y) == true) {
							req = widgets[Hovered].SizeRequest();
							widgets[Hovered].TranslateCoordinates (this, req.Width, req.Height, out w, out h);
							rect = new Rectangle (x, 0, w-x, Allocation.Height);
							ChameleonTemplates.Button.State = StateType.Prelight;
							ExposeWidget (false, true, rect, evnt);
							ChameleonTemplates.Button.State = StateType.Normal;
						}
					}
					if (Selected > -1) {
						if (widgets[Selected].TranslateCoordinates (this, 0, 0, out x, out y) == true) {
							req = widgets[Selected].SizeRequest();
							widgets[Selected].TranslateCoordinates (this, req.Width, req.Height, out w, out h);
							rect = new Rectangle (x, 0, w-x, Allocation.Height);
							ExposeWidget (true, false, rect, evnt);
						}
					}
				}
				// Draw children
				if (Child != null)
					PropagateExpose (Child, evnt);
				
				buttonStyle.Dispose();
				return (true);
			}
			base.OnExposeEvent (evnt);
			return (true);
		}
		
		public void Pack (Widget aWidget)
		{
			if (aWidget == null)
				return;
			
			if ((Count > 0) && (SeparatorLines == true)) {
				//System.Console.WriteLine("Separator");
				VSeparator vs = new VSeparator();
				vs.Show();
				widgets[Count-1].PackEnd (vs, false, false, 0);
			}

			widgets.Add (new HBox());
			widgets[Count-1].Spacing = 3;
			widgets[Count-1].Show();
			if (Homogeneous == true)
				mainbox.PackStart (widgets[Count-1], true, true, 0);
			else
				mainbox.PackStart (widgets[Count-1], false, false, 0);
			if (aWidget is HComboCell) {
				(aWidget as HComboCell).BorderWidth = 1;
				aWidget.Show();
				widgets[Count-1].PackStart (aWidget, true, true, 0);
			}
			else {
				HComboCell eb = new HComboCellWidget (aWidget);
				eb.BorderWidth = 1;
				aWidget.Show();
				eb.Show();
				widgets[Count-1].PackStart (eb, true, true, 0);
			}
			if (Count == 1)
				Selected = 0;
		}

		/// <summary>
		/// Handles ButtonPressEvent
		/// </summary>
		/// <param name="evnt">
		/// Arguments <see cref="EventButton"/>
		/// </param>
		/// <returns>
		/// true or false <see cref="System.Boolean"/>
		/// </returns>
		protected override bool OnButtonPressEvent (EventButton evnt)
		{
			if ((Hovered > -1) && (Hovered != Selected))
				Selected = Hovered;
			return (base.OnButtonPressEvent (evnt));
		}

		/// <summary>
		/// Handles LeaveNotifyEvent
		/// </summary>
		/// <param name="evnt">
		/// Arguments <see cref="EventCrossing"/>
		/// </param>
		/// <returns>
		/// true or false <see cref="System.Boolean"/>
		/// </returns>
		protected override bool OnLeaveNotifyEvent (EventCrossing evnt)
		{
			if (evnt.Mode == CrossingMode.Normal)
				Hovered = -1;
			
			return (base.OnLeaveNotifyEvent (evnt));
		}

		/// <summary>
		/// Handles MotionNotifyEvent
		/// </summary>
		/// <param name="evnt">
		/// Arguments <see cref="EventMotion"/>
		/// </param>
		/// <returns>
		/// true or false <see cref="System.Boolean"/>
		/// </returns>
		protected override bool OnMotionNotifyEvent (EventMotion evnt)
		{
			if (Count < 1) {
				Hovered = -1;
				return (false);
			}

			Gtk.Requisition mainreq = SizeRequest();
			if (Homogeneous == true) {
				Hovered = (System.Convert.ToInt32(evnt.X) / (Allocation.Width / Count));
				return (false);
			}
			int x,y,w,h = 0;
			Gtk.Requisition req;
			int i = 0;
			foreach (HBox cell in widgets) {
				if (cell.TranslateCoordinates (this, 0, 0, out x, out y) == true) {
					req = cell.SizeRequest();
					cell.TranslateCoordinates (this, req.Width, req.Height, out w, out h);
					if ((evnt.X >= x) && (evnt.X <= w)) {
						Hovered = i;
						return (false);
					}
					i++;
				}
			}
			
			Hovered = -1;
			return (false);
		}

		public HorizontalCombo()
		{
			Events |= (EventMask.PointerMotionMask | EventMask.ButtonPressMask | EventMask.VisibilityNotifyMask);
			VisibilityNotifyEvent += delegate { 
				QueueDraw(); 
			};

			AboveChild = true;
			mainbox.BorderWidth = 0;
			mainbox.Spacing = 0;
			mainbox.Homogeneous = true;
			Add (mainbox);
			mainbox.Show();
		}
	}
}
