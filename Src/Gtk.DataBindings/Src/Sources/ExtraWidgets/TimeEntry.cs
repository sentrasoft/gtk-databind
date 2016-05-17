//  
//  Copyright (C) 2009 matooo
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System;
using System.ComponentModel;
using System.Data.Bindings;
using Gdk;
using Gtk;
using Gtk.DataBindings;

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// 
	/// </summary>
	[ToolboxItem (true)]
	[Category ("Extra Gtk Widgets")]
	public class TimeEntry : CellDrawingArea
	{
		/// <value>
		/// Data parts
		/// </value>
		protected enum DataPart 
		{
			Hour = 0,
			Minute = 1,
			Second = 2,
			Millisecond = 3,
			AMPM = 4,
			HourSeparator = 5,
			MillisecondSeparator = 6,
			None = 7
		}

		#region TimeText
		
		protected class TimeText : DrawingCellEditText
		{
			private TimeEntry owner = null;
			
			private DataPart part = DataPart.HourSeparator;
			/// <value>
			/// Specifies part which is edited
			/// </value>
			public DataPart Part {
				get { return (part); }
			}
			
			/// <value>
			/// Resolves if text is selected or not
			/// </value>
			public override bool Selected {
				get { return (owner.Selected == Part); }
				set { 
					if (Selected == value)
						return;
					owner.Selected = Part; 
					OnPropertyChanged ("Selected");
				}
			}

			/// <summary>
			/// Refreshes data
			/// </summary>
			public void Refresh()
			{
				System.Globalization.CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
				switch (Part) {
				case DataPart.Hour:
					if (owner.TwelveHour == true)
						Text = (owner.Time.Hour%12).ToString("D2");
					else
						Text = owner.Time.Hour.ToString("D2");
					break;
				case DataPart.Minute:
					Text = owner.Time.Minute.ToString("D2");
					break;
				case DataPart.Second:
					Text = owner.Time.Second.ToString("D2");
					break;
				case DataPart.Millisecond:
					Text = owner.Time.Millisecond.ToString("D3");
					break;
				case DataPart.AMPM:
					Text = (owner.Time.Hour >= 12) ? 
						" " + ci.DateTimeFormat.PMDesignator : " " + ci.DateTimeFormat.AMDesignator;
					break;
				}
			}
			
			public TimeText (TimeEntry aOwner, DataPart aPart)
				: base ()
			{
				XPos = 0.5;
				part = aPart;
				owner = aOwner;
				switch (aPart) {
				case DataPart.AMPM:
					SizeText = " <b>AM</b>";
					break;
				case DataPart.HourSeparator:
					Text = ":";
					break;
				case DataPart.MillisecondSeparator:
					Text = ".";
					break;
				case DataPart.Millisecond:
					SizeText = "<b>888</b>";
					break;
				default:
					SizeText = "<b>88</b>";
					break;
				}
				Refresh();
			}
		}
		
		#endregion TextEdit
		
		private int[] max = new int[4] { 23, 59, 59, 999 };
		/// <value>
		/// Specifies max number which can be entred in selected part
		/// </value>
		protected int MaxNumber {
			get { 
				if ((int) Selected > 3)
					return (0);
				return (max[(int) Selected]);
			}
		}

		private DateTime copyBuffer = DateTime.Now;
		private static string x_SpecialName = "application/x-dotnet-time";
		
		private static TargetEntry[] validTargets = ClipboardHelper.GetTextTargetsWithSpecial (x_SpecialName, (int) TransferDataType.X_Special);
		protected static TargetEntry[] ValidTargets {
			get { return (validTargets); }
		}		

		private int atStart = 0;
		private IDrawingCell stateCell = null;
		private IconSize icon_size = IconSize.Menu;
		private DrawingCellHBox mainbox = null;
		private TimeText[] labels = new TimeText[8];
		private bool ignoreNextSeparator = false;
		private DrawingCellActivePixbuf clearimg = null;
		private DrawingCellActivePixbuf addimg = null;
		private DrawingCellButton dropdown = null;
		private DrawingCellEntry entry = null;
		private DrawingCellHBox labelcontents = null;
		
		private event StartCalculatorEvent startCalculator = null;
		/// <summary>
		/// Event triggered on start calculator 
		/// </summary>
		public event StartCalculatorEvent StartCalculator {
			add { startCalculator += value; }
			remove { startCalculator -= value; }
		}

		private event ClearDataEvent clearData = null;
		/// <summary>
		/// Event triggered on clear data
		/// </summary>
		public event ClearDataEvent ClearData {
			add { clearData += value; }
			remove { clearData -= value; }
		}

		private event DropDownEvent dropDown = null;
		/// <summary>
		/// Event triggered on drop down
		/// </summary>
		public event DropDownEvent DropDown {
			add { dropDown += value; }
			remove { dropDown -= value; }
		}

		private event TimeChangedEvent timeChanged = null;
		/// <summary>
		/// Event triggered on every time change
		/// </summary>
		public event TimeChangedEvent TimeChanged {
			add { timeChanged += value; }
			remove { timeChanged -= value; }
		}
		
		/// <summary>
		/// Executes TimeChanged event
		/// </summary>
		protected void OnTimeChanged()
		{
			if (timeChanged != null)
				timeChanged (this, new TimeChangedEventArgs (Time));
		}
		
		/// <summary>
		/// Exeutes StartCalculator event
		/// </summary>
		protected void OnStartCalculator()
		{
			if (startCalculator != null)
				startCalculator (this);
		}
		
		/// <summary>
		/// Executes ClearData event
		/// </summary>
		protected void OnClearData()
		{
			ResetEditing();
			if (clearData != null)
				clearData();
			else
				SetTime (0, 0, 0, 0);
			Selected = DataPart.Hour;
		}
		
		/// <summary>
		/// Executes DropDown event
		/// </summary>
		protected void OnDropDown()
		{
			if (dropDown != null)
				dropDown (this);
		}
		
		private bool hasDropDown = true;
		/// <value>
		/// Specifies if DropDown button is visible or not
		/// </value>
		public bool HasDropDown {
			get { return (hasDropDown); }
			set {
				if (hasDropDown == value)
					return;				
				hasDropDown = value;
				ResetEditingParts();
			}
		}
		
		private bool hasClearButton = true;
		/// <value>
		/// Specifies if DropDown button is visible or not
		/// </value>
		public bool HasClearButton {
			get { return (hasClearButton); }
			set {
				if (hasClearButton == value)
					return;				
				hasClearButton = value;
				ResetEditingParts();
			}
		}
		
		private bool hasCalculator = true;
		/// <value>
		/// Specifies if DropDown button is visible or not
		/// </value>
		public bool HasCalculator {
			get { return (hasCalculator); }
			set {
				if (hasCalculator == value)
					return;				
				hasCalculator = value;
				ResetEditingParts();
			}
		}
		
		private string currentPart = "";
		/// <value>
		/// Specifies value of current editing
		/// </value>
		[Browsable (false)]
		protected string CurrentPart {
			get { return (currentPart); }
			set { 
				if (currentPart == value)
					return;
				currentPart = value;
				PostEditing();
				if (GetTimePart(Selected) != null)
					GetTimePart (Selected).Refresh();
				QueueDraw();
			}
		}

		private bool editSeconds = true;
		/// <value>
		/// Specifies if editing of seconds is enabled
		/// </value>
		public bool EditSeconds {
			get { return (editSeconds); }
			set {
				if (editSeconds == value)
					return;
				editSeconds = value;
				ResetEditingParts();
			}
		}
		
		private bool editMilliseconds = true;
		/// <value>
		/// Specifies if editing of seconds is enabled
		/// </value>
		public bool EditMilliseconds {
			get { return (editMilliseconds); }
			set {
				if (editMilliseconds == value)
					return;
				editMilliseconds = value;
				ResetEditingParts();
			}
		}
		
		private bool twelveHour = true;
		/// <value>
		/// Specifies if widget uses 12 hour or 24
		/// </value>
		[Browsable (false)]
		protected bool TwelveHour {
			get { return (twelveHour); }
			set {
				if (twelveHour == value)
					return;
				max[0] = (value == true) ? 11 : 23;
				twelveHour = value;
				labels[7].Visible = value;
				ResetEditingParts();
			}
		}

		/// <value>
		/// Returns time as string in currently edited format
		/// </value>
		[Browsable (false)]
		public string TimeAsString {
			get {
				string s = "";
				for (int i=0; i<3; i++)
					s += labels[i].Text;
				if (EditSeconds == true) {
					s += labels[3].Text;
					s += labels[4].Text;
				}
				if ((EditSeconds == true) && (EditMilliseconds == true)) {
					s += labels[5].Text;
					s += labels[6].Text;
				}
				if (TwelveHour == true)
					s += labels[7].Text;
				return (s);
			}
		}
		
		/// <value>
		/// Returns time as string in currently edited format
		/// </value>
		/// <param name="aTime">
		/// Time <see cref="DateTime"/>
		/// </param>
		/// <returns>
		/// Time as string <see cref="System.String"/>
		/// </returns>
		public string GetTimeAsString (DateTime aTime) 
		{
			System.Globalization.CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
			string s = "";
			s += aTime.Hour.ToString("D2");
			s += labels[1].Text;
			s += aTime.Minute.ToString("D2");
			if (EditSeconds == true) {
				s += labels[3].Text;
				s += aTime.Second.ToString("D2");
			}
			if ((EditSeconds == true) && (EditMilliseconds == true)) {
				s += labels[5].Text;
				s += aTime.Millisecond.ToString("D2");
			}
			if (TwelveHour == true)
				s += (aTime.Hour >= 12) ? 
						" " + ci.DateTimeFormat.PMDesignator : " " + ci.DateTimeFormat.AMDesignator;
			return (s);
		}
		
		/// <summary>
		/// Resets editing parts
		/// </summary>
		protected void ResetEditingParts()
		{
			if (labels[3] == null)
				return;
			labels[3].Visible = (EditSeconds == true);
			labels[4].Visible = (EditSeconds == true);
			labels[5].Visible = ((EditSeconds == true) && (EditMilliseconds == true));
			labels[6].Visible = ((EditSeconds == true) && (EditMilliseconds == true));
			if (clearimg != null)
				clearimg.Visible = (HasClearButton == true);
			if (addimg != null)
				addimg.Visible = (HasCalculator == true);
			if (entry != null)
				entry.SideCut = (HasDropDown == true) ? SideCut.Right : SideCut.None;
			if (dropdown != null)
				dropdown.Visible = (HasDropDown == true);
			RefreshLabels();
			// Reset size if needed
			CellsChanged();
		}
		
		/// <summary>
		/// Gets label which displays specified part
		/// </summary>
		/// <param name="aPart">
		/// Part <see cref="DataPart"/>
		/// </param>
		/// <returns>
		/// Label cell <see cref="TimeText"/>
		/// </returns>
		private TimeText GetTimePart (DataPart aPart)
		{
			foreach (TimeText lbl in labels)
				if (lbl.Part == aPart)
					return (lbl);
			return (null);
		}

		/// <summary>
		/// Sets time by preserving the date part
		/// </summary>
		/// <param name="aHour">
		/// Hour <see cref="System.Int32"/>
		/// </param>
		/// <param name="aMinute">
		/// Minute <see cref="System.Int32"/>
		/// </param>
		/// <param name="aSecond">
		/// Second <see cref="System.Int32"/>
		/// </param>
		/// <param name="aMillisecond">
		/// Millisecond <see cref="System.Int32"/>
		/// </param>
		public void SetTime (int aHour, int aMinute, int aSecond, int aMillisecond)
		{
			Time = new DateTime (Time.Year, Time.Month, Time.Day, aHour, aMinute, aSecond, aMillisecond);
		}
		
		private DateTime time = DateTime.Now;
		/// <value>
		/// Time
		/// </value>
		[Browsable (false)]
		public DateTime Time {
			get { return (time); }
			set {
				if (time.Equals(value) == true)
					return;
				time = value;
				RefreshLabels();
				OnTimeChanged();
				OnPropertyChanged ("Time");
			}
		}
		
		private bool editable = true;
		/// <value>
		/// Specifies if widget is editable or not
		/// </value>
		public bool Editable {
			get { return (editable); }
			set { 
				if (editable == value)
					return;
				editable = value; 
			}
		}

		private DataPart selected = DataPart.None;
		/// <value>
		/// Specifies selected part
		/// </value>
		protected DataPart Selected {
			get { return (selected); }
			set {
				if (selected == value)
					return;
				ResetEditing();
				selected = value;
				switch (selected) {
				case DataPart.Hour:
					atStart = Time.Hour;
					break;
				case DataPart.Minute:
					atStart = Time.Minute;
					break;
				case DataPart.Second:
					atStart = Time.Second;
					break;
				case DataPart.Millisecond:
					atStart = Time.Millisecond;
					break;
				default:
					atStart = 0;
					break;
				}
				RefreshLabels();
			}
		}

		/// <summary>
		/// Refreshes labels and redraws
		/// </summary>
		protected void RefreshLabels()
		{
			foreach (TimeText t in labels)
				t.Refresh();
			QueueDraw();
		}
		
		/// <summary>
		/// Selects next part
		/// </summary>
		public void SelectNext()
		{
			atStart = 0;
			if (CurrentPart != "")
				atStart = System.Convert.ToInt32(CurrentPart);
			int sel = (int) Selected;
			if ((sel+1) < 4)
				Selected = (DataPart) sel+1;
		}
		
		/// <summary>
		/// Selects previous part
		/// </summary>
		public void SelectPrev()
		{
			atStart = 0;
			if (CurrentPart != "")
				atStart = System.Convert.ToInt32(CurrentPart);
			int sel = (int) Selected;
			if ((sel-1) > -1)
				Selected = (DataPart) sel-1;
		}
		
		/// <summary>
		/// Posts editing to current selection
		/// </summary>
		protected void PostEditing()
		{
			if (CurrentPart == "")
				return;
			int a = atStart;
			if (CurrentPart != "")
				a = System.Convert.ToInt32(CurrentPart);
			if (CurrentPart != null) {
				switch (Selected) {
				case DataPart.Hour:
					SetTime (a, Time.Minute, Time.Second, Time.Millisecond);
					break;
				case DataPart.Minute:
					SetTime (Time.Hour, a, Time.Second, Time.Millisecond);
					break;
				case DataPart.Second:
					SetTime (Time.Hour, Time.Minute, a, Time.Millisecond);
					break;
				case DataPart.Millisecond:
					SetTime (Time.Hour, Time.Minute, Time.Second, a);
					break;
				}
			}
		}

		/// <summary>
		/// Posts editing to current selection and resets
		/// </summary>
		private void ResetEditing()
		{
			PostEditing();
			ignoreNextSeparator = false;
			CurrentPart = "";
		}
		
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

		/// <summary>
		/// Handles focus in event
		/// </summary>
		/// <param name="evnt">
		/// Arguments <see cref="Gdk.EventFocus"/>
		/// </param>
		/// <returns>
		/// true if handled, false if not <see cref="System.Boolean"/>
		/// </returns>
		protected override bool OnFocusInEvent (Gdk.EventFocus evnt)
		{
			if (Editable == false)
				return (true);
			Selected = DataPart.Hour;
			return (base.OnFocusInEvent (evnt));
		}

		/// <summary>
		/// Handles focus out event
		/// </summary>
		/// <param name="evnt">
		/// Arguments <see cref="Gdk.EventFocus"/>
		/// </param>
		/// <returns>
		/// true if handled, false if not <see cref="System.Boolean"/>
		/// </returns>
		protected override bool OnFocusOutEvent (Gdk.EventFocus evnt)
		{
			Selected = DataPart.None;
			return (base.OnFocusOutEvent (evnt));
		}

		/// <summary>
		/// Handles SizeRequested message
		/// </summary>
		/// <param name="requisition">
		/// Requisition <see cref="Requisition"/>
		/// </param>
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			base.OnSizeRequested (ref requisition);
			if (requisition.Height < ChameleonTemplates.Entry.Requisition.Height)
				requisition.Height = ChameleonTemplates.Entry.Requisition.Height;
			double x,y=0;
			labels[0].GetSize (out x, out y);
			if (requisition.Height < (y*1.7))
				requisition.Height = System.Convert.ToInt32(y*1.7);
		}

		/// <summary>
		/// Handles KeyPress event
		/// </summary>
		/// <param name="evnt">
		/// Arguments <see cref="Gdk.EventKey"/>
		/// </param>
		/// <returns>
		/// true if handled, false if not <see cref="System.Boolean"/>
		/// </returns>
		protected override bool OnKeyPressEvent (Gdk.EventKey evnt)
		{
			switch (evnt.Key) {
			case Gdk.Key.Tab:
				return (base.OnKeyPressEvent (evnt));
				break;
			}
			if (Editable == false)
				return (true);
			switch (evnt.Key) {
			case Gdk.Key.Left:
				SelectPrev();
				return (true);
			case Gdk.Key.Right:
				SelectNext();
				return (true);
			case Gdk.Key.BackSpace:
				if (CurrentPart != "")
					CurrentPart = CurrentPart.Remove (CurrentPart.Length-1, 1);
				else {
					CurrentPart = atStart.ToString().Remove (atStart.ToString().Length-1, 1);
				}
				break;
			case Gdk.Key.Insert:
				if ((evnt.State & ModifierType.ControlMask) == ModifierType.ControlMask) {
					CopyToClipboard ((evnt.State & ModifierType.ShiftMask) == ModifierType.ShiftMask);
					return (true);
				}
				if ((evnt.State & ModifierType.ShiftMask) == ModifierType.ShiftMask) {
					ResetEditing();
					PasteFromClipboard();
					return (true);
				}
				return (false);
			case Gdk.Key.C:
			case Gdk.Key.c:
				if ((evnt.State & ModifierType.ControlMask) != ModifierType.ControlMask)
					return (false);
				ResetEditing();
				CopyToClipboard (false);
				return (true);
			case Gdk.Key.X:
			case Gdk.Key.x:
				if ((evnt.State & ModifierType.ControlMask) != ModifierType.ControlMask)
					return (false);
				ResetEditing();
				CopyToClipboard (true);
				return (true);
			case Gdk.Key.V:
			case Gdk.Key.v:
				if ((evnt.State & ModifierType.ControlMask) != ModifierType.ControlMask)
					return (false);
				ResetEditing();
				PasteFromClipboard();
				return (true);
			case Gdk.Key.Up:
			case Gdk.Key.Down:
				int inc = (evnt.Key == Gdk.Key.Down) ? -1 : 1;
				ResetEditing();
				switch (Selected) {
				case DataPart.Hour:
					if (((Time.Hour+inc) >= 0) && ((Time.Hour+inc) <= MaxNumber))
						SetTime (Time.Hour+inc, Time.Minute, Time.Second, Time.Millisecond);
					break;
				case DataPart.Minute:
					if (((Time.Minute+inc) >= 0) && ((Time.Minute+inc) <= MaxNumber))
						SetTime (Time.Hour, Time.Minute+inc, Time.Second, Time.Millisecond);
					break;
				case DataPart.Second:
					if (((Time.Second+inc) >= 0) && ((Time.Second+inc) <= MaxNumber))
						SetTime (Time.Hour, Time.Minute, Time.Second+inc, Time.Millisecond);
					break;
				case DataPart.Millisecond:
					if (((Time.Millisecond+inc) >= 0) && ((Time.Millisecond+inc) <= MaxNumber))
						SetTime (Time.Hour, Time.Minute, Time.Second, Time.Millisecond+inc);
					break;
				}
				return (true);
			}
			return (base.OnKeyPressEvent (evnt));
		}

		/// <summary>
		/// Handles KeyRelease event
		/// </summary>
		/// <param name="evnt">
		/// Arguments <see cref="Gdk.EventKey"/>
		/// </param>
		/// <returns>
		/// true if handled, false if not <see cref="System.Boolean"/>
		/// </returns>
		protected override bool OnKeyReleaseEvent (Gdk.EventKey evnt)
		{
			switch (evnt.Key) {
			case Gdk.Key.Tab:
				return (base.OnKeyReleaseEvent (evnt));
				break;
			}
			if (Editable == false)
				return (true);
			if (((char) evnt.KeyValue == '.') ||
			    ((char) evnt.KeyValue == '.')) {
				if (ignoreNextSeparator == true) {
					ignoreNextSeparator = false;
					return (true);
				}
				SelectNext();
				return (true);
			}
			if (TwelveHour == true) {
				if (((char) evnt.KeyValue).ToString().ToLower() == "a") {
					ResetEditing();
					if (Time.Hour > 11) {
						CurrentPart = (Time.Hour - 12).ToString();
						ResetEditing();
						RefreshLabels();
					}
				}
				if (((char) evnt.KeyValue).ToString().ToLower() == "p") {
					ResetEditing();
					if (Time.Hour < 12) {
						CurrentPart = (Time.Hour + 12).ToString();
						ResetEditing();
						RefreshLabels();
					}
				}
			}
			ignoreNextSeparator = false;
			if (char.IsDigit((char)evnt.KeyValue) == true) {
				string s = CurrentPart + (char) evnt.KeyValue;
				int i = System.Convert.ToInt32 (s);
				if (i <= MaxNumber) {
					CurrentPart = i.ToString();
					if ((i*10) > MaxNumber) {
						SelectNext();
						ignoreNextSeparator = true;
					}
				}
			}
			switch (evnt.Key) {
			case Gdk.Key.Delete:
				ResetEditing();
				OnClearData();
				break;
			case Gdk.Key.KP_Add:
			case Gdk.Key.KP_Subtract:
			case Gdk.Key.plus:
			case Gdk.Key.minus:
				ResetEditing();
				OnStartCalculator();
				break;
			}
			return base.OnKeyReleaseEvent (evnt);
		}

		public override Gdk.Pixbuf GetDragPixbuf()
		{
			return (labelcontents.GetAsPixbuf());
		}
		
		/// <summary>
		/// Handles ButtonPress event
		/// </summary>
		/// <param name="evnt">
		/// Arguments <see cref="Gdk.EventButton"/>
		/// </param>
		/// <returns>
		/// true if handled, false if not <see cref="System.Boolean"/>
		/// </returns>
		protected override bool OnButtonPressEvent (Gdk.EventButton evnt)
		{
			if (CanFocus == true)
				if (HasFocus == false) {
					GrabFocus();
					return (true);
				}
			if (Editable == false)
				return (base.OnButtonPressEvent (evnt));
			IDrawingCell cell = mainbox.CellAtCoordinates (System.Convert.ToInt32(evnt.X), System.Convert.ToInt32(evnt.Y));
			if (cell == null)
				return (base.OnButtonPressEvent (evnt));
			if (TypeValidator.IsCompatible(cell.GetType(), typeof(TimeText)) == true)
				if ((int) (cell as TimeText).Part < 4) {
					Selected = (cell as TimeText).Part;
					return (true);
				}
			return (base.OnButtonPressEvent (evnt));
		}

		#region ClipboardAndDragging

		/// <summary>
		/// Stores buffer
		/// </summary>
		protected void StoreBuffer()
		{
			ResetEditing();
			copyBuffer = new DateTime (Time.Year, Time.Month, Time.Day, Time.Hour, Time.Minute, Time.Second, Time.Millisecond);
		}
		
		protected void HandleDragBegin(object o, DragBeginArgs args)
		{
			Gdk.Pixbuf dragpic = GetDragPixbuf();
			Gtk.Drag.SetIconPixbuf (args.Context, dragpic, 0, 0);
			StoreBuffer();
		}

		protected void HandleDragDataGet(object o, DragDataGetArgs args)
		{
			for (int i=0; i<ValidTargets.Length; i++)
				FillSelectionData (args.SelectionData, (i<ValidTargets.Length-1) ? (uint) 0 : (uint) 1);
			args.SelectionData.Text = GetTimeAsString (copyBuffer);
		}

		protected void HandleDragDataReceived(object o, DragDataReceivedArgs args)
		{
			if (Editable == false)
				return;
			
			Gtk.Widget source = Gtk.Drag.GetSourceWidget (args.Context);
			HandleDataReceived (args.Info, source, args.SelectionData);
			source = null;
		}

		protected void HandleDataReceived (uint aType, Gtk.Widget aSource, SelectionData aData)
		{
			if (Editable == false)
				return;
			ResetEditing();
			string text = "";
			switch (aType) {
			case (int) TransferDataType.Default:
				text = System.Text.Encoding.UTF8.GetString (aData.Data);
				break;
			case (int) TransferDataType.X_Special:
				int intsize = (BitConverter.GetBytes((int) 1).Length);
				int h = BitConverter.ToInt32 (aData.Data, 0);
				int m = BitConverter.ToInt32 (aData.Data, intsize);
				int s = BitConverter.ToInt32 (aData.Data, intsize * 2);					
				int ms = BitConverter.ToInt32 (aData.Data, intsize * 2);					
				SetTime (h, m, s, ms);
				return;
			}		
			if (text != "")
				try {
					DateTime dt = DateTime.Parse (text);
					SetTime (dt.Hour, dt.Minute, dt.Second, dt.Millisecond);
				}
				catch {}
			text = "";
		}

		void FillSelectionData (SelectionData aData, uint aInfo) 
		{
			if (aInfo < (int) TransferDataType.X_Special)
				aData.Set (aData.Target, 8, System.Text.Encoding.UTF8.GetBytes (GetTimeAsString (copyBuffer)),
				           System.Text.Encoding.UTF8.GetBytes (GetTimeAsString(copyBuffer)).Length);
			else {
				byte[] num = BitConverter.GetBytes (copyBuffer.Hour);
				byte[] data = new byte[num.Length*4];
				num.CopyTo (data, 0);
				num = BitConverter.GetBytes (copyBuffer.Minute);
				num.CopyTo (data, num.Length);
				num = BitConverter.GetBytes (copyBuffer.Second);
				num.CopyTo (data, num.Length*2);
				num = BitConverter.GetBytes (copyBuffer.Millisecond);
				num.CopyTo (data, num.Length*3);
				aData.Set (aData.Target, 8, data, data.Length);
			}
		}
		
		private void CopyToClipboard (bool aCut)
		{			
			StoreBuffer();
			Clipboard clipboard = Clipboard.Get(Gdk.Atom.Intern("CLIPBOARD", true));
			clipboard.SetWithData (ValidTargets,
				delegate (Clipboard aClipboard, SelectionData aData, uint aInfo) {
					aClipboard.Text = GetTimeAsString (copyBuffer);
					FillSelectionData (aData, aInfo);
				},
				delegate (Clipboard cb) {
				}
			);
			if (aCut == true)
				OnClearData();
		}

		private void PasteFromClipboard()
		{
			Clipboard clipboard = ClipboardHelper.GetGlobalClipboard();
			bool handled = false;
			// Request types
			clipboard.RequestContents(Gdk.Atom.Intern(x_SpecialName, true), delegate(Clipboard cb, SelectionData data) {
				if (data.Length > -1) {
					handled = true;
					HandleDataReceived ((int) TransferDataType.X_Special, null, data);
				}
			});

			foreach (string s in ClipboardHelper.TextTargetList) {
				clipboard.RequestContents(Gdk.Atom.Intern(s, true), delegate(Clipboard cb, SelectionData data) {
					if (data.Length > -1) {
						handled = true;
						HandleDataReceived ((int) TransferDataType.Default, null, data);
					}
				});
				if (handled == true)
					break;
			}
		}			

		#endregion ClipboardAndDragging

		/// <summary>
		/// Creates DrawingCellBox
		/// </summary>
		/// <returns>
		/// Created box <see cref="DrawingCellBox"/>
		/// </returns>
		protected override DrawingCellBox CreateBox ()
		{
			mainbox = new DrawingCellHBox();
			return (mainbox);
		}

		public TimeEntry()
			: base()
		{
			CanFocus = true;
			Spacing = 2;
			
			int width, height;
			Icon.SizeLookup(icon_size, out width, out height);
			IconTheme theme = IconTheme.GetForScreen(Screen);

			DrawingCellHBox entrycontents = new DrawingCellHBox();
			entrycontents.PackStart (new DrawingCellNull(), true);
			entrycontents.Spacing = 0;

			labelcontents = new DrawingCellHBox();
			entrycontents.PackEnd (labelcontents, false);
			labels[0] = new TimeText (this, DataPart.Hour);
			labels[1] = new TimeText (this, DataPart.HourSeparator);
			labels[2] = new TimeText (this, DataPart.Minute);
			labels[3] = new TimeText (this, DataPart.HourSeparator);
			labels[4] = new TimeText (this, DataPart.Second);
			labels[5] = new TimeText (this, DataPart.MillisecondSeparator);
			labels[6] = new TimeText (this, DataPart.Millisecond);
			labels[7] = new TimeText (this, DataPart.AMPM);
			foreach (TimeText t in labels) {
				t.XPos = 0.5;
				t.YPos = 0.5;
				labelcontents.PackEnd (t, false);
			}
			
			string s = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
			bool res = (s.ToLower().IndexOf("t") > -1);
			if (res == false) {
				s = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongTimePattern;
				res = (s.ToLower().IndexOf("t") > -1);
			}
			TwelveHour = res;

			clearimg = new DrawingCellActivePixbuf (theme.LoadIcon(Stock.Clear, width, 0));
			clearimg.Activated += delegate {
				OnClearData();
			};
			entrycontents.PackStart (clearimg, false);
			
			addimg = new DrawingCellActivePixbuf (theme.LoadIcon(Stock.Add, width, 0));
			addimg.Activated += delegate {
				OnStartCalculator();
			};
			entrycontents.PackStart (addimg, false);

			entry = new DrawingCellEntry (entrycontents);
//			entry.Padding = 1;
			entry.SideCut = SideCut.Right;
			PackStart (entry, true);
			
			dropdown = new DrawingCellButton (new DrawingCellComboArrow());
			dropdown.SideCut = SideCut.Left;
			dropdown.Activated += delegate {
				OnDropDown();
			};

			PackEnd (dropdown, false);
			
			Gtk.Drag.DestSet (this, DestDefaults.All, ValidTargets, Gdk.DragAction.Copy);
			DragDataReceived += HandleDragDataReceived;
			
			Gtk.Drag.SourceSet (this, Gdk.ModifierType.Button1Mask, ValidTargets, DragAction.Copy);
			DragBegin += HandleDragBegin;
			DragDataGet += HandleDragDataGet;
		}
	}
}
