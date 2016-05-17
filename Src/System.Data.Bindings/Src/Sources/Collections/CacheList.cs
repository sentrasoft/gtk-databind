// CacheList.cs - Field Attribute to assign additional information for Gtk#Databindings
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
using System.Text;
using System.Collections;
using System.Reflection;
using System.Data.Bindings.Utilities;
using System.Xml;

namespace System.Data.Bindings.Collections
{
	/// <summary>
	/// Array list that contains cache elements, every element has unique
	/// key by which it is accessed. Other properties can e used to resort
	/// </summary>
	public class CacheList : ObserveableArrayList, IFileCache
	{
		private ArrayList propList = new ArrayList();
		private ArrayList keys = new ArrayList();

		private PropertyInfo keyinfo;
		
		//// <value>
		/// Returns full path for this cache file
		/// </value>
		public string CachePath {
			get { 
				return (ApplicationPreferences.GetMasterFolder().Path + 
				        System.IO.Path.DirectorySeparatorChar + FileName);
			}
		}
		
		private string fileName = "";
		/// <value>
		/// Cache file name
		/// </value>
		public string FileName {
			get { return (fileName); }
		}

		private string keyProperty = "";
		/// <value>
		/// Master key property, only uniqued objects can be added to this list.
		/// If object is added and its key already exists, then instead of addition
		/// modification starts
		/// </value>
		public string KeyProperty {
			get { return (keyProperty); }
		}

		private System.Type cacheType = null;
		/// <value>
		/// Type of data inside cache list
		/// </value>
		public System.Type CacheType { 
			get { return (cacheType); }
		}

		private int freezeCount = 0;
		private bool isFrozen {
			get { return (freezeCount > 0); }
		}

		/// <summary>
		/// Freezes list. List doesn't update sub keys on edit
		/// </summary>
		public void Freeze()
		{
			freezeCount++;
		}

		/// <summary>
		/// Unfreezes list. List doesn't update sub keys on edit
		/// </summary>
		public void Unfreeze()
		{
			if (freezeCount <= 0)
				return;
			freezeCount--;
			if (freezeCount == 0)
				UpdateKeys();
		}

		/// <summary>
		/// Enforces key lists update
		/// </summary>
		public void UpdateKeys()
		{
			if (freezeCount > 0)
				return;
			foreach (CacheKeyList l in keys)
				l.UpdateKey();
		}

		/// <summary>
		/// Adds key to this cache
		/// </summary>
		public void AddKey (string aKeyName)
		{
			if (aKeyName == KeyProperty)
				return;
			foreach (CacheKeyList key in keys)
				if (key.KeyName == aKeyName)
					return;
			CacheKeyList nl = new CacheKeyList (this, aKeyName);
			nl.UpdateKey();
			keys.Add (nl);
		}

		/// <summary>
		/// Resolves list which contains specified key sorted list
		/// </summary>
		/// <param name="aName">
		/// Name of property for key <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// List containing that key or null <see cref="CacheKeyList"/>
		/// </returns>
		public CacheKeyList GetKey(string aName) 
		{
			foreach (CacheKeyList l in keys)
				if (l.KeyName == aName)
					return (l);
			throw new Exception ("Accessing invalid cache key: " + aName); 
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
			if (aObject == null)
				return (null);
			
			if (aObject.GetType() != CacheType)
				throw new Exception ("Searching for wrong object type in CacheList");
			
//			int i = FindIndexFor (aObject);
			foreach (object o in this) {
				if (CompareData(aObject, o) == 0)
					return (o);
			}
			return (null);
		}

		private object CacheObjectExists (object aObject, int aIndex)
		{
			if (CompareData(aObject, this[aIndex]) == 0)
				return (this[aIndex]);
			return (null);
		}

		private int LFindIndexFor (object aObject)
		{
			int i = 0;
			foreach (object o in this) {
				if (CompareData(aObject, o) == 0)
					return (i);
				else 
					i++;
			}
			return (i);
		}
		
