use adapton::engine::*;

use std::hash::Hash;
use std::rc::Rc;
use std::fmt::Debug;
use std::fs::File;
use std::io::BufReader;
use std::ops::Deref;

use xml::reader::{ EventReader, XmlEvent};
use xml::name::OwnedName;
use xml::attribute::OwnedAttribute;

// generated enums
#[derive(Eq, PartialEq, Clone, Copy, Hash, Debug)]
pub enum Signal {
    FAILURE,
    STOP,
    GO
}

#[derive(Eq, PartialEq, Clone, Copy, Hash, Debug)]
pub enum Position {
    FAILURE,
    LEFT,
    RIGHT,
    STRAIGHT
}

// generated traits
pub trait Segment : TrackElement + Debug {
    fn get_length(&self) -> Option<i32>;
    fn set_length(&mut self, value: Option<i32>);
}

pub trait TrackElement : RailwayElement + Debug {
    fn get_connectsTo(&mut self) -> &mut Vec<Rc<Box<TrackElement>>>;
}

pub trait Switch : TrackElement + Debug {
    fn get_currentPosition(&self) -> Option<Position>;
    fn set_currentPosition(&mut self, value: Option<Position>);
    fn get_positions(&mut self) -> &mut Vec<Rc<Box<SwitchPosition>>>;
}

pub trait Route : RailwayElement + Debug {
    fn get_entry(&self) -> Option<Rc<Box<Semaphore>>>;
    fn set_entry(&mut self, value: Option<Rc<Box<Semaphore>>>);
    fn get_follows(&mut self) -> &mut Vec<Rc<Box<SwitchPosition>>>;
    fn get_exit(&self) -> Option<Rc<Box<Semaphore>>>;
    fn set_exit(&mut self, value: Option<Rc<Box<Semaphore>>>);
    fn get_definedBy(&mut self) -> &mut Vec<Rc<Box<Sensor>>>;
}

pub trait Semaphore : RailwayElement + Debug {
    fn get_signal(&self) -> Option<Signal>;
    fn set_signal(&mut self, value: Option<Signal>);
}

pub trait SwitchPosition : RailwayElement + Debug {
    fn get_switch(&self) -> Option<Rc<Box<Switch>>>;
    fn set_switch(&mut self, value: Option<Rc<Box<Switch>>>);
    fn get_position(&self) -> Option<Position>;
    fn set_position(&mut self, value: Option<Position>);
}

pub trait RailwayElement : Debug {
    fn get_id(&self) -> Option<i32>;
    fn set_id(&mut self, value: Option<i32>);
}

pub trait Sensor : RailwayElement + Debug {
    fn get_elements(&mut self) -> &mut Vec<Rc<Box<TrackElement>>>;
}

pub trait RailwayContainer : Debug {
    fn get_invalids(&mut self) -> &mut Vec<Rc<Box<RailwayElement>>>;
    fn get_semaphores(&mut self) -> &mut Vec<Rc<Box<Semaphore>>>;
    fn get_routes(&mut self) -> &mut Vec<Rc<Box<Route>>>;
}

// generated structs
#[derive(Debug, Default)]
pub struct SegmentImpl {
    length: Option<i32>,
    connectsTo: Vec<Rc<Box<TrackElement>>>,
    id: Option<i32>
}

#[derive(Debug)]
pub enum TrackElementImpl {
    Segment(SegmentImpl),
    Switch(SwitchImpl)
}

#[derive(Debug, Default)]
pub struct SwitchImpl {
    currentPosition: Option<Position>,
    positions: Vec<Rc<Box<SwitchPosition>>>,
    connectsTo: Vec<Rc<Box<TrackElement>>>,
    id: Option<i32>
}

#[derive(Debug, Default)]
pub struct RouteImpl {
    entry: Option<Rc<Box<Semaphore>>>,
    follows: Vec<Rc<Box<SwitchPosition>>>,
    exit: Option<Rc<Box<Semaphore>>>,
    definedBy: Vec<Rc<Box<Sensor>>>,
    id: Option<i32>
}

#[derive(Debug, Default)]
pub struct SemaphoreImpl {
    signal: Option<Signal>,
    id: Option<i32>
}

