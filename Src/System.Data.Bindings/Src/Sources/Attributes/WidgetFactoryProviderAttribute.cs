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

namespace System.Data.Bindings
{
	/// <summary>
	/// Specifies widget factory provider used for automatic widget creation
	/// </summary>
	[AttributeUsage (AttributeTargets.Class, Inherited=false, AllowMultiple=true)]
	public class WidgetFactoryProviderAttribute : Attribute
	{
		private string methodName = "";
		/// <value>
		/// Provides method name
		/// </value>
		public string MethodName {
			get { return (methodName); }
			set {
				if (methodName == value)
					return;
				methodName = value;
			}
		}
	
		private string handlerType = "";
		/// <value>
		/// Provides handler name
		/// </value>
		public string HandlerType {
			get { return (handlerType); }
			set {
				if (handlerType == value)
					return;
				handlerType = value;
			}
		}
		
		private string factoryFilter = "";
		/// <value>
		/// Provides factory filter
		/// </value>
		public string FactoryFilter {
			get { return (factoryFilter); }
			set {
				if (factoryFilter == value)
					return;
				factoryFilter = value;
			}
		}
		
		private WidgetFactoryProviderAttribute()
		{
		}
		
		public WidgetFactoryProviderAttribute (string aFactoryFilter, string aHandlerType, string aMethodName)
		{
			if (aFactoryFilter.Trim() == "")
				throw new NotSupportedException ("Factory provider without specified filter is not allowed");
			
			factoryFilter = aFactoryFilter.Trim().ToLower();
			handlerType = aHandlerType;
			methodName = aMethodName;
		}
	}
}
