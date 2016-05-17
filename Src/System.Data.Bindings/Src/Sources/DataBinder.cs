// DataBinder.cs - Field Attribute to assign additional information for Gtk#Databindings
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
	/// Provides binding for objects that don't specify their
	/// own DataSource or Mappings properties
	/// </summary>
	public static class DataBinder
	{
		private static ArrayList links = new ArrayList();
		
		/// <summary>
		/// Binds two objects and its properties with no regard to the fact if they are
		/// originally designed with SDB in mind or not
		/// </summary>
		/// <param name="aObj1">
		/// First object <see cref="System.Object"/>
		/// </param>
		/// <param name="aPropName1">
		/// Property name for first object <see cref="System.String"/>
		/// </param>
		/// <param name="aObj2">
		/// Second object <see cref="System.Object"/>
		/// </param>
		/// <param name="aPropName2">
		/// Property name for second object <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// DataLinker object which can be used if it is needed to be handled manually <see cref="DataLinker"/>
		/// </returns>
		public static DataLinker Bind (object aObj1, string aPropName1, object aObj2, string aPropName2)
		{
			return (Bind (EBindDirection.TwoWay, aObj1, aPropName1, aObj2, aPropName2));
		}

		/// <summary>
		/// Binds two objects and its properties with no regard to the fact if they are
		/// originally designed with SDB in mind or not
		/// </summary>
		/// <param name="aDirection">
		/// Direction in which data flows <see cref="EBindDirection"/>
		/// </param>
		/// <param name="aObj1">
		/// First object <see cref="System.Object"/>
		/// </param>
		/// <param name="aPropName1">
		/// Property name for first object <see cref="System.String"/>
		/// </param>
		/// <param name="aObj2">
		/// Second object <see cref="System.Object"/>
		/// </param>
		/// <param name="aPropName2">
		/// Property name for second object <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// DataLinker object which can be used if it is needed to be handled manually <see cref="DataLinker"/>
		/// </returns>
		public static DataLinker Bind (EBindDirection aDirection, object aObj1, string aPropName1, object aObj2, string aPropName2)
		{
			if ((aObj1 == null) || (aObj2 == null) || (aPropName1 == "") || (aPropName2 == ""))
				return (null);
			// Search for existing link
			foreach (DataLinker dl in links) {
				if ((aObj1 == dl.Source) && (aObj2 == dl.Destination))
					if ((aPropName1 == dl.SourceProperty) && (aPropName2 == dl.DestinationProperty))
						return (dl);
				if ((aObj1 == dl.Destination) && (aObj2 == dl.Source))
					if ((aPropName1 == dl.DestinationProperty) && (aPropName2 == dl.SourceProperty))
						return (dl);
			}
			
			// If it came to here then this binding is unique
			DataLinker ndl = new DataLinker (aDirection, aObj1, aPropName1, aObj2, aPropName2);
			links.Add (ndl);
			return (ndl);
		}
		
		/// <summary>
		/// Removes all bindings for particular object
		/// </summary>
		/// <param name="aObject">
		/// Object removed from bindings <see cref="System.Object"/>
		/// </param>
		public static void Unbind (object aObject)
		{
			if (aObject == null)
				return;
			DataLinker lnk = null;
			for (int i=(links.Count-1); i>=0; i--) {
				lnk = (DataLinker) links[i];
				if (lnk != null) {
					if ((lnk.Source == aObject) || (lnk.Destination == aObject)) {
						lnk.Disconnect();
						links.RemoveAt(i);
					}
				}
			}
		}

		/// <summary>
		/// Removes all bindings for particular object
		/// </summary>
		/// <param name="aLink">
		/// Object removed from bindings <see cref="System.Data.Bindings.DataLinker"/>
		/// </param>
		public static void Unbind (DataLinker aObject)
		{
			if (aObject == null)
				return;
			for (int i=0; i<links.Count; i++) {
				if (aObject == links[i]) {
					links.RemoveAt (i);
					aObject.Disconnect();
					aObject = null;
					return;
				}
			}
		}
	}
}
