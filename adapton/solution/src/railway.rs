#[macro_use] extern crate adapton;
#[macro_use] extern crate lazy_static;
extern crate regex;


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

use regex::Regex;

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
    fn cast_segment(&self) -> Option<&Segment>;
    fn cast_segment_mut(&mut self) -> Option<&mut Segment>;
    fn cast_switch(&self) -> Option<&Switch>;
    fn cast_switch_mut(&mut self) -> Option<&mut Switch>;
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
    fn cast_trackElement(&self) -> Option<&TrackElement>;
    fn cast_trackElement_mut(&mut self) -> Option<&mut TrackElement>;
    fn cast_route(&self) -> Option<&Route>;
    fn cast_route_mut(&mut self) -> Option<&mut Route>;
    fn cast_semaphore(&self) -> Option<&Semaphore>;
    fn cast_semaphore_mut(&mut self) -> Option<&mut Semaphore>;
    fn cast_switchPosition(&self) -> Option<&SwitchPosition>;
    fn cast_switchPosition_mut(&mut self) -> Option<&mut SwitchPosition>;
    fn cast_sensor(&self) -> Option<&Sensor>;
    fn cast_sensor_mut(&mut self) -> Option<&mut Sensor>;
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
    fn cast_segment(&self) -> Option<&Segment> { Some(self) }
    fn cast_segment_mut(&mut self) -> Option<&mut Segment> { Some(self) }
    fn cast_switch(&self) -> Option<&Switch> { None }
    fn cast_switch_mut(&mut self) -> Option<&mut Switch> { None }
}
impl RailwayElement for SegmentImpl {
    fn get_id(&self) -> Option<i32> {
        self.id.clone()
    }
    fn set_id(&mut self, value: Option<i32>) {
        self.id = value
    }
    fn cast_trackElement(&self) -> Option<&TrackElement> { Some(self) }
    fn cast_trackElement_mut(&mut self) -> Option<&mut TrackElement> { Some(self) }
    fn cast_route(&self) -> Option<&Route> { None }
    fn cast_route_mut(&mut self) -> Option<&mut Route> { None }
    fn cast_semaphore(&self) -> Option<&Semaphore> { None }
    fn cast_semaphore_mut(&mut self) -> Option<&mut Semaphore> { None }
    fn cast_switchPosition(&self) -> Option<&SwitchPosition> { None }
    fn cast_switchPosition_mut(&mut self) -> Option<&mut SwitchPosition> { None }
    fn cast_sensor(&self) -> Option<&Sensor> { None }
    fn cast_sensor_mut(&mut self) -> Option<&mut Sensor> { None }
}

