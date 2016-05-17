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
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace System.Data.Bindings
{
	public static class AttributeExtensionMethods
	{
		/// <summary>
		/// Resolves enum layout for databinding, mainly only specifies if enum 
		/// fields specify icons or not
		/// </summary>
		/// <param name="aEnum">
		/// Enum type <see cref="System.Type"/>
		/// </param>
		/// <returns>
		/// Columns <see cref="System.String"/>
		/// </returns>
		public static string[] GetEnumLayout (this System.Type aEnum)
		{
			if (aEnum.IsEnum == false)
				throw new NotSupportedException ("GetEnumLayout is only supported by enumeration types");
			bool icon = false;
			foreach (FieldInfo fi in aEnum.GetFields()) {
				Attribute[] attrs = (Attribute[]) fi.GetCustomAttributes (false);
				foreach (Attribute attr in attrs)
					if (TypeValidator.IsCompatible(attr.GetType(), typeof(ItemIconAttribute)) == true) {
						icon = true;
						break;
					}
			}
			if (icon == true)
				return (new string[2] {"icon", "title"});
			else
				return (new string[1] {"title"});
		}
		
		/// <summary>
		/// Returns enumeration description if it exists, otherwise it returns
		/// enumeration as string
		/// </summary>
		/// <param name="aEnum">
		/// Enumeration value <see cref="Enum"/>
		/// </param>
		/// <returns>
		/// Title <see cref="System.String"/>
		/// </returns>
		public static string GetEnumTitle (this Enum aEnum)
		{
			string desc = aEnum.ToString();
			FieldInfo fi = aEnum.GetType().GetField (desc);
			return (fi.GetEnumTitle());
		}
		
		/// <summary>
		/// Returns enumeration description if it exists, otherwise it returns
		/// enumeration as string
		/// </summary>
		/// <param name="aFieldInfo">
		/// Enumeration value <see cref="FieldInfo"/>
		/// </param>
		/// <returns>
		/// Title <see cref="System.String"/>
		/// </returns>
		public static string GetEnumTitle (this FieldInfo aFieldInfo)
		{
			ItemTitleAttribute[] attrs = (ItemTitleAttribute[]) aFieldInfo.GetCustomAttributes (typeof(ItemTitleAttribute), false);
			if ((attrs != null) && (attrs.Length > 0))
				return (attrs[0].Title);
			return (aFieldInfo.Name);
		}
		
		/// <summary>
		/// Returns enumeration hint if it exists, otherwise it returns
		/// enumeration as string
		/// </summary>
		/// <param name="aFieldInfo">
		/// Enumeration value <see cref="FieldInfo"/>
		/// </param>
		/// <returns>
		/// Title <see cref="System.String"/>
		/// </returns>
		public static string GetEnumHint (this FieldInfo aFieldInfo)
		{
			ItemHintAttribute[] attrs = (ItemHintAttribute[]) aFieldInfo.GetCustomAttributes (typeof(ItemHintAttribute), false);
			if ((attrs != null) && (attrs.Length > 0))
				return (attrs[0].Hint);
			return ("");
		}
		
		/// <summary>
		/// Returns enumeration icon if it exists, otherwise it returns null
		/// </summary>
		/// <param name="aFieldInfo">
		/// Enumeration value <see cref="FieldInfo"/>
		/// </param>
		/// <returns>
		/// Title <see cref="System.String"/>
		/// </returns>
		public static object GetEnumIcon (this FieldInfo aFieldInfo)
		{
			ItemIconAttribute[] attrs = (ItemIconAttribute[]) aFieldInfo.GetCustomAttributes (typeof(ItemIconAttribute), false);
			if ((attrs != null) && (attrs.Length > 0))
				return (attrs[0].Picture);
			return (null);
		}

		/// <summary>
		/// Returns column view for specified type
		/// </summary>
		/// <param name="aType">
		/// Class type <see cref="System.Type"/>
		/// </param>
		/// <returns>
		/// Array of column view names <see cref="System.String"/>
		/// </returns>
		public static string[] GetColumnViews (this System.Type aType)
		{
			List<string> lst = new List<string>();
			ColumnViewAttribute[] attrs = (ColumnViewAttribute[]) aType.GetCustomAttributes (typeof(ColumnViewAttribute), false);
			foreach (ColumnViewAttribute attr in attrs)
				if (lst.IndexOf(attr.Name) > -1)
					lst.Add (attr.Name);
			
			attrs = (ColumnViewAttribute[]) aType.GetCustomAttributes (typeof(ColumnViewAttribute), true);
			foreach (ColumnViewAttribute attr in attrs)
				if (lst.IndexOf(attr.Name) > -1)
					lst.Add (attr.Name);

			if (lst.Count == 0)
				return (null);
			string[] res = new string[lst.Count];
			for (int i=0; i<lst.Count; i++)
				res[i] = lst[i];
			lst.Clear();
			return (res);
		}
		
		private static string ResolveSubViews (System.Type aType, string aMappings)
		{
			int i = aMappings.IndexOf("#");
			//if (i < 0)
				return (aMappings);
			//aMappings.
		}
		
		/// <summary>
		/// Resolves column view and returns its mappings
		/// </summary>
		/// <param name="aType">
		/// Type <see cref="System.Type"/>
		/// </param>
		/// <param name="aName">
		/// View name <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Mappings <see cref="System.String"/>
		/// </returns>
		/// <remarks>
		/// If view specifies subview then this is resolved accordingly
		/// </remarks>
		public static string GetColumnView (this System.Type aType, string aName)
		{
			ColumnViewAttribute[] attrs = (ColumnViewAttribute[]) aType.GetCustomAttributes (typeof(ColumnViewAttribute), false);
			foreach (ColumnViewAttribute attr in attrs)
				if (aName == attr.Name)
					return (ResolveSubViews (aType, attr.Mappings));
			attrs = (ColumnViewAttribute[]) aType.GetCustomAttributes (typeof(ColumnViewAttribute), true);
			foreach (ColumnViewAttribute attr in attrs)
				if (aName == attr.Name)
					return (ResolveSubViews (aType, attr.Mappings));
			throw new Exception (string.Format ("View {0} was not found in {1}", aName, aType.Name));
		}
	}
}
