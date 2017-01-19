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
                foreach (var en in package.EClassifiers.OfType<IEEnum>())
                {
                    GenerateEnum(en, sw);
                }

                // generate traits
                foreach (var type in package.EClassifiers.OfType<IEClass>())
                {
                    GenerateTrait(type, sw);
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
                }
            }
        }

        private static void GenerateImpl(IEClass type, string target, List<IEClass> derived, StreamWriter sw)
        {
            sw.WriteLine($"impl {type.Name.ToPascalCase()} for {target} {{");
            GenerateFeatureImplementations(type, target, derived, sw);
            sw.WriteLine("}");
            foreach (var baseType in type.ESuperTypes)
            {
                GenerateImpl(baseType, target, sw);
            }
        }

        private static void GenerateFeatureImplementations(IEClass type, string target, List<IEClass> derived, StreamWriter sw)
        {
            foreach (var feature in type.EStructuralFeatures)
            {
                GenerateFeatureImplementation(feature, derived, target, sw);
            }
        }

        private static void GenerateFeatureImplementation(IEStructuralFeature feature, List<IEClass> derived, string target, StreamWriter sw)
        {
            if (feature.UpperBound.GetValueOrDefault(1) == 1)
            {
                sw.WriteLine($"    fn get_{feature.Name}(&self) -> {GetTypeName(feature.EType)} {{");
                sw.WriteLine($"        match self* {{");
                foreach (var impl in derived)
                {
                    sw.WriteLine($"            {target}::{impl.Name.ToPascalCase()}(i) => i.{feature.Name}");
                }
                sw.WriteLine($"        }}");
                sw.WriteLine("    }");
                sw.WriteLine($"    fn set_{feature.Name}(&mut self, value: {GetTypeName(feature.EType)}) {{");
                sw.WriteLine($"        match self* {{");
                foreach (var impl in derived)
                {
                    sw.WriteLine($"            {target}::{impl.Name.ToPascalCase()}(i) => i.{feature.Name} = value");
                }
                sw.WriteLine($"        }}");
                sw.WriteLine("    }");
            }
            else
            {
                sw.WriteLine($"    fn get_{feature.Name}(&self) -> Vec<{GetTypeName(feature.EType)}> {{");
                sw.WriteLine($"        match self* {{");
                foreach (var impl in derived)
                {
                    sw.WriteLine($"            {target}::{impl.Name.ToPascalCase()}(i) => i.{feature.Name}");
                }
                sw.WriteLine($"        }}");
                sw.WriteLine("    }");
            }
        }

        private static void GenerateEnum(IEClass type, List<IEClass> derived, StreamWriter sw)
        {
            sw.WriteLine($"pub enum {GetImplementationName(type)} {{");
            sw.Write($"    {derived[0].Name}({GetImplementationName(derived[0])})");
            foreach (var subType in derived.Skip(1))
            {
                sw.WriteLine(",");
                sw.Write($"    {subType.Name}({GetImplementationName(subType)})");
            }
            sw.WriteLine();
            sw.WriteLine("}");
        }

        private static void GenerateEnum(IEEnum en, StreamWriter sw)
        {
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
            sw.Write($"pub struct {GetImplementationName(type)} {{ ");
            var first = true;
            GenerateFields(type, sw, ref first);
            sw.WriteLine("}");
        }

        private static void GenerateFields(IEClass type, StreamWriter sw, ref bool first)
        {
            foreach (var feature in type.EStructuralFeatures)
            {
                GenerateFeature(feature, sw, ref first);
            }
            foreach (var baseType in type.ESuperTypes)
            {
                GenerateFields(baseType, sw, ref first);
            }
        }

        private static void GenerateFeature(IEStructuralFeature feature, StreamWriter sw, ref bool first)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                sw.Write(", ");
            }
            if (feature.UpperBound.GetValueOrDefault(1) == 1)
            {
                sw.Write($"{feature.Name}: {GetTypeName(feature.EType)}");
            }
            else
            {
                sw.Write($"{feature.Name}: Vec<{GetTypeName(feature.EType)}>");
            }
        }

        private static void GenerateImpl(IEClass type, string target, StreamWriter sw)
        {
            sw.WriteLine($"impl {type.Name.ToPascalCase()} for {target} {{");
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
                GenerateFeatureImplementation(feature, sw);
            }
        }

        private static void GenerateFeatureImplementation(IEStructuralFeature feature, StreamWriter sw)
        {
            if (feature.UpperBound.GetValueOrDefault(1) == 1)
            {
                sw.WriteLine($"    fn get_{feature.Name}(&self) -> {GetTypeName(feature.EType)} {{");
                sw.WriteLine($"        self.{feature.Name}");
                sw.WriteLine("    }");
                sw.WriteLine($"    fn set_{feature.Name}(&mut self, value: {GetTypeName(feature.EType)}) {{");
                sw.WriteLine($"        self.{feature.Name} = value");
                sw.WriteLine("    }");
            }
            else
            {
                sw.WriteLine($"    fn get_{feature.Name}(&self) -> Vec<{GetTypeName(feature.EType)}> {{");
                sw.WriteLine($"        self.{feature.Name}");
                sw.WriteLine("    }");
            }
        }

        private static void GenerateTrait(IEClass type, StreamWriter sw)
        {
            sw.WriteLine($"pub trait {type.Name.ToPascalCase()} {{");
            foreach (var feature in type.EStructuralFeatures)
            {
                GenerateFeatureTrait(feature, sw);
            }
            sw.WriteLine("}");
        }

        private static void GenerateFeatureTrait(IEStructuralFeature feature, StreamWriter sw)
        {
            if (feature.UpperBound.GetValueOrDefault(1) == 1)
            {
                sw.WriteLine($"    fn get_{feature.Name}(&self) -> {GetTypeName(feature.EType)};");
                sw.WriteLine($"    fn set_{feature.Name}(&mut self, value: {GetTypeName(feature.EType)});");
            }
            else
            {
                sw.WriteLine($"    fn get_{feature.Name}(&self) -> Vec<{GetTypeName(feature.EType)}>;");
            }
        }

        private static string GetTypeName(IEClassifier eType)
        {
            var dataType = eType as EDataType;
            if (dataType != null)
            {
                switch (dataType.InstanceClassName)
                {
                    case "java.lang.String":
                        return "&'static str";
                    case "int":
                        return "i32";
                    default:
                        break;
                }
            }
            var cls = eType as IEClass;
            if (cls != null)
            {
                return $"Box<{cls.Name.ToPascalCase()}>";
            }
            var en = eType as IEEnum;
            if (en != null)
            {
                return en.Name.ToPascalCase();
            }
            throw new ArgumentOutOfRangeException();
        }
    }
}
