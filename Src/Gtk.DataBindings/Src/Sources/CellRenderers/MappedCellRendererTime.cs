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
using System.Globalization;
using System.Data.Bindings;
using Gtk.ExtraWidgets;
using GLib;

namespace Gtk.DataBindings
{
	/// <summary>
	/// Specifies cell renderer which handles date
	/// </summary>
	[GtkCellFactoryProvider ("time", "DefaultFactoryCreate")]
	public class MappedCellRendererTime : MappedCellRendererDrawingCell
	{
		/// <summary>
		/// Registered factory creation method
		/// </summary>
		/// <param name="aArgs">
		/// Arguments <see cref="FactoryInvocationArgs"/>
		/// </param>
		/// <returns>
		/// Result widget <see cref="IMappedColumnItem"/>
		/// </returns>
		public static IMappedColumnItem DefaultFactoryCreate (FactoryInvocationArgs aArgs)
		{
			IMappedColumnItem wdg = new MappedCellRendererTime();
//			if (aArgs.State == PropertyDefinition.ReadOnly)
			wdg.MappedTo = aArgs.PropertyName;
			return (wdg);
		}
		
		private DrawingCellEditText timetext = new DrawingCellEditText();

		private bool isImportant = false;
		/// <value>
		/// Defines if text is important (bold) or not
		/// </value>
		public bool IsImportant {
			get { return (isImportant); }
			set {
				if (isImportant == value)
					return;
				isImportant = value;
			}
		}
		
		private bool showAMPMDesignator = false;
		/// <value>
		/// Specifies if seconds should be shown or not
		/// </value>
		public bool ShowAMPMDesignator {
			get { return (showSeconds); }
			set {
				if (showAMPMDesignator == value)
					return;
				showAMPMDesignator = value;
				ResetCellSize();
			}
		}
		
		private bool showSeconds = false;
		/// <value>
		/// Specifies if seconds should be shown or not
		/// </value>
		public bool ShowSeconds {
			get { return (showSeconds); }
			set {
				if (showSeconds == value)
					return;
				showSeconds = value;
				ResetCellSize();
			}
		}
		
		private bool showMilliseconds = false;
		/// <value>
		/// Specifies if milliseconds should be shown or not
		/// </value>
		public bool ShowMilliseconds {
			get { return ((showSeconds == true) && (showMilliseconds == true)); }
			set {
				if (showMilliseconds == value)
					return;
				showMilliseconds = value;
				ResetCellSize();
			}
		}

		private DateTime time = DateTime.Now;
		/// <value>
		/// Specifies property which is used to assign value
		/// </value>
		[Property ("time")]
		public DateTime Time {
			get { return (time); }
			set {
				if (time.Equals(value) == true)
					return;
				time = value;
				timetext.Text = GetTimeString();
			}
		}

		/// <summary>
		/// Returns default property for value in cellrenderer
		/// </summary>
		/// <returns>
		/// property name as defined in gtk <see cref="System.String"/>
		/// </returns>
		public override string GetDataProperty ()
		{
			return ("time");
		}

		/// <summary>
		/// Resolves display string for time
		/// </summary>
		/// <returns>
		/// String representation <see cref="System.String"/>
		/// </returns>
		private string GetTimeString()
		{
			CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
			string separator = ci.DateTimeFormat.TimeSeparator;
			string mask = string.Format ("hh{0}mm{1}{2}{3}", separator, 
			                             (ShowSeconds == true) ? (separator + "ss") : "",
			                             (ShowMilliseconds == true) ? ".fff" : "",
			                             (ShowAMPMDesignator == true) ? " tt" : "");
			return (Time.ToString(mask));
		}
		
		/// <summary>
		/// Returns default measunring string for cell size
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/>
		/// </returns>
		private string ResetCellSize()
		{
			CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
			string separator = ci.DateTimeFormat.TimeSeparator;
			string sec = string.Format ("{0}88", ci.DateTimeFormat.TimeSeparator);
			string msec = ".888";
			int i = Math.Max (ci.DateTimeFormat.AMDesignator.Length, ci.DateTimeFormat.PMDesignator.Length);
			string ampm = "";
			for (int j=0; j<i; j++)
				ampm += "M";
			if (ampm.Length > 0)
				ampm = " " + ampm;
			string s = string.Format ("88{0}88{1}{2}{3}", ci.DateTimeFormat.TimeSeparator,
			                   (ShowSeconds == true) ? sec : "",
			                   (ShowMilliseconds == true) ? msec : "",
			                   (ShowAMPMDesignator == true) ? ampm : "");
			if (IsImportant == true)
				s = string.Format ("<b>{0}</b>", s);
			return (s);
		}
		
		/// <summary>
		/// Assigns value to this column
		/// </summary>
		/// <param name="aValue">
		/// Value to be assigned <see cref="System.Object"/>
		/// </param>
		public void AssignValue (object aValue)
		{
			if (aValue != null)
				Time = (DateTime) aValue;
		}

		public MappedCellRendererTime()
			: base (new DrawingCellHBox())
		{
			showAMPMDesignator = (System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongTimePattern.IndexOf("tt") > -1);
			timetext.SizeText = ResetCellSize();
			MainBox.PackEnd (new DrawingCellNull(), true);
			MainBox.PackEnd (timetext, false);
		}
	}
}
