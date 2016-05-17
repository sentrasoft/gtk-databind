//WidgetFlasher.cs - Description
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
using Gtk;

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// Starts flashing control based on the specified params
	/// </summary>
	public class WidgetFlasher
	{
		private bool cancelled = false;
		
//		private int Timer = 0;
//		private int MaxTime = -1;
		private int color = 255;
		private byte NULL_COLOR = 255;
		private bool fwd = false;
		private byte step = 255;
		private int stepCount = 100;
		private int counter = 0;
		
		/// <value>
		/// Returns defined flash type
		/// </value>
		///<remarks>
		/// Default value is FlashingType.SingleStep
		///</remarks>
		private FlashingType flashType = FlashingType.SingleStep;
		public FlashingType FlashType {
			get { return (flashType); }
		}
		
		///<summary>
		/// Returns the number of flashes to be performed
		///</summary>
		///<remarks>
		/// Default value is 1
		///</remarks>
		private int flashCount = 1;
		public int FlashCount {
			get { return (flashCount); }
		}

		///<summary>
		/// Returns time of single flash
		///</summary>
		///<remarks>
		/// Default value is 1 second
		///</remarks>
		private int flashDuration = 1000;
		public int FlashDuration {
			get { return (flashDuration); }
		}

		///<summary>
		/// Returns time of single flash
		///</summary>
		///<remarks>
		/// Default value is 50/1000 second
		///</remarks>
		private int stepDuration = 50;
		public int StepDuration {
			get { return (stepDuration); }
		}

		///<summary>
		/// Returns if flashing of this control was cancelled
		///</summary>
		public bool IsActive {
			get { return (cancelled == false); }
		}

		///<summary>
		/// Returns total time of flashing
		///</summary>
		public int TotalFlashingTime {
			get {
				if (FlashType == FlashingType.LoopFlash)
					return (-1);
				return (FlashDuration * FlashCount); 
			}
		}

		///<summary>
		/// Specifies widget which is supposed to be flashing
		///</summary>
		private Widget flashedWidget = null;
		public Widget FlashedWidget {
			get { return (flashedWidget); }
		}
		
		///<summary>
		/// Cancels flashing of the widget
		///</summary>
		public void Cancel()
		{
			ResetColorToNormal();
			cancelled = true;
			flashedWidget = null;
			FlashingWidgets.Remove (this);
		}
		
		/// <summary>
		/// Resets widgets color back to its original state
		/// </summary>
		public void ResetColorToNormal()
		{
			if (flashedWidget == null)
				return;
			flashedWidget.ModifyBase(StateType.Normal);
			flashedWidget.ModifyBg(StateType.Normal);
			flashedWidget.ModifyFg(StateType.Normal);
			flashedWidget.ModifyBase(StateType.Selected);
			flashedWidget.ModifyBg(StateType.Selected);
			flashedWidget.ModifyFg(StateType.Selected);
			flashedWidget.ModifyBase(StateType.Prelight);
			flashedWidget.ModifyBg(StateType.Prelight);
			flashedWidget.ModifyFg(StateType.Prelight);
		}
	
		public void TestAll(Gdk.Color colr)
		{
			flashedWidget.ModifyBase(StateType.Prelight, colr);
			flashedWidget.ModifyBase(StateType.Active, colr);
			flashedWidget.ModifyBase(StateType.Insensitive, colr);
			flashedWidget.ModifyBase(StateType.Normal, colr);
			flashedWidget.ModifyBase(StateType.Selected, colr);
			flashedWidget.ModifyBg(StateType.Prelight, colr);
			flashedWidget.ModifyBg(StateType.Active, colr);
			flashedWidget.ModifyBg(StateType.Insensitive, colr);
			flashedWidget.ModifyBg(StateType.Normal, colr);
			flashedWidget.ModifyBg(StateType.Selected, colr);
			flashedWidget.ModifyFg(StateType.Prelight, colr);
			flashedWidget.ModifyFg(StateType.Active, colr);
			flashedWidget.ModifyFg(StateType.Insensitive, colr);
			flashedWidget.ModifyFg(StateType.Normal, colr);
			flashedWidget.ModifyFg(StateType.Selected, colr);
		}

		/// <summary>
		/// Changes color to new state
		/// </summary>
		public void FlashColor()
		{
			if (flashedWidget == null)
				return;
			if (color < 0)
				color = 0;
			if (color > 255)
				color = 255;
			Gdk.Color colr = new Gdk.Color (NULL_COLOR, (byte) color, (byte) color);
			if (flashedWidget is Gtk.Button) {
				(flashedWidget as Gtk.Button).ModifyBg(StateType.Prelight, colr);
				(flashedWidget as Gtk.Button).ModifyBg(StateType.Normal, colr);
				return;
			}
			if (flashedWidget is Gtk.ToolButton) {
				TestAll (colr);
				(flashedWidget as Gtk.ToolButton).ModifyBg(StateType.Prelight, colr);
				(flashedWidget as Gtk.ToolButton).ModifyBg(StateType.Normal, colr);
				return;
			}
			if (flashedWidget is Gtk.Entry) {
				(flashedWidget as Gtk.Entry).ModifyBase(StateType.Normal, colr);
				return;
			}
			if (flashedWidget is Gtk.Label) {
				(flashedWidget as Gtk.Label).ModifyFg(StateType.Normal, colr);
				return;
			}
			if (flashedWidget is Gtk.EventBox) {
				(flashedWidget as Gtk.EventBox).ModifyBg(StateType.Normal, colr);
				return;
			}
			throw new Exception ("FlashColor_Control_Type_NotSupported" + flashedWidget.ToString());
		}

		public void Flash()
		{
			if (FlashCount != -1)
			    counter++;

			if (fwd == true)
				if ((color + step) > 255)
					fwd = false;
				else {
					color += step;
					FlashColor();
				}
			else
				if ((color - step) < 0)
					fwd = true;
				else {
					color -= step;
					FlashColor();
				}
			if ((counter >= stepCount) && (FlashCount != -1))
				Cancel();
		}
		
		public bool FlashingTimer()
		{
/*			if (Active == true)
				foreach (FlashWidgetController ctrl in list)
					if (ctrl != null)
						ctrl.*/
			Flash();
			if (IsActive == false)
				Cancel();
/*			if (removelist.Count > 0) {
				foreach (FlashWidgetController ctrl in removelist)
					list.Remove(ctrl);
				removelist.Clear();
			}*/
			return (IsActive);
		}

		///<summary>
		/// Start flashing of the widget
		///</summary>
		public void Start()
		{
			// calculate helper vars
			float dstepsPerFlash = ((FlashDuration / 2) / StepDuration);
			if (dstepsPerFlash > 255) {
				dstepsPerFlash = 255;
				stepDuration = FlashDuration % 255;
			}
			// calculate color step
			float dstep = (255 / dstepsPerFlash);
			// correct step if needed
			if (dstep > 255)
				step = 255;
			else {
				if (dstep < 1)
					step = 1;
				else
					step = System.Convert.ToByte (dstep);
			}
			stepCount = System.Convert.ToInt32 (FlashCount * (dstepsPerFlash * 2)) + 1;
			// call flash method
			GLib.Timeout.Add ((uint) StepDuration, new GLib.TimeoutHandler (FlashingTimer));
		}
		
		private WidgetFlasher()
		{
			throw new Exception ("Error: WidgetFlasher > Created With Widget Null Widget!");
		}
		
		public WidgetFlasher (Gtk.Widget aWidget)
		{
			flashedWidget = aWidget;
			Start();
		}
		
		public WidgetFlasher (Gtk.Widget aWidget, int aFlashDuration, int aStepDuration)
		{
			flashedWidget = aWidget;
			flashDuration = aFlashDuration;
			stepDuration = aStepDuration;
			Start();
		}
		
		public WidgetFlasher (Gtk.Widget aWidget, int aFlashCount)
		{
			flashedWidget = aWidget;
			flashCount = aFlashCount;
			if (FlashCount < 0)
				flashType = FlashingType.LoopFlash;
			if (FlashCount > 1)
				flashType = FlashingType.TimedFlash;
			Start();
		}
		
		public WidgetFlasher (Gtk.Widget aWidget, int aFlashCount, int aFlashDuration, int aStepDuration)
		{
			//Console.WriteLine ("WidgetFlasher()");
			flashedWidget = aWidget;
			flashCount = aFlashCount;
			flashDuration = aFlashDuration;
			stepDuration = aStepDuration;
			if (FlashCount == 1) {
				Cancel();
				return;
			}
			if (FlashCount < 0)
				flashType = FlashingType.LoopFlash;
			if (FlashCount > 1)
				flashType = FlashingType.TimedFlash;
			Start();
		}
		
		~WidgetFlasher()
		{
			Cancel();
		}
	}
}
