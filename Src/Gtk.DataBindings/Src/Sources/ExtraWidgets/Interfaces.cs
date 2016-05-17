//Interfaces.cs - Description
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

using Cairo;
using System;
using System.Collections;
using System.ComponentModel;
using Gtk;
using Gtk.DataBindings;
using System.Data.Bindings;

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// Event arguments passed on color clicked event
	/// </summary>
	public class ColorClickedEventArgs
	{
		private Cairo.Color color;
		/// <value>
		/// Color
		/// </value>
		public Cairo.Color Color {
			get { return (color); }
		}
		
		public ColorClickedEventArgs (Cairo.Color aColor)
		{
			color = aColor;
		}
	}
	
	/// <summary>
	/// Delegate event on color clicked
	/// </summary>
	public delegate void ColorClickedEvent (object aSender,  ColorClickedEventArgs aArgs);
	
	/// <summary>
	/// Event arguments passed on link clicked event
	/// </summary>
	public class LinkClickedEventArgs
	{
		private string text = "";
		/// <value>
		/// Text
		/// </value>
		public string Text {
			get { return (text); }
		}
		
		private string linkUri = "";
		/// <value>
		/// Uri
		/// </value>
		public string LinkUri {
			get { return ( linkUri); }
		}
		
		public LinkClickedEventArgs (string aText, string aUri)
		{
			text = aText;
			linkUri = aUri;
		}
	}
	
	/// <summary>
	/// Delegate event on link clicked
	/// </summary>
	public delegate void LinkClickedEvent (object aSender,  LinkClickedEventArgs aArgs);
	
	public class DayDescription
	{
		private string description = "";
		public string Description { 
			get { return (description); }
			set { description = value; }
		}
		
		public Gtk.StateType State { get; set; }
		public object[] IconicState { get; set; }
		
		public DayDescription (string aDescription, Gtk.StateType aState, object[] aIconicState)
		{
			Description = aDescription;
			State = aState;
			IconicState = aIconicState;
		}
	}
	
	public class DayDescriptionEventArgs : EventArgs
	{
		private CalendarDayDescription descriptor = null;
		public CalendarDayDescription Descriptor {
			get { return (descriptor); }
			set {
				if (descriptor == value)
					return;
				descriptor = value;
			}
		}
		
		private int year = 0;
		public int Year {
			get { return (year); }
			set {
				if (year == value)
					return;
				year = value;
			}
		}
		
		private int month = 0;
		public int Month {
			get { return (month); }
			set {
				if (month == value)
					return;
				month = value;
			}
		}
		
		private int day = 0;
		public int Day {
			get { return (day); }
			set {
				if (day == value)
					return;
				day = value;
			}
		}
		
		private DayDescription description = new DayDescription ("", Gtk.StateType.Normal, null);
		public DayDescription Description {
			get { return (description); }
			set { description = value; }
		}
		
		public DayDescriptionEventArgs (CalendarDayDescription aDescriptor, int aYear, int aMonth, int aDay)
		{
			year = aYear;
			day = aDay;
			month = aMonth;
			descriptor = aDescriptor;
		}
	}
	
	public delegate void RequestDayDescriptionEvent (object aSender, DayDescriptionEventArgs aArgs);
		                       
	public class CalendarDayDescription : BaseNotifyPropertyChanged
	{
		private DayDescriptionEventArgs args = null;
		
		private bool descriptionsVisible = false;
		public bool DescriptionsVisible {
			get { return (descriptionsVisible); }
			set {
				if (descriptionsVisible == value)
					return;
				descriptionsVisible = value;
				OnPropertyChanged ("DescriptionsVisible");
			}
		}
		
		private bool iconsVisible = false;
		public bool IconsVisible {
			get { return (iconsVisible); }
			set {
				if (iconsVisible == value)
					return;
				iconsVisible = value;
				OnPropertyChanged ("IconsVisible");
			}
		}

		private int currentMonth = DateTime.Today.Month;
		public int CurrentMonth {
			get { return (currentMonth); }
			set {
				if (currentMonth == value)
					return;
				currentMonth = value;
				OnPropertyChanged ("CurrentMonth");
			}
		}
		
		private int currentYear = DateTime.Now.Year;
		public int CurrentYear {
			get { return (currentYear); }
			set {
				if (currentYear == value)
					return;
				currentYear = value;
				OnPropertyChanged ("CurrentYear");
			}
		}
		
		private event RequestDayDescriptionEvent dayDescriptionRequest = null;
		/// <summary>
		/// Handles requests for descriptions
		/// </summary>
		public event RequestDayDescriptionEvent DayDescriptionRequest {
			add { dayDescriptionRequest += value; }
			remove { dayDescriptionRequest -= value; }
		}
		
		public DayDescription OnDayDescriptionRequest (int aYear, int aMonth, int aDay)
		{
			args.Year = aYear;
			args.Month = aMonth;
			args.Day = aDay;
			args.Description.Description = "";
			args.Description.State = Gtk.StateType.Normal;
			args.Description.IconicState = null;
			if (dayDescriptionRequest != null)
				dayDescriptionRequest (this, args);
			return (args.Description);
		}
		
		public CalendarDayDescription()
		{
			args = new DayDescriptionEventArgs (this, 0, 0, 0);
		}
	}
	
	public enum CellAction
	{
		None,
		GetSize,
		Paint
	}
	
	/// <summary>
	/// Cell action arguments which can be resolved in methods which don't really support
	/// passing of them
	/// </summary>
	public class CellArguments
	{
		private CellAction actionType = CellAction.None;
		/// <value>
		/// Type of action in progress
		/// </value>
		public CellAction ActionType {
			get { return (actionType); }
		}
		
		private CellExposeEventArgs passedArguments = null;
		/// <value>
		/// Arguments passed
		/// </value>
		public CellExposeEventArgs PassedArguments {
			get { return (passedArguments); }
		}
		
		/// <summary>
		/// Starts action by setting parameters 
		/// </summary>
		/// <param name="aActionType">
		/// Action type <see cref="CellAction"/>
		/// </param>
		/// <param name="aArguments">
		/// Arguments <see cref="CellExposeEventArgs"/>
		/// </param>
		public void Start (CellAction aActionType, CellExposeEventArgs aArguments)
		{
			if ((aActionType == CellAction.None) || (aArguments == null))
				return;
			if (ActionType != CellAction.None)
				throw new Exception ("Action is already in progress");
			actionType = aActionType;
			passedArguments = aArguments;
		}
		
		/// <summary>
		/// Stops action
		/// </summary>
		public void Stop()
		{
			if (ActionType == CellAction.None)
				return;
			actionType = CellAction.None;
			passedArguments.Disconnect();
			passedArguments = null;
		}
		
		~CellArguments()
		{
			Stop();
		}
	}
	
	/// <summary>
	/// Specifies cell redraw arguments
	/// </summary>
	public class CellExposeEventArgs : EventArgs
	{
		private Gdk.EventExpose exposeEvent = null;
		/// <value>
		/// Original expose event arguments
		/// </value>
		public Gdk.EventExpose ExposeEvent {
			get { return (exposeEvent); }
		}
		
		private bool needsRecalculation = false;
		public bool NeedsRecalculation {
			get { return (needsRecalculation); }
			set { needsRecalculation = value; }
		}
		
		private bool forceRecalculation = false;
		public bool ForceRecalculation {
			get { return (forceRecalculation); }
			set { forceRecalculation = value; }
		}
		
		/// <value>
		/// Returns true if drawing is part of official expose event
		/// </value>
		public bool OfficialDraw {
			get { return (ExposeEvent != null); }
		}
		
		private Cairo.Context context = null;
		/// <value>
		/// Cairo context
		/// </value>
		public Cairo.Context Context {
			get { return (context); }
		}
		
		private Gdk.Drawable drawable = null;
		/// <value>
		/// Gdk.Drawable context
		/// </value>
		public Gdk.Drawable Drawable {
			get { return (drawable); }
		}
		
		private	CellRectangle clippingArea;
		/// <value>
		/// Clipping area
		/// </value>
		public CellRectangle ClippingArea {
			get { return (clippingArea); }
			set { clippingArea = value; }
		}
		
		private CellRectangle cellArea;
		/// <value>
		/// Cell area
		/// </value>
		public CellRectangle CellArea {
			get { return (cellArea); }
			set { cellArea = value; }
		}

		private CellRendererState flags = 0;
		public CellRendererState Flags {
			get { return (flags); }
			set { flags = value; }
		}
		
		private object widget = null;
		public object Widget {
			get { return (widget); }
			set { widget = value; }
		}
		
		private bool widgetInRenderer = false;
		public bool WidgetInRenderer {
			get { return (widgetInRenderer); }
			set {
				if (widgetInRenderer == value)
					return;
				widgetInRenderer = value;
			}
		}
		
		public bool IsRenderer {
			get { return (Renderer != null); }
		}
		
		private CellRenderer renderer = null;
		public CellRenderer Renderer {
			get { return (renderer); }
			set { renderer = value; }
		}
		
		public void Disconnect()
		{
			clippingArea = null;
			cellArea = null;
			context = null;
			drawable = null;
			exposeEvent = null;
			widget = null;
			renderer = null;
		}
		
		public CellExposeEventArgs (Gdk.EventExpose aArgs, Cairo.Context aContext, Gdk.Drawable aDrawable,
		                            CellRectangle aClippingArea, CellRectangle aCellArea)
		{
			exposeEvent = aArgs;
			context = aContext;
			drawable = aDrawable;
			clippingArea = aClippingArea;
			cellArea = aCellArea;
		}
		
		~CellExposeEventArgs()
		{
			Disconnect();
		}
	}
	
	public delegate void StoreBufferEvent();
	public delegate Gdk.Pixbuf GetPixbufEvent();
	public delegate void DataReceivedEvent (uint aType, Gtk.Widget aSource, SelectionData aData);

	/// <summary>
	/// Event triggered on time change
	/// </summary>
	public delegate void TimeChangedEvent (object aSender, TimeChangedEventArgs aArgs);
	
	/// <summary>
	/// Event arguments passed on time change
	/// </summary>
	public class TimeChangedEventArgs
	{
		private DateTime time = DateTime.Now;
		/// <value>
		/// New time
		/// </value>
		public DateTime Time {
			get { return (time); }
		}
		
		public TimeChangedEventArgs (DateTime aTime)
		{
			time = aTime;
		}
	}
	
	/// <summary>
	/// Arguments passed on activation
	/// </summary>
	public class ActivationEventArgs : EventArgs
	{
		private WeakReference activatedObject = null;
		/// <value>
		/// Activated object
		/// </value>
		public object ActivatedObject {
			get { 
				if (activatedObject != null)
					return (activatedObject.Target);
				return (activatedObject); 
			}
		}
		
		public ActivationEventArgs (object aActivatedObject)
		{
			activatedObject = new WeakReference (aActivatedObject);
		}
	}
	
	/// <summary>
	/// Interface describing activatable objects
	/// </summary>
	public interface IActivatable
	{
		/// <summary>
		/// Event sent on object activation
		/// </summary>
		event ActivatedEvent Activated;
		/// <summary>
		/// Executes activation
		/// </summary>
		void Activate();
	}
	
	/// <summary>
	/// Event sent on object activation
	/// </summary>
	public delegate void ActivatedEvent (object aSender, ActivationEventArgs aArgs);
	
	public interface ICustomGtkState : IGtkState
	{
		StateType OwnerState { get; }
		StateType CustomState { get; set; }
		ValueResolveMethod StateResolving { get; set; }
	}
	
	/// <summary>
	/// Specifies classes which are able to do prelight
	/// </summary>
	public interface IGtkState
	{
		/// <value>
		/// Prelight active or not
		/// </value>
		StateType State { get; }
	}
	
	/// <summary>
	/// Determines how state is resolved
	/// </summary>
	public enum ValueResolveMethod
	{
		/// <summary>
		/// Resolved from owner
		/// </summary>
		FromOwner,
		/// <summary>
		/// Manual state
		/// </summary>
		Manual
	}
	
	/// <summary>
	/// Specifies which side should be cutted from painting cell
	/// </summary>
	public enum SideCut
	{
		/// <summary>
		/// None, all borders are visible
		/// </summary>
		None = 0,
		/// <summary>
		/// Only left border is not visible
		/// </summary>
		Left = 1,
		/// <summary>
		/// Only right border is not visible
		/// </summary>
		Right = 2,
		/// <summary>
		/// Only top border is not visible
		/// </summary>
		Top = 4,
		/// <summary>
		/// Only bottom border is not visible
		/// </summary>
		Bottom = 8,
		/// <summary>
		/// Left and bottom borders are not visible
		/// </summary>
		LeftBottom = SideCut.Left | SideCut.Bottom,
		/// <summary>
		/// Left and top borders are not visible
		/// </summary>
		LeftTop = SideCut.Left | SideCut.Top,
		/// <summary>
		/// Left and right borders are not visible
		/// </summary>
		LeftRight = SideCut.Left | SideCut.Right,
		/// <summary>
		/// Right and bottom borders are not visible
		/// </summary>
		RightBottom = SideCut.Right | SideCut.Bottom,
		/// <summary>
		/// Right and top borders are not visible
		/// </summary>
		RightTop = SideCut.Right | SideCut.Top,
		/// <summary>
		/// Left and top borders are not visible
		/// </summary>
		TopBottom = SideCut.Top | SideCut.Bottom,
		/// <summary>
		/// Only left border is visible
		/// </summary>
		ShowLeftOnly = SideCut.Right | SideCut.Top | SideCut.Bottom,
		/// <summary>
		/// Only top border is visible
		/// </summary>
		ShowTopOnly = SideCut.Left | SideCut.Bottom | SideCut.Right,
		/// <summary>
		/// Only right border is visible
		/// </summary>
		ShowRightOnly = SideCut.Left | SideCut.Top | SideCut.Bottom,
		/// <summary>
		/// Only bottom border is visible
		/// </summary>
		ShowBottomOnly = SideCut.Left | SideCut.Top | SideCut.Right,
		/// <summary>
		/// No borders are visible
		/// </summary>
		All = SideCut.Left | SideCut.Top | SideCut.Right | SideCut.Bottom
	}
	
	/// <summary>
	/// Specifies drawing cell
	/// </summary>
	public interface IDrawingCell
	{
		/// <value>
		/// Specifies if cell is visible or not
		/// </value>
		bool Visible { get; set; }
		/// <value>
		/// Checks if cell is visible or not
		/// </value>
		bool IsVisible { get; }
		/// <value>
		/// Specifies border width for cell
		/// </value>
		double Padding { get; set; }
		/// <value>
		/// Returns top most owner of the cells who isn't IDrawingCell
		/// </value>
		object Master { get; }
		/// <value>
		/// Returns true if master is focused
		/// </value>
		bool MasterIsFocused { get; }
		/// <value>
		/// Cell owner
		/// </value>
		object Owner { get; set; }
		/// <value>
		/// Value is calculated on redraw
		/// </value>
		CellRectangle Area { get; }
		/// <value>
		/// Speceifies if cell is expanded or not
		/// </value>
		bool Expanded { get; set; }
		/// <summary>
		/// Paints cell on cairo context
		/// </summary>
		/// <param name="aContext">
		/// Context <see cref="Cairo.Context"/>
		/// </param>
		/// <param name="aClippingArea">
		/// Rectangle <see cref="Cairo.Rectangle"/>
		/// </param>
		/// <param name="aArea">
		/// Rectangle <see cref="Cairo.Rectangle"/>
		/// </param>
		void Paint (CellExposeEventArgs aArgs);
			//(Gdk.EventExpose evnt, Cairo.Context aContext, Cairo.Rectangle aClippingArea, Cairo.Rectangle aArea);
		/// <summary>
		/// Resolves size needed for cell
		/// </summary>
		/// <param name="aWidth">
		/// Cell width <see cref="System.Int32"/>
		/// </param>
		/// <param name="aHeight">
		/// Cell height <see cref="System.Int32"/>
		/// </param>
		/// <remarks>
		/// This method is internal and should not be called directly
		/// </remarks>
		void GetSize (out double aWidth, out double aHeight);
		/// <summary>
		/// Resolves size needed for cell
		/// </summary>
		/// <param name="aWidth">
		/// Cell width <see cref="System.Int32"/>
		/// </param>
		/// <param name="aHeight">
		/// Cell height <see cref="System.Int32"/>
		/// </param>
		void GetCellSize (out double aWidth, out double aHeight);
		/// <summary>
		/// Returns cell which takes place on specified coordinates
		/// </summary>
		/// <param name="aX">
		/// X <see cref="System.Double"/>
		/// </param>
		/// <param name="aY">
		/// Y <see cref="System.Double"/>
		/// </param>
		/// <returns>
		/// Cell or null <see cref="IDrawingCell"/>
		/// </returns>
		/// <remarks>
		/// This method is usefull for mouse resolving
		/// </remarks>
		IDrawingCell CellAtCoordinates (double aX, double aY);
	}
	
	/// <summary>
	/// Event arguments for horizontal combo box
	/// </summary>
	public class HorizontalComboEventArgs
	{
		private int index = 0;
		/// <value>
		/// Cell index
		/// </value>
		public int Index {
			get { return (index); }
		}
		
		private HorizontalCombo control = null;
		/// <value>
		/// Widget containing cell
		/// </value>
		public HorizontalCombo Control {
			get { return (control); }
		}
		
		public HorizontalComboEventArgs (HorizontalCombo aCombo, int aIndex)
		{
		}
	}
	
	/// <summary>
	/// Delegate method inistantiated on cell change
	/// </summary>
	public delegate void HorizontalComboBoxEvent (object aSender, HorizontalComboEventArgs aArgs);
	
	/// <summary>
	/// Type of cell change
	/// </summary>
	public enum CellChangedType {
		/// <summary>
		/// Display
		/// </summary>
		Display,
		/// <summary>
		/// State
		/// </summary>
		State,
		/// <summary>
		/// Display and state
		/// </summary>
		All
	}
	
	/// <summary>
	/// Event arguments passed on cell change
	/// </summary>
	public class EnumCellChangedEventArgs : EventArgs
	{
		private HComboCell cell = null;
		/// <value>
		/// Cell
		/// </value>
		public HComboCell Cell {
			get { return (cell); }
		}
		
		private CellChangedType changeType = CellChangedType.All;
		/// <value>
		/// Change type
		/// </value>
		public CellChangedType ChangeType {
			get { return (changeType); }
		}
		
		public EnumCellChangedEventArgs (CellChangedType aChangeType, HComboCell aCell)
		{
			cell = aCell;
			changeType = aChangeType;
		}
	}
	
	/// <summary>
	/// Delegate method inistantiated on cell change
	/// </summary>
	public delegate void EnumCellChangedEvent (object aSender, EnumCellChangedEventArgs aArgs);
	
	/// <summary>
	/// Types of chameleon event box
	/// </summary>
	public enum ChameleonStyle
	{
		/// <summary>
		/// Classic event box
		/// </summary>
		ClassicEventBox,
		/// <summary>
		/// Button style
		/// </summary>
		Button,
		/// <summary>
		/// Entry style
		/// </summary>
		Entry
	}
	
	public delegate void ClearDataEvent();
	public delegate void SelectNextEvent (Gtk.Widget aNextFrom);
	public delegate void StartCalculatorEvent (Gtk.Widget aWidget);
	public delegate void DateSelectorEvent (Gtk.Widget aWidget);
	public delegate void DropDownEvent (Gtk.Widget aWidget);

	public delegate void SearchAddClickedEvent();
	public delegate void SearchRemoveClickedEvent();
	public delegate void SearchClickedEvent();
	public delegate void StatusLeftClickedEvent();
	public delegate void StatusMiddleClickedEvent();
	public delegate void StatusRightClickedEvent();
	public delegate void StatusClickedEvent();
	public delegate void ColorChangeEvent (CustomColorDescription aColor);
	public delegate void DateEventHandler(object sender, DateEventArgs args);	
	public delegate void GdkKeyEventHandler(object sender, GdkKeyEventArgs args);	

	/// <summary>
	/// EventArgs specifiying date as parameter
	/// </summary>
	public class DateEventArgs : EventArgs
	{
		private DateTime date;
		/// <value>
		/// Date value
		/// </value>
		public DateTime Date 
		{
			get {	return (date);	}
		}
		
		public DateEventArgs (DateTime aDate)
		{
			date = aDate;
		}
	}

	/// <summary>
	/// EventArgs specifiying key as parameter
	/// </summary>
	public class GdkKeyEventArgs : EventArgs
	{
		private Gdk.Key key;
		/// <value>
		/// Key value
		/// </value>
		public Gdk.Key Key
		{
			get {	return (key);	}
		}
		
		public GdkKeyEventArgs (Gdk.Key aKey)
		{
			key = aKey;
		}
	}

	/// <summary>
	/// Class containing list of special keys
	/// </summary>
	public class KeyInfoList
	{
		private ArrayList list = new ArrayList();

		/// <summary>
		/// Clears list of keys
		/// </summary>
		public void Clear()
		{
			list.Clear();
		}

		/// <summary>
		/// Checks if key is specified in list
		/// </summary>
		/// <param name="aKey">
		/// Key <see cref="Gdk.Key"/>
		/// </param>
		/// <returns>
		/// true if key is in list, false if not
		/// </returns>
		public bool Contains (Gdk.Key aKey)
		{
			foreach (Gdk.Key k in list)
				if (k == aKey)
					return (true);
			return (false);
		}

		/// <summary>
		/// Adds key to list
		/// </summary>
		/// <param name="aKey">
		/// Key <see cref="Gdk.Key"/>
		/// </param>
		public void Add (Gdk.Key aKey)
		{
			if (Contains(aKey) == true)
				return;
			list.Add (aKey);
		}

		/// <summary>
		/// Removes key from list
		/// </summary>
		/// <param name="aKey">
		/// Key <see cref="Gdk.Key"/>
		/// </param>
		public void Remove (Gdk.Key aKey)
		{
			if (Contains(aKey) == true)
				list.Remove (aKey);
		}

		public KeyInfoList()
		{
		}

		public KeyInfoList (Gdk.Key[] aKeys)
		{
			if (aKeys == null)
				return;
			foreach (Gdk.Key k in aKeys)
				Add (k);
		}
	}
	
	/// <summary>
	/// Used to declare how widget should flash
	/// </summary>
	public enum FlashingType
	{
		SingleStep,
		TimedFlash,
		CountedFlash, 
		LoopFlash
	}

	public enum ErrorInformationType
	{
		None,
		Custom,
		Information,
		Question,
		Warning,
		Error,
		Critical
	}
	
	public class CustomColorDescription
	{
		///<summary>
		/// Returns if extra coloring was enabled
		///</summary>
		private bool enabled = false;
		public bool Enabled {
			get { return (enabled); }
			set {
				if (enabled = value)
					return;
				enabled = value;
				if (onColorChange != null)
					onColorChange (this);
			}
		}

		///<summary>
		/// Defines color 
		///</summary>
		private Gdk.Color widgetColor;
		public Gdk.Color WidgetColor {
			get { return (widgetColor); }
			set { 
				if (Gdk.Color.Equals(widgetColor, value) == true)
					return;
				widgetColor = value;
				if (onColorChange != null)
					onColorChange (this);
			}
		}
		
		///<summary>
		/// Called whenever new color has been set up 
		///</summary>
		private event ColorChangeEvent onColorChange = null;
		public event ColorChangeEvent OnColorChange {
			add { onColorChange += value; }
			remove { onColorChange -= value; }
		}
		
		/// <summary>
		/// Enables the color activity, sets WidgetColor and executes OnColorChange
		/// </summary>
		public void Set (Gdk.Color aColor)
		{
			enabled = true;
			WidgetColor = aColor;
		}
		
		/// <summary>
		/// Disables the color activity and executes OnColorChange
		/// </summary>
		public void Reset()
		{
			Enabled = false;
		}
		
		public CustomColorDescription()
		{
		}
	}

	public interface IValidator
	{
		
	}
	
	public interface ICustomColored
	{
		CustomColorDescription CustomColor { get; }
	}
}
