// MappedProperty.cs - MappedProperty implementation for Gtk#Databindings
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

#define NEWCACHE
//#define OLDCACHE

using System;
using System.Reflection;
#if NEWCACHE
using System.Collections;
#endif
using System.Data.Bindings.Cached;
using System.Data.Bindings.DebugInformation;

namespace System.Data.Bindings
{
	/// <summary>
	/// Connection between property and mapping
	/// </summary>
	public class MappedProperty
	{
#if NEWCACHE
		private bool resetResolve = false;
		private CachedProperty dataCache = null;
		private CachedProperty controlCache = null;
#endif
		private bool isSubItem = false;
		private MappedProperty masterItem = null;
#if OLDCACHE
		private bool invalid = true;
		private bool cached = false;
#endif

		internal const string NullValue = "!#not_defined";
		
		/// <summary>
		/// Returns if this MappedProperty is Sub-Item of another MappedProperty
		/// </summary>
		/// <remarks>
		/// Only valid for column properties, to allow more complex columns than one CellRenderer per
		/// column.
		/// </remarks>
		[ToDo ("Add named CellRenderer spec to allow CellRenderer to be activated on demand")]
		public bool IsSubItem {
			get { return ((masterItem != null) && (isSubItem == true)); }
		}
		
		/// <summary>
		/// MasterItem returns MasterProperty of this sub-Item.
		/// </summary>
		/// <remarks>
		/// Only valid for column properties, to allow more complex columns than one CellRenderer per
		/// column.
		/// </remarks>
		public MappedProperty MasterItem {
			get {
				if (IsSubItem == false)
					return (null);
				return (masterItem);
			}
		}
		
		private WeakReference adaptor = null;
		/// <summary>
		/// WeakReference to adaptor, and read-only IAdaptor access
		/// </summary>
		public IAdaptor Adaptor {
			get {
				if (adaptor == null)
					return (null);
				return ((IAdaptor) adaptor.Target);
			}
		}
		
		/// <summary>
		/// Returns System.Type of the mapped property
		/// </summary>
/*		private System.Type valuePropertyType;
		public System.Type ValuePropertyType {
			get { return (valuePropertyType); }
		}*/
		
		private string name = "";
		/// <summary>
		/// Mapped Property name, uses caching and if needed resolves again
		/// </summary>
		public string Name {
			get { return (name); }
			set { 
#if NEWCACHE
				if (name == value)
					return;
				name = value;
				resetResolve = true;
#endif
#if OLDCACHE
				if ((name == value) && (cached == true))
					return;
				invalid = true;
				name = value;
				cached = false;
				Resolve();
#endif
			}
		}
		
#if NEWCACHE
		private void ClearDataMappingCache()
		{
			dataCache.Clear();
		}
#endif
	
		private string hint = NullValue;
		/// <value>
		/// Specifies hint for mapped property
		/// </value>
		public string Hint {
			get { 
				if (hint == NullValue)
					hint = GetHint();
				return (hint); 
			}
		}
		
		private string title = NullValue;
		/// <value>
		/// Specifies hint for mapped property
		/// </value>
		public string Title {
			get { 
				if (title == NullValue)
					title = GetTitle();
				return (title); 
			}
		}
		
		private string mappingTarget = "";
		/// <summary>
		/// Mapping target defines which property is to be controlled with this
		/// mapping
		/// </summary>
		public string MappingTarget {
			get { return (mappingTarget); }
			set { mappingTarget = value; }
		}
		
		private string columnName = "";
		/// <summary>
		/// Column name if this is column
		/// </summary>
		public string ColumnName {
			get {
				if (IsColumnMapping == false)
					return ("");
				else
					if (columnName != "")
						return (columnName);
					else
						return (Name);
			}
			set { columnName = value; }
		}
		
