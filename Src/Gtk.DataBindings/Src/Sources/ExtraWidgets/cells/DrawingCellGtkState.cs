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
using System.Data.Bindings;
using Gdk;
using Gtk;
using Gtk.DataBindings;

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// Specifies cell which can specify state or resolve it from owner
	/// </summary>
	public class DrawingCellGtkState : DrawingCellContent, ICustomGtkState
	{
		#region ICustomGtkState
		
		/// <value>
		/// Resolves owners state
		/// </value>
		public StateType OwnerState {
			get { 
				if (Master != null) {
					if (TypeValidator.IsCompatible(Master.GetType(), typeof(IGtkState)) == true)
						return ((Master as IGtkState).State);
					else if (TypeValidator.IsCompatible(Master.GetType(), typeof(Gtk.Widget)) == true)
						return (((Gtk.Widget) Master).State);
				}
				return (CustomState); 
			}
		}
		
		private StateType customState = StateType.Normal;
		/// <value>
		/// Custom specified state
		/// </value>
		public StateType CustomState {
			get { return (customState); }
			set {
				if (customState == value)
					return;
				customState = value;
				OnPropertyChanged ("CustomState");
			}
		}
		
		private ValueResolveMethod stateResolving = ValueResolveMethod.FromOwner;
		/// <value>
		/// Method used to resolve state
		/// </value>
		public ValueResolveMethod StateResolving {
			get { return (stateResolving); }
			set {
				if (stateResolving == value)
					return;
				stateResolving = value;
				OnPropertyChanged ("State");
			}
		}
		
		/// <value>
		/// State as resolved trough specified state resolving method
		/// </value>
		public StateType State {
			get { 
				if (StateResolving == ValueResolveMethod.FromOwner)
					return (OwnerState);
				return (CustomState);
			}
		}
		
		#endregion ICustomGtkState
	}
}
