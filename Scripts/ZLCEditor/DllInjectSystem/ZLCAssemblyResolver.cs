using System;
using System.Collections.Generic;
using Mono.Cecil;
namespace ZLCEditor.DllInjectSystem
{
    public class ZLCAssemblyResolver : BaseAssemblyResolver
    {
        private readonly IDictionary<string, AssemblyDefinition> cache;

        public ZLCAssemblyResolver()
        {
            cache = new Dictionary<string, AssemblyDefinition>(StringComparer.Ordinal);
        }

        public override AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            Mixin.CheckName(name);
            AssemblyDefinition assemblyDefinition1;
            if (cache.TryGetValue(name.FullName, out assemblyDefinition1))
                return assemblyDefinition1;
            AssemblyDefinition assemblyDefinition2 = base.Resolve(name);
            cache[name.FullName] = assemblyDefinition2;
            return assemblyDefinition2;
        }

        protected void RegisterAssembly(AssemblyDefinition assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            string fullName = assembly.Name.FullName;
            if (cache.ContainsKey(fullName))
                return;
            cache[fullName] = assembly;
        }

        protected override void Dispose(bool disposing)
        {
            foreach (AssemblyDefinition assemblyDefinition in cache.Values)
                assemblyDefinition.Dispose();
            cache.Clear();
            base.Dispose(disposing);
        }
    }
}