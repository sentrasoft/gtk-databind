
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using System.IO;

namespace System.Data.Bindings
{
	public static class ResourceEnumerator
	{
		public static IEnumerable GetAssemblyResourceNames (Assembly aAssembly)
		{
			if (aAssembly != null) 
				foreach (string resourceName in aAssembly.GetManifestResourceNames())
					yield return (resourceName);
		}

		public static IEnumerable GetEntryAssemblyResourceNames()
		{
			foreach (string resourceName in GetAssemblyResourceNames(Assembly.GetEntryAssembly()))
				yield return (resourceName);
		}

		public static IEnumerable GetEntryAssemblyResourceNames (string aEndsWith)
		{
			foreach (string resourceName in GetEntryAssemblyResourceNames())
				if (resourceName.EndsWith(aEndsWith) == true)
					yield return (resourceName);
		}
		
		public static Stream GetAssemblyResourceStream (Assembly aAssembly, string aName)
		{
			return (aAssembly.GetManifestResourceStream(aName));
		}
		
		public static Stream GetEntryAssemblyResourceStream (string aName)
		{
			return (Assembly.GetEntryAssembly().GetManifestResourceStream(aName));
		}
		
		public static byte[] LoadBinaryResourceFromEntryAssembly (string aName)
		{
			return (LoadBinaryResourceFromAssembly (Assembly.GetEntryAssembly(), aName));
		}
		
		public static byte[] LoadBinaryResourceFromAssembly (Assembly aAssembly, string aName)
		{
			Stream stream = aAssembly.GetManifestResourceStream (aName);
			BinaryReader br = new BinaryReader (stream);
			byte[] res = br.ReadBytes ((int) br.BaseStream.Length);
			br.Close();
			stream = null;
			return (res);
		}
		
		public static string LoadTextResourceFromEntryAssembly (string aName)
		{
			return (LoadTextResourceFromAssembly (Assembly.GetEntryAssembly(), aName));
		}
		
		public static string LoadTextResourceFromAssembly (Assembly aAssembly, string aName)
		{
			Stream stream = GetAssemblyResourceStream (aAssembly, aName);
			if (stream == null)
				return ("");
			StreamReader streamReader = new StreamReader(stream);
			string text = streamReader.ReadToEnd();
			streamReader.Close();
			stream = null;
			return (text);
		}
		
		public static Assembly FindResource (string aName)
		{
			return (FindResource (Assembly.GetEntryAssembly(), aName));
		}
		
		public static Assembly FindResource (Assembly aAssembly, string aName)
		{
			StringCollection cache = new StringCollection();
			Assembly loc = FindResource (aAssembly, aName, cache);
			cache.Clear();
			cache = null;
			return (loc);
		}
		
		private static Assembly FindResource (Assembly aAssembly, string aName, StringCollection aCache)
		{
			foreach (string name in GetAssemblyResourceNames(aAssembly))
				if (name == aName)
					return (aAssembly);
			
			Assembly res;
			// Go trough assembly referenced assemblies
			foreach (AssemblyName mod in aAssembly.GetReferencedAssemblies())
				if (mod != null)
					if (aCache.Contains(mod.Name) == false) {
						aCache.Add(mod.Name);
						res = FindResource (Assembly.Load(mod), aName, aCache);
						if (res != null)
							return (res);
					}
			
			return (null);
		}
	}
}
