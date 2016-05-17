// AttrObject.cs - Field Attribute to assign additional information for Gtk#Databindings
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

namespace System.Data.Bindings.Runtime
{
	/// <summary>
	/// Provides object class which can be described with Attributes 
	/// during runtime
	/// </summary>
	public class RuntimeAttributesObject : IRuntimeAttributesObject
	{
		private RuntimeAttributeList runtimeAttributes = null;
		/// <value>
		/// Specifies list of runtime attributes
		/// </value>
		internal RuntimeAttributeList RuntimeAttributes {
			get { 
				if (runtimeAttributes == null)
					runtimeAttributes = new RuntimeAttributeList();
				return (runtimeAttributes); 
			}
		}
		
		/// <summary>
		/// Method called on attribute change
		/// </summary>
		/// <param name="aAttr">
		/// A <see cref="IRuntimeAttribute"/>
		/// </param>
		public virtual void OnRuntimeAttributeChange (IRuntimeAttribute aAttr)
		{
		}
		
		/// <summary>
		/// Adds new attribute to list
		/// </summary>
		/// <param name="aAttr">
		/// Attribute to be added <see cref="Attribute"/>
		/// </param>
		public void AddRuntimeAttribute (Attribute aAttr)
		{
			if (aAttr == null)
				return;
			
		}
		
		/// <summary>
		/// Removes attribute from the list
		/// </summary>
		/// <param name="aAttr">
		/// Attribute to be removed <see cref="Attribute"/>
		/// </param>
		public void RemoveRuntimeAttribute (IRuntimeAttribute aAttr)
		{
			if ((aAttr == null) || (runtimeAttributes == null))
				return;
		}
		
		/// <summary>
		/// Returns list of runtime attributes that conform with specified type
		/// </summary>
		/// <param name="aAttrType">
		/// Type searched <see cref="System.Type"/>
		/// </param>
		/// <returns>
		/// List of resulting attributes <see cref="IRuntimeAttribute"/>
		/// </returns>
		public IRuntimeAttribute[] GetRuntimeAttribute (System.Type aAttrType)
		{
			if ((aAttrType == null) || (runtimeAttributes == null))
				return (null);
			if (TypeValidator.IsCompatible(aAttrType, typeof(IRuntimeAttribute)) == false)
				throw new Exception ("Runtime attribute has to be derived from IRuntimeAttribute");
			return (null);
		}

		/// <summary>
		/// Disconnects
		/// </summary>
		public void Disconnect()
		{
		}
		
		/// <summary>
		/// Creates RuntimeAttributesObject
		/// </summary>
		public RuntimeAttributesObject()
		{
		}
	}
}
