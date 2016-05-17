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

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// Provides store where base classes can be found and shared
	/// </summary>
	public static class ChameleonTemplates
	{
		private static Button button = null;
		/// <value>
		/// Provides button template
		/// </value>
		public static Button Button {
			get {
				if (button == null)
					button = new Button();
				return (button);
			}
		}

		private static Entry entry = null;
		/// <value>
		/// Provides entry template
		/// </value>
		public static Entry Entry {
			get {
				if (entry == null)
					entry = new Entry();
				return (entry);
			}
		}

		private static ComboBox comboBox = null;
		/// <value>
		/// Provides entry template
		/// </value>
		public static ComboBox ComboBox {
			get {
				if (comboBox == null)
					comboBox = new ComboBox();
				return (comboBox);
			}
		}

		private static Arrow arrow = null;
		/// <value>
		/// Provides entry template
		/// </value>
		public static Arrow Arrow {
			get {
				if (arrow == null)
					arrow = new Arrow (ArrowType.Down, ShadowType.EtchedIn);
				return (arrow);
			}
		}

		private static EventBox eventBox = null;
		/// <value>
		/// Provides entry template
		/// </value>
		public static EventBox EventBox {
			get {
				if (eventBox == null)
					eventBox = new EventBox();
				return (eventBox);
			}
		}
		
		private static HSeparator hSeparator = null;
		/// <value>
		/// Provides HSeparator template
		/// </value>
		public static HSeparator HSeparator {
			get {
				if (hSeparator == null)
					hSeparator = new HSeparator();
				return (hSeparator);
			}
		}

		private static VSeparator vSeparator = null;
		/// <value>
		/// Provides VSeparator template
		/// </value>
		public static VSeparator VSeparator {
			get {
				if (vSeparator == null)
					vSeparator = new VSeparator();
				return (vSeparator);
			}
		}

		private static HScale hScale = null;
		/// <value>
		/// Provides HScale template
		/// </value>
		public static HScale HScale {
			get {
				if (hScale == null)
					hScale = new HScale (0, 100, 1);
				return (hScale);
			}
		}

		private static VScale vScale = null;
		/// <value>
		/// Provides VScale template
		/// </value>
		public static VScale VScale {
			get {
				if (vScale == null)
					vScale = new VScale (0, 100, 1);
				return (vScale);
			}
		}

		private static ProgressBar progressBar = null;
		/// <value>
		/// Provides VScale template
		/// </value>
		public static ProgressBar ProgressBar {
			get {
				if (progressBar == null)
					progressBar = new ProgressBar();
				return (progressBar);
			}
		}
	}
}
