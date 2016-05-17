using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace System.Data.Bindings.Generic
{
	public static class ExtensionMethods
	{
		/// <summary>
		/// Provides enumeration for DataTable by specified delegate filter
		/// </summary>
		/// <param name="aTable">
		/// Table <see cref="DataTable"/>
		/// </param>
		/// <param name="aFilter">
		/// Delegate method <see cref="DataRowIsInFilterEvent"/>
		/// </param>
		/// <returns>
		///  <see cref="IEnumerator"/>
		/// </returns>
		public static IEnumerator FilterRows (this DataTable aTable, DataRowIsInFilterEvent aFilter)
		{
			foreach (DataRow row in aTable.Rows) {
				if (aFilter == null)
					yield return (row);
				else
					if (aFilter(row) == true)
						yield return (row);
			}
		}

		/// <summary>
		/// Provides enumeration for DataTable by specified delegate filter
		/// </summary>
		/// <param name="aView">
		/// Table <see cref="DataView"/>
		/// </param>
		/// <param name="aFilter">
		/// Delegate method <see cref="DataRowIsInFilterEvent"/>
		/// </param>
		/// <returns>
		///  <see cref="IEnumerator"/>
		/// </returns>
		public static IEnumerator FilterRows (this DataView aView, DataRowIsInFilterEvent aFilter)
		{
			foreach (DataRow row in aView.Table.Rows) {
				if (aFilter == null)
					yield return (row);
				else
					if (aFilter(row) == true)
						yield return (row);
			}
		}
		
		/// <summary>
		/// Fills list by reading records from data reader by using PropertyDescriptionAttribute
		/// </summary>
		/// <param name="IList">
		/// List where records should be loaded <see cref="aList"/>
		/// </param>
		/// <param name="aType">
		/// Class type used for loading <see cref="System.Type"/>
		/// </param>
		/// <param name="aReader">
		/// Reader <see cref="IDataReader"/>
		/// </param>
		/// <returns>
		/// Number of rows read <see cref="System.Int32"/>
		/// </returns>
		public static int LoadRowsFromReader (this IList aList, System.Type aType, IDataReader aReader)
		{
			return (aList.LoadRowsFromReader (aType, aReader, true));
		}
		
		/// <summary>
		/// Fills list by reading records from data reader by using PropertyDescriptionAttribute
		/// </summary>
		/// <param name="IList">
		/// List where records should be loaded <see cref="aList"/>
		/// </param>
		/// <param name="aType">
		/// Class type used for loading <see cref="System.Type"/>
		/// </param>
		/// <param name="aReader">
		/// Reader <see cref="IDataReader"/>
		/// </param>
		/// <param name="aUsePropertyNamesToo">
		/// If this is set true then secondary discovery used is by direct property name <see cref="System.Boolean"/>
		/// </param>
		/// <returns>
		/// Number of rows read <see cref="System.Int32"/>
		/// </returns>
		public static int LoadRowsFromReader (this IList aList, System.Type aType, IDataReader aReader, bool aUsePropertyNamesToo)
		{
			if (aReader == null)
				return (-1);
			int res = 0;
			if (aType == null)
				throw new NullReferenceException ("Specified type can't be null");
			ConstructorInfo[] constructors = aType.GetConstructors();
			ConstructorInfo cons = null;
			foreach (ConstructorInfo c in constructors)
				if (c.GetParameters().Length == 0) {
					cons = c;
					break;
				}
			
			constructors = null;
			if (cons == null)
				throw new NotImplementedException (string.Format("Type {0} needs public constructor without parameters in order to be used", aType));
			try {
				int cnt = aReader.FieldCount;
				// Create dictionary of valid fields which can be read
				Dictionary<int,PropertyInfo> lst = new Dictionary<int,PropertyInfo>();
				int pos;
				// Resolve loadable properties
				foreach (PropertyInfo pi in aType.GetProperties()) {
					PropertyDescriptionAttribute[] attrs = (PropertyDescriptionAttribute[]) pi.GetCustomAttributes (typeof(PropertyDescriptionAttribute), true);
					// PropertyDescriptionAttribute takes priority before property names in every case
					if ((attrs != null) && (attrs.Length > 0) && (attrs[0].FieldName != "")) {
						if (pi.CanWrite == false)
							throw new NotSupportedException (string.Format ("Property {0} can't be written, so specifying FieldName is considered error", pi.Name));
						pos = aReader.GetOrdinal (attrs[0].FieldName);
						if (pos > -1)
							lst.Add (pos, pi);
					}
					else if ((aUsePropertyNamesToo == true) && (pi.CanWrite == true)) {
						// Check if property name should be used
						pos = aReader.GetOrdinal (pi.Name);
						if (pos > -1)
							lst.Add (pos, pi);
					}
				}
				
				if (lst.Count == 0) {
					aReader.Close();
					return (0);
				}
				
				// Read fields by implementing reflection stored in dictionary
				while (aReader.Read() == true) {
					object o = cons.Invoke (null);
					foreach (int ord in lst.Keys)
						lst[ord].SetValue (o, aReader.GetValue(ord), null);
					aList.Add (o);
					res++;
				}
				lst.Clear();
			}
			catch {
				throw new Exception (string.Format ("Trouble reading {0} from reader", aType));
			}
			
			aReader.Close();
			return (res);
		}
	}
}
