// CachedProperty.cs - Field Attribute to assign additional information for Gtk#Databindings
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

// Enables datarow caching, but also disables same widget being used for
// more than one table, until locked mode is provided this should stay 
// disabled

using System;
using System.Data;
using System.Reflection;
using System.Collections;
using System.Data.Bindings.DebugInformation;

namespace System.Data.Bindings.Cached
{
	/// <summary>
	/// Class that caches property for easy and quick access 
	/// </summary>
	public class CachedProperty : IDisconnectable, IDisposable
	{
		private bool modified = false;
		private PropertyInfo propInfo = null;
		private VirtualProperty propVirtual = null;
		private DataColumn propColumn = null;
		private WeakReference objectReference = null;
		
		/// <summary>
		/// Specifies object type for which this caching has occured 
		/// </summary>
		public System.Type ObjectType {
			get {
				if (objectReference == null)
					return (null);
				if (objectReference.Target != null)
					return (objectReference.Target.GetType());
				return (null); 
			}
		}

		private string propertyName = "";
		/// <summary>
		/// Property name
		/// </summary>
		public string PropertyName {
			get { return (propertyName); }
		}

		/// <summary>
		/// Returns type of the property
		/// </summary>
		public System.Type PropertyType {
			get {
				if (objectReference == null)
					return (null);
				return (GetCachedPropertyType (objectReference.Target)); 
			}
		}

		/// <summary>
		/// Specifies validity of property 
		/// </summary>
		public EPropertyAccess IsAccessValid {
			get {
				if (IsCached == false)
					return (EPropertyAccess.Invalid);
				if (IsProperty == true)
					return (EPropertyAccess.Valid);
				if (IsDataRowField == true)
					return (EPropertyAccess.Valid);
				if (IsVirtualProperty == true)
					return (EPropertyAccess.Valid);
				return (EPropertyAccess.Invalid);
			}
		}
		
		/// <summary>
		/// Returns if property is cached or not
		/// </summary>
		public bool IsCached {
			get {
				if (modified == true) {
					if ((objectReference == null) || (objectReference.Target == null))
						return (false);
					Resolve();
				}
				modified = false;
				return ((IsProperty == true) || 
				        (IsDataRowField == true) || 
				        (IsVirtualProperty == true));
			}
		}

		/// <summary>
		/// Returns if reading is possible
		/// </summary>
		public bool CanRead {
			get {
				if (IsCached == false)
					return (false);

				if (IsProperty == true)
					return (propInfo.CanRead);
				if (IsDataRowField == true)
					return (propColumn.ReadOnly);
				if (IsVirtualProperty == true)
					return (true);
				return (false);
			}
		}

		/// <summary>
		/// Returns if writing is possible
		/// </summary>
		public bool CanWrite {
			get {
				if (IsCached == false)
					return (false);

				if (IsProperty == true)
					return (propInfo.CanWrite);
				if (IsDataRowField == true)
					return (! propColumn.ReadOnly);
				if (IsVirtualProperty == true)
					return (true);
				return (false);
			}
		}

		/// <summary>
		/// Returns secondary compare reference
		/// </summary>
		public object Reference {
			get { 
				if (objectReference == null)
					return (null);
				return (GetReferenceObject (objectReference.Target)); 
			}
		}
		
		/// <summary>
		/// Returns if cached property is property of norma object
		/// accessible trough reflection
		/// </summary>
		public bool IsProperty {
			get { return (propInfo != null); }
		}

		/// <summary>
		/// Returns if property is DataRow field
		/// </summary>
		public bool IsDataRowField {
			get { return (propColumn != null); }
		}

		/// <summary>
		/// Returns if property is DataRow field
		/// </summary>
		public bool IsVirtualProperty {
			get { return (propVirtual != null); }
		}

		/// <summary>
		/// Clears all cached values
		/// </summary>
		public void Clear()
		{
			propColumn = null;
			propVirtual = null;
			propInfo = null;
		}

