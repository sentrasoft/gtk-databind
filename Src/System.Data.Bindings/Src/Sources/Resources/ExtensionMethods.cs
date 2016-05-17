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
	/// Provides helper methods for resources
	/// </summary>
	public static class ResourceExtensionMethods
	{
		/// <summary>
		/// Loads text resource by using specified assembly for starting point
		/// </summary>
		/// <param name="aAssembly">
		/// Starting assembly <see cref="Assembly"/>
		/// </param>
		/// <param name="aResourceName">
		/// Resource name <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Resource text if found <see cref="System.String"/>
		/// </returns>
		/// <remarks>
		/// If specified assembly does not contain resource then referenced assemblies
		/// are searched for it
		/// </remarks>
		public static string LoadTextResource (this Assembly aAssembly, string aResourceName)
		{
			return (aAssembly.LoadTextResource (aResourceName, true));
		}

		/// <summary>
		/// Loads text resource by using specified assembly for starting point if specified
		/// </summary>
		/// <param name="aAssembly">
		/// Starting assembly <see cref="Assembly"/>
		/// </param>
		/// <param name="aResourceName">
		/// Resource name <see cref="System.String"/>
		/// </param>
		/// <param name="aUseCompleteAssembly">
		/// If true then all referenced assemblies will be searched for resource too <see cref="System.Boolean"/>
		/// </param>
		/// <returns>
		/// Resource text if found <see cref="System.String"/>
		/// </returns>
		public static string LoadTextResource (this Assembly aAssembly, string aResourceName, bool aUseCompleteAssembly)
		{
			if (aUseCompleteAssembly == false)
				return (ResourceEnumerator.LoadTextResourceFromAssembly (aAssembly, aResourceName));
			else {
				Assembly loc = ResourceEnumerator.FindResource (aAssembly, aResourceName);
				if (loc == null)
					throw new ResourceNotFoundException (aResourceName);
				return (ResourceEnumerator.LoadTextResourceFromAssembly (loc, aResourceName));
			}
		}
		
		/// <summary>
		/// Loads text resource by using specified assembly for starting point
		/// </summary>
		/// <param name="aAssembly">
		/// Starting assembly <see cref="Assembly"/>
		/// </param>
		/// <param name="aResourceName">
		/// Resource name <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Resource text if found <see cref="System.Byte"/>
		/// </returns>
		/// <remarks>
		/// If specified assembly does not contain resource then referenced assemblies
		/// are searched for it
		/// </remarks>
		public static byte[] LoadBinaryResource (this Assembly aAssembly, string aResourceName)
		{
			return (aAssembly.LoadBinaryResource (aResourceName, true));
		}

		/// <summary>
		/// Loads binary resource by using specified assembly for starting point if specified
		/// </summary>
		/// <param name="aAssembly">
		/// Starting assembly <see cref="Assembly"/>
		/// </param>
		/// <param name="aResourceName">
		/// Resource name <see cref="System.String"/>
		/// </param>
		/// <param name="aUseCompleteAssembly">
		/// If true then all referenced assemblies will be searched for resource too <see cref="System.Boolean"/>
		/// </param>
		/// <returns>
		/// Resource text if found <see cref="System.Byte"/>
		/// </returns>
		public static byte[] LoadBinaryResource (this Assembly aAssembly, string aResourceName, bool aUseCompleteAssembly)
		{
			if (aUseCompleteAssembly == false)
				return (ResourceEnumerator.LoadBinaryResourceFromAssembly (aAssembly, aResourceName));
			else {
				Assembly loc = ResourceEnumerator.FindResource (aAssembly, aResourceName);
				if (loc == null)
					throw new ResourceNotFoundException (aResourceName);
				return (ResourceEnumerator.LoadBinaryResourceFromAssembly (loc, aResourceName));
			}
		}
		
		
	}
}
