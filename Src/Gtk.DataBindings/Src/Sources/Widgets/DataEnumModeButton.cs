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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Bindings;
using System.Reflection;
using Gtk;
using Gtk.ExtraWidgets;

namespace Gtk.DataBindings
{	
	/// <summary>
	/// 
	/// </summary>
	[ToolboxItem (true)]
	[Category ("widget")]
	[Description ("Adaptable ModeButton Widget connected to enumeration")]
	public class DataEnumModeButton : ModeButton, IAdaptableControl
	{
		private System.Type lastType = null;
		private ControlAdaptor adaptor = null;
		private List<Box> widgets = new List<Box>();

		private ModeButtonLayout buttonLayout = ModeButtonLayout.Horizontal;
		/// <value>
		/// Specifies mode button layout
		/// </value>
		[Browsable (true), Description ("Button layout")]
		public ModeButtonLayout ButtonLayout {
			get { return (buttonLayout); }
			set {
				if (buttonLayout == value)
					return;
				buttonLayout = value;
				ResetLayout();
			}
		}
		
		private ModeButtonDisplayMode displayMode = ModeButtonDisplayMode.TextOnly;
		/// <value>
		/// Specifies mode button display mode
		/// </value>
		[Browsable (true), Description ("Button display mode")]
		public ModeButtonDisplayMode DisplayMode {
			get { return (displayMode); }
			set {
				if (displayMode == value)
					return;
				displayMode = value;
				ResetLayout();
			}
		}
		
		/// <summary>
		/// Resolves ControlAdaptor in read-only mode
		/// </summary>
		[Browsable (false), Category ("Data Binding")]
		public ControlAdaptor Adaptor {
			get { return (adaptor); }
		}
		
		/// <summary>
		/// Defines if DataSource is inherited fom parent controls or not
		/// </summary>
		[Category ("Data Binding"), Description ("Inherited Data Source")]
		public bool InheritedDataSource {
			get { return (adaptor.InheritedDataSource); }
			set { 
				if (adaptor.InheritedDataSource == value)
					return;
				adaptor.InheritedDataSource = value; 
				ResetLayout();
			}
		}
		
		/// <summary>
		/// DataSource object control is connected to
		/// </summary>
		[Browsable (false), Category ("Data Binding")]
		public object DataSource {
			get { 
				if (adaptor == null)
					return (null);
				return (adaptor.DataSource); 
			}
			set { 
				if (adaptor == null)
					return;
				if (adaptor.DataSource == value)
					return;
				adaptor.DataSource = value; 
				ResetLayout();
			}
		}
		
		/// <summary>
		/// Link to Mappings in connected Adaptor 
		/// </summary>
		[Category ("Data Binding"), Description ("Data mappings")]
		public string Mappings { 
			get { 
				if (adaptor == null)
					return ("");
				return (adaptor.Mappings); 
			}
			set { 
				if (adaptor == null)
					return;
				if (adaptor.Mappings == value)
					return;
				adaptor.Mappings = value; 
				ResetLayout();
			}
		}
		
		/// <summary>
		/// Defines if DataSource is inherited fom parent controls or not
		/// </summary>
		[Category ("Data Binding"), Description ("Inherited Data Source")]
		public bool InheritedBoundaryDataSource {
			get { return (adaptor.InheritedBoundaryDataSource); }
			set { adaptor.InheritedBoundaryDataSource = value; }
		}
		
		/// <summary>
		/// DataSource object control is connected to
		/// </summary>
		[Browsable (false), Category ("Data Binding")]
		public IObserveable BoundaryDataSource {
			get { return (adaptor.BoundaryDataSource); }
			set { adaptor.BoundaryDataSource = value; }
		}
		
		/// <summary>
		/// Link to Mappings in connected Adaptor 
		/// </summary>
		[Category ("Data Binding"), Description ("Data Mappings")]
		public string BoundaryMappings { 
			get { return (adaptor.BoundaryMappings); }
			set { adaptor.BoundaryMappings = value; }
		}
		
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
		public event CustomGetDataEvent CustomGetData {
			add { adaptor.CustomGetData += value; }
			remove { adaptor.CustomGetData -= value; }
		}
		
		/// <summary>
		/// Calls ControlAdaptors method to transfer data, so it can be wrapped
		/// into widget specific things and all checkups
		/// </summary>
		/// <param name="aSender">
		/// Sender object <see cref="System.Object"/>
		/// </param>
		public void CallAdaptorGetData (object aSender)
		{
			Adaptor.InvokeAdapteeDataChange (this, aSender);
		}
		
