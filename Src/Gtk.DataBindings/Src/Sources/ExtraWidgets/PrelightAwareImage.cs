//PrelightAwareImage.cs - Field Attribute to assign additional information for Gtk#Databindings
//
//Author: m. <ml@arsis.net>
//
//Copyright (c) at 12:10 AM 1/19/2009
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
using Gdk;
using Gtk;
using System.ComponentModel;
using Gtk.DataBindings;

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// Serves as Image container, difference is that Image is highlighted
	/// on mouse over and emmits OnClicked event when clicked
	/// </summary>	
	[ToolboxItem (true)]
	[Category ("Extra Gtk Widgets")]
	public class PrelightAwareImage : EventBox, INotifyPropertyChanged
	{
		private static Gdk.Cursor original_cursor = new Gdk.Cursor(Gdk.CursorType.Arrow);
		private static Gdk.Cursor hand_cursor = new Gdk.Cursor(Gdk.CursorType.Hand1);

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

		private Image img = new Image();

		private Gdk.Pixbuf original_img = null;
		private Gdk.Pixbuf highlighted_img = null;
		private bool mouseover = false;
		public bool MouseOver {
			get { return (mouseover); }
			set {
				if (mouseover == value)
					return;
				mouseover = value;
				SetImage();
			}
		}
		
		public Gdk.Pixbuf Pixbuf {
			get { return (original_img); }
			set {
				original_img = value;
				if (original_img == null)
					highlighted_img = null;
				else
					highlighted_img = BasicUtilities.ColorShiftPixbuf (original_img, 30);
				SetImage();
				OnPropertyChanged ("Pixbuf");
			}
		}

		private event EventHandler clicked = null;
		public event EventHandler Clicked {
			add {
				bool wasnull = (clicked == null);
				clicked += value;
				if ((wasnull == true) && (clicked != null))
					if (img != null)
						if (img.GdkWindow != null)
							img.GdkWindow.Cursor = hand_cursor;
			}
			remove {
				bool wasnull = (clicked == null);
				clicked += value;
				if ((wasnull == false) && (clicked == null))
					if (img != null)
						if (img.GdkWindow != null)
							img.GdkWindow.Cursor = original_cursor;
			}
		}
		
		private bool internalMouseTracking = true;
		public bool InternalMouseTracking {
			get { return (internalMouseTracking); }
			set { internalMouseTracking = value; }
		}
		
		protected override bool OnEnterNotifyEvent(Gdk.EventCrossing evnt)
		{
			if (InternalMouseTracking == false)
				return (base.OnEnterNotifyEvent(evnt));
			MouseOver = true;
			return (base.OnEnterNotifyEvent(evnt));
		}
		
		protected override bool OnLeaveNotifyEvent(Gdk.EventCrossing evnt)
		{
			if (InternalMouseTracking == false)
				return (base.OnLeaveNotifyEvent(evnt));
			MouseOver = false;
			return (base.OnLeaveNotifyEvent(evnt));
		}
		
		protected override bool OnFocusInEvent(Gdk.EventFocus evnt)
		{
			if (InternalMouseTracking == false)
				return (base.OnFocusInEvent(evnt));
			bool val = base.OnFocusInEvent(evnt);
			SetImage();
			return (val);
		}
		
		protected override bool OnFocusOutEvent(Gdk.EventFocus evnt)
		{
			if (InternalMouseTracking == false)
				return (base.OnFocusOutEvent(evnt));
			bool val = base.OnFocusOutEvent(evnt);
			SetImage();
			return (val);
		}
		
		protected override bool OnButtonPressEvent(Gdk.EventButton evnt)
		{
			if (InternalMouseTracking == false)
				return (base.OnButtonPressEvent(evnt));
			
			if (evnt.Button != 1)
				return (base.OnButtonPressEvent(evnt));
			
			HasFocus = true;
			//is_pressed = true;
			QueueDraw();
			
			return (base.OnButtonPressEvent(evnt));
		}
		
		protected override bool OnButtonReleaseEvent(Gdk.EventButton evnt)
		{
			if (InternalMouseTracking == false)
				return (base.OnButtonReleaseEvent(evnt));
			if(evnt.Button != 1)
				return (base.OnButtonReleaseEvent(evnt));
			
			//is_pressed = false;
			QueueDraw();
			OnClicked();
			
			return (base.OnButtonReleaseEvent(evnt));
		}

		protected virtual void OnClicked()
		{
			if (clicked != null)
				clicked (this, null);
		}
		
		protected void SetImage()
		{
			if (MouseOver == false)
				img.Pixbuf = original_img;
			else
				img.Pixbuf = highlighted_img;
		}
		
		private void InitializeBox()
		{
			img.Show();
			this.Add (img);
			Events |= (EventMask.PointerMotionMask | EventMask.ButtonPressMask | EventMask.VisibilityNotifyMask);
			//original_cursor = img.GdkWindow.Cursor;
		}
		
		public PrelightAwareImage()
		{
			InitializeBox();
		}
		
		public PrelightAwareImage (bool aInternalMouseTracking)
		{
			InternalMouseTracking = aInternalMouseTracking;
			InitializeBox();
		}
		
		public PrelightAwareImage (Gdk.Pixbuf aPixbuf)
		{
			InitializeBox();
			Pixbuf = aPixbuf;
		}
		
		public PrelightAwareImage (bool aInternalMouseTracking, Gdk.Pixbuf aPixbuf)
		{
			InternalMouseTracking = aInternalMouseTracking;
			InitializeBox();
			Pixbuf = aPixbuf;
		}
	}
}
