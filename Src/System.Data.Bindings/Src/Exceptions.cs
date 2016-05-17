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
using System.Data.Bindings.Cached;
using System.Data.Bindings.Collections;

namespace System.Data.Bindings
{
	/// <summary>
	/// Called when wrong type is assigned to List DataSource
	/// </summary>
	public class OutOfBoundsException : Exception
	{
		/// <summary>
		/// Creates Exception
		/// </summary>
		public OutOfBoundsException()
			: base ("Executed action is out of bounds")
		{
		}

		/// <summary>
		/// Creates Exception
		/// </summary>
		public OutOfBoundsException (string aMessage)
			: base (aMessage)
		{
		}
	}

	/// <summary>
	/// Called when wrong type is assigned to List DataSource
	/// </summary>
	public class ExceptionInheritedDataRowMissingConstructor : Exception
	{
		/// <summary>
		/// Creates Exception
		/// </summary>
		private ExceptionInheritedDataRowMissingConstructor()
		{
		}

		/// <summary>
		/// Creates Exception
		/// </summary>
		public ExceptionInheritedDataRowMissingConstructor (System.Type aType)
			: base (string.Format("DataRow type ({0}) does not specify public constructor(DataRowBuilder)", aType.ToString()))
		{
		}
	}

	/// <summary>
	/// Called when wrong type is assigned to List DataSource
	/// </summary>
	public class ExceptionWrongListType : Exception
	{
		/// <summary>
		/// Creates Exception
		/// </summary>
		private ExceptionWrongListType()
		{
		}

		/// <summary>
		/// Creates Exception
		/// </summary>
		/// <param name="aControl">
		/// Control being assigned <see cref="System.Object"/>
		/// </param>
		/// <param name="aListType">
		/// List type being assigned <see cref="System.Object"/>
		/// </param>
		public ExceptionWrongListType (object aControl, object aListType)
			: base ("Wrong ListType " + aListType + " for " + aControl)
		{
		}
	}

	/// <summary>
	/// Exception thrown when non-existant type was assigned as DataSource
	/// type
	/// </summary>
	public class ExceptionDataSourceType : Exception
	{
		/// <summary>
		/// Creates Exception
		/// </summary>
		private ExceptionDataSourceType()
		{
		}

		/// <summary>
		/// Creates Exception
		/// </summary>
		/// <param name="aType">
		/// Type name that doesn't exists  <see cref="System.Type"/>
		/// </param>
		public ExceptionDataSourceType (string aType)
			: base ("Type " + aType + " can't be found in current assembly")
		{
		}
	}

	/// <summary>
	/// Exception thrown when type is not formulated correctly inside
	/// mapping string. Types are formulated inside {}
	/// </summary>
	public class ExceptionWrongFormulatedType : Exception
	{
		/// <summary>
		/// Creates Exception
		/// </summary>
		public ExceptionWrongFormulatedType()
			: base ("Wrong formulated Constriction Type")
		{
		}
	}