#[derive(Debug, Default)]
pub struct SwitchPositionImpl {
    switch: Option<Rc<Box<Switch>>>,
    position: Option<Position>,
    id: Option<i32>
}

#[derive(Debug)]
pub enum RailwayElementImpl {
    TrackElement(TrackElementImpl),
    Route(RouteImpl),
    Semaphore(SemaphoreImpl),
    SwitchPosition(SwitchPositionImpl),
    Sensor(SensorImpl)
}

#[derive(Debug, Default)]
pub struct SensorImpl {
    elements: Vec<Rc<Box<TrackElement>>>,
    id: Option<i32>
}

#[derive(Debug, Default)]
pub struct RailwayContainerImpl {
    invalids: Vec<Rc<Box<RailwayElement>>>,
    semaphores: Vec<Rc<Box<Semaphore>>>,
    routes: Vec<Rc<Box<Route>>>
}

// generate implementations
impl Segment for SegmentImpl {
    fn get_length(&self) -> Option<i32> {
        self.length.clone()
    }
    fn set_length(&mut self, value: Option<i32>) {
        self.length = value
    }
}
impl TrackElement for SegmentImpl {
    fn get_connectsTo(&mut self) -> &mut Vec<Rc<Box<TrackElement>>> {
        &mut self.connectsTo
    }
}
impl RailwayElement for SegmentImpl {
    fn get_id(&self) -> Option<i32> {
        self.id.clone()
    }
    fn set_id(&mut self, value: Option<i32>) {
        self.id = value
    }
}

impl TrackElement for TrackElementImpl {
    fn get_connectsTo(&mut self) -> &mut Vec<Rc<Box<TrackElement>>> {
        match *self {
            TrackElementImpl::Segment(ref mut i) => i.get_connectsTo(),
            TrackElementImpl::Switch(ref mut i) => i.get_connectsTo(),
        }
    }
}
impl RailwayElement for TrackElementImpl {
    fn get_id(&self) -> Option<i32> {
        match *self {
            TrackElementImpl::Segment(ref i) => i.get_id(),
            TrackElementImpl::Switch(ref i) => i.get_id(),
        }
    }
    fn set_id(&mut self, value: Option<i32>) {
        match *self {
            TrackElementImpl::Segment(ref mut i) => i.set_id(value),
            TrackElementImpl::Switch(ref mut i) => i.set_id(value),
        }
    }
}

impl Switch for SwitchImpl {
    fn get_currentPosition(&self) -> Option<Position> {
        self.currentPosition.clone()
    }
    fn set_currentPosition(&mut self, value: Option<Position>) {
        self.currentPosition = value
    }
    fn get_positions(&mut self) -> &mut Vec<Rc<Box<SwitchPosition>>> {
        &mut self.positions
    }
}
impl TrackElement for SwitchImpl {
    fn get_connectsTo(&mut self) -> &mut Vec<Rc<Box<TrackElement>>> {
        &mut self.connectsTo
    }
}
impl RailwayElement for SwitchImpl {
    fn get_id(&self) -> Option<i32> {
        self.id.clone()
    }
    fn set_id(&mut self, value: Option<i32>) {
        self.id = value
    }
}

impl Route for RouteImpl {
    fn get_entry(&self) -> Option<Rc<Box<Semaphore>>> {
        match self.entry {
            None => None,
            Some(ref val) => Some(val.clone()),
        }
    }
    fn set_entry(&mut self, value: Option<Rc<Box<Semaphore>>>) {
        self.entry = value
    }
    fn get_follows(&mut self) -> &mut Vec<Rc<Box<SwitchPosition>>> {
        &mut self.follows
    }
    fn get_exit(&self) -> Option<Rc<Box<Semaphore>>> {
        match self.exit {
            None => None,
            Some(ref val) => Some(val.clone()),
        }
    }
    fn set_exit(&mut self, value: Option<Rc<Box<Semaphore>>>) {
        self.exit = value
    }
    fn get_definedBy(&mut self) -> &mut Vec<Rc<Box<Sensor>>> {
        &mut self.definedBy
    }
}
impl RailwayElement for RouteImpl {
    fn get_id(&self) -> Option<i32> {
        self.id.clone()
    }
    fn set_id(&mut self, value: Option<i32>) {
        self.id = value
    }
}

