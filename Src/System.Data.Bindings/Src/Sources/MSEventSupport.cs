//MSEventSupport.cs - Field Attribute to assign additional information for Gtk#Databindings
//
//Author: m. <ml@arsis.net>
//
//Copyright (c) at 11:57 PM 12/21/2008
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
using System.ComponentModel;
using System.Data.Bindings.Collections;
using System.Reflection;

namespace System.Data.Bindings
{
	/// <summary>
	/// Provides support to connect to PropertyChangedEventHandler as
	/// used in INotifyPropertyChanged interface.
	///
	/// This can even connect to the same event even if interface is not
	/// specified
	/// </summary>
	public static class MSEventSupport
	{
		/// <summary>
		/// Static method to handle SDB notification
		/// </summary>
		/// <param name="aSender">
		/// Object that changed <see cref="System.Object"/>
		/// </param>
		/// <param name="e">
		/// Will be ignored as SDB doesn't use names for flexibility and speed <see cref="PropertyChangedEventArgs"/>
		/// </param>
		internal static void ObjectPropertyChanged (object aSender, PropertyChangedEventArgs e)
		{
			if (aSender != null)
				Notificator.ObjectChangedNotification (aSender);
		}

		internal static PropertyChangedEventHandler OnObjectPropertyChanged = new PropertyChangedEventHandler (ObjectPropertyChanged);
		
		/// <summary>
		/// To ensure connection didn't happen twice it first checks if
		/// adaptor with that object already exists.
		/// If Adaptor with this object specified as Target is already
		/// present then object was connected already anyway
		/// </summary>
		/// <param name="aObject">
		/// Object being connected <see cref="System.Object"/>
		/// </param>
		/// <remarks>
		/// If object being connected to is type of PropertyChangedEventHandler
		/// then connection to PropertyChanged happens, otherwise it
		/// connects to all PropertyChangedEventHandler properties
		/// </remarks>
		public static void ConnectEvent (object aObject)
		{
			if (aObject == null)
				return;
			if (DataSourceController.CountDataSourceOwners(aObject) > 1)
				return;
			if (aObject is INotifyPropertyChanged)
				(aObject as INotifyPropertyChanged).PropertyChanged += OnObjectPropertyChanged;
			else
				// Connect to all PropertyChangedEventHandler properties
				foreach (EventInfo ev in aObject.GetType().GetEvents())
					if (ev.EventHandlerType is PropertyChangedEventHandler)
						ev.AddEventHandler (aObject, OnObjectPropertyChanged);
		}

		/// <summary>
		/// Disconect only happens if only one Adaptor has this object 
		/// specified as Target.
		/// If not then connection is still valid
		/// </summary>
		/// <param name="aObject">
		/// Object being diconnected <see cref="System.Object"/>
		/// </param>
		/// <remarks>
		/// If object being disconnected from is type of PropertyChangedEventHandler
		/// then disconnection from PropertyChanged happens, otherwise it
		/// disconnects from all PropertyChangedEventHandler properties
		/// </remarks>
		public static void DisconnectEvent (object aObject)
		{
			if (aObject == null)
				return;
			if (DataSourceController.CountDataSourceOwners(aObject) > 1)
				return;
			if (aObject is INotifyPropertyChanged)
				(aObject as INotifyPropertyChanged).PropertyChanged -= OnObjectPropertyChanged;
			else
				// Disconnect from all PropertyChangedEventHandler properties
				foreach (EventInfo ev in aObject.GetType().GetEvents())
					if (ev.EventHandlerType is PropertyChangedEventHandler)
//						ev.RemoveEventHandler (aObject, new PropertyChangedEventHandler(ObjectPropertyChanged));
						ev.RemoveEventHandler (aObject, OnObjectPropertyChanged);
		}

		public static void ConnectEventToIObservableList (IObservableList aList, object aObject)
		{
			if ((aList == null) || (aObject == null))
				return;
			if (aObject is INotifyPropertyChanged)
				(aObject as INotifyPropertyChanged).PropertyChanged += aList.GetDefaultNotifyPropertyChangedHandler();
			else
				// Disconnect from all PropertyChangedEventHandler properties
				foreach (EventInfo ev in aObject.GetType().GetEvents())
					if (ev.EventHandlerType is PropertyChangedEventHandler)
						ev.AddEventHandler (aObject, aList.GetDefaultNotifyPropertyChangedHandler());
		}
		
		[Obsolete ("Class ObserveableList will be replaced with ObservableList")]
		public static void ConnectEventToObserveableList (IObserveableList aList, object aObject)
		{
			if ((aList == null) || (aObject == null))
				return;
			int i = 0;
			foreach (object o in aList)
				if (o == aObject)
					i++;
			if (i > 1)
				return;
			if (TypeValidator.IsCompatible(aList.GetType(), typeof(ObserveableList)) == false)
				return;
			if (aObject is INotifyPropertyChanged)
				(aObject as INotifyPropertyChanged).PropertyChanged += (aList as ObserveableList).ListItemPropertyChangedMethod;
			else
				// Disconnect from all PropertyChangedEventHandler properties
				foreach (EventInfo ev in aObject.GetType().GetEvents())
					if (ev.EventHandlerType is PropertyChangedEventHandler)
						ev.AddEventHandler (aObject, (aList as ObserveableList).ListItemPropertyChangedMethod);
		}

		public static void DisconnectEventFromIObservableList (IObservableList aList, object aObject)
		{
			if ((aList == null) || (aObject == null))
				return;
			if (aObject is INotifyPropertyChanged)
				(aObject as INotifyPropertyChanged).PropertyChanged -= aList.GetDefaultNotifyPropertyChangedHandler();
			else
				// Disconnect from all PropertyChangedEventHandler properties
				foreach (EventInfo ev in aObject.GetType().GetEvents())
					if (ev.EventHandlerType is PropertyChangedEventHandler)
						ev.RemoveEventHandler (aObject, aList.GetDefaultNotifyPropertyChangedHandler());
		}

		[Obsolete ("Class ObserveableList will be replaced with ObservableList")]
		public static void DisconnectEventFromObserveableList (IObserveableList aList, object aObject)
		{
			if ((aList == null) || (aObject == null))
				return;
			int i = 0;
			foreach (object o in aList)
				if (o == aObject)
					i++;
			if (i > 1)
				return;
			if (TypeValidator.IsCompatible(aList.GetType(), typeof(ObserveableList)) == false)
				return;
			if (aObject is INotifyPropertyChanged)
				(aObject as INotifyPropertyChanged).PropertyChanged -= (aList as ObserveableList).ListItemPropertyChangedMethod;
			else
				// Disconnect from all PropertyChangedEventHandler properties
				foreach (EventInfo ev in aObject.GetType().GetEvents())
					if (ev.EventHandlerType is PropertyChangedEventHandler)
						ev.RemoveEventHandler (aObject, (aList as ObserveableList).ListItemPropertyChangedMethod);
		}
	}
}
