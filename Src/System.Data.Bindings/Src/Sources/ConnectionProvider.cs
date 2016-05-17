// ConnectionProvider.cs - ConnectionProvider implementation for Gtk#Databindings
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
using System.Data.Bindings.DebugInformation;

namespace System.Data.Bindings
{
	/// <summary>
	/// Provides common basic functionality of connecting objects and adaptors.
	/// </summary>
	public static class ConnectionProvider
	{
		/// <summary>
		/// Copies value from one property to another
		/// </summary>
		/// <param name="aFromObj">
		/// Source object <see cref="System.Object"/>
		/// </param>
		/// <param name="aFrom">
		/// Source property <see cref="PropertyInfo"/>
		/// </param>
		/// <param name="aToObj">
		/// Destination object <see cref="System.Object"/>
		/// </param>
		/// <param name="aTo">
		/// Destination property <see cref="PropertyInfo"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		public static bool CopyPropertyValueByInfo (object aFromObj, PropertyInfo aFrom, object aToObj, PropertyInfo aTo)
		{
			bool res = false;
			if ((aFrom == null) || (aTo == null) || (aFromObj == null) || (aToObj == null)) {
				Debug.Warning ("CopyPropertyValueByInfo", "One is null" + (aFrom == null) + (aTo == null) + (aFromObj == null) + (aToObj == null));
				return (false);
			}
			if (aFrom.CanRead == false)
				throw new ExceptionGettingWriteOnlyProperty();
			object val = aFrom.GetValue (aFromObj, null);
			if (val == null)
				return (false);
				
			if (aTo.CanWrite == false)
				throw new ExceptionAssigningReadOnlyProperty();
			try {
				object no = System.Convert.ChangeType (val, aTo.PropertyType);
				if (no is IComparable) {
					object cval = aTo.GetValue (aToObj, null);
					if ((cval as IComparable).CompareTo(no) != 0) {
						aTo.SetValue (aToObj, no, null);
						res = true;
					}
				}
				else {
					res = true;
					aTo.SetValue (aToObj, no, null);
				}
				no = null;
			}
			catch (System.InvalidCastException) { 
				res = false;
				throw new ExceptionPropertyTranslationError();
			}
			catch (System.ArgumentNullException) {
				res = false;
				throw new ExceptionPropertyTranslationError();
			}
			catch (System.FormatException) {
				res = false;
				throw new ExceptionPropertyTranslationError();
			}
			catch (System.OverflowException) {
				res = false;
				throw new ExceptionPropertyTranslationError();
			}
			
			val = null;
			return (res);
		}
		
		/// <summary>
		/// Copies value from one property to another
		/// </summary>
		/// <param name="aFromObj">
		/// Source object <see cref="System.Object"/>
		/// </param>
		/// <param name="aFrom">
		/// Source property <see cref="System.String"/>
		/// </param>
		/// <param name="aToObj">
		/// Destination object <see cref="System.Object"/>
		/// </param>
		/// <param name="aTo">
		/// Destination property <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		public static bool CopyPropertyValue (object aFromObj, string aFrom, object aToObj, string aTo)
		{
			PropertyInfo from = ResolveMappingProperty (aFromObj, aFrom);
			PropertyInfo to = ResolveMappingProperty (aToObj, aTo);
			if (from == null)
				Debug.Warning ("ConnectionProvider.CopyPropertyValue", "Not serious (handled in more than one loop): Error resolving property " + aFrom + " in " + aFromObj);
			if (to == null)
				Debug.Warning ("ConnectionProvider.CopyPropertyValue", "Not serious (handled in more than one loop): Error resolving property " + aTo + " in " + aToObj);
			bool res = CopyPropertyValueByInfo (aFromObj, from, aToObj, to);
			from = to = null;
			return (res);
		}
		