impl Semaphore for SemaphoreImpl {
    fn get_signal(&self) -> Option<Signal> {
        self.signal.clone()
    }
    fn set_signal(&mut self, value: Option<Signal>) {
        self.signal = value
    }
}
impl RailwayElement for SemaphoreImpl {
    fn get_id(&self) -> Option<i32> {
        self.id.clone()
    }
    fn set_id(&mut self, value: Option<i32>) {
        self.id = value
    }
}

impl SwitchPosition for SwitchPositionImpl {
    fn get_switch(&self) -> Option<Rc<Box<Switch>>> {
        match self.switch {
            None => None,
            Some(ref val) => Some(val.clone()),
        }
    }
    fn set_switch(&mut self, value: Option<Rc<Box<Switch>>>) {
        self.switch = value
    }
    fn get_position(&self) -> Option<Position> {
        self.position.clone()
    }
    fn set_position(&mut self, value: Option<Position>) {
        self.position = value
    }
}
impl RailwayElement for SwitchPositionImpl {
    fn get_id(&self) -> Option<i32> {
        self.id.clone()
    }
    fn set_id(&mut self, value: Option<i32>) {
        self.id = value
    }
}

impl RailwayElement for RailwayElementImpl {
    fn get_id(&self) -> Option<i32> {
        match *self {
            RailwayElementImpl::TrackElement(ref i) => i.get_id(),
            RailwayElementImpl::Route(ref i) => i.get_id(),
            RailwayElementImpl::Semaphore(ref i) => i.get_id(),
            RailwayElementImpl::SwitchPosition(ref i) => i.get_id(),
            RailwayElementImpl::Sensor(ref i) => i.get_id(),
        }
    }
    fn set_id(&mut self, value: Option<i32>) {
        match *self {
            RailwayElementImpl::TrackElement(ref mut i) => i.set_id(value),
            RailwayElementImpl::Route(ref mut i) => i.set_id(value),
            RailwayElementImpl::Semaphore(ref mut i) => i.set_id(value),
            RailwayElementImpl::SwitchPosition(ref mut i) => i.set_id(value),
            RailwayElementImpl::Sensor(ref mut i) => i.set_id(value),
        }
    }
}

impl Sensor for SensorImpl {
    fn get_elements(&mut self) -> &mut Vec<Rc<Box<TrackElement>>> {
        &mut self.elements
    }
}
impl RailwayElement for SensorImpl {
    fn get_id(&self) -> Option<i32> {
        self.id.clone()
    }
    fn set_id(&mut self, value: Option<i32>) {
        self.id = value
    }
}

impl RailwayContainer for RailwayContainerImpl {
    fn get_invalids(&mut self) -> &mut Vec<Rc<Box<RailwayElement>>> {
        &mut self.invalids
    }
    fn get_semaphores(&mut self) -> &mut Vec<Rc<Box<Semaphore>>> {
        &mut self.semaphores
    }
    fn get_routes(&mut self) -> &mut Vec<Rc<Box<Route>>> {
        &mut self.routes
    }
}