		private bool isColumnMapping = false;
		/// <summary>
		/// Returns if this mapping is defined as column
		/// </summary>
		/// <remarks>
		/// Column mappings can't have result set and can't be defined in Boundary Adaptor
		/// </remarks>
		public bool IsColumnMapping {
			get { return (isColumnMapping); }
			set {
				if (value == true)
					if (Adaptor.IsBoundaryAdaptor == true)
						throw new ExceptionColumnInBoundaryMapping();
				isColumnMapping = value;
				if (IsColumnMapping == true)
					AllowedToWrite = false;
			}
		}
		
		private ChildMappedProperties submappings = null;
		/// <summary>
		/// Column submapings
		/// </summary>
		public ChildMappedProperties Submappings {
			get { return (submappings); }
		}
		
		/// <summary>
		/// Returns true if this mapped property has submappings
		/// </summary>
		public bool HasSubmappings {
			get {
				if (submappings == null)
					return (false);
				return (submappings.Count > 0);
			}
		}
		
		/// <summary>
		/// Returns if this is targeted value
		/// </summary>
		public bool IsTargeted {
			get { return ((IsDefault == false) && (this.MappingTarget != "")); }
		}
		
		/// <summary>
		/// Returns if this is targeted value
		/// </summary>
		public bool IsSecondaryTargeted {
			get { return ((IsDefault == false) && (IsColumnMapping == false) && (IsGlobal == false)); }
		}
		
		private string boundingClassName = "";
		/// <summary>
		/// Sets global class boundary mapping
		/// </summary>
		public string BoundingClassName {
			get { return (boundingClassName); }
			set {
				if (value != "")
					if (Adaptor.IsBoundaryAdaptor == false)
						throw new ExceptionBoundaryMappingSetToNonBoundary();
				boundingClassName = value;
			}
		}
		
		/// <summary>
		/// Returns data directions as string
		/// </summary>
		private string ReadWriteString {
			get { 
				if (IsDefault == true)
					return ("<>");
				if (IsGlobal == true)
					return (">>");
				if (IsColumnMapping == true)
					if (allowedToWrite == true)
						return ("<>");
					else
						return ("<<");
				if ((AllowedToRead == true) && (AllowedToWrite == true))
					return ("<>");
				if (AllowedToRead == true)
					return (">>");
				if (AllowedToWrite == true)
					return ("<<");
				return ("<<");
			}
		}
		
		/// <summary>
		/// Read Write flags for this mapped property
		/// </summary>
		public EReadWrite RWFlags {
			get {
				if (allowedToRead == true)
					if (allowedToWrite == true)
						return (EReadWrite.ReadWrite);
					else
						return (EReadWrite.ReadOnly);
				else
					if (allowedToWrite == true)
						return (EReadWrite.WriteOnly);
					else
						return (EReadWrite.None);
			}
			set {
				allowedToRead = ((value == EReadWrite.ReadOnly) || (value == EReadWrite.ReadWrite));
				allowedToWrite = ((value == EReadWrite.WriteOnly) || (value == EReadWrite.ReadWrite));
			}
		}
		
		private EReadWrite originalRWFlags = EReadWrite.ReadOnly;
		/// <summary>
		/// Returns original RWFlags, because in some cases they are needed, for example
		/// treeview can contain multiple types and has to resolve this differently
		/// </summary>
		public EReadWrite OriginalRWFlags {
			get { return (originalRWFlags); }
		}
		
		private bool allowedToRead = true;
		/// <summary>
		/// Returns if this mapping can read from DataSource
		/// </summary>
		public bool AllowedToRead {
			get { return (allowedToRead && Valid); }
			set { allowedToRead = value; }
		}
		
		private bool allowedToWrite = true;
		/// <summary>
		/// Returns if this mapping can write to DataSource
		/// </summary>
		public bool AllowedToWrite {
			get {
				if (IsGlobal == true)
					return (false);
				return ((allowedToWrite == true) && (Valid == true));
			}
			set { 
				allowedToWrite = value;
				// Throw exception only when AllowedToRead was intentionally set but not allowed
				if ((value == true) && (IsGlobal == true))
					throw new ExceptionPropertyWriteNotPossible();
			}
		}
		
