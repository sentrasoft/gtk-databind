//AppNotification.cs - Description
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
using System.Data.Bindings.DebugInformation;

namespace System.Data.Bindings
{	
	/// <summary>
	/// Provides notifications for application to display in any
	/// wished form. 
	/// </summary>
	/// <remarks>
	/// This class is NOT to be used with DebugInformationQueue. It provides
	/// bindings notifications of it self and would clash with debugging
	/// </remarks>
	public class AppNotification : DebugNotification
	{
		/// <value>
		/// Defines Application message type. It is the same value as MsgType
		/// </value>
		/// <remarks>
		/// Use it only if this would be the only reason to have to reference
		/// to System.Data.Bindings
		/// </remarks>
		public AppNotificationType AppMsgType {
			get { return (ToAppMsgType (MsgType)); }
			set { MsgType = ToDebugMsgType (value); }
		}

		/// <summary>
		/// Sets appllication message and notifies all Adaptors connected to
		/// this object to update their information
		/// </summary>
		/// <param name="aMsgType">
		/// Message type <see cref="AppNotificationType"/>
		/// </param>
		/// <param name="aTitle">
		/// Message title <see cref="System.String"/>
		/// </param>
		/// <param name="aText">
		/// Message text <see cref="System.String"/>
		/// </param>
		/// <param name="aObject">
		/// Object to be passed allong with message <see cref="System.Object"/>
		/// </param>
		public override bool Set (DebugNotificationType aMsgType, string aTitle, string aText, object aObject)
		{
			bool res = base.Set (aMsgType, aTitle, aText, aObject);
			if (res == true)
				System.Data.Bindings.Notificator.ObjectChangedNotification (this);
			return (res);
		}

		internal static DebugNotificationType ToDebugMsgType (AppNotificationType aType)
		{
			switch (aType) {
			case AppNotificationType.Critical : return (DebugNotificationType.Critical);
			case AppNotificationType.Custom : return (DebugNotificationType.Custom);
			case AppNotificationType.Error : return (DebugNotificationType.Error);
			case AppNotificationType.Information : return (DebugNotificationType.Information);
			case AppNotificationType.None : return (DebugNotificationType.None);
			case AppNotificationType.Question : return (DebugNotificationType.Question);
			case AppNotificationType.Suggestion : return (DebugNotificationType.Suggestion);
			case AppNotificationType.Warning : return (DebugNotificationType.Warning);
			}
			return (DebugNotificationType.None);
		}

		internal static AppNotificationType ToAppMsgType (DebugNotificationType aType)
		{
			switch (aType) {
			case DebugNotificationType.Critical : return (AppNotificationType.Critical);
			case DebugNotificationType.Custom : return (AppNotificationType.Custom);
			case DebugNotificationType.Error : return (AppNotificationType.Error);
			case DebugNotificationType.Information : return (AppNotificationType.Information);
			case DebugNotificationType.None : return (AppNotificationType.None);
			case DebugNotificationType.Question : return (AppNotificationType.Question);
			case DebugNotificationType.Suggestion : return (AppNotificationType.Suggestion);
			case DebugNotificationType.Warning : return (AppNotificationType.Warning);
			}
			return (AppNotificationType.None);
		}
	}
}