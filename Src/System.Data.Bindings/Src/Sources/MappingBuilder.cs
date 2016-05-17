// ValueMapBuilder.cs - Field Attribute to assign additional information for Gtk#Databindings
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

namespace System.Data.Bindings
{
	/// <summary>
	/// Building class to easy create mapping string which can sometimes
	/// be quite annoying when doing it by hand 
	/// </summary>
	public class MappingBuilder
	{
		private System.Type objectType = null;
		/// <summary>
		/// Specifies type which is defined with mapping
		/// </summary>
		public System.Type ObjectType {
			get { return (objectType); }
			set { objectType = value; } 
		}

		private string val = "";
		/// <summary>
		/// Resulting string value of the mapping
		/// </summary>
		public string Value {
			get { return (val); }
			set { val = value; } 
		}

		/// <summary>
		/// Adds string to Value
		/// </summary>
		/// <param name="aCheck">
		/// Specifies if check is needed <see cref="System.Boolean"/>
		/// </param>
		/// <param name="aMap">
		/// Mapping <see cref="System.String"/>
		/// </param>
		private void Add (bool aCheck, string aMap)
		{
		}
		
		/// <summary>
		/// Adds string to Value
		/// </summary>
		/// <param name="aMap">
		/// Mapping <see cref="System.String"/>
		/// </param>
		public void Add (string aMap)
		{
			Add (true, aMap);
		}
		
		/// <summary>
		/// Adds ValueMap to Value
		/// </summary>
		/// <param name="aMap">
		/// Value mapping <see cref="ValueMap"/>
		/// </param>
		public void Add (ValueMap aMap)
		{
			if (aMap == null)
				return;
			if (aMap.Valid == false)
				return;
			Add (false, aMap.Value);
		}

		/// <summary>
		/// Clears Value and ObjectType
		/// </summary>
		public void Clear()
		{
			Value = "";
			ObjectType = null;
		}
		
		/// <summary>
		/// Builds string from passed parameters
		/// </summary>
		/// <param name="aStrings">
		/// Valid mapping strings <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// String representation of mapping <see cref="System.String"/>
		/// </returns>
		public static string Format (params string[] aStrings)
		{
			return (new MappingBuilder (aStrings).Value);
		}

		/// <summary>
		/// Builds string from passed parameters
		/// </summary>
		/// <param name="aType">
		/// Specifies type which is defined with mapping <see cref="System.Type"/>
		/// </param>
		/// <param name="aStrings">
		/// Valid mapping strings <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// String representation of mapping <see cref="System.String"/>
		/// </returns>
		public static string Format (System.Type aType, params string[] aStrings)
		{
			return (new MappingBuilder (aType, aStrings).Value);
		}

		/// <summary>
		/// Builds string from passed parameters
		/// </summary>
		/// <param name="aMaps">
		/// Mappings to add <see cref="ValueMap"/>
		/// </param>
		/// <returns>
		/// String representation of mapping <see cref="System.String"/>
		/// </returns>
		public static string Format (params ValueMap[] aMaps)
		{
			return (new MappingBuilder (aMaps).Value);
		}

		/// <summary>
		/// Builds string from passed parameters
		/// </summary>
		/// <param name="aType">
		/// Specifies type which is defined with mapping <see cref="System.Type"/>
		/// </param>
		/// <param name="aMaps">
		/// Mappings to add <see cref="ValueMap"/>
		/// </param>
		/// <returns>
		/// String representation of mapping <see cref="System.String"/>
		/// </returns>
		public static string Format (System.Type aType, params ValueMap[] aMaps)
		{
			return (new MappingBuilder (aType, aMaps).Value);
		}

		/// <summary>
		/// Creates MappingBuilder
		/// </summary>
		protected MappingBuilder()
		{
		}

		/// <summary>
		/// Creates MappingBuilder
		/// </summary>
		/// <param name="aStrings">
		/// Valid mapping strings <see cref="System.String"/>
		/// </param>
		public MappingBuilder (params string[] aStrings)
			: this (null, aStrings)
		{
		}

		/// <summary>
		/// Creates MappingBuilder
		/// </summary>
		/// <param name="aType">
		/// Specifies type which is defined with mapping <see cref="System.Type"/>
		/// </param>
		/// <param name="aStrings">
		/// Valid mapping strings <see cref="System.String"/>
		/// </param>
		public MappingBuilder (System.Type aType, params string[] aStrings)
		{
			ObjectType = aType;
			foreach (string map in aStrings)
				Add (map);
		}

		/// <summary>
		/// Creates MappingBuilder
		/// </summary>
		/// <param name="aMaps">
		/// Mappings to add <see cref="ValueMap"/>
		/// </param>
		public MappingBuilder (params ValueMap[] aMaps)
			: this (null, aMaps)
		{
		}

		/// <summary>
		/// Creates MappingBuilder
		/// </summary>
		/// <param name="aType">
		/// Specifies type which is defined with mapping <see cref="System.Type"/>
		/// </param>
		/// <param name="aMaps">
		/// Mappings to add <see cref="ValueMap"/>
		/// </param>
		public MappingBuilder (System.Type aType, params ValueMap[] aMaps)
		{
			ObjectType = aType;
			foreach (ValueMap vm in aMaps)
				Add (vm);
		}
	}
}