		/// <summary>
		/// Checks if this is Global Boundary mapping
		/// </summary>
		public bool IsGlobal {
			get {
				if (Adaptor.IsBoundaryAdaptor == false)
					return (false);
				return (boundingClassName != "");
			}
		}
		
		/// <summary>
		/// Returns information about this mapped property is boundary or not
		/// </summary>
		public bool IsBoundaryMapping {
			get { return ((Adaptor.IsBoundaryAdaptor == true) && (mappingTarget != "")); }
		}
		
		/// <summary>
		/// Checks if this is default mapped property
		/// </summary>
		/// <remarks>
		/// Only non-Boundary adaptors can have default properties. Assigning default property
		/// to BoundaryAdaptor will result in Exception
		/// </remarks>
		public bool IsDefault {
			get { return ((Adaptor.IsBoundaryAdaptor == false) && (MappingTarget == "") && (IsColumnMapping == false)); }
		}
		
		private bool isNormalProperty = false;

#if OLDCACHE
		private PropertyInfo cachedInfo;
		/// <summary>
		/// Resolves current mapping property the fastest way possible
		/// </summary>
		private PropertyInfo Info {
			get { 
				if (Resolve() == true)
					return (cachedInfo);
				return (null);
			}
		}
#endif
		
		/// <summary>
		/// Resolves mapped type
		/// </summary>
		/// <returns>
		/// Type  <see cref="System.Type"/>
		/// </returns>
		public System.Type GetMappedType()
		{
			if (Resolve() == true)
#if NEWCACHE
				return (dataCache.PropertyType);
#endif
#if OLDCACHE
				return (cachedInfo.GetType());
#endif
			return (null);
		}
		
		/// <summary>
		// Returns validity of this Mapping
		/// </summary>
		public bool Valid {
			get { 
#if NEWCACHE
//				if ((this.Name == "") || (Adaptor.InheritedTarget == null))
				if ((this.Name == "") || (Adaptor.FinalTarget == null))
					return (false);

				if (Resolve() == true)
					return (dataCache.IsAccessValid == EPropertyAccess.Valid);
				return (false);
#endif
#if OLDCACHE
				if (cached == false)
					if (Adaptor.InheritedTarget == true) {
						if (Resolve() == false) 
							return (false);
					}
					else
						return (false);
				
				return (! (invalid));
#endif
			}
		}
		
		/// <summary>
		/// Get and Set the Value of the Mapping
		/// </summary>
		public object Value {
			get {
				if (AllowedToRead == true)
					return (GetValue());
				return (null);
			}
			set {
				// Only non-boundary Adaptors can set value back to the object
				if (Adaptor.IsBoundaryAdaptor == false) {
					if (AllowedToWrite == true)
						SetValue (value);
					else
						throw new ExceptionPropertyWriteNotPossible();
				}
			}
		}
		
		/// <summary>
		/// Sets read write from string
		/// </summary>
		public void SetReadWrite (string aStr)
		{
			switch (aStr) {
				case ">>" :
					AllowedToRead = true;
					AllowedToWrite = false;
					break;
				case "<<" :
					AllowedToRead = false;
					AllowedToWrite = true;
					break;
				case "<>" :
					AllowedToRead = true;
					AllowedToWrite = true;
					break;
				default :
					throw new ExceptionAssigningWrongReadWriteMapping (aStr);
			}
		}
		
		/// <summary>
		/// Resets PropertyInfo
		/// </summary>
		public void Reset()
		{
#if NEWCACHE
			resetResolve = true;
//			dataCache.SetObject (null);
#endif
#if OLDCACHE
			cached = false;
			cachedInfo = null;
#endif
		}
		
