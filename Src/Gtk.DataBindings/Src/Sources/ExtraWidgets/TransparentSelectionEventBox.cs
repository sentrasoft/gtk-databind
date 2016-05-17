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
using Gtk;
using Gdk;
using Cairo;

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// 
	/// </summary>
	public class TransparentSelectionEventBox : EventBox
	{
		private double opacity = 0;
		public double Opacity {
			get { return (opacity); }
			set { 
				if (opacity == value)
					return;
				opacity = value; 
				QueueDraw();
			}
		}
		
		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			if (Opacity > 0) {
				//System.Console.WriteLine("WTF" + Opacity.ToString());
				Cairo.Context g = Gdk.CairoHelper.Create (evnt.Window);
				//System.Console.WriteLine(Style.Backgrounds[(int) StateType.Selected]);
				Gdk.Color selection = Style.Backgrounds[(int) StateType.Selected];
				g.Color = new Cairo.Color (System.Convert.ToDouble(selection.Red)/(255.0*255.0),
				                           System.Convert.ToDouble(selection.Blue)/(255.0*255.0),
				                           System.Convert.ToDouble(selection.Green)/(255.0*255.0),
				                           .1);
				//System.Console.WriteLine(evnt.Area.Left);
				g.MoveTo (System.Convert.ToDouble(evnt.Area.Left),
				          System.Convert.ToDouble(evnt.Area.Top));
				g.LineTo (System.Convert.ToDouble(evnt.Area.Left + evnt.Area.Width),
				          System.Convert.ToDouble(evnt.Area.Top));
				g.LineTo (System.Convert.ToDouble(evnt.Area.Left + evnt.Area.Width),
				          System.Convert.ToDouble(evnt.Area.Top + evnt.Area.Bottom));
				g.LineTo (System.Convert.ToDouble(evnt.Area.Left),
				          System.Convert.ToDouble(evnt.Area.Top + evnt.Area.Bottom));
				g.LineTo (System.Convert.ToDouble(evnt.Area.Left),
				          System.Convert.ToDouble(evnt.Area.Top));
				g.ClosePath ();
				g.Fill();
				((IDisposable) g.Target).Dispose ();                                      
				((IDisposable) g).Dispose ();
			}
			// Draw children
			if (Child != null)
				PropagateExpose (Child, evnt);

			return (true);
		}

		public TransparentSelectionEventBox()
		{
		}
	}
}
