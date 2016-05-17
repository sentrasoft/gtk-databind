// Interfaces.cs - Field Attribute to assign additional information for Gtk#Databindings
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
	/// Event used to notify owner about change in clipped range
	/// </summary>
	/// <param name="aInfo">
	/// clipping range that changed <see cref="IRangeClippingInformation"/>
	/// </param>
	/// <param name="aPreviousFrom">
	/// Specifies previous From value <see cref="System.Int32"/>
	/// </param>
	/// <param name="aPreviousTo">
	/// Specifies previous To value <see cref="System.Int32"/>
	/// </param>
	public delegate void RangeChangedEvent (IRangeClippingInformation aInfo, int aPreviousFrom, int aPreviousTo);
	
	/// <summary>
	/// Event used to notify owner to add range specified elements
	/// </summary>
	/// <param name="aFrom">
	/// Specifies From value <see cref="System.Int32"/>
	/// </param>
	/// <param name="aTo">
	/// Specifies To value <see cref="System.Int32"/>
	/// </param>
	public delegate void RangeAddEvent (int aFrom, int aTo);
	
	/// <summary>
	/// Event used to notify owner to clear elements in specified range
	/// </summary>
	/// <param name="aFrom">
	/// Specifies From value <see cref="System.Int32"/>
	/// </param>
	/// <param name="aTo">
	/// Specifies To value <see cref="System.Int32"/>
	/// </param>
	public delegate void RangeClearEvent (int aFrom, int aTo);
	
	/// <summary>
	/// Defines how page buffer should load/treat pagging information
	/// </summary>
	public enum BufferPivotLocation
	{
		/// <summary>
		/// Load whole data stack at once
		/// </summary>
		Whole,
		/// <summary>
		/// Pivot location is at start of buffer
		/// </summary>
		Start,
		/// <summary>
		/// Pivot location is at center of buffer
		/// </summary>
		Center,
		/// <summary>
		/// Pivot location is at end of buffer
		/// </summary>
		End
	}
	
	/// <summary>
	/// Specifies range information 
	/// </summary>
	public interface IRangeClippingInformation
	{
		/// <value>
		/// Minimum value in range
		/// </value>
		int From { get; set; }
		/// <value>
		/// Maximum value in range
		/// </value>
		int To { get; set; }
		/// <summary>
		/// Returns if clipping information is valid or not
		/// </summary>
		bool IsValid { get; }
		/// <summary>
		/// Provides notification on clipping range change
		/// </summary>
		event RangeChangedEvent OnChanged;
	}
	
	/// <summary>
	/// Specifies buffer range information 
	/// </summary>
	public interface IBufferClippingInformation : IRangeClippingInformation
	{
		/// <summary>
		/// Checks if specified range is equal 
		/// </summary>
		/// <param name="aInfo">
		/// Range to be checked <see cref="IRangeClippingInformation"/>
		/// </param>
		/// <returns>
		/// true if equal, false if not <see cref="System.Boolean"/>
		/// </returns>
		bool IsEqual (IRangeClippingInformation aInfo);
		/// <summary>
		/// Checks if specified range is equal 
		/// </summary>
		/// <param name="aFrom">
		/// Minimum of range <see cref="System.Int32"/>
		/// </param>
		/// <param name="aTo">
		/// Maxmum of range <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// true if equal, false if not <see cref="System.Boolean"/>
		/// </returns>
		bool IsEqual (int aFrom, int aTo);
		/// <summary>
		/// Checks if range contains specified index
		/// </summary>
		/// <param name="aIdx">
		/// Index to be checked for <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// true if index is inside of range <see cref="System.Boolean"/>
		/// </returns>
		bool Contains (int aIdx);
		/// <summary>
		/// Returns if this clipping information contains specified index
		/// </summary>
		/// <param name="aInfo">
		/// Clipping information to be checked with <see cref="IRangeClippingInformation"/>
		/// </param>
		/// <returns>
		/// true if clipping information is valid and region is inside From To range <see cref="System.Boolean"/>
		/// </returns>
		bool Contains (IRangeClippingInformation aInfo);
		/// <summary>
		/// Returns if this clipping information contains specified index
		/// </summary>
		/// <param name="aFrom">
		/// Min clip parameter <see cref="System.Int32"/>
		/// </param>
		/// <param name="aTo">
		/// Max clip parameter <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// true if clipping information is valid and region is inside From To range <see cref="System.Boolean"/>
		/// </returns>
		bool Contains (int aFrom, int aTo);
		/// <summary>
		/// Returns if this clipping information contains specified index
		/// </summary>
		/// <param name="aInfo">
		/// Clipping information to be checked with <see cref="IRangeClippingInformation"/>
		/// </param>
		/// <returns>
		/// true if clipping information is vaid and index is inside From To range <see cref="System.Boolean"/>
		/// </returns>
		bool IntersectsWith (IRangeClippingInformation aInfo);
		/// <summary>
		/// Returns if this clipping information contains specified index
		/// </summary>
		/// <param name="aFrom">
		/// Min clip parameter <see cref="System.Int32"/>
		/// </param>
		/// <param name="aTo">
		/// Max clip parameter <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// true if clipping information is vaid and index is inside From To range <see cref="System.Boolean"/>
		/// </returns>
		bool IntersectsWith (int aFrom, int aTo);
		/// <summary>
		/// Adds region to this buffer if they can be connected into one
		/// </summary>
		/// <param name="aInfo">
		/// Clipping buffer to add <see cref="IRangeClippingInformation"/>
		/// </param>
		/// <returns>
		/// true if clipping region can be connected, false if not <see cref="System.Boolean"/>
		/// </returns>
		bool Add (IRangeClippingInformation aInfo);
		/// <summary>
		/// Adds region to this buffer if they can be connected into one
		/// </summary>
		/// <param name="aFrom">
		/// Min clip parameter <see cref="System.Int32"/>
		/// </param>
		/// <param name="aTo">
		/// Max clip parameter <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// true if clipping region can be connected, false if not <see cref="System.Boolean"/>
		/// </returns>
		bool Add (int aFrom, int aTo);
		/// <summary>
		/// Removes region from this buffer if they can be connected into one
		/// </summary>
		/// <param name="aInfo">
		/// Clipping buffer to add <see cref="IRangeClippingInformation"/>
		/// </param>
		/// <param name="aNewBuffer">
		/// Buffer being created when this one split in two <see cref="IRangeClippingInformation"/>
		/// </param>
		/// <returns>
		/// true if clipping region can be connected, false if not <see cref="System.Boolean"/>
		/// </returns>
		bool Remove (IRangeClippingInformation aInfo, out IRangeClippingInformation aNewBuffer);
		/// <summary>
		/// Removes region from this buffer if they can be connected into one
		/// </summary>
		/// <param name="aFrom">
		/// Min clip parameter <see cref="System.Int32"/>
		/// </param>
		/// <param name="aTo">
		/// Max clip parameter <see cref="System.Int32"/>
		/// </param>
		/// <param name="aNewBuffer">
		/// Buffer being created when this one split in two <see cref="IRangeClippingInformation"/>
		/// </param>
		/// <returns>
		/// true if clipping region can be connected, false if not <see cref="System.Boolean"/>
		/// </returns>
		bool Remove (int aFrom, int aTo, out IRangeClippingInformation aNewBuffer);
	}
}
