// TypeValidator.cs - TypeValidator implementation for Gtk#Databindings
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
using System.Reflection;
using System.Collections.Specialized;

namespace System.Data.Bindings
{
	/// <summary>
	/// Provides validation routines for type checking
	/// </summary>
	public static class TypeValidator
	{
		/// <summary>
		/// Specifies Value type 
		/// </summary>
		public enum ValueType
		{
			/// <summary>
			/// Unknown value type 
			/// </summary>
			UNKNOWN,
			/// <summary>
			/// String value type 
			/// </summary>
			STRING,
			/// <summary>
			/// Int value type 
			/// </summary>
			INT,
			/// <summary>
			/// Float value type 
			/// </summary>
			FLOAT,
			/// <summary>
			/// Date value type 
			/// </summary>
			DATE,
			/// <summary>
			/// Time value type 
			/// </summary>
			TIME,
			/// <summary>
			/// DateTime value type 
			/// </summary>
			DATETIME
		}

		public static Adaptor stringColumnAdaptor = null;
		/// <value>
		/// Creates or returns default column adaptor based on string list
		/// </value>
		public static Adaptor StringColumnAdaptor {
			get {
				if (stringColumnAdaptor == null) {
					stringColumnAdaptor = new Adaptor();
					stringColumnAdaptor.Mappings = "string[Strings]<<;";
				}
				return (stringColumnAdaptor); 
			}
		}
		
		/// <summary>
		/// Handles basic IVirtualObject type 
		/// </summary>
		public static System.Type typeIVirtualObject = LoadType ("System.Data.Bindings.IVirtualObject");
		
		/// <summary>
		/// Loads type by string, use with complete name
		/// </summary>
		/// <param name="aName">
		/// Type name <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="Type"/>
		/// </returns>
		/// <remarks>
		/// On wrong type returns with Exception
		/// </remarks>
		public static Type LoadType (string aName)
		{
			return (Type.GetType (aName, true));
		}
		
		/// <summary>
		/// Checks if types are compatible
		/// </summary>
		/// <param name="aType">
		/// First type <see cref="Type"/>
		/// </param>
		/// <param name="aCheckAgainst">
		/// Second type <see cref="Type"/>
		/// </param>
		/// <returns>
		/// true if types are compatible <see cref="System.Boolean"/>
		/// </returns>
		/// <remarks>
		/// Used internaly to check DataSource assign when DataSource is constricted
		/// to some type
		/// </remarks>
		public static bool IsCompatible (Type aType, Type aCheckAgainst)
		{
			if (aCheckAgainst == null)
				return (true);
			if (aCheckAgainst.IsInterface == true) {
				Type[] interfaces = aType.GetInterfaces();
				foreach (Type type in interfaces)
					if (aCheckAgainst == type)
						return (true);
			}
			else
				if (aType.Equals(aCheckAgainst) == true)
					return (true);
				else
					return (aType.IsSubclassOf (aCheckAgainst));
			return (false);
		}
		
		/// <summary>
		/// Checks if types are compatible
		/// </summary>
		/// <param name="aType">
		/// First type <see cref="Type"/>
		/// </param>
		/// <param name="aCheckAgainst">
		/// Second type <see cref="Type"/>
		/// </param>
		/// <returns>
		/// true if types are compatible <see cref="System.Boolean"/>
		/// </returns>
		/// <remarks>
		/// Used internaly to check DataSource assign when DataSource is constricted
		/// to some type
		/// </remarks>
		public static bool IsCompatible (Type aType, Type[] aCheckAgainst)
		{
			if (aCheckAgainst == null)
				return (true);
			foreach (Type type in aCheckAgainst) 
				if (IsCompatible(aType, type) == true)
					return (true);
			return (false);
		}
		
		/// <summary>
		/// Resolves mapping type from type which was probably specified and resolved by name
		/// </summary>
		/// <param name="aDataSource">
		/// DataSource object to help resolve <see cref="System.Object"/>
		/// </param>
		/// <param name="aType">
		/// Object type <see cref="System.Type"/>
		/// </param>
		/// <param name="aMapping">
		/// Property name <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Type of property or Exception <see cref="System.Type"/>
		/// </returns>
		public static System.Type GetMappingType (object aDataSource, System.Type aType, string aMapping)
		{
			if ((DatabaseProvider.IsValidType(aType) == true) ||
			    (DatabaseProvider.IsDataRowContainer(aType) == true)) {
				System.Type type = null;
				DataColumn col = (DataColumn) DatabaseProvider.PropertyExists (aDataSource, aMapping, true);
				if (col != null)
					type = col.DataType;
				return (type);
			}
			return (ConnectionProvider.ResolveMappingTypeByType (aType, aMapping, true));
		}

