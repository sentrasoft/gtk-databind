
using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Reflection;
using System.Data.Bindings;

namespace Gtk.DataBindings
{
	/// <summary>
	/// Handles registration and creation of custom query models
	/// </summary>
	public static class ModelSelector
	{
		private static bool initDone = false;
		private static StringCollection assembliesProcessed = new StringCollection();
		private static List<QueryModelDescription> models = new List<QueryModelDescription>();
		
		private static StringCollection cache = new StringCollection();
		
		/// <summary>
		/// Registers models in whole assembly
		/// </summary>
		public static void RegisterModels()
		{
			if (cache.Count == 0)
				AssemblyEngine.AssemblyLoaded += HandleAssemblyLoaded;
			RegisterModelsInAssembly (Assembly.GetEntryAssembly(), cache);
		}

		/// <summary>
		/// Registers models in whole assembly
		/// </summary>
		public static void InitializeModels()
		{
			if (cache.Count == 0)
				AssemblyEngine.AssemblyLoaded += HandleAssemblyLoaded;
			if (initDone == false)
				RegisterModelsInAssembly (Assembly.GetEntryAssembly(), cache);
			initDone = true;
		}

		/// <summary>
		/// Processing method which should be called on loading new 
		/// assembly in runtime
		/// </summary>
		/// <param name="aAssembly">
		/// Loaded assembly <see cref="Assembly"/>
		/// </param>
		internal static void HandleAssemblyLoaded (object aSender, NewAssemblyLoadedEventArgs aArgs)
		{
			RegisterModelsInAssembly (aArgs.NewAssembly, cache);
		}
		
		/// <summary>
		/// Current assembly wide search for the type, type will be found even
		/// if this assembly doesn't reference to the assembly where type resides
		/// As long as type is loaded, it will found it no matter what.
		/// </summary>
		/// <param name="aAssembly">
		/// Assembly to check in <see cref="Assembly"/>
		/// </param>
		/// <param name="aCache">
		/// Cache of already checked libraries <see cref="StringCollection"/>
		/// </param>
		/// <returns>
		/// null if unknown or Type if found <see cref="System.Type"/>
		/// </returns>
		internal static void RegisterModelsInAssembly (Assembly aAssembly, StringCollection aCache)
		{
			if ((aAssembly == null) || (aCache.Contains(aAssembly.GetName().Name) == true))
				return;

			aCache.Add (aAssembly.GetName().Name);
			// Find type in assembly
			if (assembliesProcessed.Contains(aAssembly.GetName().Name) == false) {
				foreach (System.Type type in aAssembly.GetTypes())
					if (type.IsClass == true)
						IsQueryModel(type);
				assembliesProcessed.Add (aAssembly.GetName().Name);
			}
			
			// Go trough assembly referenced assemblies
			foreach (AssemblyName mod in aAssembly.GetReferencedAssemblies())
				if (mod != null)
					RegisterModelsInAssembly (Assembly.Load(mod), aCache);
		}
		
		/// <summary>
		/// Checks if specified type is query model, then checks if model is already registered.
		/// Next checks for duplicate type handler and if everything is successful registers it
		/// </summary>
		/// <param name="aType">
		/// Type which is checked <see cref="System.Type"/>
		/// </param>
		/// <returns>
		/// true if it is model handler, false if not <see cref="System.Boolean"/>
		/// </returns>
		/// <remarks>
		/// If registered model for specified type already exists, then it throws
		/// ExceptionDuplicateQueryModelTypeHandler exception
		/// </remarks>
		public static bool IsQueryModel (System.Type aType)
		{
			if (aType == null)
				return (false);
			QueryModelAttribute[] attrs = (QueryModelAttribute[]) aType.GetCustomAttributes (typeof(QueryModelAttribute), false);
			if ((attrs == null) || (attrs.Length == 0))
				return (false);
			QueryModelAttribute attr = attrs[0];
			// Check if model is already registered
			if (models != null) {
				foreach (QueryModelDescription desc in models)
					if (desc != null)
						if (desc.Model == aType)
							return (true);
				
				// Check if duplicate handler exists
				foreach (QueryModelDescription desc in models)
					if (desc != null)
						if (desc.Definition.ListType == attr.ListType)
							throw new ExceptionDuplicateQueryModelTypeHandler (aType, attr.ListType);
			}

			models.Add (new QueryModelDescription (aType, attr));
			return (true);
		}
		
		/// <summary>
		/// Checks if specified type is query model, then checks if model is already registered.
		/// Next checks for duplicate type handler and if everything is successful registers it
		/// </summary>
		/// <param name="aType">
		/// Type which is checked <see cref="System.Type"/>
		/// </param>
		/// <returns>
		/// true if it is model handler, false if not <see cref="System.Boolean"/>
		/// </returns>
		/// <remarks>
		/// If registered model for specified type already exists, then it throws
		/// ExceptionDuplicateQueryModelTypeHandler exception
		/// </remarks>
		public static bool QueryModelExists (System.Type aType)
		{
			if (aType == null)
				return (false);
			// Check if model is already registered
			if (models != null) {
				foreach (QueryModelDescription desc in models)
					if (desc != null)
						if (desc.Definition.ListType == aType)
							return (true);
			}

			return (false);
		}
		
		/// <summary>
		/// Creates query implementor for specific class, first turn of checking is
		/// restricted to direct type handling, second turn checks for model that
		/// handles this type with inheritance in mind
		/// </summary>
		/// <param name="aOwnerModel">
		/// A <see cref="MappingImplementor"/>
		/// </param>
		/// <param name="aDataSource">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// A <see cref="QueryImplementor"/>
		/// </returns>
		public static QueryImplementor CreateModelFor (MappingsImplementor aOwnerModel, object aDataSource)
		{
			if (aDataSource == null)
				return (null);
			
			// Check if explicit handler exists
			foreach (QueryModelDescription desc in models)
				if (desc.Definition.ListType == aDataSource.GetType())
					return (desc.CreateModel (aOwnerModel));
			
			// Check if generic handler exists
			foreach (QueryModelDescription desc in models)
				if (desc.HandlesType(aDataSource.GetType()) == true)
					return (desc.CreateModel (aOwnerModel));
			
			return (null);
		}
	}
}
