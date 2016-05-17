// ModeButton.cs
// 
// Copyright (C) 2008 Christian Hergert <chris@dronelabs.com>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using Gtk;
using Gtk.DataBindings;
using Gdk;
using System.ComponentModel;

namespace Gtk.ExtraWidgets
{
	[ToolboxItem(true)]
	public class ModeButton: Gtk.EventBox
	{
		public event ModeButtonEventHandler ModeAdded;
		public event ModeButtonEventHandler ModeRemoved;
		public event ModeButtonEventHandler ModeChanged;
		
		private int modeCount = 0;
		public int ModeCount {
			get { return (modeCount); }
		}
		
		HBox m_box;
		Button m_button;

		private int selected = -1;
		public int Selected {
			get { return (selected); }
			set {
				if (ModeCount <= 0)
					return;
				if ((value < -1) || (value >= m_box.Children.Length - 1))
					return;

				if (selected >= 0)
					m_box.Children [selected + 1].State = StateType.Normal;

				selected = value;
				m_box.Children[value+1].State = StateType.Selected;
				QueueDraw ();

				if (this.ModeChanged != null) {
					Widget selection = (value >= 0) ? m_box.Children[value+1] : null;
					this.ModeChanged (this, new ModeButtonEventArgs (value, selection));
				}
			}
		}

		private int m_Hovered = -1;
		public int Hovered {
			get { return (m_Hovered); }
			set {
				if (ModeCount == 0)
					return;
				if ((value < -1) || (value >= m_box.Children.Length - 1))
					return;
				
				this.m_Hovered = value;
				this.QueueDraw ();
			}
		}

		public void Append (Widget widget)
		{
			bool sens = (modeCount == 0);
				
			modeCount++;
			if (sens == false)
				Sensitive = true;
			
			m_box.PackStart (widget, true, true, 6);

			if (this.ModeAdded != null) {
				int index = m_box.Children.Length - 2;
				this.ModeAdded (this, new ModeButtonEventArgs (index, widget));
			}
		}

		public void Remove (int index)
		{
			if (ModeCount <= 0)
				return;
			Widget child = m_box.Children [index];
			modeCount--;
			if (ModeCount <= 0) {
				Sensitive = false;
			}
			m_box.Remove (child);

			if (selected == index)
				selected = -1;
			else if (selected >= index)
				selected--;

			if (m_Hovered >= index)
				m_Hovered--;

			if (ModeRemoved != null)
				ModeRemoved (this, new ModeButtonEventArgs (index, child));

			QueueDraw ();
		}

		protected override bool OnButtonPressEvent(EventButton evnt)
		{
			if ((m_Hovered > -1) && (m_Hovered != Selected))
				Selected = m_Hovered;
			return (base.OnButtonPressEvent (evnt));
		}

		protected override bool OnLeaveNotifyEvent(EventCrossing evnt)
		{
			if (evnt.Mode == CrossingMode.Normal) {
				m_Hovered = -1;
				QueueDraw ();
			}
			
			return (base.OnLeaveNotifyEvent (evnt));
		}

		protected override bool OnMotionNotifyEvent(EventMotion evnt)
		{
			int n_children = m_box.Children.Length - 1;

			if (n_children < 1)
				return (false);

			double child_size = this.Allocation.Width / n_children;
			int i = -1;

			if (child_size > 0)
				i = (int) (evnt.X / child_size);

			if (i >= 0 && i < n_children)
				this.Hovered = i;

			return false;
		}

		protected override bool OnExposeEvent(EventExpose evnt)
		{
			var clip_region = new Gdk.Rectangle (0, 0, 0, 0);
			var n_children = m_box.Children.Length - 1;
			//var inner_border = 2; // StyleGetProperty is broken

			m_button.Show ();
			m_button.Hide ();

			evnt.Window.BeginPaintRect (evnt.Area);

			Style.PaintBox (m_button.Style, evnt.Window, StateType.Normal,
							ShadowType.In, evnt.Area, m_button, "button",
							evnt.Area.X, evnt.Area.Y,
							evnt.Area.Width, evnt.Area.Height);

			if (Selected >= 0) {
				if (n_children > 1) {
					clip_region.Width = evnt.Area.Width / n_children;
					clip_region.X = (clip_region.Width * Selected) + 1;
				}
				else {
					clip_region.X = 0;
					clip_region.Width = evnt.Area.Width;
				}

				clip_region.Y = evnt.Area.Y;
				clip_region.Height = evnt.Area.Height;

				Style.PaintBox (m_button.Style, evnt.Window, StateType.Selected,
							    ShadowType.EtchedOut, clip_region, m_button, "button",
							    evnt.Area.X, evnt.Area.Y,
							    evnt.Area.Width, evnt.Area.Height);
			}

			if (Hovered >= 0 && Selected != Hovered) {
				if (n_children > 1) {
					clip_region.Width = evnt.Area.Width / n_children;
					if (Hovered == 0)
						clip_region.X = 0;
					else
						clip_region.X = clip_region.Width * Hovered + 1;
				}
				else {
					clip_region.X = 0;
					clip_region.Width = evnt.Area.Width;
				}

				clip_region.Y = evnt.Area.Y;
				clip_region.Height = evnt.Area.Height;

				Style.PaintBox (m_button.Style, evnt.Window, StateType.Prelight,
							    ShadowType.In, clip_region, m_button, "button",
							    evnt.Area.X, evnt.Area.Y,
							    evnt.Area.Width, evnt.Area.Height);
			}

			/*
			1.UpTo (n_children).ForEach (i => {
				double offset = (evnt.Area.Width / n_children) * i;
				Style.PaintVline (m_button.Style, evnt.Window, StateType.Normal,
								  evnt.Area, m_button, "button",
								  evnt.Area.Y + inner_border + 1,
								  (int) (evnt.Area.Y + evnt.Area.Height - (inner_border * 2) - 1),
								  (int) (evnt.Area.X + offset + 1));
			});
			*/

			this.PropagateExpose (this.Child, evnt);

			evnt.Window.EndPaint ();
			
			return false;
		}

		public ModeButton ()
		{
			this.Events |= EventMask.PointerMotionMask
			            |  EventMask.ButtonPressMask
			            |  EventMask.VisibilityNotifyMask;

			this.VisibilityNotifyEvent += delegate { this.QueueDraw (); };

			Sensitive = false;
			
			m_box = new HBox (true, 6);
			m_box.BorderWidth = 6;
			Add (m_box);
			m_box.Show ();

			m_button = new Button ();
			m_button.Hide();
			m_box.PackStart (m_button, true, true, 0);
		}
	}
}