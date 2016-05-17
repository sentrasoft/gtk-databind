
using System;
using System.Data.Bindings;

namespace Gtk.DataBindings
{
	/// <summary>
	/// Base class from which all QueryImplementors are derived
	/// </summary>
	public class QueryImplementor : CustomTreeModel, IDisconnectable
	{
		private bool hasDeletedItems = false;
		public bool HasDeletedItems {
			get { return (hasDeletedItems); }
			set { hasDeletedItems = value; }
		}
		
		private MappingsImplementor masterImplementor = null;
		/// <value>
		/// Provides reference to master implementor
		/// </value>
		public MappingsImplementor MasterImplementor {
			get { return (masterImplementor); }
		}

		/// <value>
		/// Provides global RespectHierarchy value
		/// </value>
		public bool RespectHierarchy {
			get { return (MasterImplementor.RespectHierarchy); }
		}

		/// <value>
		/// Provides datasource resolving
		/// </value>
		public object DataSource {
			get { return (MasterImplementor.ListItems); }
		}

		/// <summary>
		/// Checks if item is deleted, classes like data table support rows
		/// wit state=deleted
		/// </summary>
		/// <param name="aObject">
		/// Object <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// true if deleted, false if not <see cref="System.Boolean"/>
		/// </returns>
		public virtual bool IsItemDeleted (object aObject)
		{
			return (false);
		}
		
		/// <summary>
		/// Disconnects references
		/// </summary>
		public virtual void Disconnect()
		{
			masterImplementor = null;
		}
		
		private QueryImplementor()
		{
		}
		
		public QueryImplementor (MappingsImplementor aMasterImplementor)
		{
			if (aMasterImplementor == null)
				throw new NullReferenceException ("MasterImplementor has to be set for QueryImplementor");
			masterImplementor = aMasterImplementor;
		}
	}
}
