//ComplexEntry.cs - Field Attribute to assign additional information for Gtk#Databindings
//
//Author: m. <ml@arsis.net>
//
//Copyright (c) at 1:01 PM 1/9/2009
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

using System;

namespace Gtk.DataBindings
{
//	[System.ComponentModel.ToolboxItem(true)]
	public partial class ComplexEntry : Gtk.Bin
	{
		private ArrayList widgets = new ArrayList();
		
		///<summary>
		/// Shows or hides frame for all Entry widgets inside this box
		///</summary>
		[Browsable(true)]
		private bool widgetsHaveFrames = true;
		public bool WidgetsHaveFrames {
			get { return (widgetsHaveFrames); }
			set { 
				widgetsHaveFrames = value;
				SingleEntry.HasFrame = value;
				FirstMasked.HasFrame = value;
				foreach (Widget wdg in widgets) {
					if (System.Data.Bindings.TypeValidator.IsCompatible(wdg.GetType(), typeof(Gtk.Entry)))
						(wdg as Gtk.Entry).HasFrame = value;
					if (System.Data.Bindings.TypeValidator.IsCompatible(wdg.GetType(), typeof(Gtk.Entry)))
						(wdg as Gtk.SpinButton).HasFrame = value;
				}
			}
		}

		protected override bool OnExposeEvent (Gdk.EventExpose ev)
		{
			int x = Allocation.Left;
			int y = Allocation.Top;
			int width = Allocation.Width;
			int height = Allocation.Height;

			Gtk.Entry fe = GetFocusedEntry();
			if (GetFocusState() == false) {
				if ((focused == true) || (first == true)) {
					Style = SingleEntry.Style.Copy();
					focused = false;
					first = false;
				}
				Gtk.Style.PaintShadow (Style, GdkWindow, State, Gtk.ShadowType.In,
				                       ev.Area, this, "entry", x, y, width, height);
			}
			else {
				if ((focused == false) || (first == true)) {
					Style = fe.Style.Copy();
					focused = true;
					first = false;
				}
				Gtk.Style.PaintShadow (fe.Style, GdkWindow, fe.State, Gtk.ShadowType.In,
				                       ev.Area, fe, "entry", x, y, width, height);
			}

			return (base.OnExposeEvent (ev));
		}
				
		///<summary>
		/// Defines if Box has to show focus frame
		///</summary>
		private bool ShowFocus {
			get { return (GetFocusedEntry() != null); }
		}

		/// <summary>
		/// Returns entry that has focus
		/// </summary>
		/// <returns>
		/// Entry that has focus <see cref="Gtk.Entry"/>
		/// </returns>
		public Gtk.Widget GetFocusedEntry()
		{
			foreach (Widget wdg in widgets)
				if ((wdg as Gtk.Widget).HasFocus == true)
					return (wdg);
			return (null);
		}

		private bool GetContainerFocusState (Gtk.Bin aContainer)
		{
			foreach (Widget wdg in aContainer.Children) {
				if (System.Data.Bindings.TypeValidator.IsCompatible(wdg.GetType(), typeof(Gtk.Bin))) {
					if (GetContainerFocusState(wdg) == true)
						return (true);
				}
				else
					if (wdg.HasFocus == true)
						return (true);
			}
			return (false);
		}
		
		/// <summary>
		/// Returns focus state
		/// </summary>
		/// <returns>
		/// True if any widget inside has focus <see cref="System.Boolean"/>
		/// </returns>
		public bool GetFocusState()
		{
			if (GetContainerFocusState(EntryHBox) == true)
				return (true);
			if (GetContainerFocusState(BeforeBox) == true)
				return (true);
			if (GetContainerFocusState(AfterBox) == true)
				return (true);
			return (false);
		}

		/// <summary>
		/// Packs widget on the left side of the box. Every next packed widget
		/// gets packed after previous
		/// </summary>
		/// <param name="aWidget">
		/// Widget to insert inside <see cref="Gtk.Widget"/>
		/// </param>
		public void PackStart (Gtk.Widget aWidget)
		{
			if (aWidget == null)
				return;
			if (BeforeAlignment.LeftPadding == 0)
				BeforeAlignment.LeftPadding = 3;
			BeforeBox.PackEnd (aWidget, false, false, 0);
			if (aWidget is Gtk.Entry) {
				(aWidget as Gtk.Widget).FocusInEvent += OnEntryFocusIn;
				(aWidget as Gtk.Widget).FocusOutEvent += OnEntryFocusOut;				
			}
			else {
				aWidget.CanFocus = false;
			}
			aWidget.Show();
		}
		
		protected virtual void OnEntryFocusIn (object o, Gtk.FocusInEventArgs args)
		{
			this.QueueDraw();
		}

		protected virtual void OnEntryFocusOut (object o, Gtk.FocusOutEventArgs args)
		{
			this.QueueDraw();
		}

		/// <summary>
		/// Packs widget on the right side of the box. Every next packed widget
		/// gets packed before previous
		/// </summary>
		/// <param name="aWidget">
		/// Widget to insert inside <see cref="Gtk.Widget"/>
		/// </param>
		public void PackEnd (Gtk.Widget aWidget)
		{
			if (aWidget == null)
				return;
			if (AfterAlignment.RightPadding == 0)
				AfterAlignment.RightPadding = 3;
			AfterBox.PackEnd (aWidget, false, false, 0);
			if (aWidget is Gtk.Entry) {
				(aWidget as Gtk.Widget).FocusInEvent += OnEntryFocusIn;
				(aWidget as Gtk.Widget).FocusOutEvent += OnEntryFocusOut;				
			}
			else {
				aWidget.CanFocus = false;
			}
			aWidget.Show();
		}
		
		public ComplexEntry()
		{
			this.Build();
			this.AppPaintable = true;
			RedefineInput (true);
			BuildEditor();
			WidgetsHaveFrames = widgetsHaveFrames;			
		}

		~ComplexEntry()
		{
			SingleEntry.FocusInEvent -= OnEntryFocusIn;
			SingleEntry.FocusOutEvent -= OnEntryFocusOut;
		}
	}
}
