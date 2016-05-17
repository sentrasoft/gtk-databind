// DataSourceController.cs - DataSourceController implementation for Gtk#Databindings
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
using System.Data.Bindings.Collections;
using System.Data.Bindings.DebugInformation;

namespace System.Data.Bindings
{
	/// <summary>
	/// Takes control over the updates for all classes that don't support this functionality by
	/// default. If DataSource connected is inherited from the Observeable or contains IObserveable
	/// interface memebers, this data class will automatically avoid the registry and handle complete
	/// data troughput by it self
	/// </summary>
	public static class DataSourceController
	{
		private static System.Int64 id = 0;
//		private static ArrayList adaptorList = new ArrayList();
//		private static ArrayList boundaryadaptorList = new ArrayList();
//		private static ArrayList datasources = new ArrayList();
		private static List<WeakReference> adaptorList = new List<WeakReference>();
		private static List<WeakReference> boundaryadaptorList = new List<WeakReference>();
		private static List<DataSourceInfo> datasources = new List<DataSourceInfo>();
		private static ObjectQueue getQueue = new ObjectQueue();
		private static ObjectQueue postQueue = new ObjectQueue();
		
		/// <summary>
		/// Defines default apply method for new DataSources that don't make explicit choice
		/// </summary>
		public static EApplyMethod DefaultApplyMethod = EApplyMethod.Instant;
		
		/// <summary>
		/// Returns registered DataSource count 
		/// </summary>
		public static int Count {
			get { return (datasources.Count); }
		}
		
		/// <summary>
		/// Returns number of active adaptors
		/// </summary>
		public static int AdaptorCount {
			get { return (adaptorList.Count); }
		}

		/// <summary>
		/// Assigns id to adaptor
		/// </summary>
		/// <returns>
		/// Adaptor Id number <see cref="System.Int32"/>
		/// </returns>
		public static System.Int64 AssignId()
		{
			id++;
			return (id);
		}

		/// <summary>
		/// Counts Adaptors connected directly to some object
		/// </summary>
		/// <param name="aObject">
		/// Object which connections are counted here <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// Number of connections <see cref="System.Int32"/>
		/// </returns>
		public static int CountDataSourceOwners (object aObject)
		{
			int i = 0;
			lock (adaptorList)
			{
				foreach (WeakReference wr in adaptorList)
					if (wr.Target != null)
						if ((wr.Target as IAdaptor).Target == aObject)
							i++;
			}
			lock (boundaryadaptorList)
			{
				foreach (WeakReference wr in boundaryadaptorList)
					if (wr.Target != null)
						if ((wr.Target as IAdaptor).Target == aObject)
							i++;
			}
			return (i);
		}
		
		/// <summary>
		/// Adds Adaptor in adaptor list as a WeakReference 
		/// </summary>
		public static void AddAdaptor (IAdaptor aAdaptor)
		{
			if (aAdaptor == null)
				return;
			WeakReference wr = new WeakReference (aAdaptor);
			// Separate boundary and non-boundary for speed
			if (aAdaptor.IsBoundaryAdaptor == true)
				boundaryadaptorList.Add (wr);
			else
				adaptorList.Add (wr);
		}
		
		/// <summary>
		/// Rechecks all inherited controls for their state
		/// </summary>
		public static void RecheckStateOfInheritedControls()
		{
			int i;
			if (AdaptorCount <= 0)
				return;
			// Separate boundary and non-boundary for speed
			for (i=0; i<AdaptorCount; i++) {
				if (adaptorList[i] == null)
					continue;
				if (((WeakReference) adaptorList[i]).Target == null)
					continue;
				if (TypeValidator.IsCompatible(((WeakReference) adaptorList[i]).Target.GetType(), typeof (Adaptor)) == false)
					continue;
				Adaptor adaptor = (Adaptor) ((WeakReference) adaptorList[i]).Target;
				if (adaptor.Control != null)
					if (adaptor.ControlAdaptor != null)
						adaptor.ControlAdaptor.CheckControlState();
				adaptor = null;
			}
		}

