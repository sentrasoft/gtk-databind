using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Bindings;
using System.Data.Bindings.DebugInformation;
using System.Data.Bindings.Cached;
using System.Data.Bindings.Collections;
using System.Runtime.InteropServices;

namespace Gtk.DataBindings
{
	public class MappingsImplementor : CustomTreeModel
	{
		internal enum ModelChainType {
			Normal = 0,
			Sort = 1,
			Filter = 2
		}
				
		private bool[] modelActivity = new bool[3] {true, false, false};
		private TreeModel[] models = new TreeModel[3] {null, null, null};
		
		private bool needsclearing = false;
		private bool justcleared = false;
		private GtkListAdaptor listadaptor = null; 
		private object lastItems = null;

		#region Filtering

		protected TreeModelFilter FilterModel {
			get { return ((TreeModelFilter) models[(int) ModelChainType.Filter]); }
		}
		
		private bool showDeletedItems = true;
		public bool ShowDeletedItems {
			get { return (showDeletedItems); }
			set {
				if (showDeletedItems == value)
					return;
				bool oldfiltering = Filtering;
				showDeletedItems = value;
				if (oldfiltering != Filtering)
					CheckModelChain();
			}
		}

		private bool IsDeletedItemVisible (object aObject)
		{
			if (query == null)
				return (false);
			if ((query.HasDeletedItems == false) || ((query.HasDeletedItems == true) && (ShowDeletedItems == false)))
				return (true);
			return (query.IsItemDeleted(aObject) == false);
		}
		
		public bool Filtering {
			get { return ((isVisibleInFilter != null) || 
				          ((query != null) && (query.HasDeletedItems == true))); }
		}
		
		private event IsVisibleInFilterEvent isVisibleInFilter = null;
		/// <summary>
		/// Filtering event
		/// </summary>
		public event IsVisibleInFilterEvent IsVisibleInFilter {
			add { 
				bool wasnull = false;
				if (isVisibleInFilter == null)
					wasnull = true;
				isVisibleInFilter += value; 
				if ((wasnull == true) && (isVisibleInFilter != null))
					CheckModelChain();
			}
			remove { 
				bool wasnull = false;
				if (isVisibleInFilter == null)
					wasnull = true;
				isVisibleInFilter -= value; 
				if ((wasnull == false) && (isVisibleInFilter == null))
					CheckModelChain();
			}
		}
		
		protected bool OnIsVisibleInFilter (object aObject)
		{
			if (isVisibleInFilter != null)
				foreach (IsVisibleInFilterEvent method in isVisibleInFilter.GetInvocationList())
					if (method(aObject) == false)
						return (false);
			return (true);
		}
		
		private bool FilterTreeModel (Gtk.TreeModel aModel, Gtk.TreeIter aIter)
		{
//System.Console.WriteLine("Filter: Model {0}", aModel);
//System.Console.WriteLine("filter?" + NodeFromIter(aIter));
			if (isVisibleInFilter != null) {
				object o = NodeFromIter (aIter);
				return (OnIsVisibleInFilter (o));
			}
//System.Console.WriteLine("seems visible");
			return (true);
		}
		#endregion Filtering
		
		#region PROPERTIES
		private WeakReference owner = null;
		public Gtk.Widget Owner {
			get {
				if (owner != null)
					return (owner.Target as Gtk.Widget);
				return (null);
			}
			internal set {
				if (value == Owner)
					return;
				if (value == null)
					owner = null;
				else
					if (value is Gtk.Widget)
						owner = new WeakReference (value);
					else
						owner = null;
			}
		}

		private bool EditingIsPossible {
			get { 
				return (TypeValidator.IsCompatible(Owner.GetType(), typeof(TreeView)) == true);
			}
		}
		
		private GtkAdaptor columnadaptor = null; 
		/// <summary>
		/// Resolves adaptor and it simplifies the simple string
		/// </summary>
		private IAdaptor ColumnAdaptor {
			get {
				if (columnadaptor == null)
					columnadaptor = new GtkAdaptor();
				// Enforce string values if type is compatible so it doesn't need to be specified
				if (columnadaptor.Mappings == "")
					if (ListItems != null)
						if ((TypeValidator.IsCompatible(ListItems.GetType(), typeof(string[])) == true) ||
						    (TypeValidator.IsCompatible(ListItems.GetType(), typeof(StringCollection)) == true) ||
						    (TypeValidator.IsCompatible(ListItems.GetType(), typeof(ObserveableStringCollection)) == true))
							return (TypeValidator.StringColumnAdaptor);
				return (columnadaptor);
			}
		}

		private TreeModelAdapter modeladapter = null;
		protected TreeModelAdapter _ModelAdapter {
			get { 
				if (modeladapter == null) {
					modeladapter = new TreeModelAdapter (this);
					models[(int) ModelChainType.Normal] = modeladapter;
				}
				return (modeladapter); 
			}
		}

		private TreeModel activeModel = null;
		public TreeModel ActiveModel {
			get { return (activeModel); }
		}
		
		private bool[] multicolumns = null;
		public bool[] MultiColumns {
			get { return (multicolumns); }
		}
		
		private CachedProperty[] propertyInfos = null;
		public CachedProperty[] PropertyInfos {
			get { return (propertyInfos); }
		}
		
		private Type[] types = null;
		public Type[] Types {
			get { return (types); }
		}
		
		private string[] names = null;
		public string[] Names {
			get { return (names); }
		}
		
//		private System.Type listItemType = null;
		public System.Type ListItemType {
			get { 
				if (ColumnAdaptor != null)
					return (ColumnAdaptor.DataSourceType);
				return (null);
//				return (listItemType); 
			}
		}

		private bool simpleMapping = false;
		public bool SimpleMapping {
			get { return (simpleMapping); }
		}
		
		private bool respectHierarchy = true;
		public bool RespectHierarchy {
			get { return ((respectHierarchy == true) && (SimpleMapping == false)); }
		}
		
		private QueryImplementor query = null;
		internal QueryImplementor Query {
			get { return (query); }
		}
		
		private string mappings = "";
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

		/// <summary>
		/// DataSource object control is connected to
		/// </summary>
		[Browsable (false), Category ("Data Binding")]
		public object ItemsDataSource {
			get { return (listadaptor.Target); }
			set { 
				object check = ConnectionProvider.ResolveTargetForObject(value);
				if (check != null)
					if (ObserveableList.IsValidListStore(check) == false)
						if (ModelSelector.QueryModelExists (check.GetType()) == false)
							throw new ExceptionWrongListType (this, value);
				needsclearing = true;
				PrepareWidget();
				if (clearSelection != null)
					clearSelection();
				_ClearTypeDescriptions();
				isVisibleInFilter -= IsDeletedItemVisible;
				if (query != null)
					query.Disconnect();
				
				listadaptor.Target = value;
				if (listadaptor.FinalTarget == null)
					query = new NullTreeModel (this);
				else {
					query = ModelSelector.CreateModelFor (this, listadaptor.FinalTarget);
					if (query == null) {
						if (listadaptor.FinalTarget.GetType().IsArray == true)
							query = new ArrayQueryModel(this);
						else if (TypeValidator.IsCompatible(listadaptor.FinalTarget.GetType(), typeof(IList)) == true)
							query = new IListTreeModel(this);
						else
							throw new NotSupportedException ("Type [{0}] is not in the list of supported query implementors");
					}
				}
				// Connect visible filter
				if (query != null)
					if (query.HasDeletedItems == true)
						isVisibleInFilter += IsDeletedItemVisible;
			
				CheckModelChain();
				
				ProcessTypeDescriptions();
				if (Owner is IChangeableControl)
					(Owner as IChangeableControl).Adaptor.CheckControlState();
				check = null;
				ResetWidgetModel();
			}
		}
		
		private object cachedItems = null;
		/// <summary>
		/// Provides access to IList with items for this control
		/// </summary>
		public virtual object ListItems {
			get {
				if (cachedItems != null)
					return (cachedItems);
				cachedItems = listadaptor.FinalTarget;
				return (cachedItems);
			}
		}
		
		private NamedCellRendererList namedCellRenderers = new NamedCellRendererList();
		/// <summary>
		/// Provides a list of CellRenderers which can be accessed by name
		/// Usage of these should go trough specifying mappings of this TreeView
		/// where 
		///    Property[Column Name]&lt;&gt;CellRendererName
		/// is the rule of how and when to use them
		/// </summary>
		public NamedCellRendererList NamedCellRenderers {
			get { return (namedCellRenderers); }
		}
		