		/// <summary>
		/// Resolves mapping type in target without caching and returning without Exception
		/// if Mapping does not exists
		/// </summary>
		/// <param name="aTarget">
		/// Object to resolve property from <see cref="System.Object"/>
		/// </param>
		/// <param name="aMapping">
		/// Property name <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Property info if successful, null if not <see cref="PropertyInfo"/>
		/// </returns>
		/// <remarks>
		/// Does not throw exception on unsuccessful try
		/// </remarks>
		public static PropertyInfo ResolveMappingProperty (object aTarget, string aMapping)
		{
			return (ResolveMappingProperty (aTarget, aMapping, false));
		}
		
		/// <summary>
		/// Resolves mapping type in target without caching and returning with Exception
		/// if Mapping does not exists
		/// </summary>
		/// <param name="aTarget">
		/// Object to resolve property from <see cref="System.Object"/>
		/// </param>
		/// <param name="aMapping">
		/// Property name <see cref="System.String"/>
		/// </param>
		/// <param name="aThrowException">
		/// Set true if throwing exception on non success is needed <see cref="System.Boolean"/>
		/// </param>
		/// <returns>
		/// Property info if successful, null if not <see cref="PropertyInfo"/>
		/// </returns>
		public static PropertyInfo ResolveMappingProperty (object aTarget, string aMapping, bool aThrowException)
		{
			if (aMapping == "")
				return (null);
			
			return (ResolveMappingPropertyByType (aTarget.GetType(), aMapping, aThrowException));
		}
		
		/// <summary>
		/// Resolves mapping type in target type without caching and returning with Exception
		/// if Mapping does not exists
		/// </summary>
		/// <param name="aTarget">
		/// Object type to resolve property from <see cref="System.Type"/>
		/// </param>
		/// <param name="aMapping">
		/// Property name <see cref="System.String"/>
		/// </param>
		/// <param name="aThrowException">
		/// Set true if throwing exception on non success is needed <see cref="System.Boolean"/>
		/// </param>
		/// <returns>
		/// Property info if successful, null if not <see cref="PropertyInfo"/>
		/// </returns>
		public static PropertyInfo ResolveMappingPropertyByType (System.Type aTarget, string aMapping, bool aThrowException)
		{
			if ((aMapping == "") || (aTarget == null))
				return (null);
			
/*			if (TypeValidator.IsCompatible(aTarget, TypeValidator.typeIVirtualObject) == true)
				throw new Exception ("Searching property by type for VirtualObject is not possible! Check and correct implementation!");*/
			foreach (PropertyInfo pInfo in aTarget.GetProperties ())
				if (pInfo.Name == aMapping)
					return (pInfo);

			if (aThrowException == true)
				throw new ExceptionPropertyNotFound (aMapping);
			return (null);
		}
		
		/// <summary>
		/// Resolves mapping type in target without caching and returning without Exception
		/// if Mapping does not exists
		/// </summary>
		/// <param name="aTarget">
		/// Object to resolve property type from <see cref="System.Object"/>
		/// </param>
		/// <param name="aMapping">
		/// Property name <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Type of property value <see cref="System.Type"/>
		/// </returns>
		/// <remarks>
		/// Does not throw exception on unsuccessful try
		/// </remarks>
		public static System.Type ResolveMappingType (object aTarget, string aMapping)
		{
			return (ResolveMappingType (aTarget, aMapping, false));
		}
		
		/// <summary>
		/// Resolves mapping type in target without caching and where aThrowException
		/// decides if Exception should be returned when aMapping does not exists
		/// </summary>
		/// <param name="aTarget">
		/// Object to resolve property type from <see cref="System.Object"/>
		/// </param>
		/// <param name="aMapping">
		/// Property name <see cref="System.String"/>
		/// </param>
		/// <param name="aThrowException">
		/// Set true if throwing exception on non success is needed <see cref="System.Boolean"/>
		/// </param>
		/// <returns>
		/// Type of property value <see cref="System.Type"/>
		/// </returns>
		public static System.Type ResolveMappingType (object aTarget, string aMapping, bool aThrowException)
		{
			if ((aTarget == null) || (aMapping == ""))
				return ((System.Type) null);
				
			if (aMapping == "")
				return ((System.Type) null);
				
			// Redirect to final target if this object is IDataAdaptor
			if (aTarget is IDataAdaptor)
				return (ResolveMappingType ((aTarget as IDataAdaptor).FinalTarget, aMapping, aThrowException));
				
			return (ResolveMappingTypeByType (aTarget.GetType(), aMapping, aThrowException));
		}
		
