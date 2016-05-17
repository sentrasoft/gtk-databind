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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;

namespace System.Data.Bindings
{
	/// <summary>
	/// Provides resolving for valid widgets
	/// </summary>
	public static class WidgetFactory
	{
		private static WidgetFactoryTheme defaultTheme = new WidgetFactoryTheme();
		/// <value>
		/// Specifies default theme for widget factory. This is used trough
		/// FactoryInvocationArgs
		/// </value>
		public static WidgetFactoryTheme DefaultTheme {
			get { return (defaultTheme); }
		}

		private static List<FactoryInvokerClass> widgetHandlers = null;
		/// <value>
		/// Provides list of registered widget creation handlers
		/// </value>
		internal static List<FactoryInvokerClass> WidgetHandlers {
			get {
				if (widgetHandlers == null) {
					widgetHandlers = new List<FactoryInvokerClass>();
					if (cellHandlers == null)
						cellHandlers = new List<CellFactoryInvokerClass>();
					EnumAutoWidgets();
				}
				return (widgetHandlers);
			}
		}
		
		private static List<CellFactoryInvokerClass> cellHandlers = null;
		/// <value>
		/// Provides list of registered cell creation handlers
		/// </value>
		internal static List<CellFactoryInvokerClass> CellHandlers {
			get {
				if (cellHandlers == null) {
					cellHandlers = new List<CellFactoryInvokerClass>();
					if (widgetHandlers == null)
						widgetHandlers = new List<FactoryInvokerClass>();
					EnumAutoWidgets();
				}
				return (cellHandlers);
			}
		}

		/// <summary>
		/// Adds class to list of factory provider handlers
		/// </summary>
		/// <param name="aType">
		/// Factory type <see cref="System.Type"/>
		/// </param>
		/// <param name="aAttr">
		/// Attribute <see cref="WidgetFactoryProviderAttribute"/>
		/// </param>
		private static void AddClass (System.Type aType, WidgetFactoryProviderAttribute aAttr)
		{
			WidgetHandlers.Add (new FactoryInvokerClass (aType, aAttr));
		}
		
		/// <summary>
		/// Adds class to list of factory provider handlers
		/// </summary>
		/// <param name="aType">
		/// Factory type <see cref="System.Type"/>
		/// </param>
		/// <param name="aAttr">
		/// Attribute <see cref="WidgetFactoryProviderAttribute"/>
		/// </param>
		private static void AddClass (System.Type aType, CellFactoryProviderAttribute aAttr)
		{
			CellHandlers.Add (new CellFactoryInvokerClass (aType, aAttr));
		}
		
		/// <summary>
		/// Checks specified type if it provides factory or not and then registers it 
		/// if needed
		/// </summary>
		/// <param name="aType">
		/// Factory type <see cref="System.Type"/>
		/// </param>
		public static void RegisterClass (System.Type aType)
		{
			if (aType == null)
				return;
			object[] attrs = (object[]) aType.GetCustomAttributes (false);
			if ((attrs == null) || (attrs.Length == 0))
				return;
			Attribute attr;
			try {
				foreach (object attrobj in attrs) {
					if (TypeValidator.IsCompatible(attrobj.GetType(), typeof(Attribute)) == false)
						continue;
					attr = (Attribute) attrobj;
					if (TypeValidator.IsCompatible(attr.GetType(), typeof(WidgetFactoryProviderAttribute)) == true)				    
						AddClass (aType, (WidgetFactoryProviderAttribute) attr);
					if (TypeValidator.IsCompatible(attr.GetType(), typeof(CellFactoryProviderAttribute)) == true)
						AddClass (aType, (CellFactoryProviderAttribute) attr);
				}
			}
			catch {}
		}
		
		/// <summary>
		/// Enumerates all factory providers by specified filter
		/// </summary>
		/// <param name="aFilter">
		/// Filter string <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// All invoker classes by specified filter <see cref="IEnumerable"/>
		/// </returns>
		public static IEnumerable AllByFilter (string aFilter)
		{
			aFilter = aFilter.Trim().ToLower();
			foreach (FactoryInvokerClass fi in WidgetHandlers)
				if (fi.FactoryProvider.FactoryFilter == aFilter)
					yield return (fi);
		}
		
