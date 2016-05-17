
using System;

namespace System.Data.Bindings
{
	/// <summary>
	/// Describes class with their respective property views
	/// </summary>
	[AttributeUsage (AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, 
	                 Inherited=true, AllowMultiple = true)]
	public class ColumnViewAttribute : Attribute
	{
		private string name = "default";
		/// <value>
		/// Defines view name
		/// </value>
		public string Name {
			get { return (name); }
		}
		
		private string mappings = "";
		/// <value>
		/// Defines mappings for that class
		/// </value>
		public string Mappings {
			get { return (mappings); }
		}
		
		private ColumnViewAttribute()
		{
		}
		
		public ColumnViewAttribute (string aMappings)
		{
			mappings = aMappings;
		}
		
		public ColumnViewAttribute (string aViewName, string aMappings)
			: this (aMappings)
		{
			name = aViewName;
		}
	}
}
