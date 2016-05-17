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
	/// Provides routines for basic conversions
	/// </summary>
	public static class TypeConversions
	{
		/// <summary>
		/// Converts number to currency string
		/// </summary>
		/// <param name="aObject">
		/// Object representing value <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// String representation <see cref="System.String"/>
		/// </returns>
		public static string NumberToCurrencyString (object aObject)
		{
			return (NumberToStringType (aObject, "C"));
		}

		/// <summary>
		/// Converts number to financial string
		/// </summary>
		/// <param name="aObject">
		/// Object representing value <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// String representation <see cref="System.String"/>
		/// </returns>
		public static string NumberToFinancialString (object aObject)
		{
			return (NumberToStringType (aObject, "N"));
		}

		public static string NumberToPercentString (object aObject)
		{
			return (NumberToStringType (aObject, "P"));
		}

		public static string NumberToNumericString (object aObject)
		{
			return (NumberToStringType (aObject, "F"));
		}

		/// <summary>
		/// Converts number to string based on conversion type
		/// </summary>
		/// <param name="aData">
		/// Object representing value <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// String representation <see cref="System.String"/>
		/// </returns>
		private static string NumberToStringType (object aData, string aConversionType)
		{
			if ((aData == null) || (TypeValidator.IsNumeric(aData.GetType()) == false))
			    return (0.ToString (aConversionType));
			System.Type type = aData.GetType();
			if (type == typeof(byte))
				return (((byte) aData).ToString (aConversionType));
			if (type == typeof(int))
				return (((int) aData).ToString (aConversionType));
			if (type == typeof(Int16))
				return (((Int16) aData).ToString (aConversionType));
			if (type == typeof(UInt16))
				return (((UInt16) aData).ToString (aConversionType));
			if (type == typeof(Int32))
				return (((Int32) aData).ToString (aConversionType));
			if (type == typeof(UInt32))
				return (((UInt32) aData).ToString (aConversionType));
			if (type == typeof(Int64))
				return (((Int64) aData).ToString (aConversionType));
			if (type == typeof(UInt64))
				return (((UInt64) aData).ToString (aConversionType));
			if (type == typeof(float))
				return (((float) aData).ToString (aConversionType));
			if (type == typeof(decimal))
				return (((decimal) aData).ToString (aConversionType));
			if (type == typeof(double))
				return (((double) aData).ToString (aConversionType));
			return (0.ToString (aConversionType));
		}
	}
}