		/// <summary>
		/// Resoves PropertyInfo from connected object
		/// </summary>
		public bool Resolve()
		{
#if NEWCACHE
			if (dataCache == null)
				dataCache = new CachedProperty (Adaptor.FinalTarget, Name);
			if (resetResolve == true) {
				dataCache.SetObject (Adaptor.FinalTarget);
				dataCache.SetProperty (Name.Trim());
			}
/*			if (dataCache.IsAccessValid == EPropertyAccess.Invalid)
				if (Adaptor.FinalTarget != null)	{
					dataCache.SetObject (Adaptor.FinalTarget);
					dataCache.SetProperty (Name.Trim());
				}*/
			return (dataCache.IsAccessValid == EPropertyAccess.Valid);
#endif			
#if OLDCACHE
			if (cached == true)
				return (true);
				
			if (name == "")
				return (false);
			
			cached = false;
			cachedInfo = null;
			if (Adaptor.FinalTarget != null)
				cachedInfo = ConnectionProvider.ResolveMappingProperty (Adaptor.FinalTarget, Name, false);
				
			cached = (cachedInfo != null);
			invalid = (cachedInfo == null);
			return (cachedInfo != null);
#endif
		}
		
		/// <summary>
		/// Connects on adaptors OnTargetChange
		/// </summary>
		public void Connect()
		{
			if (Adaptor != null)
				Adaptor.TargetChanged += TargetChanged;
		}
		
		/// <summary>
		/// Gets value from referenced Target
		/// </summary>
		public object GetValue ()
		{
#if NEWCACHE
			object val = null;
			if (Resolve() == true)
//				dataCache.GetValue (Adaptor.FinalTarget, Name, out val);
				dataCache.GetValue (out val);
			return (val);
#endif
#if OLDCACHE
//			if (cached == false)
//				Name = name;
			if ((cached == false) && (Adaptor.InheritedTarget == false))
				return (null);
			if (cached == false)
				if (Adaptor.InheritedTarget == true) {
					if (Resolve() == false)
						return (null);
				}
				else
					return (null);
			
			if (Info.CanRead == true)
				return (Info.GetValue (Adaptor.FinalTarget, null));
			return (null);
#endif
		}
		
		/// <summary>
		/// Gets hint for referenced Target
		/// </summary>
		public string GetHint ()
		{
			string val = "";
			if (Resolve() == true)
				val = dataCache.GetHint (Adaptor.FinalTarget);
			return (val);
		}
		
		/// <summary>
		/// Gets title for referenced Target
		/// </summary>
		public string GetTitle ()
		{
			string val = "";
			if (Resolve() == true)
				val = dataCache.GetTitle (Adaptor.FinalTarget);
			return (val);
		}
		
		/// <summary>
		/// Gets description for referenced Target
		/// </summary>
		public PropertyDescriptionAttribute GetDescription ()
		{
			PropertyDescriptionAttribute val = null;
			if (Resolve() == true)
				val = dataCache.GetDescription (Adaptor.FinalTarget);
			return (val);
		}
		
		/// <summary>
		/// Gets title for referenced Target
		/// </summary>
		public string ResolveTitle (System.Type aType)
		{
			return (CachedProperty.GetPropertyTitle (aType, Name));
		}
		
		/// <summary>
		/// gets value from referenced Target
		/// </summary>
		/// <param name="aValue">
		/// Value to set <see cref="System.Object"/>
		/// </param>
		public void SetValue (object aValue)
		{
#if NEWCACHE
			if (Adaptor.FinalTarget is IObserveable)
				(Adaptor.FinalTarget as IObserveable).ResetChangeCallCheckup();
				
			if (Resolve() == true)
				if (dataCache.SetValue (Adaptor.FinalTarget, Name, aValue) == true)
					if ((Adaptor.FinalTarget is IObserveable) == false)
						DataSourceController.CallChangedFor (Adaptor.FinalTarget);
					else
						if ((Adaptor.FinalTarget as IObserveable).HasCalledForChange == false)
							(Adaptor.FinalTarget as IObserveable).HasChanged = true;
#endif
#if OLDCACHE 
			if (cached == false)
				Name = name;
			if ((cached == false) && (Adaptor.InheritedTarget == false))
				return;
			if (cached == false)
				if (Adaptor.InheritedTarget == true) {
					if (Resolve() == false)
						return;
				}
				else
					return;
			
			if (Adaptor.FinalTarget is IObserveable)
				(Adaptor.FinalTarget as IObserveable).ResetChangeCallCheckup();
				
			if (ConnectionProvider.SetPropertyValue (Adaptor.FinalTarget, Info, aValue) == true)
				if ((Adaptor.FinalTarget is IObserveable) == false)
					DataSourceController.CallChangedFor (Adaptor.FinalTarget);
				else
					if ((Adaptor.FinalTarget as IObserveable).HasCalledForChange == false)
						(Adaptor.FinalTarget as IObserveable).HasChanged = true;
#endif
		}
		
