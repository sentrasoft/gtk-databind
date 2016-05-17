//Debug.cs - Description
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
	/// Static class which defines basic debug parameters, which are supposed
	/// to te handled by error handlers connected to DebugInformationQueue
	/// </summary>
	public static class Debug
	{
		private static bool active = false;
		/// <summary>
		/// Defines if debuging is active or not 
		/// </summary>
		/// <remarks>
		/// Even if debugging is active there have to be connected event
		/// handlers to DebugInformationQueue for display to happen 
		/// </remarks>
		public static bool Active {
			get { return (active); }
			set { active = value; }
		}

		private static bool developmentInfo = false;
		/// <summary>
		/// Defines if debuging development information is active or not 
		/// </summary>
		public static bool DevelopmentInfo {
			get { return (developmentInfo); }
			set { developmentInfo = value; }
		}

		private static int level = 3;
		/// <summary>
		/// Defines information level debugging should provide 
		/// </summary>
		/// <remarks>
		/// Debugging level is something which should be interpretted by error handler
		/// connected to DebugInformationQueue
		/// </remarks>
		public static int Level {
			get { return (level); }
			set { level = value; }
		}

		/// <summary>
		/// Pushes warning to DebugInformationQueue
		/// </summary>
		/// <param name="aTitle">
		/// Title to be displayed <see cref="System.String"/>
		/// </param>
		public static void Warning (string aTitle)
		{
			Warning (aTitle, "", null);
		}

		/// <summary>
		/// Pushes warning to DebugInformationQueue
		/// </summary>
		/// <param name="aTitle">
		/// Title to be displayed <see cref="System.String"/>
		/// </param>
		/// <param name="aText">
		/// Text of displayed message <see cref="System.String"/>
		/// </param>
		public static void Warning (string aTitle, string aText)
		{
			Warning (aTitle, aText, null);
		}

		/// <summary>
		/// Pushes warning to DebugInformationQueue
		/// </summary>
		/// <param name="aTitle">
		/// Title to be displayed <see cref="System.String"/>
		/// </param>
		/// <param name="aText">
		/// Text of displayed message <see cref="System.String"/>
		/// </param>
		/// <param name="aObject">
		/// Object to be passed with message <see cref="System.Object"/>
		/// </param>
		public static void Warning (string aTitle, string aText, object aObject)
		{
			DebugInformationQueue.Push (new DebugNotification (DebugNotificationType.Warning, aTitle, aText, aObject));
		}
		
		/// <summary>
		/// Pushes suggestion to DebugInformationQueue
		/// </summary>
		/// <param name="aTitle">
		/// Title to be displayed <see cref="System.String"/>
		/// </param>
		public static void Suggestion (string aTitle)
		{
			Suggestion (aTitle, "", null);
		}
		
		/// <summary>
		/// Pushes suggestion to DebugInformationQueue
		/// </summary>
		/// <param name="aTitle">
		/// Title to be displayed <see cref="System.String"/>
		/// </param>
		/// <param name="aText">
		/// Text of displayed message <see cref="System.String"/>
		/// </param>
		public static void Suggestion (string aTitle, string aText)
		{
			Suggestion (aTitle, aText, null);
		}
		
		/// <summary>
		/// Pushes suggestion to DebugInformationQueue
		/// </summary>
		/// <param name="aTitle">
		/// Title to be displayed <see cref="System.String"/>
		/// </param>
		/// <param name="aText">
		/// Text of displayed message <see cref="System.String"/>
		/// </param>
		/// <param name="aObject">
		/// Object to be passed with message <see cref="System.Object"/>
		/// </param>
		public static void Suggestion (string aTitle, string aText, object aObject)
		{
			DebugInformationQueue.Push (new DebugNotification (DebugNotificationType.Suggestion, aTitle, aText, aObject));
		}
		
		/// <summary>
		/// Pushes suggestion to DebugInformationQueue
		/// </summary>
		/// <param name="aTitle">
		/// Title to be displayed <see cref="System.String"/>
		/// </param>
		/// <remarks>
		/// This message is only valid for DEBUG mode compiled assembly
		/// </remarks>
		public static void DevelInfo (string aTitle)
		{
			if (DevelopmentInfo == false)
				return;
			DevelInfo (aTitle, "", null);
		}
		
		/// <summary>
		/// Pushes suggestion to DebugInformationQueue
		/// </summary>
		/// <param name="aTitle">
		/// Title to be displayed <see cref="System.String"/>
		/// </param>
		/// <param name="aText">
		/// Text of displayed message <see cref="System.String"/>
		/// </param>
		/// <remarks>
		/// This message is only valid for DEBUG mode compiled assembly
		/// </remarks>
		public static void DevelInfo (string aTitle, string aText)
		{
			if (DevelopmentInfo == false)
				return;
			DevelInfo (aTitle, aText, null);
		}
		
		/// <summary>
		/// Pushes development info to DebugInformationQueue
		/// </summary>
		/// <param name="aTitle">
		/// Title to be displayed <see cref="System.String"/>
		/// </param>
		/// <param name="aText">
		/// Text of displayed message <see cref="System.String"/>
		/// </param>
		/// <param name="aObject">
		/// Object to be passed with message <see cref="System.Object"/>
		/// </param>
		/// <remarks>
		/// This message is only valid for DEBUG mode compiled assembly
		/// </remarks>
		public static void DevelInfo (string aTitle, string aText, object aObject)
		{
			if (DevelopmentInfo == false)
				return;
			DebugInformationQueue.Push (new DebugNotification (DebugNotificationType.DevelInfo, aTitle, aText, aObject));
		}
		
		/// <summary>
		/// Pushes error to DebugInformationQueue
		/// </summary>
		/// <param name="aTitle">
		/// Title to be displayed <see cref="System.String"/>
		/// </param>
		public static void Error (string aTitle)
		{
			Error (aTitle, "", null);
		}
		
		/// <summary>
		/// Pushes error to DebugInformationQueue
		/// </summary>
		/// <param name="aTitle">
		/// Title to be displayed <see cref="System.String"/>
		/// </param>
		/// <param name="aText">
		/// Text of displayed message <see cref="System.String"/>
		/// </param>
		public static void Error (string aTitle, string aText)
		{
			Error (aTitle, aText, null);
		}
		
		/// <summary>
		/// Pushes error to DebugInformationQueue
		/// </summary>
		/// <param name="aTitle">
		/// Title to be displayed <see cref="System.String"/>
		/// </param>
		/// <param name="aText">
		/// Text of displayed message <see cref="System.String"/>
		/// </param>
		/// <param name="aObject">
		/// Object to be passed with message <see cref="System.Object"/>
		/// </param>
		public static void Error (string aTitle, string aText, object aObject)
		{
			DebugInformationQueue.Push (new DebugNotification (DebugNotificationType.Error, aTitle, aText, aObject));
		}
		
		/// <summary>
		/// Forces pushing error to DebugInformationQueue
		/// </summary>
		/// <param name="aTitle">
		/// Title to be displayed <see cref="System.String"/>
		/// </param>
		public static void ForceError (string aTitle)
		{
			ForceError (aTitle, "", null);
		}
		
		/// <summary>
		/// Forces pushing error to DebugInformationQueue
		/// </summary>
		/// <param name="aTitle">
		/// Title to be displayed <see cref="System.String"/>
		/// </param>
		/// <param name="aText">
		/// Text of displayed message <see cref="System.String"/>
		/// </param>
		public static void ForceError (string aTitle, string aText)
		{
			ForceError (aTitle, aText, null);
		}
		
		/// <summary>
		/// Forces pushing error to DebugInformationQueue
		/// </summary>
		/// <param name="aTitle">
		/// Title to be displayed <see cref="System.String"/>
		/// </param>
		/// <param name="aText">
		/// Text of displayed message <see cref="System.String"/>
		/// </param>
		/// <param name="aObject">
		/// Object to be passed with message <see cref="System.Object"/>
		/// </param>
		public static void ForceError (string aTitle, string aText, object aObject)
		{
			bool debugState = Debug.Active;
			Active = true;
			DebugInformationQueue.Push (new DebugNotification (DebugNotificationType.Error, aTitle, aText, aObject));
			Active = debugState;
		}
		
		/// <summary>
		/// Pushes critical to DebugInformationQueue
		/// </summary>
		/// <param name="aTitle">
		/// Title to be displayed <see cref="System.String"/>
		/// </param>
		public static void Critical (string aTitle)
		{
			Critical (aTitle, "", null);
		}		

		/// <summary>
		/// Pushes critical to DebugInformationQueue
		/// </summary>
		/// <param name="aTitle">
		/// Title to be displayed <see cref="System.String"/>
		/// </param>
		/// <param name="aText">
		/// Text of displayed message <see cref="System.String"/>
		/// </param>
		public static void Critical (string aTitle, string aText)
		{
			Critical (aTitle, aText, null);
		}		

		/// <summary>
		/// Pushes critical to DebugInformationQueue
		/// </summary>
		/// <param name="aTitle">
		/// Title to be displayed <see cref="System.String"/>
		/// </param>
		/// <param name="aText">
		/// Text of displayed message <see cref="System.String"/>
		/// </param>
		/// <param name="aObject">
		/// Object to be passed with message <see cref="System.Object"/>
		/// </param>
		public static void Critical (string aTitle, string aText, object aObject)
		{
			DebugInformationQueue.Push (new DebugNotification (DebugNotificationType.Critical, aTitle, aText, aObject));
		}		
	}
}