// CacheKeyList.cs - Field Attribute to assign additional information for Gtk#Databindings
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

namespace System.Data.Bindings.Collections
{
	/// <summary>
	/// Array list used by cache list to specify different sort of
	/// data. Always control with master list
	/// </summary>
	/// <remarks>
	/// Not even nearly optimized. Optimization has to be done
	/// </remarks>
	public class CacheKeyList : ObserveableArrayList
	{
		public class KeyCompare : IComparer
		{
			string keyName;
			WeakReference master = null; 
			
			public int Compare (object x, object y)
			{
				return (((CacheList) master.Target).CompareData (keyName, x, y));
			}
			
			public KeyCompare (CacheList aMaster, string aKeyName)
			{
				master = new WeakReference (aMaster);
				keyName = aKeyName;
			}
		}
		
		private CacheList master = null;
		/// <summary>
		/// Master list of this key
		/// </summary>
		public CacheList Master {
			get { return (master); }
			set {
				if (value != null)
					throw new Exception ("You can only assign null manually");
				master = null; 
			}
		}
		
		private string keyName = "";
		/// <summary>
		/// Name of property which is used as key
		/// </summary>
		public string KeyName {
			get { return (keyName); }
		}
		
		/// <summary>
		/// Searches for object by masters KeyProperty, since it is unique
		/// </summary>
		/// <param name="aObject">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// </returns>
		public int FindObject (object aObject)
		{
			for (int i=0; i<Count; i++) {
				object b = this[i];
				int r = master.CompareData (master.KeyProperty, aObject, b);
				if (r == 0) {
					return (i);
				}
			}
			return (-1);
		}

		/// <summary>
		/// Updates list with sorting
		/// </summary>
		public void UpdateKey()
		{
			if (master.IsFrozen == true)
				return;
			Clear();
			ArrayList lst = new ArrayList();
			foreach (object a in master)
				lst.Add (a);
			KeyCompare kc = new KeyCompare (master, KeyName);
			lst.Sort (kc);
			kc = null;
			foreach (object a in lst)
				Add (a);
			lst.Clear();
			lst = null;
/*			foreach (object a in master) {
				bool found = false;
				for (int i=0; i<Count; i++) {
					object b = this[i];
					int r = master.CompareData (KeyName, a, b);
					if (r <= 0) {
						found = true;
						Insert (i, a);
						break;
					}
				}
				if (found == false)
					Add (a);
			}*/
		}

		/// <summary>
		/// Inserts object into list
		/// </summary>
		/// <param name="aObject">
		/// Object to be inserted in list <see cref="System.Object"/>
		/// </param>
		internal void KeyInsertObject (object aObject)
		{
			if (FindObject(aObject) > -1) {
				KeyUpdateObject (aObject);
				return;
			}
			bool found = false;
			for (int i=0; i<Count; i++) {
				object b = this[i];
				int r = master.CompareData (KeyName, aObject, b);
				if (r <= 0) {
					found = true;
					Insert (i, aObject);
					break;
				}
			}
			if (found == false)
				Add (aObject);
		}

		/// <summary>
		/// Adds new object to cache, but if object with key value already
		/// exists, then instead of addition modification happens
		/// </summary>
		public void AddCacheObject (object aObject)
		{
			Master.AddCacheObject (aObject);
		}
		
		/// <summary>
		/// Checks if that object already exists, since object main key
		/// has to be unique, that is how it is checked
		/// </summary>
		/// <param name="aObject">
		/// Object to check against <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Object with that key in list <see cref="System.Object"/>
		/// </returns>
		public object CacheObjectExists (object aObject)
		{
			return (Master.CacheObjectExists (aObject));
		}
		
		/// <summary>
		/// Updates object in list
		/// </summary>
		internal void KeyUpdateObject (object aObject)
		{
			// Find object by master key and check if his position is still correct
			int pos = FindObject(aObject);
			if (pos == -1) {
				KeyInsertObject (aObject);
				return;
			}
			bool correctposition = true;
			if (pos > 0) {
				int r = master.CompareData (KeyName, aObject, this[pos-1]);
				// Safely exit if value is equal than previous
				if (r == 0)
					return;
				if (r == 1)
					correctposition = false;
			}
			if (correctposition == true)
				if (pos < (Count - 1)) {
					int r = master.CompareData (KeyName, aObject, this[pos+1]);
					// Safely exit if value is equal than previous
					if (r == 0)
						return;
					if (r == -1)
						correctposition = false;
				}
			// if position is not correct, remove old
			RemoveAt (pos);
			KeyInsertObject (aObject);
		}

		public CacheKeyList (CacheList aMaster, string aKeyName)
			: base()
		{
			if (aMaster == null)
				throw new Exception ("Trying to create CacheKeyList with null master");
			master = aMaster;
			if (master.IsValidField(aKeyName) == false)
				throw new Exception ("Trying to assign invalid key: " + aKeyName);
			keyName = aKeyName;
		}
	}
}
