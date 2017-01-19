pub enum Signal {
    FAILURE,
    STOP,
    GO
}
pub enum Position {
    FAILURE,
    LEFT,
    RIGHT,
    STRAIGHT
}
pub trait Segment {
    fn get_length(&self) -> i32;
    fn set_length(&mut self, value: i32);
}
pub trait TrackElement {
    fn get_sensor(&self) -> Box<Sensor>;
    fn set_sensor(&mut self, value: Box<Sensor>);
    fn get_connectsTo(&self) -> Vec<Box<TrackElement>>;
}
pub trait Switch {
    fn get_currentPosition(&self) -> Position;
    fn set_currentPosition(&mut self, value: Position);
    fn get_positions(&self) -> Vec<Box<SwitchPosition>>;
}
pub trait Route {
    fn get_entry(&self) -> Box<Semaphore>;
    fn set_entry(&mut self, value: Box<Semaphore>);
    fn get_follows(&self) -> Vec<Box<SwitchPosition>>;
    fn get_exit(&self) -> Box<Semaphore>;
    fn set_exit(&mut self, value: Box<Semaphore>);
    fn get_definedBy(&self) -> Vec<Box<Sensor>>;
}
pub trait Semaphore {
    fn get_signal(&self) -> Signal;
    fn set_signal(&mut self, value: Signal);
}
pub trait SwitchPosition {
    fn get_switch(&self) -> Box<Switch>;
    fn set_switch(&mut self, value: Box<Switch>);
    fn get_position(&self) -> Position;
    fn set_position(&mut self, value: Position);
    fn get_route(&self) -> Box<Route>;
    fn set_route(&mut self, value: Box<Route>);
}
pub trait RailwayElement {
    fn get_id(&self) -> i32;
    fn set_id(&mut self, value: i32);
}
pub trait Sensor {
    fn get_elements(&self) -> Vec<Box<TrackElement>>;
}
pub trait RailwayContainer {
    fn get_invalids(&self) -> Vec<Box<RailwayElement>>;
    fn get_semaphores(&self) -> Vec<Box<Semaphore>>;
    fn get_routes(&self) -> Vec<Box<Route>>;
}
pub struct SegmentImpl { length: i32, sensor: Box<Sensor>, connectsTo: Vec<Box<TrackElement>>, id: i32}
pub enum TrackElementImpl {
    Segment(SegmentImpl),
    Switch(SwitchImpl)
}
pub struct SwitchImpl { currentPosition: Position, positions: Vec<Box<SwitchPosition>>, sensor: Box<Sensor>, connectsTo: Vec<Box<TrackElement>>, id: i32}
pub struct RouteImpl { entry: Box<Semaphore>, follows: Vec<Box<SwitchPosition>>, exit: Box<Semaphore>, definedBy: Vec<Box<Sensor>>, id: i32}
pub struct SemaphoreImpl { signal: Signal, id: i32}
pub struct SwitchPositionImpl { switch: Box<Switch>, position: Position, route: Box<Route>, id: i32}
pub enum RailwayElementImpl {
    TrackElement(TrackElementImpl),
    Route(RouteImpl),
    Semaphore(SemaphoreImpl),
    SwitchPosition(SwitchPositionImpl),
    Sensor(SensorImpl)
}
pub struct SensorImpl { elements: Vec<Box<TrackElement>>, id: i32}
pub struct RailwayContainerImpl { invalids: Vec<Box<RailwayElement>>, semaphores: Vec<Box<Semaphore>>, routes: Vec<Box<Route>>}
impl Segment for SegmentImpl {
    fn get_length(&self) -> i32 {
        self.length
    }
    fn set_length(&mut self, value: i32) {
        self.length = value
    }
}
impl TrackElement for SegmentImpl {
    fn get_sensor(&self) -> Box<Sensor> {
        self.sensor
    }
    fn set_sensor(&mut self, value: Box<Sensor>) {
        self.sensor = value
    }
    fn get_connectsTo(&self) -> Vec<Box<TrackElement>> {
        self.connectsTo
    }
}
impl RailwayElement for SegmentImpl {
    fn get_id(&self) -> i32 {
        self.id
    }
    fn set_id(&mut self, value: i32) {
        self.id = value
    }
}
impl TrackElement for TrackElementImpl {
    fn get_sensor(&self) -> Box<Sensor> {
        match self* {
            TrackElementImpl::Segment(i) => i.sensor
            TrackElementImpl::Switch(i) => i.sensor
        }
    }
    fn set_sensor(&mut self, value: Box<Sensor>) {
        match self* {
            TrackElementImpl::Segment(i) => i.sensor = value
            TrackElementImpl::Switch(i) => i.sensor = value
        }
    }
    fn get_connectsTo(&self) -> Vec<Box<TrackElement>> {
        match self* {
            TrackElementImpl::Segment(i) => i.connectsTo
            TrackElementImpl::Switch(i) => i.connectsTo
        }
    }
}
impl RailwayElement for TrackElementImpl {
    fn get_id(&self) -> i32 {
        self.id
    }
    fn set_id(&mut self, value: i32) {
        self.id = value
    }
}
impl Switch for SwitchImpl {
    fn get_currentPosition(&self) -> Position {
        self.currentPosition
    }
    fn set_currentPosition(&mut self, value: Position) {
        self.currentPosition = value
    }
    fn get_positions(&self) -> Vec<Box<SwitchPosition>> {
        self.positions
    }
}
impl TrackElement for SwitchImpl {
    fn get_sensor(&self) -> Box<Sensor> {
        self.sensor
    }
    fn set_sensor(&mut self, value: Box<Sensor>) {
        self.sensor = value
    }
    fn get_connectsTo(&self) -> Vec<Box<TrackElement>> {
        self.connectsTo
    }
}
impl RailwayElement for SwitchImpl {
    fn get_id(&self) -> i32 {
        self.id
    }
    fn set_id(&mut self, value: i32) {
        self.id = value
    }
}
impl Route for RouteImpl {
    fn get_entry(&self) -> Box<Semaphore> {
        self.entry
    }
    fn set_entry(&mut self, value: Box<Semaphore>) {
        self.entry = value
    }
    fn get_follows(&self) -> Vec<Box<SwitchPosition>> {
        self.follows
    }
    fn get_exit(&self) -> Box<Semaphore> {
        self.exit
    }
    fn set_exit(&mut self, value: Box<Semaphore>) {
        self.exit = value
    }
    fn get_definedBy(&self) -> Vec<Box<Sensor>> {
        self.definedBy
    }
}
impl RailwayElement for RouteImpl {
    fn get_id(&self) -> i32 {
        self.id
    }
    fn set_id(&mut self, value: i32) {
        self.id = value
    }
}
impl Semaphore for SemaphoreImpl {
    fn get_signal(&self) -> Signal {
        self.signal
    }
    fn set_signal(&mut self, value: Signal) {
        self.signal = value
    }
}
impl RailwayElement for SemaphoreImpl {
    fn get_id(&self) -> i32 {
        self.id
    }
    fn set_id(&mut self, value: i32) {
        self.id = value
    }
}
impl SwitchPosition for SwitchPositionImpl {
    fn get_switch(&self) -> Box<Switch> {
        self.switch
    }
    fn set_switch(&mut self, value: Box<Switch>) {
        self.switch = value
    }
    fn get_position(&self) -> Position {
        self.position
    }
    fn set_position(&mut self, value: Position) {
        self.position = value
    }
    fn get_route(&self) -> Box<Route> {
        self.route
    }
    fn set_route(&mut self, value: Box<Route>) {
        self.route = value
    }
}
impl RailwayElement for SwitchPositionImpl {
    fn get_id(&self) -> i32 {
        self.id
    }
    fn set_id(&mut self, value: i32) {
        self.id = value
    }
}
impl RailwayElement for RailwayElementImpl {
    fn get_id(&self) -> i32 {
        match self* {
            RailwayElementImpl::TrackElement(i) => i.id
            RailwayElementImpl::Route(i) => i.id
            RailwayElementImpl::Semaphore(i) => i.id
            RailwayElementImpl::SwitchPosition(i) => i.id
            RailwayElementImpl::Sensor(i) => i.id
        }
    }
    fn set_id(&mut self, value: i32) {
        match self* {
            RailwayElementImpl::TrackElement(i) => i.id = value
            RailwayElementImpl::Route(i) => i.id = value
            RailwayElementImpl::Semaphore(i) => i.id = value
            RailwayElementImpl::SwitchPosition(i) => i.id = value
            RailwayElementImpl::Sensor(i) => i.id = value
        }
    }
}
impl Sensor for SensorImpl {
    fn get_elements(&self) -> Vec<Box<TrackElement>> {
        self.elements
    }
}
impl RailwayElement for SensorImpl {
    fn get_id(&self) -> i32 {
        self.id
    }
    fn set_id(&mut self, value: i32) {
        self.id = value
    }
}
impl RailwayContainer for RailwayContainerImpl {
    fn get_invalids(&self) -> Vec<Box<RailwayElement>> {
        self.invalids
    }
    fn get_semaphores(&self) -> Vec<Box<Semaphore>> {
        self.semaphores
    }
    fn get_routes(&self) -> Vec<Box<Route>> {
        self.routes
    }
}
