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
using System.Data.Bindings;

namespace Gtk.DataBindings
{
	/// <summary>
	/// 
	/// </summary>
	[QueryModel (typeof(System.Data.Bindings.AssemblyDevelopmentInformation))]
	public class AssemblyDevelopmentInformationQueryModel : HierarchicalShellQueryModel
	{
		/// <summary>
		/// Reference to IList
		/// </summary>
		public AssemblyDevelopmentInformation Items {
			get { 
				if (DataSource != null)
					if (DataSource is AssemblyDevelopmentInformation)
						return (DataSource as AssemblyDevelopmentInformation);
				return (null);
			}
		}

		#region HierarchicalShellQueryModel
		
		protected override object GetItemAtIndex (int[] aIndex)
		{
			switch (aIndex.Length) {
			case 1:
				return (Items.GetInfo(aIndex[0]));
			case 2:
				return (Items.GetInfo(aIndex[0]).GetInfo(aIndex[1]));
			}
			return (null);
		}

		protected override object GetItemChildAtIndex (object aNode, int aIndex)
		{
			if (TypeValidator.IsCompatible(aNode.GetType(), typeof(AssemblyDevelopmentInformation)) == true)
				return ((aNode as AssemblyDevelopmentInformation).GetInfo(aIndex));
			else if (TypeValidator.IsCompatible(aNode.GetType(), typeof(TypeDevelopmentInformation)) == true)
				return ((aNode as TypeDevelopmentInformation).GetInfo(aIndex));
			return (null);
		}

		protected override int GetItemChildCount (object aNode)
		{
			if (TypeValidator.IsCompatible(aNode.GetType(), typeof(AssemblyDevelopmentInformation)) == true)
				return ((aNode as AssemblyDevelopmentInformation).Count);
			else if (TypeValidator.IsCompatible(aNode.GetType(), typeof(TypeDevelopmentInformation)) == true)
				return ((aNode as TypeDevelopmentInformation).Count);
			return (0);
		}

		protected override int[] GetItemIndex (object aNode, bool aDeepSearch)
		{
			if (TypeValidator.IsCompatible(aNode.GetType(), typeof(TypeDevelopmentInformation)) == true) {
				int i = (DataSource as AssemblyDevelopmentInformation).IndexOf((aNode as TypeDevelopmentInformation));
				if (i == -1)
					return (null);
				return (new int[1] {i});
			}
			else if (TypeValidator.IsCompatible(aNode.GetType(), typeof(MemberDevelopmentInformation)) == true) {
				int i = (DataSource as AssemblyDevelopmentInformation).IndexOf((aNode as MemberDevelopmentInformation).GetMemberInfo().DeclaringType);
				TypeDevelopmentInformation ti = (DataSource as AssemblyDevelopmentInformation).GetInfo (i);
				if (i == -1)
					return (null);
				int j = ti.IndexOf(aNode as MemberDevelopmentInformation);
				if (j == -1)
					return (null);
				return (new int[2] {i,j});
			}
			return (null);
		}
		
		#endregion HierarchicalShellQueryModel
		
		public AssemblyDevelopmentInformationQueryModel (MappingsImplementor aMasterImplementor)
			: base (aMasterImplementor)
		{
		}
	}
}