		private event ClearColumnsEvent clearColumns = null;
		public event ClearColumnsEvent ClearColumns {
			add { clearColumns += value; }
			remove { clearColumns -= value; }
		}
		
		private event ResetModelEvent resetModel = null;
		public event ResetModelEvent ResetModel {
			add { resetModel += value; }
			remove { resetModel -= value; }
		}
		
		private event CellDescriptionEvent cellDescription = null;
		public event CellDescriptionEvent CellDescription {
			add { cellDescription += value; }
			remove { cellDescription -= value; }
		}
		
		private event ListElementCellParamsEvent cellLayoutDescription = null;
		public event ListElementCellParamsEvent CellLayoutDescription {
			add { cellLayoutDescription += value; }
			remove { cellLayoutDescription -= value; }
		}
		
		internal void OnCellLayoutDescription (object aList, int[] aPath, object aObject, Gtk.CellRenderer aCell)
		{
			if (cellLayoutDescription != null)
				cellLayoutDescription (aList, aPath, aObject, aCell);
		}
		
		private event ClearSelectionEvent clearSelection = null;
		public event ClearSelectionEvent ClearSelection {
			add { clearSelection += value; }
			remove { clearSelection -= value; }
		}
		
		private event CheckControlEvent checkControl = null;
		public event CheckControlEvent CheckControl {
			add { checkControl += value; }
			remove { checkControl -= value; }
		}
		#endregion PROPERTIES

		#region WIDGET_MODEL
		private void CheckModelChain()
		{
			if (ItemsDataSource == null)
				return;
			modelActivity[(int) ModelChainType.Filter] = Filtering;
			if (models[(int) ModelChainType.Filter] == null) {
				//System.Console.WriteLine("Creating filter for {0}", _ModelAdapter);
				if (_ModelAdapter != null)
					models[(int) ModelChainType.Filter] = new TreeModelFilter (models[(int) ModelChainType.Normal], null);
				FilterModel.VisibleFunc = new TreeModelFilterVisibleFunc (FilterTreeModel);
			}
//			if (models[(int) ModelChainType.Sort] == null)
//				models[(int) ModelChainType.Sort] = new TreeModelSort (models[(int) ModelChainType.Normal]);
			if (ItemsDataSource == null)
				activeModel = null;
			else if (Filtering == true) 
				activeModel = FilterModel;
			else
				activeModel = models[(int) ModelChainType.Normal];
		}
		
/*		private void NullWidgetModel()
		{
			if (TypeValidator.IsCompatible(Owner.GetType(), typeof(TreeView)) == true)
				NullTreeViewModel();
			else if (TypeValidator.IsCompatible(Owner.GetType(), typeof(ComboBox)) == true)
				NullComboBoxModel();
			else if (TypeValidator.IsCompatible(Owner.GetType(), typeof(ComboBoxEntry)) == true)
				NullComboBoxEntryModel();
			else if (TypeValidator.IsCompatible(Owner.GetType(), typeof(IconView)) == true)
				NullIconViewModel();
			else
				throw new Exception ("Widget type " + Owner.GetType() + " is not yet handled in MappingsImplementor");
		}*/
		
		private void ResetWidgetModel()
		{
			if (TypeValidator.IsCompatible(Owner.GetType(), typeof(TreeView)) == true)
				ResetTreeViewModel();
			else if (TypeValidator.IsCompatible(Owner.GetType(), typeof(ComboBox)) == true)
				ResetComboBoxModel();
			else if (TypeValidator.IsCompatible(Owner.GetType(), typeof(ComboBoxEntry)) == true)
				ResetComboBoxEntryModel();
			else if (TypeValidator.IsCompatible(Owner.GetType(), typeof(IconView)) == true)
				ResetIconViewModel();
			else
				throw new Exception ("Widget type " + Owner.GetType() + " is not yet handled in MappingsImplementor");
		}
		
		private void ResetColumns()
		{
			if (TypeValidator.IsCompatible(Owner.GetType(), typeof(TreeView)) == true)
				ResetTreeViewColumns();
			else if (TypeValidator.IsCompatible(Owner.GetType(), typeof(ComboBox)) == true)
				ResetComboBoxColumns();
			else if (TypeValidator.IsCompatible(Owner.GetType(), typeof(ComboBoxEntry)) == true)
				ResetComboBoxEntryColumns();
			else if (TypeValidator.IsCompatible(Owner.GetType(), typeof(IconView)) == true)
				ResetIconViewColumns();
			else
				throw new Exception ("Widget type " + Owner.GetType() + " is not yet handled in MappingsImplementor");
		}
		
		private void ResetWidget()
		{
			if (TypeValidator.IsCompatible(Owner.GetType(), typeof(TreeView)) == true)
				ResetTreeView();
			else if (TypeValidator.IsCompatible(Owner.GetType(), typeof(ComboBox)) == true)
				ResetComboBox();
			else if (TypeValidator.IsCompatible(Owner.GetType(), typeof(ComboBoxEntry)) == true)
				ResetComboBoxEntry();
			else if (TypeValidator.IsCompatible(Owner.GetType(), typeof(IconView)) == true)
				ResetIconView();
			else
				throw new Exception ("Widget type " + Owner.GetType() + " is not yet handled in MappingsImplementor");
		}
		
		private void PrepareWidget()
		{
			if (TypeValidator.IsCompatible(Owner.GetType(), typeof(TreeView)) == true)
				PrepareTreeView();
			else if (TypeValidator.IsCompatible(Owner.GetType(), typeof(ComboBox)) == true)
				PrepareComboBox();
			else if (TypeValidator.IsCompatible(Owner.GetType(), typeof(ComboBoxEntry)) == true)
				PrepareComboBoxEntry();
			else if (TypeValidator.IsCompatible(Owner.GetType(), typeof(IconView)) == true)
				PrepareIconView();
			else
				throw new Exception ("Widget type " + Owner.GetType() + " is not yet handled in MappingsImplementor");
		}

		public void SetWidgetModel()
		{
			if (TypeValidator.IsCompatible(Owner.GetType(), typeof(TreeView)) == true)
				SetTreeViewModel();
			else if (TypeValidator.IsCompatible(Owner.GetType(), typeof(ComboBox)) == true)
				SetComboBoxModel();
			else if (TypeValidator.IsCompatible(Owner.GetType(), typeof(ComboBoxEntry)) == true)
				SetComboBoxEntryModel();
			else if (TypeValidator.IsCompatible(Owner.GetType(), typeof(IconView)) == true)
				SetIconViewModel();
			else
				throw new Exception ("Widget type " + Owner.GetType() + " is not yet handled in MappingsImplementor");
		}
		#endregion WIDGET_MODEL
		
		#region CELL_EDITOR_METHODS

		private static CachedProperty prp = new CachedProperty(null, "");// (Obj, item.MappedTo);

		/// <summary>
		/// Used to transfer data where multicolumn is happening
		/// </summary>
		/// <param name="aColumn">
		/// Column in question <see cref="TreeViewColumn"/>
		/// </param>
		/// <param name="aCell">
		/// CellRenderer associated with this cell <see cref="CellRenderer"/>
		/// </param>
		/// <param name="aModel">
		/// TreeModel containing this cell <see cref="TreeModel"/>
		/// </param>
		/// <param name="aIter">
		/// TreeIter associated with this cell <see cref="TreeIter"/>
		/// </param>
		private void RenderColumnFuncWithData (Gtk.TreeViewColumn aColumn, Gtk.CellRenderer aCell, 
		                                       Gtk.TreeModel aModel, Gtk.TreeIter aIter)
		{
			if (aCell is IMappedColumnItem) {
				IMappedColumnItem item = (IMappedColumnItem) aCell;
				object Obj = aModel.GetValue (aIter, item.ColumnIndex);
				// Commented out to try static cached property
//				using (CachedProperty prp = new CachedProperty (Obj, item.MappedTo)) {
					if (prp != null) {
						prp.SetObject (Obj);
						// using static cachedproperty this is needed
						prp.SetProperty (item.MappedTo);
						object currval;
						prp.GetValue (out currval, null);
						item.AssignValue (currval);
					}
					prp.Disconnect();
//				}
			}
			else
				Debug.Error ("TreeView.RenderColumnWithData()", "Didn't know what to assign data to.");
			RenderColumnFunc (aColumn, aCell, aModel, aIter);
		}
		