	/// <summary>
	/// Exception thrown when adaptor doesn't support Repost method
	/// to all children controls
	/// </summary>
	public class ExceptionAdaptorRepostNotSupported : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionAdaptorRepostNotSupported()
			: base ("Override RepostRenewToAllChildren in Adaptor subclass")
		{
		}
	}


	/// <summary>
	/// Exception thrown when adaptor failed Repost method
	/// to all children controls
	/// </summary>
	public class ExceptionAdaptorRepostRenewFailed : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionAdaptorRepostRenewFailed (System.Type aType)
			: base ("RepostRenewToAllChildren in (" + aType + ") class has failed")
		{
		}
	}

	/// <summary>
	/// Exception thrown when adding wrong mapping
	/// </summary>
	public class ExceptionAddWrongMapping : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionAddWrongMapping()
			: base ("Adding wrong mapping")
		{
		}
	}

	/// <summary>
	/// Exception thrown when adding empty mapping
	/// </summary>
	public class ExceptionAddEmptyMapping : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionAddEmptyMapping()
			: base ("Tried to add empty mapping")
		{
		}
	}

	/// <summary>
	/// Exception thrown when mapping is not found
	/// </summary>
	public class ExceptionMappingNotFound : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		private ExceptionMappingNotFound()
		{
		}

		/// <summary>
		/// Throws Exception
		/// </summary>
		/// <param name="aMappingName">
		/// Mapping that doesn't exists  <see cref="System.String"/>
		/// </param>
		public ExceptionMappingNotFound (string aMappingName)
			: base ("Mapping " + aMappingName + " not found")
		{
		}
	}
	
	/// <summary>
	/// Exception thrown when trying to access default property in BoundaryAdaptor
	/// </summary>
	public class ExceptionBoundaryDefaultPropertyAccess : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionBoundaryDefaultPropertyAccess()
			: base ("Trying to access default property in BoundaryAdaptor")
		{
		}
	}
	
	/// <summary>
	/// Exception thrown when trying to assign value to non existant default MappedProperty
	/// </summary>
	public class ExceptionAssigningNonExistantDefaultProperty : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionAssigningNonExistantDefaultProperty()
			: base ("Trying to assign value to non existant default MappedProperty")
		{
		}
	}

	/// <summary>
	/// Exception thrown when Non-Boundary Adaptor is connected to Boundary event
	/// </summary>
	public class ExceptionNonBoundaryAdaptorConnectedToBoundaryEvent : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionNonBoundaryAdaptorConnectedToBoundaryEvent()
			: base ("Non-Boundary Adaptor connected to Boundary event???")
		{
		}
	}
	
	/// <summary>
	/// Exception thrown when Adaptor is connected with wrong control type
	/// </summary>
	public class ExceptionAdaptorConnectedWithWrongWidgetType : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		private ExceptionAdaptorConnectedWithWrongWidgetType()
		{
		}

		/// <summary>
		/// Throws Exception
		/// </summary>
		/// <param name="aAdaptor">
		/// Adaptor <see cref="IAdaptor"/>
		/// </param>
		/// <param name="aWidget">
		/// Control <see cref="System.Object"/>
		/// </param>
		public ExceptionAdaptorConnectedWithWrongWidgetType (IAdaptor aAdaptor, object aWidget)
			: base ("Adaptor " + aAdaptor + " was created with wrong widget type (" + aWidget + ")") 
		{
		}
	}
	
	/// <summary>
	/// Exception thrown when Adaptor is connected with wrong control type
	/// </summary>
	public class ExceptionControlAdaptorConnectedWithWrongWidgetType : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		private ExceptionControlAdaptorConnectedWithWrongWidgetType()
		{
		}

		/// <summary>
		/// Throws Exception
		/// </summary>
		/// <param name="aAdaptor">
		/// Adaptor <see cref="ControlAdaptor"/>
		/// </param>
		/// <param name="aWidget">
		/// Control <see cref="System.Object"/>
		/// </param>
		public ExceptionControlAdaptorConnectedWithWrongWidgetType (ControlAdaptor aAdaptor, object aWidget)
			: base ("Adaptor " + aAdaptor + " was created with wrong widget type (" + aWidget + ")") 
		{
		}
	}
	
	/// <summary>
	/// Exception thrown when CachedProperty SetValue fails
	/// </summary>
	public class ExceptionCachedPropertySetValueFailed : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		private ExceptionCachedPropertySetValueFailed()
		{
		}

		/// <summary>
		/// Throws Exception
		/// </summary>
		/// <param name="aProperty">
		/// Property which failed on SetValue <see cref="CachedProperty"/>
		/// </param>
		public ExceptionCachedPropertySetValueFailed (CachedProperty aProperty)
			: base ("Property was cached, but CachedProperty.SetValue() did not succedd????")
		{
		}
	}
	
	/// <summary>
	/// Exception thrown when CachedProperty GetValue fails
	/// </summary>
	public class ExceptionCachedPropertyGetValueFailed : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		private ExceptionCachedPropertyGetValueFailed()
		{
		}

		/// <summary>
		/// Throws Exception
		/// </summary>
		/// <param name="aProperty">
		/// Property which failed on GetValue <see cref="CachedProperty"/>
		/// </param>
		public ExceptionCachedPropertyGetValueFailed (CachedProperty aProperty)
			: base ("Property was cached, but CachedProperty.GetValue() did not succedd????")
		{
		}
	}
	
	/// <summary>
	/// Exception thrown when CachedProperty creation fails
	/// </summary>
	public class ExceptionCachedPropertyCreateFailed : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionCachedPropertyCreateFailed()
			: base ("Wrong creation of CachedProperty") 
		{
		}
	}
	
	/// <summary>
	/// Exception thrown when assigning value to read only property
	/// </summary>
	public class ExceptionAssigningReadOnlyProperty : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionAssigningReadOnlyProperty()
			: base ("Assigning value to property where value can't be written") 
		{
		}
	}
	
	/// <summary>
	/// Exception thrown when assigning value to read only property
	/// </summary>
	public class ExceptionGettingWriteOnlyProperty : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionGettingWriteOnlyProperty()
			: base ("Assigning from property where value can't be read")
		{
		}
	}
	
	/// <summary>
	/// Exception thrown when translating property in CopyPropertyValueByInfo
	/// </summary>
	public class ExceptionPropertyTranslationError : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionPropertyTranslationError()
			: base ("NO FUN in translation") 
		{
		}
	}
	
	/// <summary>
	/// Exception thrown when property specified in mapping is not found
	/// </summary>
	public class ExceptionPropertyNotFound : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		private ExceptionPropertyNotFound()
		{
		}

		/// <summary>
		/// Throws Exception
		/// </summary>
		/// <param name="aPropertyName">
		/// Property that doesn't exists <see cref="System.String"/>
		/// </param>
		public ExceptionPropertyNotFound (string aPropertyName)
			: base ("Property " + aPropertyName + " not found")
		{
		}

		/// <summary>
		/// Throws Exception
		/// </summary>
		/// <param name="aPropertyName">
		/// Property that doesn't exists <see cref="System.String"/>
		/// </param>
		/// <param name="aObject">
		/// Object where property doesn't exists <see cref="System.Object"/>
		/// </param>
		public ExceptionPropertyNotFound (string aPropertyName, object aObject)
			: base ("Property " + aPropertyName + " not found in type (" + aObject + ")")
		{
		}
	}	
	
	/// <summary>
	/// Exception thrown accessing Adaptor in ContainerControl
	/// </summary>
	public class ExceptionAccessAdaptorInContainerControl : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionAccessAdaptorInContainerControl()
			: base ("Tried to access Adaptor in ContainerControl") 
		{
		}
	}
	
	/// <summary>
	/// Exception thrown when method is not accessible and needs to be overriden
	/// in descedant classes
	/// </summary>
	public class ExceptionDescedantOverrideMethod : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		private ExceptionDescedantOverrideMethod()
		{
		}

		/// <summary>
		/// Throws Exception
		/// </summary>
		/// <param name="aType">
		/// Class with method that needs override <see cref="System.Type"/>
		/// </param>
		/// <param name="aMethod">
		/// Method which needs to be overriden in descedant <see cref="System.String"/>
		/// </param>
		public ExceptionDescedantOverrideMethod (System.Type aType, string aMethod)
			: base ("Every descedant of " + aType + " has to override " + aMethod + " according its capabilities")
		{
		}
	}	
	
	/// <summary>
	/// Exception thrown when property specified in mapping is not found
	/// </summary>
	public class ExceptionDatabasePropertyNotFound : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		private ExceptionDatabasePropertyNotFound()
		{
		}

		/// <summary>
		/// Throws Exception
		/// </summary>
		/// <param name="aPropertyName">
		/// Property that doesn't exists <see cref="System.String"/>
		/// </param>
		public ExceptionDatabasePropertyNotFound (string aPropertyName)
			: base ("Property " + aPropertyName + " not found")
		{
		}

		/// <summary>
		/// Throws Exception
		/// </summary>
		/// <param name="aPropertyName">
		/// Property that doesn't exists <see cref="System.String"/>
		/// </param>
		/// <param name="aObject">
		/// Object where property doesn't exists <see cref="System.Object"/>
		/// </param>
		public ExceptionDatabasePropertyNotFound (string aPropertyName, object aObject)
			: base ("Property " + aPropertyName + " not found in type (" + aObject + ")")
		{
		}
	}		
	
	/// <summary>
	/// Exception thrown when map attribute was initialized with empty name
	/// </summary>
	public class ExceptionMapAttributeNonSpecified : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionMapAttributeNonSpecified()
			: base ("MapAttribute cannot point to non-specified property")
		{
		}
	}		
	
	/// <summary>
	/// Exception thrown when column is found in boundary mapping
	/// </summary>
	public class ExceptionColumnInBoundaryMapping : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionColumnInBoundaryMapping()
			: base ("Can't define column mapping in Boundary adaptor")
		{
		}
	}		
	
	/// <summary>
	/// Exception thrown when boundary mapping is set to non-boundary adaptor
	/// </summary>
	public class ExceptionBoundaryMappingSetToNonBoundary : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionBoundaryMappingSetToNonBoundary()
			: base ("You can't set boundary mapping to non-Boundary adaptor")
		{
		}
	}		
	
	/// <summary>
	/// Exception thrown when mapped property discovers writing is not possible
	/// </summary>
	public class ExceptionPropertyWriteNotPossible : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionPropertyWriteNotPossible()
			: base ("Assigning write property mapping where it is not possible")
		{
		}

		/// <summary>
		/// Throws Exception
		/// </summary>
		/// <param name="aValid">
		/// State of valid <see cref="System.Boolean"/>
		/// </param>
		public ExceptionPropertyWriteNotPossible (bool aValid)
			: base ("Tried writing to MappedProperty when AllowedToWrite=false, Valid=" + aValid)
		{
		}
	}		

	/// <summary>
	/// Exception thrown when assigning wrong read/write value
	/// </summary>
	public class ExceptionAssigningWrongReadWriteMapping : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		private ExceptionAssigningWrongReadWriteMapping()
		{
		}

		/// <summary>
		/// Throws Exception
		/// </summary>
		/// <param name="aStr">
		/// Value of wrong read/write mapping <see cref="System.String"/>
		/// </param>
		public ExceptionAssigningWrongReadWriteMapping (string aStr)
			: base ("Assigning wrong read/write mapping `" + aStr + "`")
		{
		}
	}	
	
	/// <summary>
	/// Exception thrown when global mapping is assigned from control to target
	/// </summary>
	public class ExceptionGlobalMappingAssignedFromControlToTarget : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionGlobalMappingAssignedFromControlToTarget()
			: base ("Global mappings can't be assigned from Control to Target")
		{
		}
	}		
	
	/// <summary>
	/// Exception thrown when mapped property is being created with null adaptor
	/// </summary>
	public class ExceptionMappedPropertyWithNullAdaptor : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionMappedPropertyWithNullAdaptor()
			: base ("trying to create Mapped property with null adaptor")
		{
		}
	}		
	
	/// <summary>
	/// Exception thrown when non-boundary mapping is set to boundary adaptor
	/// </summary>
	public class ExceptionNonBoundaryMappingSetToBoundary : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionNonBoundaryMappingSetToBoundary()
			: base ("You can't set non-Boundary Mapping to Boundary adaptor")
		{
		}

		/// <summary>
		/// Throws Exception
		/// </summary>
		/// <param name="aName">
		/// Mapping name <see cref="System.String"/>
		/// </param>
		public ExceptionNonBoundaryMappingSetToBoundary (string aName)
			: base ("You can't set non-Boundary Mapping to Boundary adaptor >> Name=" + aName)
		{
		}

		/// <summary>
		/// Throws Exception
		/// </summary>
		/// <param name="aName">
		/// Mapping name <see cref="System.String"/>
		/// </param>
		/// <param name="aTarget">
		/// Target name <see cref="System.String"/>
		/// </param>
		public ExceptionNonBoundaryMappingSetToBoundary (string aName, string aTarget)
			: base ("You can't set non-Boundary Mapping to Boundary adaptor >> Name=" + aName + " Target=" + aTarget)
		{
		}

		/// <summary>
		/// Throws Exception
		/// </summary>
		/// <param name="aName">
		/// Mapping name <see cref="System.String"/>
		/// </param>
		/// <param name="aTarget">
		/// Target name <see cref="System.String"/>
		/// </param>
		/// <param name="aColumnName">
		/// Column name <see cref="System.String"/>
		/// </param>
		public ExceptionNonBoundaryMappingSetToBoundary (string aName, string aTarget, string aColumnName)
			: base ("You can't set non-Boundary Mapping to Boundary adaptor >> Name=" + aName + " Target=" + aTarget + " Column=" + aColumnName)
		{
		}

		/// <summary>
		/// Throws Exception
		/// </summary>
		/// <param name="aName">
		/// Mapping name <see cref="System.String"/>
		/// </param>
		/// <param name="aIsColumn">
		/// Doesn't matter the value <see cref="System.Boolean"/>
		/// </param>
		/// <param name="aColumnName">
		/// Column name <see cref="System.String"/>
		/// </param>
		public ExceptionNonBoundaryMappingSetToBoundary (string aName, bool aIsColumn, string aColumnName)
			: base ("You can't set non-Boundary Mapping to Boundary adaptor >> Name=" + aName + " Column=" + aColumnName)
		{
		}
	}		
	
	/// <summary>
	/// Exception thrown when maping was created without defined target
	/// </summary>
	public class ExceptionMappingRequiresDefinedTarget : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionMappingRequiresDefinedTarget()
			: base ("Boundary Mapping requires defined Target on creation")
		{
		}

		/// <summary>
		/// Throws Exception
		/// </summary>
		/// <param name="aName">
		/// Mapping name <see cref="System.String"/>
		/// </param>
		public ExceptionMappingRequiresDefinedTarget (string aName)
			: base ("Boundary Mapping requires defined Target on creation >> Name=" + aName)
		{
		}
	}		

	/// <summary>
	/// Exception thrown when allocating sub-item, but master item is null
	/// </summary>
	public class ExceptionMasterItemIsNull : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionMasterItemIsNull()
			: base ("Master Item was null when allocating sub-item")
		{
		}
	}		

	/// <summary>
	/// Exception thrown when observeable list didn't override CreateList
	/// </summary>
	public class ExceptionObserveableListCreatedWithNullList : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionObserveableListCreatedWithNullList()
			: base ("ObserveableList was initialized as null with CreatList()")
		{
		}
	}		

	/// <summary>
	/// Exception thrown when observeable list is create with null parameter
	/// </summary>
	public class ExceptionObserveableListCreatedWithNullListParameter : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionObserveableListCreatedWithNullListParameter()
			: base ("ObserveableList was initialized with null List Parameter")
		{
		}
	}		

	/// <summary>
	/// Exception thrown when observeable list was spawned with untyped array
	/// </summary>
	public class ExceptionObserveableSpawnedWithUntypedArray : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionObserveableSpawnedWithUntypedArray()
			: base ("Untyped array was spawned")
		{
		}
	}		

	/// <summary>
	/// Exception thrown when value list adaptor is null
	/// </summary>
	public class ExceptionValueListAdaptorIsNull : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionValueListAdaptorIsNull()
			: base ("Adaptor in ValueList is null")
		{
		}
	}		
	
	/// <summary>
	/// Exception thrown when VirtualProperty was accessed with wrong index
	/// </summary>
	public class ExceptionWrongVirtualPropertyIndex : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		private ExceptionWrongVirtualPropertyIndex()
		{
		}

		/// <summary>
		/// Throws Exception
		/// </summary>
		/// <param name="aIdx">
		/// Index <see cref="System.Int32"/>
		/// </param>
		/// <param name="aType">
		/// Object type <see cref="System.String"/>
		/// </param>
		public ExceptionWrongVirtualPropertyIndex (int aIdx, string aType)
			: base ("Wrong index " + aIdx + " for accessing VirtualProperty on type " + aType)
		{
		}
	}
	
	/// <summary>
	/// Exception thrown when VirtualProperty was accessed with wrong index
	/// </summary>
	public class ExceptionSettingAlreadyPresetVirtualObjectType : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		private ExceptionSettingAlreadyPresetVirtualObjectType()
		{
		}

		/// <summary>
		/// Throws Exception
		/// </summary>
		/// <param name="aName">
		/// Name <see cref="System.String"/>
		/// </param>
		/// <param name="aNewName">
		/// New name <see cref="System.String"/>
		/// </param>
		public ExceptionSettingAlreadyPresetVirtualObjectType (string aName, string aNewName)
			: base ("Can't set already preset ObjectTypeName (" + aName + " to " + aNewName)
		{
		}
	}

	/// <summary>
	/// Exception thrown when VirtualObject was created with null type 
	/// </summary>
	public class ExceptionVirtualObjectTypeCantBeNull : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionVirtualObjectTypeCantBeNull()
			: base ("Can't set VirtualObject type to null")
		{
		}
	}		

	/// <summary>
	/// Exception thrown when resetting preexisting type 
	/// </summary>
	public class ExceptionVirtualObjectCantResetType : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionVirtualObjectCantResetType()
			: base ("Can't reset VirtualObject to another type")
		{
		}
	}
	
	/// <summary>
	/// Exception thrown when VirtualProperty was accessed with wrong name
	/// </summary>
	public class ExceptionWrongVirtualPropertyName : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		private ExceptionWrongVirtualPropertyName()
		{
		}

		/// <summary>
		/// Throws Exception
		/// </summary>
		/// <param name="aType">
		/// Object type <see cref="System.String"/>
		/// </param>
		public ExceptionWrongVirtualPropertyName (string aType)
			: base ("Empty name for accessing VirtualProperty on type " + aType + " is not allowed")
		{
		}

		/// <summary>
		/// Throws Exception
		/// </summary>
		/// <param name="aName">
		/// Property name <see cref="System.String"/>
		/// </param>
		/// <param name="aType">
		/// Object type <see cref="System.String"/>
		/// </param>
		public ExceptionWrongVirtualPropertyName (string aName, string aType)
			: base ("Wrong Name " + aName + " for accessing VirtualProperty on type " + aType)
		{
		}
	}	

	/// <summary>
	/// Exception thrown when adding property to virtual object with null type
	/// </summary>
	public class ExceptionCantAddPropertyToVirtualObjectType : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionCantAddPropertyToVirtualObjectType()
			: base ("Can't add VirtualProperty to null ObjectType")
		{
		}
	}

	/// <summary>
	/// Exception thrown when adding property to virtual object with null type
	/// </summary>
	public class ExceptionCantRemovePropertyFromVirtualObjectType : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionCantRemovePropertyFromVirtualObjectType()
			: base ("Can't remove VirtualProperty from null ObjectType")
		{
		}
	}

	/// <summary>
	/// Exception thrown when VirtualObject tried inheriting from null object
	/// </summary>
	public class ExceptionVirtualObjectCantInheritNullObject : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionVirtualObjectCantInheritNullObject()
			: base ("Can't inherit null object")
		{
		}
	}

	/// <summary>
	/// Exception thrown when VirtualObject tried inheriting from null type
	/// </summary>
	public class ExceptionVirtualObjectCantInheritNullType : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionVirtualObjectCantInheritNullType()
			: base ("Can't inherit null type")
		{
		}
	}

	/// <summary>
	/// Exception thrown when registering duplicate object
	/// </summary>
	public class ExceptionDuplicateVirtualObjectRegistered : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		private ExceptionDuplicateVirtualObjectRegistered()
		{
		}

		/// <summary>
		/// Throws Exception
		/// </summary>
		/// <param name="aName">
		/// Object name <see cref="System.String"/>
		/// </param>
		public ExceptionDuplicateVirtualObjectRegistered (string aName)
			: base ("Duplicate object registered '" + aName + "'")
		{
		}
	}	

	/// <summary>
	/// Exception thrown when locking null virtual object
	/// </summary>
	public class ExceptionLockNullVirtualObject : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionLockNullVirtualObject()
			: base ("Can't lock null object")
		{
		}
	}	

	/// <summary>
	/// Exception thrown when unlocking null virtual object
	/// </summary>
	public class ExceptionUnlockNullVirtualObject : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionUnlockNullVirtualObject()
			: base ("Can't unlock null object")
		{
		}
	}	

	/// <summary>
	/// Exception thrown when locking null virtual object
	/// </summary>
	public class ExceptionLockNullNonExistantVirtualObject : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		private ExceptionLockNullNonExistantVirtualObject()
		{
		}

		/// <summary>
		/// Throws Exception
		/// </summary>
		/// <param name="aName">
		/// Object name <see cref="System.String"/>
		/// </param>
		public ExceptionLockNullNonExistantVirtualObject (string aName)
			: base ("Can't lock object '" + aName + "' because it doesn't exist!")
		{
		}
	}	

	/// <summary>
	/// Exception thrown when unlocking null virtual object
	/// </summary>
	public class ExceptionUnlockNullNonExistantVirtualObject : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		private ExceptionUnlockNullNonExistantVirtualObject()
		{
		}

		/// <summary>
		/// Throws Exception
		/// </summary>
		/// <param name="aName">
		/// Object name <see cref="System.String"/>
		/// </param>
		public ExceptionUnlockNullNonExistantVirtualObject (string aName)
			: base ("Can't unlock object '" + aName + "' because it doesn't exist!")
		{
		}
	}	

	/// <summary>
	/// Exception thrown when renaming already named virtual object
	/// </summary>
	public class ExceptionCantRenameAlreadyNamedVirtualObject : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionCantRenameAlreadyNamedVirtualObject()
			: base ("Can't rename object that already has a name")
		{
		}
	}	

	/// <summary>
	/// Exception thrown when adding member to locked virtual object
	/// </summary>
	public class ExceptionCantAddMemberToLockedVirtualObject : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionCantAddMemberToLockedVirtualObject()
			: base ("Can't add member to locked Object")
		{
		}
	}	

	/// <summary>
	/// Exception thrown when removing member from locked virtual object
	/// </summary>
	public class ExceptionCantRemoveMemberFromLockedVirtualObject : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionCantRemoveMemberFromLockedVirtualObject()
			: base ("Can't remove member from locked Object")
		{
		}
	}	

	/// <summary>
	/// Exception thrown when creating virtual property twice, but with different type
	/// </summary>
	public class ExceptionCreatingVirtualPropertyTwiceWithDifferentType : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionCreatingVirtualPropertyTwiceWithDifferentType()
			: base ("Creating VirtualProprty twice, but different type")
		{
		}

		/// <summary>
		/// Throws Exception
		/// </summary>
		/// <param name="aName">
		/// Property name <see cref="System.String"/>
		/// </param>
		public ExceptionCreatingVirtualPropertyTwiceWithDifferentType (string aName)
			: base ("Creating VirtualProprty " + aName + " twice, but different type")
		{
		}
	}	

	/// <summary>
	/// Exception thrown when adding member to locked virtual object
	/// </summary>
	public class ExceptionIndexOutOfRangeWhenRemovingVirtualMember : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionIndexOutOfRangeWhenRemovingVirtualMember()
			: base ("Index out of range when removing VirtualMember")
		{
		}
	}	

	/// <summary>
	/// Exception thrown when removing member with different name
	/// </summary>
	public class ExceptionRemovingMemberWithDifferentName : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		public ExceptionRemovingMemberWithDifferentName()
			: base ("Removing member with different name")
		{
		}
	}	

	/// <summary>
	/// Exception thrown when setting wrong type of value to virtual property
	/// </summary>
	public class ExceptionWrongTypeValueSetToVirtualProperty : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		private ExceptionWrongTypeValueSetToVirtualProperty()
		{
		}

		/// <summary>
		/// Throws Exception
		/// </summary>
		/// <param name="aOriginalType">
		/// Property name <see cref="System.Type"/>
		/// </param>
		/// <param name="aValueType">
		/// Property name <see cref="System.Type"/>
		/// </param>
		public ExceptionWrongTypeValueSetToVirtualProperty (System.Type aOriginalType, System.Type aValueType)
			: base ("Wrong type value set to VirtualProperty original=" + aOriginalType + " value=" + aValueType)
		{
		}
	}	

	/// <summary>
	/// Exception thrown when it faultsassigning value to virtual property
	/// </summary>
	public class ExceptionErrorAssigningValueToVirtualProperty : Exception
	{
		/// <summary>
		/// Throws Exception
		/// </summary>
		private ExceptionErrorAssigningValueToVirtualProperty()
		{
		}

		/// <summary>
		/// Throws Exception
		/// </summary>
		/// <param name="aOriginalType">
		/// Property name <see cref="System.Type"/>
		/// </param>
		/// <param name="aValueType">
		/// Property name <see cref="System.Type"/>
		/// </param>
		public ExceptionErrorAssigningValueToVirtualProperty (System.Type aOriginalType, System.Type aValueType)
			: base ("Error assigning VirtualProperty original=" + aOriginalType + " value=" + aValueType)
		{
		}
	}	
}
