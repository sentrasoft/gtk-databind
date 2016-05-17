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
using System.Collections;
using System.Collections.Generic;
using System.Data.Bindings;
using System.Reflection;

namespace Gtk.DataBindings
{
	/// <summary>
	/// 
	/// </summary>
	[QueryModel (typeof(System.Data.Bindings.DevelopmentDescriptionCollection))]
	public class MemberDevelopmentInformationQueryModel : HierarchicalShellQueryModel
	{
		/// <summary>
		/// Reference to IList
		/// </summary>
		public DevelopmentDescriptionCollection Items {
			get { 
				if (DataSource != null)
					if (DataSource is DevelopmentDescriptionCollection)
						return (DataSource as DevelopmentDescriptionCollection);
				return (null);
			}
		}

		#region HierarchicalShellQueryModel
		
		protected override object GetItemAtIndex (int[] aIndex)
		{
			switch (aIndex.Length) {
			case 1:
				if ((aIndex[0] < 0) || (aIndex[0] >= Items.Count))
					return (null);
				return (Items[aIndex[0]]);
			case 2:
				if ((aIndex[0] < 0) || (aIndex[0] >= Items.Count))
					return (null);
				if ((aIndex[1] < 0) || (aIndex[1] >= Items[aIndex[0]].Count))
					return (null);
				return (Items[aIndex[0]][aIndex[1]]);
			}
			return (null);
		}

		protected override object GetItemChildAtIndex (object aNode, int aIndex)
		{
			if (TypeValidator.IsCompatible(aNode.GetType(), typeof(DevelopmentDescriptionCollection)) == true) {
				if ((aIndex < 0) || (aIndex >= (aNode as DevelopmentDescriptionCollection).Count))
					return (null);
				return ((aNode as DevelopmentDescriptionCollection)[aIndex]);
			}
			else if (TypeValidator.IsCompatible(aNode.GetType(), typeof(DevelopmentInformationAttribute)) == true) {
				if ((aIndex < 0) || (aIndex >= (aNode as DevelopmentInformationAttribute).Count))
					return (null);
				return ((aNode as DevelopmentInformationAttribute)[aIndex]);
			}
			return (null);
		}

		protected override int GetItemChildCount (object aNode)
		{
			if (TypeValidator.IsCompatible(aNode.GetType(), typeof(DevelopmentDescriptionCollection)) == true)
				return ((aNode as DevelopmentDescriptionCollection).Count);
			else if (TypeValidator.IsCompatible(aNode.GetType(), typeof(DevelopmentInformationAttribute)) == true)
				return ((aNode as DevelopmentInformationAttribute).Count);
			return (0);
		}

		protected override int[] GetItemIndex (object aNode, bool aDeepSearch)
		{
			if (TypeValidator.IsCompatible(aNode.GetType(), typeof(DevelopmentInformationAttribute)) == true) {
				int i = (DataSource as DevelopmentDescriptionCollection).IndexOf(aNode);
				if (i == -1)
					return (null);
				return (new int[1] {i});
			}
			else if (TypeValidator.IsCompatible(aNode.GetType(), typeof(DevelopmentInformationItem)) == true) {
				DevelopmentInformationAttribute di = (aNode as DevelopmentInformationItem).Owner;
				int i = (DataSource as DevelopmentDescriptionCollection).IndexOf(di);
				if (i == -1)
					return (null);
				int j = di.IndexOf(aNode as DevelopmentInformationItem);
				if (j == -1)
					return (null);
				return (new int[2] {i,j});
			}
			return (null);
		}
		
		#endregion HierarchicalShellQueryModel
		
		public MemberDevelopmentInformationQueryModel (MappingsImplementor aMasterImplementor)
			: base (aMasterImplementor)
		{
		}
	}
}