		/// <summary>
		/// Decides wheter some Cell should be visible or not for iterators cell
		/// </summary>
		/// <param name="aColumn">
		/// Column in question <see cref="TreeViewColumn"/>
		/// </param>
		/// <param name="aCell">
		/// CellRenderer associated with this cell <see cref="CellRenderer"/>
		/// </param>
		/// <param name="aModel">
		/// TreeModel containing this cell <see cref="TreeModel"/>
		/// </param>
		/// <param name="aIter">
		/// TreeIter associated with this cell <see cref="TreeIter"/>
		/// </param>
		private void RenderColumnFunc (Gtk.TreeViewColumn aColumn, Gtk.CellRenderer aCell, 
		                               Gtk.TreeModel aModel, Gtk.TreeIter aIter)
		{
/*			if (aCell is IMappedColumnItem) {
				IMappedColumnItem cell = (IMappedColumnItem) aCell;
				TreeIter iter = aIter;
				if (aModel == FilterModel)
					iter = FilterModel.ConvertIterToChildIter (aIter);
				object obj = NodeFromIter (iter);
				if (TypeValidator.IsCompatible(obj.GetType(), ListItemType) == false) {
					bool vis = (ConnectionProvider.ResolveMappingProperty(obj, cell.MappedTo, false) != null);
					if ((vis == false) &&
					    (DatabaseProvider.IsDataRowContainer(obj) == true))
						aCell.Visible = (TypeValidator.GetMappingType(ListItems, ListItemType, cell.MappedTo) != null);
					else
						aCell.Visible = vis;
				}
				else
					aCell.Visible = true;
				if (cellDescription != null)
					cellDescription (aColumn, obj, aCell);
				obj = null;
				cell = null;
			}
			else {
				aCell.Visible = true;
				if (cellDescription != null)
					cellDescription (aColumn, NodeFromIter(aIter), aCell);
			}*/
		}
		
		/// <summary>
		/// Generic editing function of String data
		/// </summary>
		/// <param name="o">
		/// CellRenderer in which editing happened <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// Arguments <see cref="Gtk.EditedArgs"/>
		/// </param>
		private void NumericCellEdited (object o, Gtk.EditedArgs args)
		{
			Gtk.TreeIter iter;

			if (o is IMappedColumnItem)
				if (o is MappedCellRendererText) {
					MappedCellRendererText cell = (MappedCellRendererText) o;
					
					// Resolve path as it was passed in the arguments
					Gtk.TreePath tp = new Gtk.TreePath (args.Path);
					// Change value in the original object
					if (GetIter (out iter, tp)) {
						object obj = NodeFromIter (iter);
						CachedProperty info = new CachedProperty (obj, cell.MappedTo);
						if (info != null) 
							if (info.CanWrite == true) {
								try {
									object newval = System.Convert.ChangeType (args.NewText, info.PropertyType);
									info.SetValue (newval);
/*									if (TypeValidator.IsNumeric(types[cell.ColumnIndex]) == true)
										if (TypeValidator.IsFloat(types[cell.ColumnIndex]) == true)
											SetValue (iter, cell.ColumnIndex, System.Convert.ToDouble(newval));
										else
											SetValue (iter, cell.ColumnIndex, System.Convert.ToInt32(newval));*/
								}
								catch (System.InvalidCastException) {
									Debug.DevelInfo ("DataTreeView.NumericCellEdited()", "Trouble Converting Value");
								}
								catch (System.OverflowException) {
									Debug.DevelInfo ("DataTreeView.NumericCellEdited()", "Trouble Converting Value");
								}
								DataSourceController.GetRequest (obj);
							}
						info.Disconnect();
						info = null;
					}
					cell = null;
					tp.Dispose();
				}
		}
		
		/// <summary>
		/// Generic editing function of Text data
		/// </summary>
		/// <param name="o">
		/// CellRenderer in which editing happened <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// Arguments <see cref="Gtk.EditedArgs"/>
		/// </param>
		private void TextCellEdited (object o, Gtk.EditedArgs args)
		{
			Gtk.TreeIter iter;

			if (o is IMappedColumnItem)
				if (o is MappedCellRendererText) {
					MappedCellRendererText cell = (MappedCellRendererText) o;
					
					// Resolve path as it was passed in the arguments
					Gtk.TreePath tp = new Gtk.TreePath (args.Path);
					// Change value in the original object
					if (GetIter (out iter, tp)) {
						object obj = NodeFromIter(iter);
						CachedProperty info = new CachedProperty (obj, cell.MappedTo);
						if (info != null) 
							if (info.CanWrite == true) {
//								SetValue (iter, cell.ColumnIndex, args.NewText);
								info.SetValue (args.NewText);
								DataSourceController.GetRequest (obj);
							}
						info.Disconnect();
						info = null;
					}
					cell = null;
					tp.Dispose();
				}
		}
	
		/// <summary>
		/// Generic editing function of Bool data
		/// </summary>
		/// <param name="o">
		/// CellRenderer in which editing happened <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// Arguments <see cref="Gtk.ToggledArgs"/>
		/// </param>
		private void BoolCellToggled (object o, Gtk.ToggledArgs args)
		{
			Gtk.TreeIter iter;

			if (o is IMappedColumnItem)
				if (o is MappedCellRendererToggle) {
					MappedCellRendererToggle cell = (MappedCellRendererToggle) o;
					
					// Resolve path as it was passed in the arguments
					Gtk.TreePath tp = new Gtk.TreePath (args.Path);
					// Change value in the original object
					if (GetIter (out iter, tp)) {
						object obj = NodeFromIter(iter);
						CachedProperty info = new CachedProperty (obj, cell.MappedTo);
						if (info != null) 
							if (info.CanWrite == true) {
								bool old = false;
								object v;
								info.GetValue (out v, null);
								try {
									old = System.Convert.ToBoolean (v);
								}
								catch (System.ArgumentNullException) {
									old = false;
								}
								catch (System.InvalidCastException) {
									old = false;
								}
								info.SetValue (!old);
//								SetValue (iter, cell.ColumnIndex, !old);
								DataSourceController.GetRequest (obj);
							}
						info.Disconnect();
						info = null;
					}
					cell = null;
					tp.Dispose();
				}
		}
		
		/// <summary>
		/// Checks if editing is possible on this item
		/// </summary>
		/// <param name="o">
		/// CellRenderer in which editing happened <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// Arguments <see cref="Gtk.EditingStartedArgs"/>
		/// </param>
		private void CellOnEditingStarted (object o, Gtk.EditingStartedArgs args)
		{
			// Resolve path as it was passed in the arguments
			Gtk.TreeIter iter;
			Gtk.TreePath tp = new Gtk.TreePath (args.Path);
			CellRenderer cellr = (CellRenderer) o;
			if (o is IMappedColumnItem) {
				IMappedColumnItem cell = (IMappedColumnItem) o;
				
				if (GetIter (out iter, tp)) {
					object obj = NodeFromIter(iter);
					CachedProperty info = new CachedProperty (obj, cell.MappedTo);
					if (info == null) {
						cellr.CancelEditing();
						cellr.StopEditing(true);
						args.RetVal = 0;
					}
					info.Disconnect();
					info = null;
				}
				else
					cellr.CancelEditing();
			
				cell = null;
			}
			else
				cellr.CancelEditing();
			tp.Dispose();
			cellr = null;
		}
		#endregion CELL_EDITOR_METHODS

		#region TREEVIEW_WIDGET_SPECIFICS
		private void ResetTreeViewColumns()
		{
			TreeView tv = (Owner as TreeView);
			while (tv.Columns.Length > 0)
				tv.RemoveColumn(tv.Columns[0]);
		}
		
/*		private void NullTreeViewModel()
		{
			TreeView tv = (Owner as TreeView);
			tv.Model = null;
			modeladapter = null;
		}*/
		
		private void ResetTreeViewModel()
		{
			TreeView tv = (Owner as TreeView);
			tv.Model = null;
			tv.Model = ActiveModel;
		}
		
		private void ResetTreeView()
		{
			TreeView tv = (Owner as TreeView);
			tv.Model = null;
			while (tv.Columns.Length > 0)
				tv.RemoveColumn(tv.Columns[0]);
		}
		
		private void PrepareTreeView()
		{
			TreeView tv = (Owner as TreeView);
			tv.Model = null;
			while (tv.Columns.Length > 0)
				tv.RemoveColumn(tv.Columns[0]);
		}
		
