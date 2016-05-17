// BufferClippingInformation.cs - Field Attribute to assign additional information for Gtk#Databindings
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
using System.Data.Bindings;

namespace System.Data.Bindings.Cached
{
	/// <summary>
	/// Describes clipping information in buffer
	/// </summary>
	public class BufferClippingInformation : IBufferClippingInformation, IDisconnectable
	{
		private int from = 0;
		/// <summary>
		/// Specifies min index region of this clipping info
		/// </summary>
		public int From {
			get { return (from); }
			set {
				if (from == value)
					return;
				from = value;
				Changed();
			} 
		}

		private int to = 0;
		/// <summary>
		/// Specifies max index region of this clipping info
		/// </summary>
		public int To {
			get { return (to); }
			set { 
				if (to == value)
					return;
				to = value; 
				Changed();
			} 
		}

		private event RangeChangedEvent onChanged = null;
		/// <summary>
		/// Provides notification on clipping range change
		/// </summary>
		public event RangeChangedEvent OnChanged {
			add { onChanged += value; }
			remove { onChanged -= value; }
		}
		
		/// <summary>
		/// Returns if clipping information is valid or not
		/// </summary>
		public bool IsValid {
			get { return (From <= To); }
		}

		/// <summary>
		/// Checks if specified range is equal 
		/// </summary>
		/// <param name="aInfo">
		/// Range to be checked <see cref="IRangeClippingInformation"/>
		/// </param>
		/// <returns>
		/// true if equal, false if not <see cref="System.Boolean"/>
		/// </returns>
		public bool IsEqual (IRangeClippingInformation aInfo)
		{
			if (aInfo == null)
				return (false);
			return (IsEqual (aInfo.From, aInfo.To));
		}
		
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
		public bool IsEqual (int aFrom, int aTo)
		{
			return ((From == aFrom) && (To == aTo));
		}
		
		/// <summary>
		/// Returns if this clipping information contains specified index
		/// </summary>
		/// <param name="aIdx">
		/// Index to check for <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// true if clipping information is valid and index is inside From To range <see cref="System.Boolean"/>
		/// </returns>
		public bool Contains (int aIdx)
		{
			return ((aIdx >= From) && (aIdx <= To) && (IsValid == true)); 
		}
		
		/// <summary>
		/// Returns if this clipping information contains specified index
		/// </summary>
		/// <param name="aInfo">
		/// Clipping information to be checked with <see cref="IRangeClippingInformation"/>
		/// </param>
		/// <returns>
		/// true if clipping information is valid and region is inside From To range <see cref="System.Boolean"/>
		/// </returns>
		public bool Contains (IRangeClippingInformation aInfo)
		{
			if (aInfo == null)
				return (false);
			return (Contains (aInfo.From, aInfo.To)); 
		}

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
		public bool Contains (int aFrom, int aTo)
		{
			return (Contains(aFrom) && Contains(aTo)); 
		}
		
		/// <summary>
		/// Returns if clipping range specified is valid or not
		/// </summary>
		/// <param name="aFrom">
		/// Minimum of range <see cref="System.Int32"/>
		/// </param>
		/// <param name="aTo">
		/// Maxmum of range <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// true if range is valid or not <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsRangeValid (int aFrom, int aTo) 
		{
			return (aFrom <= aTo);
		}

		/// <summary>
		/// Returns if this clipping information contains specified index
		/// </summary>
		/// <param name="aInfo">
		/// Clipping information to be checked with <see cref="IRangeClippingInformation"/>
		/// </param>
		/// <returns>
		/// true if clipping information is vaid and index is inside From To range <see cref="System.Boolean"/>
		/// </returns>
		public bool IntersectsWith (IRangeClippingInformation aInfo)
		{
			if (aInfo == null)
				return (false);
			return (IntersectsWith (aInfo.From, aInfo.To));
		}
		
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
		public bool IntersectsWith (int aFrom, int aTo)
		{
			if (IsValid == false)
				return (false);
			return ((Contains(aFrom) == true) || 
			        (Contains(aTo) == true) ||
			        ((aFrom <= From) && (aTo >= To)));
		}
		