		/// <summary>
		/// Activated whenever Target in Adaptor changes
		/// </summary>
		protected void TargetChanged (IAdaptor aAdaptor)
		{
			hint = NullValue;
			title = NullValue;
#if NEWCACHE
			Reset();
#endif
#if OLDCACHE 
			cached = false;
			cachedInfo = null;
#endif
		}
		
		/// <summary>
		/// Assigns Value in given direction between Control and DataSource Target
		/// </summary>
		/// <param name="aDirection">
		/// Direction of data <see cref="EDataDirection"/>
		/// </param>
		/// <param name="aObject">
		/// Object which contains data <see cref="System.Object"/>
		/// </param>
		/// <param name="aControl">
		/// Control used to edit <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Returns true if successful <see cref="System.Boolean"/>
		/// </returns>
		public bool AssignValueToObject (EDataDirection aDirection, object aObject, object aControl)
		{
			bool res = false;
			// Check if 
			if (IsGlobal == true) {
				if (aDirection == EDataDirection.FromControlToDataSource)
					throw new ExceptionGlobalMappingAssignedFromControlToTarget();
					
#if NEWCACHE
				if (dataCache == null)
					dataCache = new CachedProperty (Adaptor.FinalTarget, Name);
				if (controlCache == null)
					controlCache = new CachedProperty (aControl, MappingTarget);
				if (controlCache.IsCached == true) {
					object val;
					if (dataCache.IsCached == true)
						if (dataCache.GetValue (out val) == true)
							if (controlCache.SetValue (val) == true) {
								val = null;
								return (true);
							}
				}
				return (false);
#endif
#if OLDCACHE 
				object FromObject = aObject;
				object ToObject = aControl;
				string FromProperty = Name;
				string ToProperty = MappingTarget;

				// assign, direction is already correct
				res = ConnectionProvider.CopyPropertyValue (FromObject, FromProperty, ToObject, ToProperty);
				FromObject = null;
				ToObject = null;
				return (res);
#endif
			}
			else {
#if NEWCACHE
				CachedProperty FromObject;
				CachedProperty ToObject;
				bool canbedone;
				if (aDirection == EDataDirection.FromControlToDataSource) {
					FromObject = controlCache;
					ToObject = dataCache;
					canbedone = AllowedToWrite;
					if (ToObject is IObserveable)
						(ToObject as IObserveable).ResetChangeCallCheckup();
				}
				else {
					FromObject = dataCache;
					ToObject = controlCache;
					canbedone = AllowedToRead;
				}
				
				if (controlCache == null)
					controlCache = new CachedProperty (aControl, MappingTarget);

				object val = null;
				// assign in set direction
				if ((canbedone == true) && (FromObject != null) && (ToObject != null)) {
					if (FromObject.GetValue (out val) == true) {
						if (ToObject.SetValue (val) == true)
							if (aDirection == EDataDirection.FromControlToDataSource)
								if ((ToObject is IObserveable) == false)
									DataSourceController.CallChangedFor (ToObject);
								else
									if ((ToObject as IObserveable).HasCalledForChange == false)
										(ToObject as IObserveable).HasChanged = true;
						res = true;
					}
				}
				FromObject = null;
				ToObject = null;
				return (res);
#endif
#if OLDCACHE 
				object FromObject;
				object ToObject;
				string FromProperty;
				string ToProperty;
				bool canbedone;
				// swap direction if needed 
				if (aDirection == EDataDirection.FromControlToDataSource) {
					FromObject = aControl;
					ToObject = aObject;
					FromProperty = MappingTarget;
					ToProperty = Name;
					canbedone = AllowedToWrite;
					if (ToObject is IObserveable)
						(ToObject as IObserveable).ResetChangeCallCheckup();
				}
				else {
					FromObject = aObject;
					ToObject = aControl;
					FromProperty = Name;
					ToProperty = MappingTarget;
					canbedone = AllowedToRead;
				}
				
				// assign in set direction
				if (canbedone == true) {
					if (ConnectionProvider.CopyPropertyValue(FromObject, FromProperty, ToObject, ToProperty) == true) {
						if (aDirection == EDataDirection.FromControlToDataSource)
							if ((ToObject is IObserveable) == false)
								DataSourceController.CallChangedFor (ToObject);
							else
								if ((ToObject as IObserveable).HasCalledForChange == false)
									(ToObject as IObserveable).HasChanged = true;
						res = true;
					}
//					else
//						Debug.Warning ("MappedProperty.AssignValueToObject", "CopyPropertyValue not successful");
				}
				FromObject = null;
				ToObject = null;
				return (res);
#endif
			}
		}
		