		private int FindIndexFor (object aObject, int aDiv, int aIndex)
		{
			int newdiv = aDiv / 2;
			switch (CompareData(aObject, this[aIndex])) {
			case 0:
				return (aIndex);
				break;
			case 1:
				if (newdiv != 0)
					return (FindIndexFor (aObject, newdiv, aIndex + aDiv));
				else
					return (aIndex + 1);
				break;
			case -1:
				if (newdiv != 0)
					return (FindIndexFor (aObject, newdiv, aIndex + aDiv));
				else
					return (aIndex + 1);
				break;
			}
			return (-2);
		}
		
		private int FindIndexFor (object aObject)
		{
			if (aObject == null)
				return (-1);
			
			if (aObject.GetType() != CacheType)
				throw new Exception ("Searching for wrong object type in CacheList");

			if (Count < 10)
				return (LFindIndexFor (aObject));
			int div = Count / 2;
			return (FindIndexFor (aObject, div / 2, div - 1));
		}
		
		/// <summary>
		/// Adds new object to cache, but if object with key value already
		/// exists, then instead of addition modification happens
		/// </summary>
		public void AddCacheObject (object aObject)
		{
			if (aObject == null)
				return;
			if (aObject.GetType() != CacheType)
				throw new Exception ("Wrong type in cache list");
//			int pos = FindIndexFor (aObject);
			
			object o = null;
//			if ((pos >= 0) && (CompareData(aObject, this[pos]) == 0)) {
//				System.Console.WriteLine("Optimized found");
//				o = this[pos];
//			}
//			else
			o = this.CacheObjectExists (aObject);
			if (o == null) {
				for (int i=0; i<Count; i++) {
					if (CompareData(aObject, this[i]) < 0) {
						Insert (i, aObject);
//						System.Console.WriteLine("i=" + i + " pos=" + pos);
						return;
					}
				}
//				System.Console.WriteLine("pos=" + pos);
				Add (aObject);
				if (IsFrozen == false)
					foreach (CacheKeyList key in keys)
						key.KeyInsertObject (aObject);
			}
			else {
				CopyData (aObject, o);
				if (IsFrozen == false)
					foreach (CacheKeyList key in keys)
						key.KeyUpdateObject (aObject);
			}
		}

		/// <summary>
		/// Checks if property name is valid for object type or not
		/// </summary>
		/// <param name="aField">
		/// Property name <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// true if valid, false if not <see cref="System.Boolean"/>
		/// </returns>
		public bool IsValidField (string aField)
		{
			foreach (PropertyInfo i in propList)
				if (aField == i.Name)
					return (true);
			return (false);
		}

		/// <summary>
		/// Compares int data
		/// </summary>
		/// <param name="a">
		/// First value <see cref="System.Int32"/>
		/// </param>
		/// <param name="b">
		/// Second value <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// -1 if first is lower then second
		/// 0 if equal
		/// 1 if first is higher than second <see cref="System.Int32"/>
		/// </returns>
		public static int CompareData (int a, int b)
		{
			if (a < b)
				return (-1);
			if (a == b)
				return (0);
			return (1);
		}
		
		public static int CompareData (DateTime a, DateTime b)
		{
			if (a < b)
				return (-1);
			if (a == b)
				return (0);
			return (1);
		}
		
		public static int CompareData (string a, string b)
		{
			return (a.CompareTo(b));
/*			StringBuilder bld = new StringBuilder(a.ToUpper());
			bld.Replace ("Č", "Cxx");
//			bld.Replace ("č", "cxx");
			bld.Replace ("Š", "Sxx");
//			bld.Replace ("š", "sxx");
			bld.Replace ("Ž", "Zxx");
//			bld.Replace ("ž", "zxx");
			a = bld.ToString();
			bld.Remove (0, bld.Length);
			bld.Append (b.ToUpper());
			bld.Replace ("Č", "Cxx");
//			bld.Replace ("č", "cxx");
			bld.Replace ("Š", "Sxx");
//			bld.Replace ("š", "sxx");
			bld.Replace ("Ž", "Zxx");
//			bld.Replace ("ž", "zxx");
			b = bld.ToString();
			bld = null;
			return (String.Compare(a, b));*/
		}
		
