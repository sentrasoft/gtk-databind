//CustomSpinEntry.cs - Field Attribute to assign additional information for Gtk#Databindings
//
//Author: m. <ml@arsis.net>
//
//Copyright (c) at 12:50 PM 1/12/2009
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
using System.Collections;
using Gtk;
using Gdk;

namespace Gtk.ExtraWidgets
{
	public class CustomSpinEntry : SpinButton
	{
		private KeyInfoList specialOnPressKeys = new KeyInfoList();
		public KeyInfoList SpecialOnPressKeys {
			get { return (specialOnPressKeys); }
		}
		
		private KeyInfoList specialOnReleaseKeys = new KeyInfoList();
		public KeyInfoList SpecialOnReleaseKeys {
			get { return (specialOnReleaseKeys); }
		}

		private event GdkKeyEventHandler specialKeyPressEvent = null;
		public event GdkKeyEventHandler SpecialKeyPressEvent {
			add { specialKeyPressEvent += value; }
			remove { specialKeyPressEvent -= value; }
		}
		
		private event GdkKeyEventHandler specialKeyReleaseEvent = null;
		public event GdkKeyEventHandler SpecialKeyReleaseEvent {
			add { specialKeyReleaseEvent += value; }
			remove { specialKeyReleaseEvent -= value; }
		}
		
		protected override bool OnKeyReleaseEvent (Gdk.EventKey evnt)
		{
			if (SpecialOnReleaseKeys.Contains(evnt.Key) == true) {
				if (specialKeyReleaseEvent != null) 
					specialKeyReleaseEvent (this, new GdkKeyEventArgs (evnt.Key));
				return (true);
			}
			return (base.OnKeyReleaseEvent (evnt));
		}

		protected override bool OnKeyPressEvent (Gdk.EventKey evnt)
		{
			if (SpecialOnPressKeys.Contains(evnt.Key) == true) {
				if (specialKeyPressEvent != null) 
					specialKeyPressEvent (this, new GdkKeyEventArgs (evnt.Key));
				return (true);
			}
			return (base.OnKeyPressEvent (evnt));
		}
		
		public CustomSpinEntry (double aMin, double aMax, double aStep)
			: base (aMin, aMax, aStep)
		{
/*			Events =  EventMask.ExposureMask    | EventMask.LeaveNotifyMask   | EventMask.AllEventsMask     | 
				           EventMask.ScrollMask      | EventMask.EnterNotifyMask   | EventMask.ExposureMask      |
				       	   EventMask.ButtonPressMask | EventMask.PointerMotionMask | EventMask.ButtonReleaseMask |
				       	   EventMask.PointerMotionHintMask;*/
//			HasFrame = false;
//			SetFlag( Gtk.WidgetFlags.AppPaintable );
//			SetFlag( Gtk.WidgetFlags.Sensitive );
		}
	}
}
