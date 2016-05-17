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

namespace System.Data.Bindings
{	
	/// <summary>
	/// Provides additional WidgetFactory extension methods
	/// </summary>
	public static class FactoryExtensionMethods
	{
		/// <summary>
		/// Resolves Range attribute for property
		/// </summary>
		/// <param name="aInfo">
		/// Property info <see cref="PropertyInfo"/>
		/// </param>
		/// <returns>
		/// Range attribute or null <see cref="PropertyRangeAttribute"/>
		/// </returns>
		public static PropertyRangeAttribute GetPropertyRange (this PropertyInfo aInfo)
		{
			PropertyRangeAttribute[] attr = (PropertyRangeAttribute[]) aInfo.GetCustomAttributes (typeof(PropertyRangeAttribute), true);
			if ((attr != null) && (attr.Length > 0))
				return (attr[0]);
			return (null);
		}
		
		/// <summary>
		/// Resolves Description attribute for property
		/// </summary>
		/// <param name="aInfo">
		/// Property info <see cref="PropertyInfo"/>
		/// </param>
		/// <returns>
		/// Description attribute or null <see cref="PropertyDescriptionAttribute"/>
		/// </returns>
		public static PropertyDescriptionAttribute GetPropertyDescription (this PropertyInfo aInfo)
		{
			PropertyDescriptionAttribute[] attr = (PropertyDescriptionAttribute[]) aInfo.GetCustomAttributes (typeof(PropertyDescriptionAttribute), true);
			if ((attr != null) && (attr.Length > 0))
				return (attr[0]);
			return (null);
		}

		/// <summary>
		/// Adds current theme from WidgetFactory
		/// </summary>
		/// <param name="aArgs">
		/// Arguments <see cref="FactoryInvocationArgs"/>
		/// </param>
		public static void AddDefaultTheme (this FactoryInvocationArgs aArgs)
		{
			foreach (string s in WidgetFactory.DefaultTheme.Filters)
				aArgs.AddFilter (s);
		}

		/// <summary>
		/// Adds theme to creation parameters
		/// </summary>
		/// <param name="aArgs">
		/// Arguments <see cref="FactoryInvocationArgs"/>
		/// </param>
		/// <param name="aTheme">
		/// Theme <see cref="WidgetFactoryTheme"/>
		/// </param>
		public static void AddTheme (this FactoryInvocationArgs aArgs, WidgetFactoryTheme aTheme)
		{
			if (aTheme == null)
				return;
			foreach (string s in aTheme.Filters)
				aArgs.AddFilter (s);
		}
	}
}