		public int CompareData (string aField, object aComparer, object aObject)
		{
			PropertyInfo info = CacheType.GetProperty (aField);
			if (aField == null)
				throw new Exception ("Trying to sort with non existant property");
			if ((aObject == null) || (aComparer == null))
				throw new Exception ("Trying to sort with null object:\n" +
				                     "  Comparer: " + aComparer + "\n" +
				                     "  Object: " + aObject);
			if ((aObject.GetType() != CacheType) || (aComparer.GetType() != CacheType))
				throw new Exception ("Trying to sort with wrong object");

			if (info.PropertyType is IComparable)
				return ((info.GetValue (aComparer, null) as IComparable).CompareTo (info.GetValue (aObject, null)));
			
			if ((info.PropertyType == typeof(int)) || (info.PropertyType == typeof(System.Int32))) {
				int a = (int) info.GetValue (aComparer, null);
				int b = (int) info.GetValue (aObject, null);
				return (CacheList.CompareData (a, b)); 
			}
			if ((info.PropertyType is string) || (info.PropertyType == typeof(System.String))) {
				string a = (string) info.GetValue (aComparer, null);
				string b = (string) info.GetValue (aObject, null);
				return (CacheList.CompareData (a, b));
			}
			if (info.PropertyType is DateTime) {
				DateTime a = (DateTime) info.GetValue (aComparer, null);
				DateTime b = (DateTime) info.GetValue (aObject, null);
				return (CacheList.CompareData (a, b));
			}
			throw new Exception ("Wrong type comparision: " + info.PropertyType);
		}
		
		private int CompareKeyData (object aComparer, object aObject)
		{
			if ((aObject == null) || (aComparer == null))
				throw new Exception ("Trying to sort with null object:\n" +
				                     "  Comparer: " + aComparer + "\n" +
				                     "  Object: " + aObject);
			if ((aObject.GetType() != CacheType) || (aComparer.GetType() != CacheType))
				throw new Exception ("Trying to sort with wrong object");

			if (keyinfo.PropertyType is IComparable)
				return ((keyinfo.GetValue (aComparer, null) as IComparable).CompareTo (keyinfo.GetValue (aObject, null)));
			
			if ((keyinfo.PropertyType == typeof(int)) || (keyinfo.PropertyType == typeof(System.Int32))) {
				int a = (int) keyinfo.GetValue (aComparer, null);
				int b = (int) keyinfo.GetValue (aObject, null);
				return (CacheList.CompareData (a, b)); 
			}
			if ((keyinfo.PropertyType is string) || (keyinfo.PropertyType == typeof(System.String))) {
				string a = (string) keyinfo.GetValue (aComparer, null);
				string b = (string) keyinfo.GetValue (aObject, null);
				return (CacheList.CompareData (a, b));
			}
			if (keyinfo.PropertyType is DateTime) {
				DateTime a = (DateTime) keyinfo.GetValue (aComparer, null);
				DateTime b = (DateTime) keyinfo.GetValue (aObject, null);
				return (CacheList.CompareData (a, b));
			}
			throw new Exception ("Wrong type comparision: " + keyinfo.PropertyType);
		}
		
		public int CompareData (object aComparer, object aObject)
		{
			return (CompareKeyData (aComparer, aObject));
		}
		
		public void StoreCache()
		{
			XmlDocument doc = new XmlDocument();
			XmlElement root = doc.CreateElement("Cache");
			doc.AppendChild (root);
			XmlAttribute attr = null;
			foreach (object o in this) {
				XmlElement childNode = doc.CreateElement("CacheObject");
				root.AppendChild(childNode);
				foreach (PropertyInfo info in propList) { 
					attr = doc.CreateAttribute (info.Name);
					attr.Value = info.GetValue(o, null).ToString();
					childNode.Attributes.Append (attr);
				}
				childNode = null;
			}
			System.Console.WriteLine(CachePath);
			doc.Save (CachePath);
			attr = null;
			root = null;
			doc = null;
		}

		public void CleanUpCache()
		{
			Clear();
			foreach (CacheKeyList l in keys)
				l.Clear();
			if (System.IO.File.Exists(CachePath) == true)
				System.IO.File.Delete (CachePath);
		}
		
