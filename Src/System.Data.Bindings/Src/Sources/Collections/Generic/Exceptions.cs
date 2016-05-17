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

namespace System.Data.Bindings.Collections.Generic
{
	/// <summary>
	/// Provides exception which is thrown whenever generic list gets wrong type trough
	/// IList methods
	/// </summary>
	public class ItemNotFoundException : Exception
	{
		public ItemNotFoundException()
			: base ("Item not found")
		{
		}
	}

	/// <summary>
	/// Provides exception which is thrown whenever generic list gets wrong type trough
	/// IList methods
	/// </summary>
	public class ExceptionWrongGenericType : Exception
	{
		private ExceptionWrongGenericType()
		{
		}
		
		public ExceptionWrongGenericType (System.Type aPassedType, System.Type aDefinedType)
			: base (string.Format ("Wrong type({0}) passed to generic<{1}> class", aPassedType, aDefinedType))
		{
		}
	}
}
