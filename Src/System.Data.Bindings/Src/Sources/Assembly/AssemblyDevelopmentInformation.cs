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
	/// Provides list of types which still need development
	/// </summary>
	public class AssemblyDevelopmentInformation : IDevelopmentInformation
	{
		private Assembly assembly = null;
		private List<System.Type> types = new List<System.Type>();
		private TypeDevelopmentInformation[] typeInfos = null;

		public string Name {
			get { return (assembly.GetName().Name); }
		}

		public string Description {
			get { return (Name); }
		}
		
		private void Parse (Assembly aAssembly)
		{
			if (aAssembly.GetName().Name == "System")
				return;
			BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Public |
					BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField |
					BindingFlags.GetProperty | BindingFlags.SetField | 
					BindingFlags.SetProperty | BindingFlags.Instance | 
					BindingFlags.InvokeMethod | BindingFlags.CreateInstance |
					BindingFlags.ExactBinding | BindingFlags.OptionalParamBinding |
					BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty |
					BindingFlags.SuppressChangeType;
			object[] attrs;
			
			foreach (System.Type type in aAssembly.GetTypes()) {
				bool found = false;
				attrs = type.GetCustomAttributes (false);
				foreach (object attr in attrs) 
					if (TypeValidator.IsCompatible(attr.GetType(), typeof(DevelopmentInformationAttribute)) == true) {
						types.Add (type);
						found = true;
						break;
					}
				if (found == false) {
					foreach (MemberInfo info in type.GetMembers(flags)) {
						try { attrs = info.GetCustomAttributes (false); }
						catch { break; }
						foreach (object attr in attrs) 
							if (TypeValidator.IsCompatible(attr.GetType(), typeof(DevelopmentInformationAttribute)) == true) {
								types.Add (type);
								found = true;
								break;
							}
						if (found == true)
							break;
					}
				}
			}
		}
		
		public int Count {
			get { return (types.Count); }
		}
		
		public TypeDevelopmentInformation GetInfo (int aIndex)
		{
			if ((aIndex < 0) || (aIndex >= Count))
				return (null);
			if (typeInfos == null) {
				typeInfos = new TypeDevelopmentInformation[Count];
				for (int i=0; i<Count; i++)
					typeInfos[i] = null;
			}
			if (typeInfos[aIndex] == null)
				typeInfos[aIndex] = new TypeDevelopmentInformation (types[aIndex]);
			return (typeInfos[aIndex]);
		}

		public int IndexOf (System.Type aType)
		{
			if (aType == null)
				return (-1);
			return (types.IndexOf (aType));
		}

		public int IndexOf (TypeDevelopmentInformation aType)
		{
			if (aType == null)
				return (-1);
			return (types.IndexOf (aType.GetTypeInfo()));
		}
		
		public System.Type[] GetTypesInDevelopment()
		{
			System.Type[] res = new System.Type [Count];
			for (int i=0; i<Count; i++)
				res[i] = types[i];
			return (res);
		}

		private DevelopmentDescriptionCollection developmentDescriptions = null; 
		/// <value>
		/// Provides collection of development descriptions
		/// </value>
		public DevelopmentDescriptionCollection DevelopmentDescriptions { 
			get { 
				if (developmentDescriptions == null)
					developmentDescriptions = new DevelopmentDescriptionCollection (assembly);
				if (developmentDescriptions.Count == 0)
					return (null);
				return (developmentDescriptions); 
			}
		}
		
		public DevelopmentInformationAttribute[] GetDevelopmentInformations (Type aType)
		{
			ArrayList res = new ArrayList();
			Attribute[] attrs = (Attribute[]) aType.GetCustomAttributes (false);
			foreach (Attribute attr in attrs)
				if (TypeValidator.IsCompatible(attr.GetType(), typeof(DevelopmentInformationAttribute)) == true)
					res.Add (attr);
			DevelopmentInformationAttribute[] arr = new DevelopmentInformationAttribute[res.Count];
			for (int i=0; i<Count; i++)
				arr[i] = (DevelopmentInformationAttribute) res[i];
			res.Clear();
			res = null;
			return (arr);
		}

		private AssemblyDevelopmentInformation()
		{
		}
		
		public AssemblyDevelopmentInformation (Assembly aAssembly)
		{
			assembly = aAssembly;
			Parse (aAssembly);
		}
	}
}
