//Interfaces.cs - Description
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

namespace System.Data.Bindings.DebugInformation
{
	/// <summary>
	/// Provides various debug message types
	/// </summary>
	public enum DebugNotificationType
	{
		/// <summary>
		/// No type
		/// </summary>
		None,
		/// <summary>
		/// Development information
		/// </summary>
		DevelInfo,
		/// <summary>
		/// Custom message type
		/// </summary>
		Custom,
		/// <summary>
		/// Information
		/// </summary>
		Information,
		/// <summary>
		/// Suggestion
		/// </summary>
		Suggestion,
		/// <summary>
		/// Question
		/// </summary>
		Question,
		/// <summary>
		/// Warning
		/// </summary>
		Warning,
		/// <summary>
		/// Error
		/// </summary>
		Error,
		/// <summary>
		/// Critical
		/// </summary>
		Critical
	}

	/// <summary>
	/// Handles event where debug information was pushed into DebugInformationQueue
	/// </summary>
	/// <param name="aNotification">
	/// Debug Notification <see cref="DebugNotification"/>
	/// </param>
	public delegate void DebugHandlerEvent (DebugNotification aNotification);
}
