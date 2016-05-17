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
using System.Reflection;

namespace Gtk.DataBindings
{
	/// <summary>
	/// Provides description for QueryImplementor models
	/// </summary>
	public class QueryModelDescription
	{
		private ConstructorInfo constructor = null;
		
		private QueryModelAttribute definition = null;
		/// <value>
		/// Attribute which specifies Query definitions
		/// </value>
		public QueryModelAttribute Definition {
			get { return (definition); }
		}
		
		private System.Type model = null;
		/// <value>
		/// Model type
		/// </value>
		public System.Type Model {
			get { return (model); }
		}
		
		/// <summary>
		/// Creates new model based on Model type
		/// </summary>
		/// <param name="aImplementor">
		/// Master implementor <see cref="MappingsImplemetor"/>
		/// </param>
		/// <returns>
		/// New query implementor <see cref="QueryImplementor"/>
		/// </returns>
		public QueryImplementor CreateModel (MappingsImplementor aImplementor)
		{
			return ((QueryImplementor) constructor.Invoke (new object[1] {aImplementor}));
		}
		
		/// <summary>
		/// Checks if this model description handles specified type
		/// </summary>
		/// <param name="aType">
		/// Type <see cref="System.Type"/>
		/// </param>
		/// <returns>
		/// true if type is handled, false if not <see cref="System.Boolean"/>
		/// </returns>
		public bool HandlesType (System.Type aType)
		{
			if ((Definition.Inherited == true) || (aType.IsInterface == true))
				return (TypeValidator.IsCompatible(Definition.ListType, aType));
			return (aType == Definition.ListType);
		}
		
		public QueryModelDescription (System.Type aModel, QueryModelAttribute aDescription)
		{
			if (aModel == null)
				throw new NullReferenceException ("QueryModelDescription: Model type can't be null");
			if (aDescription == null)
				throw new NullReferenceException ("QueryModelDescription: Attribute must be specified");

			foreach (ConstructorInfo info in aModel.GetConstructors()) 
				if (info.GetParameters().Length == 1)
					if (TypeValidator.IsCompatible(info.GetParameters()[0].ParameterType, typeof(MappingsImplementor)) == true) {
						constructor = info;
						break;
					}
			
			if (constructor == null)
				throw new NotImplementedException ("QueryModelDescription: QueryImplementor needs public .ctor (MappingsImplementor)");
			model = aModel;
			definition = aDescription;
		}
	}
}
