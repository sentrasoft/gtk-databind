// VersionNumber.cs - VersionNumber implementation for Gtk#Databindings
//
// Author: m. <ml@arsis.net>
//
// Copyright (c) 2006 m.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of version 2 of the Lesser GNU General 
// Public License as published by the Free Software Foundation.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this program; if not, write to the
// Free Software Foundation, Inc., 59 Temple Place - Suite 330,
// Boston, MA 02111-1307, USA.

using System;
using System.Data.Bindings.DebugInformation;

namespace System.Data.Bindings.Utilities
{
	//<summary>
	/// Version number resolver struct. Also provides complete comparing.
	/// It is pushing complex unix like versioning to the limits
	//</summary>
	public struct VersionNumber
	{
		//<summary>
		/// Returns Version number
		//</summary>
		public Int16[] VersionNr;
		
		/// <summary>
		/// Resolves if Version is beta, alpha or final
		/// </summary>
		/// <value>
		/// true if version specifies final release, false if not
		/// </value>
		public bool IsFinalRelease {
			get { 
				for (int i=0; i<VersionNr.Length; i++)
					if (VersionNr[i] < 0)
						return (false);
				return (true); 
			}
		}
		
		//<summary>
		/// Provides == comparission operator function
		//</summary>
		/// <param name="aVal1">
		/// First value <see cref="VersionNumber"/>
		/// </param>
		/// <param name="aVal2">
		/// Second value <see cref="VersionNumber"/>
		/// </param>
		/// <returns>
		/// true if versions are equal <see cref="System.Boolean"/>
		/// </returns>
		public static bool operator == (VersionNumber aVal1, VersionNumber aVal2)
		{
			if (aVal1.VersionNr.Length != aVal2.VersionNr.Length)
				return (false);
			for (int i=0; i<aVal1.VersionNr.Length; i++)
				if (aVal1.VersionNr[i] != aVal2.VersionNr[i])
					return (false);
			return (true);
		}

		//<summary>
		/// Provides != comparission operator function
		//</summary>
		/// <param name="aVal1">
		/// First value <see cref="VersionNumber"/>
		/// </param>
		/// <param name="aVal2">
		/// Second value <see cref="VersionNumber"/>
		/// </param>
		/// <returns>
		/// true if versions are not equal <see cref="System.Boolean"/>
		/// </returns>
		public static bool operator != (VersionNumber aVal1, VersionNumber aVal2)
		{
			return (! (aVal1 == aVal2));
		}

		//<summary>
		/// Provides > comparission operator function
		//</summary>
		/// <param name="aVal1">
		/// First value <see cref="VersionNumber"/>
		/// </param>
		/// <param name="aVal2">
		/// Second value <see cref="VersionNumber"/>
		/// </param>
		/// <returns>
		/// true if first value is greater than second <see cref="System.Boolean"/>
		/// </returns>
		public static bool operator > (VersionNumber aVal1, VersionNumber aVal2)
		{
			int i1 = aVal1.VersionNr.Length;
			int i2 = aVal2.VersionNr.Length;
			int len;
			if (i1 < i2)
				len = i1;
			else
				len = i2;
			for (int i=0; i<len; i++)
				if (aVal1.VersionNr[i] > aVal2.VersionNr[i])
					return (true);
				else
					if (aVal1.VersionNr[i] < aVal2.VersionNr[i])
						return (false);
			// length is equal and so were all numbers
			if (i1 == i2)
				return (false);
			else {
				// Fallback to check if next sub release is defining lower or higher number
				if (i1 > i2)
					return (aVal1.VersionNr[i2] >= 0);
				else
        			return (aVal2.VersionNr[i1] >= 0);
        	}
		}

		/// <summary>
		/// Provides < comparission operator function
		/// </summary>
		/// <param name="aVal1">
		/// First value <see cref="VersionNumber"/>
		/// </param>
		/// <param name="aVal2">
		/// Second value <see cref="VersionNumber"/>
		/// </param>
		/// <returns>
		/// true if first value is lower than second <see cref="System.Boolean"/>
		/// </returns>
		public static bool operator < (VersionNumber aVal1, VersionNumber aVal2)
		{
        	return (! ((aVal1 == aVal2) || (aVal1 > aVal2)));
		}

