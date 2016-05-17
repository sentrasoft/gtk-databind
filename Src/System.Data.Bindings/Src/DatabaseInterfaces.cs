// DatabaseInterfaces.cs
//
// Author: m. <ml@arsis.net>
//
// Copyright (c) 2006 m.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of version 2 of the Lesser GNU General 
// Public License as published by the Free Software Foundation.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this program; if not, write to the
// Free Software Foundation, Inc., 59 Temple Place - Suite 330,
// Boston, MA 02111-1307, USA.

using System;
using System.Data;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace System.Data.Bindings.Database
{
	/// <summary>
	/// Specifies few generics which allow to transparently work with
	/// common objects for all registred databases even without knowing
	/// the specific objects used for accessing some database type
	/// </summary>
	public interface IDatabaseAccessController
	{
		/// <summary>
		/// Creates Database connection based on parameters
		/// </summary>
		/// <param name="aConnectionParams">
		/// String containing login information <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Connection object to database <see cref="DbConnection"/>
		/// </returns>
		System.Data.IDbConnection CreateDBConnection (string aConnectionParams);
		/// <summary>
		/// Creates Database connection based on parameters
		/// </summary>
		/// <param name="aConnection">
		/// Connection parameters <see cref="IDatabaseConnection"/>
		/// </param>
		/// <returns>
		/// Connection object to database <see cref="System.Data.IDbConnection"/>
		/// </returns>
		System.Data.IDbConnection CreateDBConnection (IDatabaseConnection aConnection);
		/// <summary>
		/// Translates Database connection parameters to string
		/// </summary>
		/// <param name="aConnection">
		/// Connection parameters <see cref="IDatabaseConnection"/>
		/// </param>
		/// <returns>
		/// String with connection parameters <see cref="System.String"/>
		/// </returns>
		string GetConnectionParams (IDatabaseConnection aConnection);
		/// <summary>
		/// Enumerates datasets in specified connection
		/// </summary>
		/// <param name="aConnection">
		/// Connection parameters <see cref="IDatabaseConnection"/>
		/// </param>
		/// <returns>
		/// Datasets in current connection, null if invalid connection <see cref="StringCollection"/>
		/// </returns>
		StringCollection EnumDatasets (IDatabaseConnection aConnection);
		/// <summary>
		/// Enumerates tables in specified connection
		/// </summary>
		/// <param name="aConnection">
		/// Connection parameters <see cref="IDatabaseConnection"/>
		/// </param>
		/// <returns>
		/// Tables in current connection, null if invalid connection <see cref="StringCollection"/>
		/// </returns>
		StringCollection EnumTables (IDatabaseConnection aConnection);
		/// <summary>
		/// Checks if dataset already exists
		/// </summary>
		/// <param name="aConnection">
		/// Connection parameters <see cref="IDatabaseConnection"/>
		/// </param>
		/// <param name="aDatasetName">
		/// Name of searched dataset <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// true if exists, false if not <see cref="System.Boolean"/>
		/// </returns>
		bool DatasetExists (IDatabaseConnection aConnection, string aDatasetName);
		/// <summary>
		/// Checks if table already exists
		/// </summary>
		/// <param name="aConnection">
		/// Connection parameters <see cref="IDatabaseConnection"/>
		/// </param>
		/// <param name="aTableName">
		/// Name of searched table <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// true if exists, false if not <see cref="System.Boolean"/>
		/// </returns>
		bool TableExists (IDatabaseConnection aConnection, string aTableName);
		/// <summary>
		/// Forces creation of new dataset
		/// </summary>
		/// <param name="aConnection">
		/// Connection parameters <see cref="IDatabaseConnection"/>
		/// </param>
		/// <param name="aDatasetName">
		/// Dataset name <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		/// <remarks>
		/// Throws exception if dataset already exists
		/// </remarks>
		bool ForceCreateDataset (IDatabaseConnection aConnection, string aDatasetName);
		/// <summary>
		/// Creates new dataset
		/// </summary>
		/// <param name="aConnection">
		/// Connection parameters <see cref="IDatabaseConnection"/>
		/// </param>
		/// <param name="aDatasetName">
		/// Dataset name <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// true if successful or if dataset already exists <see cref="System.Boolean"/>
		/// </returns>
		bool CreateDataset (IDatabaseConnection aConnection, string aDatasetName);
		/// <summary>
		/// Drops dataset
		/// </summary>
		/// <param name="aConnection">
		/// Connection parameters <see cref="IDatabaseConnection"/>
		/// </param>
		/// <param name="aDatasetName">
		/// Dataset name <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// true if successful <see cref="System.Boolean"/>
		/// </returns>
		/// <remarks>
		/// Throws exception if dataset doesn't exists
		/// </remarks>
		bool ForceDropDataset (IDatabaseConnection aConnection, string aDatasetName);
		/// <summary>
		/// Drops new database
		/// </summary>
		/// <param name="aConnection">
		/// Connection parameters <see cref="IDatabaseConnection"/>
		/// </param>
		/// <param name="aDatasetName">
		/// Dataset name <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// true if successful or if dataset does not exists <see cref="System.Boolean"/>
		/// </returns>
		bool DropDataset (IDatabaseConnection aConnection, string aDatasetName);
		/// <summary>
		/// Creates adaptor specific for that driver
		/// </summary>
		/// <param name="aConnection">
		/// Connection used for creation <see cref="IDatabaseConnection"/>
		/// </param>
		/// <returns>
		/// Reference to new adapter object <see cref="IDataAdapter"/>
		/// </returns>
		IDataAdapter CreateAdapter (IDatabaseConnection aConnection);
		/// <summary>
		/// Creates adaptor specific for that driver
		/// </summary>
		/// <param name="aConnection">
		/// Connection used for creation <see cref="IDatabaseConnection"/>
		/// </param>
		/// <param name="aSQLSelect">
		/// Select command <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Reference to new adapter object <see cref="IDataAdapter"/>
		/// </returns>
		IDataAdapter CreateAdapter (IDatabaseConnection aConnection, string aSQLSelect);
		/// <summary>
		/// Creates driver specific DbCommand object
		/// </summary>
		/// <param name="aConnection">
		/// Connection used to access data <see cref="IDatabaseConnection"/>
		/// </param>
		/// <param name="aCommand">
		/// Command <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Reference to command object <see cref="IDbCommand"/>
		/// </returns>
		IDbCommand CreateCommand (IDatabaseConnection aConnection, string aCommand);
	}

	/// <summary>
	/// Interface to activate connections
	/// </summary>
	public interface IDatabaseConnection
	{
		/// <value>
		/// Defines connection should be activated or not
		/// </value>
		bool Active { get; set; }
		/// <value>
		/// Defines if connection is active or not
		/// </value>
		bool IsActive { get; }
		/// <value>
		/// Connection object used to access data
		/// </value>
		System.Data.IDbConnection Connection { get; }
		/// <value>
		/// Database type which should be used to access data
		/// </value>
		string DatabaseType { get; set; }
		/// <value>
		/// Name of server
		/// </value>
		string Server { get; set; }
		/// <value>
		/// Database name
		/// </value>
		string Dataset { get; set; }
		/// <value>
		/// Username used to connect
		/// </value>
		string Username { get; set; }
		/// <value>
		/// Password used to connect
		/// </value>
		string Password { get; set; }
		/// <value>
		/// Additional connecting parameters
		/// </value>
		IVirtualObject Parameters { get; }
		/// <value>
		/// Database name if connection is active
		/// </value>
		string ConnectedDataset { get; }
		/// <value>
		/// State of database connection
		/// </value>
		ConnectionState State { get; }
		/// <value>
		/// List of tables connected to this DatabaseAccess
		/// </value>
		ITableCollection Tables { get; }
		/// <summary>
		/// Enumerates databases on server
		/// </summary>
		/// <returns>
		/// Returns names of databases <see cref="StringCollection"/>
		/// </returns>
		StringCollection EnumDatasets();
	}
	
	/// <summary>
	/// Specifies interface to access table object in more transparent manner
	/// </summary>
	public interface IDatabaseTable : IListSource
	{
		/// <value>
		/// Defines if table is active or not
		/// </value>
		bool Active { get; set; }
		/// <value>
		/// Defines if table is active or not
		/// </value>
		bool IsActive { get; }
		/// <value>
		/// Table name
		/// </value>
		string Name { get; set; }
		/// <value>
		/// Master table, it returns the owner cursors table 
		/// </value>
		IDatabaseTable MasterTable { get; }
		/// <value>
		/// Master cursor, where query belongs to
		/// </value>
		IRowCursor MasterCursor { get; }
		/// <value>
		/// Connection where table is connected to
		/// </value>
		IDatabaseConnection DatabaseConnection { get; set; }
		/// <value>
		/// Table object which is connected to this one
		/// </value>
		DataTable Table { get; }
		/// <value>
		/// Data rows in table
		/// </value>
		DataRowCollection Rows { get; }
		/// <value>
		/// Data pointers which point to specific DataRow objects
		/// </value>
		IRowCursor[] Cursors { get; }
	}

	/// <summary>
	/// Specifies interfaces to
	/// </summary>
	public interface IRowCursor
	{
		/// <value>
		/// Specifies table where pointer belongs to
		/// </value>
		IDatabaseTable OwnerTable { get; }
		/// <value>
		/// Active row where cursor is currently pointing
		/// </value>
		DataRow ActiveRow { get; set; }
		/// <summary>
		/// Moves cursor to first data row
		/// </summary>
		/// <returns>
		/// true if successful, false if not <see cref="System.Boolean"/>
		/// </returns>
		bool First();
		/// <summary>
		/// Moves cursor to next data row
		/// </summary>
		/// <returns>
		/// true if successful, false if not <see cref="System.Boolean"/>
		/// </returns>
		bool Next();
		/// <summary>
		/// Moves cursor to previous data row
		/// </summary>
		/// <returns>
		/// true if successful, false if not <see cref="System.Boolean"/>
		/// </returns>
		bool Prev();
		/// <summary>
		/// Moves cursor to last data row
		/// </summary>
		/// <returns>
		/// true if successful, false if not <see cref="System.Boolean"/>
		/// </returns>
		bool Last();
	}
	
	/// <summary>
	/// Query access to the table
	/// </summary>
	public interface IDatabaseQuery
	{
		/// <value>
		/// SQL controlling this data
		/// </value>
		string SQL { get; set; }
	}

	/// <summary>
	/// Contains collection of tables
	/// </summary>
	public interface ITableCollection : IListSource, IEnumerable
	{
		/// <summary>
		/// Returns number of tables inside of this list
		/// </summary>
		int Count { get; }
		/// <summary>
		/// Returns table at index
		/// </summary>
		/// <returns>
		/// Index <see cref="IList"/>
		/// </returns>
		IDatabaseTable this [int aIdx] { get; }
		/// <summary>
		/// Adds table to collection
		/// </summary>
		/// <param name="aTable">
		/// Table to add to collection <see cref="IDatabaseTable"/>
		/// </param>
		void Add (IDatabaseTable aTable);
		/// <summary>
		/// Adds table to collection
		/// </summary>
		/// <param name="aTable">
		/// Table to remove to collection <see cref="IDatabaseTable"/>
		/// </param>
		void Remove (IDatabaseTable aTable);
		/// <summary>
		/// Adds table to collection
		/// </summary>
		/// <param name="aTable">
		/// Table to search for in collection <see cref="IDatabaseTable"/>
		/// </param>
		/// <returns>
		/// Index of table in collection <see cref="System.Int32"/>
		/// </returns>
		int IndexOf (IDatabaseTable aTable);
		/// <summary>
		/// Adds table to collection
		/// </summary>
		/// <param name="aTable">
		/// Table to search for in collection <see cref="IDatabaseTable"/>
		/// </param>
		/// <returns>
		/// true if found, false if not <see cref="System.Int32"/>
		/// </returns>
		bool Contains (IDatabaseTable aTable);
	}	
}
