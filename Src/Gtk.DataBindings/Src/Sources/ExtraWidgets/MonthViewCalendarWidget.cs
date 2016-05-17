
using System;
using System.ComponentModel;
using System.Data.Bindings;
using Gtk.DataBindings;
using System.Globalization;
using System.Threading;

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// Provides widget which displays month view
	/// </summary>
	[ToolboxItem (true)]
	[Category ("Extra Gtk Widgets")]
	public class MonthViewCalendarWidget : CellDrawingArea
	{
		#region DayTitle
		
		protected class DayTitle
		{
		}
		
		#endregion DayTitle
		
		#region DayCell
		
		protected class DayCell : DrawingCellGtkStateBin
		{
			public CalendarDayDescription Descriptor = new CalendarDayDescription();
			public DrawingCellVBox MainBox = new DrawingCellVBox();
			public DrawingCellEditText DayNumber = new DrawingCellEditText();
			public DrawingCellEditText Description = new DrawingCellEditText();
			public DrawingCellHBox Icons = new DrawingCellHBox();
			
			private bool important = false;
			public bool Important {
				get { return (DayNumber.IsImportant); }
				set {
					if (DayNumber.IsImportant == value)
						return;
					DayNumber.IsImportant = value;
					OnPropertyChanged ("Important");
				}
			}

			private bool inactive = false;
			public bool Inactive {
				get { return (DayNumber.State == StateType.Normal); }
				set {
					if (inactive == value)
						return;
					DayNumber.CustomState = (value == true) ? StateType.Insensitive : StateType.Normal;
					OnPropertyChanged ("Inactive");
				}
			}

			private DateTime date = DateTime.Now;
			public DateTime Date {
				get { return (date); }
				set {
					if (date.Equals(value) == true)
						return;
					date = value;
					DayNumber.Text = date.Day.ToString();
					OnPropertyChanged ("Date");
				}
			}
			
			public override void GetSize (out double aWidth, out double aHeight)
			{
				base.GetSize (out aWidth, out aHeight);
			}
			
			public DayCell (CalendarDayDescription aDescriptor)
				: base ()
			{
				DayNumber.StateResolving = ValueResolveMethod.Manual;
				Descriptor = aDescriptor;
				Padding = 2;
				Pack (MainBox);
				MainBox.Spacing = 1;
				DayNumber.XPos = 0.0;
				DayNumber.YPos = 0.0;
				MainBox.PackEnd (DayNumber, false);
				MainBox.PackEnd (Description, true);
				Description.Visible = aDescriptor.DescriptionsVisible;
				MainBox.PackEnd (Icons, false);
				Icons.Visible = aDescriptor.IconsVisible;
			}
		}

		#endregion DayCell
		
		#region DayRows
		
		public class DayRows : DrawingCellVBox
		{
			private CalendarDayDescription descriptor = new CalendarDayDescription();

			private DayOfWeek weekDay = 0;
			public DayOfWeek WeekDay {
				get { return (weekDay); }
				set {
					if (weekDay == value)
						return;
					weekDay = value;
					OnPropertyChanged ("WeekDay");
				}
			}
			
			protected override void CalculateCellAreas (CellRectangle aRect)
			{
				double w,h,ch = h = w = 0;
				Cells[0].GetCellSize (out w, out h);
				ch = (aRect.Height-h)/6;
				for (int i=0; i<Count; i++) {
					Cells[i].GetCellSize (out w, out h);
					if (i == 0)
						Cells[i].Area.Set (aRect.Left, aRect.Top, aRect.Right, h);
					else
						Cells[i].Area.Set (aRect.Left, aRect.Top+h+((i-1)*ch), aRect.Right, ch);
				}
			}

			public DayRows (DayOfWeek aWeekDay, CalendarDayDescription aDescriptor)
			{
				Spacing = 1;
				descriptor = aDescriptor;
				weekDay = aWeekDay;
				CultureInfo ci = Thread.CurrentThread.CurrentCulture;
				DrawingCellEditText day = new DrawingCellEditText();
				day.Text = ci.DateTimeFormat.GetAbbreviatedDayName(weekDay);
				PackEnd (day, false);
				//day.XPos = 0.5;

				DateTime dt = new DateTime (aDescriptor.CurrentYear, aDescriptor.CurrentMonth, 1);
				
				for (int i=0; i<7; i++)
					PackEnd (new DayCell (aDescriptor), false);
			}
		}
		
		#endregion DayRows
		
		#region WeekColumns
		
		public class WeekColumns : DrawingCellHBox
		{
			private CalendarDayDescription descriptor = new CalendarDayDescription();

			public override void PaintBackground (CellExposeEventArgs aArgs)
			{
				base.PaintBackground (aArgs);
				aArgs.CellArea.Shrink (1);
				aArgs.CellArea.DrawRoundedRectangle (aArgs.Context, 2);
				aArgs.Context.Color = new Cairo.Color(1,0,0);
				aArgs.Context.LineWidth = 1;
				aArgs.Context.StrokePreserve();
				aArgs.Context.Color = new Cairo.Color(1,1,1);
				aArgs.Context.Fill();
				aArgs.CellArea.Grow (1);
			}

			public override void GetCellSize (out double aWidth, out double aHeight)
			{
				double w,h,cw,ch = cw = h = w = 0;
				for (int i=0; i<Count; i++) {
					Cells[i].GetCellSize (out w, out h);
					if (w > cw)
						cw = w;
					if (h > ch)
						ch = h;
				}
				aWidth = cw*7;
				aHeight = ch;
			}
			
			protected override void CalculateCellAreas (CellRectangle aRect)
			{
				double width = aRect.Width/7;
				for (int i=0; i<Count; i++) {
					Cells[i].Area.Set (aRect.Left+(width*i), aRect.Top, width, aRect.Bottom);
					(Cells[i] as DrawingCellBox).DoCalculateCellAreas (Cells[i].Area);
				}
			}

			public WeekColumns (CalendarDayDescription aDescriptor)
			{
				descriptor = aDescriptor;
				CultureInfo ci = Thread.CurrentThread.CurrentCulture;
				int d = (int) ci.DateTimeFormat.FirstDayOfWeek;
				for (int i=0; i<7; i++) {
					PackEnd (new DayRows ((DayOfWeek) d, aDescriptor), false);
					d = (d + 1) % 7;
				}
			}
		}
		
		#endregion WeekColumns
		
		private CalendarDayDescription descriptor = new CalendarDayDescription();
		
		private event RequestDayDescriptionEvent dayDescriptionRequest = null;
		/// <summary>
		/// Handles requests for descriptions
		/// </summary>
		public event RequestDayDescriptionEvent DayDescriptionRequest {
			add { descriptor.DayDescriptionRequest += value; }
			remove { descriptor.DayDescriptionRequest -= value; }
		}
		
		protected DayCell CreateDayCell()
		{
			return (new DayCell(descriptor));
		}
		
		/// <summary>
		/// Returns box which contains cells describing single day
		/// </summary>
		/// <param name="aDay">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// A <see cref="DrawingCellBox"/>
		/// </returns>
		public DrawingCellBox CellPerDay (int aDay)
		{
			return (null);
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
//			if (requisition.Height < ChameleonTemplates.Entry.Requisition.Height)
//				requisition.Height = ChameleonTemplates.Entry.Requisition.Height;
//			double x,y=0;
//			MainBox.GetSize (out x, out y);
//			if (requisition.Height < (y*1.7))
//				requisition.Height = System.Convert.ToInt32(y*1.7);
			System.Console.WriteLine("{0}x{1}", requisition.Width, requisition.Height);
		}

		protected override DrawingCellBox CreateBox ()
		{
			WeekColumns b = new WeekColumns(descriptor);
			b.Spacing = 2;
			b.Padding = 2;
			b.Homogeneous = true;
			return (b);
		}

		public MonthViewCalendarWidget()
			: base ()
		{
			descriptor.CurrentMonth = DateTime.Now.Month;
			descriptor.CurrentYear = DateTime.Now.Year;
			System.Console.WriteLine(Count);
			
		}
	}
}
