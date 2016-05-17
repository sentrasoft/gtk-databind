// VirtualObject.cs - VirtualObject implementation for Gtk#Databindings
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
using System.Collections;

namespace System.Data.Bindings
{
	/// <summary>
	/// VirtualObject is object that can be declared during runtime, simply by adding properties
	///
	/// Its purpose is to act like DataRow, where data contained in this object can be inherited from
	/// either real or virtual object. Data can also be modified on fly and objects can be accessed 
	/// according to type
	///
	/// Accessing virtual property is as simple as obj["PropertyName"] and it is not limited to base
	/// types
	/// 
	/// Data can be copied from various types and copy method will simply resolve similiarities
	/// trough Reflection and make a smart copy of that object
	///
	/// Example of copy method (types are just simple abbreviation, and not defined as they should be):
	///   object1 [string Name, int Age, bool Male]
	///   object2 [int Age, bool Male, string SomethingElse]
	/// would mean Age and Male would be copied from one to another.
	/// </summary>
	public class VirtualObject : IVirtualObject
	{
		private VirtualProperty[] properties = null;
		
		private bool isUnique = false;
		
		/// <summary>
		/// Returns property by index
		/// </summary>
		/// <remarks>
		/// This is unsafe method to access Property, so it should be used with Locking the type
		/// </remarks>
		public VirtualProperty this [int a_Idx] {
			get {
				if ((a_Idx < 0) || (a_Idx >= properties.Length))
					throw new ExceptionWrongVirtualPropertyIndex (a_Idx, objectType.Name);
				return (properties[a_Idx]); 
			}
		}
		
		/// <summary>
		/// Returns Property by name 
		/// </summary>
		/// <remarks>
		/// This is safe method to access Property, so it can be used without Locking the type
		/// </remarks>
		public VirtualProperty this [string a_Name] {
			get { return (properties[GetPropertyIndex(a_Name)]); }
		}
		
		/// <summary>
		/// Returns Name of the ObjectType 
		/// </summary>
		public string ObjectTypeName {
			get {
				if (objectType != null)
					return (objectType.Name);
				return ("");
			}
			set {
				if (objectType != null)
					if (value == objectType.Name)
						return;
					else
						throw new ExceptionSettingAlreadyPresetVirtualObjectType (objectType.Name, value);
				VirtualObjectType obj = VirtualObjects.ObjectByName (value);
				if (obj == null) {
					if (isUnique == true)
						obj = new VirtualObjectType (isUnique, value);
					else
						obj = new VirtualObjectType (value);
					ObjectType = obj;
				}
				else
					ObjectType = obj;
				obj = null;
			}
		}
		
		private VirtualObjectType objectType = null;
		/// <summary>
		/// Returns ObjectType where all the description lies
		/// </summary>
		public VirtualObjectType ObjectType {
			get { return (objectType); }
			set {
				if ((objectType != null) && (objectType == value))
					throw new ExceptionVirtualObjectCantResetType();
				if (value == null)
					throw new ExceptionVirtualObjectTypeCantBeNull();
				objectType = value;
				CreateObjectProperties();
			}
		}
		
		private event EventOnPropertyChange onChange;
		/// <summary>
		/// Events to be delegated when any property in this object changeschanges
		/// </summary>
		public event EventOnPropertyChange OnChange {
			add { onChange += value; }
			remove { onChange -= value; }
		}

		/// <summary>
		/// Called whenever property in this virtual object changes
		/// </summary>
		/// <param name="aObject">
		/// Object related to change <see cref="IVirtualObject"/>
		/// </param>
		/// <param name="aProperty">
		/// Property that changed <see cref="VirtualProperty"/>
		/// </param>
		public virtual void Changed (IVirtualObject aObject, VirtualProperty aProperty)
		{
			if (onChange != null)
				onChange (aObject, aProperty);
		}
		
		/// <summary>
		/// IList enumeration needs
		/// </summary>
		/// <returns>
		/// Returns enumerator object <see cref="IEnumerator"/>
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return (properties.GetEnumerator());
		}
		
		/// <summary>
		/// IList enumeration needs
		/// </summary>
		/// <returns>
		/// Returns enumerator object <see cref="IEnumerator"/>
		/// </returns>
		public IEnumerator GetEnumerator()
		{
			return (properties.GetEnumerator());
		}

