// DatabaseHelper.cs - Field Attribute to assign additional information for Gtk#Databindings
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
using System.Data;
using System.Data.Bindings.Collections;

namespace System.Data.Bindings
{
	/// <summary>
	/// Provides needed routines for resolving and hadling data with
	/// database objects
	/// </summary>
	public static class DatabaseProvider
	{
		private static Type[] databaseTypes =
			new System.Type[3] {
				typeof(DataTable), 
				typeof(DataView), 
//				typeof(DbObservableList),
				typeof(DataRowCollection)
			};
		
		private static Type[] databaseExceptionTypes =
			new System.Type[1] {
				typeof(DbObservableList)
			};
		
		private static Type[] databaseRowTypes =
			new System.Type[2] {
				typeof(DataRow), 
				typeof(DataRowView)
			};
		
		/// <summary>
		/// Checks if object is database elated or not
		/// </summary>
		/// <param name="aObject">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsValidType (object aObject)
		{
			if (aObject == null)
				return (false);
			return (IsValidType(aObject.GetType()) == true);
		}
		
		/// <summary>
		/// Checks if object is database elated or not
		/// </summary>
		/// <param name="aObjectType">
		/// A <see cref="System.Type"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsValidType (System.Type aObjectType)
		{
			if (aObjectType == null)
				return (false);
			if ((TypeValidator.IsCompatible(aObjectType, databaseTypes) == true) ||
			    (TypeValidator.IsCompatible(aObjectType, databaseRowTypes) == true))
				return (true);
			return (false);
		}
		
		/// <summary>
		/// Checks if object has collection of datarows
		/// </summary>
		/// <param name="aObject">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsDataRowContainer (object aObject)
		{
			if (aObject == null)
				return (false);
			return (IsDataRowContainer(aObject.GetType()) == true);
		}
		
		/// <summary>
		/// Checks if object has collection of datarows
		/// </summary>
		/// <param name="aObjectType">
		/// A <see cref="System.Type"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsDataRowContainer (System.Type aObjectType)
		{
			if (aObjectType == null)
				return (false);
			if (TypeValidator.IsCompatible(aObjectType, databaseRowTypes) == true)
				return (true);
			return (false);
		}
		
		/// <summary>
		/// Checks if object has collection of datarows
		/// </summary>
		/// <param name="aObject">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsDatabaseException (object aObject)
		{
			if (aObject == null)
				return (false);
			if (TypeValidator.IsCompatible(aObject.GetType(), databaseExceptionTypes) == true)
				return (true);
			return (false);
		}
		
		/// <summary>
		/// Checks if object has collection of datarows
		/// </summary>
		/// <param name="aObjectType">
		/// A <see cref="System.Type"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsDatabaseException (System.Type aObjectType)
		{
			if (aObjectType == null)
				return (false);
			if (TypeValidator.IsCompatible(aObjectType, databaseExceptionTypes) == true)
				return (true);
			return (false);
		}
		
		/// <summary>
		/// Checks if specified property exists in database object and returns DataColumn
		/// </summary>
		/// <param name="aObject">
		/// Object to check in <see cref="System.Object"/>
		/// </param>
		/// <param name="aPropertyName">
		/// Property name <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Object to column, null if it doesn't exists <see cref="System.Object"/>
		/// </returns>
		public static object PropertyExists (object aObject, string aPropertyName)
		{
			if ((IsValidType(aObject) == false) &&
			    (IsDataRowContainer(aObject) == false) &&
			    (IsDatabaseException(aObject) == false))
				return (null);
			if (TypeValidator.IsCompatible(aObject.GetType(), typeof(DataTable)) == true)
				return ((aObject as DataTable).Columns[aPropertyName.Trim()]);
			if (TypeValidator.IsCompatible(aObject.GetType(), typeof(DataView)) == true)
				return ((aObject as DataView).Table.Columns[aPropertyName.Trim()]);
			if (TypeValidator.IsCompatible(aObject.GetType(), typeof(DbObservableList)) == true)
				return ((aObject as DbObservableList).Table.Columns[aPropertyName.Trim()]);
			if (TypeValidator.IsCompatible(aObject.GetType(), typeof(DataRowView)) == true)
				return ((aObject as DataRowView).Row.Table.Columns[aPropertyName.Trim()]);
			return ((aObject as DataRow).Table.Columns[aPropertyName.Trim()]);
		}

		/// <summary>
		/// Resolves property hint from property attributes
		/// </summary>
		/// <param name="aObject">
		/// Object property belongs to <see cref="System.Object"/>
		/// </param>
		/// <param name="aProp">
		/// Data column info <see cref="DataColumn"/>
		/// </param>
		/// <returns>
		/// Hint string <see cref="System.String"/>
		/// </returns>
		public static string GetPropertyHint (object aObject, DataColumn aProp)
		{
			return ("");
		}
		