		/// <summary>
		/// Enumerates all factory providers by specified filter
		/// </summary>
		/// <param name="aFilter">
		/// Filter string <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// All invoker classes by specified filter <see cref="IEnumerable"/>
		/// </returns>
		public static IEnumerable AllCellsByFilter (string aFilter)
		{
			aFilter = aFilter.Trim().ToLower();
			foreach (CellFactoryInvokerClass fi in CellHandlers)
				if (fi.FactoryProvider.FactoryFilter == aFilter)
					yield return (fi);
		}
		
		/// <summary>
		/// Enumerates all factory providers by specified filter
		/// </summary>
		/// <param name="aFilter">
		/// Filter string <see cref="System.String"/>
		/// </param>
		/// <param name="aType">
		/// Type for which responsible widgets should be enumerated <see cref="System.Type"/>
		/// </param>
		/// <returns>
		/// All invoker classes by specified filter <see cref="IEnumerable"/>
		/// </returns>
		public static IEnumerable AllByFilterForType (string aFilter, System.Type aType)
		{
			aFilter = aFilter.Trim().ToLower();
			foreach (FactoryInvokerClass fi in WidgetHandlers)
				if (TypeValidator.IsCompatible(fi.FactoryProvider.GetType(), typeof(TypeWidgetFactoryProviderAttribute)) == true)
					if (fi.FactoryProvider.FactoryFilter == aFilter)
						if ((fi.FactoryProvider as TypeWidgetFactoryProviderAttribute).ValueType == aType)
							yield return (fi);
		}
		
		/// <summary>
		/// Enumerates all factory providers by specified filter
		/// </summary>
		/// <param name="aFilter">
		/// Filter string <see cref="System.String"/>
		/// </param>
		/// <param name="aType">
		/// Type for which responsible widgets should be enumerated <see cref="System.Type"/>
		/// </param>
		/// <returns>
		/// All invoker classes by specified filter <see cref="IEnumerable"/>
		/// </returns>
		public static IEnumerable AllCellsByFilterForType (string aFilter, System.Type aType)
		{
			aFilter = aFilter.Trim().ToLower();
			foreach (CellFactoryInvokerClass fi in CellHandlers)
				if (TypeValidator.IsCompatible(fi.FactoryProvider.GetType(), typeof(TypeCellFactoryProviderAttribute)) == true)
					if (fi.FactoryProvider.FactoryFilter == aFilter)
						if ((fi.FactoryProvider as TypeCellFactoryProviderAttribute).ValueType == aType)
							yield return (fi);
		}
		
		/// <summary>
		/// Factory handler invokation
		/// </summary>
		/// <param name="aArgs">
		/// Handler name <see cref="System.String"/>
		/// </param>
		/// <param name="aArgs">
		/// Widget creation arguments <see cref="FactoryInvocationArgs"/>
		/// </param>
		/// <returns>
		/// Result widget <see cref="IAdaptableControl"/>
		/// </returns>
		private static IAdaptableControl InvokeHandler (string aHandler, FactoryInvocationArgs aArgs)
		{
			foreach (string fn in aArgs.Filter)
				foreach (FactoryInvokerClass fi in AllByFilter(fn))
					if (fi.FactoryProvider.HandlerType == aHandler)
						return (fi.Invoke (aArgs));
			throw new NotImplementedException (string.Format ("Handler [{0}] in filter [{1}] is not registered", aHandler, aArgs.Filter));
		}
		