		/// <summary>
		/// Creates object properties for this object type
		/// </summary>
		protected void CreateObjectProperties()
		{
			properties = new VirtualProperty [ObjectType.Count];
			for (int i=0; i<ObjectType.Count; i++)
				properties[i] = new VirtualProperty (this, ObjectType[i].Name, ObjectType[i].PropertyType);
		}

		/// <summary>
		/// Gets the index for property with name as passed in argument 
		/// </summary>
		/// <param name="a_Name">
		/// Property name <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Index of property <see cref="System.Int32"/>
		/// </returns>
		/// <remarks>
		/// Throws exception if property is not found
		/// </remarks>
		public int GetPropertyIndex (string a_Name)
		{
			if (a_Name == "")
				throw new ExceptionWrongVirtualPropertyName (objectType.Name);
			for (int i=0; i<properties.Length; i++)
				if (a_Name == properties[i].Name)
					return (i);
			throw new ExceptionWrongVirtualPropertyName (a_Name, objectType.Name);
		}
		
		/// <summary>
		/// Adds property to ObjectType
		/// </summary>
		/// <param name="a_Name">
		/// Name of new property <see cref="System.String"/>
		/// </param>
		/// <param name="a_Type">
		/// Type of property value <see cref="System.Type"/>
		/// </param>
		/// <remarks>
		/// It results in Exception if this ObjectType is locked
		/// </remarks>
		public void AddProperty (string a_Name, System.Type a_Type)
		{
			if (ObjectType == null)
				throw new ExceptionCantAddPropertyToVirtualObjectType ();
			ObjectType.AddMember (a_Name, a_Type);
			VirtualProperty[] newprops = new VirtualProperty [ObjectType.Count];
			properties.CopyTo (newprops, 0);
			int i = newprops.Length - 1;
			newprops[i] = new VirtualProperty (this, ObjectType[i].Name, ObjectType[i].PropertyType);
			// Help GC to be more proactive
			for (int j=0; j < properties.Length; j++)
				properties[j] = null;
			properties = null;
			properties = newprops;
			newprops = null;
		}
		
		/// <summary>
		/// Inherits the properties from other type
		/// </summary>
		/// <param name="a_Object">
		/// Object as template <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true <see cref="System.Boolean"/>
		/// </returns>
		/// <remarks>
		/// Throws exception if not successful
		/// 
		/// also adds properties
		/// </remarks>
		public bool InheritStrict (object a_Object)
		{
			if (a_Object == null)
				throw new ExceptionVirtualObjectCantInheritNullObject();
			foreach (PropertyInfo prop in a_Object.GetType().GetProperties())
				if (prop != null)
					AddProperty (prop.Name, prop.PropertyType);
			return (true);
		}
		
		/// <summary>
		/// Inherits the properties from other type
		/// </summary>
		/// <param name="a_Type">
		/// Type as template <see cref="VirtualObjectType"/>
		/// </param>
		/// <returns>
		/// true <see cref="System.Boolean"/>
		/// </returns>
		/// <remarks>
		/// Throws Exception if any property already exists
		/// 
		/// also adds properties
		/// </remarks>
		public bool InheritStrict (VirtualObjectType a_Type)
		{
			if (a_Type == null)
				throw new ExceptionVirtualObjectCantInheritNullObject();
			for (int i=0; i<a_Type.Count; i++)
				AddProperty (a_Type[i].Name, a_Type[i].PropertyType);
			return (true);
		}
		
		/// <summary>
		/// Inherits the properties from other type
		/// </summary>
		/// <param name="a_Object">
		/// Object to inherit from <see cref="IVirtualObject"/>
		/// </param>
		/// <returns>
		/// true <see cref="System.Boolean"/>
		/// </returns>
		/// <remarks>
		/// Throws Exception if property already exists
		/// 
		/// also adds properties
		/// </remarks>
		public bool InheritStrict (IVirtualObject a_Object)
		{
			return (InheritStrict (a_Object.ObjectType));
		}
		
		/// <summary>
		/// Inherits the properties from other type
		/// </summary>
		/// <param name="a_Object">
		/// Object to inherit from <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true <see cref="System.Boolean"/>
		/// </returns>
		/// <remarks>
		/// If property already exists then it simply skips it
		/// 
		/// also adds properties
		/// </remarks>
		public bool InheritRelaxed (object a_Object)
		{
			if (a_Object == null)
				throw new ExceptionVirtualObjectCantInheritNullObject();
			foreach (PropertyInfo prop in a_Object.GetType().GetProperties())
				if (prop != null)
					if (this[prop.Name] == null)
						AddProperty (prop.Name, prop.PropertyType);
			return (true);
		}
		
