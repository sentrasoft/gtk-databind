//ConsoleDebugHandler.cs - Description
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
using System.Diagnostics;

namespace System.Data.Bindings.DebugInformation
{
	/// <summary>
	/// Provides basic handler of pushed debug messages by writing them formated to
	/// Error Console
	/// </summary>
	public static class ConsoleDebug
	{
		/// <summary>
		/// Consoledebug main method
		/// </summary>
		/// <param name="aNotification">
		/// Notification to display <see cref="DebugNotification"/>
		/// </param>
		public static void ConsoleDebugger (DebugNotification aNotification)
		{
//#if DEBUG
			if (aNotification == null)
				return;
			bool pushfound = false;
			string type = aNotification.MsgType.ToString();
			Console.Error.WriteLine (type.ToUpper() + ": " +
			                         aNotification.Title + "\n" +
			                         "   " + aNotification.Text);
			if (Debug.Level > 5) {
				StackTrace st = new StackTrace(true);
				string stackIndent = "     ";
				for (int i =0; i<st.FrameCount; i++) {
					// Note that at this level, there are four
					// stack frames, one for each method invocation.
					StackFrame sf = st.GetFrame(i);
					string method = sf.GetMethod().ToString(); 
					if (method == "Void Push(System.DebugInformation.DebugNotification)") {
						pushfound = true;
						continue;
					}
					if (method == "Void Critical(System.String, System.String, System.Object)")
						continue;
					if (method == "Void Critical(System.String, System.String)")
						continue;
					if (method == "Void Critical(System.String)")
						continue;
					if (method == "Void Error(System.String, System.String, System.Object)")
						continue;
					if (method == "Void Error(System.String, System.String)")
						continue;
					if (method == "Void Error(System.String)")
						continue;
					if (method == "Void ForceError(System.String, System.String, System.Object)")
						continue;
					if (method == "Void ForceError(System.String, System.String)")
						continue;
					if (method == "Void ForceError(System.String)")
						continue;
					if (method == "Void Suggestion(System.String, System.String, System.Object)")
						continue;
					if (method == "Void Suggestion(System.String, System.String)")
						continue;
					if (method == "Void Suggestion(System.String)")
						continue;
					if (method == "Void Warning(System.String, System.String, System.Object)")
						continue;
					if (method == "Void Warning(System.String, System.String)")
						continue;
					if (method == "Void Warning(System.String)")
						continue;
					if (pushfound == false)
						continue;
					Console.Error.WriteLine();
					Console.Error.WriteLine(stackIndent + " Method: {0}", method);
					Console.Error.WriteLine(stackIndent + " File: {0}", sf.GetFileName());
					Console.Error.WriteLine(stackIndent + " Line Number: {0}", sf.GetFileLineNumber());
					stackIndent += "  ";
				}
			}
//#endif
		}
		
		/// <summary>
		/// Consoledebug main method
		/// </summary>
		/// <param name="aMessage">
		/// Notification to display <see cref="System.String"/>
		/// </param>
		public static void TraceStack (string aMessage)
		{
			if (aMessage != "")
				Console.Error.WriteLine (aMessage);
			StackTrace st = new StackTrace(true);
			string stackIndent = "  ";
			for (int i=0; i<st.FrameCount; i++) {
				if (i == 0)
					continue;
				// Note that at this level, there are four
				// stack frames, one for each method invocation.
				StackFrame sf = st.GetFrame(i);
				string method = sf.GetMethod().ToString(); 
//				Console.Error.WriteLine();
				Console.Error.WriteLine(stackIndent + " Method: {0}", method);
				Console.Error.WriteLine(stackIndent + " File: {0}", sf.GetFileName());
				Console.Error.WriteLine(stackIndent + " Line Number: {0}", sf.GetFileLineNumber());
				stackIndent += "  ";
			}
		}
		
		private static bool connected = false;

		/// <summary>
		/// Connects ConsoleDebug to DebugInformationQueue
		/// </summary>
		public static void Connect()
		{
			if (connected == true)
				return;
			DebugInformationQueue.OnMessagePush += ConsoleDebugger;
		}
		
		/// <summary>
		/// Disconnects ConsoleDebug from DebugInformationQueue
		/// </summary>
		public static void Disconnect()
		{
			if (connected == false)
				return;
			DebugInformationQueue.OnMessagePush -= ConsoleDebugger;
		}
	}
}