		/// <summary>
		/// Set object type which will be cached
		/// </summary>
		/// <param name="aObject">
		/// Object whos type is reference for cache <see cref="System.Object"/>
		/// </param>
		public void SetObject (object aObject)
		{
			if (aObject == null) {
				Clear();
				return;
			}
			if ((objectReference != null) && (objectReference.Target != null))
				if (TypeValidator.IsCompatible(aObject.GetType(), ObjectType) == true) {
					if (objectReference.Target != aObject)
						objectReference.Target = aObject;
					if (Reference == GetReferenceObject(aObject))
						return;
				}

			modified = true;
			Clear();
			objectReference = new WeakReference (aObject);
//			objectReference.Target = aObject;
		}

		/// <summary>
		/// Checks if type of the object is compatible with cached type
		/// </summary>
		/// <param name="aObject">
		/// Object to compare against cached type <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if compatible, false if not <see cref="System.Boolean"/>
		/// </returns>
		public bool IsCompatible (object aObject)
		{
			if (aObject == null)
				return (false);
			if (IsCached == false)
				return (false);
			return ((TypeValidator.IsCompatible(aObject.GetType(), ObjectType) == true) &&
			        (Reference == GetReferenceObject(aObject)));
		}

		/// <summary>
		/// Returns secondary reference to object
		/// </summary>
		/// <returns>
		/// Secondary refrence to object <see cref="System.Object"/>
		/// </returns>
		private object GetReferenceObject (object aObject)
		{
			if ((IsCached == false) || (objectReference == null))
				return (null);
			if (TypeValidator.IsCompatible(objectReference.Target.GetType(), typeof(DataRow)) == true)
				return ((objectReference.Target as DataRow).Table);
			if (TypeValidator.IsCompatible(objectReference.Target.GetType(), typeof(DataRowView)) == true)
				return ((objectReference.Target as DataRowView).Row.Table);
			return (null);
		}
		
		/// <summary>
		/// Resolves cache for current values
		/// </summary>
		private void Resolve()
		{
			if ((objectReference == null) || (objectReference.Target == null))
				return;
			if (DatabaseProvider.IsValidType(objectReference.Target) == true) {
				propColumn = (DataColumn) DatabaseProvider.PropertyExists (objectReference.Target, PropertyName);
			}
			else if (VirtualObjectProvider.IsValidType(objectReference.Target) == true) {
				propVirtual = (objectReference.Target as IVirtualObject)[PropertyName];
			}
			else {
				// Object is just a normal class
				propInfo = ConnectionProvider.ResolveMappingPropertyByType (objectReference.Target.GetType(), PropertyName, false);
			}
		}
		
		/// <summary>
		/// Returns cached type of property
		/// </summary>
		/// <returns>
		/// Type of cached property <see cref="System.Type"/>
		/// </returns>
		private System.Type GetCachedPropertyType (object aObject)
		{
			if (aObject == null)
				return (null);
			if (IsCached == false)
				return (null);

			if (IsProperty == true)
				return (propInfo.GetType());
			if (IsDataRowField == true)
				return (propColumn.DataType);
			if (IsVirtualProperty == true)
				return ((aObject as IVirtualObject)[PropertyName].PropertyType);
			return (null);
		}

		/// <summary>
		/// Caches fast access to property for data transfer
		/// </summary>
		/// <param name="aObjectType">
		/// Object type for which property will be cached <see cref="System.Type"/>
		/// </param>
		/// <param name="aName">
		/// Property name which will be cached <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// true if successful, false if not <see cref="System.Boolean"/>
		/// </returns>
		/// <remarks>
		/// if object type is specified and differs from defined CachedType then
		/// it simply returns false, just the same as if CacheType is not specified
		/// </remarks>
		public bool SetProperty (string aName)
		{
			if (PropertyName == aName)
				return (IsAccessValid == EPropertyAccess.Valid);
			propertyName = aName;
			modified = true;
			Clear();
			if (IsCached == true)
				return (IsAccessValid == EPropertyAccess.Valid);
			return (false);
		}

		/// <summary>
		/// Sets value to cached property
		/// </summary>
		/// <param name="aValue">
		/// New value <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if successful, false if not <see cref="System.Boolean"/>
		/// </returns>
		public bool SetValue (object aValue)
		{
			if (objectReference == null)
				return (false);
			return (SetValue (objectReference.Target, PropertyName, aValue));
		}
		