		private void SetTreeViewModel()
		{
			TreeView tv = (Owner as TreeView);
			tv.Model = ActiveModel;
		}
		
		/// <summary>
		/// Creates new column if needed and cell renderer according to the specified needs in mappings
		/// </summary>
		/// <param name="aProp">
		/// Mapped property for which column is being created <see cref="MappedProperty"/>
		/// </param>
		/// <param name="aType">
		/// Type of column <see cref="System.Type"/>
		/// </param>
		/// <param name="aColumnIndex">
		/// Column index <see cref="System.Int32"/>
		/// </param>
		/// <param name="aSubColumnIndex">
		/// Subcolumn index <see cref="System.Int32"/>
		/// </param>
		/// <param name="aItemGroup">
		/// true if this is group item <see cref="System.Boolean"/>
		/// </param>
		private void TreeViewCreateColumnForMapping (MappedProperty aProp, System.Type aType, int aColumnIndex, int aSubColumnIndex, bool aItemGroup)
		{
			TreeView wdg = (Owner as TreeView);
			
			TreeViewColumn tv = null;
			CellRenderer cell = null;
			
			// Column is already created
			if (aSubColumnIndex != -1)
				tv = wdg.Columns [aColumnIndex];
			
			int colnr = aSubColumnIndex;
			if ((aSubColumnIndex != -1) && (aItemGroup == true))
				colnr = 0;
			// Add CustomCellRenderer selector if specified
			if (aProp.MappingTarget != "") {
				bool succ = false;
//				System.Console.WriteLine("Property={0} Mapping={1}", aProp.Name, aProp.MappingTarget);
				NamedCellRendererList.NamedCellRenderer cr = NamedCellRenderers[aProp.MappingTarget];
				if (cr != null) {
					cell = cr.Renderer;
					if (aSubColumnIndex == -1)
						if (aItemGroup == false)
							tv = new TreeViewColumn(aProp.ColumnName, cr.Renderer, cr.Arg, wdg.Columns.Length);
						else {
							tv = new TreeViewColumn();
							wdg.AppendColumn (tv);
							tv.Title = aProp.ColumnName;
							tv.PackStart (cell, true);
						}
					else
						tv.PackStart (cell, true);
					succ = true;
				}
				else {
					System.Console.WriteLine("Factory");
					GtkFactoryInvocationArgs args = new GtkFactoryInvocationArgs (PropertyDefinition.ReadOnly, ListItemType, aProp.Name);
					args.HandlerOverride = aProp.MappingTarget;
//					System.Console.WriteLine("{0} {1}", args.Description.HandlerType, args.Description.DataTypeHandler);
					IMappedColumnItem item = GtkWidgetFactory.CreateCell (args);
					System.Console.WriteLine(item);
					if (item != null) {
						cell = (CellRenderer) item;
						System.Console.WriteLine("Cell: {0}", cell.GetType());
						if (aSubColumnIndex == -1)
							if (aItemGroup == false)
								tv = new TreeViewColumn(aProp.ColumnName, cell, cell.ResolveDataProperty(), wdg.Columns.Length);
							else {
								tv = new TreeViewColumn();
								wdg.AppendColumn (tv);
								tv.Title = aProp.ColumnName;
								tv.PackStart (cell, true);
							}
						else
							tv.PackStart (cell, true);
						succ = true;
					}
				}
				if (succ == false)
					throw new ExceptionSpecialCellRendererNotFound (aProp.MappingTarget);
				cr = null;
			}
			else
				if (aType == typeof (bool)) {
					MappedCellRendererToggle tgl = new MappedCellRendererToggle();
					cell = tgl;
					if (aSubColumnIndex == -1)
						if (aItemGroup == false)
							tv = new TreeViewColumn(aProp.ColumnName, tgl, "active", wdg.Columns.Length);
						else {
							tv = new TreeViewColumn();
							wdg.AppendColumn (tv);
							tv.Title = aProp.ColumnName;
							tv.PackStart (cell, true);
						}
					else
						tv.PackStart (cell, true);

					if ((aProp.OriginalRWFlags == EReadWrite.ReadWrite) && (EditingIsPossible == true)) {
						tgl.Activatable = true;
						tgl.Toggled += BoolCellToggled;
						tgl.EditingStarted += CellOnEditingStarted;
					}
					tgl = null;
				}
				else
					if (aType == typeof (Gdk.Pixbuf)) {
						MappedCellRendererPixbuf pix = new MappedCellRendererPixbuf();
						cell = pix;
						if (aSubColumnIndex == -1)
							if (aItemGroup == false)
								tv = new TreeViewColumn(aProp.ColumnName, pix, "pixbuf", wdg.Columns.Length);
							else {
								tv = new TreeViewColumn();
								wdg.AppendColumn (tv);
								tv.Title = aProp.ColumnName;
								tv.PackStart (cell, true);
							}
						else
							tv.PackStart (cell, true);
					}
					else
						if (TypeValidator.IsNumeric(aType) == true) {
							MappedCellRendererText rndr = new MappedCellRendererText();
							cell = rndr;
							rndr.Xalign = 1;
							if (aSubColumnIndex == -1)
								if (aItemGroup == false)
									tv = new TreeViewColumn(aProp.ColumnName, rndr, "text", wdg.Columns.Length);
								else {
									tv = new TreeViewColumn();
									wdg.AppendColumn (tv);
									tv.Title = aProp.ColumnName;
									tv.PackStart (cell, true);
								}
							else
								tv.AddAttribute (cell, "text", aColumnIndex);

							if ((aProp.OriginalRWFlags == EReadWrite.ReadWrite) && (EditingIsPossible == true)) {
								rndr.Editable = true;
								rndr.Edited += NumericCellEdited;
								rndr.EditingStarted += CellOnEditingStarted;
							}
							rndr = null;
						}
						else {
							MappedCellRendererText txt = new MappedCellRendererText();
							cell = txt;
							if (aSubColumnIndex == -1)
								if (aItemGroup == false) 
									tv = new TreeViewColumn(aProp.ColumnName, txt, "text", wdg.Columns.Length);
								else {
									tv = new TreeViewColumn();
									wdg.AppendColumn (tv);
									tv.Title = aProp.ColumnName;
									tv.PackStart (cell, true);
								}
							else
								tv.PackStart (cell, true);

							if ((aProp.OriginalRWFlags == EReadWrite.ReadWrite) && (EditingIsPossible == true)) {
								txt.Editable = true;
								txt.Edited += TextCellEdited;
								txt.EditingStarted += CellOnEditingStarted;
							}
						}
			if (cell != null)
				if (cell is IMappedColumnItem) {
					IMappedColumnItem icell = (cell as IMappedColumnItem);
					icell.MappedType = aType;
					icell.ColumnIndex = aColumnIndex;
					icell.SubColumnIndex = colnr;
					if (SimpleMapping == true)
						(cell as IMappedItem).MappedTo = "";
					else {
					System.Console.WriteLine("Set data func");
						if (tv != null)
							if (aItemGroup == true)
								// Istead of AddAttribute this is where cell insertion happens
								tv.SetCellDataFunc (cell, new Gtk.TreeCellDataFunc (RenderColumnFuncWithData));
							else
								tv.SetCellDataFunc (cell, new Gtk.TreeCellDataFunc (RenderColumnFunc));
						(cell as IMappedItem).MappedTo = aProp.Name;
					}
					icell = null;
				}
			
			if (aSubColumnIndex == -1)
				if (tv != null)
					wdg.AppendColumn (tv);
			tv = null;
		}
		
		private void CreateColumnForMapping (MappedProperty aProp, System.Type aType, int aColumnIndex, int aSubColumnIndex, bool aItemGroup)
		{
			if (Owner is TreeView)
				TreeViewCreateColumnForMapping (aProp, aType, aColumnIndex, aSubColumnIndex, aItemGroup);
			else
				throw new NotImplementedException ("Widget type " + Owner.GetType() + " is not yet handled in MappingsImplementor");
		}
		#endregion TREEVIEW_WIDGET_SPECIFICS

		#region ICONVIEW_WIDGET_SPECIFICS
		private void ResetIconViewColumns()
		{
			(Owner as IconView).Clear();
		}
		
/*		private void NullIconViewModel()
		{
			IconView tv = (Owner as IconView);
			tv.Model = null;
			modeladapter = null;
		}*/
		
