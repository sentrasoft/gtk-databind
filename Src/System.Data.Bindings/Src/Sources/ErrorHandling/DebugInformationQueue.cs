//ErrorStorage.cs - Description
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
using System.Data.Bindings;
using System.Collections.Specialized;

namespace System.Data.Bindings.DebugInformation
{
	/// <summary>
	/// Provides error storage for connected handlers to access it
	/// 
	/// Main difference is that external handlers can be connected
	/// </summary>
	public static class DebugInformationQueue
	{
		private static StringCollection filters = new StringCollection();
		
		/// <summary>
		/// Pushes application message
		/// </summary>
		/// <param name="aMessage">
		/// Message to be pushed in queue <see cref="AppNotification"/>
		/// </param>
		public static void Push (DebugNotification aMessage)
		{
			if (aMessage == null)
				return;
			if (Debug.Active == true)
				if (onMessagePush != null)
					onMessagePush (aMessage);
		}

		/// <summary>
		/// Disconnects all debug handlers
		/// </summary>
		public static void DisconnectAll()
		{
			onMessagePush = null;
		}
		
		private static event DebugHandlerEvent onMessagePush = null;
		/// <summary>
		/// All interested debug handlers should connect to this event 
		/// </summary>
		public static event DebugHandlerEvent OnMessagePush {
			add { onMessagePush += value; }
			remove { onMessagePush -= value; }
		}
	}
}
