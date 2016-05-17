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

namespace System.Data.Bindings
{
	/// <summary>
	/// Base of the triggers. It is also the main storage for the
	/// client connections and trigger maintenance  
	/// </summary>
	public static class Notificator
	{
		private static AppNotification appMessage = new AppNotification();
		///<summary>
		/// Provides common ground for cross application message events.
		/// Based on this gui for displaying these event should be provided
		/// and connected.
		///</summary>
		public static AppNotification AppMessage {
			get { return (appMessage); }
		}

		/// <summary>
		/// Called by TriggerWrapper when object is unlocked
		/// </summary>
		/// <param name="aObject">
		/// Object that has been unlocked <see cref="System.Object"/>
		/// </param>
		public static void ObjectUnlocked (object aObject)
		{
			if (aObject != null)
				ObjectChangedNotification (aObject, null);
		}

		/// <summary>
		/// Executes notification in Triggers engine
		/// </summary>
		/// <param name="aObject">
		/// DataSource Object which was subject to event <see cref="System.Object"/>
		/// </param>
		/// <param name="aType">
		/// Type of event <see cref="TriggerType"/>
		/// </param>
		public static void Notify (object aObject, TriggerType aType)
		{
			Notify (aObject, aType, null);
		}
		
		/// <summary>
		/// Executes notification in Triggers engine
		/// </summary>
		/// <param name="aObject">
		/// DataSource Object which was subject to event <see cref="System.Object"/>
		/// </param>
		/// <param name="aType">
		/// Type of event <see cref="TriggerType"/>
		/// </param>
		/// <param name="aChangedBy">
		/// Object that caused the event, this client will avoid update <see cref="System.Object"/>
		/// </param>
		public static void Notify (object aObject, TriggerType aType, object aChangedBy)
		{
			if (aType == TriggerType.ObjectChanged)
				ObjectChangedNotification (aObject, aChangedBy);
			else
				ReloadObjectNotification (aObject, aChangedBy);
		}
		
		/// <summary>
		/// Executes notification ObjectChanged in Triggers engine
		/// </summary>
		/// <param name="aObject">
		/// DataSource Object which was subject to event <see cref="System.Object"/>
		/// </param>
		public static void ObjectChangedNotification (object aObject)
		{
			ObjectChangedNotification (aObject, aObject);
		}
		
		/// <summary>
		/// Executes notification ReloadObject in Triggers engine
		/// </summary>
		/// <param name="aObject">
		/// DataSource Object which was subject to event <see cref="System.Object"/>
		/// </param>
		public static void ReloadObjectNotification (object aObject)
		{
			ReloadObjectNotification (aObject, aObject);
		}

		/// <summary>
		/// Executes notification in Triggers engine
		/// </summary>
		/// <param name="aObject">
		/// DataSource Object which was subject to event <see cref="System.Object"/>
		/// </param>
		/// <param name="aChangedBy">
		/// Object that caused the event, this client will avoid update <see cref="System.Object"/>
		/// </param>
		public static void ObjectChangedNotification (object aObject, object aChangedBy)
		{
			if (aObject == null)
				return;
			DataSourceController.GetRequest (aObject);
		}
		
		/// <summary>
		/// Executes notification ReloadObject in Triggers engine
		/// </summary>
		/// <param name="aObject">
		/// DataSource Object which was subject to event <see cref="System.Object"/>
		/// </param>
		/// <param name="aChangedBy">
		/// Object that caused the event, this client will avoid update <see cref="System.Object"/>
		/// </param>
		public static void ReloadObjectNotification (object aObject, object aChangedBy)
		{
			if (aObject == null)
				return;
			DataSourceController.PostRequest (aObject);			
		}
	}
}
