using System;
using System.Data;

namespace System.Data.Bindings.Generic
{
	/// <summary>
	/// Delegate returning whether specified DataRow is in filter or not
	/// </summary>
	public delegate bool DataRowIsInFilterEvent (DataRow aRow);
	
	/// <summary>
	/// Only needed if one needs type safe Rows handing
	/// enumerations and everything can simply go trough Rows as
	/// ordinary DataTable
	/// </summary>
	public interface IGenericDataTable<T> 
		where T : DataRow
	{
		TypedDataRowCollection<T> TypedRows { get; }
	}
}