		/// <summary>
		/// Sets value to cached property
		/// </summary>
		/// <param name="aObject">
		/// Object where property resides <see cref="System.Object"/>
		/// </param>
		/// <param name="aProperty">
		/// Property name <see cref="System.String"/>
		/// </param>
		/// <param name="aValue">
		/// New value <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if successful, false if not <see cref="System.Boolean"/>
		/// </returns>
		public bool SetValue (object aObject, string aProperty, object aValue)
		{
			bool startMethod = false;
			if (IsCached == true) {
				if (aProperty == PropertyName) 
					startMethod = true;
			}
			else {
				if (IsCompatible(aObject) == true)
					if (PropertyName.Trim() == aProperty.Trim())
						startMethod = IsCached;
			}
			// Start copying data
			if (startMethod == true) {
				if (IsProperty == true)
					return (ConnectionProvider.SetPropertyValue (aObject, propInfo, aValue));
				if (IsVirtualProperty == true) {
					propVirtual.Value = aValue;
					return (true);
				}
				if (IsDataRowField == true)
					return (DatabaseProvider.SetValue (aObject, propColumn, aValue));
				throw new ExceptionCachedPropertySetValueFailed (this);
			}
			else
				// If method arrived here then this is not a valid cache for that property
				// fallback creates temporary cache and sets new value trough it
				return (CachedProperty.UncachedSetValue (aObject, aProperty, aValue));
			return (false);
		}
		
		/// <summary>
		/// Gets hint for cached property
		/// </summary>
		/// <param name="aObject">
		/// Object which needs resolving <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Hint string <see cref="System.String"/>
		/// </returns>
		/// 
		public string GetHint (object aObject)
		{
			if ((aObject == null) || (PropertyName == ""))
				return (MappedProperty.NullValue);
			if (IsProperty == true)
				return (ConnectionProvider.GetPropertyHint (aObject, propInfo));
			if (IsVirtualProperty == true)
				return ("");
			if (IsDataRowField == true)
				return (DatabaseProvider.GetPropertyHint (aObject, propColumn));
			return ("");
		}

		public PropertyDescriptionAttribute GetDescription (object aObject)
		{
			if (IsProperty == true)
				return (ConnectionProvider.GetPropertyDescription (aObject, propInfo));
			return (null);
		}
		
		/// <summary>
		/// Gets title for cached property
		/// </summary>
		/// <param name="aObject">
		/// Object which needs resolving <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Title string <see cref="System.String"/>
		/// </returns>
		public string GetTitle (object aObject)
		{
			if ((aObject == null) || (PropertyName == ""))
				return (MappedProperty.NullValue);
			if (IsProperty == true)
				return (ConnectionProvider.GetPropertyTitle (aObject, propInfo));
			if (IsVirtualProperty == true)
				return ("");
			if (IsDataRowField == true)
				return (DatabaseProvider.GetPropertyTitle (aObject, propColumn));
			return ("");
		}

		/// <summary>
		/// Gets title for cached property
		/// </summary>
		/// <param name="aType">
		/// Class type which needs resolving <see cref="System.Type"/>
		/// </param>
		/// <param name="aPropertyName">
		/// Property name <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Title string <see cref="System.String"/>
		/// </returns>
		public static string GetPropertyTitle (System.Type aType, string aPropertyName)
		{
			if ((aType == null) || (aPropertyName == ""))
				return (MappedProperty.NullValue);
			return (ConnectionProvider.GetPropertyTitle (aType, aPropertyName));
		}

		/// <summary>
		/// Gets title for cached property
		/// </summary>
		/// <param name="aObject">
		/// Object which needs resolving <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Title string <see cref="System.String"/>
		/// </returns>
		public string GetPropertyHandler (object aObject)
		{
			if ((aObject == null) || (PropertyName == ""))
				return (MappedProperty.NullValue);
			if (IsProperty == true)
				return (ConnectionProvider.GetPropertyHandler (aObject, propInfo));
			if (IsVirtualProperty == true)
				return ("");
			if (IsDataRowField == true)
				return (DatabaseProvider.GetPropertyHandler (aObject, propColumn));
			return ("");
		}

