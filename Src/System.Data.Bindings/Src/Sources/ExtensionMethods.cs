//ExtensionMethods.cs - Field Attribute to assign additional information for Gtk#Databindings
//
//Author: m. <ml@arsis.net>
//
//Copyright (c) at 3:35 PM 12/31/2008
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
using System.Globalization;

namespace System.Data.Bindings
{
	public static class ExtensionMethods
	{
		public static string GetStringBetween (this string aStr, char aStartChar, char aEndChar)
		{
			int s = aStr.IndexOf(aStartChar);
			if (s < 0)
				return (null);
			int e = aStr.IndexOf(aEndChar, s+1);
			if (e < 0)
				return (null);
			return (null);
		}
		
		public static string GetStringBetween (this string aStr, char aChar)
		{
			return (aStr.GetStringBetween (aChar, aChar));
		}
		
		/// <summary>
		/// Converts EReadWrite to PropertyDefinition
		/// </summary>
		/// <param name="aState">
		/// State <see cref="EReadWrite"/>
		/// </param>
		/// <returns>
		/// PropertyDefinition state which equals original state <see cref="PropertyDefinition"/>
		/// </returns>
		public static PropertyDefinition GetPropertyDefinition (this EReadWrite aState)
		{
			return ((aState == EReadWrite.ReadWrite) ? PropertyDefinition.ReadWrite : PropertyDefinition.ReadOnly);
		}

		private static void SetSlovenianCommon (this CultureInfo aCultureInfo)
		{
			aCultureInfo.DateTimeFormat.DateSeparator = ".";
			aCultureInfo.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;
			aCultureInfo.DateTimeFormat.DayNames = new String[7] { "Nedelja", "Ponedeljek", "Torek", "Sreda", "Četrtek", "Petek", "Sobota" };
			aCultureInfo.DateTimeFormat.AbbreviatedDayNames = new String[7] { "Ned", "Pon", "Tor", "Sre", "Čet", "Pet", "Sob" };
			aCultureInfo.DateTimeFormat.ShortestDayNames = new String[7] { "N", "P", "T", "S", "Č", "P", "S" };
			System.Console.WriteLine(aCultureInfo.DateTimeFormat.GetDayName (DayOfWeek.Monday));
		}
		
		/// <summary>
		/// Sets correct handling for slovenian CultureInfo
		/// </summary>
		[RelatedBug ("Normal [Incorrect handling of Slovenian CultureInfo in .Net] Reffer to https://bugzilla.novell.com/show_bug.cgi?id=513002")]
		public static void SetSlovenian (this CultureInfo aCultureInfo)
		{
			if (aCultureInfo.Name != "sl-SI")
				return;
			aCultureInfo.SetSlovenianCommon();
			aCultureInfo.NumberFormat.CurrencySymbol = "€";
			aCultureInfo.NumberFormat.CurrencyNegativePattern = 8;
			aCultureInfo.NumberFormat.CurrencyPositivePattern = 3;
		}
		
		/// <summary>
		/// Sets correct handling for slovenian CultureInfo
		/// </summary>
		[RelatedBug ("Normal [Incorrect handling of Slovenian CultureInfo in .Net] Reffer to https://bugzilla.novell.com/show_bug.cgi?id=513002")]
		public static void SetSlovenianWithEUR (this CultureInfo aCultureInfo)
		{
			if (aCultureInfo.Name != "sl-SI")
				return;
			aCultureInfo.SetSlovenianCommon();
			aCultureInfo.NumberFormat.CurrencySymbol = "EUR";
			aCultureInfo.NumberFormat.CurrencyNegativePattern = 5;
			aCultureInfo.NumberFormat.CurrencyPositivePattern = 1;
		}
		