		/// <summary>
		/// Provides <= comparission operator function
		/// </summary>
		/// <param name="aVal1">
		/// First value <see cref="VersionNumber"/>
		/// </param>
		/// <param name="aVal2">
		/// Second value <see cref="VersionNumber"/>
		/// </param>
		/// <returns>
		/// true if first value is lower or equal than second <see cref="System.Boolean"/>
		/// </returns>
		public static bool operator <= (VersionNumber aVal1, VersionNumber aVal2)
		{
			return ((aVal1 == aVal2) || (aVal1 < aVal2));
		}
		
		/// <summary>
		/// Provides >= comparission operator function
		/// </summary>
		/// <param name="aVal1">
		/// First value <see cref="VersionNumber"/>
		/// </param>
		/// <param name="aVal2">
		/// Second value <see cref="VersionNumber"/>
		/// </param>
		/// <returns>
		/// true if first value is greater or equal than second <see cref="System.Boolean"/>
		/// </returns>
		public static bool operator >= (VersionNumber aVal1, VersionNumber aVal2)
		{
			return ((aVal1 == aVal2) || (aVal1 > aVal2));
		}
		
		/// <summary>
		/// Resolves single number from string as part of FromString()
		/// </summary>
		/// <param name="aStr">
		/// String containing version <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Returns sortable version of descriptive named version <see cref="Int16"/>
		/// </returns>
		private Int16 NumberFromString (string aStr)
		{
			string s = aStr.ToUpper(); 
			switch (s) {
				case "" : return (0);
				case "B" : return (-1);
				case "BETA" : return (-1);
				case "A" : return (-2);
				case "ALPHA" : return (-2);
				default :
					try { return (System.Convert.ToInt16(s)); } 
					catch {
						Debug.Error ("Translation[Major] of " + aStr + " to VersionNumber failed");
						return (-3);
					}						
			}
		}
		
		/// <summary>
		/// Resolve complete version from string 
		/// </summary>
		/// <param name="aStr">
		/// Resolves version from string <see cref="System.String"/>
		/// </param>
		private void FromString (string aStr)
		{
			string s = aStr;
			string s2;
			int i = 0;
			int j = s.IndexOf(".");
			while (j >= 0) {
				if (j == 0) {
					s = s.Remove(0, 1);
					VersionNr[i] = 0;
				}
				else {
					s2 = s.Substring(0, j);
					VersionNr[i] = NumberFromString (s2);
					s = s.Remove(0, j + 1);
				}
				i++;
				if (i > 30)
					return;
				j = s.IndexOf(".");
			}
			if (s != "")
				VersionNr[VersionNr.Length - 1] = NumberFromString (s);
		}

		/// <summary>
		/// Comares if two VersionNumber objects are the same
		/// </summary>
		/// <param name="aObj">
		/// Object to compare with <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Returns true if equal <see cref="System.Boolean"/>
		/// </returns>
		/// <remarks>
		/// For compare can be used either string or VersionNumber
		/// otherwise it simply returns false
		/// </remarks>
		public override bool Equals (object aObj)
		{
			VersionNumber ver;
			if (aObj is VersionNumber) {
				ver = (VersionNumber) aObj;
				return (this == ver);
			}

			if (aObj is string) {
				ver = new VersionNumber ((string) aObj);
				bool res = (this == ver);
				return (res);
			}

			return (false);
		}
		
		/// <summary>
		/// Returns HashCode from string based value of this VersionNumber
		/// </summary>
		/// <returns>
		/// HashCode of version string
		/// </returns>
		public override int GetHashCode()
		{
			return (ToString().GetHashCode());
		}

		/// <summary>
		/// Returns string value for VersionNumber
		/// </summary>
		/// <returns>
		/// Returns formated string of version
		/// </returns>
		public override string ToString()
		{
			string res = "";
			for (int i=0; i<VersionNr.Length; i++) {
				switch (VersionNr[i]) {
					case -1 :
						res = res + "beta";
						break;
					case -2 :
						res = res + "alpha";
						break;
					default :
						res = res + VersionNr[i];
						break;
				}
				if (i < (VersionNr.Length - 1))
					res = res + ".";
			}
			return (res);
		}

		//<summary>
		/// Creates version number fom string. String should be as "ver.ver.ver..."
		/// Just posing next dot, means same as posing 0 and alpha, beta are applying 
		/// negative values.
		//</summary>
		public VersionNumber (string aVersion)
		{
			string s = aVersion;
			int i = 1;
			int j = s.IndexOf(".");
			VersionNr = new Int16[3] {0, 0, 0};
			while (j >=0) {
				s = s.Remove(j, 1);
				i++;
				if (i > 20)
					return;
				j = s.IndexOf(".");
			}
			VersionNr = new Int16[i];
			FromString(aVersion);
		}
	}
}