//AdaptorSelector.cs - Field Attribute to assign additional information for Gtk#Databindings
//
//Author: m. <ml@arsis.net>
//
//Copyright (c) at 10:50 PM 12/11/2008
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;

namespace System.Data.Bindings
{
	/// <summary>
	/// 
	/// </summary>
	public static class AdaptorSelector
	{
//		private static ArrayList selectors = null;
		private static List<IAdaptorSelector> selectors = null;
		private static IAdaptorSelector baseselector = new DefaultAdaptorSelector();
		
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
		internal static void GetSelectorsInAssembly (Assembly aAssembly, StringCollection aCache)
		{
			bool found = false;
			foreach (System.Type exptype in aAssembly.GetExportedTypes()) {
				found = false;
				if (exptype.GetCustomAttributes(typeof(AdaptorSelectorAttribute), false).Length > 0) {
					foreach (IAdaptorSelector sel in selectors) {
						if (sel.GetType() == exptype)
							found = true;	
					}
					if (found == false) {
						ConstructorInfo[] infos = exptype.GetConstructors();
						foreach (ConstructorInfo info in infos)
							if (info.GetParameters().Length == 0)
								selectors.Add ((IAdaptorSelector) info.Invoke (null));
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
		internal static void GetSelectorsInReferencedAssemblies (Assembly aAssembly, StringCollection aCache)
		{
			if (aAssembly == null)
				return;
				
			// Find type in assembly
			GetSelectorsInAssembly (aAssembly, aCache);
				
			// Go trough assembly referenced assemblies
			foreach (AssemblyName mod in aAssembly.GetReferencedAssemblies())
				if (mod != null) {
					if (aCache.Contains(mod.Name) == false) {
						aCache.Add(mod.Name);
						GetSelectorsInReferencedAssemblies (Assembly.Load(mod), aCache);
					}
				}
		}

		/// <summary>
		/// Enumerates IAdaptorSelector classes and creates one
		/// instance into list of possible selectors
		/// </summary>
		public static void EnumAdaptorSelectors()
		{
			StringCollection cache = new StringCollection();

			// Since basic GetType already searches in mscorlib we can safely put it as already done
			cache.Add ("mscorlib");
			
			GetSelectorsInReferencedAssemblies (Assembly.GetEntryAssembly(), cache);
			cache.Clear();
			cache = null;
		}

		/// <summary>
		/// Resolves correct adaptor
		/// </summary>
		/// <param name="aObject">
		/// Object to be checked <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// AdaptorSelector class which defines Adaptor type <see cref="System.Type"/>
		/// </returns>
		public static IAdaptorSelector GetCorrectAdaptor (object aObject)
		{
			if (selectors == null) {
//				selectors = new ArrayList();
				selectors = new List<IAdaptorSelector>();
				EnumAdaptorSelectors();
			}
			foreach (IAdaptorSelector sel in selectors)
				if (sel.CheckType(aObject) != null)
					return (sel);
			return (baseselector);
		}
	}
}
