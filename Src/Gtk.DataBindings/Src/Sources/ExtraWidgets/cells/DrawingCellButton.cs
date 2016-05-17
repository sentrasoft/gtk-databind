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

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// Specifies drawing cell which acts as container and draws like button
	/// </summary>
	public class DrawingCellButton : DrawingCellThemedBin, IActivatable
	{
		private bool pressed = false;
		/// <value>
		/// Specifies if button should be drawn as pressed or not
		/// </value>
		public bool Pressed {
			get { return (pressed); }
			set { 
				if (pressed == value)
					return;
				pressed = value;
				OnPropertyChanged ("Pressed");
			}
		}
		
		private bool hasDefault = false;
		/// <value>
		/// Specifies if button should be drawn as pressed or not
		/// </value>
		public bool HasDefault {
			get { return (hasDefault); }
			set { 
				if (hasDefault == value)
					return;
				hasDefault = value;
				OnPropertyChanged ("HasDefault");
			}
		}
		
		private bool hasFocus = false;
		/// <value>
		/// Specifies if button should be drawn as pressed or not
		/// </value>
		public bool HasFocus {
			get { return (hasFocus); }
			set { 
				if (hasFocus == value)
					return;
				hasFocus = value;
				OnPropertyChanged ("HasFocus");
			}
		}
		
		/// <summary>
		/// Paints cell on cairo context
		/// </summary>
		/// <param name="aArgs">
		/// A <see cref="CellExposeEventArgs"/>
		/// </param>
		public override void PaintBackground (CellExposeEventArgs aArgs)
		{
			Widget wdg = (Widget) Master;
			ResolveStyle();
			Gdk.Rectangle rect = aArgs.CellArea.CopyToGdkRectangle();
			Gdk.Rectangle clip = aArgs.ClippingArea.CopyToGdkRectangle();
			Gdk.Rectangle borderclip = aArgs.ClippingArea.CopyToGdkRectangle();
			borderclip.X += Style.XThickness;
			borderclip.Y += Style.YThickness;
			borderclip.Width -= Style.XThickness*2;
			borderclip.Height -= Style.YThickness*2;
			
			if (wdg.IsSensitive() == false) {
				Style.PaintFlatBox (Style, aArgs.Drawable, StateType.Insensitive, (Pressed == true) ? ShadowType.In : ShadowType.Out, clip, 
				                wdg, "button", rect.X, rect.Y, rect.Width, rect.Height);
				Style.PaintBox (Style, aArgs.Drawable, StateType.Insensitive, (Pressed == true) ? ShadowType.In : ShadowType.Out, clip, 
				                wdg, "button", rect.X, rect.Y, rect.Width, rect.Height);
			}
			else {
				if (HasDefault == true) {
					Style.PaintBox (Style, aArgs.Drawable, StateType.Selected, (Pressed == true) ? ShadowType.In : ShadowType.Out, clip, 
					                wdg, "button", rect.X, rect.Y, rect.Width, rect.Height);
					Style.PaintBox (Style, aArgs.Drawable, StateType.Normal, (Pressed == true) ? ShadowType.In : ShadowType.Out, borderclip, 
					                wdg, "buttondefault", rect.X, rect.Y, rect.Width, rect.Height);
				}
				else
					Style.PaintBox (Style, aArgs.Drawable, State, (Pressed == true) ? ShadowType.In : ShadowType.Out, clip, 
				    	            wdg, "button", rect.X, rect.Y, rect.Width, rect.Height);
				if (HasFocus == true) {
					rect.X += Style.XThickness;
					rect.Y += Style.YThickness;
					rect.Width -= Style.XThickness*2;
					rect.Height -= Style.YThickness*2;
					Style.PaintFocus (Style, aArgs.Drawable, State, borderclip, 
				    	            wdg, "button", rect.X, rect.Y, rect.Width, rect.Height);
				}
			}
			FreeStyle();
		}

		#region IActivatable implementation
		
		private event ActivatedEvent activated = null;
		/// <summary>
		/// Event called on click
		/// </summary>
		public event ActivatedEvent Activated {
			add { activated += value; }
			remove { activated -= value; }
		}
		
		/// <summary>
		/// Calls Activated event
		/// </summary>
		public void Activate ()
		{
			if (activated != null)
				activated (Master, new ActivationEventArgs (this));
		}

		#endregion

		/// <summary>
		/// Resolves style used for drawing this container
		/// </summary>
		/// <returns>
		/// Style <see cref="Style"/>
		/// </returns>
		protected override Style GetStyle ()
		{
			return (Rc.GetStyle (ChameleonTemplates.Button));
		}
		
		public DrawingCellButton()
			: base()
		{
			StateResolving = ValueResolveMethod.Manual;
		}
		
		public DrawingCellButton (IDrawingCell aCell)
			: base (aCell)
		{
			StateResolving = ValueResolveMethod.Manual;
		}
	}
}
