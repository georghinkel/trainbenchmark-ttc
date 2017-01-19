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

            using (var sw = new StreamWriter(@"..\..\..\solution\railway.rs"))
            {
                // generate traits
                foreach (var type in package.EClassifiers.OfType<IEClass>())
                {
                    GenerateTrait(type, sw);
                }

                // generate structs
                foreach (var type in package.EClassifiers.OfType<IEClass>())
                {
                    if (type.Abstract.GetValueOrDefault(false))
                    {
                        GenerateStruct(type, sw);
                    }
                }

                // generate implementations
                foreach (var type in package.EClassifiers.OfType<IEClass>())
                {
                    if (type.Abstract.GetValueOrDefault(false))
                    {
                        GenerateImpl(type, GetImplementationName(type), sw);
                    }
                }
            }
        }

        private static string GetImplementationName(IEClass type)
        {
            return type.Name.ToPascalCase() + "Impl";
        }

        private static void GenerateStruct(IEClass type, StreamWriter sw)
        {
            sw.WriteLine($"pub struct {GetImplementationName(type)} {{");
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
                sw.Write($"{feature.Name}: vec<{GetTypeName(feature.EType)}>");
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
                sw.WriteLine($"    fn get_{feature.Name}(&self) -> vec<{GetTypeName(feature.EType)}> {{");
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
                sw.WriteLine($"    fn get_{feature.Name}(&self) -> vec<{GetTypeName(feature.EType)}>;");
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
                    default:
                        break;
                }
            }
            return null;
        }
    }
}