		/// <summary>
		/// Factory handler invokation
		/// </summary>
		/// <param name="aArgs">
		/// Handler name <see cref="System.String"/>
		/// </param>
		/// <param name="aArgs">
		/// Widget creation arguments <see cref="FactoryInvocationArgs"/>
		/// </param>
		/// <returns>
		/// Result widget <see cref="IMappedColumnItem"/>
		/// </returns>
		private static IMappedColumnItem InvokeCellHandler (string aHandler, FactoryInvocationArgs aArgs)
		{
			foreach (string fn in aArgs.Filter)
				foreach (CellFactoryInvokerClass fi in AllCellsByFilter(fn))
					if (fi.FactoryProvider.HandlerType == aHandler)
						return (fi.Invoke (aArgs));
			throw new NotImplementedException (string.Format ("Handler [{0}] in filter [{1}] is not registered", aHandler, aArgs.Filter));
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
		/// <remarks>
		/// Throws exception
		/// </remarks>
		private static IAdaptableControl CreateDefaultWidget (FactoryInvocationArgs aArgs)
		{
			throw new NotSupportedException ("Default factory method must be specified");
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
		/// <remarks>
		/// Throws exception
		/// </remarks>
		private static IMappedColumnItem CreateDefaultCellWidget (FactoryInvocationArgs aArgs)
		{
			throw new NotSupportedException ("Default factory method must be specified");
		}
		
		/// <summary>
		/// Factory handler selector
		/// </summary>
		/// <param name="aDefaultMethod">
		/// Default creation method fallback <see cref="WidgetCreationEvent"/>
		/// </param>
		/// <param name="aArgs">
		/// Widget creation arguments <see cref="FactoryInvocationArgs"/>
		/// </param>
		/// <returns>
		/// Result widget <see cref="IAdaptableControl"/>
		/// </returns>
		private static IAdaptableControl CreateHandlerWidget (WidgetCreationEvent aDefaultMethod, FactoryInvocationArgs aArgs)
		{
			if ((aArgs.HandlerOverride == "") && (aArgs.HandlerOverride.ToLower().Trim() == "default")) {
				if (aArgs.Description == null)
					return (aDefaultMethod (aArgs));
			
				if (aArgs.Description.HandlerType == PropertyHandlerType.Default)
					return (aDefaultMethod (aArgs));
			}

			if ((aArgs.Filter == null) || (aArgs.Filter.Length == 0))
				throw new NotSupportedException ("Widget created by Handler has to define filter");
			
			string handler = "default";
			if (aArgs.State == PropertyDefinition.ReadOnly) {
				if ((aArgs.HandlerOverride != "") && (aArgs.HandlerOverride.ToLower().Trim() != "default"))
					handler = aArgs.HandlerOverride;
				else if ((aArgs.Description.ReadOnlyDataTypeHandler != "default") && (aArgs.Description.ReadOnlyDataTypeHandler != ""))
					handler = aArgs.Description.ReadOnlyDataTypeHandler;
				else if ((aArgs.Description.DataTypeHandler != "default") && (aArgs.Description.DataTypeHandler != ""))
					handler = aArgs.Description.DataTypeHandler;
				else
					return (aDefaultMethod (aArgs));
			}
			else {
				if ((aArgs.HandlerOverride != "") && (aArgs.HandlerOverride.ToLower().Trim() != "default"))
					handler = aArgs.HandlerOverride;
				else if ((aArgs.Description.DataTypeHandler != "default") && (aArgs.Description.DataTypeHandler != ""))
					handler = aArgs.Description.DataTypeHandler;
				else
					return (aDefaultMethod (aArgs));
			}
System.Console.WriteLine("Invoking handler [{0}]", handler);
			if (handler == "default")
				return (aDefaultMethod (aArgs));
			return (InvokeHandler (handler, aArgs));
		}
		
		/// <summary>
		/// Factory handler selector
		/// </summary>
		/// <param name="aDefaultMethod">
		/// Default creation method fallback <see cref="CellCreationEvent"/>
		/// </param>
		/// <param name="aArgs">
		/// Widget creation arguments <see cref="FactoryInvocationArgs"/>
		/// </param>
		/// <returns>
		/// Result widget <see cref="IMappedColumnItem"/>
		/// </returns>
		private static IMappedColumnItem CreateHandlerCell (CellCreationEvent aDefaultMethod, FactoryInvocationArgs aArgs)
		{
			if ((aArgs.HandlerOverride == "") && (aArgs.HandlerOverride.ToLower().Trim() == "default")) {
				if (aArgs.Description == null)
					return (aDefaultMethod (aArgs));
			
				if (aArgs.Description.HandlerType == PropertyHandlerType.Default)
					return (aDefaultMethod (aArgs));
			}

			if ((aArgs.Filter == null) || (aArgs.Filter.Length == 0))
				throw new NotSupportedException ("Widget created by Handler has to define filter");
			
			string handler = "default";
			if (aArgs.State == PropertyDefinition.ReadOnly) {
				if ((aArgs.HandlerOverride != "") && (aArgs.HandlerOverride.ToLower().Trim() != "default"))
					handler = aArgs.HandlerOverride;
				else if ((aArgs.Description.ReadOnlyDataTypeHandler != "default") && (aArgs.Description.ReadOnlyDataTypeHandler != ""))
					handler = aArgs.Description.ReadOnlyDataTypeHandler;
				else if ((aArgs.Description.DataTypeHandler != "default") && (aArgs.Description.DataTypeHandler != ""))
					handler = aArgs.Description.DataTypeHandler;
				else
					return (aDefaultMethod (aArgs));
			}
			else {
				if ((aArgs.HandlerOverride != "") && (aArgs.HandlerOverride.ToLower().Trim() != "default"))
					handler = aArgs.HandlerOverride;
				else if ((aArgs.Description.DataTypeHandler != "default") && (aArgs.Description.DataTypeHandler != ""))
					handler = aArgs.Description.DataTypeHandler;
				else
					return (aDefaultMethod (aArgs));
			}
System.Console.WriteLine("Invoking handler [{0}]", handler);
			if (handler == "default")
				return (aDefaultMethod (aArgs));
			return (InvokeCellHandler (handler, aArgs));
		}
		
		/// <summary>
		/// Factory widget creation
		/// </summary>
		/// <param name="aDefaultMethod">
		/// Default creation method fallback <see cref="WidgetCreationEvent"/>
		/// </param>
		/// <param name="aArgs">
		/// Widget creation arguments <see cref="FactoryInvocationArgs"/>
		/// </param>
		/// <returns>
		/// Result widget <see cref="IAdaptableControl"/>
		/// </returns>
		public static IAdaptableControl CreateWidget (WidgetCreationEvent aDefaultMethod, FactoryInvocationArgs aArgs)
		{
			if (aDefaultMethod == null)
				aDefaultMethod = new WidgetCreationEvent (CreateDefaultWidget);
			
			IAdaptableControl wdg = null;
			if (aArgs.PropertyInfo == null)
				return (null);
			if (aArgs.Description != null) {
				switch (aArgs.Description.HandlerType) {
				case PropertyHandlerType.Default:
					wdg = aDefaultMethod (aArgs);
					break;
				case PropertyHandlerType.Custom:
					wdg = CreateHandlerWidget (aDefaultMethod, aArgs);
					break;
				}
			}
			else if ((aArgs.HandlerOverride != "") && (aArgs.HandlerOverride.ToLower().Trim() != "default"))
				wdg = CreateHandlerWidget (aDefaultMethod, aArgs);
			else
				wdg = aDefaultMethod (aArgs);
			
			if (wdg != null)
				wdg.InheritedDataSource = true;
			return (wdg);
		}

		/// <summary>
		/// Factory widget creation
		/// </summary>
		/// <param name="aDefaultMethod">
		/// Default creation method fallback <see cref="CellCreationEvent"/>
		/// </param>
		/// <param name="aArgs">
		/// Widget creation arguments <see cref="FactoryInvocationArgs"/>
		/// </param>
		/// <returns>
		/// Result widget <see cref="IMappedColumnItem"/>
		/// </returns>
		public static IMappedColumnItem CreateCell (CellCreationEvent aDefaultMethod, FactoryInvocationArgs aArgs)
		{
System.Console.WriteLine("CreateCell");
			if (aDefaultMethod == null)
				aDefaultMethod = new CellCreationEvent (CreateDefaultCellWidget);
			
			IMappedColumnItem wdg = null;
			if (aArgs.PropertyInfo == null)
				return (null);
			if (aArgs.Description != null) {
				switch (aArgs.Description.HandlerType) {
				case PropertyHandlerType.Default:
					wdg = aDefaultMethod (aArgs);
					break;
				case PropertyHandlerType.Custom:
					wdg = CreateHandlerCell (aDefaultMethod, aArgs);
					break;
				}
			}
			else if ((aArgs.HandlerOverride != "") && (aArgs.HandlerOverride.ToLower().Trim() != "default"))
				wdg = CreateHandlerCell (aDefaultMethod, aArgs);
			else
				wdg = aDefaultMethod (aArgs);
			
			return (wdg);
		}

		/// <summary>
		/// Current assembly wide search for the type, type will be found even
		/// if this assembly doesn't reference to the assembly where type resides
		/// As long as type is loaded, it will found it no matter what.
		/// </summary>
		/// <param name="aAssembly">
		/// Assembly to check in <see cref="Assembly"/>
		/// </param>
		/// <param name="aCache">
		/// Cache of already checked libraries <see cref="StringCollection"/>
		/// </param>
		private static void GetWidgetsInAssembly (Assembly aAssembly, StringCollection aCache)
		{
			bool found = false;
			foreach (System.Type exptype in aAssembly.GetExportedTypes()) {
				foreach (Attribute attr in exptype.GetCustomAttributes (false)) {
					if ((TypeValidator.IsCompatible(attr.GetType(), typeof(WidgetFactoryProviderAttribute)) == true) ||
					    (TypeValidator.IsCompatible(attr.GetType(), typeof(CellFactoryProviderAttribute)) == true)) {
						RegisterClass (exptype);
						break;
					}
				}
			}
		}
		
		/// <summary>
		/// Current assembly wide search for the type, type will be found even
		/// if this assembly doesn't reference to the assembly where type resides
		/// As long as type is loaded, it will found it no matter what.
		/// </summary>
		/// <param name="aAssembly">
		/// Assembly to check in <see cref="Assembly"/>
		/// </param>
		/// <param name="aCache">
		/// Cache of already checked libraries <see cref="StringCollection"/>
		/// </param>
		private static void GetWidgetsInReferencedAssemblies (Assembly aAssembly, StringCollection aCache)
		{
			if (aAssembly == null)
				return;
				
			// Find type in assembly
			GetWidgetsInAssembly (aAssembly, aCache);
				
			// Go trough assembly referenced assemblies
			foreach (AssemblyName mod in aAssembly.GetReferencedAssemblies())
				if (mod != null) {
					if (aCache.Contains(mod.Name) == false) {
						aCache.Add(mod.Name);
						GetWidgetsInReferencedAssemblies (Assembly.Load(mod), aCache);
					}
				}
		}

		private static StringCollection cache = new StringCollection();

		/// <summary>
		/// Processing method which should be called on loading new 
		/// assembly in runtime
		/// </summary>
		/// <param name="aAssembly">
		/// Loaded assembly <see cref="Assembly"/>
		/// </param>
		internal static void HandleAssemblyLoaded (object aSender, NewAssemblyLoadedEventArgs aArgs)
		{
			GetWidgetsInReferencedAssemblies (aArgs.NewAssembly, cache);
		}
		
		/// <summary>
		/// Enumerates IAdaptorSelector classes and creates one
		/// instance into list of possible selectors
		/// </summary>
		internal static void EnumAutoWidgets()
		{
			// Since basic GetType already searches in mscorlib we can safely put it as already done
			if (cache.Count == 0) {
				cache.Add ("mscorlib");
				AssemblyEngine.AssemblyLoaded += HandleAssemblyLoaded;
			}
			
			GetWidgetsInReferencedAssemblies (Assembly.GetEntryAssembly(), cache);
		}
	}
}
