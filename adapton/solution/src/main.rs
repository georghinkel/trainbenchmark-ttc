mod railway;

extern crate xml;

use std::fs::File;
use std::io::BufReader;

use self::xml::reader::{EventReader, XmlEvent};

fn indent(size: usize) -> String {
    const INDENT: &'static str = "    ";
    (0..size).map(|_| INDENT)
             .fold(String::with_capacity(size*INDENT.len()), |r, s| r + s)
}

pub fn load(file_name :String) {
    let file = File::open(file_name).unwrap();
    let file = BufReader::new(file);

    let parser = EventReader::new(file);
    let mut depth = 0;
    let mut container = railway::RailwayContainerImpl::default();
    for e in parser {
        match e {
            Ok(XmlEvent::StartElement { name, attributes, namespace }) => {
                if name.local_name == "definedBy" {
                    println!("definedBy");
                }
                if name.local_name == "elements" {
                    println!("elements");
                }
            }
            _ => {}
        }
    }
}

fn main() {
    load("railway-1.xmi".to_owned());
}