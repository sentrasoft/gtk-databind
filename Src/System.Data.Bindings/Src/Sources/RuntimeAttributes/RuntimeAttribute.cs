// RuntimeAttribute.cs - Field Attribute to assign additional information for Gtk#Databindings
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
	/// Runtime attribute base class.
	/// 
	/// Run time attributes can describe object more terrily than object
	/// was predicted during development
	/// </summary>
	public class RuntimeAttribute : IRuntimeAttribute
	{
		private WeakReference owner = new WeakReference (null);
		/// <value>
		/// Returns attribute owner
		/// </value>
		public object Owner {
			get { return (owner.Target); }
			set {
				if (owner.Target == value)
					return;
				owner.Target = value;
			}
		}
		
		/// <summary>
		/// Calls owners OnChange
		/// </summary>
		public void OnChange()
		{			
			if (Owner != null)
				if (Owner is IRuntimeAttributesObject)
					(Owner as IRuntimeAttributesObject).OnRuntimeAttributeChange (this);
		}

		/// <summary>
		/// Disconnects object
		/// </summary>
		public void Disconnect()
		{
			owner.Target = null;
			owner = null;
		}
		
		/// <summary>
		/// Creates runtime attribute 
		/// </summary>
		public RuntimeAttribute (object aOwner)
		{
			if (aOwner == null)
				throw new Exception ("Owner object has to be specified");
			owner.Target = aOwner;
		}
	}
}
