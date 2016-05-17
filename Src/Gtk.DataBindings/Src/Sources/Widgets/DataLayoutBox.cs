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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Bindings;

namespace Gtk.DataBindings
{
	/// <summary>
	/// 
	/// </summary>
	[ToolboxItem (true)]
	[Category ("Databound Containers")]
	public class DataLayoutBox : DataTable
	{
		private bool needsclearing = false;
		
		private GtkAdaptor columnadaptor = null; 
		/// <summary>
		/// Resolves adaptor and it simplifies the simple string
		/// </summary>
		protected IAdaptor ColumnAdaptor {
			get {
				if (columnadaptor == null)
					columnadaptor = new GtkAdaptor();
				return (columnadaptor);
			}
		}

		protected System.Type DataSourceType {
			get { 
				if (ColumnAdaptor != null)
					return (ColumnAdaptor.DataSourceType);
				return (null);
			}
		}

		private bool simpleMapping = false;
		protected bool SimpleMapping {
			get { return (simpleMapping); }
		}
		
		private string mappings = "";
		/// <summary>
		/// Link to Widget Mappings in connected Adaptor 
		/// </summary>
		[Browsable (true), Category ("Data Binding"), Description ("Widget Data mappings")]
		public string Mappings {
			get { return (mappings); }
			set { 
				if (mappings == value)
					return;
				needsclearing = true;
				_ClearTypeDescriptions();
				mappings = value;
				ColumnAdaptor.Mappings = value;
				ProcessTypeDescriptions();
			}
		}

		private void _ClearContainer (Gtk.Container aContainer)
		{
			List<Widget> widgets = new List<Widget>();
			foreach (Widget wdg in AllChildren)
				widgets.Add (wdg);
			
			foreach (Widget wdg in widgets)
				if (TypeValidator.IsCompatible(wdg.GetType(), typeof(Gtk.Container)) == true)
					_ClearContainer (wdg as Gtk.Container);
			
			foreach (Widget wdg in widgets) {
				Remove (wdg);
				if (wdg is IDisconnectable)
					(wdg as IDisconnectable).Disconnect();
				wdg.Destroy();
			}
			widgets.Clear();
			widgets = null;
		}
		
		private void _ClearTypeDescriptions()
		{
			if (needsclearing == false)
				return;
			_ClearContainer (this);
			NRows = 1;
		}
		
		private void ProcessTypeDescriptions()
		{
			int columnCnt = 0;
			// Load the DataSourceType
			MappedProperty prop = null;
			for (int i=0; i<ColumnAdaptor.MappingCount; i++) {
				prop = ColumnAdaptor.Mapping (i);
				if (prop != null)
					if (prop.IsColumnMapping == true) {
						columnCnt += 1;
						// Check if simple mapping was specified
						if ((prop.Name == "string") || (prop.Name == "bool") || (prop.Name == "int") || (prop.Name == "double"))
							simpleMapping = true;
					}
			}
			
			// If DataSource only presents simple mapping, but complex was introduced
			if (simpleMapping == true)
				throw new NotSupportedException ("Simple mapping is not supported");

			if (columnCnt > 0) {
				int j=0;
				for (uint i=0; i<ColumnAdaptor.MappingCount; i++) {
					prop = ColumnAdaptor.Mapping ((int) i);
					if (prop != null) {
						if (prop.IsColumnMapping == true) {
							
						}
						else if (prop.ColumnName == "!auto") {
							DataTitleLabel title = new DataTitleLabel (prop.Name);
							title.InheritedDataSource = true;
							Attach (title, 0, 1, i, i+1);
							IAdaptableContainer cont = null;
						}
					}
				}
			}
		}
		
		public DataLayoutBox()
			: base (1, 2, false)
		{
		}
		
		~DataLayoutBox()
		{
			columnadaptor.Disconnect();
		}
	}
}
