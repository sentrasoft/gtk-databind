
using System;
using System.ComponentModel;
using System.Net;

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// Specifies widget usable for entering ip
	/// </summary>
	[ToolboxItem (true)]
	[Category ("Extra Gtk Widgets")]
	public class IPv4Entry : CellDrawingArea
	{
		protected enum DataPart
		{
			A = 0,
			B = 2,
			C = 4,
			D = 6,
			Separator = 1,
			None = 8
		}
		
		protected class IpText : DrawingCellEditText
		{
			private DataPart part = DataPart.Separator;
			public DataPart Part {
				get { return (part); }
			}

			private IPv4Entry entry = null;
			
			public override bool Selected {
				get { return (entry.Selected == Part); }
				set { 
					if (Selected == value)
						return;
					entry.Selected = Part; 
					OnPropertyChanged ("Selected");
				}
			}

			public void Refresh()
			{
				switch (Part) {
				case DataPart.A:
					Text = entry.Ip.GetAddressBytes()[0].ToString();
					break;
				case DataPart.B:
					Text = entry.Ip.GetAddressBytes()[1].ToString();
					break;
				case DataPart.C:
					Text = entry.Ip.GetAddressBytes()[2].ToString();
					break;
				case DataPart.D:
					Text = entry.Ip.GetAddressBytes()[3].ToString();
					break;
				}
			}
			
			public IpText (IPv4Entry aEntry, DataPart aPart)
				: base()
			{
				entry = aEntry;
				part = aPart;
				if (aPart == DataPart.Separator)
					Text = ".";
				else {
					Text = "255";
					SizeText = "<b>255</b>";
				}
			}
		}
		
		private DrawingCellHBox mainbox = null;
		private IpText[] cells = new IpText[7];
		private DrawingCellActivePixbuf clearimg = null;
		private DrawingCellActivePixbuf addimg = null;
		private DrawingCellButton dropdown = null;
		private DrawingCellEntry entry = null;
		private DrawingCellHBox labelcontents = null;

		private IconSize icon_size = IconSize.Menu;
		private bool ignoreNextSeparator = false;
		
		private byte[] copyBuffer = new byte[4] {0, 0, 0, 0};
		private static string x_SpecialName = "application/x-dotnet-ipv4";
		
		private static TargetEntry[] validTargets = ClipboardHelper.GetTextTargetsWithSpecial (x_SpecialName, (int) TransferDataType.X_Special);
		protected static TargetEntry[] ValidTargets {
			get { return (validTargets); }
		}		

		private byte atStart = 0;
		private string currentPart = "";
		protected string CurrentPart {
			get { return (currentPart); }
			set { 
				if (currentPart == value)
					return;
				currentPart = value;
				PostEditing();
				GetIpPart (Selected).Refresh();
				QueueDraw();
			}
		}
		
		private IpText GetIpPart (DataPart aPart)
		{
			foreach (IpText lbl in cells)
				if (lbl.Part == aPart)
					return (lbl);
			return (null);
		}
		
		protected void PostEditing()
		{
			if (CurrentPart == "")
				return;
			byte a = atStart;
			if (CurrentPart != "")
				a = System.Convert.ToByte(CurrentPart);
			if (CurrentPart != null) {
				switch (Selected) {
				case DataPart.A:
					Ip = new IPAddress (new byte[4] { a, Ip.GetAddressBytes()[1], Ip.GetAddressBytes()[2], Ip.GetAddressBytes()[3] });
					break;
				case DataPart.B:
					Ip = new IPAddress (new byte[4] { Ip.GetAddressBytes()[0], a, Ip.GetAddressBytes()[2], Ip.GetAddressBytes()[3] });
					break;
				case DataPart.C:
					Ip = new IPAddress (new byte[4] { Ip.GetAddressBytes()[0], Ip.GetAddressBytes()[1], a, Ip.GetAddressBytes()[3] });
					break;
				case DataPart.D:
					Ip = new IPAddress (new byte[4] { Ip.GetAddressBytes()[0], Ip.GetAddressBytes()[1], Ip.GetAddressBytes()[2], a });
					break;
				}
			}
		}

		private void ResetEditing()
		{
			PostEditing();
			ignoreNextSeparator = false;
			CurrentPart = "";
		}
		
		private DataPart selected = DataPart.None;
		protected DataPart Selected {
			get { return (selected); }
			set {
				if (selected == value)
					return;
				ResetEditing();
				selected = value;
				switch (selected) {
				case DataPart.A:
					atStart = Ip.GetAddressBytes()[0];
					break;
				case DataPart.B:
					atStart = Ip.GetAddressBytes()[1];
					break;
				case DataPart.C:
					atStart = Ip.GetAddressBytes()[2];
					break;
				case DataPart.D:
					atStart = Ip.GetAddressBytes()[3];
					break;
				default:
					atStart = 0;
					break;
				}
				RefreshLabels();
			}
		}
		
		protected void RefreshLabels()
		{
			GetIpPart (DataPart.A).Refresh();
			GetIpPart (DataPart.B).Refresh();
			GetIpPart (DataPart.C).Refresh();
			GetIpPart (DataPart.D).Refresh();
			QueueDraw();
		}
		
		public void SelectNext()
		{
			atStart = 0;
			if (CurrentPart != "")
				atStart = System.Convert.ToByte(CurrentPart);
			int sel = (int) Selected;
			if ((sel+2) < 7)
				Selected = (DataPart) sel+2;
		}
		
		public void SelectPrev()
		{
			atStart = 0;
			if (CurrentPart != "")
				atStart = System.Convert.ToByte(CurrentPart);
			int sel = (int) Selected;
			if ((sel-2) > -1)
				Selected = (DataPart) sel-2;
		}
		
		private IPAddress ip = new IPAddress(new byte[4] { 0, 0, 0, 0 });
		[Browsable (false)]
		public IPAddress Ip {
			get { return (ip); }
			set {
				if (ip.Equals(value) == true)
					return;
				ip = value;
				OnPropertyChanged ("Ip");
			}
		}
		
		private event ClearDataEvent clearData = null;
		public event ClearDataEvent ClearData {
			add { clearData += value; }
			remove { clearData -= value; }
		}

		private event StartCalculatorEvent startCalculator = null;
		/// <summary>
		/// Event triggered on start calculator 
		/// </summary>
		public event StartCalculatorEvent StartCalculator {
			add { startCalculator += value; }
			remove { startCalculator -= value; }
		}

		private event DropDownEvent dropDown = null;
		public event DropDownEvent DropDown {
			add { dropDown += value; }
			remove { dropDown -= value; }
		}

		/// <summary>
		/// Exeutes StartCalculator event
		/// </summary>
		public void OnStartCalculator()
		{
			if (startCalculator != null)
				startCalculator (this);
		}
		
		public void OnClearData()
		{
			if (clearData != null)
				clearData();
			else
				Ip = new IPAddress(new byte[4] { 0, 0, 0, 0 });
			Selected = DataPart.A;
		}
		
		public void OnDropDown()
		{
			if (dropDown != null)
				dropDown (this);
		}
		
		private bool hasDropDown = true;
		public bool HasDropDown {
			get { return (hasDropDown); }
			set {
				if (hasDropDown == value)
					return;
				hasDropDown = value;
				QueueDraw();
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

		public bool ClearEnabled {
			get { return (clearimg.Visible); }
			set { clearimg.Visible = value; }
		}
		
		public bool DropDownEnabled {
			get { return (dropdown.Visible); }
			set { dropdown.Visible = value; }
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

		protected override bool OnFocusInEvent (Gdk.EventFocus evnt)
		{
			Selected = DataPart.A;
			return base.OnFocusInEvent (evnt);
		}

		protected override bool OnFocusOutEvent (Gdk.EventFocus evnt)
		{
			Selected = DataPart.None;
			return base.OnFocusOutEvent (evnt);
		}

		protected override bool OnKeyPressEvent (Gdk.EventKey evnt)
		{
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
			}
			return base.OnKeyPressEvent (evnt);
		}

		protected override bool OnKeyReleaseEvent (Gdk.EventKey evnt)
		{
			if ((char)evnt.KeyValue == '.') {
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
				if (i < 256) {
					CurrentPart = i.ToString();
					if ((i > 99) || ((i < 100) && (i > 25))) {
						SelectNext();
						ignoreNextSeparator = true;
					}
				}
			}
			switch (evnt.Key) {
			}
			return base.OnKeyReleaseEvent (evnt);
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
			cells[0].GetSize (out x, out y);
			if (requisition.Height < (y*1.7))
				requisition.Height = System.Convert.ToInt32(y*1.7);
		}

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

		public IPv4Entry()
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
			cells[0] = new IpText (this, DataPart.A);
			cells[1] = new IpText (this, DataPart.Separator);
			cells[2] = new IpText (this, DataPart.B);
			cells[3] = new IpText (this, DataPart.Separator);
			cells[4] = new IpText (this, DataPart.C);
			cells[5] = new IpText (this, DataPart.Separator);
			cells[6] = new IpText (this, DataPart.D);
			foreach (IpText t in cells)
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
			
/*			Gtk.Drag.DestSet (this, DestDefaults.All, ValidTargets, Gdk.DragAction.Copy);
			DragDataReceived += HandleDragDataReceived;
			
			Gtk.Drag.SourceSet (this, Gdk.ModifierType.Button1Mask, ValidTargets, DragAction.Copy);
			DragBegin += HandleDragBegin;
			DragDataGet += HandleDragDataGet;*/
		}
	}
}