		/// <summary>
		/// Removes WeakReference for Adaptor from the list
		/// </summary>
		public static void RemoveAdaptor (IAdaptor aAdaptor)
		{
			int i;
			if ((aAdaptor == null) || (AdaptorCount <= 0))
				return;
			// Separate boundary and non-boundary for speed
			if (aAdaptor.IsBoundaryAdaptor == true) {
				for (i=0; i<boundaryadaptorList.Count; i++)
					lock(boundaryadaptorList) {
						if (((WeakReference) boundaryadaptorList[i]).Target == aAdaptor) {
								((WeakReference) boundaryadaptorList[i]).Target = null;
								boundaryadaptorList[i] = null;
								boundaryadaptorList.RemoveAt(i);
							return;
						}
					}
			}
			else
				for (i=0; i<AdaptorCount; i++)
					lock(adaptorList) {
						if (((WeakReference) adaptorList[i]).Target == aAdaptor) {
								((WeakReference) adaptorList[i]).Target = null;
								adaptorList[i] = null;
								adaptorList.RemoveAt(i);
							return;
						}
					}
		}

		/// <summary>
		/// Auto connects properties with controls based on the same name
		///
		/// AutoConnect() should be called after DataSource is specified. After this
		/// only special bindings should be specified
		/// </summary>
		public static void AutoConnect (IAdaptor aAdaptor)
		{
			if (aAdaptor != null)
				aAdaptor.AutoConnect();
		}
		
		/// <summary>
		/// Sets object state to changed and calls for update of controls after posting stops
		/// </summary>
		public static void CallChangedFor (object aObj)
		{
			IObserveable observer = GetInfoFor (aObj);
			if (observer == null)
				return;
			
			observer.HasChanged = true;
		}
		
		/// <summary>
		/// Threadsafe index get, just assign null as ReferenceTarget if you don't need specific Target
		/// </summary>
		public static IObserveable GetInfo (int aIdx, object aReferenceTarget)
		{
			if ((aIdx >= 0) && (aIdx < Count)) {
				DataSourceInfo res = (DataSourceInfo) datasources[aIdx];
				if (aReferenceTarget != null)
					if (aReferenceTarget == res.Target)
						return (res);
					else
						return (GetInfoFor (aReferenceTarget));
				else
					return (res);
			}
			return (null);
		}

		/// <summary>
		/// Threadsafe index get, but not SAFE way
		/// </summary>
		public static IObserveable GetInfo (int aIdx)
		{
			return (GetInfo (aIdx, null));
		}

		/// <summary>
		/// Gets IObserveable for the DataSource
		/// </summary>
		public static IObserveable GetInfoFor (object aDataSource)
		{
			if (aDataSource is IDataAdaptor)
				return (GetInfoFor ((aDataSource as IDataAdaptor).FinalTarget));
			
			if (aDataSource == null)
				return (null);
				
			if (aDataSource is IObserveable) 
				return ((IObserveable) aDataSource);
				
			foreach (DataSourceInfo ds in datasources)
				if (ds.Target == aDataSource)
					return (ds);
			return (null);
		}
		
		/// <summary>
		/// Gets ApplyMethod for the DataSource
		/// </summary>
		public static EApplyMethod DataSourceGetApplyMethod (object aDataSource)
		{
			if (aDataSource == null)
				return (EApplyMethod.Invalid);

			if (aDataSource is IDataAdaptor)
				return (DataSourceGetApplyMethod ((aDataSource as IDataAdaptor).FinalTarget));
				
			if (aDataSource is IObserveable)
				return ((aDataSource as IObserveable).ApplyMethod);
				
			foreach (DataSourceInfo ds in datasources)
				if (ds.Target == aDataSource)
					return (ds.ApplyMethod);
			return (EApplyMethod.Invalid);
		}
		