		/// <summary>
		/// Assigns Value in given direction between MappingTarget in Control and DataSource Target
		/// </summary>
		/// <param name="aDirection">
		/// Direction of data <see cref="EDataDirection"/>
		/// </param>
		/// <param name="aObject">
		/// Object which contains data <see cref="System.Object"/>
		/// </param>
		/// <param name="aControl">
		/// Control used to edit <see cref="System.Object"/>
		/// </param>
		/// <param name="aMappingTarget">
		/// Mapping which should be used to transfer data <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Returns true if successful <see cref="System.Boolean"/>
		/// </returns>
		public bool AssignValueToObject (EDataDirection aDirection, object aObject, object aControl, string aMappingTarget)
		{
			if (aMappingTarget == "")
				return (false);
			string oldmapping = MappingTarget;
			// Fake mapping target
			mappingTarget = aMappingTarget;
			// Call assign as usual
			bool res = AssignValueToObject (aDirection, aObject, aControl);
			// Restore mapping target
			mappingTarget = oldmapping;
			return (res);
		}
		
		/// <summary>
		/// Overrides default ToString and provides actual mapping
		/// </summary>
		/// <returns>
		/// String interpretation of object <see cref="System.String"/>
		/// </returns>
		public override string ToString()
		{
			if (IsDefault == true)
				return (Name);
				
			if (IsGlobal == true)
				return ("(" + BoundingClassName + ")" + Name + ReadWriteString + MappingTarget);
			if (IsColumnMapping == true)
				return (Name + "[" + ColumnName + "]" + ReadWriteString + MappingTarget);
			else
				return (Name + ReadWriteString + MappingTarget);
		}
		
		/// <summary>
		/// Disconnects on adaptors OnTargetChange
		/// </summary>
		public void Disconnect()
		{
			if (dataCache != null)
				dataCache.Disconnect();
			dataCache = null;
			if (controlCache != null)
				controlCache.Disconnect();
			controlCache = null;
			if (IsSubItem == true)
				MasterItem.Submappings.RemoveMapping (this);
			else
				if (submappings != null)
					submappings.Disconnect();
			submappings = null;
			masterItem = null;
			if (Adaptor != null)
				Adaptor.TargetChanged -= TargetChanged;
			adaptor.Target = null;
			adaptor = null;
		}

		/// <summary>
		/// Creates Mapped property
		/// </summary>
		protected MappedProperty()
		{
		}

		/// <summary>
		/// Creates Mapped property
		/// </summary>
		/// <param name="aAdaptor">
		/// Adaptor mapped property is connected to <see cref="IAdaptor"/>
		/// </param>
		/// <param name="aName">
		/// Name of mapped property <see cref="System.String"/>
		/// </param>
		public MappedProperty (IAdaptor aAdaptor, string aName)
		{
			// Default Mapped property
			if (aAdaptor == null)
				throw new ExceptionMappedPropertyWithNullAdaptor();
			adaptor = new WeakReference (aAdaptor);
			if (Adaptor.IsBoundaryAdaptor == true)
				throw new ExceptionNonBoundaryMappingSetToBoundary (aName);
			Name = aName;
			RWFlags = EReadWrite.ReadWrite;
			originalRWFlags = RWFlags;
			Resolve();
		}

