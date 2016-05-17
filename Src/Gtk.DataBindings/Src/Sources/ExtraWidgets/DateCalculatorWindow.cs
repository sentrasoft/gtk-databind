//DateCalculatorWindow.cs - Field Attribute to assign additional information for Gtk#Databindings
//
//Author: m. <ml@arsis.net>
//
//Copyright (c) at 6:16 AM 1/25/2009
//
//This program is free software; you can redistribute it and/or
//modify it under the terms of version 2 of the Lesser GNU General 
//Public License as published by the Free Software Foundation.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this program; if not, write to the
//Free Software Foundation, Inc., 59 Temple Place - Suite 330,
//Boston, MA 02111-1307, USA.

using System;

namespace Gtk.ExtraWidgets
{
	public partial class DateCalculatorWindow : Gtk.Window
	{
		private DateTime originalDate;
		private event DateEventHandler OnChange = null;
		private event EventHandler OnQuitEditing = null;
		private CustomSpinEntry spinbutton2 = new CustomSpinEntry (-100000, 100000, 1);
		
		private DateTime date = DateTime.Now;
		public DateTime Date {
			get { return (date); }
			set {
				if (date.Equals(value) == true)
					return;
				date = value;
				if (OnChange != null) 
					OnChange(this, new DateEventArgs(date));
			}
		}
		
		public DateCalculatorWindow (int x, int y, int width, DateTime aDate, 
		                             DateEventHandler handler, EventHandler closehandler)
			: base(Gtk.WindowType.Popup)
		{
			Move (x, y);
			WidthRequest = width;
//			HeightRequest = 5;
			this.Build();
			date = new DateTime (aDate.Year, aDate.Month, aDate.Day);
			originalDate = new DateTime (aDate.Year, aDate.Month, aDate.Day);
			spinbutton2.Value = 0;
			spinbutton2.SpecialOnReleaseKeys.Add (Gdk.Key.Return);
//			spinbutton2.SpecialOnReleaseKeys.Add (Gdk.Key.Return);
			spinbutton2.ValueChanged += OnSpinbutton2ValueChanged;
			spinbutton2.SpecialKeyReleaseEvent += delegate(object sender, GdkKeyEventArgs args) {
				if (args.Key == Gdk.Key.Escape) {
					Date = originalDate;
					Close();
				}
				if (args.Key == Gdk.Key.Return) {
					Close();
				}
			};
			spinbutton2.Show();
			vbox1.PackStart (spinbutton2, true, true, 0);
			
			OnChange = handler;
			OnQuitEditing = closehandler;
			BasicUtilities.FocusPopupGrab.GrabWindow(this);
			spinbutton2.GrabFocus();
		}
		
		private void Close()
		{
			spinbutton2.ValueChanged -= OnSpinbutton2ValueChanged;
			BasicUtilities.FocusPopupGrab.RemoveGrab(this);
			OnChange = null;
			OnQuitEditing = null;
			Hide();
			Destroy();
			if (OnQuitEditing != null)
				OnQuitEditing(null, null);
		}

		protected virtual void OnButtonPressEvent (object o, Gtk.ButtonPressEventArgs args)
		{
			int w, h;
			GetSize (out w, out h);
			if ((args.Event.X < 0) ||
			    (args.Event.Y < 0) ||
			    (args.Event.X > w) ||
			    (args.Event.Y > h))
				Close();
		}

		protected virtual void OnKeyReleaseEvent (object o, Gtk.KeyReleaseEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Escape) {
				Date = originalDate;
				Close();
			}
			if (args.Event.Key == Gdk.Key.Return) {
				Close();
			}
		}

		protected virtual void OnSpinbutton2ValueChanged (object sender, System.EventArgs e)
		{
			Date = originalDate.AddDays(spinbutton2.ValueAsInt);
		}

		protected virtual void OnKeyPressEvent (object o, Gtk.KeyPressEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Escape) {
				Date = originalDate;
				Close();
			}
			if (args.Event.Key == Gdk.Key.Return) {
				Close();
			}
		}
	}
}
