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
using System.Collections;

namespace Gtk.DataBindings
{
	/// <summary>
	/// Provides factory
	/// </summary>
	public static class GtkWidgetFactory
	{
		/// <summary>
		/// Checks specified type if it provides factory or not and then registers it 
		/// if needed
		/// </summary>
		/// <param name="aType">
		/// Factory type <see cref="System.Type"/>
		/// </param>
		public static void RegisterClass (System.Type aType)
		{
			WidgetFactory.RegisterClass (aType);
		}

		/// <summary>
		/// Enumerates all factory providers registred for Gtk
		/// </summary>
		/// <returns>
		/// All invoker classes by specified filter <see cref="IEnumerable"/>
		/// </returns>
		public static IEnumerable AllWidgets()
		{
			return (WidgetFactory.AllByFilter ("Gtk"));
		}
		
		/// <summary>
		/// Enumerates all factory providers registred for Gtk
		/// </summary>
		/// <returns>
		/// All invoker classes by specified filter <see cref="IEnumerable"/>
		/// </returns>
		public static IEnumerable AllCellWidgets()
		{
			return (WidgetFactory.AllCellsByFilter ("Gtk"));
		}
		
		/// <summary>
		/// Enumerates all factory providers by specified filter
		/// </summary>
		/// <param name="aType">
		/// Type for which responsible widgets should be enumerated <see cref="System.Type"/>
		/// </param>
		/// <returns>
		/// All invoker classes by specified filter <see cref="IEnumerable"/>
		/// </returns>
		public static IEnumerable AllWidgetsForType (System.Type aType)
		{
			return (WidgetFactory.AllByFilterForType ("Gtk", aType));
		}
		
		/// <summary>
		/// Enumerates all factory providers by specified filter
		/// </summary>
		/// <param name="aType">
		/// Type for which responsible widgets should be enumerated <see cref="System.Type"/>
		/// </param>
		/// <returns>
		/// All invoker classes by specified filter <see cref="IEnumerable"/>
		/// </returns>
		public static IEnumerable AllCellWidgetsForType (System.Type aType)
		{
			return (WidgetFactory.AllCellsByFilterForType ("Gtk", aType));
		}
		
		/// <summary>
		/// Creates default widgets for properties which don't specify their property description
		/// </summary>
		/// <param name="aArgs">
		/// Widget creation arguments <see cref="FactoryInvocationArgs"/>
		/// </param>
		/// <returns>
		/// Result widget <see cref="IAdaptableControl"/>
		/// </returns>
		private static IAdaptableControl CreateDefaultGtkWidget (FactoryInvocationArgs aArgs)
		{
			foreach (FactoryInvokerClass invoker in AllWidgetsForType(aArgs.PropertyInfo.PropertyType))
				return (invoker.Invoke (aArgs));

			// Silly fallback if registration is not done
			IAdaptableControl wdg = null;
			switch (aArgs.State) {
			case PropertyDefinition.ReadOnly:
				if (aArgs.PropertyInfo.PropertyType == typeof(string)) {
					wdg = new DataLabel (aArgs.PropertyName);
					break;
				}
				else if (aArgs.PropertyInfo.PropertyType == typeof(bool)) {
					wdg = new DataCheckButton (aArgs.PropertyName);
					(wdg as DataCheckButton).Editable = false;
				}
				else if (aArgs.PropertyInfo.PropertyType == typeof(Gdk.Pixbuf)) {
					wdg = new DataImage (aArgs.PropertyName);
				}
				else {
					wdg = new DataLabel (aArgs.PropertyName);
				}
				break;
			case PropertyDefinition.ReadWrite:
				if (aArgs.PropertyInfo.PropertyType == typeof(string)) {
					wdg = new DataEntry (aArgs.PropertyName);
				}
				else if (aArgs.PropertyInfo.PropertyType == typeof(bool)) {
					wdg = new DataCheckButton (aArgs.PropertyName);
				}
				else if ((aArgs.PropertyInfo.PropertyType == typeof(int)) || (aArgs.PropertyInfo.PropertyType == typeof(float)) || (aArgs.PropertyInfo.PropertyType == typeof(double))) {
					wdg = new DataSpinButton (-100000000, 100000000, 1, aArgs.PropertyName);
				}
				else if (aArgs.PropertyInfo.PropertyType == typeof(DateTime)) {
					wdg = new DataCalendar (aArgs.PropertyName);
				}
				else if (aArgs.PropertyInfo.PropertyType == typeof(Gdk.Pixbuf)) {
					wdg = new DataImage (aArgs.PropertyName);
				}
				else {
					wdg = new DataLabel (aArgs.PropertyName);
				}
				break;
			}
			return (wdg);
		}

