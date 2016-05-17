//GtkAdaptor.cs - Description
//
//Author: m. <ml@arsis.net>
//
//Copyright (c) 2008 m.
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
//
//

using System;
using System.Data.Bindings;
using System.Data.Bindings.DebugInformation;

namespace Gtk.DataBindings
{
	/// <summary>
	/// Overrides System.Data.Bindings.Adaptor to make it Gtk compatible 
	/// </summary>
	public class GtkAdaptor : System.Data.Bindings.Adaptor
	{
		/// <value>
		/// Returns Gtk.Widget typecast of Adapter.Control 
		/// </value>
		public Gtk.Widget GtkWidget {
			get { return ((Gtk.Widget) Control); }
		}
		
		/// <summary>
		/// Reposts renewal to all children controls
		/// </summary>
		public override void RepostRenewToAllChildren (object aControl, EActionType aAction)
		{
			if (aControl != null)
				if (aControl is IAdaptableContainer)
					if ((aControl as IAdaptableContainer).InheritedDataSource == true)
						if ((aControl as IAdaptableContainer).Adaptor != null)
							(aControl as IAdaptableContainer).Adaptor.SendAdaptorMessage (null, aAction); 

			if (aControl is Gtk.Container)
				foreach (Gtk.Widget wg in (aControl as Gtk.Container).Children) 
					if (wg != null)
						RepostRenewToAllChildren (wg, aAction);
		}

		/// <summary>
		/// Notifies all connected parties and then  proceeds to global dispatch
		/// loop which sends this same message to all Adaptors who have 
		/// InheritedDataSource = true and are part of this Adaptors container.
		///
		/// Example: WindowAdaptor having set DataSource is automatically dispatching 
		/// the same messages about changes to its child controls that have not set
		/// their own DataSource and have InheritedDataSource = true
		/// </summary>
		/// <param name="aSender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		public override void AdapteeDataChanged (object aSender)
		{
			Gtk.Application.Invoke (delegate {
				base.AdapteeDataChanged (aSender);
			});
		}
		
		/// <summary>
		/// Executes anonymous delegate event in specialized terms. For example
		/// all data transfers in Gtk.DataBindings have to be executed trough
		/// Gtk.Invoke
		/// </summary>
		/// <param name="aEvent">
		/// Event to be executed <see cref="AnonymousDelegateEvent"/>
		/// </param>
		public override void ExecuteUserMethod (AnonymousDelegateEvent aEvent)
		{
			Gtk.Application.Invoke (delegate {
				aEvent();
			});
		}
		
		/// <summary>
		/// Creates GtkAdaptor
		/// </summary>
		/// <remarks>
		/// Used to create pointer like Adaptor objects
		/// </remarks>
		public GtkAdaptor()
			: base()
		{
		}
		
		/// <summary>
		/// Creates GtkAdaptor
		/// </summary>
		/// <param name="aIsBoundary">
		/// Set true if creating Boundary adaptor <see cref="System.Boolean"/>
		/// </param>
		/// <param name="aControlAdaptor">
		/// ControlAdaptor which will be controling this one <see cref="ControlAdaptor"/>
		/// </param>
		/// <param name="aControl">
		/// Control which will bind this Adaptor <see cref="System.Object"/>
		/// </param>
		/// <param name="aSingleMappingOnly">
		/// Set true if Control allows single mapping <see cref="System.Boolean"/>
		/// </param>
		public GtkAdaptor(bool aIsBoundary, ControlAdaptor aControlAdaptor, object aControl, bool aSingleMappingOnly)
			: base (aIsBoundary, aControlAdaptor, aControl, aSingleMappingOnly)
		{
		}
	}
}