		/// <summary>
		/// Resolves mapping type in target type without caching and where aThrowException
		/// decides if Exception should be returned when aMapping does not exists
		/// </summary>
		/// <param name="aTarget">
		/// Object type from which to resolve property type from <see cref="System.Type"/>
		/// </param>
		/// <param name="aMapping">
		/// Property name <see cref="System.String"/>
		/// </param>
		/// <param name="aThrowException">
		/// Set true if throwing exception on non success is needed <see cref="System.Boolean"/>
		/// </param>
		/// <returns>
		/// Type of property value <see cref="System.Type"/>
		/// </returns>
		public static System.Type ResolveMappingTypeByType (System.Type aTarget, string aMapping, bool aThrowException)
		{
			if ((aTarget == null) || (aMapping == ""))
				return ((System.Type) null);
				
			if (aMapping == "")
				return ((System.Type) null);
				
			// Redirect to final target if this object is IDataAdaptor
			if (aTarget is IDataAdaptor)
				return (ResolveMappingType ((aTarget as IDataAdaptor).FinalTarget, aMapping, aThrowException));
				
			Type objType = aTarget;
			
			foreach (PropertyInfo pInfo in objType.GetProperties ())
				if (pInfo.Name == aMapping)
					return (pInfo.PropertyType);
			
			if (aThrowException == true) {
				Console.WriteLine ("CLASS DEBUG");
				Console.WriteLine ("=============================================");
				foreach (PropertyInfo pInfo in objType.GetProperties ())
					Console.WriteLine ("INSIDEOF THIS CLASS: " + pInfo.Name);
				throw new ExceptionPropertyNotFound (aMapping, aTarget);
			}
			return ((System.Type) null);
		}
		
		/// <summary>
		/// Resolves target for given adaptor
		/// </summary>
		/// <param name="aAdaptor">
		/// Adaptor from which to resolve target <see cref="IAdaptor"/>
		/// </param>
		/// <returns>
		/// Object to which adaptor is pointing <see cref="System.Object"/>
		/// </returns>
		public static object ResolveTargetFor (IAdaptor aAdaptor)
		{
			if (aAdaptor == null)
				return (null);
				
//			return (ResolveTargetForObject (aAdaptor.FinalTarget));
			return (ResolveTargetForObject (aAdaptor.Target));
		}
		
		/// <summary>
		/// Resolves target for given object or adaptor
		/// </summary>
		/// <param name="aTarget">
		/// Object to resolve target for <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Target of adaptor or returns object if this wasn't adaptor <see cref="System.Object"/>
		/// </returns>
		public static object ResolveTargetForObject (object aTarget)
		{
			if (aTarget == null)
				return (null);
				
			if (aTarget is IDataAdaptor) 
				return (ResolveTargetForObject ((aTarget as IDataAdaptor).Target));

			return (aTarget);
		}
		
		/// <summary>
		/// Resolves aMapping property value in target and returns null without Exception
		/// if Mapping does not exists
		/// </summary>
		/// <param name="aTarget">
		/// Source object <see cref="System.Object"/>
		/// </param>
		/// <param name="aMapping">
		/// Property name <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Mapped property value <see cref="System.Object"/>
		/// </returns>
		/// <remarks>
		/// Does not throw exception on unsuccessful try
		/// </remarks>
		public static object ResolveMappingValue (object aTarget, string aMapping)
		{
			return (ResolveMappingValue (aTarget, aMapping, false));
		}
		
