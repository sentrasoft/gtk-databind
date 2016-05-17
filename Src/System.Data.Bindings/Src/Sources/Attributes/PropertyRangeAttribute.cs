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
	/// Defines property range
	/// </summary>
	[AttributeUsage (AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
	public class PropertyRangeAttribute : Attribute
	{
		private object min = null;
		/// <value>
		/// Min value
		/// </value>
		public object Min {
			get { return (min); }
			set {
				if (value != null)
					if (TypeValidator.IsCompatible(value.GetType(), typeof(IComparable)) == false)
						throw new NotSupportedException ("Range values must be derived from IComparable");
				if (min == value)
					return;
				min = value;
				if ((min != null) && (max != null))
					if ((Min as IComparable).CompareTo(Max) > 0) {
						object tmp = min;
						min = max;
						max = tmp;
					}
			}
		}
	
		private object max = null;
		/// <value>
		/// Max value
		/// </value>
		public object Max {
			get { return (max); }
			set {
				if (value != null)
					if (TypeValidator.IsCompatible(value.GetType(), typeof(IComparable)) == false)
						throw new NotSupportedException ("Range values must be derived from IComparable");
				if (max == value)
					return;
				max = value;
				if ((min != null) && (max != null))
					if ((Min as IComparable).CompareTo(Max) > 0) {
						object tmp = min;
						min = max;
						max = tmp;
					}
			}
		}
	
		public PropertyRangeAttribute (object aMin, object aMax)
		{
			Min = aMin;
			Max = aMax;
		}
	}
}
