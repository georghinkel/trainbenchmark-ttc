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
                sw.WriteLine("use adapton::engine::*;");
                sw.WriteLine();
                sw.WriteLine("use std::hash::Hash;");
                sw.WriteLine("use std::rc::Rc;");
                sw.WriteLine("use std::fmt::Debug;");
                sw.WriteLine("use std::fs::File;");
                sw.WriteLine("use std::io::BufReader;");
                sw.WriteLine("use std::ops::Deref;");
                sw.WriteLine();
                sw.WriteLine("use xml::reader::{ EventReader, XmlEvent};");
                sw.WriteLine("use xml::name::OwnedName;");
                sw.WriteLine("use xml::attribute::OwnedAttribute;");
                sw.WriteLine();
                sw.WriteLine("// generated enums");
                foreach (var en in package.EClassifiers.OfType<IEEnum>())
                {
                    GenerateEnum(en, sw);
                    sw.WriteLine();
                }

                sw.WriteLine("// generated traits");
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

                sw.WriteLine("// generated structs");
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

                sw.WriteLine("// generate implementations");
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

                sw.WriteLine("// generated parser");
                GenerateParserState(package.EClassifiers.OfType<IEClass>(), package.NsPrefix, sw);
            }
        }

        private static void GenerateImpl(IEClass type, string target, List<IEClass> derived, StreamWriter sw)
        {
            sw.WriteLine($"impl {type.Name.ToPascalCase()} for {target} {{");
            GenerateFeatureImplementations(type, target, derived, sw);
            sw.WriteLine("}");
            foreach (var baseType in type.ESuperTypes)
            {
                GenerateImpl(baseType, target, derived, sw);
            }
        }

        private static void GenerateEnum(IEClass type, List<IEClass> derived, StreamWriter sw)
        {
            sw.WriteLine("#[derive(Debug)]");
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
            sw.WriteLine("#[derive(Debug, Default)]");
            sw.WriteLine($"pub struct {GetImplementationName(type)} {{");
            var first = true;
            GenerateFields(type, sw, false, ref first);
            sw.WriteLine();
            sw.WriteLine("}");
        }

        private static void GenerateFields(IEClass type, StreamWriter sw, bool initialize, ref bool first)
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
                GenerateFields(baseType, sw, initialize, ref first);
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
                sw.WriteLine($"    fn get_{feature.Name}(&self) -> {GetTypeName(feature, mutable: false)} {{");
                if (r != null)
                {
                    sw.WriteLine($"        match self.{feature.Name} {{");
                    sw.WriteLine("            None => None,");
                    sw.WriteLine("            Some(ref val) => Some(val.clone()),");
                    sw.WriteLine("        }");
                }
                else
                {
                    sw.WriteLine($"        self.{feature.Name}.clone()");
                }
                sw.WriteLine("    }");
                sw.WriteLine($"    fn set_{feature.Name}(&mut self, value: {GetTypeName(feature)}) {{");
                sw.WriteLine($"        self.{feature.Name} = value");
                sw.WriteLine("    }");
            }
            else
            {
                sw.WriteLine($"    fn get_{feature.Name}(&mut self) -> &mut {GetTypeName(feature)} {{");
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
                sw.WriteLine($"    fn get_{feature.Name}(&self) -> {GetTypeName(feature, mutable: false)} {{");
                sw.WriteLine($"        match *self {{");
                foreach (var impl in derived)
                {
                    sw.WriteLine($"            {target}::{impl.Name.ToPascalCase()}(ref i) => i.get_{feature.Name}(),");
                }
                sw.WriteLine($"        }}");
                sw.WriteLine("    }");
                sw.WriteLine($"    fn set_{feature.Name}(&mut self, value: {GetTypeName(feature)}) {{");
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
                sw.WriteLine($"    fn get_{feature.Name}(&mut self) -> &mut {GetTypeName(feature)} {{");
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
            sw.Write($"pub trait {type.Name.ToPascalCase()} : ");
            foreach (var baseType in type.ESuperTypes)
            {
                sw.Write(baseType.Name.ToPascalCase());
                sw.Write(" + ");
            }
            sw.WriteLine("Debug {");
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
                sw.WriteLine($"    fn get_{feature.Name}(&self) -> {GetTypeName(feature, mutable: false)};");
                sw.WriteLine($"    fn set_{feature.Name}(&mut self, value: {GetTypeName(feature)});");
            }
            else
            {
                sw.WriteLine($"    fn get_{feature.Name}(&mut self) -> &mut {GetTypeName(feature)};");
            }
        }

        private static string GetTypeName(IEStructuralFeature feature, bool lifetime = false, bool mutable = true)
        {
            var dataType = feature.EType as EDataType;
            var boxType = feature.UpperBound.GetValueOrDefault(1) != 1 ? "Vec" : "Option";
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
                    return $"{boxType}<Rc<Box<{cls.Name.ToPascalCase()}>>>";
                }
                else
                {
                    return $"{boxType}<Rc<Box<{cls.Name.ToPascalCase()}>>>";
                }
            }
            var en = feature.EType as IEEnum;
            if (en != null)
            {
                return $"{boxType}<{en.Name.ToPascalCase()}>";
            }
            throw new ArgumentOutOfRangeException();
        }

        private static void GenerateParserState(IEnumerable<IEClass> classes, string prefix, StreamWriter sw)
        {
            sw.WriteLine(@"fn find_type<'a>(attributes : &'a Vec<OwnedAttribute>, default : &'a String) -> &'a String {
	for att in attributes {
		match att.name.prefix {
			Some(ref prefix) => {
				if prefix == ""xsi"" && att.name.local_name == ""type"" {
                    return &att.value
                }
            },
			None => continue
		}
	}
	return default;
}");
            sw.WriteLine("#[derive(Clone)]");
            sw.WriteLine("enum ParserState {");
            foreach (var cl in classes)
            {
                if (cl.Abstract.GetValueOrDefault(false)) continue;
                sw.WriteLine($"    {cl.Name.ToPascalCase()}(Rc<Box<{cl.Name.ToPascalCase()}>>),");
            }
            sw.WriteLine("}");
            sw.WriteLine("impl ParserState {");
            sw.WriteLine("    fn push(reference : &String, stack : &mut Vec<ParserState>) {");
            foreach (var cl in classes)
            {
                if (cl.Abstract.GetValueOrDefault(false)) continue;
                sw.WriteLine($"        if reference == \"{prefix}:{cl.Name.ToPascalCase()}\" {{");
                sw.WriteLine($"            let element : Rc<Box<{cl.Name.ToPascalCase()}>> = Rc::new(Box::new({cl.Name.ToPascalCase()}Impl::default()));");
                sw.WriteLine($"            stack.push(ParserState::{cl.Name.ToPascalCase()}(element));");
                sw.WriteLine("        }");
            }
            sw.WriteLine("    }");
            sw.WriteLine("    fn parse(&self, event : XmlEvent, stack : &mut Vec<ParserState>)");
            sw.WriteLine("    {");
            sw.WriteLine("        match event {");
            sw.WriteLine("            XmlEvent::EndElement { name } => {");
            sw.WriteLine("                if stack.len() > 1 {");
            sw.WriteLine("                    stack.pop();");
            sw.WriteLine("                }");
            sw.WriteLine("            }");
            sw.WriteLine("            XmlEvent::StartElement { name, attributes, namespace } => {");
            sw.WriteLine("                match *self {");
            foreach (var cl in classes)
            {
                if (cl.Abstract.GetValueOrDefault(false)) continue;
                sw.WriteLine($"                    ParserState::{cl.Name.ToPascalCase()}(ref element) => {{");
                foreach (var r in cl.EStructuralFeatures.OfType<IEReference>())
                {
                    if (r.Containment.GetValueOrDefault(false))
                    {
                        sw.WriteLine($"                        if name.local_name == \"{r.Name}\" {{");
                        sw.WriteLine($"                            let r_name = String::from(\"{prefix}:{r.EType.Name.ToPascalCase()}\");");
                        sw.WriteLine($"                            ParserState::push(find_type(&attributes, &r_name), stack);");
                        sw.WriteLine("                            return;");
                        sw.WriteLine("                        }");
                    }
                }
                sw.WriteLine("                        panic!(\"Unexpected element found\");");
                sw.WriteLine("                    }");
            }
            sw.WriteLine("                }");
            sw.WriteLine("            }");
            sw.WriteLine("            _ => {},");
            sw.WriteLine("        }");
            sw.WriteLine("    }");
            sw.WriteLine("}");

            sw.WriteLine(@"pub fn load(file_name :String) {
    let file = File::open(file_name).unwrap();
    let file = BufReader::new(file);

    let parser = EventReader::new(file);
    let mut container = RailwayContainerImpl::default();
    let mut state : ParserState;
    for e in parser {
        match e {
            Ok(XmlEvent::StartElement { name, attributes, namespace }) => {
            }
            _ => {}
        }
    }
}");
        }
    }
}
