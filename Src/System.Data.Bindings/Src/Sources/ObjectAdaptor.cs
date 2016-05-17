// ObjectAdaptor.cs - Field Attribute to assign additional information for Gtk#Databindings
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
//
//

using System;
using System.Data.Bindings;
using System.Data.Bindings.DebugInformation;

namespace System.Data.Bindings
{
	/// <summary>
	/// Provides DataBindings ControlAdaptor functionality to simpler objects
	/// </summary>
	public class ObjectAdaptor : ControlAdaptor
	{
		/// <summary>
		/// Validates type of the object if it is compatible with adaptor
		/// </summary>
		/// <param name="aObject">
		/// Object to validate type for <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if object is right type, false if not <see cref="System.Boolean"/>
		/// </returns>
		/// <remarks>
		/// Returns true by default, false if object is null
		/// </remarks>
		public override bool ValidateControlType (object aObject)
		{
			return (aObject != null);
		}

		/// <summary>
		/// Checks if object is master parent (Window like) type
		/// </summary>
		/// <param name="aObject">
		/// Object to check for <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if object is master parent (like window), false if not <see cref="System.Boolean"/>
		/// </returns>
		public override bool ControlIsWindow (object aObject)
		{
			return (false);
		}
		
		/// <summary>
		/// Resolves parent object
		/// </summary>
		/// <param name="aObject">
		/// Object whos parent should be resolved <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Parent object, null if parent does not exists <see cref="System.Object"/>
		/// </returns>
		/// <remarks>
		/// Returns null by default
		/// </remarks>
		public override object GetParentOfControl (object aObject)
		{
			return (null);
		}
		
		/// <summary>
		/// Resolves parent object
		/// </summary>
		/// <param name="aControl">
		/// Object whos parent should be resolved <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Parent object, null if parent does not exists <see cref="System.Object"/>
		/// </returns>
		/// <remarks>
		/// Returns null by default
		/// </remarks>
		public  object GetLowestParentOfControl (object aControl)
		{
			return (null);
		}
		
		/// <summary>
		/// Returns ParentWindow (which translates to parent object) for the specified object
		/// </summary>
		/// <param name="aObject">
		/// Object whose parent we need <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// reference to parent object <see cref="System.Object"/>
		/// </returns>
		/// <remarks>
		/// Returns null
		/// </remarks>
		public override object ParentWindow (object aObject)
		{
			return (null);
		}
		
		/// <summary>
		/// Checks if control is Box (which translates to something like list) type.
		/// </summary>
		/// <param name="aObject">
		/// Object to check for <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if control is window, false if not <see cref="System.Boolean"/>
		/// </returns>
		/// <remarks>
		/// Returns false by default
		/// </remarks>
		public override bool ControlIsContainer (object aObject)
		{
			return (false);
		}
		
		/// <summary>
		/// Checks if boundary adaptor is allowed
		/// </summary>
		/// <returns>
		/// true if allowed, false if not <see cref="System.Boolean"/>
		/// </returns>
		/// <remarks>
		/// Returns true by default
		/// </remarks>
		public override bool IsBoundaryAdaptorAllowed()
		{
			return (false);
		}
		
		/// <summary>
		/// Creates ObjectAdaptor
		/// </summary>
		protected ObjectAdaptor() 
			: base ()
		{
		}

		/// <summary>
		/// Creates ObjectAdaptor
		/// </summary>
		/// <param name="aObject">
		/// Object where this Adaptor is connected to <see cref="System.Object"/>
		/// </param>
		/// <param name="aSingleMapping">
		/// Adaptor allows single mapping <see cref="System.Boolean"/>
		/// </param>
		public ObjectAdaptor (object aObject, bool aSingleMapping)
			: base (aObject, aSingleMapping)
		{
		}

		/// <summary>
		/// Creates ObjectAdaptor
		/// </summary>
		/// <param name="aObject">
		/// Object where this Adaptor is connected to <see cref="System.Object"/>
		/// </param>
		/// <param name="aSingleMapping">
		/// Adaptor allows single mapping <see cref="System.Boolean"/>
		/// </param>
		/// <param name="aMappings">
		/// Mappings string <see cref="System.String"/>
		/// </param>
		public ObjectAdaptor (object aObject, bool aSingleMapping, string aMappings)
			: base (aObject, aSingleMapping, aMappings)
		{
		}
		
		/// <summary>
		/// Creates ObjectAdaptor
		/// </summary>
		/// <param name="aObject">
		/// Object where this Adaptor is connected to <see cref="System.Object"/>
		/// </param>
		/// <param name="aSingleMapping">
		/// Adaptor allows single mapping <see cref="System.Boolean"/>
		/// </param>
		/// <param name="aDataSource">
		/// DataSource connected to this Adaptor <see cref="System.Object"/>
		/// </param>
		/// <param name="aMappings">
		/// Mappings string <see cref="System.String"/>
		/// </param>
		public ObjectAdaptor (object aObject, bool aSingleMapping, object aDataSource, string aMappings)
			: base (aObject, aSingleMapping, aDataSource, aMappings)
		{
		}
		
		/// <summary>
		/// Creates ObjectAdaptor
		/// </summary>
		/// <param name="aObject">
		/// Object where this Adaptor is connected to <see cref="System.Object"/>
		/// </param>
		/// <param name="aAdaptor">
		/// Custom created Adaptor to be connected with this one <see cref="IAdaptor"/>
		/// </param>
		public ObjectAdaptor (object aObject, IAdaptor aAdaptor)
			: base (aObject, aAdaptor, null)
		{
		}

		/// <summary>
		/// Creates ObjectAdaptor
		/// </summary>
		/// <param name="aObject">
		/// Object where this Adaptor is connected to <see cref="System.Object"/>
		/// </param>
		/// <param name="aAdaptor">
		/// Custom created Adaptor to be connected with this one <see cref="IAdaptor"/>
		/// </param>
		/// <param name="aDataSource">
		/// DataSource connected to this Adaptor <see cref="System.Object"/>
		/// </param>
		/// <param name="aMappings">
		/// Mappings string <see cref="System.String"/>
		/// </param>
		public ObjectAdaptor (object aObject, IAdaptor aAdaptor, object aDataSource, string aMappings)
			: base (aObject, aAdaptor, null, aDataSource, aMappings)
		{
		}
	}
}
