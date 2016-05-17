//  
//  Copyright (C) 2009 matooo
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System;

namespace Gtk.ExtraWidgets
{
	public enum TransferDataType
	{
		Default = 0,
		X_Special = 1
	}
	
	/// <summary>
	/// Provides methods to handle clipboard
	/// </summary>
	public static class ClipboardHelper
	{
		private static TargetList textTargets = null;
		private static string[] textClipboardTargetList = null;
		public static string[] TextTargetList {
			get {
				if (textClipboardTargetList != null)
					return (textClipboardTargetList);
				if (textTargets == null) {
					textTargets = new TargetList();
					textTargets.AddTextTargets ((int) TransferDataType.Default);
				}
				string[] res = new string [((TargetEntry[]) textTargets).Length];
				for (int i=0; i<res.Length; i++)
					res[i] = ((TargetEntry[]) textTargets)[i].Target;
				return (res);
			}
		}

		public static TargetList GetTextTargetList()
		{
			TargetList targetlist = new TargetList();
			targetlist.AddTextTargets ((int) TransferDataType.Default);
			return (targetlist);
		}
		
		public static TargetEntry[] GetTextTargetsWithSpecial (string aName, uint aType) 
		{
			TargetList targetlist = GetTextTargetList();
			if (aName.Trim() != "")
				targetlist.Add (aName.Trim(), 0, aType);
			return ((TargetEntry[]) targetlist);
		}
		
		public static Clipboard GetGlobalClipboard()
		{
			return (Clipboard.Get (Gdk.Atom.Intern ("CLIPBOARD", true)));
		}
	}
}