		/// <summary>
		/// Notification method activated from Adaptor 
		/// </summary>
		/// <param name="aSender">
		/// Object that made change <see cref="System.Object"/>
		/// </param>
		public virtual void GetDataFromDataSource (object aSender)
		{
			adaptor.DataChanged = false;
			if (ModeCount <= 0)
				return;
			if ((adaptor.Adaptor.FinalTarget != null) && (Mappings != "")) {
				System.Type type = adaptor.Adaptor.Values[0].Value.GetType();
				if (type.IsEnum == false)
					throw new NotSupportedException ("Only enumerations are supported in DataEnumModeButton");
				Array enumValues = Enum.GetValues (type);
				for (int i=0; i<enumValues.Length; i++)
					if (enumValues.GetValue(i).Equals(adaptor.Value))
						Selected = i;
			}
		}
		
		private void CleanBox (Box aBox)
		{
			Widget wdg;
			while (aBox.Children.Length > 0) {
				wdg = aBox.Children[aBox.Children.Length-1];
				wdg.Hide();
				aBox.Remove (wdg);
				wdg.Destroy();
			}
		}
		
		/// <summary>
		/// Resets widget layout
		/// </summary>
		private void ResetLayout()
		{
			if ((adaptor.Adaptor.FinalTarget != null) && (Mappings != "")) {
				System.Type type = adaptor.Adaptor.Values[0].Value.GetType();
				if (type == lastType)
					return;
			}
			while (ModeCount > 0) {
				lastType = null;
				CleanBox (widgets[widgets.Count-1]);
				widgets[widgets.Count-1].Hide();
				Remove (widgets.Count-1);
				widgets[widgets.Count-1].Destroy();
				widgets.RemoveAt (widgets.Count-1);
			}

			lastType = null;
			if ((adaptor.Adaptor.FinalTarget != null) && (Mappings != "")) {
				//System.Console.WriteLine("Building");
				System.Type type = adaptor.Adaptor.Values[0].Value.GetType();
				lastType = type;
				if (type.IsEnum == false)
					throw new NotSupportedException (string.Format("DataRadioGroup only supports enum types as mapping, specified was {0}", type));
				Gtk.Label lbl;
				Gtk.Image pic;
				foreach (FieldInfo info in type.GetFields()) {
					string s = info.GetEnumTitle();
					if (s == "value__")
						continue;
					if (ButtonLayout == ModeButtonLayout.Horizontal)
						widgets.Add (new HBox (false, 2));
					else
						widgets.Add (new VBox (false, 2));
/*					widgets[widgets.Count-1].Events = 0;//EventMask.PointerMotionMask
			            |  EventMask.ButtonPressMask
			            |  EventMask.VisibilityNotifyMask;*/
					if (DisplayMode != ModeButtonDisplayMode.TextOnly) {
						pic = new Image();
						pic.Show();
						pic.Pixbuf = (Gdk.Pixbuf) info.GetEnumIcon();
						widgets[widgets.Count-1].PackStart (pic, false, false, 0);
					}
					if (DisplayMode != ModeButtonDisplayMode.IconsOnly) {
						lbl = new Label (s);
						lbl.Show();
						widgets[widgets.Count-1].PackStart (lbl, false, false, 0);
					}
					widgets[widgets.Count-1].Show();
					Append (widgets[widgets.Count-1]);
				}
				GetDataFromDataSource (this);
			}
		}

		/// <summary>
		/// Creates Widget 
		/// </summary>
		public DataEnumModeButton()
			: this (null, "")
		{
		}
		
		/// <summary>
		/// Creates Widget 
		/// </summary>
		/// <param name="aMappings">
		/// Mappings with this widget <see cref="System.String"/>
		/// </param>
		public DataEnumModeButton (string aMappings)
			: this (null, aMappings)
		{
		}
		
		/// <summary>
		/// Creates Widget 
		/// </summary>
		/// <param name="aDataSource">
		/// DataSource connected to this widget <see cref="System.Object"/>
		/// </param>
		/// <param name="aMappings">
		/// Mappings with this widget <see cref="System.String"/>
		/// </param>
		public DataEnumModeButton (object aDataSource, string aMappings)
			: base()
		{
			adaptor = new GtkControlAdaptor (this, true, aDataSource, aMappings);
			GetDataFromDataSource (null);
		}
		
		/// <summary>
		/// Destroys and disconnects Widget
		/// </summary>
		~DataEnumModeButton()
		{
			adaptor.Disconnect();
			adaptor = null;
		}
	}
}