		private void ResetIconViewModel()
		{
			IconView tv = (Owner as IconView);
			tv.Model = null;
			tv.Model = ActiveModel;
		}
		
		private void ResetIconView()
		{
			IconView tv = (Owner as IconView);
			tv.Model = null;
			ResetComboBoxColumns();
		}
		
		private void PrepareIconView()
		{
			IconView tv = (Owner as IconView);
			tv.Model = null;
			ResetComboBoxColumns();
		}
		
		private void SetIconViewModel()
		{
			(Owner as IconView).Model = ActiveModel;
		}
		
		/// <summary>
		/// Decides wheter some Cell should be visible or not for iterators cell
		/// </summary>
		/// <param name="aCellLayout">
		/// CellLayout in question <see cref="CellLayout"/>
		/// </param>
		/// <param name="aCell">
		/// CellRenderer associated with this cell <see cref="CellRenderer"/>
		/// </param>
		/// <param name="aModel">
		/// TreeModel containing this cell <see cref="TreeModel"/>
		/// </param>
		/// <param name="aIter">
		/// TreeIter associated with this cell <see cref="TreeIter"/>
		/// </param>
		private void RenderIconViewFunc (CellLayout aCellLayout, CellRenderer aCell, TreeModel aModel, TreeIter aIter)
		{
			TreePath tp = aModel.GetPath (aIter);
			object obj = NodeFromIter (aIter);
			if (aCell is IMappedColumnItem) {
				IMappedColumnItem cell = (IMappedColumnItem) aCell;
//				object obj = (Owner as DataComboBox).GetCurrentObject();
				              //HierarchicalList.Get (ListItems, tp.Indices);
				if (TypeValidator.IsCompatible(obj.GetType(), ListItemType) == false) {
					aCell.Visible = (ConnectionProvider.ResolveMappingProperty(obj, cell.MappedTo, false) != null);
					if ((aCell.Visible == false) &&
					    (DatabaseProvider.IsDataRowContainer(obj) == true))
						aCell.Visible = (TypeValidator.GetMappingType(ListItems, ListItemType, cell.MappedTo) != null);
				}
				else
					aCell.Visible = true;
				//(Owner as DataIconView).
				OnCellLayoutDescription (ListItems, tp.Indices, obj, aCell);
				obj = null;
			}
			else {
				aCell.Visible = true;
				//(Owner as DataIconView).
				OnCellLayoutDescription (ListItems, tp.Indices, obj, aCell);
			}
			tp.Dispose();
		}
		#endregion ICONVIEW_WIDGET_SPECIFICS

		#region COMBOBOX_WIDGET_SPECIFICS
		private void ResetComboBoxColumns()
		{
			(Owner as ComboBox).Clear();
		}
		
/*		private void NullComboBoxModel()
		{
			ComboBox tv = (Owner as ComboBox);
			tv.Model = null;
			modeladapter = null;
		}*/
		
		private void ResetComboBoxModel()
		{
			ComboBox tv = (Owner as ComboBox);
			tv.Model = null;
			tv.Model = ActiveModel;
		}
		
		private void ResetComboBox()
		{
			ComboBox tv = (Owner as ComboBox);
			tv.Model = null;
			ResetComboBoxColumns();
		}
		
		private void PrepareComboBox()
		{
			ComboBox tv = (Owner as ComboBox);
			tv.Model = null;
			ResetComboBoxColumns();
		}
		
		private void SetComboBoxModel()
		{
			(Owner as ComboBox).Model = ActiveModel;
		}
		
		/// <summary>
		/// Decides wheter some Cell should be visible or not for iterators cell
		/// </summary>
		/// <param name="aCellLayout">
		/// CellLayout in question <see cref="CellLayout"/>
		/// </param>
		/// <param name="aCell">
		/// CellRenderer associated with this cell <see cref="CellRenderer"/>
		/// </param>
		/// <param name="aModel">
		/// TreeModel containing this cell <see cref="TreeModel"/>
		/// </param>
		/// <param name="aIter">
		/// TreeIter associated with this cell <see cref="TreeIter"/>
		/// </param>
		private void RenderComboCellFunc (CellLayout aCellLayout, CellRenderer aCell, TreeModel aModel, TreeIter aIter)
		{
			TreePath tp = aModel.GetPath (aIter);
			object obj = NodeFromIter (aIter);
			if (aCell is IMappedColumnItem) {
				IMappedColumnItem cell = (IMappedColumnItem) aCell;
//				object obj = (Owner as DataComboBox).GetCurrentObject();
				              //HierarchicalList.Get (ListItems, tp.Indices);
				if (TypeValidator.IsCompatible(obj.GetType(), ListItemType) == false) {
					aCell.Visible = (ConnectionProvider.ResolveMappingProperty(obj, cell.MappedTo, false) != null);
					if ((aCell.Visible == false) &&
					    (DatabaseProvider.IsDataRowContainer(obj) == true))
						aCell.Visible = (TypeValidator.GetMappingType(ListItems, ListItemType, cell.MappedTo) != null);
				}
				else
					aCell.Visible = true;
/*				if (TypeValidator.IsCompatible(Owner.GetType(), typeof(DataComboBox)) == true)
					(Owner as DataComboBox).OnCellLayoutDescription (ListItems, tp.Indices, obj, aCell);
				else
					(Owner as DataComboBoxEntry).*/
				OnCellLayoutDescription (ListItems, tp.Indices, obj, aCell);
					
				obj = null;
			}
			else {
				aCell.Visible = true;
				/*if (TypeValidator.IsCompatible(Owner.GetType(), typeof(DataComboBox)) == true)
					(Owner as DataComboBox).OnCellLayoutDescription (ListItems, tp.Indices, obj, aCell);
				else
					(Owner as DataComboBoxEntry).*/
				OnCellLayoutDescription (ListItems, tp.Indices, obj, aCell);
			}
			tp.Dispose();
		}
		#endregion COMBOBOX_WIDGET_SPECIFICS

		#region COMBOBOXENTRY_WIDGET_SPECIFICS
		private void ResetComboBoxEntryColumns()
		{
			(Owner as ComboBoxEntry).Clear();
		}
		
/*		private void NullComboBoxEntryModel()
		{
			ComboBoxEntry tv = (Owner as ComboBoxEntry);
			tv.Model = null;
			modeladapter = null;
		}*/
		
		private void ResetComboBoxEntryModel()
		{
			ComboBoxEntry tv = (Owner as ComboBoxEntry);
			tv.Model = null;
			tv.Model = ActiveModel;
		}
		
		private void ResetComboBoxEntry()
		{
			ComboBoxEntry tv = (Owner as ComboBoxEntry);
			tv.Model = null;
			ResetComboBoxEntryColumns();
		}
		
		private void PrepareComboBoxEntry()
		{
			ComboBoxEntry tv = (Owner as ComboBoxEntry);
			tv.Model = null;
			ResetComboBoxEntryColumns();
		}
		
		private void SetComboBoxEntryModel()
		{
			(Owner as ComboBoxEntry).Model = ActiveModel;
		}
		
		/// <summary>
		/// Decides wheter some Cell should be visible or not for iterators cell
		/// </summary>
		/// <param name="aCellLayout">
		/// CellLayout in question <see cref="CellLayout"/>
		/// </param>
		/// <param name="aCell">
		/// CellRenderer associated with this cell <see cref="CellRenderer"/>
		/// </param>
		/// <param name="aModel">
		/// TreeModel containing this cell <see cref="TreeModel"/>
		/// </param>
		/// <param name="aIter">
		/// TreeIter associated with this cell <see cref="TreeIter"/>
		/// </param>
		private void RenderComboEntryCellFunc (CellLayout aCellLayout, CellRenderer aCell, TreeModel aModel, TreeIter aIter)
		{
			TreePath tp = aModel.GetPath (aIter);
			object obj = NodeFromIter (aIter);
			if (aCell is IMappedColumnItem) {
				IMappedColumnItem cell = (IMappedColumnItem) aCell;
//				object obj = (Owner as DataComboBox).GetCurrentObject();
				              //HierarchicalList.Get (ListItems, tp.Indices);
				if (TypeValidator.IsCompatible(obj.GetType(), ListItemType) == false) {
					aCell.Visible = (ConnectionProvider.ResolveMappingProperty(obj, cell.MappedTo, false) != null);
					if ((aCell.Visible == false) &&
					    (DatabaseProvider.IsDataRowContainer(obj) == true))
						aCell.Visible = (TypeValidator.GetMappingType(ListItems, ListItemType, cell.MappedTo) != null);
				}
				else
					aCell.Visible = true;
				//(Owner as DataComboBoxEntry).
				OnCellLayoutDescription (ListItems, tp.Indices, obj, aCell);
				obj = null;
			}
			else {
				aCell.Visible = true;
				//(Owner as DataComboBoxEntry).
				OnCellLayoutDescription (ListItems, tp.Indices, obj, aCell);
			}
			tp.Dispose();
		}
		#endregion COMBOBOXENTRY_WIDGET_SPECIFICS

