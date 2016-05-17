//DefaultAdaptorSelector.cs - Field Attribute to assign additional information for Gtk#Databindings
//
//Author: m. <ml@arsis.net>
//
//Copyright (c) at 11:00 PM 12/11/2008
//
//This program is free software; you can redistribute it and/or
//modify it under the terms of version 2 of the Lesser GNU General 
//Public License as published by the Free Software Foundation.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this program; if not, write to the
//Free Software Foundation, Inc., 59 Temple Place - Suite 330,
//Boston, MA 02111-1307, USA.

using System;

namespace System.Data.Bindings
{
	/// <summary>
	/// Fallback for base Adaptor to be allocated
	/// </summary>
	/// <remarks>
	/// This is the only IAdaptorSelector class not specifying
	/// AdaptorSelectorAttribute. This one is allocated automatically
	/// as default fallback
	/// </remarks>
	public class DefaultAdaptorSelector : IAdaptorSelector
	{
		/// <summary>
		/// Checks class if its type is correct for this selector
		/// </summary>
		/// <param name="aObject">
		/// Object to be checked <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// AdaptorSelector instance or null if type is not correct <see cref="IAdaptorSelector"/>
		/// </returns>
		public IAdaptorSelector CheckType (object aObject)
		{
			return (this);
		}
		
		/// <summary>
		/// Creates adaptor for that type
		/// </summary>
		/// <returns>
		/// Adaptor <see cref="IAdaptor"/>
		/// </returns>
		public IAdaptor CreateAdaptor()
		{
			return (new Adaptor());
		}
		
		/// <summary>
		/// Returns type of adaptor to be checked against if already
		/// allocated is of correct type
		/// </summary>
		/// <returns>
		/// Adaptor type <see cref="System.Type"/>
		/// </returns>
		public System.Type GetAdaptorType()
		{
			return (typeof(Adaptor));
		}
		
		public DefaultAdaptorSelector()
		{
		}
	}
}