		/// <summary>
		/// Adds region to this buffer if they can be connected into one
		/// </summary>
		/// <param name="aInfo">
		/// Clipping buffer to add <see cref="IRangeClippingInformation"/>
		/// </param>
		/// <returns>
		/// true if clipping region can be connected, false if not <see cref="System.Boolean"/>
		/// </returns>
		public bool Add (IRangeClippingInformation aInfo)
		{
			if (aInfo == null)
				return (false);
			return (Add (aInfo.From, aInfo.To));
		}
		
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
		public bool Add (int aFrom, int aTo)
		{
			if (IsValid == false) {
				from = aFrom;
				to = aTo;
				return (IsValid);
			}
			if (IntersectsWith(aFrom, aTo) == false)
				return (false);
			bool res = ((From < aFrom) || (To > aTo));
			if (res == true) {
				if (From < aFrom)
					from = aFrom;
				if (To > aTo)
					to = aTo;
				Changed();
			}
			return (res);
		}
		
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
		public bool Remove (IRangeClippingInformation aInfo, out IRangeClippingInformation aNewBuffer)
		{
			aNewBuffer = null;
			if (aInfo == null)
				return (false);
			return (Remove (aInfo.From, aInfo.To, out aNewBuffer));
		}
		
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
		public bool Remove (int aFrom, int aTo, out IRangeClippingInformation aNewBuffer)
		{
			aNewBuffer = null;
			if (IsRangeValid(aFrom, aTo) == false)
				return (false);
			if (IsEqual(aFrom, aTo) == true) {
				from = 0;
				to = -1;
				Changed();
				return (true);
			}
			if (IntersectsWith(aFrom, aTo) == false)
				return (false);
			if (Contains(aFrom, aTo) == true) {
				// split this range into two
				int t = To;
				to = aTo - 1;
				aNewBuffer = new BufferClippingInformation (aTo + 1, t);
				if (aNewBuffer.IsValid == false)
					aNewBuffer = null;
				Changed();
				return (true);
			}
			if (aFrom < From)
				from = aTo + 1;
			else
				to = aFrom - 1;
			return (true);
		}

		/// <summary>
		/// Provides operator to check
		/// </summary>
		/// <param name="aFirst">
		/// First clipping information <see cref="BufferClippingInformation"/>
		/// </param>
		/// <param name="aSecond">
		/// Second clipping information <see cref="BufferClippingInformation"/>
		/// </param>
		/// <returns>
		/// false if any is null, not valid, are equal or one contains another <see cref="System.Boolean"/>
		/// </returns>
		public static bool operator < (BufferClippingInformation aFirst, BufferClippingInformation aSecond)
		{
			if ((aFirst == null) || (aSecond == null))
				return (false);
			if ((aFirst.IsValid == false) || (aSecond.IsValid == false))
				return (false);
			if (aFirst.IsEqual(aSecond) == true)
				return (false);
			if (aFirst.Contains(aSecond) == true)
				return (false);
			if (aSecond.Contains(aFirst) == true)
				return (false);
			return (aFirst.From < aSecond.From);
		}
		
		/// <summary>
		/// Provides operator to check
		/// </summary>
		/// <param name="aFirst">
		/// First clipping information <see cref="BufferClippingInformation"/>
		/// </param>
		/// <param name="aSecond">
		/// Second clipping information <see cref="BufferClippingInformation"/>
		/// </param>
		/// <returns>
		/// false if any is null, not valid, are equal or one contains another <see cref="System.Boolean"/>
		/// </returns>
		public static bool operator > (BufferClippingInformation aFirst, BufferClippingInformation aSecond)
		{
			if ((aFirst == null) || (aSecond == null))
				return (false);
			if ((aFirst.IsValid == false) || (aSecond.IsValid == false))
				return (false);
			if (aFirst.IsEqual(aSecond) == true)
				return (false);
			if (aFirst.Contains(aSecond) == true)
				return (false);
			if (aSecond.Contains(aFirst) == true)
				return (false);
			return (aFirst.From > aSecond.From);
		}

		/// <summary>
		/// Notifies about change in this range
		/// </summary>
		public void Changed()
		{
			if (onChanged != null)
				onChanged (this, 0, 0);
		}
		
		/// <summary>
		/// Disconnects and destroys
		/// </summary>
		public void Disconnect()
		{
			onChanged = null;
		}
		
		/// <summary>
		/// Creates clipping information
		/// </summary>
		public BufferClippingInformation()
			: this (0, -1)
		{
		}
		
		/// <summary>
		/// Creates clipping information
		/// </summary>
		public BufferClippingInformation (int aFrom, int aTo)
		{
			from = aFrom;
			to = aTo;
		}

		/// <summary>
		/// Disconnects and destroys this object
		/// </summary>
		~BufferClippingInformation()
		{
			Disconnect();
		}
	}
}
