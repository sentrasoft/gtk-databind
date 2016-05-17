//DatePickerWindow.cs - Field Attribute to assign additional information for Gtk#Databindings
//
//Author: m. <ml@arsis.net>
//
//Copyright (c) at 1:14 PM 1/11/2009
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
using Gtk;
using Gdk;

namespace Gtk.ExtraWidgets
{
	public partial class DatePickerWindow : Gtk.Window
	{
		private DateTime originalDate;
		private event DateEventHandler OnChange = null;
		private event EventHandler OnQuitEditing = null;

		public DateTime Date {
			get { return (calendar.Date); }
			set { calendar.Date = value;	}
		}
				
		public DatePickerWindow (int x, int y, int width, DateTime aDate, 
		                         DateEventHandler handler, EventHandler closehandler)
			: base(Gtk.WindowType.Popup)
		{
			Move (x, y);
//			HeightRequest = 5;
			Build();
			if (calendar.Requisition.Width > width)
				WidthRequest = calendar.Requisition.Width;
			else
				WidthRequest = width;
			calendar.Date = new DateTime (aDate.Year, aDate.Month, aDate.Day);
			originalDate = new DateTime (aDate.Year, aDate.Month, aDate.Day);
			OnChange = handler;
			OnQuitEditing = closehandler;
			BasicUtilities.FocusPopupGrab.GrabWindow(this);
		}
		
		private void Close()
		{
			BasicUtilities.FocusPopupGrab.RemoveGrab(this);
			OnChange = null;
			OnQuitEditing = null;
			Hide();
			Destroy();
			if (OnQuitEditing != null)
				OnQuitEditing(null, null);
		}

		protected virtual void OnCalendar2DaySelectedDoubleClick (object sender, System.EventArgs e)
		{
			if (OnChange != null) 
				OnChange(this, new DateEventArgs(Date));
			Close();
		}

		protected virtual void OnCalendar2DaySelected (object sender, System.EventArgs e)
		{
			if (OnChange != null) 
				OnChange(this, new DateEventArgs(Date));
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

		protected virtual void OnCalendarKeyReleaseEvent (object o, Gtk.KeyReleaseEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Escape) {
				calendar.Date = new DateTime (originalDate.Year, originalDate.Month, originalDate.Day);
				Close();
			}
			if (args.Event.Key == Gdk.Key.Return) {
				Close();
			}
		}
	}
}