		/// <summary>
		/// Sets ApplyMethod for the DataSource
		/// </summary>
		public static bool DataSourceSetApplyMethod (object aDataSource, EApplyMethod aMethod)
		{
			if (aDataSource == null)
				return (false);

			if (aDataSource is IDataAdaptor)
				return (DataSourceSetApplyMethod ((aDataSource as IDataAdaptor).FinalTarget, aMethod));
				
			if (aDataSource is IObserveable) {
				(aDataSource as IObserveable).ApplyMethod = aMethod;
				return (true);
			}
				
			foreach (DataSourceInfo ds in datasources)
				if (ds.Target == aDataSource) {
					ds.ApplyMethod = aMethod;
					return (true);
				}
			return (false);
		}
		
		/// <summary>
		/// Resolves if update is needed in some occasion
		/// </summary>
		public static bool DataSourceNeedsUpdateOn (object aDataSource, EApplyMethod aMethod)
		{
			return ((int) DataSourceGetApplyMethod(aDataSource) >= (int) aMethod);
		}
		
		/// <summary>
		/// Adds object to registry or connects events with Adaptor if object is 
		/// supporting interface IObserveable
		///
		/// If DataSource is IObserveable then it will be bypassing registry all the time
		/// </summary>
		public static bool Add (object aDataSource, IAdaptor aAdaptor)
		{
			return (Add (aDataSource, aAdaptor, DefaultApplyMethod));
		}

		/// <summary>
		/// Adds object to registry or connects events with Adaptor if object is 
		/// supporting interface IObserveable
		///
		/// If DataSource is IObserveable then it will be bypassing registry all the time
		/// </summary>
		public static bool Add (object aDataSource, IAdaptor aAdaptor, EApplyMethod aInstant)
		{
			if (aDataSource is IDataAdaptor)
				return (Add ((aDataSource as IDataAdaptor).FinalTarget, aAdaptor, aInstant));
			if (aDataSource is IObserveable) {
				// If target is IObserveable then there's no need to enter it into the registry
				// it can handle all on its own, so connection is made to him directly
				if (aAdaptor.IsBoundaryAdaptor == false) {
					(aDataSource as IObserveable).PostRequested += aAdaptor.PostMethod;
					(aDataSource as IObserveable).DataChanged += aAdaptor.AdapteeDataChanged;
				}
				else
					(aDataSource as IObserveable).DataChanged += aAdaptor.BoundaryAdapteeDataChanged;
			}
			else {
				IObserveable ob = GetInfoFor(aDataSource);
				if (ob != null) {
					DataSourceInfo ds = (DataSourceInfo) ob;
					if (aAdaptor.IsBoundaryAdaptor == false) {
						ds.PostRequested += aAdaptor.PostMethod;
						ds.DataChanged += aAdaptor.AdapteeDataChanged;
					}
					else
						ds.DataChanged += aAdaptor.BoundaryAdapteeDataChanged;
					// Count direct adaptors only, ignore indirect
					if ((aAdaptor.Target is IAdaptor) == false)
						ds.RefCount += 1;
					ds = null;
					return (true);
				}
				DataSourceInfo nds = new DataSourceInfo (aDataSource, aInstant);
				if (aAdaptor.IsBoundaryAdaptor == false) {
					nds.PostRequested += aAdaptor.PostMethod;
					nds.DataChanged += aAdaptor.AdapteeDataChanged;
				}
				else
					nds.DataChanged += aAdaptor.BoundaryAdapteeDataChanged;
				datasources.Add (nds);
				nds = null;
			}
			return (true);
		}

