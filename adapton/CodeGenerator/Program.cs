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
    class Path : IEquatable<Path>
    {
        public Path(List<IEReference> references)
        {
            Segments = references;
        }

        public List<IEReference> Segments { get; private set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Path);
        }

        public bool Equals(Path other)
        {
            if (other == null) return false;
            return Segments.SequenceEqual(other.Segments);
        }

        public override int GetHashCode()
        {
            var hash = 0;
            foreach (var seg in Segments)
            {
                hash ^= seg.GetHashCode();
            }
            return hash;
        }

        public override string ToString()
        {
            return string.Join("", Segments.Select(r => r.Name.ToPascalCase()));
        }

        public string PrintRegex()
        {
            var sb = new StringBuilder();
            sb.Append("^#//");
            foreach (var r in Segments)
            {
                sb.Append(r.Name);
                if (r.UpperBound != 1)
                {
                    sb.Append(@"\.(\d+)");
                }
                sb.Append("/");
            }
            sb.Append("$");
            return sb.ToString();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var package = EcoreInterop.LoadPackageFromFile(@"..\..\..\railway.ecore");
            var subTypes = new Dictionary<IEClass, List<IEClass>>();
            var root = package.EClassifiers.FirstOrDefault(c => c.Name == "RailwayContainer") as IEClass;
            var paths = new Dictionary<IEClass, List<Path>>();

            PopulateDerivingAndPaths(package, root, subTypes, paths);

            using (var sw = new StreamWriter(@"..\..\..\solution\src\railway.rs"))
            {
                sw.WriteLine("#[macro_use] extern crate adapton;");
                sw.WriteLine("#[macro_use] extern crate lazy_static;");
                sw.WriteLine("extern crate regex;");
                sw.WriteLine();
                sw.WriteLine();
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
                sw.WriteLine("use regex::Regex;");
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
                    List<IEClass> derived;
                    subTypes.TryGetValue(type, out derived);
                    GenerateTrait(type, sw, derived);
                    sw.WriteLine();
                }

                sw.WriteLine("// generated structs");
                foreach (var type in package.EClassifiers.OfType<IEClass>())
                {
                    List<IEClass> derived;
                    if (!subTypes.TryGetValue(type, out derived) || derived.Count == 0)
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
                    if (!subTypes.TryGetValue(type, out derived) || derived.Count == 0)
                    {
                        GenerateStructImpl(type, null, GetImplementationName(type), subTypes, sw);
                    }
                    else
                    {
                        GenerateEnumImpl(type, null, GetImplementationName(type), derived, subTypes, sw);
                    }
                    sw.WriteLine();
                }

                sw.WriteLine("// generated parser");
                GenerateParserState(package.EClassifiers.OfType<IEClass>(), package.NsPrefix, root, paths, sw);
            }
        }

        private static void PopulateDerivingAndPaths(EPackage package, IEClass rc, Dictionary<IEClass, List<IEClass>> deriving, Dictionary<IEClass, List<Path>> paths)
        {
            foreach (var type in package.EClassifiers.OfType<IEClass>())
            {
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

            var currentPath = new List<IEReference>();
            PopulatePaths(rc, currentPath, paths, deriving);
        }

        private static void PopulatePaths(IEClass rc, List<IEReference> currentPath, Dictionary<IEClass, List<Path>> paths, Dictionary<IEClass, List<IEClass>> deriving)
        {
            foreach (var r in rc.EStructuralFeatures.OfType<IEReference>())
            {
                if (!r.Containment.GetValueOrDefault(false)) continue;
                var targetType = r.EType as IEClass;
                var list = new List<IEReference>(currentPath);
                list.Add(r);
                AddPath(list, paths, deriving, r, targetType);
            }
        }

        private static void AddPath(List<IEReference> currentPath, Dictionary<IEClass, List<Path>> paths, Dictionary<IEClass, List<IEClass>> deriving, IEReference r, IEClass targetType)
        {
            List<Path> pathsOfType;
            if (!paths.TryGetValue(targetType, out pathsOfType))
            {
                pathsOfType = new List<Path>();
                paths.Add(targetType, pathsOfType);
            }
            var path = new Path(currentPath);
            pathsOfType.Add(path);
            PopulatePaths(targetType, currentPath, paths, deriving);
            List<IEClass> derived;
            if (deriving.TryGetValue(targetType, out derived))
            {
                foreach (var subType in derived)
                {
                    AddPath(currentPath, paths, deriving, r, subType);
                }
            }
        }

        private static void GenerateEnumImpl(IEClass type, IEClass last, string target, List<IEClass> derived, Dictionary<IEClass, List<IEClass>> subTypes, StreamWriter sw)
        {
            sw.WriteLine($"impl {type.Name.ToPascalCase()} for {target} {{");
            GenerateFeatureImplementations(type, target, derived, sw);
            if (last != null)
            {
                List<IEClass> subTypesOfType;
                subTypes.TryGetValue(type, out subTypesOfType);
                if (subTypesOfType != null)
                {
                    foreach (var subType in subTypesOfType)
                    {
                        var val = subType == last ? "Some(self)" : "None";
                        sw.WriteLine($"    fn cast_{subType.Name.ToCamelCase()}(&self) -> Option<&{subType.Name.ToPascalCase()}> {{ {val} }}");
                        sw.WriteLine($"    fn cast_{subType.Name.ToCamelCase()}_mut(&mut self) -> Option<&mut {subType.Name.ToPascalCase()}> {{ {val} }}");
                    }
                }
            }
            else
            {
                foreach (var subType in derived)
                {
                    sw.WriteLine($"    fn cast_{subType.Name.ToCamelCase()}(&self) -> Option<&{subType.Name.ToPascalCase()}> {{");
                    sw.WriteLine($"        match *self {{");
                    foreach (var impl in derived)
                    {
                        var val = impl == subType ? "Some(self)" : "None";
                        sw.WriteLine($"            {target}::{impl.Name.ToPascalCase()}(ref i) => {val},");
                    }
                    sw.WriteLine($"        }}");
                    sw.WriteLine("    }");
                    sw.WriteLine($"    fn cast_{subType.Name.ToCamelCase()}_mut(&mut self) -> Option<&mut {subType.Name.ToPascalCase()}> {{");
                    sw.WriteLine($"        match *self {{");
                    foreach (var impl in derived)
                    {
                        var val = impl == subType ? "Some(self)" : "None";
                        sw.WriteLine($"            {target}::{impl.Name.ToPascalCase()}(ref i) => {val},");
                    }
                    sw.WriteLine($"        }}");
                    sw.WriteLine("    }");
                }
            }
            sw.WriteLine("}");
            foreach (var baseType in type.ESuperTypes)
            {
                GenerateEnumImpl(baseType, type, target, derived, subTypes, sw);
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

        private static void GenerateStructImpl(IEClass type, IEClass last, string target, Dictionary<IEClass, List<IEClass>> subTypes, StreamWriter sw)
        {
            sw.WriteLine($"impl {type.Name.ToPascalCase()} for {target} {{");
            GenerateFeatureImplementations(type, target, sw);
            List<IEClass> derived;
            subTypes.TryGetValue(type, out derived);
            if (derived != null)
            {
                foreach (var subType in derived)
                {
                    var val = subType == last ? "Some(self)" : "None";
                    sw.WriteLine($"    fn cast_{subType.Name.ToCamelCase()}(&self) -> Option<&{subType.Name.ToPascalCase()}> {{ {val} }}");
                    sw.WriteLine($"    fn cast_{subType.Name.ToCamelCase()}_mut(&mut self) -> Option<&mut {subType.Name.ToPascalCase()}> {{ {val} }}");
                }
            }
            sw.WriteLine("}");
            foreach (var baseType in type.ESuperTypes)
            {
                GenerateStructImpl(baseType, type, target, subTypes, sw);
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

        private static void GenerateTrait(IEClass type, StreamWriter sw, List<IEClass> derived)
        {
            sw.Write($"pub trait {type.Name.ToPascalCase()} : ");
            foreach (var baseType in type.ESuperTypes)
            {
                sw.Write(baseType.Name.ToPascalCase());
                sw.Write(" + ");
            }
            sw.WriteLine("Debug {");
            // features
            foreach (var feature in type.EStructuralFeatures)
            {
                var r = feature as IEReference;
                if (r != null && r.EOpposite != null && r.EOpposite.Containment.GetValueOrDefault(false))
                {
                    continue;
                }
                GenerateFeatureTrait(feature, sw);
            }
            // casts
            if (derived != null)
            {
                foreach (var subType in derived)
                {
                    sw.WriteLine($"    fn cast_{subType.Name.ToCamelCase()}(&self) -> Option<&{subType.Name.ToPascalCase()}>;");
                    sw.WriteLine($"    fn cast_{subType.Name.ToCamelCase()}_mut(&mut self) -> Option<&mut {subType.Name.ToPascalCase()}>;");
                }
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

        private static void GenerateParserState(IEnumerable<IEClass> classes, string prefix, IEClass root, Dictionary<IEClass, List<Path>> paths, StreamWriter sw)
        {
            sw.WriteLine(@"fn find_type<'a>(attributes : &'a Vec<OwnedAttribute>, default : &'a str) -> &'a str {
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
            sw.WriteLine("    Root,");
            foreach (var cl in classes)
            {
                if (cl.Abstract.GetValueOrDefault(false)) continue;
                sw.WriteLine($"    {cl.Name.ToPascalCase()}(Rc<Box<{cl.Name.ToPascalCase()}>>),");
            }
            sw.WriteLine("}");
            sw.WriteLine("enum NeedResolve {");
            foreach (var cl in classes)
            {
                foreach (var r in cl.EStructuralFeatures.OfType<IEReference>())
                {
                    if (r.Containment.GetValueOrDefault(false) || (r.EOpposite != null && r.EOpposite.Containment.GetValueOrDefault(false))) continue;
                    sw.WriteLine($"    {cl.Name.ToPascalCase()}{r.Name.ToPascalCase()} {{ element : Rc<Box<{cl.Name.ToPascalCase()}>>, reference : String }},");
                }
            }
            sw.WriteLine("}");
            sw.WriteLine("impl ParserState {");
            sw.WriteLine("    fn push(reference : &str, attributes : &Vec<OwnedAttribute>, stack : &mut Vec<ParserState>) {");
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
            sw.WriteLine("                    ParserState::Root => {");
            sw.WriteLine("                        let mut fullName : String = String::from(name.prefix.unwrap());");
            sw.WriteLine("                        fullName.push(':');");
            sw.WriteLine("                        fullName.push_str(&name.local_name);");
            sw.WriteLine("                        ParserState::push(&fullName, &attributes, stack);");
            sw.WriteLine("                    },");
            foreach (var cl in classes)
            {
                if (cl.Abstract.GetValueOrDefault(false)) continue;
                sw.WriteLine($"                    ParserState::{cl.Name.ToPascalCase()}(ref element) => {{");
                foreach (var r in cl.EStructuralFeatures.OfType<IEReference>())
                {
                    if (r.Containment.GetValueOrDefault(false))
                    {
                        sw.WriteLine($"                        if name.local_name == \"{r.Name}\" {{");
                        sw.WriteLine($"                            let r_name = \"{prefix}:{r.EType.Name.ToPascalCase()}\";");
                        sw.WriteLine($"                            ParserState::push(find_type(&attributes, r_name), &attributes, stack);");
                        sw.WriteLine("                            return;");
                        sw.WriteLine("                        }");
                    }
                }
                sw.WriteLine("                        panic!(\"Unexpected element found\");");
                sw.WriteLine("                    },");
            }
            sw.WriteLine("                }");
            sw.WriteLine("            }");
            sw.WriteLine("            _ => {},");
            sw.WriteLine("        }");
            sw.WriteLine("    }");
            sw.WriteLine("}");
            sw.WriteLine("impl NeedResolve {");
            sw.WriteLine($"    fn resolve(self, root : Rc<Box<{root.Name.ToPascalCase()}>>) {{");
            sw.WriteLine("        match self {");
            foreach (var cl in classes)
            {
                foreach (var r in cl.EStructuralFeatures.OfType<IEReference>())
                {
                    if (r.Containment.GetValueOrDefault(false) || (r.EOpposite != null && r.EOpposite.Containment.GetValueOrDefault(false))) continue;
                    sw.WriteLine($"            NeedResolve::{cl.Name.ToPascalCase()}{r.Name.ToPascalCase()} {{ element : Rc<Box<{cl.Name.ToPascalCase()}>>, reference : String }} => {{");

                    List<Path> allowedPaths;
                    paths.TryGetValue((IEClass)r.EType, out allowedPaths);

                    foreach (var path in allowedPaths)
                    {
                        sw.WriteLine("                lazy_static! {");
                        sw.WriteLine($"                    static ref {path.ToString()}RE : Regex = Regex::new(\"{path.PrintRegex()}\").unwrap();");
                        sw.WriteLine("                }");
                        sw.WriteLine($"                if {path.ToString()}RE.is_match(reference) {{");
                        sw.WriteLine($"                    let cap = {path.ToString()}RE.captures(reference).unwrap();");
                        var index = 1;
                        var last = "root";
                        for (int i = 0; i < path.Segments.Count; i++)
                        {
                            var seg = path.Segments[i];
                            var indexAndCast = "";
                            if (seg.UpperBound.GetValueOrDefault(1) != 1)
                            {
                                indexAndCast = $"[cap[{index}].parse::<i32>.unwrap()]";
                                index++;
                            }
                            IEClass next;
                            if (i == path.Segments.Count - 1)
                            {
                                next = (IEClass)r.EType;
                            }
                            else
                            {
                                next = path.Segments[i + 1].EContainingClass;
                            }
                            if (next != seg.EType)
                            {
                                var stack = new Stack<IEClass>();
                                stack.Push(next);
                                while (stack.Peek() != seg.EType)
                                {
                                    stack.Push(stack.Peek().ESuperTypes[0]);
                                }
                                stack.Pop();
                                while (stack.Count > 0)
                                {
                                    indexAndCast += $".cast_{stack.Pop().Name.ToCamelCase()}().unwrap()";
                                }
                            }
                            sw.WriteLine($"                    let {seg.Name.ToCamelCase()} = {last}.get_{seg.Name}(){indexAndCast};");
                            last = seg.Name.ToCamelCase();
                        }
                        if (r.UpperBound.GetValueOrDefault(1) == 1)
                        {
                            sw.WriteLine($"                    element.set_{r.Name}(Some({last}));");
                        }
                        else
                        {
                            sw.WriteLine($"                    element.get_{r.Name}().push({last});");
                        }
                        sw.WriteLine("                    return;");
                        sw.WriteLine("                }");
                    }

                    sw.WriteLine("            },");
                }
            }
            sw.WriteLine("        }");
            sw.WriteLine("    }");
            sw.WriteLine("}");

            sw.WriteLine(@"pub fn load(file_name :String) {
    let file = File::open(file_name).unwrap();
    let file = BufReader::new(file);

    let parser = EventReader::new(file);
    let mut stack : Vec<ParserState> = Vec<ParserState>::new();
    let mut resolves : Vec<NeedResolve> = Vec<NeedResolve>::new();
    let root : ParserState = ParserState::Root;
    stack.push(root);
    let mut state : &ParserState = &stack[0];
    for e in parser {
        state.parse(e, &stack);
        state = &stack[stack.len()-1];
    }
}");
        }
    }
}
