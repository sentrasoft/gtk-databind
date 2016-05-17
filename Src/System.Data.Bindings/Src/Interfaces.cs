// Interfaces.cs - Various Interfaces and structures for Gtk#Databindings
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
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace System.Data.Bindings
{
	/// <summary>
	/// Empty interface to describe always visible widgets
	/// </summary>
	public interface ILayoutWidget
	{
	}
	
	/// <summary>
	/// Specifies classes which support automatic resolving of their
	/// title trough processing of attributes on properties
	/// </summary>
	public interface IAutomaticTitle
	{
		/// <value>
		/// Specifies if title should be automaticaly resolved trough
		/// attribute processing based on mapping
		/// </value>
		bool AutomaticTitle { get; set; }
	}
	
	/// <summary>
	/// Defines strictness type
	/// </summary>
	public enum StrictnessType
	{
		/// <summary>
		/// Specifies non strict actions
		/// </summary>
		NonStrict,
		/// <summary>
		/// Avoid any actions out of bounds
		/// </summary>
		RefuseAction,
		/// <summary>
		/// Any out of bounds action throws exception
		/// </summary>
		ThrowException
	}
	
	/// <summary>
	/// Provides base interface for client lists
	/// </summary>
	public interface IObservableListClient<T> : INotifyPropertyChanged, IListEvents, IDisconnectable
	{
		/// <value>
		/// Specifies parent view for this list
		/// </value>
		IList<T> ParentView { get; }
		/// <summary>
		/// Returns master list
		/// </summary>
		/// <returns>
		/// Master list <see cref="IList"/>
		/// </returns>
		IList<T> GetMasterList();
		/// <summary>
		/// Calls clear on parent view
		/// </summary>
		void ClearParent();
		/// <summary>
		/// Calls clear on master view
		/// </summary>
		void ClearMaster();
	}
	
	/// <summary>
	/// Provides base interface for client lists
	/// </summary>
	public interface IObservableListClient : INotifyPropertyChanged, IListEvents, IDisconnectable
	{
		/// <value>
		/// Specifies parent view for this list
		/// </value>
		IList ParentView { get; }
		/// <summary>
		/// Returns master list
		/// </summary>
		/// <returns>
		/// Master list <see cref="IList"/>
		/// </returns>
		IList GetMasterList();
		/// <summary>
		/// Calls clear on parent view
		/// </summary>
		void ClearParent();
		/// <summary>
		/// Calls clear on master view
		/// </summary>
		void ClearMaster();
	}
	
	/// <summary>
	/// Specifies filtered element
	/// </summary>
	public interface IFiltered
	{
		/// <value>
		/// Defines if class should act strictly with filter or not
		/// </value>
		StrictnessType Strictness { get; set; }
		/// <summary>
		/// Provides call to refilter everything
		/// </summary>
		void Refilter();
		/// <summary>
		/// Should be called to refilter only visible items with even more strict filter
		/// </summary>
		void FilterMore();
		/// <summary>
		/// Should be called to refilter only invisible items with less strict filter
		/// </summary>
		void FilterLess();
		/// <summary>
		/// Event handler which resolves if particular object is in filter or not
		/// </summary>
		event IsVisibleInFilterEvent IsVisibleInFilter;
	}
	
	/// <summary>
	/// Event sent to see if object is visible or not
	/// </summary>
	public delegate bool IsVisibleInFilterEvent (object aObject);

	/// <summary>
	/// Interface used to notify connected parties about language change
	/// </summary>
	public interface ILanguageTranslator : INotifyPropertyChanged
	{
		/// <value>
		/// Current language, whwnever it is changed it calls for
		/// PropertyChanged ("Current")
		/// </value>
		CultureInfo Current { get; set; }
		/// <summary>
		/// Translates string from default language to destination
		/// </summary>
		/// <param name="aStr">
		/// String which needs translation <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Translated text if language exists or aStr <see cref="System.String"/>
		/// </returns>
		string Translate (string aStr);
	}
	
	/// <summary>
	/// Delegate method without params, useful for anonymous delegates
	/// </summary>
	public delegate void AnonymousDelegateEvent();
	
	/// <summary>
	/// Delegate method for invoking update on controls
	/// </summary>
	/// <param name="aSender">
	/// Sender of this event <see cref="System.Object"/>
	/// </param>
	public delegate void AdapteeDataChangeEvent (object aSender);
	
	/// <summary>
	/// Delegate method whenever target changes
	/// </summary>
	/// <param name="aAdaptor">
	/// Calling adaptor <see cref="IAdaptor"/>
	/// </param>
	public delegate void TargetChangedEvent (IAdaptor aAdaptor);
	
	/// <summary>
	/// Delegate method to notify about data post
	/// </summary>
	public delegate void PostMethodEvent();

	/// <summary>
	/// Delegate method which notifies about change in list
	/// </summary>
	/// <param name="aList">
	/// List being changed <see cref="IList"/>
	/// </param>
//	public delegate void ListChangedEvent (IList aList);

	/// <summary>
	/// Delegate method which notifies about change in list
	/// </summary>
	/// <param name="aList">
	/// List being changed <see cref="System.Object"/>
	/// </param>
	public delegate void ListChangedEvent (object aList);

	/// <summary>
	/// Delegate method which notifies about addition to list
	/// </summary>
	/// <param name="aList">
	/// List being changed <see cref="IList"/>
	/// </param>
	/// <param name="aIdx">
	/// Path to changed item in list <see cref="System.Int32"/>
	/// </param>
	public delegate void ListElementAddedEvent (IList aList, int[] aIdx);

	/// <summary>
	/// Notification about elements in list being sorted
	/// </summary>
	/// <param name="aList">
	/// List being changed <see cref="System.Object"/>
	/// </param>
	/// <param name="aIdx">
	/// Path to changed item in list or null if owner list being sorted
	/// is specified by aObject<see cref="System.Int32"/>
	/// </param>
	public delegate void ElementsInListSortedEvent (object aObject, int[] aIdx);

	/// <summary>
	/// Delegate method which notifies about addition to list
	/// </summary>
	/// <param name="aList">
	/// List being changed <see cref="System.Object"/>
	/// </param>
	/// <param name="aIdx">
	/// Path to changed item in list <see cref="System.Int32"/>
	/// </param>
	public delegate void ElementAddedInListObjectEvent (object aList, int[] aIdx);

	/// <summary>
	/// Delegate method which notifies about addition to list before happening
	/// </summary>
	/// <param name="aList">
	/// List being changed <see cref="IList"/>
	/// </param>
	/// <param name="aObject">
	/// Object being added <see cref="System.Object"/>
	/// </param>
	public delegate void ListBeforeElementAdd (IList aList, object aObject);

	/// <summary>
	/// Delegate method which notifies about addition to list after happening
	/// </summary>
	/// <param name="aList">
	/// List being changed <see cref="IList"/>
	/// </param>
	/// <param name="aObject">
	/// Object being added <see cref="System.Object"/>
	/// </param>
	public delegate void ListAfterElementAdd (IList aList, object aObject);

	/// <summary>
	/// Delegate method which notifies about removal from list
	/// </summary>
	/// <param name="aList">
	/// List being changed <see cref="IList"/>
	/// </param>
	/// <param name="aIdx">
	/// Path to removed element <see cref="System.Int32"/>
	/// </param>
	public delegate void ListElementRemovedEvent (IList aList, int[] aIdx);

	/// <summary>
	/// Delegate method which notifies about removal from list
	/// </summary>
	/// <param name="aList">
	/// List being changed <see cref="System.Object"/>
	/// </param>
	/// <param name="aIdx">
	/// Path to removed element <see cref="System.Int32"/>
	/// </param>
	/// <param name="aObject">
	/// Object being removed <see cref="System.Object"/>
	/// </param>
	public delegate void ElementRemovedFromListObjectEvent (object aList, int[] aIdx, object aObject);

	/// <summary>
	/// Delegate method which notifies about removal from list before happening
	/// </summary>
	/// <param name="aList">
	/// List being changed <see cref="IList"/>
	/// </param>
	/// <param name="aObject">
	/// Object being removed <see cref="System.Object"/>
	/// </param>
	public delegate void ListBeforeElementRemove (IList aList, object aObject);

	/// <summary>
	/// Delegate method which notifies about removal from list after happening
	/// </summary>
	/// <param name="aList">
	/// List being changed <see cref="IList"/>
	/// </param>
	/// <param name="aObject">
	/// Object being removed <see cref="System.Object"/>
	/// </param>
	public delegate void ListAfterElementRemove (IList aList, object aObject);

	/// <summary>
	/// Delegate method which notifies about change in list
	/// </summary>
	/// <param name="aList">
	/// List being changed <see cref="IList"/>
	/// </param>
	/// <param name="aIdx">
	/// Path to changed element <see cref="System.Int32"/>
	/// </param>
	public delegate void ListElementChangedEvent (IList aList, int[] aIdx);

	/// <summary>
	/// Delegate method which notifies about change in list
	/// </summary>
	/// <param name="aList">
	/// List being changed <see cref="System.Object"/>
	/// </param>
	/// <param name="aIdx">
	/// Path to changed element <see cref="System.Int32"/>
	/// </param>
	public delegate void ElementChangedInListObjectEvent (object aList, int[] aIdx);

	/// <summary>
	/// Delegate method which notifies about child in list has changed
	/// </summary>
	/// <param name="aList">
	/// List being changed <see cref="IList"/>
	/// </param>
	/// <param name="aIdx">
	/// Path to changed element <see cref="System.Int32"/>
	/// </param>
	public delegate void ChildChangedEvent (Observeable aObject, int[] aIdx);

	/// <summary>
	/// Event arguments used in custom get and post data delegates
	/// </summary>
	public class CustomDataTransferArgs : EventArgs
	{
		private WeakReference adaptor = null;
		/// <value>
		/// Control adaptor that bounds this connection
		/// </value>
		public ControlAdaptor Adaptor {
			get {
				if (adaptor == null)
					return (null);
				return ((ControlAdaptor) adaptor.Target);
			}
		}
		
		private WeakReference dataSource = null;
		/// <value>
		/// Specifies destination object, but in case if DataSource to widget was 
		/// Adaptor it specifies its FinalTarget
		/// </value>
		public object DataSource {
			get {
				if (dataSource == null)
					return (null);
				return (dataSource.Target);
			}
		}
		
		private EDataDirection direction = EDataDirection.FromControlToDataSource;
		/// <value>
		/// Direction of data flow
		/// </value>
		public EDataDirection Direction {
			get { return (direction); }
		}
		
		private CustomDataTransferArgs()
		{
		}
		
		public CustomDataTransferArgs (ControlAdaptor aAdaptor, object aDataSource, EDataDirection aDirection)
		{
			adaptor = new WeakReference (aAdaptor);
			dataSource = new WeakReference (aDataSource);
			direction = aDirection;
		}
	}
	
	/// <summary>
	/// Delegate method notification about GetData event 
	/// </summary>
	/// <param name="aSender">
	/// Sender of this message <see cref="System.Object"/>
	/// </param>
	/// <param name="aArgs">
	/// Event arguments related to message <see cref="CustomDataTransferArgs"/>
	/// </param>
	public delegate void CustomGetDataEvent (object aSender, CustomDataTransferArgs aArgs);

	/// <summary>
	/// Delegate method notification about PostData event 
	/// </summary>
	/// <param name="aSender">
	/// Sender of this message <see cref="System.Object"/>
	/// </param>
	/// <param name="aArgs">
	/// Event arguments related to message <see cref="CustomDataTransferArgs"/>
	/// </param>
	public delegate void CustomPostDataEvent (object aSender, CustomDataTransferArgs aArgs);
	
	/// <summary>
	/// Event which is triggered to load translated application message.
	/// When connected to System.Triggers.Notificator.AppMessage.OnLoadMsgString
	/// it defines method with which every message string is translated into
	/// visible form to user
	/// </summary>
	/// <param name="aError">
	/// Specify message code trough which message will be loaded <see cref="System.String"/>
	/// </param>
	/// <returns>
	/// Translated message <see cref="System.String"/>
	/// </returns>
	public delegate string LoadMsgStringEvent (string aError);

	/// <summary>
	/// Specifies if widget is editable or not
	/// </summary>
	public interface IEditable
	{
		/// <value>
		/// Sets if widget is editable or not
		/// </value>
		bool Editable { get; set; }
	}
	
	/// <summary>
	/// Provides various message types
	/// </summary>
	public enum AppNotificationType
	{
		/// <summary>
		/// No type
		/// </summary>
		None,
		/// <summary>
		/// Development information
		/// </summary>
		DevelInfo,
		/// <summary>
		/// Custom message type
		/// </summary>
		Custom,
		/// <summary>
		/// Information
		/// </summary>
		Information,
		/// <summary>
		/// Suggestion
		/// </summary>
		Suggestion,
		/// <summary>
		/// Question
		/// </summary>
		Question,
		/// <summary>
		/// Warning
		/// </summary>
		Warning,
		/// <summary>
		/// Error
		/// </summary>
		Error,
		/// <summary>
		/// Critical
		/// </summary>
		Critical
	}
	
	/// <summary>
	/// Possible list reordering
	/// </summary>
	public enum EListReorderable
	{
		/// <summary>
		/// Static
		/// </summary>
		None,
		/// <summary>
		/// Reorderable with drag'n'drop
		/// </summary>
		DragDrop,
		/// <summary>
		/// Is reorderable
		/// </summary>
		Reorderable
	}
	
	/// <summary>
	/// Drag object type
	/// </summary>
	public enum EDragObjectType
	{
		/// <summary>
		/// Drag as objects
		/// </summary>
		Object,
		/// <summary>
		/// Drag as list item
		/// </summary>
		ListItem
	}

	/// <summary>
	/// Defines validity of property access trough cache
	/// </summary>
	public enum EPropertyAccess
	{
		/// <summary>
		/// Access is valid
		/// </summary>
		Valid,
		/// <summary>
		/// Access is conditionaly valid
		/// </summary>
		Conditional,
		/// <summary>
		/// Access is invalid
		/// </summary>
		Invalid
	}
	
	/// <summary>
	/// Defines simple countable objects
	/// </summary>
	public interface ICountable
	{
		/// <value>
		/// Returns count of items
		/// </value>
		int Count { get; }
	}
	
	/// <summary>
	/// Just a failsafe against unwanted writing in software
	/// </summary>
	public enum EReadOnlyMode { 
		/// <summary>
		/// Write allowed
		/// </summary>
		roCanWrite, 
		/// <summary>
		/// Read only
		/// </summary>
		roReadOnly, 
		/// <summary>
		/// Admin can write
		/// </summary>
		roAdminModeWriteOnly 
	}
	
	/// <summary>
	/// Provides state of the list
	/// </summary>
	public enum EListMode {
		/// <summary>
		/// None
		/// </summary>
		lmNone, 
		/// <summary>
		/// List is loading
		/// </summary>
		lmLoading, 
		/// <summary>
		/// List is being created
		/// </summary>
		lmCreating 
	}
	
	/// <summary>
	/// Provides the interface needs for validatable list
	/// </summary>
	public interface IValidatableList : IList
	{
		/// <value>
		/// Set true if List accepts null values
		/// </value>
		bool AcceptNullValues { get; set; }
		/// <value>
		/// List is writable
		/// </value>
		bool CanWrite { get; }
		/// <value>
		/// List is readonly 
		/// </value>
		EReadOnlyMode ReadOnly { get; set; }
		/// <value>
		/// List mode
		/// </value>
		EListMode ListMode { get; set; }
	}

	/// <summary>
	/// Defines common ground for mapped items
	/// </summary>
	public interface IMappedItem
	{
		/// <value>
		/// Type of mapped property
		/// </value>
		System.Type MappedType { get; set; }
		/// <value>
		/// Name of the mapped property
		/// </value>
		string MappedTo { get; set; }
		/// <summary>
		/// Assign value to property
		/// </summary>
		/// <param name="aValue">
		/// Value <see cref="System.Object"/>
		/// </param>
		void AssignValue (object aValue);
	}
	
	/// <summary>
	/// Defines common ground for mapped column items
	/// </summary>
	public interface IMappedColumnItem : IMappedItem
	{
		/// <value>
		/// Index of column
		/// </value>
		int ColumnIndex { get; set; }
		/// <value>
		/// Describes if item is subitem of another
		/// </value>
		bool IsSubItem { get; set; }
		/// <value>
		/// Index of subcolumn
		/// </value>
		int SubColumnIndex { get; set; }
	}
	
	/// <summary>
	/// Events needed to connect to ObserveableList notifications
	/// </summary>
	public interface IListEvents
	{
		/// <summary>
		/// Event called when list gets changed
		/// </summary>
		event ListChangedEvent ListChanged;
		/// <summary>
		/// Event called when element is added to list
		/// </summary>
		event ElementAddedInListObjectEvent ElementAdded;
		/// <summary>
		/// Event called when element in list is changed
		/// </summary>
		event ElementChangedInListObjectEvent ElementChanged;
		/// <summary>
		/// Event called when element in list is removed
		/// </summary>
		event ElementRemovedFromListObjectEvent ElementRemoved;
		/// <summary>
		/// Event called whenever list or any of its children is being sorted
		/// </summary>
		event ElementsInListSortedEvent ElementsSorted;
	}

	/// <summary>
	/// ObserveableList is a wrapper around IList and provides notifications for
	/// all connected controls
	/// </summary>
	public interface IObserveableList : IList, IListEvents
	{
		/// <value>
		/// Resolves value by using path index
		/// </value>
		object this [int[] aIdx] { get; }
		/// <value>
		/// Decides if elements should be checked
		/// </value>
		bool ElementCheckEnabled { get; set; }
		/// <value>
		/// Type of list element stored in list
		/// </value>
		Type ListElementType { get; set; } 
		/// <summary>
		/// Checks if element is valid for list
		/// </summary>
		/// <param name="aObject">
		/// Object to be checked <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if element is valid <see cref="System.Boolean"/>
		/// </returns>
		bool IsValidObject (object aObject);
		/// <summary>
		/// Created list
		/// </summary>
		/// <returns>
		/// List object <see cref="IList"/>
		/// </returns>
		IList CreateList();
		/// <summary>
		/// Method invoked when list child is changed
		/// </summary>
		/// <param name="aObject">
		/// Object being changed <see cref="System.Object"/>
		/// </param>
		/// <param name="aAction">
		/// Change action description <see cref="EListAction"/>
		/// </param>
		/// <param name="aPath">
		/// Path to object in list <see cref="System.Int32"/>
		/// </param>
		void ListChildChanged (object aObject, EListAction aAction, int[] aPath);
	}

	/// <summary>
	/// ObserveableList is a wrapper around IList and provides notifications for
	/// all connected controls
	/// </summary>
	public interface IObservableList : IList, IListEvents
	{
		/// <value>
		/// Resolves value by using path index
		/// </value>
		object this [int[] aIdx] { get; }
		/// <summary>
		/// Created list
		/// </summary>
		/// <returns>
		/// List object <see cref="IList"/>
		/// </returns>
		IList CreateList();
		/// <summary>
		/// Returns default NotifyPropertyChanged handler method
		/// </summary>
		/// <returns>
		/// Default handler method <see cref="PropertyChangedEventHandler"/>
		/// </returns>
		PropertyChangedEventHandler GetDefaultNotifyPropertyChangedHandler();
	}

	/// <summary>
	/// Provides interface for list wrapper around DataTable and DataView
	/// </summary>
	public interface IDbObservableList : IObservableList
	{
		/// <value>
		/// Provides direct access to DataTable (or Table property of DataView)
		/// </value>
		DataTable Table { get; }
	}
	
	/// <summary>
	/// Basic List Action
	/// </summary>
	public enum EListAction {
		/// <summary>
		/// Action add
		/// </summary>
		Add = 1,
		/// <summary>
		/// Action change
		/// </summary>
		Change = 2,
		/// <summary>
		/// Action remove
		/// </summary>
		Remove = 3
	}

	/// <summary>
	/// Specifies direction for data to flow in DataBinder
	/// </summary>
	public enum EBindDirection {
		/// <summary>
		/// Data goes from Source to Destination only
		/// </summary>
		Simple,
		/// <summary>
		/// Data goes from Source to Destination and back
		/// </summary>
		TwoWay
	}
	
	/// <summary>
	/// Defines how property is allowed to be accessed
	/// </summary>
	public enum EReadWrite {
		/// <summary>
		/// None
		/// </summary>
		None,
		/// <summary>
		/// Read only
		/// </summary>
		ReadOnly,
		/// <summary>
		/// Write only
		/// </summary>
		WriteOnly,
		/// <summary>
		/// Read write
		/// </summary>
		ReadWrite
	}
	
	/// <summary>
	/// Direction of Data Flow
	/// </summary>
	public enum EDataDirection {
		/// <summary>
		/// Data from DataSource to control
		/// </summary>
		FromDataSourceToControl,
		/// <summary>
		/// Data from control to DataSource
		/// </summary>
		FromControlToDataSource
	}
	
	/// <summary>
	/// Defines ApplyMethod with which DataSource wishes to communicate 
	/// with Adaptors
	/// 
	/// Instant - is only possible with IObserveable objects, because object
	/// has to provide OnDataChange events
	/// OnLeave is executed every time connected control receives on leave
	/// OnPost is executed manually only
	/// </summary>
	public enum EApplyMethod {
		/// <summary>
		/// Invalid
		/// </summary>
		Invalid = 0,
		/// <summary>
		/// Apply on post
		/// </summary>
		OnPost = 1,
		/// <summary>
		/// Apply on leave
		/// </summary>
		OnLeave = 2,
		/// <summary>
		/// Instant apply
		/// </summary>
		Instant = 3 
	}

	/// <summary>
	/// Used for excluding posting and requesting being activated by another, because
	/// Observeable must know if other is in progress so the Changed() calls can be
	/// ignored
	/// </summary>
	public enum EObserveableState {
		/// <summary>
		/// None
		/// </summary>
		None,
		/// <summary>
		/// Observeable is posting
		/// </summary>
		PostDataInProgress,
		/// <summary>
		/// Observeable is getting
		/// </summary>
		GetDataInProgress
	}
	
	/// <summary>
	/// Adaptor action type
	/// </summary>
	public enum EActionType {
		/// <summary>
		/// Post request
		/// </summary>
		PostRequest,
		/// <summary>
		/// Get request
		/// </summary>
		GetRequest,
		/// <summary>
		/// Renew parents
		/// </summary>
		RenewParents,
		/// <summary>
		/// Renew targets
		/// </summary>
		RenewTargets
	}
	
	/// <summary>
	/// Data change Trigger types
	/// 
	/// ObjectChanged - should be called from inside object when it changes internally
	/// 
	/// UpdateObject - should be called from outside object to specified. If any changes
	/// occur during UpdateObject ObjectChanged notification is queued 
	/// </summary>
	public enum TriggerType
	{
		/// <summary>
		/// Object has changed
		/// </summary>
		ObjectChanged,
		/// <summary>
		/// Update object
		/// </summary>
		UpdateObject
	}

	/// <summary>
	/// Interface which defines disconnectable classes
	/// </summary>
	public interface IDisconnectable
	{
		/// <summary>
		/// Disconnects everything inside the class
		/// </summary>
		void Disconnect();
	}
	
	/// <summary>
	/// Provides interface to any changeable object that is accepting notification
	/// requests.
	/// </summary>
	public interface IChangeable
	{
		/// <summary>
		/// Event called when data is changed
		/// </summary>
		event AdapteeDataChangeEvent DataChanged;
	}

	/// <summary>
	/// Provides interface to any changeable object that is accepting notification
	/// requests.
	/// </summary>
	public interface IChangeableControl
	{
		/// <value>
		/// ControlAdaptor connected to control
		/// </value>
		ControlAdaptor Adaptor { get; }
		/// <summary>
		/// Default event when data in object is changed. It redirects its action
		/// trough its ControlAdaptor. This way all the needed information steps are taken
		/// automatically 
		/// </summary>
		/// <param name="aSender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		void CallAdaptorGetData (object aSender);
		/// <summary>
		/// Default event when data in object is changed
		/// </summary>
		/// <param name="aSender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		void GetDataFromDataSource (object aSender);
	}

	/// <summary>
	/// Provides interface to any changeable object that is accepting notification
	/// requests.
	/// </summary>
	public interface IPostableControl : ICustomPostData
	{
		/// <summary>
		/// Default event when data is needed to transfer from control to object
		/// </summary>
		/// <param name="aSender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		void PutDataToDataSource (object aSender);
	}

	/// <summary>
	/// Base of the every data object. It provides all methods for data to really become alive
	/// Every data class being inherited from this one automatically avoids DataSourceController
	/// registry, because all the functionality needed is provided by the class it self
	/// </summary>
	public interface IObserveable : IChangeable
	{
		/// <value>
		/// Specifies if object is frozen (will post notifications about update or not)
		/// </value>
		bool IsFrozen { get; }
		/// <value>
		/// Specifies if object can execute GetData
		/// </value>
		bool CanGet { get; }
		/// <value>
		/// Specifies if object can execute PostData
		/// </value>
		bool CanPost { get; }
		/// <value>
		/// Currecnt object state
		/// </value>
		EObserveableState State { get; }
		/// <value>
		/// Default apply method of this object
		/// </value>
		EApplyMethod ApplyMethod { get; set; }
		/// <value>
		/// Defines if object has changed or not
		/// </value>
		bool HasChanged { get; set; }
		/// <value>
		/// Defines if object has called about change meanwhile or not
		/// </value>
		bool HasCalledForChange { get; } 
		/// <summary>
		/// Reset call about change
		/// </summary>
		void ResetChangeCallCheckup();
		/// <summary>
		/// Sets state of the object
		/// </summary>
		/// <param name="aState">
		/// State <see cref="EObserveableState"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		bool SetState (EObserveableState aState);
		/// <summary>
		/// Freezes object, all updates are quiet until unfrozen
		/// </summary>
		/// <returns>
		/// Freeze counter <see cref="System.Int32"/>
		/// </returns>
		int Freeze();
		/// <summary>
		/// Unfreezes object, if object has called about change and
		/// freeze counter is 0 then it posts notification about
		/// object being changed
		/// </summary>
		/// <returns>
		/// Freeze counter <see cref="System.Int32"/>
		/// </returns>
		int Unfreeze();
		/// <summary>
		/// Invoked PostRequest
		/// </summary>
		void PostRequest();
		/// <summary>
		/// Invokes GetRequest
		/// </summary>
		void GetRequest();
		/// <summary>
		/// Method called when data changed
		/// </summary>
		/// <param name="aSender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		void AdapteeDataChanged (object aSender);
		/// <summary>
		/// Default method called when child object is changed
		/// </summary>
		/// <param name="aObject">
		/// Object being changed <see cref="IObserveable"/>
		/// </param>
		/// <param name="aPath">
		/// Path to object <see cref="System.Int32"/>
		/// </param>
		void ChildChanged (IObserveable aObject, int[] aPath);
		/// <summary>
		/// Event called on PostRequest
		/// </summary>
		event PostMethodEvent PostRequested;
	}
	
	/// <summary>
	/// Provides interface to any changeable object that is accepting notification
	/// requests.
	/// </summary>
	public interface IChangeableContainerControl : IChangeableControl
	{
		/// <summary>
		/// Event called on target change
		/// </summary>
		event TargetChangedEvent TargetChanged;
	}

	/// <summary>
	/// Any container implementing this interface is automaticaly supporting 
	/// connecting to Adaptor
	/// </summary>
	public interface IBoundedContainer : IChangeableControl
	{
		/// <value>
		/// ControlAdaptor connected to control
		/// </value>
//		ControlAdaptor Adaptor { get; }
		/// <value>
		/// Defines if boundary DataSource is inherited or not
		/// </value>
		bool InheritedBoundaryDataSource { get; set; }
		/// <value>
		/// Boundary DataSource
		/// </value>
		IObserveable BoundaryDataSource { get; set; }
		/// <value>
		/// Property mappings for boundary DataSource
		/// </value>
		string BoundaryMappings { get; set; }
	}
	
	/// <summary>
	/// Just a dummy function which enables to resolve if control is container or data
	/// </summary>
	public interface IContainerControl
	{
		/// <summary>
		/// Returns if control is container
		/// </summary>
		/// <returns>
		/// true if control is container <see cref="System.Boolean"/>
		/// </returns>
		bool IsContainer();
	}
	
	/// <summary>
	/// Any container implementing this interface is automaticaly supporting 
	/// connecting to Adaptor
	/// </summary>
	public interface IAdaptableContainer : IBoundedContainer
	{
		/// <value>
		/// DataSource connected to container
		/// </value>
		object DataSource { get; set; }
		/// <value>
		/// Defines is DataSource is inherited or not
		/// </value>
		bool InheritedDataSource { get; set; }
	}

	/// <summary>
	/// Any container implementing this interface is automaticaly supporting 
	/// connecting to Adaptor
	/// </summary>
	public interface IAdaptableListControl
	{
		/// <value>
		/// Provides access to current selection
		/// </value>
		Adaptor CurrentSelection { get; }
		/// <value>
		/// DataSource where control will take list items from
		/// </value>
		object ItemsDataSource { get; set; }
	}
	
	/// <summary>
	/// Any control implementing this interface is automaticaly supporting 
	/// connecting to Adaptor
	/// </summary>
	public interface IAdaptableControl : IAdaptableContainer, ICustomGetData
	{
		/// <value>
		/// Property mappings for control
		/// </value>
		string Mappings { get; set; }
	}
	
	/// <summary>
	/// Any control implementing this interface is automaticaly supporting 
	/// connecting to Adaptor
	/// </summary>
/*	public interface IComplexAdaptableControl : IAdaptableControl, IDisconnectable
	{
		/// <summary>
		/// Clears mappings before setting new ones
		/// </summary>
		void ClearBeforeRemapping();
		/// <summary>
		/// Invokes control remaping
		/// </summary>
		void RemapControl();
	}*/
	
	/// <summary>
	/// Provides interface to any changeable object that is accepting notification
	/// requests.
	/// </summary>
	public interface IAdaptable
	{
		/// <value>
		/// DataSource adaptor
		/// </value>
		IAdaptor DataSource { get; set; }
	}

	/// <summary>
	/// Specifies interface where control enables user events to handle GetRequest
	/// </summary>
	public interface ICustomGetData
	{
		/// <summary>
		/// Overrides basic Get data behaviour
		///
		/// Assigning this avoids any value transfer between object and data
		/// Basic assigning in DateCalendar for example is
		///    	Date = (DateTime) Adaptor.Value;
		/// where Date is the DateCalendar property and Adaptor.Value is direct
		/// reference to the mapped property
		///
		///     public delegate void UserGetDataEvent (ControlAdaptor Adaptor);
		/// </summary>
		event CustomGetDataEvent CustomGetData;
	}
	
	/// <summary>
	/// Specifies interface where control enables user events to handle PostRequest
	/// </summary>
	public interface ICustomPostData
	{
		/// <summary>
		/// Overrides basic Post data behaviour
		///
		/// Assigning this avoids any value transfer between object and data
		/// Basic assigning in DateCalendar for example is
		///    	adaptor.Value = Date;
		/// where Date is the DateCalendar property and Adaptor.Value is direct
		/// reference to the mapped property
		///
		///     public delegate void UserPostDataEvent (ControlAdaptor Adaptor);
		/// </summary>
		event CustomPostDataEvent CustomPostData;
	}
	
	/// <summary>
	/// Specifies interface where control enables user events to handle GetRequest and PostRequest
	/// </summary>
	public interface ICustomDataEvents : ICustomGetData, ICustomPostData
	{
	}

	/// <summary>
	/// Declares simple object which accesses information trough the DataSource
	/// </summary>
	public interface IAdaptableObjectReader
	{
		/// <value>
		/// Adaptor used to access information
		/// </value>
		IAdaptor Adaptor { get; }
		/// <value>
		/// DataSource connected to container
		/// </value>
		object DataSource { get; set; }
		/// <value>
		/// Property mappings for control
		/// </value>
		string Mappings { get; set; }
		/// <summary>
		/// Default event when data in object is changed
		/// </summary>
		/// <param name="aSender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		void GetDataFromDataSource (object aSender);
	}
	
	/// <summary>
	/// Declares simple object which iteracts information trough the DataSource both ways
	/// </summary>
	public interface IAdaptableObject : IAdaptableObjectReader
	{
		/// <summary>
		/// Default event when data is needed to transfer from control to object
		/// </summary>
		/// <param name="aSender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		void PutDataToDataSource (object aSender);
	}
	
	/// <summary>
	/// Minimal base of Adaptor types
	/// </summary>
	public interface IDataAdaptor
	{
		/// <value>
		/// Specifies if adaaptor is activated or not
		/// </value>
		bool Activated { get; }
		/// <value>
		/// Resolves final target for adaptor
		/// </value>
		object FinalTarget { get; }
		/// <value>
		/// Direct target of adaptor
		/// </value>
		object Target { get; set; }
	}
	
	/// <summary>
	/// Adaptor interface providing Target object and information about requested
	/// mappings.
	/// </summary>
	public interface IAdaptor : IChangeable, IDataAdaptor
	{
		/// <value>
		/// Represents adaptor id
		/// </value>
		System.Int64 Id { get; }
		/// <value>
		/// Name of DataSourceType if specified
		/// </value>
		string DataSourceTypeName { get; }
		/// <value>
		/// Type of DataSource
		/// </value>
		Type DataSourceType { get; }
		/// <value>
		/// Specifies if silenced or not
		/// </value>
		bool Silence { get; set; }
		/// <value>
		/// Specifies if adaptor supports single mapping only
		/// </value>
		bool SingleMappingOnly { get; }
		/// <value>
		/// Specifies control to which adaptor is connected
		/// </value>
		object Control { get; }
		/// <value>
		/// Specifies mappings for adaptor
		/// </value>
		string Mappings { get; set; }
		/// <summary>
		/// Returns mapped property resolved by name
		/// </summary>
		/// <param name="aName">
		/// Name of property <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Object to mapped property <see cref="MappedProperty"/>
		/// </returns>
		MappedProperty MappingByName (string aName);
		/// <summary>
		/// Returns mapped property resolved by index
		/// </summary>
		/// <param name="aIdx">
		/// Index of mapped property <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// Object to mapped property <see cref="MappedProperty"/>
		/// </returns>
		MappedProperty Mapping (int aIdx);
		/// <summary>
		/// Specifies mapping type of specified mapping
		/// </summary>
		/// <param name="aIdx">
		/// Index of mapping <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// Type of mapping <see cref="System.Type"/>
		/// </returns>
		System.Type MappingType (int aIdx);
		/// <value>
		/// Defines if adaptor has default mapping or not
		/// </value>
		bool HasDefaultMapping { get; }
		/// <summary>
		/// Adds new mapping
		/// </summary>
		/// <param name="aName">
		/// Mapping name <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Returns true if successful <see cref="System.Boolean"/>
		/// </returns>
		bool AddMapping (string aName);
		/// <summary>
		/// Adds new mapping
		/// </summary>
		/// <param name="aName">
		/// Mapping name <see cref="System.String"/>
		/// </param>
		/// <param name="aType">
		/// Type of mapping <see cref="System.Type"/>
		/// </param>
		/// <returns>
		/// Returns true if successful <see cref="System.Boolean"/>
		/// </returns>
		bool AddMapping (string aName, System.Type aType);
		/// <summary>
		/// Adds new mapping
		/// </summary>
		/// <param name="aName">
		/// Mapping name <see cref="System.String"/>
		/// </param>
		/// <param name="aType">
		/// Type of mapping <see cref="System.Type"/>
		/// </param>
		/// <param name="aRW">
		/// Read-write settings for mapped property <see cref="EReadWrite"/>
		/// </param>
		/// <param name="aClassName">
		/// Defines class name <see cref="System.String"/>
		/// </param>
		/// <param name="aIsColumn">
		/// Defines if mapping is coumn mapping or not <see cref="System.Boolean"/>
		/// </param>
		/// <param name="aColumnName">
		/// Name of column <see cref="System.String"/>
		/// </param>
		/// <param name="aMapping">
		/// Mapping string <see cref="System.String"/>
		/// </param>
		/// <param name="aSubItems">
		/// List of subitems <see cref="SMappedItem"/>
		/// </param>
		/// <returns>
		/// Returns true if successful <see cref="System.Boolean"/>
		/// </returns>
		bool AddMapping (string aName, System.Type aType, EReadWrite aRW, string aClassName, 
		                 bool aIsColumn, string aColumnName, string aMapping, SMappedItem[] aSubItems);
		/// <summary>
		/// Resolves mapping by name
		/// </summary>
		/// <param name="aName">
		/// Mapping name <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		bool RemoveMapping (string aName);
		/// <value>
		/// returns if destroy is in progress
		/// </value>
		bool DestroyInProgress { get; set; }
		/// <value>
		/// Returns if mapping is valid or not
		/// </value>
		bool IsValidMapping { get; }
		/// <value>
		/// Returns if adaptor is boundary or not
		/// </value>
		bool IsBoundaryAdaptor { get; }
		/// <value>
		/// Returns if adaptor is controlling widget or not
		/// </value>
		bool IsControllingWidget { get; }
		/// <value>
		/// Sets if target is inherited or not
		/// </value>
		bool InheritedTarget { get; set; }
		/// <value>
		/// Returns count of mapped properties
		/// </value>
		int MappingCount { get; }
		/// <value>
		/// Returns list of values
		/// </value>
		ValueList Values { get; }
		/// <summary>
		/// Returns default mapping value
		/// </summary>
		/// <returns>
		/// Value of default mapping <see cref="System.Object"/>
		/// </returns>
		object GetDefaultMappingValue();
		/// <summary>
		/// Checks if mapped property exists or not
		/// </summary>
		/// <param name="aName">
		/// Name of searched mapped property <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// true if property exists <see cref="System.Boolean"/>
		/// </returns>
		bool Exists (string aName);
		/// <summary>
		/// Sets value for default mapped property
		/// </summary>
		/// <param name="aValue">
		/// Value to be set <see cref="System.Object"/>
		/// </param>
		void SetDefaultMappingValue (object aValue);
		/// <summary>
		/// Resolves value of mapped property 
		/// </summary>
		/// <param name="aName">
		/// Name of mapped property <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Value of mapped property <see cref="System.Object"/>
		/// </returns>
		object GetMappingValue (string aName);
		/// <summary>
		/// Sets value of mapped property 
		/// </summary>
		/// <param name="aName">
		/// Name of mapped property <see cref="System.String"/>
		/// </param>
		/// <param name="aValue">
		/// New value <see cref="System.Object"/>
		/// </param>
		void SetMappingValue(string aName, object aValue);
		/// <summary>
		/// Gets mapped property value by property index
		/// </summary>
		/// <param name="aIdx">
		/// Index of mapped propery <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// Value of mapped property <see cref="System.Object"/>
		/// </returns>
		object GetMappingValue (int aIdx);
		/// <summary>
		/// Gets mapped property value by property index
		/// </summary>
		/// <param name="aIdx">
		/// Index of mapped propery <see cref="System.Int32"/>
		/// </param>
		/// <param name="aValue">
		/// New value <see cref="System.Object"/>
		/// </param>
		void SetMappingValue(int aIdx, object aValue);
		/// <summary>
		/// Clears mappings in adaptor
		/// </summary>
		void ClearMappings();
		/// <summary>
		/// Transfers all secondary values from DataSource to Requester and notifies about
		/// change if needed
		/// </summary>
		/// <param name="aRequester">
		/// Object to be filled with secondary values <see cref="System.Object"/>
		/// </param>
		/// <remarks>
		/// In case of error caller is responsible to handle exceptions
		/// </remarks>
		void FillObjectWithDataSourceValues (object aRequester);
		/// <summary>
		/// Transfers all secondary values from Control to DataSource and notifies about
		/// change if needed
		/// </summary>
		/// <param name="aRequester">
		/// Object to be used as source for secondary values <see cref="System.Object"/>
		/// </param>
		/// <remarks>
		/// In case of error caller is responsible to handle exceptions
		/// </remarks>
		void FillDataSourceWithObjectValues (object aRequester);
		/// <summary>
		/// Invokes PostRequest for this adaptor
		/// </summary>
		void PostMethod();
		/// <summary>
		/// Invokes notification of control that data has changed
		/// </summary>
		/// <param name="aSender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		void AdapteeDataChanged (object aSender);
		/// <summary>
		/// Invokes notification of control that boundary data has changed
		/// </summary>
		/// <param name="aSender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		void BoundaryAdapteeDataChanged (object aSender);
		/// <summary>
		/// Disconnects Control and adaptor
		/// </summary>
		void Disconnect();
		/// <summary>
		/// Sends message to bindings engine
		/// </summary>
		/// <param name="aSender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		/// <param name="aAction">
		/// Message action <see cref="EActionType"/>
		/// </param>
		void SendAdaptorMessage (object aSender, EActionType aAction);
		/// <summary>
		/// Invoked when adaptor target changes
		/// </summary>
		void AdaptorTargetChanged (IAdaptor aAdaptor);
		/// <summary>
		/// Autoconnects property and control
		/// </summary>
		void AutoConnect();
		/// <summary>
		/// Executes anonymous delegate event in specialized terms. For example
		/// all data transfers in Gtk.DataBindings have to be executed trough
		/// Gtk.Invoke
		/// </summary>
		/// <param name="aEvent">
		/// Event to be executed <see cref="AnonymousDelegateEvent"/>
		/// </param>
		void ExecuteUserMethod (AnonymousDelegateEvent aEvent);
		/// <summary>
		/// Event called when target changes
		/// </summary>
		event TargetChangedEvent TargetChanged;
		/// <summary>
		/// Event called on PostRequest
		/// </summary>
		event PostMethodEvent PostRequested;
	}
	
	/// <summary>
	/// This interface provides functionality for templated imports as in
	/// additional availability to extend the DnD and Copy/Paste functionality
	/// </summary>
	public interface ITemplatedImport
	{
		/// <summary>
		/// Import method
		/// </summary>
		/// <param name="aObject">
		/// Object to be imported <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		bool Import (object aObject); 
	}

	/// <summary>
	/// Interface to be used for custom AdaptorSelectors
	/// </summary>
	public interface IAdaptorSelector
	{
		/// <summary>
		/// Checks class if its type is correct for this selector
		/// </summary>
		/// <param name="aObject">
		/// Object to be checked <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// AdaptorSelector type or null if type is not correct <see cref="IAdaptorSelector"/>
		/// </returns>
		IAdaptorSelector CheckType (object aObject);
		/// <summary>
		/// Creates adaptor for that type
		/// </summary>
		/// <returns>
		/// Adaptor <see cref="IAdaptor"/>
		/// </returns>
		IAdaptor CreateAdaptor();
		/// <summary>
		/// Returns type of adaptor to be checked against if already
		/// allocated is of correct type
		/// </summary>
		/// <returns>
		/// Adaptor type <see cref="System.Type"/>
		/// </returns>
		System.Type GetAdaptorType();
	}
	
	/// <summary>
	/// Interface for simple elements, difference between VirtualObject and this one
	/// is that this one supports Same named values, while Virtual doesn't
	/// It is very suitable to directly read from XML
	/// </summary>
	public interface IElement
	{
		/// <value>
		/// Element name
		/// </value>
		string Name { get; set; }
		/// <value>
		/// Element value
		/// </value>
		string Value { get; set; }
		/// <value>
		/// Int16 value representation of element
		/// </value>
		Int16 Int16Value { get; set; }
		/// <value>
		/// Int32 value representation of element
		/// </value>
		Int32 Int32Value { get; set; }
		/// <value>
		/// Int64 value representation of element
		/// </value>
		Int64 Int64Value { get; set; }
		/// <value>
		/// Int value representation of element
		/// </value>
		int IntValue { get; set; }
		/// <value>
		/// DateTime representation value of element
		/// </value>
		DateTime DateTimeValue { get; set; }
		/// <value>
		/// Double representation value of element
		/// </value>
		double DoubleValue { get; set; }
		/// <value>
		/// UInt representation value of element
		/// </value>
		uint UIntValue { get; set; }
	}

	/// <summary>
	/// Supports list of elements and provides all that is needed to remove
	/// them from the list
	/// </summary>
	public interface IRemoveableElements
	{
		/// <summary>
		/// Removes element
		/// </summary>
		/// <param name="aElement">
		/// Element to be removed <see cref="IElement"/>
		/// </param>
		void RemoveElement (IElement aElement);
		/// <summary>
		/// Removes element by name
		/// </summary>
		/// <param name="aName">
		/// Name of element <see cref="System.String"/>
		/// </param>
		void Remove (string aName);
		/// <summary>
		/// Removes all element by same name
		/// </summary>
		/// <param name="aName">
		/// Name of element <see cref="System.String"/>
		/// </param>
		void RemoveAll (string aName);
		/// <summary>
		/// Removes element by value
		/// </summary>
		/// <param name="aValue">
		/// Value of element <see cref="System.String"/>
		/// </param>
		void RemoveValue (string aValue);
		/// <summary>
		/// Removes all element with value
		/// </summary>
		/// <param name="aValue">
		/// Value of element <see cref="System.String"/>
		/// </param>
		void RemoveAllValues (string aValue);
		/// <summary>
		/// Returns element by name
		/// </summary>
		IElement this[string aName] { get; }
	}
	
	/// <summary>
	/// Provides additional methods to add attributes to RemoveableElements
	/// </summary>
	public interface IAttributeList : IRemoveableElements, IEnumerable
	{
		/// <summary>
		/// Adds element to attribute list
		/// </summary>
		/// <param name="aElement">
		/// Element to be added <see cref="IElement"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		bool AddAttr (IElement aElement);
		/// <summary>
		/// Creates element with name and value, then adds it to attribute list
		/// </summary>
		/// <param name="aName">
		/// Name of new element <see cref="System.String"/>
		/// </param>
		/// <param name="aValue">
		/// Value of new element <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		bool AddAttr (string aName, string aValue);
	}

	/// <summary>
	/// Provides additional methods to add elements to RemoveableElements
	/// </summary>
	public interface IElementList : IRemoveableElements, IEnumerable
	{
		/// <summary>
		/// Adds element to attribute list
		/// </summary>
		/// <param name="aElement">
		/// Element to be added <see cref="IElement"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		bool AddElement (IElement aElement);
		/// <summary>
		/// Creates element with name and value, then adds it to attribute list
		/// </summary>
		/// <param name="aName">
		/// Name of new element <see cref="System.String"/>
		/// </param>
		/// <param name="aValue">
		/// Value of new element <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		bool AddElement (string aName, string aValue);
	}

	/// <summary>
	/// Defines objects that can contain element that contains both, list of
	/// attributes and elements
	/// </summary>
	public interface IFullElement : IElement
	{
		/// <value>
		/// List of attributes
		/// </value>
		IAttributeList Attributes { get; }
		/// <value>
		/// List of elements
		/// </value>
		IElementList Elements { get; }
	}

	/// <summary>
	/// Resolves MappedProperty from string
	/// used in Adaptor where resolving mapped properties
	/// </summary>
	public struct SMappedItem
	{
		/// <value>
		/// Classname specified by mapping
		/// </value>
		public string ClassName;
		/// <value>
		/// Name of mapped property
		/// </value>
		public string Name;
		/// <value>
		/// Column name
		/// </value>
		public string ColumnName;
		/// <value>
		/// String representation of ReadWrite attributes
		/// </value>
		public string RW;
		/// <value>
		/// Mappe item
		/// </value>
		public string MappedItem;
		/// <value>
		/// List of column items
		/// </value>
		public SMappedItem[] ColumnItems;

		/// <value>
		/// readWrite flags of this mapped property
		/// </value>
		public EReadWrite RWFlags {
			get {
				switch (RW) {
					case "<>" : return (EReadWrite.ReadWrite);
					case ">>" : return (EReadWrite.ReadOnly);
					case "<<" : return (EReadWrite.WriteOnly);
					default : return (EReadWrite.None);
				}
			}
		}
		
		/// <summary>
		/// Resolves new mapping from string 
		/// </summary>
		/// <param name="aMapping">
		/// Mapping name <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Resolved mapping <see cref="SMappedItem"/>
		/// </returns>
		public SMappedItem ResolveMappingString(string aMapping)
		{
			return (new SMappedItem(aMapping));
		}
		
		/// <summary>
		/// Resolves new mappings
		/// </summary>
		/// <param name="aMapping">
		/// Creates new mappings from mapping string <see cref="System.String"/>
		/// </param>
		public void SetNewMappingsFromStrings(string aMapping)
		{
			ArrayList lst = new ArrayList();
			string map = "";
			int pos = aMapping.IndexOf(";");
			while (pos > -1) {
				map = aMapping.Substring (0, pos).Trim();
				if (map != "")
					lst.Add (ResolveMappingString (map));
				aMapping = aMapping.Remove (0, pos+1);
				pos = aMapping.IndexOf(";");
			}
			aMapping = aMapping.Trim();
			if (aMapping != "")
				lst.Add (ResolveMappingString (aMapping));

			if (lst.Count > 0) {
				// Count valid only
				int i = 0;
				foreach (SMappedItem item in lst)
					if (item.Name != "")
						i++;
				int j = 0;
				if (i > 0) {
					ColumnItems = new SMappedItem [i];
					foreach (SMappedItem item in lst)
						if (item.Name != "") {
							if (item.RWFlags == EReadWrite.WriteOnly)
								throw new Exception ("Sub-Column can't be write only");
							if (item.ClassName != "")
								throw new Exception ("Sub-Column can't specify CLASS");
							if (item.ColumnName != "")
								throw new Exception ("Sub-Column can't specify sub-column");
							ColumnItems[j] = item;
							j++;
						}
						else
							throw new Exception ("Sub-Column mapping error, can't define default herre!");
				}
			}
			lst.Clear();
			lst = null;
		}

		/// <summary>
		/// Creates new mapping
		/// </summary>
		/// <param name="aMapping">
		/// Mapping string to be resolved <see cref="System.String"/>
		/// </param>
		public SMappedItem (string aMapping)
		{
			ColumnItems = null;
			ClassName = "";
			Name = "";
			ColumnName = "";
			RW = "";
			MappedItem = "";
			aMapping = aMapping.Trim();
			if (aMapping == "")
				return;
			int s = aMapping.IndexOf("(");
			int e = aMapping.IndexOf(")");
			if (s != e)
				if ((s+1) >= e)
					throw new Exception ("Wrong formulated CLASSNAME in property mapping");
				else
					ClassName = aMapping.Substring (s+1, e-s-1).Trim();
			if (e > -1)
				aMapping = aMapping.Remove (s, e-s+1);
			aMapping = aMapping.Trim();
			s = aMapping.IndexOf("[");
			e = aMapping.IndexOf("]");
			if (s != e)
				if ((s+1) >= e)
					throw new Exception ("Wrong formulated CLASSNAME in property mapping");
				else {
					ColumnName = aMapping.Substring (s+1, e-s-1).Trim();
					if (ColumnName.IndexOf("[") > -1)
						throw new Exception ("Column is containg column (\"[\" inside [])? Syntax error!!!");
					// Does column name provide complex arrangement???
					int pos = ColumnName.IndexOf("::");
					if (pos > -1) {
						string colname = ColumnName.Substring (0, pos);
						string mappings = ColumnName.Substring (pos+2, ColumnName.Length-(pos+2));
						ColumnName = colname;
						// Resolve submappings
						SetNewMappingsFromStrings (mappings);
					}
				}
			if (e > -1)
				aMapping = aMapping.Remove (s, e-s+1);
			aMapping = aMapping.Trim();
			if (aMapping.IndexOf("<>") > -1)
				RW = "<>";
			else
				if (aMapping.IndexOf(">>") > -1)
					RW = ">>";
				else
					if (aMapping.IndexOf("<<") > -1)
						RW = "<<";
			if (RW == "")
				Name = aMapping;
			else {
				s = aMapping.IndexOf (RW);
				Name = aMapping.Substring (0, s).Trim();
				MappedItem = aMapping.Remove (0, s+2).Trim();
			}
		}

		/// <summary>
		/// Converts mapping into string
		/// </summary>
		/// <returns>
		/// debugging value <see cref="System.String"/>
		/// </returns>
		public override string ToString()
		{
			string res = "ClassName=`" + ClassName + "`, Name=`" + Name + "`, ColumnName=`" + ColumnName + "`, RW=`" + RW + "`, MappedItem=`" + MappedItem + "`";
			if (ColumnItems != null)
				for (int i=0; i<ColumnItems.Length; i++)
					res = res + "\n  COLUMN | " + ColumnItems[i].ToString();
			return (res);
		}
	}
}