impl TrackElement for TrackElementImpl {
    fn get_connectsTo(&mut self) -> &mut Vec<Rc<Box<TrackElement>>> {
        match *self {
            TrackElementImpl::Segment(ref mut i) => i.get_connectsTo(),
            TrackElementImpl::Switch(ref mut i) => i.get_connectsTo(),
        }
    }
    fn cast_segment(&self) -> Option<&Segment> {
        match *self {
            TrackElementImpl::Segment(ref i) => Some(self),
            TrackElementImpl::Switch(ref i) => None,
        }
    }
    fn cast_segment_mut(&mut self) -> Option<&mut Segment> {
        match *self {
            TrackElementImpl::Segment(ref i) => Some(self),
            TrackElementImpl::Switch(ref i) => None,
        }
    }
    fn cast_switch(&self) -> Option<&Switch> {
        match *self {
            TrackElementImpl::Segment(ref i) => None,
            TrackElementImpl::Switch(ref i) => Some(self),
        }
    }
    fn cast_switch_mut(&mut self) -> Option<&mut Switch> {
        match *self {
            TrackElementImpl::Segment(ref i) => None,
            TrackElementImpl::Switch(ref i) => Some(self),
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
    fn cast_trackElement(&self) -> Option<&TrackElement> { Some(self) }
    fn cast_trackElement_mut(&mut self) -> Option<&mut TrackElement> { Some(self) }
    fn cast_route(&self) -> Option<&Route> { None }
    fn cast_route_mut(&mut self) -> Option<&mut Route> { None }
    fn cast_semaphore(&self) -> Option<&Semaphore> { None }
    fn cast_semaphore_mut(&mut self) -> Option<&mut Semaphore> { None }
    fn cast_switchPosition(&self) -> Option<&SwitchPosition> { None }
    fn cast_switchPosition_mut(&mut self) -> Option<&mut SwitchPosition> { None }
    fn cast_sensor(&self) -> Option<&Sensor> { None }
    fn cast_sensor_mut(&mut self) -> Option<&mut Sensor> { None }
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
    fn cast_segment(&self) -> Option<&Segment> { None }
    fn cast_segment_mut(&mut self) -> Option<&mut Segment> { None }
    fn cast_switch(&self) -> Option<&Switch> { Some(self) }
    fn cast_switch_mut(&mut self) -> Option<&mut Switch> { Some(self) }
}
impl RailwayElement for SwitchImpl {
    fn get_id(&self) -> Option<i32> {
        self.id.clone()
    }
    fn set_id(&mut self, value: Option<i32>) {
        self.id = value
    }
    fn cast_trackElement(&self) -> Option<&TrackElement> { Some(self) }
    fn cast_trackElement_mut(&mut self) -> Option<&mut TrackElement> { Some(self) }
    fn cast_route(&self) -> Option<&Route> { None }
    fn cast_route_mut(&mut self) -> Option<&mut Route> { None }
    fn cast_semaphore(&self) -> Option<&Semaphore> { None }
    fn cast_semaphore_mut(&mut self) -> Option<&mut Semaphore> { None }
    fn cast_switchPosition(&self) -> Option<&SwitchPosition> { None }
    fn cast_switchPosition_mut(&mut self) -> Option<&mut SwitchPosition> { None }
    fn cast_sensor(&self) -> Option<&Sensor> { None }
    fn cast_sensor_mut(&mut self) -> Option<&mut Sensor> { None }
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
    fn cast_trackElement(&self) -> Option<&TrackElement> { None }
    fn cast_trackElement_mut(&mut self) -> Option<&mut TrackElement> { None }
    fn cast_route(&self) -> Option<&Route> { Some(self) }
    fn cast_route_mut(&mut self) -> Option<&mut Route> { Some(self) }
    fn cast_semaphore(&self) -> Option<&Semaphore> { None }
    fn cast_semaphore_mut(&mut self) -> Option<&mut Semaphore> { None }
    fn cast_switchPosition(&self) -> Option<&SwitchPosition> { None }
    fn cast_switchPosition_mut(&mut self) -> Option<&mut SwitchPosition> { None }
    fn cast_sensor(&self) -> Option<&Sensor> { None }
    fn cast_sensor_mut(&mut self) -> Option<&mut Sensor> { None }
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
    fn cast_trackElement(&self) -> Option<&TrackElement> { None }
    fn cast_trackElement_mut(&mut self) -> Option<&mut TrackElement> { None }
    fn cast_route(&self) -> Option<&Route> { None }
    fn cast_route_mut(&mut self) -> Option<&mut Route> { None }
    fn cast_semaphore(&self) -> Option<&Semaphore> { Some(self) }
    fn cast_semaphore_mut(&mut self) -> Option<&mut Semaphore> { Some(self) }
    fn cast_switchPosition(&self) -> Option<&SwitchPosition> { None }
    fn cast_switchPosition_mut(&mut self) -> Option<&mut SwitchPosition> { None }
    fn cast_sensor(&self) -> Option<&Sensor> { None }
    fn cast_sensor_mut(&mut self) -> Option<&mut Sensor> { None }
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
    fn cast_trackElement(&self) -> Option<&TrackElement> { None }
    fn cast_trackElement_mut(&mut self) -> Option<&mut TrackElement> { None }
    fn cast_route(&self) -> Option<&Route> { None }
    fn cast_route_mut(&mut self) -> Option<&mut Route> { None }
    fn cast_semaphore(&self) -> Option<&Semaphore> { None }
    fn cast_semaphore_mut(&mut self) -> Option<&mut Semaphore> { None }
    fn cast_switchPosition(&self) -> Option<&SwitchPosition> { Some(self) }
    fn cast_switchPosition_mut(&mut self) -> Option<&mut SwitchPosition> { Some(self) }
    fn cast_sensor(&self) -> Option<&Sensor> { None }
    fn cast_sensor_mut(&mut self) -> Option<&mut Sensor> { None }
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
    fn cast_trackElement(&self) -> Option<&TrackElement> {
        match *self {
            RailwayElementImpl::TrackElement(ref i) => Some(self),
            RailwayElementImpl::Route(ref i) => None,
            RailwayElementImpl::Semaphore(ref i) => None,
            RailwayElementImpl::SwitchPosition(ref i) => None,
            RailwayElementImpl::Sensor(ref i) => None,
        }
    }
    fn cast_trackElement_mut(&mut self) -> Option<&mut TrackElement> {
        match *self {
            RailwayElementImpl::TrackElement(ref i) => Some(self),
            RailwayElementImpl::Route(ref i) => None,
            RailwayElementImpl::Semaphore(ref i) => None,
            RailwayElementImpl::SwitchPosition(ref i) => None,
            RailwayElementImpl::Sensor(ref i) => None,
        }
    }
    fn cast_route(&self) -> Option<&Route> {
        match *self {
            RailwayElementImpl::TrackElement(ref i) => None,
            RailwayElementImpl::Route(ref i) => Some(self),
            RailwayElementImpl::Semaphore(ref i) => None,
            RailwayElementImpl::SwitchPosition(ref i) => None,
            RailwayElementImpl::Sensor(ref i) => None,
        }
    }
    fn cast_route_mut(&mut self) -> Option<&mut Route> {
        match *self {
            RailwayElementImpl::TrackElement(ref i) => None,
            RailwayElementImpl::Route(ref i) => Some(self),
            RailwayElementImpl::Semaphore(ref i) => None,
            RailwayElementImpl::SwitchPosition(ref i) => None,
            RailwayElementImpl::Sensor(ref i) => None,
        }
    }
    fn cast_semaphore(&self) -> Option<&Semaphore> {
        match *self {
            RailwayElementImpl::TrackElement(ref i) => None,
            RailwayElementImpl::Route(ref i) => None,
            RailwayElementImpl::Semaphore(ref i) => Some(self),
            RailwayElementImpl::SwitchPosition(ref i) => None,
            RailwayElementImpl::Sensor(ref i) => None,
        }
    }
    fn cast_semaphore_mut(&mut self) -> Option<&mut Semaphore> {
        match *self {
            RailwayElementImpl::TrackElement(ref i) => None,
            RailwayElementImpl::Route(ref i) => None,
            RailwayElementImpl::Semaphore(ref i) => Some(self),
            RailwayElementImpl::SwitchPosition(ref i) => None,
            RailwayElementImpl::Sensor(ref i) => None,
        }
    }
    fn cast_switchPosition(&self) -> Option<&SwitchPosition> {
        match *self {
            RailwayElementImpl::TrackElement(ref i) => None,
            RailwayElementImpl::Route(ref i) => None,
            RailwayElementImpl::Semaphore(ref i) => None,
            RailwayElementImpl::SwitchPosition(ref i) => Some(self),
            RailwayElementImpl::Sensor(ref i) => None,
        }
    }
    fn cast_switchPosition_mut(&mut self) -> Option<&mut SwitchPosition> {
        match *self {
            RailwayElementImpl::TrackElement(ref i) => None,
            RailwayElementImpl::Route(ref i) => None,
            RailwayElementImpl::Semaphore(ref i) => None,
            RailwayElementImpl::SwitchPosition(ref i) => Some(self),
            RailwayElementImpl::Sensor(ref i) => None,
        }
    }
    fn cast_sensor(&self) -> Option<&Sensor> {
        match *self {
            RailwayElementImpl::TrackElement(ref i) => None,
            RailwayElementImpl::Route(ref i) => None,
            RailwayElementImpl::Semaphore(ref i) => None,
            RailwayElementImpl::SwitchPosition(ref i) => None,
            RailwayElementImpl::Sensor(ref i) => Some(self),
        }
    }
    fn cast_sensor_mut(&mut self) -> Option<&mut Sensor> {
        match *self {
            RailwayElementImpl::TrackElement(ref i) => None,
            RailwayElementImpl::Route(ref i) => None,
            RailwayElementImpl::Semaphore(ref i) => None,
            RailwayElementImpl::SwitchPosition(ref i) => None,
            RailwayElementImpl::Sensor(ref i) => Some(self),
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
    fn cast_trackElement(&self) -> Option<&TrackElement> { None }
    fn cast_trackElement_mut(&mut self) -> Option<&mut TrackElement> { None }
    fn cast_route(&self) -> Option<&Route> { None }
    fn cast_route_mut(&mut self) -> Option<&mut Route> { None }
    fn cast_semaphore(&self) -> Option<&Semaphore> { None }
    fn cast_semaphore_mut(&mut self) -> Option<&mut Semaphore> { None }
    fn cast_switchPosition(&self) -> Option<&SwitchPosition> { None }
    fn cast_switchPosition_mut(&mut self) -> Option<&mut SwitchPosition> { None }
    fn cast_sensor(&self) -> Option<&Sensor> { Some(self) }
    fn cast_sensor_mut(&mut self) -> Option<&mut Sensor> { Some(self) }
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
fn find_type<'a>(attributes : &'a Vec<OwnedAttribute>, default : &'a str) -> &'a str {
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
    Root,
    Segment(Rc<Box<Segment>>),
    Switch(Rc<Box<Switch>>),
    Route(Rc<Box<Route>>),
    Semaphore(Rc<Box<Semaphore>>),
    SwitchPosition(Rc<Box<SwitchPosition>>),
    Sensor(Rc<Box<Sensor>>),
    RailwayContainer(Rc<Box<RailwayContainer>>),
}
enum NeedResolve {
    TrackElementConnectsTo { element : Rc<Box<TrackElement>>, reference : String },
    SwitchPositions { element : Rc<Box<Switch>>, reference : String },
    RouteEntry { element : Rc<Box<Route>>, reference : String },
    RouteExit { element : Rc<Box<Route>>, reference : String },
    SwitchPositionSwitch { element : Rc<Box<SwitchPosition>>, reference : String },
}
impl ParserState {
    fn push(reference : &str, attributes : &Vec<OwnedAttribute>, stack : &mut Vec<ParserState>) {
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
                    ParserState::Root => {
                        let mut fullName : String = String::from(name.prefix.unwrap());
                        fullName.push(':');
                        fullName.push_str(&name.local_name);
                        ParserState::push(&fullName, &attributes, stack);
                    },
                    ParserState::Segment(ref element) => {
                        panic!("Unexpected element found");
                    },
                    ParserState::Switch(ref element) => {
                        panic!("Unexpected element found");
                    },
                    ParserState::Route(ref element) => {
                        if name.local_name == "follows" {
                            let r_name = "hu.bme.mit.trainbenchmark:SwitchPosition";
                            ParserState::push(find_type(&attributes, r_name), &attributes, stack);
                            return;
                        }
                        if name.local_name == "definedBy" {
                            let r_name = "hu.bme.mit.trainbenchmark:Sensor";
                            ParserState::push(find_type(&attributes, r_name), &attributes, stack);
                            return;
                        }
                        panic!("Unexpected element found");
                    },
                    ParserState::Semaphore(ref element) => {
                        panic!("Unexpected element found");
                    },
                    ParserState::SwitchPosition(ref element) => {
                        panic!("Unexpected element found");
                    },
                    ParserState::Sensor(ref element) => {
                        if name.local_name == "elements" {
                            let r_name = "hu.bme.mit.trainbenchmark:TrackElement";
                            ParserState::push(find_type(&attributes, r_name), &attributes, stack);
                            return;
                        }
                        panic!("Unexpected element found");
                    },
                    ParserState::RailwayContainer(ref element) => {
                        if name.local_name == "invalids" {
                            let r_name = "hu.bme.mit.trainbenchmark:RailwayElement";
                            ParserState::push(find_type(&attributes, r_name), &attributes, stack);
                            return;
                        }
                        if name.local_name == "semaphores" {
                            let r_name = "hu.bme.mit.trainbenchmark:Semaphore";
                            ParserState::push(find_type(&attributes, r_name), &attributes, stack);
                            return;
                        }
                        if name.local_name == "routes" {
                            let r_name = "hu.bme.mit.trainbenchmark:Route";
                            ParserState::push(find_type(&attributes, r_name), &attributes, stack);
                            return;
                        }
                        panic!("Unexpected element found");
                    },
                }
            }
            _ => {},
        }
    }
}
impl NeedResolve {
    fn resolve(self, root : Rc<Box<RailwayContainer>>) {
        match self {
            NeedResolve::TrackElementConnectsTo { element : Rc<Box<TrackElement>>, reference : String } => {
                lazy_static! {
                    static ref InvalidsRE : Regex = Regex::new("^#//invalids\.(\d+)/$").unwrap();
                }
                if InvalidsRE.is_match(reference) {
                    let cap = InvalidsRE.captures(reference).unwrap();
                    let invalids = root.get_invalids()[cap[1].parse::<i32>.unwrap()].cast_trackElement().unwrap();
                    element.get_connectsTo().push(invalids);
                    return;
                }
                lazy_static! {
                    static ref InvalidsDefinedByElementsRE : Regex = Regex::new("^#//invalids\.(\d+)/definedBy\.(\d+)/elements\.(\d+)/$").unwrap();
                }
                if InvalidsDefinedByElementsRE.is_match(reference) {
                    let cap = InvalidsDefinedByElementsRE.captures(reference).unwrap();
                    let invalids = root.get_invalids()[cap[1].parse::<i32>.unwrap()].cast_route().unwrap();
                    let definedBy = invalids.get_definedBy()[cap[2].parse::<i32>.unwrap()];
                    let elements = definedBy.get_elements()[cap[3].parse::<i32>.unwrap()];
                    element.get_connectsTo().push(elements);
                    return;
                }
                lazy_static! {
                    static ref InvalidsElementsRE : Regex = Regex::new("^#//invalids\.(\d+)/elements\.(\d+)/$").unwrap();
                }
                if InvalidsElementsRE.is_match(reference) {
                    let cap = InvalidsElementsRE.captures(reference).unwrap();
                    let invalids = root.get_invalids()[cap[1].parse::<i32>.unwrap()].cast_sensor().unwrap();
                    let elements = invalids.get_elements()[cap[2].parse::<i32>.unwrap()];
                    element.get_connectsTo().push(elements);
                    return;
                }
                lazy_static! {
                    static ref RoutesDefinedByElementsRE : Regex = Regex::new("^#//routes\.(\d+)/definedBy\.(\d+)/elements\.(\d+)/$").unwrap();
                }
                if RoutesDefinedByElementsRE.is_match(reference) {
                    let cap = RoutesDefinedByElementsRE.captures(reference).unwrap();
                    let routes = root.get_routes()[cap[1].parse::<i32>.unwrap()];
                    let definedBy = routes.get_definedBy()[cap[2].parse::<i32>.unwrap()];
                    let elements = definedBy.get_elements()[cap[3].parse::<i32>.unwrap()];
                    element.get_connectsTo().push(elements);
                    return;
                }
            },
            NeedResolve::SwitchPositions { element : Rc<Box<Switch>>, reference : String } => {
                lazy_static! {
                    static ref InvalidsFollowsRE : Regex = Regex::new("^#//invalids\.(\d+)/follows\.(\d+)/$").unwrap();
                }
                if InvalidsFollowsRE.is_match(reference) {
                    let cap = InvalidsFollowsRE.captures(reference).unwrap();
                    let invalids = root.get_invalids()[cap[1].parse::<i32>.unwrap()].cast_route().unwrap();
                    let follows = invalids.get_follows()[cap[2].parse::<i32>.unwrap()];
                    element.get_positions().push(follows);
                    return;
                }
                lazy_static! {
                    static ref InvalidsRE : Regex = Regex::new("^#//invalids\.(\d+)/$").unwrap();
                }
                if InvalidsRE.is_match(reference) {
                    let cap = InvalidsRE.captures(reference).unwrap();
                    let invalids = root.get_invalids()[cap[1].parse::<i32>.unwrap()].cast_switchPosition().unwrap();
                    element.get_positions().push(invalids);
                    return;
                }
                lazy_static! {
                    static ref RoutesFollowsRE : Regex = Regex::new("^#//routes\.(\d+)/follows\.(\d+)/$").unwrap();
                }
                if RoutesFollowsRE.is_match(reference) {
                    let cap = RoutesFollowsRE.captures(reference).unwrap();
                    let routes = root.get_routes()[cap[1].parse::<i32>.unwrap()];
                    let follows = routes.get_follows()[cap[2].parse::<i32>.unwrap()];
                    element.get_positions().push(follows);
                    return;
                }
            },
            NeedResolve::RouteEntry { element : Rc<Box<Route>>, reference : String } => {
                lazy_static! {
                    static ref InvalidsRE : Regex = Regex::new("^#//invalids\.(\d+)/$").unwrap();
                }
                if InvalidsRE.is_match(reference) {
                    let cap = InvalidsRE.captures(reference).unwrap();
                    let invalids = root.get_invalids()[cap[1].parse::<i32>.unwrap()].cast_semaphore().unwrap();
                    element.set_entry(Some(invalids));
                    return;
                }
                lazy_static! {
                    static ref SemaphoresRE : Regex = Regex::new("^#//semaphores\.(\d+)/$").unwrap();
                }
                if SemaphoresRE.is_match(reference) {
                    let cap = SemaphoresRE.captures(reference).unwrap();
                    let semaphores = root.get_semaphores()[cap[1].parse::<i32>.unwrap()];
                    element.set_entry(Some(semaphores));
                    return;
                }
            },
            NeedResolve::RouteExit { element : Rc<Box<Route>>, reference : String } => {
                lazy_static! {
                    static ref InvalidsRE : Regex = Regex::new("^#//invalids\.(\d+)/$").unwrap();
                }
                if InvalidsRE.is_match(reference) {
                    let cap = InvalidsRE.captures(reference).unwrap();
                    let invalids = root.get_invalids()[cap[1].parse::<i32>.unwrap()].cast_semaphore().unwrap();
                    element.set_exit(Some(invalids));
                    return;
                }
                lazy_static! {
                    static ref SemaphoresRE : Regex = Regex::new("^#//semaphores\.(\d+)/$").unwrap();
                }
                if SemaphoresRE.is_match(reference) {
                    let cap = SemaphoresRE.captures(reference).unwrap();
                    let semaphores = root.get_semaphores()[cap[1].parse::<i32>.unwrap()];
                    element.set_exit(Some(semaphores));
                    return;
                }
            },
            NeedResolve::SwitchPositionSwitch { element : Rc<Box<SwitchPosition>>, reference : String } => {
                lazy_static! {
                    static ref InvalidsRE : Regex = Regex::new("^#//invalids\.(\d+)/$").unwrap();
                }
                if InvalidsRE.is_match(reference) {
                    let cap = InvalidsRE.captures(reference).unwrap();
                    let invalids = root.get_invalids()[cap[1].parse::<i32>.unwrap()].cast_trackElement().unwrap().cast_switch().unwrap();
                    element.set_switch(Some(invalids));
                    return;
                }
                lazy_static! {
                    static ref InvalidsDefinedByElementsRE : Regex = Regex::new("^#//invalids\.(\d+)/definedBy\.(\d+)/elements\.(\d+)/$").unwrap();
                }
                if InvalidsDefinedByElementsRE.is_match(reference) {
                    let cap = InvalidsDefinedByElementsRE.captures(reference).unwrap();
                    let invalids = root.get_invalids()[cap[1].parse::<i32>.unwrap()].cast_route().unwrap();
                    let definedBy = invalids.get_definedBy()[cap[2].parse::<i32>.unwrap()];
                    let elements = definedBy.get_elements()[cap[3].parse::<i32>.unwrap()].cast_switch().unwrap();
                    element.set_switch(Some(elements));
                    return;
                }
                lazy_static! {
                    static ref InvalidsElementsRE : Regex = Regex::new("^#//invalids\.(\d+)/elements\.(\d+)/$").unwrap();
                }
                if InvalidsElementsRE.is_match(reference) {
                    let cap = InvalidsElementsRE.captures(reference).unwrap();
                    let invalids = root.get_invalids()[cap[1].parse::<i32>.unwrap()].cast_sensor().unwrap();
                    let elements = invalids.get_elements()[cap[2].parse::<i32>.unwrap()].cast_switch().unwrap();
                    element.set_switch(Some(elements));
                    return;
                }
                lazy_static! {
                    static ref RoutesDefinedByElementsRE : Regex = Regex::new("^#//routes\.(\d+)/definedBy\.(\d+)/elements\.(\d+)/$").unwrap();
                }
                if RoutesDefinedByElementsRE.is_match(reference) {
                    let cap = RoutesDefinedByElementsRE.captures(reference).unwrap();
                    let routes = root.get_routes()[cap[1].parse::<i32>.unwrap()];
                    let definedBy = routes.get_definedBy()[cap[2].parse::<i32>.unwrap()];
                    let elements = definedBy.get_elements()[cap[3].parse::<i32>.unwrap()].cast_switch().unwrap();
                    element.set_switch(Some(elements));
                    return;
                }
            },
        }
    }
}
pub fn load(file_name :String) {
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
}