		/// <summary>
		/// Creates Mapped property
		/// </summary>
		/// <param name="aAdaptor">
		/// Adaptor mapped property is connected to <see cref="IAdaptor"/>
		/// </param>
		/// <param name="aName">
		/// Name of mapped property <see cref="System.String"/>
		/// </param>
		/// <param name="aRWFlags">
		/// Read write flags <see cref="EReadWrite"/>
		/// </param>
		/// <param name="aTarget">
		/// Target mapping <see cref="System.String"/>
		/// </param>
		public MappedProperty (IAdaptor aAdaptor, string aName, EReadWrite aRWFlags, string aTarget)
		{
			// Secondary Mapped property
			if (aAdaptor == null)
				throw new ExceptionMappedPropertyWithNullAdaptor();
			adaptor = new WeakReference (aAdaptor);
			if (Adaptor.IsBoundaryAdaptor == true)
				throw new ExceptionNonBoundaryMappingSetToBoundary (aName, aTarget); 
			Name = aName;
			mappingTarget = aTarget;
			originalRWFlags = aRWFlags;
			RWFlags = aRWFlags;
			if (aTarget == "")
				throw new ExceptionMappingRequiresDefinedTarget (aName); 
			Resolve();
		}

		/// <summary>
		/// Creates Mapped property
		/// </summary>
		/// <param name="aAdaptor">
		/// Adaptor mapped property is connected to <see cref="IAdaptor"/>
		/// </param>
		/// <param name="aIsColumn">
		/// Defines if this is column or not <see cref="System.Boolean"/>
		/// </param>
		/// <param name="aName">
		/// Name of mapped property <see cref="System.String"/>
		/// </param>
		/// <param name="aColumnName">
		/// Name of column <see cref="System.String"/>
		/// </param>
		/// <param name="aRWFlags">
		/// Read write flags <see cref="EReadWrite"/>
		/// </param>
		/// <param name="aSubItems">
		/// Sub items <see cref="SMappedItem"/>
		/// </param>
		public MappedProperty (IAdaptor aAdaptor, bool aIsColumn, string aName, string aColumnName, 
		                       EReadWrite aRWFlags, SMappedItem[] aSubItems)
		{
			// Column Mapped property
			if (aAdaptor == null)
				throw new ExceptionMappedPropertyWithNullAdaptor();
			submappings = new ChildMappedProperties();
			adaptor = new WeakReference (aAdaptor);
			if (Adaptor.IsBoundaryAdaptor == true)
				throw new ExceptionNonBoundaryMappingSetToBoundary (aName, true, aColumnName); 
			Name = aName;
			IsColumnMapping = aIsColumn;
			originalRWFlags = aRWFlags;
			RWFlags = aRWFlags;
			if (IsColumnMapping == true)
				submappings = new ChildMappedProperties();
			if (aSubItems != null) {
				Submappings.Size = aSubItems.Length;
				foreach (SMappedItem item in aSubItems)
					Submappings.AddMapping (new MappedProperty (this, item));
			}
			ColumnName = aColumnName;
			Resolve();
		}