		/// <summary>
		/// Creates default widgets for properties which don't specify their property description
		/// </summary>
		/// <param name="aArgs">
		/// Widget creation arguments <see cref="FactoryInvocationArgs"/>
		/// </param>
		/// <returns>
		/// Result widget <see cref="IMappedColumnItem"/>
		/// </returns>
		private static IMappedColumnItem CreateDefaultGtkCellWidget (FactoryInvocationArgs aArgs)
		{
			foreach (CellFactoryInvokerClass invoker in AllCellWidgetsForType(aArgs.PropertyInfo.PropertyType))
				return (invoker.Invoke (aArgs));

			// Silly fallback if registration is not done
			IMappedColumnItem wdg = null;
			switch (aArgs.State) {
			case PropertyDefinition.ReadOnly:
				if (aArgs.PropertyInfo.PropertyType == typeof(string)) {
					wdg = new MappedCellRendererText();
					break;
				}
				else if (aArgs.PropertyInfo.PropertyType == typeof(bool)) {
					wdg = new MappedCellRendererToggle();
//					(wdg as DataCheckButton).Editable = false;
				}
				else if (aArgs.PropertyInfo.PropertyType == typeof(Gdk.Pixbuf)) {
					wdg = new MappedCellRendererPixbuf();
				}
				else {
					wdg = new MappedCellRendererText();
				}
				break;
			case PropertyDefinition.ReadWrite:
				if (aArgs.PropertyInfo.PropertyType == typeof(string)) {
					wdg = new MappedCellRendererText();
				}
				else if (aArgs.PropertyInfo.PropertyType == typeof(bool)) {
					wdg = new MappedCellRendererToggle();
				}
				else if ((aArgs.PropertyInfo.PropertyType == typeof(int)) || (aArgs.PropertyInfo.PropertyType == typeof(float)) || (aArgs.PropertyInfo.PropertyType == typeof(double))) {
					wdg = new MappedCellRendererSpin();
				}
				else if (aArgs.PropertyInfo.PropertyType == typeof(DateTime)) {
					wdg = new MappedCellRendererText();
				}
				else if (aArgs.PropertyInfo.PropertyType == typeof(Gdk.Pixbuf)) {
					wdg = new MappedCellRendererPixbuf();
				}
				else {
					wdg = new MappedCellRendererText();
				}
				break;
			}
			wdg.MappedTo = aArgs.PropertyName;
			
			return (wdg);
		}

		private static WidgetCreationEvent gtkCreationMethod = new WidgetCreationEvent (CreateDefaultGtkWidget);
		private static CellCreationEvent gtkCellCreationMethod = new CellCreationEvent (CreateDefaultGtkCellWidget);
		
		/// <summary>
		/// Factory widget creation
		/// </summary>
		/// <param name="aArgs">
		/// A <see cref="FactoryInvocationArgs"/>
		/// </param>
		/// <returns>
		/// A <see cref="IAdaptableControl"/>
		/// </returns>
		public static IAdaptableControl CreateWidget (FactoryInvocationArgs aArgs)
		{
			return (WidgetFactory.CreateWidget (gtkCreationMethod, aArgs));
		}

		/// <summary>
		/// Factory widget creation
		/// </summary>
		/// <param name="aArgs">
		/// A <see cref="FactoryInvocationArgs"/>
		/// </param>
		/// <returns>
		/// A <see cref="IMappedColumnItem"/>
		/// </returns>
		public static IMappedColumnItem CreateCell (FactoryInvocationArgs aArgs)
		{
			return (WidgetFactory.CreateCell (gtkCellCreationMethod, aArgs));
		}
	}
}
