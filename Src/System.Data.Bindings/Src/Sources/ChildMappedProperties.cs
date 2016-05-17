// ChildMappedProperties.cs - ChildMappedProperties implementation for Gtk#Databindings
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
	/// List of mapped properties. Used in columns
	/// </summary>
	public class ChildMappedProperties
	{
		private object[] submappings = null;
		/// <summary>
		/// Itentifies submappings, valid for column mappings only
		/// </summary>
		public MappedProperty this [int aIdx] {
			get { 
				if ((submappings != null) && (aIdx > -1) && (aIdx < submappings.Length))
					return ((MappedProperty) submappings[aIdx]);
				else
					return (null);
			}
		}
		
		/// <summary>
		/// Returns number of submappings
		/// </summary>
		public int Count {
			get {
				if (submappings == null)
					return (0);
				return (submappings.Length);
			}
		}

		private int bpos = 0;
		private int size = 0;
		/// <summary>
		///
		/// </summary>
		public int Size {
			get { return (size); }
			set { 
				if (submappings != null)
					throw new Exception ("Can't define size after start of addition");
				size = value; 
				submappings = new object [size];
			} 
		}

		/// <summary>
		/// Adds submapping to the original mapping, accessible trough Submappings
		/// </summary>
		public void AddMapping (MappedProperty aProp)
		{
			if (Size == 0) {
				object[] nsub;
				if (submappings == null) {
					nsub = new object [1];
				}
				else {
					nsub = new object [submappings.Length];
					submappings.CopyTo(nsub, 0);
				}
				nsub[nsub.Length-1] = aProp;
				FastDisconnect();
				submappings = nsub;
			}
			else {
				submappings[bpos] = aProp;
				bpos++;
			}
		}

		/// <summary>
		/// Removes submapping from the original mapping, accessible trough Submappings
		/// </summary>
		public void RemoveMapping (MappedProperty aProp)
		{
			if ((aProp == null) || (submappings == null))
				return;
			int pos = -1;
			bool exists = false;
			for (int i=0; i<submappings.Length; i++)
				if (submappings[i] == aProp) {
					exists = true;
					pos = i;
					break;
				}
			if (exists == true) {
				if (submappings.Length == 1) {
					submappings[0] = null;
					submappings = null;
					return;
				}
				
				object[] nsub = new object [submappings.Length];
				int j = 0;
				for (int i=0; i<submappings.Length; i++)
					if (i != pos) {
						nsub[j] = submappings[i];
						j++;
					}
				(submappings[pos] as MappedProperty).Disconnect();
				FastDisconnect();
				submappings = nsub;
			}
		}
		
		/// <summary>
		/// Disconnects everything to simplify buffer swap
		/// </summary>
		public void FastDisconnect()
		{
			if (submappings != null)
				for (int i=0; i<submappings.Length; i++)
					submappings[i] = null;
			submappings = null;
			bpos = 0;
		}
		
		/// <summary>
		/// Disconnects everything to speed up GC process
		/// </summary>
		public void Disconnect()
		{
			if (submappings != null)
				for (int i=0; i<submappings.Length; i++)
					(submappings[i] as MappedProperty).Disconnect();
			FastDisconnect();
		}
		
		/// <summary>
		/// Creates ChildMappedProperties
		/// </summary>
		public ChildMappedProperties()
		{
		}
		
		/// <summary>
		/// Destroys ChildMappedProperties and calls Disconnect()
		/// </summary>
		~ChildMappedProperties()
		{
			Disconnect();
		}
	}
}
