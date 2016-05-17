//ComplexEntryBox.cs - Description
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

using System;
using System.Collections;
using System.ComponentModel;
using Gtk;
using Gtk.DataBindings;

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// Paints box with background of Entry widget and enables complex widget setup inside
	/// </summary>
	public partial class ComplexEntryBox : Gtk.Bin
	{
		bool first = true;
		bool focused = false;
		private ArrayList maskPack = new ArrayList();
		
		///<summary>
		/// Defines mask for string which will be edited inside
		///</summary>
		[Browsable(true)]
		private string stringMask = "";
		public string StringMask {
			get { return (stringMask); }
			set { 
				if (stringMask == value)
					return;
				stringMask = value;
				RedefineInput (false);
			}
		}

		///<summary>
		/// Shows or hides frame for all Entry widgets inside this box
		///</summary>
		[Browsable(true)]
		private bool entryWidgetsHaveFrames = true;
		public bool EntryWidgetsHaveFrames {
			get { return (entryWidgetsHaveFrames); }
			set { 
				entryWidgetsHaveFrames = value;
				SingleEntry.HasFrame = value;
				FirstMasked.HasFrame = value;
				foreach (Widget wdg in maskPack)
					if (System.Data.Bindings.TypeValidator.IsCompatible(wdg.GetType(), typeof(Gtk.Entry)))
						(wdg as Gtk.Entry).HasFrame = value;
			}
		}

		private void RedefineInput (bool aFirst)
		{
			if (StringMask == "") {
//				MaskedHBox.Parent = null;
				MaskedHBox.Visible = false;
				MaskedHBox.Sensitive = false;
				EntryHBox.Remove (MaskedHBox);
				FirstMasked.Visible = false;
				SingleEntry.Visible = true;
				if (aFirst == false)
					EntryHBox.PackStart (SingleEntry, true, true, 0);
			}
			else {
				SingleEntry.Visible = false;
				EntryHBox.Remove (SingleEntry);
				MaskedHBox.Visible = true;
				MaskedHBox.Sensitive = true;
				FirstMasked.Visible = true;
				if (aFirst == false)
					EntryHBox.PackStart (MaskedHBox, true, true, 0);
			}
		}
		
		protected override bool OnExposeEvent (Gdk.EventExpose ev)
		{
			int x = Allocation.Left;
			int y = Allocation.Top;
			int width = Allocation.Width;
			int height = Allocation.Height;

			Gtk.Entry fe = GetFocusedEntry();
			if (fe == null) {
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
		public Gtk.Entry GetFocusedEntry()
		{
			if (SingleEntry.HasFocus)
				return (SingleEntry);
			if (FirstMasked.HasFocus == true)
				return (FirstMasked);
			foreach (Widget wdg in maskPack)
				if (System.Data.Bindings.TypeValidator.IsCompatible(wdg.GetType(), typeof(Gtk.Entry)))
					if ((wdg as Gtk.Entry).HasFocus == true)
						return (wdg as Gtk.Entry);
			return (null);
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
			aWidget.CanFocus = false;
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
				(aWidget as Gtk.Entry).FocusInEvent += OnEntryFocusIn;
				(aWidget as Gtk.Entry).FocusOutEvent += OnEntryFocusOut;				
			}
			else {
				aWidget.CanFocus = false;
			}
			aWidget.Show();
		}
		
		public ComplexEntryBox()
		{
			this.Build();
			this.AppPaintable = true;
			RedefineInput (true);
	
			EntryWidgetsHaveFrames = entryWidgetsHaveFrames;
			SingleEntry.FocusInEvent += OnEntryFocusIn;
			SingleEntry.FocusOutEvent += OnEntryFocusOut;
		}

		~ComplexEntryBox()
		{
			SingleEntry.FocusInEvent -= OnEntryFocusIn;
			SingleEntry.FocusOutEvent -= OnEntryFocusOut;
		}
	}
}