		/// <summary>
		/// Gets title for cached property
		/// </summary>
		/// <param name="aObject">
		/// Object which needs resolving <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Title string <see cref="System.String"/>
		/// </returns>
		public PropertyHandlerType GetPropertyHandlerType (object aObject)
		{
			if ((aObject == null) || (PropertyName == ""))
				return (PropertyHandlerType.Default);
			if (IsProperty == true)
				return (ConnectionProvider.GetPropertyHandlerType (aObject, propInfo));
			if (IsVirtualProperty == true)
				return (PropertyHandlerType.Default);
			if (IsDataRowField == true)
				return (DatabaseProvider.GetPropertyHandlerType (aObject, propColumn));
			return (PropertyHandlerType.Default);
		}

		/// <summary>
		/// Gets field name for cached property
		/// </summary>
		/// <param name="aObject">
		/// Object which needs resolving <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Title string <see cref="System.String"/>
		/// </returns>
		public string GetFieldName (object aObject)
		{
			if ((aObject == null) || (PropertyName == ""))
				return (MappedProperty.NullValue);
			if (IsProperty == true)
				return (ConnectionProvider.GetPropertyFieldName (aObject, propInfo));
			if (IsVirtualProperty == true)
				return ("");
			if (IsDataRowField == true)
				return (DatabaseProvider.GetPropertyFieldName (aObject, propColumn));
			return ("");
		}

		/// <summary>
		/// Sets value to cached property
		/// </summary>
		/// <param name="aValue">
		/// New value <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if successful, false if not <see cref="System.Boolean"/>
		/// </returns>
		/// <remarks>
		/// calls GetValue with null as default value
		/// </remarks>
		public bool GetValue (out object aValue)
		{
			aValue = null;
			if (objectReference == null)
				return (false);
			return (GetValue (objectReference.Target, PropertyName, out aValue, null));
		}
		
		/// <summary>
		/// Sets value to cached property
		/// </summary>
		/// <param name="aValue">
		/// New value <see cref="System.Object"/>
		/// </param>
		/// <param name="aDefaultValue">
		/// Default value which should be returned if method was unsuccessful <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if successful, false if not <see cref="System.Boolean"/>
		/// </returns>
		/// <remarks>
		/// calls GetValue with null as default value
		/// </remarks>
		public bool GetValue (out object aValue, object aDefaultValue)
		{
			aValue = aDefaultValue;
			if (objectReference == null)
				return (false); 
			return (GetValue (objectReference.Target, PropertyName, out aValue, aDefaultValue));
		}
		
		/// <summary>
		/// Sets value to cached property
		/// </summary>
		/// <param name="aObject">
		/// Object where property resides <see cref="System.Object"/>
		/// </param>
		/// <param name="aProperty">
		/// Property name <see cref="System.String"/>
		/// </param>
		/// <param name="aValue">
		/// New value <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if successful, false if not <see cref="System.Boolean"/>
		/// </returns>
		/// <remarks>
		/// calls GetValue with null as default value
		/// </remarks>
		public bool GetValue (object aObject, string aProperty, out object aValue)
		{
			return (GetValue (aObject, aProperty, out aValue, null));
		}
		
		/// <summary>
		/// Sets value to cached property
		/// </summary>
		/// <param name="aObject">
		/// Object where property resides <see cref="System.Object"/>
		/// </param>
		/// <param name="aProperty">
		/// Property name <see cref="System.String"/>
		/// </param>
		/// <param name="aValue">
		/// New value <see cref="System.Object"/>
		/// </param>
		/// <param name="aDefaultValue">
		/// Default value which should be returned if method was unsuccessful <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if successful, false if not <see cref="System.Boolean"/>
		/// </returns>
		public bool GetValue (object aObject, string aProperty, out object aValue, object aDefaultValue)
		{
			bool startMethod = false;
			aValue = aDefaultValue;
			if (IsCached == true) {
				if (aProperty == PropertyName) 
					startMethod = true;
			}
			else {
				if (IsCompatible(aObject) == true)
					if (aProperty == PropertyName) 
						startMethod = IsCached;
			}
			// Start copying data
			if (startMethod == true) {
				if (IsProperty == true) {
					aValue = ConnectionProvider.GetPropertyValue (aObject, propInfo);
					return (true);
				}
				if (IsVirtualProperty == true) {
					aValue = propVirtual.Value;
					return (true);
				}
				if (IsDataRowField == true)
					return (DatabaseProvider.GetValue (aObject, propColumn, out aValue, aDefaultValue));
				throw new ExceptionCachedPropertyGetValueFailed (this);
			}
			else
				// If method arrived here then this is not a valid cache for that property
				// fallback creates temporary cache and gets value trough it
				return (CachedProperty.UncachedGetValue (aObject, aProperty, out aValue, aDefaultValue));
			return (false);
		}

