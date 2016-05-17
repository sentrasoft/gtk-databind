using System;
using System.Data;

namespace System.Data.Bindings.Generic
{
	/// <summary>
	/// Base class which provides automatic support for use in
	/// TypedDataTable
	/// </summary>
	public class BaseTypedDataRow : DataRow
	{		
		public BaseTypedDataRow (DataRowBuilder rowBuilder) 
			: base(rowBuilder)
		{
        }
	}
}
