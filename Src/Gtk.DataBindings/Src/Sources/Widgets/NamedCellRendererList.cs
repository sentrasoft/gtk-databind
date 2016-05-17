// NamedCellRendererList.cs - NamedCellRendererList implementation for Gtk#Databindings
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
using Gtk;

namespace Gtk.DataBindings
{
	/// <summary>
	/// Provides named CellRenderers, which can be accessed trough mapping property
	/// of the treeview
	/// </summary>
	/// <remarks>
	/// CellRenderers must be created prior to mapping, otherwise program will
	/// throw Exception
	/// </remarks>
	public class NamedCellRendererList
	{
		/// <summary>
		/// Simple class defining the NamedCellRenderer, providing access
		/// to its name and object
		/// </summary>
		public class NamedCellRenderer
		{
			/// <value>
			/// Name of this CellRenderer
			/// </value>
			public string Name;
			
			/// <value>
			/// CellRenderer used for this name
			/// </value>
			public CellRenderer Renderer;
			
			/// <value>
			/// Arguments for this CellRenderer
			/// </value>
			public string Arg;
			
			/// <summary>
			/// Disconnects CellRenderer to help GC do his job
			/// </summary>
			public void Disconnect()
			{
				Name = "";
				Renderer = null;
				Arg = "";
			}
			
			/// <value>
			/// Creates cell renderer based on parameters
			/// </value>
			/// <param name="aName">
			/// Name of created cell renderer <see cref="System.String"/>
			/// </param>
			/// <param name="aRenderer">
			/// Cell renderer used with this description <see cref="Gtk.CellRenderer"/>
			/// </param>
			/// <param name="aArg">
			/// Arguments passed with cell renderer <see cref="System.String"/>
			/// </param>
			public NamedCellRenderer(string aName, CellRenderer aRenderer, string aArg)
			{
				Name = aName;
				Renderer = aRenderer;
				Arg = aArg;
			}
			
			/// <value>
			/// Disconnects and destroys NamedCellRenderer
			/// </value>
			~NamedCellRenderer()
			{
				Disconnect();
			}
		}
		
		private ArrayList rendererlist = null;
		
		/// <summary>
		/// Returns NamedCellRenderer Count in this list 
		/// </summary>
		public int Count {
			get {
				if (rendererlist == null)
					return (0);
				return (rendererlist.Count);
			}
		}
		
		/// <summary>
		/// Returns NamedCellRenderer at the given index
		/// </summary>
		/// <param name="aIdx">
		/// Index of cell renderer <see cref="System.Int32"/>
		/// </param>
		public NamedCellRenderer this [int aIdx] {
			get {
				if (rendererlist == null)
					throw new ExceptionNoRendererAssignedYet();
				return ((NamedCellRenderer) rendererlist[aIdx]); 
			}
		}
		
		/// <summary>
		/// Returns CellRenderer that was named as searched
		/// </summary>
		/// <param name="aName">
		/// Name of cell renderer <see cref="System.Int32"/>
		/// </param>
		public NamedCellRenderer this [string aName] {
			get {
				if (rendererlist == null)
					return (null);
					
				foreach (NamedCellRenderer rndr in rendererlist)
					if (rndr.Name == aName)
						return (rndr);
				return (null);
			}
		}
		
		/// <summary>
		/// Adds NamedCellRenderer to the list
		/// </summary>
		/// <param name="aRenderer">
		/// Cell renderer to be added into list <see cref="NamedCellRenderer"/>
		/// </param>
		/// <remarks>
		/// Throws Exception is Renderer with the same name already exists in this list
		/// or if NamedCellRenderer is invalid
		/// </remarks>
		public void Add (NamedCellRenderer aRenderer)
		{
			if (aRenderer == null)
				throw new ExceptionAddingInvalidCellRenderer();
			if (this[aRenderer.Name] != null)
				throw new ExceptionAddingAlreadyExistingCellRenderer (aRenderer.Name);
			if (rendererlist == null)
				rendererlist = new ArrayList();
			rendererlist.Add (aRenderer);
		}
		
		/// <summary>
		/// Adds new NamedCellRenderer with aName and aRenderer as params to the list
		/// </summary>
		/// <param name="aName">
		/// Name of new cell renderer <see cref="System.String"/>
		/// </param>
		/// <param name="aRenderer">
		/// Cell renderer to use with description <see cref="CellRenderer"/>
		/// </param>
		/// <param name="aArg">
		/// Arguments used with this cell renderer <see cref="System.String"/>
		/// </param>
		/// <remarks>
		/// Throws Exception is Renderer with the same name already exists in this list
		/// or if NamedCellRenderer is invalid
		/// </remarks>
		public void Add (string aName, CellRenderer aRenderer, string aArg)
		{
			if ((aRenderer == null) || (aName == ""))
				throw new ExceptionAddingInvalidCellRenderer();
			Add (new NamedCellRenderer (aName, aRenderer, aArg));
		}
		
		/// <summary>
		/// Removes CellRenderer with given name from the list
		/// </summary>
		/// <param name="aName">
		/// Remove cell renderer with specified name <see cref="System.String"/>
		/// </param>
		public void Remove (string aName)
		{
			if (rendererlist == null)
				return;
				
			NamedCellRenderer del = null;
			foreach (NamedCellRenderer rndr in rendererlist)
				if (rndr.Name == aName) {
					del = rndr;
					break;
				}
			
			if (del != null) {
				del.Disconnect();
				rendererlist.Remove (del);
			}

			if (rendererlist.Count == 0)
				Clear();
		}

		/// <summary>
		/// Clears and disconnects everything
		/// </summary>
		/// <remarks>
		/// Doesn't destroy original CellRenderers, this are still active just not named anymore
		/// </remarks>
		public void Clear()
		{
			if (rendererlist != null) {
				foreach (NamedCellRenderer rndr in rendererlist)
					rndr.Disconnect();
				rendererlist.Clear();
			}
			rendererlist = null;
		}
		
		/// <summary>
		/// Clears and destroys NamedCellRendererList
		/// </summary>
		~NamedCellRendererList()
		{
			Clear();
		}
	}
}
