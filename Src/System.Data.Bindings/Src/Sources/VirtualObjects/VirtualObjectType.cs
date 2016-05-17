// VirtualObjectType.cs - VirtualObjectType implementation for Gtk#Databindings
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
	/// This object provides specification for all on-fly objects
	/// Objects provided this way don't need any kind of compile time specification
	/// they can be run-time defined, maintained and destroyed
	/// </summary>
	public class VirtualObjectType
	{
		private VirtualMember[] properties = null;

		private bool isUnique = false;
		/// <summary>
		/// Returns if this is unique object
		/// </summary>
		public bool IsUnique {
			get { return (isUnique); }
		}
		
		private string name = "";
		/// <summary>
		/// Returns name of this type
		/// </summary>
		public string Name {
			get { return (name); }
			set {
				if (value == "")
					return;
				if (name != "")
					throw new ExceptionCantRenameAlreadyNamedVirtualObject();
				name = value; 
				if (IsUnique == false)
					VirtualObjects.Add (this);
			}
		}
		
		/// <summary>
		/// Returns property count
		/// </summary>
		public int Count {
			get {
				if (properties == null)
					return (0);
				return (properties.Length);
			}
		}
		
		private int refCounter = 0;
		
		private int lockcounter = 0;
		/// <summary>
		/// Returns current locking state of this object type
		/// </summary>
		public bool IsLocked {
			get { return (lockcounter > 0); }
		}
		
		/// <summary>
		/// Returns VirtualMember by the index
		/// It is much faster than accessing by name, but it should be used along with locking
		/// </summary>
		public VirtualMember this [int a_Idx] {
			get { 
				if (properties == null)
					return (null);
				return (properties[a_Idx]); 
			}
		}
		
		/// <summary>
		/// Returns VirtualMember by name, which is slower than acessing by index,
		/// but it is safer and can safely be used without locking
		/// </summary>
		public VirtualMember this [string a_Name] {
			get {
				if (properties == null)
					return (null);
				foreach (VirtualMember vm in properties)
					if (vm.Name == a_Name)
						return (vm);
				return (null);
			}
		}
		
		/// <summary>
		/// Increments the Reference counter for this type
		/// </summary>
		public void Ref()
		{
			refCounter++;
		}
		
		/// <summary>
		/// Decrements the Reference counter for this type
		/// </summary>
		public bool Unref()
		{
			refCounter--;
			if (IsUnique == true)
				return (refCounter <= 0);
			if (refCounter <= 0)
				VirtualObjects.Remove (this);
			return (refCounter <= 0);
		}
		
		/// <summary>
		/// Increments the Lock counter for this type, any changes to properties will
		/// result in Exception
		/// </summary>
		public void Lock()
		{
			lockcounter++;
		}
		
		/// <summary>
		/// Decrements the Lock counter for this type
		/// </summary>
		public bool Unlock()
		{
			lockcounter--;
			if (lockcounter < 0)
				lockcounter = 0;
			return (IsLocked); 
		}
		
		/// <summary>
		/// Adds new memeber for this ObjectType
		/// </summary>
		/// <param name="a_Name">
		/// Member name <see cref="System.String"/>
		/// </param>
		/// <param name="a_Type">
		/// Member type <see cref="System.Type"/>
		/// </param>
		public void AddMember (string a_Name, System.Type a_Type)
		{
			if (IsLocked == true)
				throw new ExceptionCantAddMemberToLockedVirtualObject();
			VirtualMember member = this[a_Name];
			if (member != null)
				if (a_Type == member.PropertyType)
					return;
				else
					throw new ExceptionCreatingVirtualPropertyTwiceWithDifferentType (a_Name);
			member = null;
			
			VirtualMember[] newprops;
			if (properties != null) {
				newprops = new VirtualMember [properties.Length+1];
				properties.CopyTo (newprops, 0);
				newprops[newprops.Length-1] = new VirtualMember (a_Name, a_Type, newprops.Length-1);
			}
			else {
				newprops = new VirtualMember [1];
				newprops[0] = new VirtualMember (a_Name, a_Type, 0);				
			}
			properties = null;
			properties = newprops;
			newprops = null;
		}
		
		/// <summary>
		/// Removes Member from this ObjectType
		/// </summary>
		/// <param name="a_Index">
		/// Member index <see cref="System.Int32"/>
		/// </param>
		public void RemoveMember (int a_Index)
		{
			if ((a_Index < 0) && (a_Index >= Count))
				throw new ExceptionIndexOutOfRangeWhenRemovingVirtualMember();
			if (IsLocked == true)
				throw new ExceptionCantRemoveMemberFromLockedVirtualObject();
			if (Count == 1) {
				properties[0] = null;
				properties = null;
			}
			else {
				VirtualMember[] newprops = (VirtualMember[]) new object [properties.Length-1];
				int j = 0;
				for (int i=0; i<properties.Length; i++)
					if (i != a_Index) {
						properties[i].Index = j;
						newprops[j] = properties[i];
						properties[i] = null;
						j++;
					}
				properties = null;
				properties = newprops;
			}
		}
		
		/// <summary>
		/// Removes Member from this ObjectType
		/// </summary>
		/// <param name="a_Member">
		/// Virtual member <see cref="VirtualMember"/>
		/// </param>
		public void RemoveMember (VirtualMember a_Member)
		{
			if (a_Member == null)
				return;
			if (a_Member.Name == properties[a_Member.Index].Name)
				RemoveMember (a_Member.Index);
			else
				throw new ExceptionRemovingMemberWithDifferentName();
		}
		
		/// <summary>
		/// Removes Member from this ObjectType
		/// </summary>
		/// <param name="a_Name">
		/// Member name <see cref="System.String"/>
		/// </param>
		public void RemoveMember (string a_Name)
		{
			VirtualMember member = this[a_Name];
			if (member != null)
				RemoveMember (member.Index);
			else
				throw new ExceptionCreatingVirtualPropertyTwiceWithDifferentType(a_Name);
			member = null;
		}
		
		/// <summary>
		/// Inherits the properties from other type
		/// </summary>
		/// <param name="a_Type">
		/// Virtual object type <see cref="VirtualObjectType"/>
		/// </param>
		/// <returns>
		/// true <see cref="System.Boolean"/>
		/// </returns>
		/// <remarks>
		/// Throws Exception if property already exists
		///
		/// WARNING! Modifying type doesn't updates already created objects
		/// </remarks>
		public bool InheritStrict (VirtualObjectType a_Type)
		{
			if (a_Type == null)
				throw new ExceptionVirtualObjectCantInheritNullType();
			for (int i=0; i<Count; i++)
				AddMember (a_Type[i].Name, a_Type[i].PropertyType);
			return (true);
		}
		
		/// <summary>
		/// Inherits the properties from other type
		/// </summary>
		/// <param name="a_Type">
		/// Object type <see cref="VirtualObjectType"/>
		/// </param>
		/// <returns>
		/// true <see cref="System.Boolean"/>
		/// </returns>
		/// <remarks>
		/// If property already exists, it avoids copying it
		///
		/// WARNING! Modifying type doesn't updates already created objects
		/// </remarks>
		public bool InheritRelaxed (VirtualObjectType a_Type)
		{
			if (a_Type == null)
				return (false);
			for (int i=0; i<Count; i++)
				if (this[a_Type[i].Name] == null)
					AddMember (a_Type[i].Name, a_Type[i].PropertyType);
			return (true);
		}
		
		/// <summary>
		/// Creates new VirtualObjectType
		/// </summary>
		public VirtualObjectType()
		{
		}

		/// <summary>
		/// Creates new VirtualObjectType
		/// </summary>
		/// <param name="a_IsUnique">
		/// Defines if this is unique type <see cref="System.Boolean"/>
		/// </param>
		internal VirtualObjectType (bool a_IsUnique)
		{
			isUnique = a_IsUnique;
		}

		/// <summary>
		/// Creates new VirtualObjectType
		/// </summary>
		/// <param name="a_Name">
		/// Name of object type <see cref="System.String"/>
		/// </param>
		public VirtualObjectType (string a_Name)
		{
			Name = a_Name;
		}

		/// <summary>
		/// Creates new VirtualObjectType
		/// </summary>
		/// <param name="a_IsUnique">
		/// Defines if this is unique type <see cref="System.Boolean"/>
		/// </param>
		/// <param name="a_Name">
		/// Type name <see cref="System.String"/>
		/// </param>
		internal VirtualObjectType (bool a_IsUnique, string a_Name)
		{
			isUnique = a_IsUnique;
			Name = a_Name;
		}
	}
}
