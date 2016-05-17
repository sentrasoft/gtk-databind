// DataLinker.cs - Field Attribute to assign additional information for Gtk#Databindings
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
	/// Links two properties together and serves as data pipeline
	/// </summary>
	public class DataLinker
	{
		private IAdaptor source = null;
		private IAdaptor destination = null;

		private bool active = true;
		/// <summary>
		/// Defines if Linker is active or not
		/// </summary>
		public bool Active {
			get { return (active); }
			set { active = value; }
		}

		private EBindDirection direction = EBindDirection.TwoWay;
		/// <summary>
		/// Specifies direction in which data is transfered
		/// </summary>
		public EBindDirection Direction {
			get { return (direction); }
			set { direction = value; }
		}
		
		/// <summary>
		/// Returns object specified as first datasource
		/// </summary>
		public object Source {
			get {
				if (source != null)
					return (source.Target);
				return (null);
			}
		}

		/// <summary>
		/// Returns property name for first datasource
		/// </summary>
		public string SourceProperty {
			get {
				if (source != null)
					return (source.Mappings);
				return ("");
			}
		}
		
		/// <summary>
		/// Returns object specified as second datasource
		/// </summary>
		public object Destination {
			get { 
				if (destination != null)
					return (destination.Target); 
				return (null);
			}
		}

		/// <summary>
		/// Returns property name for first datasource
		/// </summary>
		public string DestinationProperty {
			get {
				if (destination != null)
					return (destination.Mappings);
				return ("");
			}
		}
		
		/// <summary>
		/// Disconnects adaptors and objects for this link
		/// </summary>
		public void Disconnect()
		{
			source.Disconnect();
			source = null;
			destination.Disconnect();
			destination = null;
		}
		
		/// <summary>
		/// Notifies all connected parties about new Target
		///
		/// The only ones connected here are ControlAdaptor types which took care
		/// of the controls
		/// </summary>
		/// <param name="aAdaptor">
		/// Calling adaptor <see cref="IAdaptor"/>
		/// </param>
		public void SourceTargetChanged (IAdaptor aAdaptor)
		{
			if (Active == false)
				return;
			if (source.FinalTarget != null) {
				// Check if this Adaptor is optimal for this type or remake it
				IAdaptorSelector sel = AdaptorSelector.GetCorrectAdaptor(aAdaptor.FinalTarget);
				if (source.GetType() != sel.GetAdaptorType()) {
					IAdaptor newsrc = sel.CreateAdaptor();
					newsrc.Target = source.Target;
					newsrc.Mappings = source.Mappings;
					newsrc.DataChanged += SourceDataChanged;
					newsrc.TargetChanged += SourceTargetChanged;
					source.Disconnect();
					source = newsrc;
				}
			}
			TransferData (source, destination);
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
		public virtual void SourceDataChanged (object aSender)
		{
			if (Active == false)
				return;
			TransferData (source, destination);
		}
		
		/// <summary>
		/// Notifies all connected parties about new Target
		///
		/// The only ones connected here are ControlAdaptor types which took care
		/// of the controls
		/// </summary>
		/// <param name="aAdaptor">
		/// Calling adaptor <see cref="IAdaptor"/>
		/// </param>
		public void DestinationTargetChanged (IAdaptor aAdaptor)
		{
			if (Active == false)
				return;
			if (destination.FinalTarget != null) {
				// Check if this Adaptor is optimal for this type or remake it
				IAdaptorSelector sel = AdaptorSelector.GetCorrectAdaptor(aAdaptor.FinalTarget);
				if (destination.GetType() != sel.GetAdaptorType()) {
					IAdaptor newdest = sel.CreateAdaptor();
					newdest.Target = destination.Target;
					newdest.Mappings = destination.Mappings;
					newdest.DataChanged += DestinationDataChanged;
					newdest.TargetChanged += DestinationTargetChanged;
					destination.Disconnect();
					destination = newdest;
				}
			}
			TransferData (source, destination);
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
		public virtual void DestinationDataChanged (object aSender)
		{
			if (Active == false)
				return;
			if (Direction == EBindDirection.Simple)
				return;
			TransferData (destination, source);
		}

		/// <summary>
		/// Transfers data between source and destination object
		/// </summary>
		/// <param name="aSrc">
		/// Object where source data resides <see cref="IAdaptor"/>
		/// </param>
		/// <param name="aDest">
		/// Object which will get new data <see cref="IAdaptor"/>
		/// </param>
		public void TransferData (IAdaptor aSrc, IAdaptor aDest)
		{
			if ((aSrc.FinalTarget == null) || (aDest.FinalTarget == null))
				return;
			if ((aSrc.Mappings == "") || (aDest.Mappings == ""))
				return;
			if ((aSrc.FinalTarget == aDest.FinalTarget) && (aSrc.Mappings == aDest.Mappings))
				return;
			try {
				aDest.ExecuteUserMethod (delegate {
					aDest.SetDefaultMappingValue (aSrc.GetDefaultMappingValue());
				});
			}
			catch {
				throw new Exception ("Error transfering data in DataLinker");
			}
		}
		
		private DataLinker()
		{
		}

		internal DataLinker (EBindDirection aDirection, object aObj1, string aPropName1, object aObj2, string aPropName2)
		{
			active = false;
			source = new Adaptor();
			destination = new Adaptor();
			source.DataChanged += SourceDataChanged;
			source.TargetChanged += SourceTargetChanged;
			destination.DataChanged += DestinationDataChanged;
			destination.TargetChanged += DestinationTargetChanged;
			source.Target = aObj1;
			destination.Target = aObj2;
			source.Mappings = aPropName1;
			destination.Mappings = aPropName2;
			active = true;
			TransferData (source, destination);
		}
		
		/// <summary>
		/// Calls disconnect()
		/// </summary>
		~DataLinker()
		{
			Disconnect();
		}
	}
}