		/// <summary>
		/// Resolves aMapping property value in target and returns null or Exception
		/// if Mapping does not exists based on the aThrowException parameter
		/// </summary>
		/// <param name="aTarget">
		/// Source object <see cref="System.Object"/>
		/// </param>
		/// <param name="aMapping">
		/// Property name <see cref="System.String"/>
		/// </param>
		/// <param name="aThrowException">
		/// Set true if throwing exception on non success is needed <see cref="System.Boolean"/>
		/// </param>
		/// <returns>
		/// Mapped property value <see cref="System.Object"/>
		/// </returns>
		public static object ResolveMappingValue (object aTarget, string aMapping, bool aThrowException)
		{
			if ((aTarget == null) || (aMapping == ""))
				return (null);
				
			if (aTarget is IDataAdaptor)
				return (ResolveMappingValue ((aTarget as IDataAdaptor).FinalTarget, aMapping, aThrowException));
				
			Type type = aTarget.GetType();
			
			foreach (PropertyInfo pi in type.GetProperties ())
				if (pi.Name == aMapping)
					return (pi.GetValue(aTarget, null));
			
			if (aThrowException == true)
				throw new ExceptionPropertyNotFound (aMapping, aTarget);
			return (null);
		}
		
		/// <summary>
		/// Takes care of connecting control and Adaptor 
		/// </summary>
		/// <param name="aControl">
		/// Control that needs connection <see cref="System.Object"/>
		/// </param>
		/// <param name="aAdaptor">
		/// Adaptor to connect control with <see cref="IAdaptor"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		public static bool ConnectControlAndAdaptor (object aControl, IAdaptor aAdaptor)
		{
			if ((aControl == null) || (aAdaptor == null))
				return (false);
			if (aControl is IAdaptableControl)
//				aAdaptor.OnDataChange += (aControl as IAdaptableControl).Adaptor.AdapteeDataChange;
//				aAdaptor.OnDataChange += (aControl as IAdaptableControl).GetDataFromDataSource;
				aAdaptor.DataChanged += (aControl as IAdaptableControl).CallAdaptorGetData;
			return (true);
		}
		
		/// <summary>
		/// Takes care of disconnecting control and Adaptor
		/// </summary>
		/// <param name="aControl">
		/// Control that needs disconnection <see cref="System.Object"/>
		/// </param>
		/// <param name="aAdaptor">
		/// Adaptor to disconnect from control <see cref="IAdaptor"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		public static bool DisconnectControlAndAdaptor (object aControl, IAdaptor aAdaptor)
		{
			if ((aControl == null) || (aAdaptor == null))
				return (false);
			if (aControl is IAdaptableControl)
//				aAdaptor.OnDataChange -= (aControl as IAdaptableControl).Adaptor.AdapteeDataChange;
//				aAdaptor.OnDataChange -= (aControl as IAdaptableControl).GetDataFromDataSource;
				aAdaptor.DataChanged -= (aControl as IAdaptableControl).CallAdaptorGetData;
			return (true);
		}

		/// <summary>
		/// Resolves DaraSource from parent control
		/// </summary>
		/// <param name="aAdaptor">
		/// Control adaptor to use <see cref="ControlAdaptor"/>
		/// </param>
		/// <param name="aControl">
		/// Control which parent is to be resolved <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Object of parent controls datasource adaptor or null <see cref="System.Object"/>
		/// </returns>
		public static object ResolveDataSourceFromParent (ControlAdaptor aAdaptor, object aControl)
		{		
			if ((aControl == null) || (aAdaptor == null))
				return (null);
			object ctrl = aAdaptor.GetParentOfControl (aControl);
			if (aAdaptor.ControlIsWindow(aControl) == true)
				return (null);

			while (ctrl != null) {
				if (ctrl is IAdaptableContainer)
					return ((ctrl as IAdaptableContainer).Adaptor.Adaptor);
				ctrl = aAdaptor.GetParentOfControl (ctrl);
			}
			return (null);
		}
		
		/// <summary>
		/// Resolves parent control
		/// </summary>
		/// <param name="aAdaptor">
		/// Control adaptor to use <see cref="ControlAdaptor"/>
		/// </param>
		/// <param name="aControl">
		/// Control which parent is to be resolved <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Parent container or null <see cref="System.Object"/>
		/// </returns>
		public static object ResolveParentContainer (ControlAdaptor aAdaptor, object aControl)
		{		
			if ((aControl == null) || (aAdaptor == null))
				return (null);
			object ctrl = aAdaptor.GetParentOfControl (aControl);
			if (aAdaptor.ControlIsWindow(aControl) == true)
				return (null);