		#region TYPE_PROCESSING
		internal void _ClearTypeDescriptions()
		{
			if (justcleared == true)
				return;
			justcleared = true;
			needsclearing = false;
//			ResetWidget();
			multicolumns = null;
			if (PropertyInfos != null) {
				foreach (CachedProperty prop in PropertyInfos)
					prop.Disconnect();
				for (int i = 0; i<PropertyInfos.Length; i++)
					PropertyInfos[i] = null;
				propertyInfos = null;
			}
			types = null;
//			listItemType = null;
			simpleMapping = false;
			names = null;
			// ClearModel
			ResetColumns();
			if (clearColumns != null)
				clearColumns();
		}
		
		/// <summary>
		/// Introducing new mapping columns for TreeView
		/// </summary>
		public bool RemapComboBox (bool aAsEntry)
		{
			bool enforcedsimpleapping = false;
			
			names = null;
			types = null;
			propertyInfos = null;
			if (ListItems == null)
				return (false);
				
			// Check if simple mapping should be enforced
			if ((ListItems is ObserveableStringCollection) || (ListItems is StringCollection))
				enforcedsimpleapping = true;
			if (ListItems is string[])
				enforcedsimpleapping = true;
			int columnCnt = 0;
			simpleMapping = false;

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
				
			if ((ListItemType != null) || (simpleMapping == true)) {
				// If DataSource only presents simple mapping, but complex was introduced
				if ((enforcedsimpleapping == true) && (simpleMapping == false))
					throw new ExceptionDataSourceSimpleMappingOnly (GetType());
					
				if (columnCnt > 0) {
					names = new string [columnCnt];
					types = new Type [columnCnt];
					// Throw error if simple mapping was introduced but more than one mapping
					if ((columnCnt > 1) && (simpleMapping == true))
						throw new ExceptionSimpleTypeUsedWithMultiColumn (GetType());
					propertyInfos = new CachedProperty [columnCnt];
					int j=0;
					for (int i=0; i<ColumnAdaptor.MappingCount; i++) {
						prop = ColumnAdaptor.Mapping (i);
						if (prop != null)
							if (prop.IsColumnMapping == true) {
								// if simple mapping was specified then resolving has to godifferent way
								if (simpleMapping == false) {
									Type res = (Type) null;
									if (ListItemType != null)
										res = TypeValidator.GetMappingType (ListItems, ListItemType, prop.Name);
									else
										throw new ExceptionDataSourceTypeWasNotSpecified (GetType(), ColumnAdaptor);
									CachedProperty prp = (CachedProperty) null;
									if (ListItemType != null)
										prp = new CachedProperty (ListItemType, prop.Name);

									names[j] = prop.Name;
									if (TypeValidator.IsNumeric(res) == true)
										if (TypeValidator.IsFloat(res) == true)
											types[j] = typeof (double);
										else
											types[j] = typeof (int);
									else
										types[j] = res;
									propertyInfos[j] = prp;
									prp = (CachedProperty) null;
									res = (Type) null;
								}
								else {
									// simple mapping resolving
									propertyInfos[j] = null;
									names[j] = "";
									switch (prop.Name) {
										case "string" :
											types[j] = typeof (string);
											break;
										case "bool" :
											types[j] = typeof (bool);
											break;
										case "int" :
											types[j] = typeof (int);
											break;
										case "double" :
											types[j] = typeof (double);
											break;
									}
								}

								CellRenderer cell = null;
								
								if (prop.HasSubmappings == true)
									throw new ExceptionColumnSubItemsNotSupported (GetType());
									
								// Add customCellRenderer selector if specified
								if (prop.MappingTarget != "") {
									NamedCellRendererList.NamedCellRenderer cr = NamedCellRenderers[prop.MappingTarget];
									if (cr != null) {
										cell = cr.Renderer;
										if (aAsEntry == false) {
											(Owner as ComboBox).PackStart (cr.Renderer, true);
											(Owner as ComboBox).AddAttribute (cr.Renderer, cr.Arg, j);
										}
										else {
											(Owner as ComboBoxEntry).PackStart (cr.Renderer, true);
											(Owner as ComboBoxEntry).AddAttribute (cr.Renderer, cr.Arg, j);
										}
									}
									else 
										throw new ExceptionSpecialCellRendererNotFound (prop.MappingTarget);
									cr = null;
								}
								else
									if (types[j] == typeof(bool)) {
										MappedCellRendererToggle tgl = new MappedCellRendererToggle();
										cell = tgl;
										if (aAsEntry == false) {
											(Owner as ComboBox).PackStart (cell, true);
											(Owner as ComboBox).AddAttribute (tgl, "active", j);
										}
										else {
											(Owner as ComboBox).PackStart (cell, true);
											(Owner as ComboBox).AddAttribute (tgl, "active", j);
										}
										tgl = null;
									}
									else
										if (types[j] == typeof (Gdk.Pixbuf)) {
											MappedCellRendererPixbuf pix = new MappedCellRendererPixbuf();
											cell = pix;
											if (aAsEntry == false) {
												(Owner as ComboBox).PackStart (cell, true);
												(Owner as ComboBox).AddAttribute (pix, "pixbuf", j);
											}
											else {
												(Owner as ComboBoxEntry).PackStart (cell, true);
												(Owner as ComboBoxEntry).AddAttribute (pix, "pixbuf", j);
											}
										}
										else
											if (TypeValidator.IsNumeric(types[j]) == true) {
												MappedCellRendererText rndr = new MappedCellRendererText();
												cell = rndr;
												rndr.Xalign = 1;
												if (aAsEntry == false) {
													(Owner as ComboBox).PackStart (cell, true);
													(Owner as ComboBox).AddAttribute (rndr, "text", j);
												}
												else {
													(Owner as ComboBoxEntry).PackStart (cell, true);
													(Owner as ComboBoxEntry).AddAttribute (rndr, "text", j);
												}
												rndr = null;
											}
											else {
												MappedCellRendererText txt = new MappedCellRendererText();
												cell = txt;
												if (aAsEntry == false) {
													(Owner as ComboBox).PackStart (cell, true);
													(Owner as ComboBox).AddAttribute (txt, "text", j);
												}
												else {
													(Owner as ComboBoxEntry).PackStart (cell, true);
													(Owner as ComboBoxEntry).AddAttribute (txt, "text", j);
												}
											}
								if (cell != null) 
									if (cell is IMappedColumnItem) {
										if (simpleMapping == true)
											(cell as IMappedItem).MappedTo = "";
										else {
											(cell as IMappedItem).MappedTo = prop.Name;
											if (aAsEntry == false)
												(Owner as ComboBox).SetCellDataFunc (cell, new CellLayoutDataFunc (RenderComboCellFunc));
											else
												(Owner as ComboBoxEntry).SetCellDataFunc (cell, new CellLayoutDataFunc (RenderComboEntryCellFunc));
										}
										(cell as IMappedColumnItem).MappedTo = prop.Name;
										(cell as IMappedColumnItem).MappedType = types[j];
										(cell as IMappedColumnItem).ColumnIndex = j;
									}
								j++;
							}
					}

					if (modeladapter == null) {
						modeladapter = new TreeModelAdapter (this);
						if (ListItems != null)
							DSChanged (ListItems);
					}
				}
			}
			return (true);
		}
		
