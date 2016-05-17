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
	/// Defines property description
	/// </summary>
	[AttributeUsage (AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
	public class PropertyDescriptionAttribute : Attribute
	{
		private string title = "";
		/// <value>
		/// Property title
		/// </value>
		public string Title {
			get { return (title); }
		}
	
		private string hint = "";
		/// <value>
		/// Hint glued to property
		/// </value>
		public string Hint {
			get { return (hint); }
			set { hint = value; }
		}
	
		private PropertyHandlerType handlerType = PropertyHandlerType.Default;
		/// <value>
		/// Defines if handler type is custom or default
		/// </value>
		public PropertyHandlerType HandlerType {
			get { 
				if ((dataTypeHandler == "") || (dataTypeHandler == "default"))
					return (PropertyHandlerType.Default);
				return (handlerType); 
			}
		}
		
		private string fieldName = "";
		/// <value>
		/// Name of the field for reading and writing
		/// </value>
		public string FieldName {
			get { return (fieldName); }
			set { fieldName = value; }
		}
		
		private string dataTypeHandler = "";
		/// <value>
		/// Defines data type handler
		/// </value>
		public string DataTypeHandler {
			get { 
				if (handlerType == PropertyHandlerType.Default)
					return ("default");
				return (dataTypeHandler); 
			}
			set { 
				if ((value != "") && (value != "default"))
					handlerType = PropertyHandlerType.Custom;
				else
					handlerType = PropertyHandlerType.Default;
				dataTypeHandler = value; 
			}
		}
		
		private string readonlyDataTypeHandler = "";
		/// <value>
		/// Defines readonly type handler
		/// </value>
		public string ReadOnlyDataTypeHandler {
			get { 
				if (handlerType == PropertyHandlerType.Default)
					return ("default");
				return (readonlyDataTypeHandler); 
			}
			set { readonlyDataTypeHandler = value; }
		}
		
		public PropertyDescriptionAttribute (string aTitle)
		{
			title = aTitle;
		}
	}
}