		/// <summary>
		/// Creates Mapped property
		/// </summary>
		/// <param name="aAdaptor">
		/// Adaptor mapped property is connected to <see cref="IAdaptor"/>
		/// </param>
		/// <param name="aIsColumn">
		/// Defines if this is column or not <see cref="System.Boolean"/>
		/// </param>
		/// <param name="aName">
		/// Name of mapped property <see cref="System.String"/>
		/// </param>
		/// <param name="aColumnName">
		/// Name of column <see cref="System.String"/>
		/// </param>
		/// <param name="aRWFlags">
		/// Read write flags <see cref="EReadWrite"/>
		/// </param>
		/// <param name="aTarget">
		/// Target mapping <see cref="System.String"/>
		/// </param>
		/// <param name="aSubItems">
		/// Sub items <see cref="SMappedItem"/>
		/// </param>
		public MappedProperty (IAdaptor aAdaptor, bool aIsColumn, string aName, string aColumnName, EReadWrite aRWFlags, 
		                       string aTarget, SMappedItem[] aSubItems)
		{
			// Targeted Column Mapped property
			if (aAdaptor == null)
				throw new ExceptionMappedPropertyWithNullAdaptor();
			adaptor = new WeakReference (aAdaptor);
			if (Adaptor.IsBoundaryAdaptor == true)
				throw new ExceptionNonBoundaryMappingSetToBoundary (aName, aTarget, aColumnName); 
			Name = aName;
			submappings = new ChildMappedProperties();
			mappingTarget = aTarget;
			IsColumnMapping = aIsColumn;
			if (IsColumnMapping == true)
				submappings = new ChildMappedProperties();
			ColumnName = aColumnName;
			originalRWFlags = aRWFlags;
			RWFlags = aRWFlags;
			if (aSubItems != null)
				foreach (SMappedItem item in aSubItems)
					Submappings.AddMapping (new MappedProperty (this, item));
			if (aTarget == "")
				throw new ExceptionMappingRequiresDefinedTarget (aName); 
			Resolve();
		}

		/// <summary>
		/// Creates Mapped property
		/// </summary>
		/// <param name="aAdaptor">
		/// Adaptor mapped property is connected to <see cref="IAdaptor"/>
		/// </param>
		/// <param name="aClass">
		/// Class name conected to mapping <see cref="System.String"/>
		/// </param>
		/// <param name="aName">
		/// Name of mapped property <see cref="System.String"/>
		/// </param>
		/// <param name="aRWFlags">
		/// Read write flags <see cref="EReadWrite"/>
		/// </param>
		/// <param name="aTarget">
		/// Target mapping <see cref="System.String"/>
		/// </param>
		public MappedProperty (IAdaptor aAdaptor, string aClass, string aName, EReadWrite aRWFlags, string aTarget)
		{
			// Global Mapped property
			if (aAdaptor == null)
				throw new ExceptionMappedPropertyWithNullAdaptor();
			adaptor = new WeakReference (aAdaptor);
			if (Adaptor.IsBoundaryAdaptor == true)
				throw new ExceptionNonBoundaryMappingSetToBoundary (aName, aTarget); 
			Name = aName;
			mappingTarget = aTarget;
			BoundingClassName = aClass;
			originalRWFlags = aRWFlags;
			RWFlags = aRWFlags;
			if (aTarget == "")
				throw new ExceptionMappingRequiresDefinedTarget (aName); 
			Resolve();
		}

		/// <summary>
		/// Creates Mapped property
		/// </summary>
		/// <param name="aMasterItem">
		/// Master item <see cref="MappedProperty"/>
		/// </param>
		/// <param name="aItem">
		/// Item <see cref="SMappedItem"/>
		/// </param>
		public MappedProperty (MappedProperty aMasterItem, SMappedItem aItem)
		{
			if (aMasterItem == null)
				throw new ExceptionMasterItemIsNull();
			masterItem = aMasterItem;
			isSubItem = true;
			adaptor = new WeakReference (masterItem.Adaptor);
/*			if (Adaptor.IsBoundaryAdaptor == true)
				throw new Exception ("You can't set Boundary Mapping to non-Boundary adaptor >> Name=" + aItem.Name + " Target=" + aItem.MappedItem);*/ 
			Name = aItem.Name;
			BoundingClassName = aItem.ClassName;
			mappingTarget = aItem.MappedItem;
			originalRWFlags = aItem.RWFlags;
			RWFlags = aItem.RWFlags;
//			aMasterItem.Submappings.AddMapping(this);
		}

		/// <summary>
		/// Disconnects and destroys MappedProperty
		/// </summary>
		~MappedProperty()
		{
			Disconnect();
		}
	}
}
