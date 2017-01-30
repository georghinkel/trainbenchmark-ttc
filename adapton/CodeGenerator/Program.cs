using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NMF.Interop.Ecore;
using NMF.Utilities;

namespace CodeGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var package = EcoreInterop.LoadPackageFromFile(@"..\..\..\railway.ecore");
            var deriving = new Dictionary<IEClass, List<IEClass>>();

            using (var sw = new StreamWriter(@"..\..\..\solution\src\railway.rs"))
            {
                sw.WriteLine("use std::hash::Hash;");
                sw.WriteLine("use std::marker::PhantomData;");
                sw.WriteLine();
                foreach (var en in package.EClassifiers.OfType<IEEnum>())
                {
                    GenerateEnum(en, sw);
                    sw.WriteLine();
                }

                // generate traits
                foreach (var type in package.EClassifiers.OfType<IEClass>())
                {
                    GenerateTrait(type, sw);
                    sw.WriteLine();
                    foreach (var baseType in type.ESuperTypes)
                    {
                        List<IEClass> derived;
                        if (!deriving.TryGetValue(baseType, out derived))
                        {
                            derived = new List<IEClass>();
                            deriving.Add(baseType, derived);
                        }
                        derived.Add(type);
                    }
                }

                // generate structs
                foreach (var type in package.EClassifiers.OfType<IEClass>())
                {
                    List<IEClass> derived;
                    if (!deriving.TryGetValue(type, out derived) || derived.Count == 0)
                    {
                        GenerateStruct(type, sw);
                    }
                    else
                    {
                        GenerateEnum(type, derived, sw);
                    }
                    sw.WriteLine();
                }

                // generate implementations
                foreach (var type in package.EClassifiers.OfType<IEClass>())
                {
                    List<IEClass> derived;
                    if (!deriving.TryGetValue(type, out derived) || derived.Count == 0)
                    {
                        GenerateImpl(type, GetImplementationName(type), sw);
                    }
                    else
                    {
                        GenerateImpl(type, GetImplementationName(type), derived, sw);
                    }
                    sw.WriteLine();
                }
            }
        }

        private static void GenerateImpl(IEClass type, string target, List<IEClass> derived, StreamWriter sw)
        {
            sw.WriteLine($"impl <'a> {type.Name.ToPascalCase()}<'a> for {target}<'a> {{");
            GenerateFeatureImplementations(type, target, derived, sw);
            sw.WriteLine("}");
            foreach (var baseType in type.ESuperTypes)
            {
                GenerateImpl(baseType, target, derived, sw);
            }
        }

        private static void GenerateEnum(IEClass type, List<IEClass> derived, StreamWriter sw)
        {
            sw.WriteLine("#[derive(Eq, PartialEq, Hash, Debug)]");
            sw.WriteLine($"pub enum {GetImplementationName(type)}<'a> {{");
            sw.Write($"    {derived[0].Name}({GetImplementationName(derived[0])}<'a>)");
            foreach (var subType in derived.Skip(1))
            {
                sw.WriteLine(",");
                sw.Write($"    {subType.Name}({GetImplementationName(subType)}<'a>)");
            }
            sw.WriteLine();
            sw.WriteLine("}");
        }

        private static void GenerateEnum(IEEnum en, StreamWriter sw)
        {
            sw.WriteLine("#[derive(Eq, PartialEq, Clone, Copy, Hash, Debug)]");
            sw.WriteLine($"pub enum {en.Name.ToPascalCase()} {{");
            var first = true;
            foreach (var lit in en.ELiterals)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sw.WriteLine(",");
                }
                sw.Write($"    {lit.Name}");
            }
            sw.WriteLine();
            sw.WriteLine("}");
        }

        private static string GetImplementationName(IEClass type)
        {
            return type.Name.ToPascalCase() + "Impl";
        }

        private static void GenerateStruct(IEClass type, StreamWriter sw)
        {
            sw.WriteLine("#[derive(Eq, PartialEq, Hash, Debug, Default)]");
            sw.WriteLine($"pub struct {GetImplementationName(type)}<'a> {{");
            var first = true;
            var lifetime_needed = false;
            GenerateFields(type, sw, false, ref first, ref lifetime_needed);
            if (!lifetime_needed)
            {
                if (!first) sw.WriteLine(",");
                sw.Write("    phantom: PhantomData<&'a String>");
            }
            sw.WriteLine();
            sw.WriteLine("}");
        }

        private static void GenerateFields(IEClass type, StreamWriter sw, bool initialize, ref bool first, ref bool lifetime_needed)
        {
            foreach (var feature in type.EStructuralFeatures)
            {
                var r = feature as IEReference;
                if (r != null && r.EOpposite != null && r.EOpposite.Containment.GetValueOrDefault(false))
                {
                    continue;
                }
                if (first)
                {
                    first = false;
                }
                else
                {
                    sw.WriteLine(",");
                }
                if (r != null)
                {
                    lifetime_needed = true;
                }
                sw.Write("    ");
                if (initialize)
                {
                    GenerateInitialization(feature, sw);
                }
                else
                {
                    GenerateFeature(feature, sw);
                }
            }
            foreach (var baseType in type.ESuperTypes)
            {
                GenerateFields(baseType, sw, initialize, ref first, ref lifetime_needed);
            }
        }

        private static void GenerateInitialization(IEStructuralFeature feature, StreamWriter sw)
        {
            sw.Write($"{feature.Name} : ");
            if (feature.UpperBound.GetValueOrDefault(1) == 1)
            {
                sw.Write("None");
            }
            else
            {
                sw.Write("Vec::new()");
            }
        }

        private static void GenerateFeature(IEStructuralFeature feature, StreamWriter sw)
        {
            if (feature.UpperBound.GetValueOrDefault(1) == 1)
            {
                sw.Write($"{feature.Name}: {GetTypeName(feature, lifetime: true)}");
            }
            else
            {
                sw.Write($"{feature.Name}: {GetTypeName(feature, lifetime: true)}");
            }
        }

        private static void GenerateImpl(IEClass type, string target, StreamWriter sw)
        {
            sw.WriteLine($"impl <'a> {type.Name.ToPascalCase()}<'a> for {target}<'a> {{");
            GenerateFeatureImplementations(type, target, sw);
            sw.WriteLine("}");
            foreach (var baseType in type.ESuperTypes)
            {
                GenerateImpl(baseType, target, sw);
            }
        }

        private static void GenerateFeatureImplementations(IEClass type, string target, StreamWriter sw)
        {
            foreach (var feature in type.EStructuralFeatures)
            {
                var r = feature as IEReference;
                if (r != null && r.EOpposite != null && r.EOpposite.Containment.GetValueOrDefault(false))
                {
                    continue;
                }
                GenerateFeatureImplementation(feature, sw);
            }
        }

        private static void GenerateFeatureImplementation(IEStructuralFeature feature, StreamWriter sw)
        {
            var r = feature as IEReference;
            if (feature.UpperBound.GetValueOrDefault(1) == 1)
            {
                sw.WriteLine($"    fn get_{feature.Name}(&'a self) -> {GetTypeName(feature, mutable: false)} {{");
                if (r != null)
                {
                    sw.WriteLine($"        match self.{feature.Name} {{");
                    sw.WriteLine("            None => None,");
                    sw.WriteLine("            Some(ref val) => Some(val),");
                    sw.WriteLine("        }");
                }
                else
                {
                    sw.WriteLine($"        self.{feature.Name}.clone()");
                }
                sw.WriteLine("    }");
                sw.WriteLine($"    fn set_{feature.Name}(&'a mut self, value: {GetTypeName(feature)}) {{");
                sw.WriteLine($"        self.{feature.Name} = value");
                sw.WriteLine("    }");
            }
            else
            {
                sw.WriteLine($"    fn get_{feature.Name}(&'a mut self) -> &mut {GetTypeName(feature)} {{");
                sw.WriteLine($"        &mut self.{feature.Name}");
                sw.WriteLine("    }");
            }
        }

        private static void GenerateFeatureImplementations(IEClass type, string target, List<IEClass> derived, StreamWriter sw)
        {
            foreach (var feature in type.EStructuralFeatures)
            {
                var r = feature as IEReference;
                if (r != null && r.EOpposite != null && r.EOpposite.Containment.GetValueOrDefault(false))
                {
                    continue;
                }
                GenerateFeatureImplementation(feature, derived, target, sw);
            }
        }

        private static void GenerateFeatureImplementation(IEStructuralFeature feature, List<IEClass> derived, string target, StreamWriter sw)
        {
            var r = feature as IEReference;
            if (feature.UpperBound.GetValueOrDefault(1) == 1)
            {
                sw.WriteLine($"    fn get_{feature.Name}(&'a self) -> {GetTypeName(feature, mutable: false)} {{");
                sw.WriteLine($"        match *self {{");
                foreach (var impl in derived)
                {
                    sw.WriteLine($"            {target}::{impl.Name.ToPascalCase()}(ref i) => i.get_{feature.Name}(),");
                }
                sw.WriteLine($"        }}");
                sw.WriteLine("    }");
                sw.WriteLine($"    fn set_{feature.Name}(&'a mut self, value: {GetTypeName(feature)}) {{");
                sw.WriteLine($"        match *self {{");
                foreach (var impl in derived)
                {
                    sw.WriteLine($"            {target}::{impl.Name.ToPascalCase()}(ref mut i) => i.set_{feature.Name}(value),");
                }
                sw.WriteLine($"        }}");
                sw.WriteLine("    }");
            }
            else
            {
                sw.WriteLine($"    fn get_{feature.Name}(&'a mut self) -> &mut {GetTypeName(feature)} {{");
                sw.WriteLine($"        match *self {{");
                foreach (var impl in derived)
                {
                    sw.WriteLine($"            {target}::{impl.Name.ToPascalCase()}(ref mut i) => i.get_{feature.Name}(),");
                }
                sw.WriteLine($"        }}");
                sw.WriteLine("    }");
            }
        }

        private static void GenerateTrait(IEClass type, StreamWriter sw)
        {
            sw.Write($"pub trait {type.Name.ToPascalCase()}<'a> : ");
            foreach (var baseType in type.ESuperTypes)
            {
                sw.Write(baseType.Name.ToPascalCase() + "<'a>");
                sw.Write(" + ");
            }
            sw.WriteLine("Eq + PartialEq + Hash {");
            foreach (var feature in type.EStructuralFeatures)
            {
                var r = feature as IEReference;
                if (r != null && r.EOpposite != null && r.EOpposite.Containment.GetValueOrDefault(false))
                {
                    continue;
                }
                GenerateFeatureTrait(feature, sw);
            }
            sw.WriteLine("}");
        }

        private static void GenerateFeatureTrait(IEStructuralFeature feature, StreamWriter sw)
        {
            if (feature.UpperBound.GetValueOrDefault(1) == 1)
            {
                sw.WriteLine($"    fn get_{feature.Name}(&'a self) -> {GetTypeName(feature, mutable: false)};");
                sw.WriteLine($"    fn set_{feature.Name}(&'a mut self, value: {GetTypeName(feature)});");
            }
            else
            {
                sw.WriteLine($"    fn get_{feature.Name}(&'a mut self) -> &'a mut {GetTypeName(feature)};");
            }
        }

        private static string GetTypeName(IEStructuralFeature feature, bool lifetime = false, bool mutable = true)
        {
            var dataType = feature.EType as EDataType;
            var boxType = feature.UpperBound.GetValueOrDefault(1) != 1 ? "Vec" : "Option";
            var lifetimeSpec = "'a";
            if (mutable) lifetimeSpec += " mut";
            var lifetimePost = lifetime ? "<'a>" : "";
            if (dataType != null)
            {
                switch (dataType.InstanceClassName)
                {
                    case "java.lang.String":
                        return $"{boxType}<&String>";
                    case "int":
                        return $"{boxType}<i32>";
                    default:
                        break;
                }
            }
            var cls = feature.EType as IEClass;
            if (cls != null)
            {
                var r = feature as IEReference;
                if (r != null && r.Containment.GetValueOrDefault(false))
                {
                    return $"{boxType}<{GetImplementationName(cls)}<'a>>";
                }
                else
                {
                    return $"{boxType}<&{lifetimeSpec} {GetImplementationName(cls)}<'a>>";
                }
            }
            var en = feature.EType as IEEnum;
            if (en != null)
            {
                return $"{boxType}<{en.Name.ToPascalCase()}>";
            }
            throw new ArgumentOutOfRangeException();
        }
    }
}