		/// <summary>
		/// Creates temporary cache and sets value to the property
		/// </summary>
		/// <param name="aObject">
		/// Object which needs value to be set <see cref="System.Object"/>
		/// </param>
		/// <param name="aProperty">
		/// Property name <see cref="System.String"/>
		/// </param>
		/// <param name="aValue">
		/// New property value <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if successful, false if not <see cref="System.Boolean"/>
		/// </returns>
		public static bool UncachedSetValue (object aObject, string aProperty, object aValue)
		{
			bool res = false;
			CachedProperty tempCache = new CachedProperty (aObject, aProperty);
			if (tempCache.IsAccessValid == EPropertyAccess.Valid)
				res = tempCache.SetValue (aObject, aProperty, aValue);
			tempCache.Disconnect();
			tempCache = null;
			return (res);
		}
		
		/// <summary>
		/// Creates temporary cache and gets value from the property
		/// </summary>
		/// <param name="aObject">
		/// Object which needs value to be set <see cref="System.Object"/>
		/// </param>
		/// <param name="aProperty">
		/// Property name <see cref="System.String"/>
		/// </param>
		/// <param name="aValue">
		/// Property value <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if successful, false if not <see cref="System.Boolean"/>
		/// </returns>
		/// <remarks>
		/// calls CachedProperty.UncachedGetValue with null as default value
		/// </remarks>
		public static bool UncachedGetValue (object aObject, string aProperty, out object aValue)
		{
			return (CachedProperty.UncachedGetValue (aObject, aProperty, out aValue, null));
		}
		
		/// <summary>
		/// Creates temporary cache and gets value from the property
		/// </summary>
		/// <param name="aObject">
		/// Object which needs value to be set <see cref="System.Object"/>
		/// </param>
		/// <param name="aProperty">
		/// Property name <see cref="System.String"/>
		/// </param>
		/// <param name="aValue">
		/// Property value <see cref="System.Object"/>
		/// </param>
		/// <param name="aDefaultValue">
		/// Default value which should be returned if method was unsuccessful <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if successful, false if not <see cref="System.Boolean"/>
		/// </returns>
		public static bool UncachedGetValue (object aObject, string aProperty, out object aValue, object aDefaultValue)
		{
			aValue = aDefaultValue;
			bool res = false;
			CachedProperty tempCache = new CachedProperty (aObject, aProperty);
			if (tempCache.IsAccessValid == EPropertyAccess.Valid)
				res = tempCache.GetValue (aObject, aProperty, out aValue, aDefaultValue);
			tempCache.Disconnect();
			tempCache = null;
			return (res);
		}
		
		/// <summary>
		/// Disconnects object
		/// </summary>
		public void Disconnect()
		{
			Clear();
			propertyName = "";
//			objectReference.Target = null;
			objectReference = null;
		}
		
		/// <summary>
		/// Creates cached property
		/// </summary>
		private CachedProperty()
		{
			throw new ExceptionCachedPropertyCreateFailed();
		}
		
		/// <summary>
		/// Creates cached property
		/// </summary>
		public CachedProperty (object aObject, string aProperty)
		{
			SetObject (aObject);
			SetProperty (aProperty);
		}

		/// <summary>
		/// Disposes class
		/// </summary>
		public void Dispose()
		{
			//Finalize();
			System.GC.SuppressFinalize (this);
		}
		
		/// <summary>
		/// Clears and destroys CachedProperty
		/// </summary>
		~CachedProperty()
		{
			Disconnect();
		}
	}
}