		/// <summary>
		/// Resolves property title from property attributes
		/// </summary>
		/// <param name="aObject">
		/// Object property belongs to <see cref="System.Object"/>
		/// </param>
		/// <param name="aProp">
		/// Data column info <see cref="DataColumn"/>
		/// </param>
		/// <returns>
		/// Title string <see cref="System.String"/>
		/// </returns>
		public static string GetPropertyTitle (object aObject, DataColumn aProp)
		{
			if ((aProp == null) || (aObject == null))
				return ("");
			return (aProp.Caption);
		}
		
		/// <summary>
		/// Resolves property field name from property attributes
		/// </summary>
		/// <param name="aObject">
		/// Object property belongs to <see cref="System.Object"/>
		/// </param>
		/// <param name="aProp">
		/// Data column info <see cref="DataColumn"/>
		/// </param>
		/// <returns>
		/// Field name <see cref="System.String"/>
		/// </returns>
		public static string GetPropertyFieldName (object aObject, DataColumn aProp)
		{
			if ((aProp == null) || (aObject == null))
				return ("");
			return (aProp.ColumnName);
		}

		/// <summary>
		/// Resolves property handler from property attributes
		/// </summary>
		/// <param name="aObject">
		/// Object property belongs to <see cref="System.Object"/>
		/// </param>
		/// <param name="aProp">
		/// Data column info <see cref="DataColumn"/>
		/// </param>
		/// <returns>
		/// Property handler name <see cref="System.String"/>
		/// </returns>
		public static string GetPropertyHandler (object aObject, DataColumn aProp)
		{
			return ("");
		}
		
		/// <summary>
		/// Resolves property handler type from property attributes
		/// </summary>
		/// <param name="aObject">
		/// Object property belongs to <see cref="System.Object"/>
		/// </param>
		/// <param name="aProp">
		/// Data column info <see cref="DataColumn"/>
		/// </param>
		/// <returns>
		/// Property handler type <see cref="PropertyHandlerType"/>
		/// </returns>
		public static PropertyHandlerType GetPropertyHandlerType (object aObject, DataColumn aProp)
		{
			return (PropertyHandlerType.Default);
		}
		
		/// <summary>
		/// Checks if specified property exists in database object and returns DataColumn
		/// </summary>
		/// <param name="aObject">
		/// Object to check in <see cref="System.Object"/>
		/// </param>
		/// <param name="aPropertyName">
		/// Property name <see cref="System.String"/>
		/// </param>
		/// <param name="aThrowException">
		/// Throws exception if property is not found <see cref="System.Boolean"/>
		/// </param>
		/// <returns>
		/// Object to column, null if it doesn't exists <see cref="System.Object"/>
		/// </returns>
		public static object PropertyExists (object aObject, string aPropertyName, bool aThrowException)
		{
			object res = PropertyExists (aObject, aPropertyName);
			if ((aThrowException == true) && (res == null))
				if ((IsValidType(aObject) == true) ||
				    (IsDataRowContainer(aObject) == true))
					throw new ExceptionDatabasePropertyNotFound (aPropertyName, aObject);
			return (res);
		}

		/// <summary>
		/// Sets value to DataRow object, if not it is simply redirected to
		/// ConnectionProvider
		/// </summary>
		/// <param name="aObject">
		/// Object (must be DataRow) <see cref="System.Object"/>
		/// </param>
		/// <param name="aPropertyName">
		/// Name of the property to be set value <see cref="System.String"/>
		/// </param>
		/// <param name="aValue">
		/// New value <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if succeeded, false if not <see cref="System.Boolean"/>
		/// </returns>
		public static bool SetValue (object aObject, string aPropertyName, object aValue)
		{
			if (aObject == null)
				return (true);
			try {
				if (aObject is DataRowView)
					if ((aObject as DataRowView).Row.RowState == DataRowState.Deleted)
						return (false);
					else if ((aObject as DataRow).RowState == DataRowState.Deleted)
						return (false);
				if (aObject is DataRowView)
					(aObject as DataRowView)[aPropertyName] = aValue;
				else
					(aObject as DataRow)[aPropertyName] = aValue;
				return (true);
			}
			catch {
				return (false);
			}
		}
		
		/// <summary>
		/// Sets value to DataRow object, if not it is simply redirected to
		/// ConnectionProvider
		/// </summary>
		/// <param name="aObject">
		/// Object (must be DataRow) <see cref="System.Object"/>
		/// </param>
		/// <param name="aProperty">
		/// Column used to describe data <see cref="System.Data.DataColumn"/>
		/// </param>
		/// <param name="aValue">
		/// New value <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if succeeded, false if not <see cref="System.Boolean"/>
		/// </returns>
		public static bool SetValue (object aObject, DataColumn aProperty, object aValue)
		{
			if (aObject == null)
				return (true);
			try {
				if (aObject is DataRowView)
					if ((aObject as DataRowView).Row.RowState == DataRowState.Deleted)
						return (false);
					else if ((aObject as DataRow).RowState == DataRowState.Deleted)
						return (false);
				if (aObject is DataRowView)
					(aObject as DataRowView)[aProperty.Ordinal] = aValue;
				else
					(aObject as DataRow)[aProperty.Ordinal] = aValue;
				return (true);
			}
			catch {
				return (false);
			}
		}
		
