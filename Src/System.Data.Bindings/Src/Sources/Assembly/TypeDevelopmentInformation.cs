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
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace System.Data.Bindings
{
	/// <summary>
	/// 
	/// </summary>
	public class TypeDevelopmentInformation : IDevelopmentInformation
	{
		private System.Type typeInfo;
		
		private List<MemberInfo> members = new List<MemberInfo>();
		private MemberDevelopmentInformation[] memberInfos = null;

		private void Parse (System.Type aType)
		{
			object[] attrs;
			foreach (MemberInfo info in aType.GetMembers()) {
				bool found = false;
				attrs = info.GetCustomAttributes (false);
				foreach (object attr in attrs) {
					if (TypeValidator.IsCompatible(attr.GetType(), typeof(DevelopmentInformationAttribute)) == true) {
						members.Add (info);
						found = true;
						break;
					}
				}
			}
		}
		
		private DevelopmentDescriptionCollection developmentDescriptions = null; 
		/// <value>
		/// Provides collection of development descriptions
		/// </value>
		public DevelopmentDescriptionCollection DevelopmentDescriptions { 
			get { 
				if (developmentDescriptions == null)
					developmentDescriptions = new DevelopmentDescriptionCollection (typeInfo);
				if (developmentDescriptions.Count == 0)
					return (null);
				return (developmentDescriptions); 
			}
		}
		
		public System.Type GetTypeInfo()
		{
			return (typeInfo);
		}
		
		public int Count {
			get { return (members.Count); }
		}
		
		public string Name {
			get { return (GetTypeInfo().Name); }
		}
		
		public string Description {
			get { return (Name); }
		}
		
		public int IndexOf (MemberInfo aInfo)
		{
			if (aInfo == null)
				return (-1);
			return (members.IndexOf (aInfo));
		}
		
		public int IndexOf (MemberDevelopmentInformation aInfo)
		{
			if (aInfo == null)
				return (-1);
			return (members.IndexOf (aInfo.GetMemberInfo()));
		}
		
		public MemberDevelopmentInformation GetInfo (int aIndex)
		{
			if ((aIndex < 0) || (aIndex >= Count))
				return (null);
			if (memberInfos == null) {
				memberInfos = new MemberDevelopmentInformation[Count];
				for (int i=0; i<Count; i++)
					memberInfos[i] = null;
			}
			if (memberInfos[aIndex] == null)
				memberInfos[aIndex] = new MemberDevelopmentInformation (members[aIndex]);
			return (memberInfos[aIndex]);
		}
		
		public TypeDevelopmentInformation (Type aType)
		{
			typeInfo = aType;
			Parse (aType);
		}
	}
}