		/// <summary>
		/// Introducing new mapping columns for TreeView
		/// </summary>
		private bool RemapTreeView()
		{
			bool enforcedsimpleapping = false;
			
			if (ListItems == null)
				return (false);
			if ((ListItems is ObserveableStringCollection) || (ListItems is StringCollection))
				enforcedsimpleapping = true;

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
			if ((enforcedsimpleapping == true) && (simpleMapping == false))
				throw new ExceptionDataSourceSimpleMappingOnly (GetType());

			if ((ListItemType != null) || (simpleMapping == true)) {
				if (columnCnt > 0) {
					names = new string [columnCnt];
					types = new Type [columnCnt];
					multicolumns = new bool [columnCnt];
					// Throw error if simple mapping was introduced but more than one mapping
					if ((columnCnt > 1) && (simpleMapping == true))
						throw new ExceptionSimpleTypeUsedWithMultiColumn (GetType());
					propertyInfos = new CachedProperty [columnCnt];
					int j=0;
					for (int i=0; i<ColumnAdaptor.MappingCount; i++) {
						prop = ColumnAdaptor.Mapping (i);
						if (prop != null)
							if (prop.IsColumnMapping == true) {
								multicolumns[j] = false;
								// if simple mapping was specified then resolving has to godifferent way
								System.Type originalType;
								if (simpleMapping == false) {
									Type res = (Type) null;
									if (ListItemType != null)
										res = TypeValidator.GetMappingType (ListItems, ListItemType, prop.Name);
									else
										throw new ExceptionDataSourceTypeWasNotSpecified (GetType(), ColumnAdaptor);
									CachedProperty prp = (CachedProperty) null;
									if (ListItemType != null)
										prp = new CachedProperty (ListItemType, prop.Name);

									names[j] = prop.Name;
									if (TypeValidator.IsNumeric(res) == true)
										if (TypeValidator.IsFloat(res) == true)
											types[j] = typeof (double);
										else
											types[j] = typeof (int);
									else
										types[j] = res;
										
									originalType = types[j];
									// If this property packs more than one mapping
									// object will be ppassed instead direct type and 
									// resolved in cell assigning option
									if (prop.HasSubmappings == true) {
										multicolumns[j] = true;
										types[j] = typeof (object);
									}
										
									propertyInfos[j] = prp;
									prp = (CachedProperty) null;
									res = (Type) null;
								}
								else {
									// simple mapping resolving
									propertyInfos[j] = null;
									names[j] = "";
									switch (prop.Name) {
										case "string" :
											types[j] = typeof (string);
											break;
										case "bool" :
											types[j] = typeof (bool);
											break;
										case "int" :
											types[j] = typeof (int);
											break;
										case "double" :
											types[j] = typeof (double);
											break;
									}
									originalType = types[j];
								}
								
								multicolumns[j] = ((prop.HasSubmappings == true) && (simpleMapping == false) && (enforcedsimpleapping == false));
								// Create Cell renderer for this mapping
								int cnt=prop.Submappings.Count;
								CreateColumnForMapping (prop, originalType, j, -1, multicolumns[j]);
								if ((multicolumns[j] == true) && (prop.Submappings != null)) {
									for (int spi=0; spi<cnt; spi++) {
										MappedProperty sprop = prop.Submappings[spi];
										if (sprop != null) {
											System.Type sres = TypeValidator.GetMappingType (ListItems, ListItemType, sprop.Name);
											CreateColumnForMapping (sprop, sres, j, spi+1, true);
											sprop = null;
										}
									}
								}
								else
									if (prop.Submappings.Count > 0)
										Debug.DevelInfo ("DataTreeView.RemapControl()", "ERROR");

								j++;
							}
					}

					if (resetModel != null)
						resetModel();
//					if (internalTreeStore == null) {
//						internalTreeStore = new AdaptableTreeStore(types);
//					if (internalModel == null) {
//						internalModel = new AdaptableTreeStore(types);
//						if (ListItems != null)
//							DSChanged (ListItems);
/*						if (listadaptor.Target is ObserveableList)
							Reorderable = (listadaptor.Target as ObserveableList).IsReorderable;
						if ((Reorderable == true) || ((ListItems is IObserveableList) && ((ListItems as ObserveableList).GUIDragDrop == true))) {
							UnsetDragFunctionality();
							SetDragFunctionality();
						}
						else
							UnsetDragFunctionality();*/
//					}
				}
			}
			return (true);
		}

		private bool RemapControl()
		{
			_ClearTypeDescriptions();
			justcleared = false;
			if (TypeValidator.IsCompatible(Owner.GetType(), typeof(TreeView)) == true)
				return (RemapTreeView());
			else if (TypeValidator.IsCompatible(Owner.GetType(), typeof(ComboBox)) == true)
				return (RemapComboBox(false));
			else if (TypeValidator.IsCompatible(Owner.GetType(), typeof(ComboBoxEntry)) == true)
				return (RemapComboBox(true));
			return (false);
		}
		
		internal void ProcessTypeDescriptions()
		{
			if (RemapControl() == true) 
				SetWidgetModel();
		}
		
		#endregion TYPE_PROCESSING

		#region COMMON_HANDLING
		public override GLib.GType GetColumnType (int aColumn)
		{
			GLib.GType result;
			System.Type type = ListItemType;
			if (SimpleMapping == false)
				type = Types[aColumn];
			if ((SimpleMapping == false) && 
			    ((MultiColumns != null) && (MultiColumns[aColumn] == true)))
				result = GLib.GType.Object;
			else if (TypeValidator.IsCompatible(type, typeof(string)) == true)
				result = GLib.GType.String;
			else if (TypeValidator.IsCompatible(type, typeof(int)) == true)
				result = GLib.GType.Int;
			else if (TypeValidator.IsCompatible(type, typeof(Int64)) == true)
				result = GLib.GType.Int64;
			else if (TypeValidator.IsCompatible(type, typeof(char)) == true)
				result = GLib.GType.Char;
			else if (TypeValidator.IsCompatible(type, typeof(double)) == true)
				result = GLib.GType.Double;
			else if (TypeValidator.IsCompatible(type, typeof(uint)) == true)
				result = GLib.GType.UInt;
			else if (TypeValidator.IsCompatible(type, typeof(UInt64)) == true)
				result = GLib.GType.UInt64;
			else if (TypeValidator.IsCompatible(type, typeof(float)) == true)
				result = GLib.GType.Float;
			else
				result = GLib.GType.Object;
//			System.Console.WriteLine(result.ToString());
			return (result);
		}

		public override int GetNColumns ()
		{
			if (SimpleMapping == true)
				return (1);
			if (PropertyInfos == null)
				return (0);
			return (PropertyInfos.Length);
		}
		
		public override void SetValue (TreeIter aIter, int aColumn, object value)
		{
			object node = NodeFromIter (aIter);
			if (node == null)
				return;
			if (SimpleMapping == true)
				throw new Exception ("SimpleMapping TypedTreeModel.SetValue is not yet handled");
			if (MultiColumns[aColumn] == true)
				throw new Exception (string.Format ("Posting of multicolumns not yet handled: Column={0}, Data={1}, Object={2}", aColumn, value, node));
			PropertyInfos[aColumn].SetObject (node);
			PropertyInfos[aColumn].SetValue (value);
			using (TreePath tp = GetPath (aIter)) {
				ActiveModel.EmitRowChanged (tp, aIter);
			}
		}
		
		public override void GetValue (TreeIter aIter, int aColumn, ref GLib.Value value)
		{
			if (aColumn > (Types.Length-1))
				return;
			object node = NodeFromIter (aIter);
//			value = GLib.Value.Empty;
			if (node == null)
				return;

			if (MultiColumns != null)
				if ((MultiColumns[aColumn] == true) || (SimpleMapping == true)) {
					value = new GLib.Value (node);
//					value.Val = node;
					return;
				}
			object o;
			PropertyInfos[aColumn].SetObject (node);
			PropertyInfos[aColumn].GetValue (out o, null);
			if (o != null) {
//				value.Dispose();
				value = new GLib.Value (o);
//				value.Val = o;
			}
			else {
				System.Type type = ListItemType;
				if (SimpleMapping == false)
					type = Types[aColumn];
				if ((SimpleMapping == false) && (MultiColumns[aColumn] == true))
					value = GLib.Value.Empty;
				else if (TypeValidator.IsCompatible(type, typeof(string)) == true)
					value = new GLib.Value ("");
				else if (TypeValidator.IsCompatible(type, typeof(int)) == true)
					value = new GLib.Value ((int) 0);
				else if (TypeValidator.IsCompatible(type, typeof(Int64)) == true)
					value = new GLib.Value ((Int64) 0);
				else if (TypeValidator.IsCompatible(type, typeof(char)) == true)
					value = new GLib.Value ("");
				else if (TypeValidator.IsCompatible(type, typeof(double)) == true)
					value = new GLib.Value ((double) 0);
				else if (TypeValidator.IsCompatible(type, typeof(uint)) == true)
					value = new GLib.Value ((uint) 0);
				else if (TypeValidator.IsCompatible(type, typeof(UInt64)) == true)
					value = new GLib.Value ((UInt64) 0);
				else if (TypeValidator.IsCompatible(type, typeof(float)) == true)
					value = new GLib.Value ((float) 0);
				else
					value = GLib.Value.Empty;
			}
			node = null;
			o = null;
		}
		#endregion COMMON_HANDLING

