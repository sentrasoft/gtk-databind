//BaseNotifyPropertyChanged.cs - Field Attribute to assign additional information for Gtk#Databindings
//
//Author: m. <ml@arsis.net>
//
//Copyright (c) at 10:21 PM 12/24/2008
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
using System.ComponentModel;

namespace System.Data.Bindings
{
	/// <summary>
	/// Provides simplification for derivation of INotifyPropertyChanged
	/// classes. It also provides OnPropertyChanged which can safely be 
	/// called anytime
	/// </summary>
	public class BaseNotifyPropertyChanged : INotifyPropertyChanged
	{
		/// <summary>
		/// PropertyChanged delegate as specified in INotifyPropertyChanged
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Method calls PropertyChanged if it is not null, but it allows external
		/// objects to access this one for convinience
		/// </summary>
		/// <param name="aPropertyName">
		/// Name of the property which changed <see cref="System.String"/>
		/// </param>
		public virtual void OnPropertyChanged (string aPropertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged (this, new PropertyChangedEventArgs(aPropertyName));
		}
	}
}
