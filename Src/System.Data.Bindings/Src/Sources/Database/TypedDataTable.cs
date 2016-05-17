using System;
using System.Data;
using System.Reflection;

namespace System.Data.Bindings.Generic
{
	/// <summary>
	/// Type safe DataTable for use with classes inherited from DataRow 
	/// </summary>
	public class TypedDataTable<T> : DataTable
		where T : DataRow
	{
		private object[] cparams = new object[1];
		private ConstructorInfo cons = null;
		
		private TypedDataRowCollection<T> typedRows = null;
		/// <value>
		/// Only needed if one needs type safe Rows handing
		/// enumerations and everything can simply go trough Rows as
		/// ordinary DataTable
		/// </value>
		public TypedDataRowCollection<T> TypedRows { 
			get {
				if (typedRows == null)
					typedRows = new TypedDataRowCollection<T> (Rows);
				return (typedRows);
			}
		}

		/// <summary>
		/// Overrides DataRow creation by reflecting constructor from
		/// type specified as T
		/// </summary>
		/// <param name="builder">
		/// Row builder <see cref="DataRowBuilder"/>
		/// </param>
		/// <returns>
		/// Reference to new subclassed DataRow <see cref="DataRow"/>
		/// </returns>
		protected override DataRow NewRowFromBuilder (DataRowBuilder builder)
		{
			cparams[0] = builder;
			DataRow res = (DataRow) cons.Invoke (cparams);
			cparams[0] = null;
			return (res);
		}
		
		/// <summary>
		/// Resolves constructor in subclassed DataRow class
		/// Constructor that has to be exposed is
		///   ctor(DataRowBuilder)
		/// </summary>
		private void InitRowActivator()
		{
			System.Type t = typeof(T);
			ConstructorInfo[] constructors = t.GetConstructors();
			foreach (ConstructorInfo ci in constructors) {
				ParameterInfo[] pinfos = ci.GetParameters();
				if (pinfos != null)
					if (pinfos.Length == 1)
						if (pinfos[0].ParameterType == typeof(DataRowBuilder)) {
							cons = ci;
							return;
						}
			}
			throw new ExceptionInheritedDataRowMissingConstructor (typeof(T));
		}
		
		public TypedDataTable()
		{
			InitRowActivator();
		}
		
		public TypedDataTable (string tableName)
			: base (tableName)
		{
			InitRowActivator();
		}
		
		public TypedDataTable (string tableName, string tbNamespace)
			: base (tableName, tbNamespace)
		{
			InitRowActivator();
		}
	}
}
