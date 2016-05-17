// HierarchicalList.cs - HierarchicalList implementation for Gtk#Databindings
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
using System.Data.Bindings;

namespace System.Data.Bindings.Collections
{
	/// <summary>
	/// Provides routines to access any list as hierarchical, but does not depend on
	/// Observeable 
	/// </summary>
	public static class HierarchicalList
	{
		/// <summary>
		/// Resolves object from hierarchy, catch in this one is that it can resolve
		/// any list, not just Observeable ones
		/// </summary>
		/// <param name="aList">
		/// List object <see cref="IList"/>
		/// </param>
		/// <param name="aIdx">
		/// Index of item <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// Item at index <see cref="System.Object"/>
		/// </returns>
		public static object Get (IList aList, int[] aIdx) 
		{
			if ((aIdx == null) || (aList == null))
				return (null);
				
			IList lst = aList;
			for (int i=0; i<aIdx.Length; i++) {
				if (lst == null)
					return (null);
				if (i < (aIdx.Length-1)) {
					if ((lst[aIdx[i]] is IList) == false)
						return (null);
					else
						lst = (lst[aIdx[i]] as IList);
				}
				else
					if (aIdx[i] < (lst.Count))
						return (lst[aIdx[i]]);
					else
						return (null);
			}
			return (null);
		}
		
		/// <summary>
		/// Resolves object from hierarchy, catch in this one is that it can resolve
		/// any list, not just Observeable ones
		/// </summary>
		/// <param name="aList">
		/// List object <see cref="IList"/>
		/// </param>
		/// <param name="aIdx">
		/// Index of item <see cref="System.Int32"/>
		/// </param>
		/// <param name="aValue">
		/// New value <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		public static bool Set (IList aList, int[] aIdx, object aValue) 
		{
			if ((aIdx == null) || (aList == null))
				return (false);
				
			IList lst = aList;
			for (int i=0; i<aIdx.Length; i++) {
				if (lst == null)
					return (false);
				if (i < (aIdx.Length-1)) {
					if ((lst[aIdx[i]] is IList) == false)
						return (false);
					else
						lst = (lst[aIdx[i]] as IList);
				}
				else {
					lst[aIdx[i]] = aValue;
					return (true);
				}
			}
			return (false);
		}
		
		/// <summary>
		/// Moves one object from one place to another and takes care of the messages for notifications
		/// </summary>
		/// <param name="aList">
		/// Moves element in list from one place to another <see cref="IList"/>
		/// </param>
		/// <param name="aFrom">
		/// Path specifying from location <see cref="System.Int32"/>
		/// </param>
		/// <param name="aTo">
		/// Path specifying destination <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// true if successfull <see cref="System.Boolean"/>
		/// </returns>
		public static bool Move (IList aList, int[] aFrom, int[] aTo)
		{
			return (false);
		}
		
		/// <summary>
		/// Adds object in hierarchy, catch in this one is that it can resolve
		/// any list, not just Observeable ones, so you can use this function to add
		/// directly into hierarchy
		/// </summary>
		/// <param name="aList">
		/// List object <see cref="IList"/>
		/// </param>
		/// <param name="aIdx">
		/// Path wherre to add element <see cref="System.Int32"/>
		/// </param>
		/// <param name="aValue">
		/// Object to be add <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		[ToDo ("Take care of manual messaging on IList")]
		public static bool Add (IList aList, int[] aIdx, object aValue) 
		{
			if ((aIdx == null) || (aList == null))
				return (false);
				
			IList lst = aList;
			if ((aIdx == null) || (aIdx.Length <= 1)) {
				if ((lst[aIdx[0]] is IList) == false)
					lst.Add (aValue);
				else
					((IList) lst[aIdx[0]]).Add (aValue);
				return (true);
			}
			else
				for (int i=0; i<aIdx.Length; i++) {
					if (lst == null)
						return (false);
					if (i < (aIdx.Length-1)) {
						if ((lst[aIdx[i]] is IList) == false)
							return (false);
						else
							lst = (lst[aIdx[i]] as IList);
					}
					else {
						if (lst[aIdx[i]] is IList) {
							((IList) lst[aIdx[i]]).Add (aValue);
							if ((lst is IObserveableList) == false) {
								//TODO: The need to take care of manual messaging
							}
							return (true);
						}
						return (false);
					}
				}
			return (false);
		}

		/// <summary>
		/// Returns parent object of specified object in master list
		/// </summary>
		/// <param name="aList">
		/// Master list <see cref="IList"/>
		/// </param>
		/// <param name="aObject">
		/// Object in question <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Parent object or null if parent does not exists <see cref="System.Object"/>
		/// </returns>
		public static object GetParentOf (IList aList, object aObject)
		{
			int[] newPath = IndexOf (aList, aObject);
			return (GetParentOf(aList, newPath));
		}
		
		/// <summary>
		/// Returns parent object of specified object in master list
		/// </summary>
		/// <param name="aList">
		/// Master list <see cref="IList"/>
		/// </param>
		/// <param name="aPath">
		/// Path to object in question <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// Parent object or null if parent does not exists <see cref="System.Object"/>
		/// </returns>
		public static object GetParentOf (IList aList, int[] aPath)
		{
			if ((aPath == null) || (aList == null))
				return (null);
			if (aPath.Length == 1)
				return (aList);
			int[] res = new int[aPath.Length - 1];
			for (int i=0; i<(aPath.Length - 1); i++)
				res[i] = aPath[i];
			return (Get(aList, res));
		}
		