			while (ctrl != null) {
				if (ctrl is IAdaptableContainer)
					return (ctrl as IAdaptableContainer);
//					return ((ctrl as IAdaptableContainer).Adaptor.Adaptor);
				ctrl = aAdaptor.GetParentOfControl (ctrl);
			}
			return (null);
		}
		
		/// <summary>
		/// Resolves first parent which is IBoundedContainer and returns its DataSource  
		/// </summary>
		/// <param name="aAdaptor">
		/// Control adaptor to use <see cref="ControlAdaptor"/>
		/// </param>
		/// <param name="aControl">
		/// Control which parent is to be resolved <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Object of parent controls datasource or null <see cref="System.Object"/>
		/// </returns>
		public static object ResolveBoundaryDataSourceFromParent (ControlAdaptor aAdaptor, object aControl)
		{
			if (aControl == null)
				return (null);
			object ctrl = aAdaptor.GetParentOfControl (aControl);
			while (ctrl != null) {
				if (ctrl is IBoundedContainer)
					if ((ctrl as IBoundedContainer).InheritedBoundaryDataSource == false)
						return ((ctrl as IBoundedContainer).BoundaryDataSource);
				ctrl = aAdaptor.GetParentOfControl (ctrl);
			}
			return (null);
		}

		/// <summary>
		/// Resolves value from referenced Target
		/// </summary>
		/// <param name="aObject">
		/// Object to resolve value from <see cref="System.Object"/>
		/// </param>
		/// <param name="aProp">
		/// Property which to resolve <see cref="PropertyInfo"/>
		/// </param>
		/// <returns>
		/// Value of the property <see cref="System.Object"/>
		/// </returns>
		public static object GetPropertyValue (object aObject, PropertyInfo aProp)
		{
			if ((aProp == null) || (aObject == null))
				return (null);
			
			if (aObject is IDataAdaptor)
				return (GetPropertyValue ((aObject as IDataAdaptor).FinalTarget, aProp));
				
			if (aProp.CanRead == true)
				return (aProp.GetValue (aObject, null));
			return (null);
		}

		/// <summary>
		/// Resolves property hint from property attributes
		/// </summary>
		/// <param name="aObject">
		/// Object property belongs to <see cref="System.Object"/>
		/// </param>
		/// <param name="aProp">
		/// Property info <see cref="PropertyInfo"/>
		/// </param>
		/// <returns>
		/// Hint string <see cref="System.String"/>
		/// </returns>
		public static string GetPropertyHint (object aObject, PropertyInfo aProp)
		{
			PropertyDescriptionAttribute attr = GetPropertyDescription (aObject, aProp);
			if (attr == null)
				return ("");
			return (attr.Hint);
		}
		
		/// <summary>
		/// Resolves property title from property attributes
		/// </summary>
		/// <param name="aObject">
		/// Object property belongs to <see cref="System.Object"/>
		/// </param>
		/// <param name="aProp">
		/// Property info <see cref="PropertyInfo"/>
		/// </param>
		/// <returns>
		/// Title string <see cref="System.String"/>
		/// </returns>
		public static string GetPropertyTitle (object aObject, PropertyInfo aProp)
		{
			PropertyDescriptionAttribute attr = GetPropertyDescription (aObject, aProp);
			if (attr == null)
				return ("");
			return (attr.Title);
		}
		
		/// <summary>
		/// Resolves property description from property attributes
		/// </summary>
		/// <param name="aObject">
		/// Object property belongs to <see cref="System.Object"/>
		/// </param>
		/// <param name="aProp">
		/// Property info <see cref="PropertyInfo"/>
		/// </param>
		/// <returns>
		/// Title string <see cref="PropertyDescriptionAttribute"/>
		/// </returns>
		public static PropertyDescriptionAttribute GetPropertyDescription (object aObject, PropertyInfo aProp)
		{
			if ((aProp == null) || (aObject == null))
				return (null);
			PropertyDescriptionAttribute[] attrs = (PropertyDescriptionAttribute[]) aProp.GetCustomAttributes (typeof(PropertyDescriptionAttribute), true);
			if ((attrs == null) || (attrs.Length == 0))
				return (null);
			return ((PropertyDescriptionAttribute) attrs[0]);
		}
		