		#region LIST_EVENTS
		/// <summary>
		/// OnListChanged Message handler
		/// </summary>
		/// <param name="aList">
		/// List object that changed <see cref="IList"/>
		/// </param>
		protected void DSChanged (object aList)
		{
			object o = ItemsDataSource;
			ItemsDataSource = null;
			ItemsDataSource = o;
		}
		
		/// <summary>
		/// OnElementAdded Message handler
		/// </summary>
		/// <param name="aList">
		/// List object that had added element <see cref="IList"/>
		/// </param>
		/// <param name="aIdx">
		/// Index path to object added <see cref="System.Int32"/>
		/// </param>
		protected void DSElementAdded (object aList, int[] aIdx)
		{
			TreePath tp = new TreePath (aIdx);
			TreeIter iter;
			if (GetIter(out iter, tp) == true)
				ActiveModel.EmitRowInserted (tp, iter);
			tp.Dispose();
		}
		
		/// <summary>
		/// OnElementChanged Message handler 
		/// </summary>
		/// <param name="aList">
		/// List object that had changed element <see cref="IList"/>
		/// </param>
		/// <param name="aIdx">
		/// Index path to object changed <see cref="System.Int32"/>
		/// </param>
		protected void DSElementChanged (object aList, int[] aIdx)
		{
			TreePath tp = new TreePath (aIdx);
			TreeIter iter;
			if (GetIter(out iter, tp) == true)
				ActiveModel.EmitRowChanged (tp, iter);
			tp.Dispose();
		}
		
		/// <summary>
		/// OnElementRemoved Message handler 
		/// </summary>
		/// <param name="aList">
		/// List object that had removed element <see cref="System.Object"/>
		/// </param>
		/// <param name="aIdx">
		/// Index path to object removed <see cref="System.Int32"/>
		/// </param>
		/// <param name="aObject">
		/// Removed element <see cref="System.Object"/>
		/// </param>
		protected void DSElementRemoved (object aList, int[] aIdx, object aObject)
		{
			if ((Owner as IAdaptableListControl).CurrentSelection.FinalTarget == aObject)
				(Owner as IAdaptableListControl).CurrentSelection.Target = null;
			TreePath tp = new TreePath (aIdx);
			ActiveModel.EmitRowDeleted (tp);
			tp.Dispose();
		}
		
		/// <summary>
		/// ElementsSorted Message handler 
		/// </summary>
		/// <param name="aList">
		/// List object that contains sorted element <see cref="System.Object"/>
		/// </param>
		/// <param name="aIdx">
		/// Index path to object being sorted or null if whole list <see cref="System.Int32"/>
		/// </param>
		protected void DSElementsSorted (object aList, int[] aIdx)
		{
			if (aIdx == null)
				ResetWidgetModel();
			else {
				TreePath tp = new TreePath (aIdx);
				TreeIter iter;
				if (GetIter(out iter, tp) == true)
//					ActiveModel.EmitRowsReordered (tp, iter);
					ActiveModel.EmitRowsReordered (tp, iter, null);
				tp.Dispose();
			}
		}		
		#endregion LIST_EVENTS
		
		#region QUERY_METHODS
		public override TreeModelFlags GetFlags()
		{
			if (RespectHierarchy == false)
				return (TreeModelFlags.ListOnly);
			if (Query == null)
				base.GetFlags();
			return (Query.GetFlags());
		}
		
		public override object GetNodeAtPath (TreePath aPath)
		{
			if (Query == null)
				throw new NotImplementedException();
			return (Query.GetNodeAtPath(aPath));
		}

		public override TreePath PathFromNode (object aNode)
		{
			if (Query == null)
				throw new NotImplementedException();
			return (Query.PathFromNode (aNode));
		}

		public override bool IterNext (ref TreeIter aIter)
		{
			object node = NodeFromIter (aIter);
			if (node == null)
				return (false);
			if (Query == null)
				throw new NotImplementedException();
			return (Query.IterNext (ref aIter));
		}

		public override int ChildCount (object aNode)
		{
			if (RespectHierarchy == true)
				if (Query != null)
					return (Query.ChildCount (aNode));
				else
					throw new NotImplementedException();
			return (0);
		}

		public override bool IterChildren (out TreeIter aChild, TreeIter aParent)
		{
			aChild = TreeIter.Zero;
			if (Query == null)
				throw new NotImplementedException();
			return (Query.IterChildren (out aChild, aParent));
		}

		public override bool IterHasChild (TreeIter aIter)
		{
			if (RespectHierarchy == false)
				return (false);
			if (Query == null)
				throw new NotImplementedException();
			return (Query.IterHasChild (aIter));
		}

		public override int IterNChildren (TreeIter aIter)
		{
			if (RespectHierarchy == false)
				return (0);
			if (Query == null)
				throw new NotImplementedException();
			return (Query.IterNChildren (aIter));
		}

		public override bool IterNthChild (out TreeIter aChild, TreeIter aParent, int n)
		{
			aChild = TreeIter.Zero;

			if (RespectHierarchy == false)
				if (aParent.UserData != IntPtr.Zero)
					return (false);
			if (Query == null)
				throw new NotImplementedException();
			return (Query.IterNthChild (out aChild, aParent, n));
		}

		public override bool IterParent (out TreeIter aParent, TreeIter aChild)
		{
			aParent = TreeIter.Zero;
			if (RespectHierarchy == false)
				return (false);
			if (Query == null)
				throw new NotImplementedException();
			return (Query.IterParent (out aParent, aChild));
		}
		#endregion QUERY_METHODS
		
		public override void Disconnect ()
		{
			base.Disconnect ();
			if (listadaptor != null) {
				listadaptor.ListChanged -= DSChanged;
				listadaptor.ElementAdded -= DSElementAdded;
				listadaptor.ElementChanged -= DSElementChanged;
				listadaptor.ElementRemoved -= DSElementRemoved;
				listadaptor.ElementsSorted -= DSElementsSorted;
				listadaptor.TargetChanged -= ListTargetChanged;
				listadaptor.Disconnect();
				listadaptor = null;
			}
			if (NamedCellRenderers != null) {
				NamedCellRenderers.Clear();
				namedCellRenderers = null;
			}
			if (columnadaptor != null) {
				columnadaptor.Disconnect();
				columnadaptor = null;
			}
			modeladapter = null;
		}
		/// <summary>
		/// Called when ItemsDataSource changes
		/// </summary>
		private void ListTargetChanged (IAdaptor aAdaptor)
		{
			cachedItems = null;
			if (ItemsDataSource == null) {
				if (clearSelection != null)
					clearSelection();
//				CurrentSelection.Target = null;
			}
			else {
//				if ((ItemsDataSource is IAdaptor) == false)
//					return;
				object check = ConnectionProvider.ResolveTargetForObject(ItemsDataSource);
				if (check != lastItems) {
					lastItems = check;
					check = ItemsDataSource;
					ItemsDataSource = null;
					ItemsDataSource = check;
				}
			}
			if (checkControl != null)
				checkControl();
		}
		
		private void CreateAdaptors()
		{
			listadaptor = new GtkListAdaptor(false, null, this, false); 
			listadaptor.ListChanged += DSChanged;
			listadaptor.ElementAdded += DSElementAdded;
			listadaptor.ElementChanged += DSElementChanged;
			listadaptor.ElementRemoved += DSElementRemoved;
			listadaptor.ElementsSorted += DSElementsSorted;
			listadaptor.TargetChanged += ListTargetChanged;
//			listadaptor.OnTargetChange += ItemsTargetChanged;
		}
		
		#region CONSTRUCTORS
		public MappingsImplementor (Gtk.Widget aOwner)
		{
			ModelSelector.InitializeModels();
			Owner = aOwner;
			if (Owner == null)
				throw new Exception ("Owner of MappingsImplementor is not set");
			CreateAdaptors();
		}
		
		~MappingsImplementor()
		{
			Disconnect();
		}
		#endregion CONSTRUCTORS
	}
}
