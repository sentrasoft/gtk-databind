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
using Gtk.DataBindings;

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// Cell that supports drawing tet in different states
	/// </summary>
	public class DrawingCellEditText : DrawingCellText, ICustomGtkState
	{
		private bool error = false;
		/// <value>
		/// Specifies if error should be drawn
		/// </value>
		public virtual bool Error {
			get { return (error); }
			set {
				if (error == value)
					return;
				error = value;
				OnPropertyChanged ("Error");
			}
		}
		
		private bool isselected = false;
		/// <value>
		/// Specifies if selection should be drawn
		/// </value>
		public virtual bool Selected {
			get { return (isselected); }
			set {
				if (isselected == value)
					return;
				isselected = value;
				OnPropertyChanged ("Selected");
			}
		}

		private bool isImportant = false;
		/// <value>
		/// Defines text as important
		/// </value>
		public bool IsImportant {
			get { return (isImportant); }
			set {
				if (isImportant == value)
					return;
				isImportant = value;
				OnPropertyChanged ("IsImportant");
			}
		}
		
		private bool selectionOrErrorIsImportrant = true;
		/// <value>
		/// Specifies if error or selection should be drawn as important
		/// </value>
		public bool SelectionOrErrorIsImportant {
			get { return (selectionOrErrorIsImportrant); }
			set {
				if (selectionOrErrorIsImportrant == value)
					return;
				selectionOrErrorIsImportrant = value;
				OnPropertyChanged ("SelectionOrErrorIsImportant");
			}
		}

		/// <value>
		/// Checks if text should be drawn as important
		/// </value>
		protected bool DrawImportant {
			get { return ((IsImportant == true) || ((SelectionOrErrorIsImportant == true) && ((Selected == true) || (Error == true)))); }
		}
		
		/// <value>
		/// Returns markup string for display text
		/// </value>
		public override string DisplayText {
			get {
				string s = base.DisplayText;
				if ((Arguments != null) && (Arguments.PassedArguments != null) && (Arguments.PassedArguments.IsRenderer == true)) {}
				else if (DrawImportant == true)
					s = "<b>" + s + "</b>";
				if (Master == null)
					return (s);
				if (Arguments.ActionType == CellAction.Paint) {
					Style st;
					bool sens = true;
					Widget wdg = null;
					if ((Arguments != null) && (Arguments.PassedArguments != null) && (Arguments.PassedArguments.IsRenderer == true) && (Arguments.PassedArguments.Widget != null)) {
						wdg = (Gtk.Widget) Arguments.PassedArguments.Widget;
						sens = ((Arguments.PassedArguments.Flags & CellRendererState.Insensitive) != CellRendererState.Insensitive);
						Selected = ((Arguments.PassedArguments.Flags & CellRendererState.Selected) == CellRendererState.Selected);
					}
					else if ((Arguments != null) && (Arguments.PassedArguments != null) && (Arguments.PassedArguments.Widget != null))
						wdg = (Gtk.Widget) Arguments.PassedArguments.Widget;
					else
						wdg = ChameleonTemplates.Entry;
					using (st = Rc.GetStyle (wdg)) {
						if (TypeValidator.IsCompatible(Master.GetType(), typeof(Gtk.Widget)) == true)
							sens = this.MasterIsSensitive();
						if (sens == false) {
							string selbkg = st.Backgrounds[(int) StateType.Insensitive].ToHtmlColor();
							string seltxt = st.Foregrounds[(int) StateType.Insensitive].ToHtmlColor();
							s = "<span foreground=\"" + seltxt + 
								((Arguments.PassedArguments.IsRenderer == false) ? ("\" background=\"") + selbkg : "") + 
								"\">" + s;
							s = s + "</span>";
						}
						else if (Selected == true) {
							string selbkg = st.Backgrounds[(int) StateType.Selected].ToHtmlColor();
							string seltxt = st.Foregrounds[(int) StateType.Selected].ToHtmlColor();
							s = "<span foreground=\"" + seltxt + 
								((Arguments.PassedArguments.IsRenderer == false) ? ("\" background=\"" + selbkg) : "") + 
								"\">" + s;
							s = s + "</span>";
						}
						else if (Error == true) {
							string selbkg = st.Backgrounds[(int) StateType.Insensitive].ToHtmlColor();
							string seltxt = st.Foregrounds[(int) StateType.Normal].ToHtmlColor();
							s = "<span foreground=\"" + seltxt + 
								((Arguments.PassedArguments.IsRenderer == false) ? ("\" background=\"" + selbkg) : "") + 
								"\">" + s;
							s = s + "</span>";
						}
					}
					//st.Dispose();
					st = null;
				}
				return (s);
			}
		}

		#region ICustomGtkState
		
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
		public ValueResolveMethod StateResolving {
			get { return (stateResolving); }
			set {
				if (stateResolving == value)
					return;
				stateResolving = value;
				OnPropertyChanged ("State");
			}
		}
		
		public StateType State {
			get { 
				if (StateResolving == ValueResolveMethod.FromOwner)
					return (OwnerState);
				return (CustomState);
			}
		}
		
		#endregion ICustomGtkState
		
		public DrawingCellEditText()
			: base ()
		{
		}

		public DrawingCellEditText (string aText)
			: base (aText)
		{
		}
		
		public DrawingCellEditText (string aText, string aSizeText)
			: base (aText, aSizeText)
		{
		}		
	}
}
