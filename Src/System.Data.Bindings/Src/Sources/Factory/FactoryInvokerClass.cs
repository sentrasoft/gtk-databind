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
using System.Reflection;

namespace System.Data.Bindings
{	
	/// <summary>
	/// Defines class invoker which is used by factory provider
	/// </summary>
	public class FactoryInvokerClass
	{
		private static MethodInfo ShellMethod = null;
		private static ParameterInfo[] ShellMethodParameters = null;
		
		public IAdaptableControl Invoke (FactoryInvocationArgs aArgs)
		{
			if (FactoryMethod == null)
				throw new NullReferenceException ("Method can't be null");
			return ((IAdaptableControl) FactoryMethod.Invoke (null, new object[1] { aArgs }));
		}
		
		private System.Type factoryType = null;
		/// <value>
		/// Specifies type of factory provider
		/// </value>
		public System.Type FactoryType {
			get { return (factoryType); }
			set {
				if (factoryType == value)
					return;
				factoryType = value;
			}
		}
	
		private WidgetFactoryProviderAttribute factoryProvider = null;
		/// <value>
		/// Specifies factory provider
		/// </value>
		public WidgetFactoryProviderAttribute FactoryProvider {
			get { return (factoryProvider); }
			set {
				if (factoryProvider == value)
					return;
				factoryProvider = value;
			}
		}
	
		private MethodInfo factoryMethod = null;
		/// <value>
		/// Specifies factory provider
		/// </value>
		public MethodInfo FactoryMethod {
			get { return (factoryMethod); }
			set {
				if (factoryMethod == value)
					return;
				factoryMethod = value;
			}
		}

		public FactoryInvokerClass (System.Type aType, WidgetFactoryProviderAttribute aAttr)
		{
			factoryType = aType;
			factoryProvider = aAttr;

			if (ShellMethod == null)
				ShellMethod = this.GetType().GetMethod ("Invoke");
			if (ShellMethodParameters == null)
				ShellMethodParameters = ShellMethod.GetParameters();
			
			MethodInfo[] methods = aType.GetMethods();
			foreach (MethodInfo method in methods) {
				if ((method.IsStatic == true) && (method.Name == aAttr.MethodName)) {
					ParameterInfo[] parms = method.GetParameters();
					if (method.ReturnType != ShellMethod.ReturnType)
						continue;
					if (ShellMethodParameters.Length == parms.Length) {
						for (int i=0; i<ShellMethodParameters.Length; i++)
							if (ShellMethodParameters[i].ParameterType != parms[i].ParameterType)
								continue;
					}
					factoryMethod = method;
				}
			}
			if (factoryMethod == null)
				throw new Exception ("Class doesn't support factory method");
		}
	}
}