		/// <summary>
		/// Gets value from DataRow object, if not it is simply redirected to
		/// ConnectionProvider
		/// </summary>
		/// <param name="aObject">
		/// Object (must be DataRow) <see cref="System.Object"/>
		/// </param>
		/// <param name="aPropertyName">
		/// Name of the property to be set value <see cref="System.String"/>
		/// </param>
		/// <param name="aValue">
		/// New value <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if succeeded, false if not <see cref="System.Boolean"/>
		/// </returns>
		public static bool GetValue (object aObject, string aPropertyName, out object aValue)
		{
			return (GetValue (aObject, aPropertyName, out aValue, null));
		}
		
		/// <summary>
		/// Gets value from DataRow object, if not it is simply redirected to
		/// ConnectionProvider
		/// </summary>
		/// <param name="aObject">
		/// Object (must be DataRow) <see cref="System.Object"/>
		/// </param>
		/// <param name="aPropertyName">
		/// Name of the property to be set value <see cref="System.String"/>
		/// </param>
		/// <param name="aValue">
		/// New value <see cref="System.Object"/>
		/// </param>
		/// <param name="aDefaultValue">
		/// Default value to be set, but return will be false <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if succeeded, false if not <see cref="System.Boolean"/>
		/// </returns>
		public static bool GetValue (object aObject, string aPropertyName, out object aValue, object aDefaultValue)
		{
			aValue = aDefaultValue;
			if (aObject == null)
				return (true);
			try {
				if (aObject is DataRowView) {
					if ((aObject as DataRowView).Row.RowState == DataRowState.Deleted)
						aValue = (aObject as DataRowView).Row[aPropertyName, DataRowVersion.Original];
					else if ((aObject as DataRow).RowState == DataRowState.Deleted)
						aValue = (aObject as DataRow)[aPropertyName, DataRowVersion.Original];
					return (true);
				}
				if (aObject is DataRowView)
					aValue = (aObject as DataRowView)[aPropertyName];
				else
					aValue = (aObject as DataRow)[aPropertyName];
				return (true);
			}
			catch {
				aValue = aDefaultValue;
				return (false);
			}
		}
		
		/// <summary>
		/// Gets value from DataRow object, if not it is simply redirected to
		/// ConnectionProvider
		/// </summary>
		/// <param name="aObject">
		/// Object (must be DataRow) <see cref="System.Object"/>
		/// </param>
		/// <param name="aProperty">
		/// DataColumn of the property to be set value <see cref="System.Data.DataColumn"/>
		/// </param>
		/// <param name="aValue">
		/// New value <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if succeeded, false if not <see cref="System.Boolean"/>
		/// </returns>
		public static bool GetValue (object aObject, DataColumn aProperty, out object aValue)
		{
			return (GetValue (aObject, aProperty, out aValue, null));
		}
		
		/// <summary>
		/// Gets value from DataRow object, if not it is simply redirected to
		/// ConnectionProvider
		/// </summary>
		/// <param name="aObject">
		/// Object (must be DataRow) <see cref="System.Object"/>
		/// </param>
		/// <param name="aProperty">
		/// DataColumn of the property to be set value <see cref="System.Data.DataColumn"/>
		/// </param>
		/// <param name="aValue">
		/// New value <see cref="System.Object"/>
		/// </param>
		/// <param name="aDefaultValue">
		/// Default value to be set, but return will be false <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if succeeded, false if not <see cref="System.Boolean"/>
		/// </returns>
		public static bool GetValue (object aObject, DataColumn aProperty, out object aValue, object aDefaultValue)
		{
			aValue = aDefaultValue;
			if (aObject == null)
				return (true);
			try {
				DataRow row = null;
				if (aObject is DataRowView)
					row = (aObject as DataRowView).Row;
				else
					row = (aObject as DataRow);
				if (row.RowState == DataRowState.Deleted)
					aValue = row[aProperty.Ordinal, DataRowVersion.Original];
				else
					aValue = row[aProperty.Ordinal];
				row = null;
				return (true);
			}
			catch {
				aValue = aDefaultValue;
				return (false);
			}
		}
		
		/// <summary>
		/// Resolves adaptor target for specific object, used in ListAdaptor
		/// </summary>
		/// <param name="aObject">
		/// Object to check target for <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Returns final destination or null <see cref="System.Object"/>
		/// </returns>
		public static object ResolveTarget (object aObject)
		{
			if (aObject == null)
				return (null);
			if (IsDataRowContainer(aObject) == true) {
				return ((aObject as DataTable).Rows);
			}
			return (aObject);
		}
	}
}
