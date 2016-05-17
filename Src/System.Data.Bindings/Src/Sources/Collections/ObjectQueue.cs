// ObjectQueue.cs - Field Attribute to assign additional information for Gtk#Databindings
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
using System.Collections.Generic;

namespace System.Data.Bindings.Collections
{
	/// <summary>
	/// Queue which stacks objects which will Get/Post Request for
	/// data transferal
	/// </summary>
	public class ObjectQueue
	{
//		private ArrayList queue = new ArrayList();
		private List<WeakReference> queue = new List<WeakReference>();
		
		/// <summary>
		/// Returns queue object count
		/// </summary>
		public int Count {
			get { return (queue.Count); }
		}
		
		private WeakReference current = new WeakReference (null);
		/// <summary>
		/// Returns current object
		/// </summary>
		public object Current {
			get { return (current.Target); }
			set {
				if (value == null) {
					if (Count > 0)
						current.Target = Dequeue();
					else
						current.Target = null;
				}
				else {
					if (current.Target != value) {
						if (current.Target == null)
							current.Target = value;
						else
							Enqueue (value);
					}
				}
			}
		}
		
		/// <summary>
		/// Adds object to queue list
		/// </summary>
		/// <param name="aObject">
		/// Object to be add to queue <see cref="System.Object"/>
		/// </param>
		/// <remarks>
		/// If objects already exists in queue nothing will happen
		/// </remarks>
		private void Enqueue (object aObject)
		{
			if (aObject == null)
				return;
			if (queue == null) {
				System.Console.WriteLine("Queue restoration?");
//				queue = new ArrayList();
				queue = new List<WeakReference>();
			}
			foreach (WeakReference r in queue)
				if (r.Target == aObject)
					return;
			queue.Add (new WeakReference (aObject));
		}
		
		/// <summary>
		/// Dequeues next object in queue and returns it instance
		/// </summary>
		/// <returns>
		/// Object next in list <see cref="System.Object"/>
		/// </returns>
		private object Dequeue()
		{
			if (queue.Count == 0)
				return (null);
			object obj = (queue[0] as WeakReference).Target;
			(queue[0] as WeakReference).Target = null;
			queue[0] = null;
			queue.RemoveAt (0);
			return (obj);
		}

		/// <summary>
		/// Creates ObjectQueue
		/// </summary>
		public ObjectQueue()
		{
		}
	}
}