		/// <summary>
		/// Inherits the properties from other type
		/// </summary>
		/// <param name="a_Type">
		/// Object type to inherit from <see cref="VirtualObjectType"/>
		/// </param>
		/// <returns>
		/// true <see cref="System.Boolean"/>
		/// </returns>
		/// <remarks>
		/// If property already exists, it avoids copying it
		/// 
		/// also adds properties
		/// </remarks>
		public bool InheritRelaxed (VirtualObjectType a_Type)
		{
			if (a_Type == null)
				return (false);
			for (int i=0; i<a_Type.Count; i++)
				if (this[a_Type[i].Name] == null)
					AddProperty (a_Type[i].Name, a_Type[i].PropertyType);
			return (true);
		}
		
		/// <summary>
		/// Inherits the properties from other type
		/// </summary>
		/// <param name="a_Object">
		/// Object to inherit from <see cref="IVirtualObject"/>
		/// </param>
		/// <returns>
		/// true <see cref="System.Boolean"/>
		/// </returns>
		/// <remarks>
		/// If property already exists, it avoids copying it
		/// 
		/// also adds properties
		/// </remarks>
		public bool InheritRelaxed (IVirtualObject a_Object)
		{
			return (InheritRelaxed (a_Object.ObjectType));
		}
		
		/// <summary>
		/// Removes property from ObjectType
		/// </summary>
		/// <param name="a_Name">
		/// Property name which to remove <see cref="System.String"/>
		/// </param>
		/// <remarks>
		/// It results in Exception if this ObjectType is locked
		/// </remarks>
		public void RemoveProperty (string a_Name)
		{
			if (ObjectType == null)
				throw new ExceptionCantRemovePropertyFromVirtualObjectType();
			ObjectType.RemoveMember (a_Name); 
		}
		
		/// <summary>
		/// Copies all data to object
		/// </summary>
		/// <param name="a_From">
		/// Source object <see cref="IVirtualObject"/>
		/// </param>
		/// <param name="a_To">
		/// Destination object <see cref="IVirtualObject"/>
		/// </param>
		public static void CopyDataTo (IVirtualObject a_From, IVirtualObject a_To)
		{
			if ((a_From == null) && (a_To == null))
				return;
			for (int i=0; i<a_From.ObjectType.Count; i++) {
				VirtualProperty vp = a_To[a_From[i].Name];
				if (vp != null)
					if (TypeValidator.IsCompatible(vp.PropertyType, a_From[i].PropertyType) == true)
						vp.Value = a_From[i].Value;
				vp = null;
			}
		}
		
		/// <summary>
		/// Copies all data to object
		/// </summary>
		/// <param name="a_From">
		/// Source object <see cref="IVirtualObject"/>
		/// </param>
		/// <param name="a_To">
		/// Destination object <see cref="System.Object"/>
		/// </param>
		public static void CopyDataTo (IVirtualObject a_From, object a_To)
		{
			if ((a_From == null) && (a_To == null))
				return;
			
			foreach (PropertyInfo prop in a_To.GetType().GetProperties()) {
				VirtualProperty vp = a_From[prop.Name];
				if ((prop != null) && (vp != null))
					if (prop.CanWrite == true)
						if (TypeValidator.IsCompatible(vp.PropertyType, prop.PropertyType) == true)
							prop.SetValue (a_To, vp.Value, null);
				vp = null;
			}
		}
		
		/// <summary>
		/// Copies all data to object
		/// </summary>
		/// <param name="a_From">
		/// Source object <see cref="System.Object"/>
		/// </param>
		/// <param name="a_To">
		/// Destination object <see cref="IVirtualObject"/>
		/// </param>
		public static void CopyDataFrom (object a_From, IVirtualObject a_To)
		{
			if ((a_From == null) && (a_To == null))
				return;
			
			foreach (PropertyInfo prop in a_From.GetType().GetProperties()) {
				VirtualProperty vp = a_To[prop.Name];
				if ((prop != null) && (vp != null))
					if (prop.CanRead == true)
						vp.Value = prop.GetValue (a_From, null);
				vp = null;
			}
		}
		
