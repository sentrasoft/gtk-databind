//Notificator.cs - Description
//
//Author: m. <ml@arsis.net>
//
//Copyright (c) 2008 m.
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
//
//

using System;

namespace Gtk.DataBindings
{
	/// <summary>
	/// Adds simple redirection to avoid need for assembly to
	/// reference to System.Data.Bindings
	/// </summary>
	public static class Notificator
	{
		/// <summary>
		/// Executes notification ObjectChanged in Triggers engine
		/// </summary>
		/// <param name="aObject">
		/// DataSource Object which was subject to event <see cref="System.Object"/>
		/// </param>
		public static void ObjectChangedNotification (object aObject)
		{
			System.Data.Bindings.Notificator.ObjectChangedNotification (aObject, null);
		}
		
		/// <summary>
		/// Executes notification ReloadObject in Triggers engine
		/// </summary>
		/// <param name="aObject">
		/// DataSource Object which was subject to event <see cref="System.Object"/>
		/// </param>
		public static void ReloadObjectNotification (object aObject)
		{
			System.Data.Bindings.Notificator.ReloadObjectNotification (aObject, null);
		}
		
		/// <summary>
		/// Disconnects everything inside specified widget
		/// </summary>
		/// <param name="aWidget">
		/// Widget to be disconnected <see cref="Gtk.Widget"/>
		/// </param>
		public static void Disconnect (Gtk.Widget aWidget)
		{
			if (aWidget == null)
				return;
			
			if (System.Data.Bindings.TypeValidator.IsCompatible(aWidget.GetType(), typeof(Gtk.Container)) == true) {
				foreach (Gtk.Widget wdg in (aWidget as Gtk.Container).Children)
					Disconnect (wdg);
			}
			if ((aWidget is System.Data.Bindings.IDisconnectable) == true)
				(aWidget as System.Data.Bindings.IDisconnectable).Disconnect();
			else
				if ((aWidget is System.Data.Bindings.IChangeableControl) == true)
					(aWidget as System.Data.Bindings.IChangeableControl).Adaptor.Disconnect();
		}
	}
}
