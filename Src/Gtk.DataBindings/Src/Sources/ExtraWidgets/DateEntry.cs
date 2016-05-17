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

using Cairo;
using Pango;
using System;
using System.ComponentModel;
using System.Data.Bindings;
using System.Threading;
using Gtk.ExtraWidgets;
using Gdk;
using GLib;

namespace Gtk.DataBindings
{
	[ToolboxItem(true)]
	[Category ("Extra Gtk Widgets")]
	public class DateEntry : CellDrawingArea, IEditable, CellEditableImplementor
	{
		/// <value>
		/// Data parts
		/// </value>
		protected enum DataPart 
		{
			Day = 0,
			Month = 1,
			Year = 2,
			Separator = 3,
			None = 4
		}

		#region TimeText
		
		protected class DateText : DrawingCellEditText
		{
			private DateEntry owner = null;
			
			private DataPart part = DataPart.Separator;
			/// <value>
			/// Specifies part which is edited
			/// </value>
			public DataPart Part {
				get { return (part); }
			}
			
			public override bool Error {
				get {
					if (Part == DataPart.Day)
						return (owner.Day != owner.EntredDay);
					return (base.Error);
				}
				set { base.Error = value; }
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
				case DataPart.Day:
					Text = owner.Date.Day.ToString("D2");
					break;
				case DataPart.Month:
					Text = owner.Date.Month.ToString("D2");
					break;
				case DataPart.Year:
					Text = owner.Date.Year.ToString("D4");
					break;
				}
			}
			
			public DateText (DateEntry aOwner, DataPart aPart)
				: base ()
			{
				XPos = 0.5;
				YPos = 0.5;
				part = aPart;
				owner = aOwner;
				switch (aPart) {
				case DataPart.Year:
					SizeText = " <b>8888</b>";
					break;
				case DataPart.Separator:
					Text = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.DateSeparator;
					break;
				default:
					SizeText = "<b>88</b>";
					break;
				}
				Refresh();
			}
		}
		
		#endregion TextEdit
		
		static GType type;

		/// <value>
		/// Specifies max number which can be entred in selected part
		/// </value>
		protected int MaxNumber {
			get { 
				switch (Selected) {
				case DataPart.Day:
					return (31);
				case DataPart.Month:
					return (12);
				case DataPart.Year:
					return (9999);
				}
				return (0);
			}
		}

		private DateTime copyBuffer = DateTime.Now;
		private static string x_SpecialName = "application/x-dotnet-date";
		
		private static TargetEntry[] validTargets = ClipboardHelper.GetTextTargetsWithSpecial (x_SpecialName, (int) TransferDataType.X_Special);
		protected static TargetEntry[] ValidTargets {
			get { return (validTargets); }
		}
		
		private int atStart = 0;
		private IDrawingCell stateCell = null;
		private IconSize icon_size = IconSize.Menu;
		private DrawingCellHBox mainbox = null;
		private DateText[] labels = new DateText[5];
		private bool ignoreNextSeparator = false;
		private DrawingCellActivePixbuf clearimg = null;
		private DrawingCellActivePixbuf addimg = null;
		private DrawingCellButton dropdown = null;
		private DrawingCellEntry entry = null;
		private DrawingCellHBox labelcontents = null;
		private DrawingCellHBox entrycontents = null;
		private string[] laststr = new string[3] { "", "", "" };
		private bool[] lastvis = new bool[3] { false, false, false };
		
		protected DataPart ResolveMode (char c)
		{
			switch (c) {
			case 'd':
				return (DataPart.Day);
			case 'm':
				return (DataPart.Month);
			case 'y':
				return (DataPart.Year);
			}
			return (DataPart.None);
		}