		/// <summary>
		/// Copies all data to object
		/// </summary>
		/// <param name="a_Object">
		/// Destination object <see cref="IVirtualObject"/>
		/// </param>
		public void CopyTo (IVirtualObject a_Object)
		{
			VirtualObject.CopyDataTo (this, a_Object);
		}
		
		/// <summary>
		/// Copies all data to object
		/// </summary>
		/// <param name="a_Object">
		/// Destination object <see cref="System.Object"/>
		/// </param>
		public void CopyTo (object a_Object)
		{
			VirtualObject.CopyDataTo (this, a_Object);
		}
		
		/// <summary>
		/// Copies all data from object
		/// </summary>
		/// <param name="a_Object">
		/// Source object <see cref="IVirtualObject"/>
		/// </param>
		public void CopyFrom (IVirtualObject a_Object)
		{
			VirtualObject.CopyDataTo (a_Object, this);
		}
		
		/// <summary>
		/// Copies all data from object
		/// </summary>
		/// <param name="a_Object">
		/// Source object <see cref="System.Object"/>
		/// </param>
		public void CopyFrom (object a_Object)
		{
			VirtualObject.CopyDataFrom (a_Object, this);
		}
		
		/// <summary>
		/// Creaes VirtualObject
		/// </summary>
		public VirtualObject()
		{
		}

		/// <summary>
		/// Creates VirtualObject
		/// </summary>
		/// <param name="a_IsUnique">
		/// Specifies if this object is unique or not <see cref="System.Boolean"/>
		/// </param>
		internal VirtualObject (bool a_IsUnique)
		{
			isUnique = a_IsUnique;
		}
		
		/// <summary>
		/// Creates VirtualObject
		/// </summary>
		/// <param name="a_Name">
		/// Name of object <see cref="System.String"/>
		/// </param>
		public VirtualObject (string a_Name)
		{
			ObjectTypeName = a_Name;
		}

		/// <summary>
		/// Creates VirtualObject
		/// </summary>
		/// <param name="a_Object">
		/// Template object <see cref="IVirtualObject"/>
		/// </param>
		public VirtualObject (IVirtualObject a_Object)
		{
			if (a_Object != null) {
				ObjectTypeName = a_Object.ObjectType.Name;
				InheritStrict (a_Object);
				CopyFrom (a_Object);		
			}
		}

		/// <summary>
		/// Creates VirtualObject
		/// </summary>
		/// <param name="a_Object">
		/// Template object <see cref="System.Object"/>
		/// </param>
		public VirtualObject (object a_Object)
		{
			if (a_Object != null) {
				ObjectTypeName = a_Object.GetType().Name;
				InheritStrict (a_Object);
				CopyFrom (a_Object);
			}
		}

		/// <summary>
		/// Creates VirtualObject
		/// </summary>
		/// <param name="a_Name">
		/// Name of object <see cref="System.String"/>
		/// </param>
		/// <param name="a_Object">
		/// Template object <see cref="System.Object"/>
		/// </param>
		public VirtualObject (string a_Name, object a_Object)
		{
			if (a_Object != null) {
				ObjectTypeName = a_Name;
				InheritStrict (a_Object);
				CopyFrom (a_Object);
			}
		}

		/// <summary>
		/// Creates VirtualObject
		/// </summary>
		/// <param name="a_Name">
		/// Name of object <see cref="System.String"/>
		/// </param>
		/// <param name="a_Object">
		/// Template object <see cref="IVirtualObject"/>
		/// </param>
		internal VirtualObject (string a_Name, IVirtualObject a_Object)
		{
			ObjectTypeName = a_Name;
			if (a_Object != null) {
				InheritStrict (a_Object);
				CopyFrom (a_Object);		
			}
		}

		/// <summary>
		/// Creates VirtualObject
		/// </summary>
		/// <param name="a_IsUnique">
		/// Specifies if this object is unique or not <see cref="System.Boolean"/>
		/// </param>
		/// <param name="a_Name">
		/// Name of object <see cref="System.String"/>
		/// </param>
		internal VirtualObject (bool a_IsUnique, string a_Name)
		{
			isUnique = a_IsUnique;
			ObjectTypeName = a_Name;
		}

		/// <summary>
		/// Destroys VirtualObject
		/// </summary>
		~VirtualObject()
		{
			if (ObjectType != null)
				ObjectType.Unref();
			objectType = null;
			for (int i=0; i<properties.Length; i++) {
				properties[i].Disconnect();
				properties[i] = null;
			}
			properties = null;
		}
	}
	
}
