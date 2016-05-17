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
using System.Data.Bindings;

namespace System.Data.Bindings.DebugInformation
{	
	/// <summary>
	/// Provides debug notification for application to use it with
	/// DebugInformationQueue
	/// </summary>
	public class DebugNotification
	{
		private bool titleloaded = false;
		private bool textloaded = false;
		
		private DebugNotificationType msgType = DebugNotificationType.None;
		/// <value>
		/// Describes debug message type
		/// </value>
		public DebugNotificationType MsgType {
			get { return (msgType); }
			set {
				if (msgType == value)
					return;
				msgType = value;
				System.Data.Bindings.Notificator.ObjectChangedNotification (this);
			}
		}

		private string titleString = "";
		/// <summary>
		/// Message Title index or string. If OnLoadMsgString is assigned then
		/// this will load new string into Title by calling the event function.
		/// If OnLoadMsgString is not assigned then this will simply copy value
		/// in Title property
		/// </summary>
		/// <remarks>
		/// Contains raw value of Title
		/// </remarks>
		protected string TitleString {
			get { return (titleString); }
			set {
				if (titleString == value)
					return;
				titleString = value;
				title = "";
				titleloaded = false;
				System.Data.Bindings.Notificator.ObjectChangedNotification (this);
			}
		}

		private string title = "";
		///<summary>
		/// Returns correct (loaded or direct) value of TitleString 
		///</summary>
		public string Title {
			get {
				if (titleString == "")
					return ("");
				if (title != "")
					return (title);
				if ((onLoadMsgString != null) && (titleloaded == false)) {
					titleloaded = true;
					title = onLoadMsgString (titleString);
				}
				if (title == "")
					return (titleString);
				else
					return (title);
			}
		}

		private string textString = "";
		///<summary>
		/// Message Text index or string. If OnLoadMsgString is assigned then
		/// this will load new string into Text by calling the event function.
		/// If OnLoadMsgString is not assigned then this will simply copy value
		/// in Text property
		///</summary>
		protected string TextString {
			set {
				if (textString == value)
					return;
				textString = value;
				text = "";
				textloaded = false;
				System.Data.Bindings.Notificator.ObjectChangedNotification (this);
			}
		}

		private string text = "";
		///<summary>
		/// Returns correct (loaded or direct) value of TextString 
		///</summary>
		public string Text {
			get {
				if (textString == "")
					return ("");
				if (text != "")
					return (text);
				if ((onLoadMsgString != null) && (textloaded == false)) {
					textloaded = true;
					text = onLoadMsgString (textString);
				}
				if (text == "")
					return (textString);
				else
					return (text);
			}
		}

		///<summary>
		/// Returns if message should be visible or not
		///</summary>
		public bool Visible {
			get { return (((MsgType != DebugNotificationType.None) && (Title != "") && (Text != ""))); }
		}

		private event LoadMsgStringEvent onLoadMsgString = null;
		/// <summary>
		/// Stores event which should be called if strings need to be loaded 
		/// </summary>
		public event LoadMsgStringEvent OnLoadMsgString {
			add { OnLoadMsgString += value; }
			remove { OnLoadMsgString -= value; }
		}
		
		/// <summary>
		/// Clears the value of Notification
		/// </summary>
		public void Clear()
		{
			Set (DebugNotificationType.None, "", "", null);
		}

		private object attachedObject = null;
		/// <summary>
		/// Object attached to message
		/// </summary>
		public object AttachedObject {
			get { return (attachedObject); }
			set { attachedObject = value; }
		}

		/// <summary>
		/// Sets debug message with new values
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
		public virtual bool Set (DebugNotificationType aMsgType, string aTitle, string aText, object aObject)
		{
			if ((aMsgType == msgType) && (aTitle == titleString) && (aText == textString))
				return (false);
			msgType = aMsgType;
			textString = aText;
			titleString = aTitle;
			titleloaded = false;
			textloaded = false;
			return (true);
		}

		/// <summary>
		/// Creates a copy of specified notification, usefull if one wishes 
		/// to provide same information binding wise as debug wise
		/// </summary>
		/// <param name="aMsg">
		/// Notification which must be copied <see cref="DebugNotification"/>
		/// </param>
		/// <returns>
		/// Copy of notification, null if original was null <see cref="DebugNotification"/>
		/// </returns>
		public static DebugNotification Copy (DebugNotification aMsg)
		{
			if (aMsg == null)
				return (null);
			return (new DebugNotification (aMsg.MsgType, aMsg.TitleString, aMsg.Text, aMsg.AttachedObject));
		}
		
		/// <summary>
		/// Creates debug message
		/// </summary>
		public DebugNotification()
		{
		}
		
		/// <summary>
		/// Creates debug message
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
		public DebugNotification (DebugNotificationType aMsgType, string aTitle, string aText, object aObject)
		{
			Set (aMsgType, aTitle, aText, aObject);
		}

		/// <summary>
		/// Disconnects and destroys
		/// </summary>
		~DebugNotification()
		{
			onLoadMsgString = null;
		}
	}
}