		protected string GetShortMask()
		{
			string s = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
			s = s.ToLower();
			string d = "";
			string vchars = "dmy";
			for (int i=0; i<s.Length; i++)
				if (vchars.IndexOf(s[i]) > -1)
					if (d.IndexOf(s[i]) == -1)
						d += s[i];
			return (d);
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
				PostEditing(false);
//				if (GetDatePart(Selected) != null)
//					GetDatePart(Selected).Refresh(); 
				RefreshLabels();				
//				QueueDraw();
			}
		}

		public int MaxDaysInCurrentMonth {
			get { return (DateTime.DaysInMonth(Date.Year, Date.Month)); }
		}
		
		public int MaxDaysInMonth (int aMonth, int aYear) 
		{
			return (DateTime.DaysInMonth(aYear, aMonth));
		}
		
		private int GetYear (int aYear)
		{
			if (aYear < 20)
				return (2000 + aYear);
			if (aYear < 100)
				return (1900 + aYear);
			return (aYear);
		}
		
		private int day = -1;
		/// <value>
		/// Specifies day which is valid
		/// </value>
		[Browsable (false)]
		protected int Day {
			get { 
				if (day == -1)
					return (Date.Day);
				return ((day > MaxDaysInCurrentMonth) ? MaxDaysInCurrentMonth : day);
			}
			set { day = value; }
		}
		
		/// <value>
		/// Specifies date which was entred
		/// </value>
		[Browsable (false)]
		protected int EntredDay {
			get { 
				if (day == -1)
					return (Day);
				return (day); 
			}
		}

		protected void _SetDate (int aYear, int aMonth, int aDay)
		{
			Date = new DateTime (aYear, aMonth, aDay, Date.Hour, Date.Minute, Date.Second, Date.Millisecond);
		}
		
		private void SetCorrectDateTime (int aYear, int aMonth, int aDay)
		{
			if (Selected == DataPart.Day)
				Day = aDay;
			if (EntredDay > MaxDaysInMonth(aMonth, aYear))
				_SetDate (aYear, aMonth, MaxDaysInMonth(aMonth, aYear));
			else
				_SetDate (aYear, aMonth, EntredDay);
		}

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

		private event DateChangedEvent dateChanged = null;
		/// <summary>
		/// Event triggered on every date change
		/// </summary>
		public event DateChangedEvent DateChanged {
			add { dateChanged += value; }
			remove { dateChanged -= value; }
		}
		
		/// <summary>
		/// Executes DateChanged event
		/// </summary>
		protected virtual void OnDateChanged()
		{
			if (dateChanged != null)
				dateChanged (this, new DateChangedEventArgs (Date));
		}
		
		private void EventOnPopupChange (object sender, DateEventArgs args)
		{
			Date = args.Date;
		}
		
		private void EventOnClosePopup (object sender, EventArgs args)
		{
			GrabFocus();
		}
		
		/// <summary>
		/// Exeutes StartCalculator event
		/// </summary>
		public void OnStartCalculator()
		{
			if (startCalculator != null)
				startCalculator (this);
			else {
				int x, y, w;
				ParentWindow.GetPosition(out x,out y);	
				x += Allocation.Left;
				y += Allocation.Top + Allocation.Height;
				w = Allocation.Right - Allocation.Left;
				DateCalculatorWindow wnd = new DateCalculatorWindow (x, y, w, 
				                                                     new DateTime (Date.Year, Date.Month, Date.Day),
				                                                     EventOnPopupChange, EventOnClosePopup);
			}
		}
		
		/// <summary>
		/// Executes ClearData event
		/// </summary>
		protected void OnClearData()
		{
			ResetEditing();
			if (clearData != null)
				clearData();
			else {
				DateTime dt = DateTime.Now;
				SetCorrectDateTime (dt.Year, dt.Month, dt.Day);
			}
			Selected = labels[0].Part;
		}
		
		/// <summary>
		/// Executes DropDown event
		/// </summary>
		protected void OnDropDown()
		{
			if (dropDown != null)
				dropDown (this);
			else {
				int x, y, w;
				ParentWindow.GetPosition(out x,out y);	
				x += Allocation.Left;
				y += Allocation.Top + Allocation.Height;
				w = Allocation.Right - Allocation.Left;
				DatePickerWindow wnd = new DatePickerWindow (x, y, w, 
				                                             new DateTime (Date.Year, Date.Month, Date.Day),
				                                             EventOnPopupChange, EventOnClosePopup);
			}
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
		
		/// <value>
		/// Returns time as string in currently edited format
		/// </value>
		[Browsable (false)]
		public string TimeAsString {
			get {
				string s = "";
				for (int i=0; i<5; i++)
					s += labels[i].Text;
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
		public string GetTimeAsString (DateTime aDate) 
		{
			return (aDate.ToShortDateString());
		}
		
		/// <summary>
		/// Resets editing parts
		/// </summary>
		protected void ResetEditingParts()
		{
			if (clearimg != null)
				clearimg.Visible = (HasClearButton == true);
			if (addimg != null)
				addimg.Visible = (HasCalculator == true);
			if (entry != null)
				entry.SideCut = (HasDropDown == true) ? SideCut.Right : SideCut.None;
			if (dropdown != null)
				dropdown.Visible = (HasDropDown == true);
			if (labels[0] != null) {
				RefreshLabels();
				// Reset size if needed
				CellsChanged();
			}
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
		private DateText GetDatePart (DataPart aPart)
		{
			foreach (DateText lbl in labels)
				if (lbl.Part == aPart)
					return (lbl);
			return (null);
		}

		private DateTime date = DateTime.Now;
		/// <value>
		/// Time
		/// </value>
		[Browsable (false)]
		public DateTime Date {
			get { return (date); }
			set {
				if (date.Equals(value) == true)
					return;
				date = value;
				RefreshLabels();
				OnDateChanged();
				OnPropertyChanged ("Date");
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
				case DataPart.Day:
					atStart = Date.Day;
					break;
				case DataPart.Month:
					atStart = Date.Month;
					break;
				case DataPart.Year:
					atStart = Date.Year;
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
			foreach (DateText t in labels)
				if ((t.Part != DataPart.Separator) && (t.Part != DataPart.None))
					t.Refresh();
			if ((laststr[0] == labels[0].DisplayText) &&
				(laststr[1] == labels[2].DisplayText) &&
				(laststr[2] == labels[4].DisplayText) &&
				(lastvis[0] == addimg.Visible) &&
				(lastvis[1] == clearimg.Visible) &&
				(lastvis[2] == dropdown.Visible))
				return;

			laststr[0] = labels[0].DisplayText;
			laststr[1] = labels[2].DisplayText;
			laststr[2] = labels[4].DisplayText;
			lastvis[0] = addimg.Visible;
			lastvis[1] = clearimg.Visible;
			lastvis[2] = dropdown.Visible;
			QueueDraw();
		}

		private int IndexOf (DataPart aIndex)
		{
			int sel = -1;
			for (int i=0; i<5; i++)
				if (Selected == labels[i].Part)
					return (i);
			return (-1);
		}
		
		/// <summary>
		/// Selects next part
		/// </summary>
		public void SelectNext()
		{
			atStart = 0;
			if (CurrentPart != "")
				atStart = System.Convert.ToInt32(CurrentPart);
					
			int sel = IndexOf (Selected);
			if ((sel+2) < 5)
				Selected = labels[sel+2].Part;
			ResetEditing();
		}
		
		/// <summary>
		/// Selects previous part
		/// </summary>
		public void SelectPrev()
		{
			atStart = 0;
			if (CurrentPart != "")
				atStart = System.Convert.ToInt32(CurrentPart);
			int sel = IndexOf (Selected);
			if ((sel-2) > -1)
				Selected = labels[sel-2].Part;
			ResetEditing();
		}
		
		/// <summary>
		/// Posts editing to current selection
		/// </summary>
		protected void PostEditing(bool aChanging)
		{
			if (CurrentPart == "")
				return;
			int a = atStart;
			if (CurrentPart != "")
				a = System.Convert.ToInt32(CurrentPart);
			if ((CurrentPart != null) && (CurrentPart != "")) {
				switch (Selected) {
				case DataPart.Year:
					if (Selected == DataPart.Year) {
						if (CurrentPart.Length > 0) {
							if (CurrentPart.Length < 4)
//								SetCorrectDateTime (GetYear(System.Convert.ToInt32(current_part)), Date.Month, Date.Day);
								SetCorrectDateTime ((aChanging == true) ? GetYear(System.Convert.ToInt32(CurrentPart)) : System.Convert.ToInt32(CurrentPart), Date.Month, Day);
							else
//								SetCorrectDateTime (System.Convert.ToInt32(current_part), Date.Month, Date.Day);
								SetCorrectDateTime (System.Convert.ToInt32(CurrentPart), Date.Month, Day);
						}
					}
//					SetCorrectDateTime (a, Date.Month, Date.Day);
					break;
				case DataPart.Month:
//					SetCorrectDateTime (Date.Year, a, Date.Day);
					SetCorrectDateTime (Date.Year, a, Day);
					break;
				case DataPart.Day:
//					SetCorrectDateTime (Date.Year, Date.Month, a);
					SetCorrectDateTime (Date.Year, Date.Month, a);
					break;
				}
			}
		}

		/// <summary>
		/// Posts editing to current selection and resets
		/// </summary>
		private void ResetEditing()
		{
			PostEditing(true);
			ignoreNextSeparator = false;
			CurrentPart = "";
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
			Selected = labels[0].Part;
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
			ResetEditing();
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
			if (IsCellRenderer == true) {
				double w,h;
				entrycontents.GetCellSize (out w, out h);
				requisition.Width = System.Convert.ToInt32(w);
				requisition.Height = System.Convert.ToInt32(h)+1;
				return;
			}
			base.OnSizeRequested (ref requisition);
			if (requisition.Height < ChameleonTemplates.Entry.Requisition.Height)
				requisition.Height = ChameleonTemplates.Entry.Requisition.Height;
			double x,y=0;
			labels[0].Arguments.Start (CellAction.GetSize, new CellExposeEventArgs (null, null, this.GdkWindow, null, null));
			labels[0].Arguments.PassedArguments.Widget = this;
			labels[0].GetSize (out x, out y);
			labels[0].Arguments.Stop();
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
			case Gdk.Key.Escape:
				if (IsCellRenderer == true) {
					System.Console.WriteLine("Escape");
				}
				return (false);
				break;
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
				case DataPart.Day:
					if (((Date.Day+inc) > 0) && ((Date.Day+inc) <= MaxDaysInCurrentMonth))
						SetCorrectDateTime (Date.Year, Date.Month, Date.Day+inc);
					break;
				case DataPart.Month:
					if (((Date.Month+inc) > 0) && ((Date.Month+inc) <= MaxNumber))
						SetCorrectDateTime (Date.Year, Date.Month+inc, Date.Day);
					break;
				case DataPart.Year:
					if (((Date.Year+inc) > 0) && ((Date.Year+inc) <= MaxNumber))
						SetCorrectDateTime (Date.Year+inc, Date.Month, Date.Day);
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
			if ((char) evnt.KeyValue == System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.DateSeparator[0]) {
				if (ignoreNextSeparator == true) {
					ignoreNextSeparator = false;
					return (true);
				}
				SelectNext();
				return (true);
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
			if (HasFocus == false)
				return (base.OnButtonPressEvent (evnt));
			IDrawingCell cell = mainbox.CellAtCoordinates (System.Convert.ToInt32(evnt.X), System.Convert.ToInt32(evnt.Y));
			if (cell == null)
				return (base.OnButtonPressEvent (evnt));
			if (TypeValidator.IsCompatible(cell.GetType(), typeof(DateText)) == true)
				if ((int) (cell as DateText).Part < 3) {
					Selected = (cell as DateText).Part;
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
			copyBuffer = new DateTime (Date.Year, Date.Month, Date.Day, Date.Hour, Date.Minute, Date.Second, Date.Millisecond);
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
				int y = BitConverter.ToInt32 (aData.Data, 0);
				int m = BitConverter.ToInt32 (aData.Data, intsize);
				int d = BitConverter.ToInt32 (aData.Data, intsize * 2);
				day = d;
				SetCorrectDateTime (y, m, d);
				return;
			}		
			if (text != "")
				try {
					DateTime dt = DateTime.Parse (text);
					day = dt.Day;
					SetCorrectDateTime (dt.Year, dt.Month, dt.Day);
				}
				catch {}
			text = "";
		}

		void FillSelectionData (SelectionData aData, uint aInfo) 
		{
			if (aInfo < (int) TransferDataType.X_Special)
				aData.Set (aData.Target, 8, System.Text.Encoding.UTF8.GetBytes (copyBuffer.ToShortDateString()),
				           System.Text.Encoding.UTF8.GetBytes (copyBuffer.ToShortDateString()).Length);
			else {
				byte[] num = BitConverter.GetBytes (copyBuffer.Year);
				byte[] data = new byte[num.Length*3];
				num.CopyTo (data, 0);
				num = BitConverter.GetBytes (copyBuffer.Month);
				num.CopyTo (data, num.Length);
				num = BitConverter.GetBytes (copyBuffer.Day);
				num.CopyTo (data, num.Length*2);
				aData.Set (aData.Target, 8, data, data.Length);
			}
		}
		
		private void CopyToClipboard (bool aCut)
		{			
			StoreBuffer();
			Clipboard clipboard = Clipboard.Get(Gdk.Atom.Intern("CLIPBOARD", true));
			clipboard.SetWithData (ValidTargets,
				delegate (Clipboard aClipboard, SelectionData aData, uint aInfo) {
					aClipboard.Text = copyBuffer.ToShortDateString();
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


		public DateEntry()
			: base()
		{
			CanFocus = true;
			Spacing = 2;
			
			int width, height;
			Icon.SizeLookup(icon_size, out width, out height);
			IconTheme theme = IconTheme.GetForScreen(Screen);

			entrycontents = new DrawingCellHBox();
			entrycontents.PackStart (new DrawingCellNull(), true);
			entrycontents.Spacing = 0;

			string mask = GetShortMask();
			labelcontents = new DrawingCellHBox();
			entrycontents.PackEnd (labelcontents, false);
			labels[0] = new DateText (this, ResolveMode(mask[0]));
			labels[1] = new DateText (this, DataPart.Separator);
			labels[2] = new DateText (this, ResolveMode(mask[1]));
			labels[3] = new DateText (this, DataPart.Separator);
			labels[4] = new DateText (this, ResolveMode(mask[2]));
			foreach (DateText t in labels)
				labelcontents.PackEnd (t, false);

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

		#region CellEditableImplementor implementation
		public void StartEditing (Event evnt)
		{
			System.Console.WriteLine("DateEntry.StartEditing()");
//			throw new System.NotImplementedException();
		}
		#endregion

	}
}