		/// <summary>
		/// Searches hierarchicaly over list structure for specified object
		/// </summary>
		/// <param name="aList">
		/// Master list <see cref="IList"/>
		/// </param>
		/// <param name="aObject">
		/// Searched object <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Path to object or null if not found <see cref="System.Int32"/>
		/// </returns>
		public static int[] IndexOf (IList aList, object aObject)
		{
			if (aList == null)
				return (null);
			int[] newPath = null;
			int i = aList.IndexOf (aObject);
			if (i >= 0) {
				newPath = new int [1];
				newPath[0] = i;
				return (newPath);
			}
			else {
				int j = 0;
				foreach (object o in aList) {
					if (o is IList) {
						newPath = IndexOf (o as IList, aObject);
						if (newPath != null) {
							newPath = newPath.AddPathIndexOnStart(j);
							return (newPath);
						}
					}
					j++;
				}
			}
			return (null);
		}
		
		/// <summary>
		/// Adds object in hierarchy, catch in this one is that it can resolve
		/// any list, not just Observeable ones, so you can use this function to add
		/// directly into hierarchy
		/// </summary>
		/// <param name="aList">
		/// List object <see cref="IList"/>
		/// </param>
		/// <param name="aIdx">
		/// Path where object currently resides <see cref="System.Int32"/>
		/// </param>
		/// <param name="aValue">
		/// Object to add <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		[ToDo ("Take care of manual messaging on IList")]
		public static bool AddToLevelDown (IList aList, int[] aIdx, object aValue) 
		{
			if ((aIdx == null) || (aList == null))
				return (false);
				
			IList lst = aList;
			if ((aIdx == null) || (aIdx.Length <= 2)) {
				lst.Add (aValue);
				return (true);
			}
			else
				for (int i=0; i<(aIdx.Length-1); i++) {
					if (lst == null)
						return (false);
					if (i < (aIdx.Length-2)) {
						if ((lst[aIdx[i]] is IList) == false)
							return (false);
						else
							lst = (lst[aIdx[i]] as IList);
					}
					else {
						if (lst[aIdx[i]] is IList) {
							((IList) lst[aIdx[i]]).Add (aValue);
							if ((lst is IObserveableList) == false) {
								//TODO: The need to take care of manual messaging
							}
							return (true);
						}
						return (false);
					}
				}
			return (false);
		}
		
		/// <summary>
		/// Resolves object from hierarchy, catch in this one is that it can resolve
		/// any list, not just Observeable ones
		/// </summary>
		/// <param name="aList">
		/// List object <see cref="IList"/>
		/// </param>
		/// <param name="aIdx">
		/// Path to object which needs removal <see cref="System.Int32"/>
		/// </param>
		/// <param name="aValue">
		/// Object to be removed <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		public static bool Remove (IList aList, int[] aIdx, object aValue) 
		{
			if ((aIdx == null) || (aList == null))
				return (false);
				
			IList lst = aList;
			if ((aIdx == null) || (aIdx.Length <= 1)) {
				lst.Remove (aValue);
				return (true);
			}
			else
				for (int i=0; i<aIdx.Length; i++) {
					if (lst == null)
						return (false);
					if (i < (aIdx.Length-1)) {
						if ((lst[aIdx[i]] is IList) == false)
							return (false);
						else
							lst = (lst[aIdx[i]] as IList);
					}
					else {
						if (lst[aIdx[i]] is IList) {
							((IList) lst[aIdx[i]]).Remove (aValue);
							return (true);
						}
						return (false);
					}
				}
			return (false);
		}
		
		/// <summary>
		/// Resolves object from hierarchy, catch in this one is that it can resolve
		/// any list, not just Observeable ones
		/// </summary>
		/// <param name="aList">
		/// List object <see cref="IList"/>
		/// </param>
		/// <param name="aIdx">
		/// Path to objects that needs removal <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		public static bool RemoveAt (IList aList, int[] aIdx) 
		{
			if ((aIdx == null) || (aList == null))
				return (false);
				
			IList lst = aList;
			if (aIdx.Length == 1) {
				lst.RemoveAt (aIdx[0]);
				return (true);
			}
			else
				for (int i=0; i<aIdx.Length; i++) {
					if (lst == null)
						return (false);
					if (i < (aIdx.Length-2)) {
						if ((lst[aIdx[i]] is IList) == false)
							return (false);
						else
							lst = (lst[aIdx[i]] as IList);
					}
					else {
						if (lst[aIdx[i]] is IList) {
							((IList) lst[aIdx[i]]).RemoveAt (aIdx[i+1]);
							return (true);
						}
						return (false);
					}
				}
			return (false);
		}

		/// <summary>
		/// Inserts object to hierarchy, catch in this one is that it can resolve
		/// any list, not just Observeable ones
		/// </summary>
		/// <param name="aList">
		/// List object <see cref="IList"/>
		/// </param>
		/// <param name="aIdx">
		/// Path to object index <see cref="System.Int32"/>
		/// </param>
		/// <param name="aObject">
		/// Object to be inserted <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		public static bool Insert (IList aList, int[] aIdx, object aObject) 
		{
			if ((aIdx == null) || (aList == null) || (aObject == null))
				return (false);
				
			IList lst = aList;
			if (aIdx.Length == 1) {
				lst.Insert (aIdx[0], aObject);
				return (true);
			}
			else
				for (int i=0; i<aIdx.Length; i++) {
					if (lst == null)
						return (false);
					if (i < (aIdx.Length-2)) {
						if ((lst[aIdx[i]] is IList) == false)
							return (false);
						else
							lst = (lst[aIdx[i]] as IList);
					}
					else {
						if (lst[aIdx[i]] is IList) {
							if (((IList) lst[aIdx[i]]).Count == aIdx[i+1])
								((IList) lst[aIdx[i]]).Add (aObject);
							else
								((IList) lst[aIdx[i]]).Insert (aIdx[i+1], aObject);
							return (true);
						}
						return (false);
					}
				}
			return (false);
		}
	}
}
