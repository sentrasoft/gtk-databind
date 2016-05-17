// VirtualProperty.cs - VirtualProperty implementation for Gtk#Databindings
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
	/// Stores actual data for VirtualProperty. All the work is done trough reflection
	/// </summary>
	public class VirtualProperty
	{
		private WeakReference master = new WeakReference (null);
		
		private string name = "";
		/// <summary>
		/// Name of this property 
		/// </summary>
		public string Name {
			get { return (name); }
		}
		
		private object propertyValue;
		/// <summary>
		/// Value represented by this property
		/// </summary>
		public object Value {
			get { return (propertyValue); }
			set {
				if (value != null)
					if (TypeValidator.IsCompatible(value.GetType(), propertyType) == false)
						throw new ExceptionWrongTypeValueSetToVirtualProperty (propertyType, value.GetType());

				try {
					// Check first if conversion is not needed
					if ((value.GetType() == propertyType) || (value.GetType().IsSubclassOf(propertyType))) {
						propertyValue = value;
						if (onChange != null)
							onChange ((IVirtualObject) master.Target, this);
					}
					else {
						// Try converting
						object no = System.Convert.ChangeType (value, PropertyType);
						if (no is IComparable) {
							if ((propertyValue != null) && ((propertyValue as IComparable).CompareTo(no) != 0)) {
								propertyValue = no;
								if (onChange != null)
									onChange ((IVirtualObject) master.Target, this);
							}
							else
								if ((value != null) && (propertyValue == null)) {
									propertyValue = value;
									if (onChange != null)
										onChange ((IVirtualObject) master.Target, this);
								}
						}
						else {
							propertyValue = value;
							if (onChange != null)
								onChange ((IVirtualObject) master.Target, this);
						}
						no = null;
					}
				}
				catch {
					throw new ExceptionErrorAssigningValueToVirtualProperty (propertyType, value.GetType());
				}
			}
		}
		
		private System.Type propertyType;
		/// <summary>
		/// Type this property can accept
		/// </summary>
		public System.Type PropertyType {
			get { return (propertyType); }
		}

		private event EventOnPropertyChange onChange;
		/// <summary>
		/// Events to be delegated when this property changes
		/// </summary>
		public event EventOnPropertyChange OnChange {
			add { onChange += value; }
			remove { onChange -= value; }
		}
		
		/// <summary>
		/// Disconnects all data so GC can be more reliable
		/// </summary>
		public void Disconnect()
		{
			if (master.Target != null)
				onChange -= (master.Target as IVirtualObject).Changed;
			master = null;
			name = "";
			propertyValue = null;
			propertyType = null;
			onChange = null;
		}

		/// <summary>
		/// Creates VirtualProperty
		/// </summary>
		/// <param name="a_Master">
		/// Master object <see cref="IVirtualObject"/>
		/// </param>
		/// <param name="a_Name">
		/// Property name <see cref="System.String"/>
		/// </param>
		/// <param name="a_PropertyType">
		/// Type of value <see cref="System.Type"/>
		/// </param>
		public VirtualProperty (IVirtualObject a_Master, string a_Name, System.Type a_PropertyType)
		{
			master.Target = a_Master;
			onChange += a_Master.Changed;
			name = a_Name;
			propertyType = a_PropertyType;
			propertyValue = null;
			onChange = null;
		}
		
		/// <summary>
		/// Creates VirtualProperty
		/// </summary>
		/// <param name="a_Master">
		/// Master object <see cref="IVirtualObject"/>
		/// </param>
		/// <param name="a_Name">
		/// Property name <see cref="System.String"/>
		/// </param>
		/// <param name="a_PropertyType">
		/// Type of value <see cref="System.Type"/>
		/// </param>
		/// <param name="a_Value">
		/// Property value <see cref="System.Object"/>
		/// </param>
		public VirtualProperty (IVirtualObject a_Master, string a_Name, System.Type a_PropertyType, object a_Value)
		{
			master.Target = a_Master;
			onChange += a_Master.Changed;
			name = a_Name;
			propertyType = a_PropertyType;
			propertyValue = a_Value;
			onChange = null;
		}

		/// <summary>
		/// Disconnects and destroys VirtualProperty
		/// </summary>
		~VirtualProperty()
		{
			Disconnect();
		}
	}
}
