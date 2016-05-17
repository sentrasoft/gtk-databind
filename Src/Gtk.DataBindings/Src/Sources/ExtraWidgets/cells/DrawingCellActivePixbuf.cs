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
using System.ComponentModel;
using System.Data.Bindings;
using Gdk;
using Gtk;
using Gtk.DataBindings;

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// 
	/// </summary>
	public class DrawingCellActivePixbuf : DrawingCellPixbuf, IActivatable, ICustomGtkState
	{
		private Gdk.Pixbuf[] statePixbufs = new Gdk.Pixbuf[Enum.GetValues(typeof(StateType)).Length];
		
		protected Gdk.Pixbuf GetPixbuf (Gtk.StateType aState)
		{
			switch (aState) {
			case StateType.Normal:
				return (base.Pixbuf);
			default:
				if (statePixbufs[(int) aState] == null)
					return (base.Pixbuf);
				return (statePixbufs[(int) aState]);
			}
			return (base.Pixbuf);
		}
		
		public override Pixbuf Pixbuf {
			get { return (GetPixbuf (State)); }
			set { base.Pixbuf = value; }
		}

		#region IActivatable

		private event ActivatedEvent activated = null;
		public event ActivatedEvent Activated {
			add { activated += value; }
			remove { activated -= value; }
		}
		
		public void Activate ()
		{
			if (activated != null)
				activated (Master, new ActivationEventArgs (this));
		}
		
		#endregion IActivatable

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
				if (this.MasterIsSensitive() == false)
					return (StateType.Insensitive);
				return (CustomState);
			}
		}
		
		#endregion ICustomGtkState
		
		private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Pixbuf") {
				for (int i=0; i<statePixbufs.Length; i++) {
					if (statePixbufs[i] != null)
						statePixbufs[i].Dispose();
					statePixbufs[i] = null;
				}
				if (pixbuf != null) {
					statePixbufs[(int) Gtk.StateType.Prelight] = pixbuf.CreatePrelightCopy();
					statePixbufs[(int) Gtk.StateType.Insensitive] = pixbuf.CreateAlphaCopy();
				}
			}
		}

		protected override void InitCell()
		{
			PropertyChanged += HandlePropertyChanged;
			StateResolving = ValueResolveMethod.Manual;
		}
		
		public DrawingCellActivePixbuf()
			: base()
		{
		}

		public DrawingCellActivePixbuf (Pixbuf aPixbuf)
			: base (aPixbuf)
		{
		}
		
		public DrawingCellActivePixbuf (string aName)
			: base (aName)
		{
		}
	}
}
