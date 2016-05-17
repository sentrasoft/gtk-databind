
using System;
using System.Data.Bindings;
using Gtk;
using Gtk.DataBindings;

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// Provides classic bin with support to specify state
	/// </summary>
	public class DrawingCellGtkStateBin : DrawingCellBin, IGtkState
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