		/// <summary>
		/// Resolves if type is any int or uint variation
		/// </summary>
		/// <param name="aType">
		/// Type to be checked <see cref="System.Type"/>
		/// </param>
		/// <returns>
		/// true if type in any int or uint variation <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsInt (System.Type aType)
		{
			return ((aType == typeof (byte)) ||
			        (aType == typeof (Int16)) ||
			        (aType == typeof (UInt16)) ||
			        (aType == typeof (Int32)) ||
			        (aType == typeof (UInt32)) ||
			        (aType == typeof (Int64)) ||
			        (aType == typeof (UInt64)));
		}

		/// <summary>
		/// Resolves if type is float
		/// </summary>
		/// <param name="aType">
		/// Type to be checked <see cref="System.Type"/>
		/// </param>
		/// <returns>
		/// true if type is float or double <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsFloat (System.Type aType)
		{
			return ((aType == typeof (float)) ||
			        (aType == typeof (decimal)) ||
			        (aType == typeof (double)));
		}

		/// <summary>
		/// Resolves if type is numeric  
		/// </summary>
		/// <param name="aType">
		/// Type to be checked <see cref="System.Type"/>
		/// </param>
		/// <returns>
		/// true if IsInt or IsFloat return true <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsNumeric (System.Type aType)
		{
			return ((IsInt(aType) == true) || (IsFloat(aType) == true));
		}
		
		/// <summary>
		/// Checks if type is string type
		/// </summary>
		/// <param name="aType">
		/// Type to check <see cref="System.Type"/>
		/// </param>
		/// <returns>
		/// true if type is string type, false if not <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsString (System.Type aType)
		{
			return ((aType == typeof (string)));
		}
		
		/// <summary>
		/// Checks if type is string type
		/// </summary>
		/// <param name="aType">
		/// Type to check <see cref="System.Type"/>
		/// </param>
		/// <returns>
		/// true if type is DateTime type, false if not <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsDateTime (System.Type aType)
		{
			return ((aType == typeof (DateTime)));
		}
		
		/// <summary>
		/// Returns group description of data type contained by property
		/// </summary>
		/// <param name="aType">
		/// Type in question <see cref="System.Type"/>
		/// </param>
		/// <returns>
		/// Value type of given type <see cref="ValueType"/>
		/// </returns>
		public static ValueType GetValueType (System.Type aType)
		{
			// try to discover basics
			if (IsString(aType) == true)
				return (ValueType.STRING);
			if (IsInt(aType) == true)
				return (ValueType.INT);
			if (IsFloat(aType) == true)
				return (ValueType.FLOAT);
			if (IsDateTime(aType) == true)
				return (ValueType.DATETIME);
				
			// return as unknown value
			return (ValueType.UNKNOWN);
		}

		/// <summary>
		/// Current assembly wide search for the type, type will be found even
		/// if this assembly doesn't reference to the assembly where type resides
		/// As long as type is loaded, it will found it no matter what.
		/// </summary>
		/// <param name="aName">
		/// Name if type searched, has to be specified with namespace <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// null if unknown or Type if found <see cref="System.Type"/>
		/// </returns>
		public static System.Type GetTypeInWholeAssembly (string aName)
		{
			// Try to find type localy
			System.Type dtype = Type.GetType (aName, false); // static method
			if (dtype != null)
				return (dtype);
				
			// Try to find type in main assembly
			StringCollection cache = new StringCollection();
			// Since basic GetType already searches in mscorlib we can safely put it as already done
			cache.Add ("mscorlib");
			
			dtype = GetTypeInAssembly (Assembly.GetEntryAssembly(), aName, cache);
			cache.Clear();
			cache = null;
			
			return (dtype);
		}

		/// <summary>
		/// Current assembly wide search for the type, type will be found even
		/// if this assembly doesn't reference to the assembly where type resides
		/// As long as type is loaded, it will found it no matter what.
		/// </summary>
		/// <param name="aAssembly">
		/// Assembly to check in <see cref="Assembly"/>
		/// </param>
		/// <param name="aName">
		/// Name if type searched, has to be specified with namespace <see cref="System.String"/>
		/// </param>
		/// <param name="aCache">
		/// Cache of already checked libraries <see cref="StringCollection"/>
		/// </param>
		/// <returns>
		/// null if unknown or Type if found <see cref="System.Type"/>
		/// </returns>
		internal static System.Type GetTypeInAssembly (Assembly aAssembly, string aName, StringCollection aCache)
		{
			if (aAssembly == null)
				return ((System.Type) null);
				
			// Find type in assembly
			System.Type dtype = aAssembly.GetType (aName, false); // static method
			if (dtype != null)
				return (dtype);
				
			// Go trough assembly referenced assemblies
			foreach (AssemblyName mod in aAssembly.GetReferencedAssemblies())
				if (mod != null) {
					if (aCache.Contains(mod.Name) == false) {
						aCache.Add(mod.Name);
						dtype = GetTypeInAssembly (Assembly.Load(mod), aName, aCache);
					}
					
					if (dtype != null)
						return (dtype);
				}
				
			return ((System.Type) null);
		}
	}
}