		/// <summary>
		/// Resolves property title from property attributes
		/// </summary>
		/// <param name="aType">
		/// Type property belongs to <see cref="System.Type"/>
		/// </param>
		/// <param name="aPropertyName">
		/// Property name <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Property description <see cref="PropertyDescriptionAttribute"/>
		/// </returns>
		public static PropertyDescriptionAttribute GetPropertyDescription (System.Type aType, string aPropertyName)
		{
			if (aType == null)
				return (null);
			PropertyInfo info = aType.GetProperty (aPropertyName);
			if (info == null)
				return (null);
			PropertyDescriptionAttribute[] attrs = (PropertyDescriptionAttribute[]) info.GetCustomAttributes (typeof(PropertyDescriptionAttribute), true);
			if ((attrs == null) || (attrs.Length == 0))
				return (null);
			return ((PropertyDescriptionAttribute) attrs[0]);
		}
		
		/// <summary>
		/// Resolves property title from property attributes
		/// </summary>
		/// <param name="aType">
		/// Type property belongs to <see cref="System.Type"/>
		/// </param>
		/// <param name="aPropertyName">
		/// Property name <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Title string <see cref="System.String"/>
		/// </returns>
		public static string GetPropertyTitle (System.Type aType, string aPropertyName)
		{
			PropertyDescriptionAttribute attr = GetPropertyDescription (aType, aPropertyName);
			if (attr == null)
				return ("");
			return (attr.Title);
		}
		
		/// <summary>
		/// Resolves property field name from property attributes
		/// </summary>
		/// <param name="aObject">
		/// Object property belongs to <see cref="System.Object"/>
		/// </param>
		/// <param name="aProp">
		/// Property info <see cref="PropertyInfo"/>
		/// </param>
		/// <returns>
		/// Field name <see cref="System.String"/>
		/// </returns>
		public static string GetPropertyFieldName (object aObject, PropertyInfo aProp)
		{
			PropertyDescriptionAttribute attr = GetPropertyDescription (aObject, aProp);
			if (attr == null)
				return ("");
			return (attr.FieldName);
		}

		/// <summary>
		/// Resolves property handler from property attributes
		/// </summary>
		/// <param name="aObject">
		/// Object property belongs to <see cref="System.Object"/>
		/// </param>
		/// <param name="aProp">
		/// Property info <see cref="PropertyInfo"/>
		/// </param>
		/// <returns>
		/// Property handler name <see cref="System.String"/>
		/// </returns>
		public static string GetPropertyHandler (object aObject, PropertyInfo aProp)
		{
			PropertyDescriptionAttribute attr = GetPropertyDescription (aObject, aProp);
			if (attr == null)
				return ("");
			return (attr.DataTypeHandler);
		}
		
		/// <summary>
		/// Resolves property handler type from property attributes
		/// </summary>
		/// <param name="aObject">
		/// Object property belongs to <see cref="System.Object"/>
		/// </param>
		/// <param name="aProp">
		/// Property info <see cref="PropertyInfo"/>
		/// </param>
		/// <returns>
		/// Property handler type <see cref="PropertyHandlerType"/>
		/// </returns>
		public static PropertyHandlerType GetPropertyHandlerType (object aObject, PropertyInfo aProp)
		{
			PropertyDescriptionAttribute attr = GetPropertyDescription (aObject, aProp);
			if (attr == null)
				return (PropertyHandlerType.Default);
			return (attr.HandlerType);
		}
		
