use std::hash::Hash;
use std::marker::PhantomData;

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

pub trait Segment<'a> : TrackElement<'a> + Eq + PartialEq + Hash {
    fn get_length(&'a self) -> Option<i32>;
    fn set_length(&'a mut self, value: Option<i32>);
}

pub trait TrackElement<'a> : RailwayElement<'a> + Eq + PartialEq + Hash {
    fn get_connectsTo(&'a mut self) -> &'a mut Vec<&'a mut TrackElementImpl<'a>>;
}

pub trait Switch<'a> : TrackElement<'a> + Eq + PartialEq + Hash {
    fn get_currentPosition(&'a self) -> Option<Position>;
    fn set_currentPosition(&'a mut self, value: Option<Position>);
    fn get_positions(&'a mut self) -> &'a mut Vec<&'a mut SwitchPositionImpl<'a>>;
}

pub trait Route<'a> : RailwayElement<'a> + Eq + PartialEq + Hash {
    fn get_entry(&'a self) -> Option<&'a mut SemaphoreImpl<'a>>;
    fn set_entry(&'a mut self, value: Option<&'a mut SemaphoreImpl<'a>>);
    fn get_follows(&'a mut self) -> &'a mut Vec<&'a mut SwitchPositionImpl<'a>>;
    fn get_exit(&'a self) -> Option<&'a mut SemaphoreImpl<'a>>;
    fn set_exit(&'a mut self, value: Option<&'a mut SemaphoreImpl<'a>>);
    fn get_definedBy(&'a mut self) -> &'a mut Vec<&'a mut SensorImpl<'a>>;
}

pub trait Semaphore<'a> : RailwayElement<'a> + Eq + PartialEq + Hash {
    fn get_signal(&'a self) -> Option<Signal>;
    fn set_signal(&'a mut self, value: Option<Signal>);
}

pub trait SwitchPosition<'a> : RailwayElement<'a> + Eq + PartialEq + Hash {
    fn get_switch(&'a self) -> Option<&'a mut SwitchImpl<'a>>;
    fn set_switch(&'a mut self, value: Option<&'a mut SwitchImpl<'a>>);
    fn get_position(&'a self) -> Option<Position>;
    fn set_position(&'a mut self, value: Option<Position>);
}

pub trait RailwayElement<'a> : Eq + PartialEq + Hash {
    fn get_id(&'a self) -> Option<i32>;
    fn set_id(&'a mut self, value: Option<i32>);
}

pub trait Sensor<'a> : RailwayElement<'a> + Eq + PartialEq + Hash {
    fn get_elements(&'a mut self) -> &'a mut Vec<&'a mut TrackElementImpl<'a>>;
}

pub trait RailwayContainer<'a> : Eq + PartialEq + Hash {
    fn get_invalids(&'a mut self) -> &'a mut Vec<&'a mut RailwayElementImpl<'a>>;
    fn get_semaphores(&'a mut self) -> &'a mut Vec<&'a mut SemaphoreImpl<'a>>;
    fn get_routes(&'a mut self) -> &'a mut Vec<&'a mut RouteImpl<'a>>;
}

#[derive(Eq, PartialEq, Hash, Debug, Default)]
pub struct SegmentImpl<'a> {
    length: Option<i32>,
    sensor: Option<&'a mut SensorImpl<'a>>,
    connectsTo: Vec<&'a mut TrackElementImpl<'a>>,
    id: Option<i32>
}

#[derive(Eq, PartialEq, Hash, Debug)]
pub enum TrackElementImpl<'a> {
    Segment(SegmentImpl<'a>),
    Switch(SwitchImpl<'a>)
}

#[derive(Eq, PartialEq, Hash, Debug, Default)]
pub struct SwitchImpl<'a> {
    currentPosition: Option<Position>,
    positions: Vec<&'a mut SwitchPositionImpl<'a>>,
    sensor: Option<&'a mut SensorImpl<'a>>,
    connectsTo: Vec<&'a mut TrackElementImpl<'a>>,
    id: Option<i32>
}

#[derive(Eq, PartialEq, Hash, Debug, Default)]
pub struct RouteImpl<'a> {
    entry: Option<&'a mut SemaphoreImpl<'a>>,
    follows: Vec<&'a mut SwitchPositionImpl<'a>>,
    exit: Option<&'a mut SemaphoreImpl<'a>>,
    definedBy: Vec<&'a mut SensorImpl<'a>>,
    id: Option<i32>
}

#[derive(Eq, PartialEq, Hash, Debug, Default)]
pub struct SemaphoreImpl<'a> {
    signal: Option<Signal>,
    id: Option<i32>,
    phantom: PhantomData<&'a String>
}

#[derive(Eq, PartialEq, Hash, Debug, Default)]
pub struct SwitchPositionImpl<'a> {
    switch: Option<&'a mut SwitchImpl<'a>>,
    position: Option<Position>,
    route: Option<&'a mut RouteImpl<'a>>,
    id: Option<i32>
}

#[derive(Eq, PartialEq, Hash, Debug)]
pub enum RailwayElementImpl<'a> {
    TrackElement(TrackElementImpl<'a>),
    Route(RouteImpl<'a>),
    Semaphore(SemaphoreImpl<'a>),
    SwitchPosition(SwitchPositionImpl<'a>),
    Sensor(SensorImpl<'a>)
}

#[derive(Eq, PartialEq, Hash, Debug, Default)]
pub struct SensorImpl<'a> {
    elements: Vec<&'a mut TrackElementImpl<'a>>,
    id: Option<i32>
}

#[derive(Eq, PartialEq, Hash, Debug, Default)]
pub struct RailwayContainerImpl<'a> {
    invalids: Vec<&'a mut RailwayElementImpl<'a>>,
    semaphores: Vec<&'a mut SemaphoreImpl<'a>>,
    routes: Vec<&'a mut RouteImpl<'a>>
}

impl <'a> Segment<'a> for SegmentImpl<'a> {
    fn get_length(&'a self) -> Option<i32> {
        self.length.clone()
    }
    fn set_length(&'a mut self, value: Option<i32>) {
        self.length = value
    }
}
impl <'a> TrackElement<'a> for SegmentImpl<'a> {
    fn get_connectsTo(&'a mut self) -> &mut Vec<&'a mut TrackElementImpl<'a>> {
        &mut self.connectsTo
    }
}
impl <'a> RailwayElement<'a> for SegmentImpl<'a> {
    fn get_id(&'a self) -> Option<i32> {
        self.id.clone()
    }
    fn set_id(&'a mut self, value: Option<i32>) {
        self.id = value
    }
}

impl <'a> TrackElement<'a> for TrackElementImpl<'a> {
    fn get_connectsTo(&'a mut self) -> &mut Vec<&'a mut TrackElementImpl<'a>> {
        match *self {
            TrackElementImpl::Segment(ref mut i) => i.get_connectsTo(),
            TrackElementImpl::Switch(ref mut i) => i.get_connectsTo(),
        }
    }
}
impl <'a> RailwayElement<'a> for TrackElementImpl<'a> {
    fn get_id(&'a self) -> Option<i32> {
        match *self {
            TrackElementImpl::Segment(ref i) => i.get_id(),
            TrackElementImpl::Switch(ref i) => i.get_id(),
        }
    }
    fn set_id(&'a mut self, value: Option<i32>) {
        match *self {
            TrackElementImpl::Segment(ref mut i) => i.set_id(value),
            TrackElementImpl::Switch(ref mut i) => i.set_id(value),
        }
    }
}

impl <'a> Switch<'a> for SwitchImpl<'a> {
    fn get_currentPosition(&'a self) -> Option<Position> {
        self.currentPosition.clone()
    }
    fn set_currentPosition(&'a mut self, value: Option<Position>) {
        self.currentPosition = value
    }
    fn get_positions(&'a mut self) -> &mut Vec<&'a mut SwitchPositionImpl<'a>> {
        &mut self.positions
    }
}
impl <'a> TrackElement<'a> for SwitchImpl<'a> {
    fn get_connectsTo(&'a mut self) -> &mut Vec<&'a mut TrackElementImpl<'a>> {
        &mut self.connectsTo
    }
}
impl <'a> RailwayElement<'a> for SwitchImpl<'a> {
    fn get_id(&'a self) -> Option<i32> {
        self.id.clone()
    }
    fn set_id(&'a mut self, value: Option<i32>) {
        self.id = value
    }
}

impl <'a> Route<'a> for RouteImpl<'a> {
    fn get_entry(&'a self) -> Option<&'a mut SemaphoreImpl<'a>> {
        self.entry.clone()
    }
    fn set_entry(&'a mut self, value: Option<&'a mut SemaphoreImpl<'a>>) {
        self.entry = value
    }
    fn get_follows(&'a mut self) -> &mut Vec<&'a mut SwitchPositionImpl<'a>> {
        &mut self.follows
    }
    fn get_exit(&'a self) -> Option<&'a mut SemaphoreImpl<'a>> {
        self.exit.clone()
    }
    fn set_exit(&'a mut self, value: Option<&'a mut SemaphoreImpl<'a>>) {
        self.exit = value
    }
    fn get_definedBy(&'a mut self) -> &mut Vec<&'a mut SensorImpl<'a>> {
        &mut self.definedBy
    }
}
impl <'a> RailwayElement<'a> for RouteImpl<'a> {
    fn get_id(&'a self) -> Option<i32> {
        self.id.clone()
    }
    fn set_id(&'a mut self, value: Option<i32>) {
        self.id = value
    }
}

impl <'a> Semaphore<'a> for SemaphoreImpl<'a> {
    fn get_signal(&'a self) -> Option<Signal> {
        self.signal.clone()
    }
    fn set_signal(&'a mut self, value: Option<Signal>) {
        self.signal = value
    }
}
impl <'a> RailwayElement<'a> for SemaphoreImpl<'a> {
    fn get_id(&'a self) -> Option<i32> {
        self.id.clone()
    }
    fn set_id(&'a mut self, value: Option<i32>) {
        self.id = value
    }
}

impl <'a> SwitchPosition<'a> for SwitchPositionImpl<'a> {
    fn get_switch(&'a self) -> Option<&'a mut SwitchImpl<'a>> {
        self.switch.clone()
    }
    fn set_switch(&'a mut self, value: Option<&'a mut SwitchImpl<'a>>) {
        self.switch = value
    }
    fn get_position(&'a self) -> Option<Position> {
        self.position.clone()
    }
    fn set_position(&'a mut self, value: Option<Position>) {
        self.position = value
    }
}
impl <'a> RailwayElement<'a> for SwitchPositionImpl<'a> {
    fn get_id(&'a self) -> Option<i32> {
        self.id.clone()
    }
    fn set_id(&'a mut self, value: Option<i32>) {
        self.id = value
    }
}

impl <'a> RailwayElement<'a> for RailwayElementImpl<'a> {
    fn get_id(&'a self) -> Option<i32> {
        match *self {
            RailwayElementImpl::TrackElement(ref i) => i.get_id(),
            RailwayElementImpl::Route(ref i) => i.get_id(),
            RailwayElementImpl::Semaphore(ref i) => i.get_id(),
            RailwayElementImpl::SwitchPosition(ref i) => i.get_id(),
            RailwayElementImpl::Sensor(ref i) => i.get_id(),
        }
    }
    fn set_id(&'a mut self, value: Option<i32>) {
        match *self {
            RailwayElementImpl::TrackElement(ref mut i) => i.set_id(value),
            RailwayElementImpl::Route(ref mut i) => i.set_id(value),
            RailwayElementImpl::Semaphore(ref mut i) => i.set_id(value),
            RailwayElementImpl::SwitchPosition(ref mut i) => i.set_id(value),
            RailwayElementImpl::Sensor(ref mut i) => i.set_id(value),
        }
    }
}

impl <'a> Sensor<'a> for SensorImpl<'a> {
    fn get_elements(&'a mut self) -> &mut Vec<&'a mut TrackElementImpl<'a>> {
        &mut self.elements
    }
}
impl <'a> RailwayElement<'a> for SensorImpl<'a> {
    fn get_id(&'a self) -> Option<i32> {
        self.id.clone()
    }
    fn set_id(&'a mut self, value: Option<i32>) {
        self.id = value
    }
}

impl <'a> RailwayContainer<'a> for RailwayContainerImpl<'a> {
    fn get_invalids(&'a mut self) -> &mut Vec<&'a mut RailwayElementImpl<'a>> {
        &mut self.invalids
    }
    fn get_semaphores(&'a mut self) -> &mut Vec<&'a mut SemaphoreImpl<'a>> {
        &mut self.semaphores
    }
    fn get_routes(&'a mut self) -> &mut Vec<&'a mut RouteImpl<'a>> {
        &mut self.routes
    }
}