		/// <summary>
		/// Checks if specific event handler is already registered in event
		/// </summary>
		/// <param name="aHandler">
		/// Searched handler <see cref="Delegate"/>
		/// </param>
		/// <param name="aEventHandler">
		/// Event handler <see cref="Delegate"/>
		/// </param>
		/// <returns>
		/// true if present, false if not <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsEventHandlerRegistered (this Delegate aEventHandler, Delegate aHandler)
		{   
			if (aEventHandler != null) {
				foreach (Delegate method in aEventHandler.GetInvocationList()) {
					if (method == aHandler)
						return (true);
				}
			}
			return (false);
		}
		
		/// <summary>
		/// Returns path as string
		/// </summary>
		/// <returns>
		/// String interpretation of the path <see cref="System.String"/>
		/// </returns>
		public static string PathToString (this int[] aPath)
		{
			if (aPath == null)
				return ("[]");
			if (aPath.Length == 1)
				return ("[" + aPath[0] + "]");
			string res = "[";
			for (int i=0; i<(aPath.Length-1); i++)
				res = res + aPath[i] + ",";
			return (res + aPath[aPath.Length-1] + "]");
		}

		/// <summary>
		/// Adds new index on path start
		/// </summary>
		/// <param name="aIdx">
		/// Index to be inserted <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// New path <see cref="System.Int32"/>
		/// </returns>
		public static int[] AddPathIndexOnStart (this int[] aPath, int aIdx)
		{
			int[] newPath;
			if (aPath != null)
				newPath = new int [aPath.Length+1];
			else
				newPath = new int [1];
			if (aPath != null)
				aPath.CopyTo(newPath, 1);
			newPath[0] = aIdx;
			aPath = null;
			return (newPath);
		}
		
		/// <summary>
		/// Copies path
		/// </summary>
		/// <param name="aIdx">
		/// Path to be copied <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// Copy of the path <see cref="System.Int32"/>
		/// </returns>
		public static int[] CopyPath (this int[] aIdx)
		{
			int[] res = new int[aIdx.Length];
			aIdx.CopyTo (res, 0);
			return (res);
		}
		
		/// <summary>
		/// Adds new index on path end
		/// </summary>
		/// <param name="aIdx">
		/// Index to be inserted <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// New path <see cref="System.Int32"/>
		/// </returns>
		public static int[] AddPathIndexOnEnd (this int[] aPath, int aIdx)
		{
			int[] newPath;
			if (aPath != null)
				newPath = new int [aPath.Length+1];
			else
				newPath = new int [1];
			if (aPath != null)
				aPath.CopyTo(newPath, 0);
			newPath[newPath.Length-1] = aIdx;
			aPath = null;
			return (newPath);
		}

		/// <summary>
		/// Compares two paths
		/// </summary>
		/// <param name="aToPath">
		/// Second path <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// true if paths are equal <see cref="System.Boolean"/>
		/// </returns>
		public static bool PathIsEqual (this int[] aFromPath, int[] aToPath)
		{
			if (aFromPath.Length != aToPath.Length)
				return (false);
			for (int i=0; i<aFromPath.Length; i++)
				if (aFromPath[i] != aToPath[i])
					return (false);
			return (true);
		}

		/// <summary>
		/// Compares two paths
		/// </summary>
		/// <param name="aToPath">
		/// Second path <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// 0 if paths are equal
		/// 1 if first path is greater
		/// -1 if second path is greater <see cref="System.Int32"/>
		/// </returns>
		public static int ComparePath (this int[] aFromPath, int[] aToPath)
		{
			bool fromdef = false;
			int max = aFromPath.Length;
			bool canequal = (aFromPath.Length == aToPath.Length);
			
			if (aFromPath.Length > aToPath.Length) {
				max = aToPath.Length;
				fromdef = true;
			}
			
			for (int i=0; i<max; i++) {
				if (aFromPath[i] == aToPath[i]) {
					if (i == (max-1))
						if (canequal == true)
							return (0);
						else
							if (fromdef == true)
								return (1);
							else
								return (-1);
				}
				else
					if (aFromPath[i] > aToPath[i])
						return (1);
					else
						return (-1);
			}
			
			// will never happen' :)
			return (0);
		}
	}
}