		/// <summary>
		/// Resolves value from referenced Target
		/// </summary>
		/// <param name="aObject">
		/// Object to resolve value from <see cref="System.Object"/>
		/// </param>
		/// <param name="aProp">
		/// Property which to resolve <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Value of the property <see cref="System.Object"/>
		/// </returns>
		public static object GetPropertyValue (object aObject, string aProp)
		{
			PropertyInfo info = ResolveMappingProperty (aObject, aProp);
			return (GetPropertyValue (aObject, info));
		}
		
		/// <summary>
		/// Gets value from referenced Target
		/// </summary>
		/// <param name="aObject">
		/// Object to set property value to <see cref="System.Object"/>
		/// </param>
		/// <param name="aProp">
		/// Property which needs setting <see cref="PropertyInfo"/>
		/// </param>
		/// <param name="aValue">
		/// Value to be set <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		public static bool SetPropertyValue (object aObject, PropertyInfo aProp, object aValue)
		{
			if ((aProp == null) || (aObject == null))
				return (false);
			
			if (aObject is IDataAdaptor)
				return (SetPropertyValue ((aObject as IDataAdaptor).FinalTarget, aProp, aValue));
				
			bool res = false;
			if (aProp.CanWrite == true) {
				object no = System.Convert.ChangeType (aValue, aProp.PropertyType);
				if (no is IComparable) {
					object cval = aProp.GetValue (aObject, null);
					if ((cval as IComparable).CompareTo(no) != 0) {
						aProp.SetValue (aObject, no, null);
						res = true;
					}
				}
				else {
					res = true;
					aProp.SetValue (aObject, no, null);
				}
				no = null;
			}
			else
				throw new ExceptionAssigningReadOnlyProperty();
			return (res);
		}
		
		/// <summary>
		/// Gets value from referenced Target
		/// </summary>
		/// <param name="aObject">
		/// Object to set property value to <see cref="System.Object"/>
		/// </param>
		/// <param name="aProp">
		/// Property which needs setting <see cref="System.String"/>
		/// </param>
		/// <param name="aValue">
		/// Value to be set <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		public static bool SetPropertyValue (object aObject, string aProp, object aValue)
		{
			PropertyInfo info = ResolveMappingProperty (aObject, aProp);
			return (SetPropertyValue (aObject, info, aProp));
		}
		
		/// <summary>
		/// Adds new index on path start
		/// </summary>
		/// <param name="aIdx">
		/// Index to be inserted <see cref="System.Int32"/>
		/// </param>
		/// <param name="aPath">
		/// Path where it has to be inserted <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// New path <see cref="System.Int32"/>
		/// </returns>
		public static int[] AddPathIndexOnStart (int aIdx, int[] aPath)
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
		/// Adds new index on path end
		/// </summary>
		/// <param name="aIdx">
		/// Index to be inserted <see cref="System.Int32"/>
		/// </param>
		/// <param name="aPath">
		/// Path where it has to be inserted <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// New path <see cref="System.Int32"/>
		/// </returns>
		public static int[] AddPathIndexOnEnd (int aIdx, int[] aPath)
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
		/// Returns path as string
		/// </summary>
		/// <param name="aPath">
		/// Path <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// String interpretation of the path <see cref="System.String"/>
		/// </returns>
		public static string PathToString (int[] aPath)
		{
			string res = "[";
			if (aPath != null) {
				res = res + aPath[0];
				for (int i=1; i<aPath.Length; i++)
					res = res + "," + aPath[i];
			}
			res = res + "]";
			return (res);
		}
		
		/// <summary>
		/// Compares two paths
		/// </summary>
		/// <param name="aFromPath">
		/// First path <see cref="System.Int32"/>
		/// </param>
		/// <param name="aToPath">
		/// Second path <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// true if paths are equal <see cref="System.Boolean"/>
		/// </returns>
		public static bool PathIsEqual (int[] aFromPath, int[] aToPath)
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
		/// <param name="aFromPath">
		/// First path <see cref="System.Int32"/>
		/// </param>
		/// <param name="aToPath">
		/// Second path <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// 0 if paths are equal
		/// 1 if first path is greater
		/// -1 if second path is greater <see cref="System.Int32"/>
		/// </returns>
		public static int ComparePath (int[] aFromPath, int[] aToPath)
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
