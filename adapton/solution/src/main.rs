#[macro_use]
extern crate xml;

#[macro_use]
extern crate adapton;

pub mod railway;

use std::rc::Rc;
use std::fs::File;
use std::io::BufReader;

use xml::reader::{EventReader, XmlEvent};
use xml::name::OwnedName;
use xml::attribute::OwnedAttribute;

fn indent(size: usize) -> String {
    const INDENT: &'static str = "    ";
    (0..size).map(|_| INDENT)
             .fold(String::with_capacity(size*INDENT.len()), |r, s| r + s)
}

fn find_type(attributes : &Vec<OwnedAttribute>) -> Option<&String> {
	for att in attributes {
		match att.name.prefix {
			Some(ref prefix) => {
				if prefix == "xsi" && att.name.local_name == "type" {
					return Some(&att.value)
				}
			},
			None => continue
		}
	}
	return None
}

fn main() {
}