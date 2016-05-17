// ValueList.cs - ValueList implementation for Gtk#Databindings
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
using System.Collections;

namespace System.Data.Bindings
{
	/// <summary>
	/// ValueList provides enumeration of values and access by index and name to 
	/// MappedProperties
	/// </summary>
	public class ValueList : IEnumerable
	{
		private WeakReference adaptor = null;
		/// <summary>
		/// WeakReference to Adaptor owning this ValueList
		/// </summary>
		public IAdaptor Adaptor {
			get {
				if (adaptor == null)
					throw new ExceptionValueListAdaptorIsNull();
				return ((IAdaptor) adaptor.Target);
			}
		}
		
		/// <summary>
		/// Returns MappedProperty by Index
		/// </summary>
		public MappedProperty this [int aIdx] {
			get { return (Adaptor.Mapping(aIdx)); }
		}
		
		/// <summary>
		/// Returns MappedProperty by Name
		/// </summary>
		public MappedProperty this [string aName] {
			get { return (Adaptor.MappingByName(aName)); }
		}
		
		/// <summary>
		/// Basic enumeration needs
		/// </summary>
		public class MappedPropertyEnumerator : IEnumerator
		{
			private int idx = -1;
			private IAdaptor master = null;

			/// <summary>
			/// Returns current object 
			/// </summary>
			public object Current {
				get { return ((idx != -1) ? master.Values[idx] : null); }
			}
			
			/// <summary>
			/// Resets enumerator 
			/// </summary>
			public void Reset()
			{
				idx = -1;
			}
			
			/// <summary>
			/// Returns next object 
			/// </summary>
			/// <returns>
			/// true if successful
			/// </returns>
			public bool MoveNext()
			{
				idx++;
				return ((idx < master.MappingCount) ? true : false); 
			}

			/// <summary>
			/// Creates MappedPropertyEnumerator
			/// </summary>
			/// <returns>
			/// Adaptor owner <see cref="IAdaptor"/>
			/// </returns>
			public MappedPropertyEnumerator (IAdaptor aMaster)
			{
				master = aMaster;
			}
		}

		/// <summary>
		/// Creates enumerator instance
		/// </summary>
		/// <returns>
		/// Enumerator instance <see cref="IEnumerator"/>
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return (new MappedPropertyEnumerator (Adaptor));
		}
		
		/// <summary>
		/// Creates enumerator instance
		/// </summary>
		/// <returns>
		/// Enumerator instance <see cref="IEnumerator"/>
		/// </returns>
		public IEnumerator GetEnumerator()
		{
			return (new MappedPropertyEnumerator (Adaptor));
		}
		
		/// <value>
		/// Returns number of items
		/// </value>
		public int Count {
			get { return (Adaptor.MappingCount); }
		}
		
		/// <summary>
		/// Checks if MappedProperty exists
		/// </summary>
		/// <param name="aName">
		/// Name of mapped property <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		public bool Exists (string aName)
		{
			return (Adaptor.Exists (aName));
		}
		
		/// <summary>
		/// Disconnects ValueList and releases everything for GC to step in
		/// </summary>
		public void Disconnect()
		{
			foreach (MappedProperty mp in this)
				mp.Disconnect();
				
			if (adaptor != null)
				adaptor.Target = null;
			adaptor = null;
		}

		/// <summary>
		/// Creates ValueList
		/// </summary>
		protected ValueList()
		{
		}
		
		/// <summary>
		/// Creates ValueList
		/// </summary>
		/// <param name="aAdaptor">
		/// Adaptor owner <see cref="IAdaptor"/>
		/// </param>
		public ValueList (IAdaptor aAdaptor)
		{
			if (aAdaptor == null)
				throw new ExceptionValueListAdaptorIsNull();
			adaptor = new WeakReference (aAdaptor);
		}

		/// <summary>
		/// Disconnects and destroys ValueList 
		/// </summary>
		~ValueList()
		{
			Disconnect();
		}
	}
}
