// Exceptions.cs - Field Attribute to assign additional information for Gtk#Databindings
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
using System.Data.Bindings;

namespace Gtk.DataBindings
{
	public class ExceptionDuplicateQueryModelTypeHandler : Exception
	{
		public ExceptionDuplicateQueryModelTypeHandler (System.Type aModel, System.Type aType)
			: base (string.Format("{0} is a duplicate handler for {2}. Use CreateCustomModel event instead", aModel, aType))
		{
		}
	}

	public class ExceptionInvalidTreeModelParameters : Exception
	{
		public ExceptionInvalidTreeModelParameters ()
			: base ("Invalid TypedTreeModel parameters!")
		{
		}
	}

	public class ExceptionInvalidAssignmentCellRenderer : Exception
	{
		public ExceptionInvalidAssignmentCellRenderer (System.Type aType)
			: base ("Invalid CellRenderer for assignment in (" + aType + ")")
		{
		}
	}

	public class ExceptionDataSourceSimpleMappingOnly : Exception
	{
		public ExceptionDataSourceSimpleMappingOnly (System.Type aType)
			: base ("DataSource only supports simple mapping in (" + aType + ")")
		{
		}
	}

	public class ExceptionSimpleTypeUsedWithMultiColumn : Exception
	{
		public ExceptionSimpleTypeUsedWithMultiColumn (System.Type aType)
			: base ("Simple types can't be used with multiple columns in (" + aType + ")")
		{
		}
	}

	public class ExceptionDataSourceTypeWasNotSpecified : Exception
	{
		public ExceptionDataSourceTypeWasNotSpecified (System.Type aType, IAdaptor aAdaptor)
			: base ("DataSource Type constriction was not specified in mapping in (" + aType + ")")
		{
		}
	}

	public class ExceptionColumnSubItemsNotSupported : Exception
	{
		public ExceptionColumnSubItemsNotSupported (System.Type aType)
			: base ("(" + aType + ") doesn't suppport subitems")
		{
		}
	}

	public class ExceptionSpecialCellRendererNotFound : Exception
	{
		public ExceptionSpecialCellRendererNotFound (string aRenderer)
			: base ("Special CellRenderer " + aRenderer + " was specified, but not found")
		{
		}
	}

	public class ExceptionResolvingPathFromTreeIterOnChanged : Exception
	{
		public ExceptionResolvingPathFromTreeIterOnChanged (System.Type aType)
			: base ("Trouble resolving path from TreeIter in (" + aType + ") OnChanged")
		{
		}
	}

	public class ExceptionOnlySpecificMappingTargetsAllowed : Exception
	{
		public ExceptionOnlySpecificMappingTargetsAllowed (System.Type aType, string aTarget, string aValidTargets)
			: base ("(" + aType + ") Only " + aValidTargets + " mapping targets are allowed here, but " + aTarget + " was specified!")
		{
		}
	}

	public class ExceptionWrongCaseOfActionControllerInitialization : Exception
	{
		public ExceptionWrongCaseOfActionControllerInitialization()
			: base ("Wrong case of ActionController initialization")
		{
		}
	}

	public class ExceptionNoRendererAssignedYet : Exception
	{
		public ExceptionNoRendererAssignedYet()
			: base ("No Renderer has been assigned yet")
		{
		}
	}

	public class ExceptionAddingInvalidCellRenderer : Exception
	{
		public ExceptionAddingInvalidCellRenderer()
			: base ("Trying to add invalid Cell Renderer")
		{
		}
	}

	public class ExceptionAddingAlreadyExistingCellRenderer : Exception
	{
		public ExceptionAddingAlreadyExistingCellRenderer (string aName)
			: base ("CellRenderer by name " + aName + " already exists")
		{
		}
	}

	public class ExceptionDragObjectIsNull : Exception
	{
		public ExceptionDragObjectIsNull()
			: base ("DragObjectData.Object = null")
		{
		}
	}

	public class ExceptionListAdaptorIsNull : Exception
	{
		public ExceptionListAdaptorIsNull()
			: base ("ListAdaptor = null")
		{
		}
	}

	public class ExceptionClearBeforeRemapping : Exception
	{
		public ExceptionClearBeforeRemapping (System.Type aType)
			: base ("Error clearing before remapping (" + aType + ")")
		{
		}
	}

}