		public void LoadCache()
		{
//System.Console.WriteLine("Loading cache from: " + FileName);
			PropertyInfo info;
			object o;
			
			int i=0;
			if (System.IO.File.Exists(CachePath) == true) {
				XmlDocument doc = new XmlDocument();
				doc.Load (CachePath);
				foreach (XmlNode node in doc.ChildNodes) {
					foreach (XmlNode cnode in node.ChildNodes) {
						o = CreateObjectInstance();
						foreach (XmlAttribute attr in cnode.Attributes) {
							info = o.GetType().GetProperty (attr.Name);
							if (info != null) {
//								if ((info.PropertyType == typeof(int)) || (info.PropertyType == typeof(System.Int32)))
								if (info.PropertyType == typeof(System.Int32))
									info.SetValue (o, System.Convert.ToInt32 (attr.Value), null);
//								if ((info.PropertyType is string) || (info.PropertyType == typeof(System.String)))
								if (info.PropertyType == typeof(System.String))
									info.SetValue (o, attr.Value, null);
								if (info.PropertyType == typeof(DateTime))
									info.SetValue (o, System.Convert.ToDateTime (attr.Value), null);
								if (info.PropertyType == typeof(bool))
									info.SetValue (o, System.Convert.ToBoolean (attr.Value), null);
							}
						}
						Add (o);
						i++;
//						if ((i % 100) == 0)
//							System.Console.WriteLine("Reading object :" + i);
//						AddCacheObject (o);
						o = null;
					}
				}
				doc = null;
				UpdateKeys();
			}
		}
		
		public void CopyData (object aFrom, object aTo)
		{
			if ((aFrom == null) || (aTo == null))
				return;
			if (aFrom.GetType() != CacheType)
				throw new Exception ("Object to copy from is not right type: " +
				                     aFrom.GetType() + " CacheType(" + CacheType + ")");
			if (aTo.GetType() != CacheType)
				throw new Exception ("Object to copy to is not right type: " +
				                     aTo.GetType() + " CacheType(" + CacheType + ")");
			foreach (PropertyInfo i in propList)
				i.SetValue(aTo, i.GetValue(aFrom, null), null);
		}

		/// <summary>
		/// Copies all properties specifying CachedPropertyValue attribute
		/// </summary>
		public static void ObjectCopyData (object aFrom, object aTo)
		{
/*			PropertyInfo[] props = 
			foreach (PropertyInfo i in propList)
				i.SetValue(aTo, i.GetValue(aFrom, null), null);*/
		}
		
		public object CreateObjectInstance()
		{
			ConstructorInfo[] infos = CacheType.GetConstructors();
			foreach (ConstructorInfo i in infos)
				if (i.GetParameters().Length == 0)
					return (i.Invoke (null));
			throw new Exception ("Couldn't create cache object, no constructor without params");
		}
		
		public CacheList (string aFileName, string aKeyProperty, System.Type aCacheType)
			: base ()
		{
			System.Console.WriteLine("Initializing CACHE " + aFileName);
			keyProperty = aKeyProperty;
			if (KeyProperty == "")
				throw new Exception ("Key property for cache list can't be empty");
			cacheType = aCacheType;
			
			if (CacheType == null)
				throw new Exception ("Cache type for cache list can't be null");
			PropertyInfo info = CacheType.GetProperty (KeyProperty);
			keyinfo = info;
			if (info == null)
				throw new Exception ("Property " + KeyProperty + " not found in " + aCacheType + " class!");
			fileName = aFileName;
			if (FileName == "")
				throw new Exception ("Filename for cache list can't be empty");
			
			PropertyInfo[] infos = aCacheType.GetProperties();
			foreach (PropertyInfo i in infos) {
				if ((i.CanRead == true) && (i.CanWrite == true)) {
					object[] attrs = i.GetCustomAttributes (typeof(CachedPropertyValueAttribute), true);
					foreach (object o in attrs) {
						if (((CachedPropertyValueAttribute) o).Enabled == true)
							propList.Add (i);
					}
				}
			}
			LoadCache();
		}
	}
}
