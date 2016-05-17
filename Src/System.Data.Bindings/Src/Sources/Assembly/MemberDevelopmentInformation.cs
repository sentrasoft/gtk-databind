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
	public class MemberDevelopmentInformation : IDevelopmentInformation
	{
		private MemberInfo memberInfo;
		
		private List<DevelopmentInformationAttribute> members = new List<DevelopmentInformationAttribute>();

		public int Count {
			get { return (members.Count); }
		}

		public string Name {
			get { return (memberInfo.Name); }
		}
		
		public string Description {
			get { return (Name); }
		}
		
		public MemberInfo GetMemberInfo()
		{
			return (memberInfo);
		}
		
		private DevelopmentDescriptionCollection developmentDescriptions = null; 
		/// <value>
		/// Provides collection of development descriptions
		/// </value>
		public DevelopmentDescriptionCollection DevelopmentDescriptions { 
			get { 
				if (developmentDescriptions == null)
					developmentDescriptions = new DevelopmentDescriptionCollection (memberInfo);
				return (developmentDescriptions); 
			}
		}
		
		public void Parse (MemberInfo aMember)
		{
			object[] attrs = aMember.GetCustomAttributes (false);
			foreach (object attr in attrs)
				if (TypeValidator.IsCompatible(attr.GetType(), typeof(DevelopmentInformationAttribute)) == true)
					members.Add ((DevelopmentInformationAttribute) attr);
		}
		
		private MemberDevelopmentInformation()
		{
		}
		
		public MemberDevelopmentInformation (MemberInfo aMember)
		{
			memberInfo = aMember;
			Parse (aMember);
		}
	}
}