		/// <summary>
		/// Either removes object from central registry or disconnects Adaptor from the 
		/// DataSource if object is supporting interface IObserveable
		///
		/// If DataSource is IObserveable then it was bypassing registry all along
		/// </summary>
		public static void Remove (object aDataSource, IAdaptor aAdaptor)
		{
			int idx = 0;
			bool found = false;
			
			if (aDataSource is IDataAdaptor) {
				Remove ((aDataSource as IDataAdaptor).FinalTarget, aAdaptor);
				return;
			}
			
			if (aDataSource is IObserveable) {
				// If target is IObserveable then there's no need to enter it into the registry
				// it can handle all on its own, so connection is made to him directly
				if (aAdaptor != null)
					if (aAdaptor.IsBoundaryAdaptor == false) {
						(aDataSource as IObserveable).PostRequested -= aAdaptor.PostMethod;
						(aDataSource as IObserveable).DataChanged -= aAdaptor.AdapteeDataChanged;
					}
					else
						(aDataSource as IObserveable).DataChanged -= aAdaptor.BoundaryAdapteeDataChanged;
			}
			else {
				foreach (DataSourceInfo ds in datasources)
					if (ds.Target == aDataSource) {
						found = true;
						break;
					}
					else
						idx++;
				if (found == true) {
					DataSourceInfo ds = (DataSourceInfo) datasources[idx];
					// Count direct adaptors only, ignore indirect
					if ((aAdaptor.Target is IAdaptor) == false)
						ds.RefCount--;
					if (ds != null)
						if (ds.RefCount <= 0) {
							if (aAdaptor != null) {
								if (aAdaptor.IsBoundaryAdaptor == false) {
									ds.PostRequested -= aAdaptor.PostMethod;
									ds.DataChanged -= aAdaptor.AdapteeDataChanged;
								}
								else
									ds.DataChanged -= aAdaptor.BoundaryAdapteeDataChanged;
							}
							ds.Target = null;
							datasources.RemoveAt (idx);
						}
					ds = null;
				}
			}
		}

		/// <summary>
		/// Calls PostRequest for the datasource, and which invokes the action
		/// where all Adaptors connected start posting data back to the DataSource
		///
		/// If DataSource is IObserveable then this request is redirected directly to
		/// the DataSource, otherwise it is redirected to central registry where all
		/// information needed for this to happen' is collected
		/// </summary>
		public static void PostRequest (object aDataSource)
		{
			if (aDataSource == null)
				return;

			postQueue.Current = aDataSource;
			if (postQueue.Current != aDataSource)
				return;
			
			if (aDataSource is IObserveable) {
				// If target is IObserveable then there's no need to enter it into the registry
				// it can handle all on its own, so connection is made to him directly
				(aDataSource as IObserveable).PostRequest();
			}
			else {
				// Since DataSource is a stupid object registry has to handle his bindings and connections
				DataSourceInfo ds = (DataSourceInfo) GetInfoFor (aDataSource);
				if (ds != null) {
					ds.PostRequest();
					ds = null;
				}
			}
			postQueue.Current = null;
			if (postQueue.Current != null)
				PostRequest (postQueue.Current);
		}
		
		/// <summary>
		/// Calls GetRequest for the datasource, and which invokes the action
		/// where all Adaptors connected start posting data back to the DataSource
		///
		/// If DataSource is IObserveable then this request is redirected directly to
		/// the DataSource, otherwise it is redirected to central registry where all
		/// information needed for this to happen' is collected
		/// </summary>
		public static void GetRequest (object aDataSource)
		{
			if (aDataSource == null)
				return;

			getQueue.Current = aDataSource;
			if (getQueue.Current != aDataSource)
				return;
			
			// Check if this is adaptor we're asking about 
			if (TypeValidator.IsCompatible(aDataSource.GetType(), typeof(Adaptor)) == true) {
				aDataSource = (aDataSource as Adaptor).FinalTarget;
				if (aDataSource == null)
					return;
			}

			if (aDataSource is IObserveable) {
				// If target is IObserveable then there's no need to enter it into the registry
				// it can handle all on its own, so connection is made to him directly
				(aDataSource as IObserveable).GetRequest();
			}
			else {
				// Since DataSource is a stupid object registry has to handle his bindings and connections
				DataSourceInfo ds = (DataSourceInfo) GetInfoFor (aDataSource);
				if (ds != null) { 
					GetRequestForInfo (ds);
					ds = null;
				}
			}
			getQueue.Current = null;
			if (getQueue.Current != null)
				GetRequest (getQueue.Current);
		}
		
		/// <summary>
		/// Usefull if one does want to provide caching method for geting updates and avoid discovery each time
		/// </summary>
		public static void GetRequestForInfo (DataSourceInfo aInfo)
		{
			if (aInfo == null)
				return;
			aInfo.GetRequest();
		}
	}
}
