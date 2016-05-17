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
	/// Factory invokation arguments
	/// </summary>
	public class FactoryInvocationArgs : EventArgs
	{
		private PropertyDefinition state = PropertyDefinition.ReadWrite;
		/// <value>
		/// Specifies property state
		/// </value>
		public PropertyDefinition State {
			get { return (state); }
			set { state = value; }
		}
	
		private System.Type dataSourceType = null;
		/// <value>
		/// Specifies data source state
		/// </value>
		public System.Type DataSourceType {
			get { return (dataSourceType); }
			set { 
				if (dataSourceType == value)
					return;
				dataSourceType = value; 
				Resolve();
			}
		}
	
		private WeakReference dataSource = null;
		/// <value>
		/// Specifies data source
		/// </value>
		public object DataSource {
			get { 
				if (dataSource != null)
					return (dataSource.Target); 
				return (null);
			}
			set { 
				if (value == DataSource)
					return;
				if (value == null)
					dataSource = null;
				else
					dataSource = new WeakReference (value); 
				Resolve();
			}
		}
	
		private string[] filter = null;
		/// <value>
		/// Specifies invokation filter
		/// </value>
		public string[] Filter {
			get { return (filter); }
		}
		
		/// <summary>
		/// Adds filter into filter invocation queue
		/// </summary>
		/// <param name="aFilterName">
		/// Filter name <see cref="System.String"/>
		/// </param>
		public void AddFilter (string aFilterName)
		{
			if (aFilterName.Trim() == "")
				return;
			if (filter == null)
				filter = new string[1] { aFilterName.Trim() };
			else {
				string[] nf = new string[filter.Length+1];
				nf[0] = aFilterName.Trim();
				for (int i=0; i<filter.Length; i++) {
					nf[i+1] = filter[i];
					filter[i] = null;
				}
				filter = nf;
			}
		}
		
		private string propertyName = "";
		/// <value>
		/// Specifies property name
		/// </value>
		public string PropertyName {
			get { return (propertyName); }
			set {
				if (propertyName == value)
					return;
				propertyName = value;
				Resolve();
			}
		}

		private PropertyInfo propertyInfo = null;
		/// <value>
		/// Specifies property info if it exists
		/// </value>
		public PropertyInfo PropertyInfo {
			get { return (propertyInfo); }
		}

		private PropertyDescriptionAttribute description = null;
		/// <value>
		/// Set if property contains PropertyDescription attribute
		/// </value>
		public PropertyDescriptionAttribute Description {
			get { return (description); }
		}
		
		private PropertyRangeAttribute range = null;
		/// <value>
		/// Set if property contains PropertyRange attribute
		/// </value>
		public PropertyRangeAttribute Range {
			get { return (range); }
		}

		/// <value>
		/// Specifies if title should be resolved through Description
		/// </value>
		public bool ResolveTitle {
			get { return ((Title != "") && (Description != null)); }
		}
		
		private string title = "";
		/// <value>
		/// Custom title
		/// </value>
		public string Title {
			get { return (title); }
			set {
				if (title == value)
					return;
				title = value;
			}
		}
	
		/// <value>
		/// Returns default invokation handler for these parameters
		/// </value>
		[ToDo ("Use this in Widget factory to simplify invokation")]
		public string InvokationHandler {
			get {
				if (HandlerOverride.Trim() != "")
					return (HandlerOverride);
				if (Description != null) {
					if ((State == PropertyDefinition.ReadOnly) && (Description.ReadOnlyDataTypeHandler.Trim() != ""))
					    return (Description.ReadOnlyDataTypeHandler);
					else
						return (Description.DataTypeHandler);
				}
				return ("default");
			}
		}
		
		private string handlerOverride = "";
		/// <value>
		/// Specifies handler override if needed
		/// </value>
		public string HandlerOverride {
			get { return (handlerOverride); }
			set {
				if (handlerOverride == value)
					return;
				handlerOverride = value;
			}
		}
		
		/// <summary>
		/// Resolves property info, description and range
		/// </summary>
		private void Resolve()
		{
			if ((DataSourceType != null) && (PropertyName != "")) {
				if (TypeValidator.IsCompatible(DataSourceType, typeof(DataRow)) == true)
					throw new NotSupportedException ("DataRow is not supported");
				if (TypeValidator.IsCompatible(DataSourceType, typeof(IVirtualObject)) == true)
					throw new NotSupportedException ("IVirtualObject is not supported");
				propertyInfo = DataSourceType.GetProperty (PropertyName);
				if (State == PropertyDefinition.ReadWrite)
					if (propertyInfo != null)
						if (propertyInfo.CanWrite == false)
							State = PropertyDefinition.ReadOnly;
				if (propertyInfo != null) {
					description = propertyInfo.GetPropertyDescription();
					range = propertyInfo.GetPropertyRange();
				}
				else {
					description = null;
					range = null;
				}
				return;
			}
			propertyInfo = null;
			description = null;
			range = null;
		}

		public FactoryInvocationArgs (PropertyDefinition aState, System.Type aDataSourceType, string aPropertyName)
			: this (aState, aDataSourceType, aPropertyName, null)
		{
		}
		
		public FactoryInvocationArgs (PropertyDefinition aState, System.Type aDataSourceType, string aPropertyName, object aDataSource)
		{
			state = aState;
			DataSource = aDataSource;
			DataSourceType = aDataSourceType;
			PropertyName = aPropertyName;
		}
	}
}