// generated parser
fn find_type<'a>(attributes : &'a Vec<OwnedAttribute>, default : &'a String) -> &'a String {
	for att in attributes {
		match att.name.prefix {
			Some(ref prefix) => {
				if prefix == "xsi" && att.name.local_name == "type" {
                    return &att.value
                }
            },
			None => continue
		}
	}
	return default;
}
#[derive(Clone)]
enum ParserState {
    Segment(Rc<Box<Segment>>),
    Switch(Rc<Box<Switch>>),
    Route(Rc<Box<Route>>),
    Semaphore(Rc<Box<Semaphore>>),
    SwitchPosition(Rc<Box<SwitchPosition>>),
    Sensor(Rc<Box<Sensor>>),
    RailwayContainer(Rc<Box<RailwayContainer>>),
}
impl ParserState {
    fn push(reference : &String, stack : &mut Vec<ParserState>) {
        if reference == "hu.bme.mit.trainbenchmark:Segment" {
            let element : Rc<Box<Segment>> = Rc::new(Box::new(SegmentImpl::default()));
            stack.push(ParserState::Segment(element));
        }
        if reference == "hu.bme.mit.trainbenchmark:Switch" {
            let element : Rc<Box<Switch>> = Rc::new(Box::new(SwitchImpl::default()));
            stack.push(ParserState::Switch(element));
        }
        if reference == "hu.bme.mit.trainbenchmark:Route" {
            let element : Rc<Box<Route>> = Rc::new(Box::new(RouteImpl::default()));
            stack.push(ParserState::Route(element));
        }
        if reference == "hu.bme.mit.trainbenchmark:Semaphore" {
            let element : Rc<Box<Semaphore>> = Rc::new(Box::new(SemaphoreImpl::default()));
            stack.push(ParserState::Semaphore(element));
        }
        if reference == "hu.bme.mit.trainbenchmark:SwitchPosition" {
            let element : Rc<Box<SwitchPosition>> = Rc::new(Box::new(SwitchPositionImpl::default()));
            stack.push(ParserState::SwitchPosition(element));
        }
        if reference == "hu.bme.mit.trainbenchmark:Sensor" {
            let element : Rc<Box<Sensor>> = Rc::new(Box::new(SensorImpl::default()));
            stack.push(ParserState::Sensor(element));
        }
        if reference == "hu.bme.mit.trainbenchmark:RailwayContainer" {
            let element : Rc<Box<RailwayContainer>> = Rc::new(Box::new(RailwayContainerImpl::default()));
            stack.push(ParserState::RailwayContainer(element));
        }
    }
    fn parse(&self, event : XmlEvent, stack : &mut Vec<ParserState>)
    {
        match event {
            XmlEvent::EndElement { name } => {
                if stack.len() > 1 {
                    stack.pop();
                }
            }
            XmlEvent::StartElement { name, attributes, namespace } => {
                match *self {
                    ParserState::Segment(ref element) => {
                        panic!("Unexpected element found");
                    }
                    ParserState::Switch(ref element) => {
                        panic!("Unexpected element found");
                    }
                    ParserState::Route(ref element) => {
                        if name.local_name == "follows" {
                            let r_name = String::from("hu.bme.mit.trainbenchmark:SwitchPosition");
                            ParserState::push(find_type(&attributes, &r_name), stack);
                            return;
                        }
                        if name.local_name == "definedBy" {
                            let r_name = String::from("hu.bme.mit.trainbenchmark:Sensor");
                            ParserState::push(find_type(&attributes, &r_name), stack);
                            return;
                        }
                        panic!("Unexpected element found");
                    }
                    ParserState::Semaphore(ref element) => {
                        panic!("Unexpected element found");
                    }
                    ParserState::SwitchPosition(ref element) => {
                        panic!("Unexpected element found");
                    }
                    ParserState::Sensor(ref element) => {
                        if name.local_name == "elements" {
                            let r_name = String::from("hu.bme.mit.trainbenchmark:TrackElement");
                            ParserState::push(find_type(&attributes, &r_name), stack);
                            return;
                        }
                        panic!("Unexpected element found");
                    }
                    ParserState::RailwayContainer(ref element) => {
                        if name.local_name == "invalids" {
                            let r_name = String::from("hu.bme.mit.trainbenchmark:RailwayElement");
                            ParserState::push(find_type(&attributes, &r_name), stack);
                            return;
                        }
                        if name.local_name == "semaphores" {
                            let r_name = String::from("hu.bme.mit.trainbenchmark:Semaphore");
                            ParserState::push(find_type(&attributes, &r_name), stack);
                            return;
                        }
                        if name.local_name == "routes" {
                            let r_name = String::from("hu.bme.mit.trainbenchmark:Route");
                            ParserState::push(find_type(&attributes, &r_name), stack);
                            return;
                        }
                        panic!("Unexpected element found");
                    }
                }
            }
            _ => {},
        }
    }
}
pub fn load(file_name :String) {
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
}
