//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

using NMF.Collections.Generic;
using NMF.Collections.ObjectModel;
using NMF.Expressions;
using NMF.Expressions.Linq;
using NMF.Models;
using NMF.Models.Collections;
using NMF.Models.Expressions;
using NMF.Models.Meta;
using NMF.Models.Repository;
using NMF.Serialization;
using NMF.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace TTC2015.TrainBenchmark.Railway
{
    
    
    /// <summary>
    /// The default implementation of the SwitchPosition class
    /// </summary>
    [XmlNamespaceAttribute("http://www.semanticweb.org/ontologies/2015/ttc/trainbenchmark")]
    [XmlNamespacePrefixAttribute("hu.bme.mit.trainbenchmark")]
    [ModelRepresentationClassAttribute("http://www.semanticweb.org/ontologies/2015/ttc/trainbenchmark#//SwitchPosition")]
    public partial class SwitchPosition : RailwayElement, ISwitchPosition, IModelElement
    {
        
        /// <summary>
        /// The backing field for the Position property
        /// </summary>
        private Position _position;
        
        private static Lazy<ITypedElement> _positionAttribute = new Lazy<ITypedElement>(RetrievePositionAttribute);
        
        private static Lazy<ITypedElement> _switchReference = new Lazy<ITypedElement>(RetrieveSwitchReference);
        
        /// <summary>
        /// The backing field for the Switch property
        /// </summary>
        private ISwitch _switch;
        
        private static Lazy<ITypedElement> _routeReference = new Lazy<ITypedElement>(RetrieveRouteReference);
        
        private static IClass _classInstance;
        
        /// <summary>
        /// The position property
        /// </summary>
        [XmlElementNameAttribute("position")]
        [XmlAttributeAttribute(true)]
        public Position Position
        {
            get
            {
                return this._position;
            }
            set
            {
                if ((this._position != value))
                {
                    Position old = this._position;
                    ValueChangedEventArgs e = new ValueChangedEventArgs(old, value);
                    this.OnPositionChanging(e);
                    this.OnPropertyChanging("Position", e, _positionAttribute);
                    this._position = value;
                    this.OnPositionChanged(e);
                    this.OnPropertyChanged("Position", e, _positionAttribute);
                }
            }
        }
        
        /// <summary>
        /// The switch property
        /// </summary>
        [XmlElementNameAttribute("switch")]
        [XmlAttributeAttribute(true)]
        [XmlOppositeAttribute("positions")]
        public ISwitch Switch
        {
            get
            {
                return this._switch;
            }
            set
            {
                if ((this._switch != value))
                {
                    ISwitch old = this._switch;
                    ValueChangedEventArgs e = new ValueChangedEventArgs(old, value);
                    this.OnSwitchChanging(e);
                    this.OnPropertyChanging("Switch", e, _switchReference);
                    this._switch = value;
                    if ((old != null))
                    {
                        old.Positions.Remove(this);
                        old.Deleted -= this.OnResetSwitch;
                    }
                    if ((value != null))
                    {
                        if ((value.Positions.Contains(this) != true))
                        {
                            value.Positions.Add(this);
                        }
                        value.Deleted += this.OnResetSwitch;
                    }
                    this.OnSwitchChanged(e);
                    this.OnPropertyChanged("Switch", e, _switchReference);
                }
            }
        }
        
        /// <summary>
        /// The route property
        /// </summary>
        [XmlElementNameAttribute("route")]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [XmlAttributeAttribute(true)]
        [XmlOppositeAttribute("follows")]
        public IRoute Route
        {
            get
            {
                return ModelHelper.CastAs<IRoute>(this.Parent);
            }
            set
            {
                this.Parent = value;
            }
        }
        
        /// <summary>
        /// Gets the referenced model elements of this model element
        /// </summary>
        public override IEnumerableExpression<IModelElement> ReferencedElements
        {
            get
            {
                return base.ReferencedElements.Concat(new SwitchPositionReferencedElementsCollection(this));
            }
        }
        
        /// <summary>
        /// Gets the Class model for this type
        /// </summary>
        public new static IClass ClassInstance
        {
            get
            {
                if ((_classInstance == null))
                {
                    _classInstance = ((IClass)(MetaRepository.Instance.Resolve("http://www.semanticweb.org/ontologies/2015/ttc/trainbenchmark#//SwitchPosition")));
                }
                return _classInstance;
            }
        }
        
        /// <summary>
        /// Gets fired before the Position property changes its value
        /// </summary>
        public event System.EventHandler<ValueChangedEventArgs> PositionChanging;
        
        /// <summary>
        /// Gets fired when the Position property changed its value
        /// </summary>
        public event System.EventHandler<ValueChangedEventArgs> PositionChanged;
        
        /// <summary>
        /// Gets fired before the Switch property changes its value
        /// </summary>
        public event System.EventHandler<ValueChangedEventArgs> SwitchChanging;
        
        /// <summary>
        /// Gets fired when the Switch property changed its value
        /// </summary>
        public event System.EventHandler<ValueChangedEventArgs> SwitchChanged;
        
        /// <summary>
        /// Gets fired before the Route property changes its value
        /// </summary>
        public event System.EventHandler<ValueChangedEventArgs> RouteChanging;
        
        /// <summary>
        /// Gets fired when the Route property changed its value
        /// </summary>
        public event System.EventHandler<ValueChangedEventArgs> RouteChanged;
        
        private static ITypedElement RetrievePositionAttribute()
        {
            return ((ITypedElement)(((ModelElement)(TTC2015.TrainBenchmark.Railway.SwitchPosition.ClassInstance)).Resolve("position")));
        }
        
        /// <summary>
        /// Raises the PositionChanging event
        /// </summary>
        /// <param name="eventArgs">The event data</param>
        protected virtual void OnPositionChanging(ValueChangedEventArgs eventArgs)
        {
            System.EventHandler<ValueChangedEventArgs> handler = this.PositionChanging;
            if ((handler != null))
            {
                handler.Invoke(this, eventArgs);
            }
        }
        
        /// <summary>
        /// Raises the PositionChanged event
        /// </summary>
        /// <param name="eventArgs">The event data</param>
        protected virtual void OnPositionChanged(ValueChangedEventArgs eventArgs)
        {
            System.EventHandler<ValueChangedEventArgs> handler = this.PositionChanged;
            if ((handler != null))
            {
                handler.Invoke(this, eventArgs);
            }
        }
        
        private static ITypedElement RetrieveSwitchReference()
        {
            return ((ITypedElement)(((ModelElement)(TTC2015.TrainBenchmark.Railway.SwitchPosition.ClassInstance)).Resolve("switch")));
        }
        
        /// <summary>
        /// Raises the SwitchChanging event
        /// </summary>
        /// <param name="eventArgs">The event data</param>
        protected virtual void OnSwitchChanging(ValueChangedEventArgs eventArgs)
        {
            System.EventHandler<ValueChangedEventArgs> handler = this.SwitchChanging;
            if ((handler != null))
            {
                handler.Invoke(this, eventArgs);
            }
        }
        
        /// <summary>
        /// Raises the SwitchChanged event
        /// </summary>
        /// <param name="eventArgs">The event data</param>
        protected virtual void OnSwitchChanged(ValueChangedEventArgs eventArgs)
        {
            System.EventHandler<ValueChangedEventArgs> handler = this.SwitchChanged;
            if ((handler != null))
            {
                handler.Invoke(this, eventArgs);
            }
        }
        
        /// <summary>
        /// Handles the event that the Switch property must reset
        /// </summary>
        /// <param name="sender">The object that sent this reset request</param>
        /// <param name="eventArgs">The event data for the reset event</param>
        private void OnResetSwitch(object sender, System.EventArgs eventArgs)
        {
            this.Switch = null;
        }
        
        private static ITypedElement RetrieveRouteReference()
        {
            return ((ITypedElement)(((ModelElement)(TTC2015.TrainBenchmark.Railway.SwitchPosition.ClassInstance)).Resolve("route")));
        }
        
        /// <summary>
        /// Raises the RouteChanging event
        /// </summary>
        /// <param name="eventArgs">The event data</param>
        protected virtual void OnRouteChanging(ValueChangedEventArgs eventArgs)
        {
            System.EventHandler<ValueChangedEventArgs> handler = this.RouteChanging;
            if ((handler != null))
            {
                handler.Invoke(this, eventArgs);
            }
        }
        
        /// <summary>
        /// Gets called when the parent model element of the current model element is about to change
        /// </summary>
        /// <param name="oldParent">The old parent model element</param>
        /// <param name="newParent">The new parent model element</param>
        protected override void OnParentChanging(IModelElement newParent, IModelElement oldParent)
        {
            IRoute oldRoute = ModelHelper.CastAs<IRoute>(oldParent);
            IRoute newRoute = ModelHelper.CastAs<IRoute>(newParent);
            ValueChangedEventArgs e = new ValueChangedEventArgs(oldRoute, newRoute);
            this.OnRouteChanging(e);
            this.OnPropertyChanging("Route", e, _routeReference);
        }
        
        /// <summary>
        /// Raises the RouteChanged event
        /// </summary>
        /// <param name="eventArgs">The event data</param>
        protected virtual void OnRouteChanged(ValueChangedEventArgs eventArgs)
        {
            System.EventHandler<ValueChangedEventArgs> handler = this.RouteChanged;
            if ((handler != null))
            {
                handler.Invoke(this, eventArgs);
            }
        }
        
        /// <summary>
        /// Gets called when the parent model element of the current model element changes
        /// </summary>
        /// <param name="oldParent">The old parent model element</param>
        /// <param name="newParent">The new parent model element</param>
        protected override void OnParentChanged(IModelElement newParent, IModelElement oldParent)
        {
            IRoute oldRoute = ModelHelper.CastAs<IRoute>(oldParent);
            IRoute newRoute = ModelHelper.CastAs<IRoute>(newParent);
            if ((oldRoute != null))
            {
                oldRoute.Follows.Remove(this);
            }
            if ((newRoute != null))
            {
                newRoute.Follows.Add(this);
            }
            ValueChangedEventArgs e = new ValueChangedEventArgs(oldRoute, newRoute);
            this.OnRouteChanged(e);
            this.OnPropertyChanged("Route", e, _routeReference);
            base.OnParentChanged(newParent, oldParent);
        }
        
        /// <summary>
        /// Resolves the given attribute name
        /// </summary>
        /// <returns>The attribute value or null if it could not be found</returns>
        /// <param name="attribute">The requested attribute name</param>
        /// <param name="index">The index of this attribute</param>
        protected override object GetAttributeValue(string attribute, int index)
        {
            if ((attribute == "POSITION"))
            {
                return this.Position;
            }
            return base.GetAttributeValue(attribute, index);
        }
        
        /// <summary>
        /// Sets a value to the given feature
        /// </summary>
        /// <param name="feature">The requested feature</param>
        /// <param name="value">The value that should be set to that feature</param>
        protected override void SetFeature(string feature, object value)
        {
            if ((feature == "SWITCH"))
            {
                this.Switch = ((ISwitch)(value));
                return;
            }
            if ((feature == "ROUTE"))
            {
                this.Route = ((IRoute)(value));
                return;
            }
            if ((feature == "POSITION"))
            {
                this.Position = ((Position)(value));
                return;
            }
            base.SetFeature(feature, value);
        }
        
        /// <summary>
        /// Gets the property expression for the given attribute
        /// </summary>
        /// <returns>An incremental property expression</returns>
        /// <param name="attribute">The requested attribute in upper case</param>
        protected override NMF.Expressions.INotifyExpression<object> GetExpressionForAttribute(string attribute)
        {
            if ((attribute == "Switch"))
            {
                return new SwitchProxy(this);
            }
            if ((attribute == "Route"))
            {
                return new RouteProxy(this);
            }
            return base.GetExpressionForAttribute(attribute);
        }
        
        /// <summary>
        /// Gets the property expression for the given reference
        /// </summary>
        /// <returns>An incremental property expression</returns>
        /// <param name="reference">The requested reference in upper case</param>
        protected override NMF.Expressions.INotifyExpression<NMF.Models.IModelElement> GetExpressionForReference(string reference)
        {
            if ((reference == "Switch"))
            {
                return new SwitchProxy(this);
            }
            if ((reference == "Route"))
            {
                return new RouteProxy(this);
            }
            return base.GetExpressionForReference(reference);
        }
        
        /// <summary>
        /// Gets the Class for this model element
        /// </summary>
        public override IClass GetClass()
        {
            if ((_classInstance == null))
            {
                _classInstance = ((IClass)(MetaRepository.Instance.Resolve("http://www.semanticweb.org/ontologies/2015/ttc/trainbenchmark#//SwitchPosition")));
            }
            return _classInstance;
        }
        
        /// <summary>
        /// The collection class to to represent the children of the SwitchPosition class
        /// </summary>
        public class SwitchPositionReferencedElementsCollection : ReferenceCollection, ICollectionExpression<IModelElement>, ICollection<IModelElement>
        {
            
            private SwitchPosition _parent;
            
            /// <summary>
            /// Creates a new instance
            /// </summary>
            public SwitchPositionReferencedElementsCollection(SwitchPosition parent)
            {
                this._parent = parent;
            }
            
            /// <summary>
            /// Gets the amount of elements contained in this collection
            /// </summary>
            public override int Count
            {
                get
                {
                    int count = 0;
                    if ((this._parent.Switch != null))
                    {
                        count = (count + 1);
                    }
                    if ((this._parent.Route != null))
                    {
                        count = (count + 1);
                    }
                    return count;
                }
            }
            
            protected override void AttachCore()
            {
                this._parent.SwitchChanged += this.PropagateValueChanges;
                this._parent.RouteChanged += this.PropagateValueChanges;
            }
            
            protected override void DetachCore()
            {
                this._parent.SwitchChanged -= this.PropagateValueChanges;
                this._parent.RouteChanged -= this.PropagateValueChanges;
            }
            
            /// <summary>
            /// Adds the given element to the collection
            /// </summary>
            /// <param name="item">The item to add</param>
            public override void Add(IModelElement item)
            {
                if ((this._parent.Switch == null))
                {
                    ISwitch switchCasted = item.As<ISwitch>();
                    if ((switchCasted != null))
                    {
                        this._parent.Switch = switchCasted;
                        return;
                    }
                }
                if ((this._parent.Route == null))
                {
                    IRoute routeCasted = item.As<IRoute>();
                    if ((routeCasted != null))
                    {
                        this._parent.Route = routeCasted;
                        return;
                    }
                }
            }
            
            /// <summary>
            /// Clears the collection and resets all references that implement it.
            /// </summary>
            public override void Clear()
            {
                this._parent.Switch = null;
                this._parent.Route = null;
            }
            
            /// <summary>
            /// Gets a value indicating whether the given element is contained in the collection
            /// </summary>
            /// <returns>True, if it is contained, otherwise False</returns>
            /// <param name="item">The item that should be looked out for</param>
            public override bool Contains(IModelElement item)
            {
                if ((item == this._parent.Switch))
                {
                    return true;
                }
                if ((item == this._parent.Route))
                {
                    return true;
                }
                return false;
            }
            
            /// <summary>
            /// Copies the contents of the collection to the given array starting from the given array index
            /// </summary>
            /// <param name="array">The array in which the elements should be copied</param>
            /// <param name="arrayIndex">The starting index</param>
            public override void CopyTo(IModelElement[] array, int arrayIndex)
            {
                if ((this._parent.Switch != null))
                {
                    array[arrayIndex] = this._parent.Switch;
                    arrayIndex = (arrayIndex + 1);
                }
                if ((this._parent.Route != null))
                {
                    array[arrayIndex] = this._parent.Route;
                    arrayIndex = (arrayIndex + 1);
                }
            }
            
            /// <summary>
            /// Removes the given item from the collection
            /// </summary>
            /// <returns>True, if the item was removed, otherwise False</returns>
            /// <param name="item">The item that should be removed</param>
            public override bool Remove(IModelElement item)
            {
                if ((this._parent.Switch == item))
                {
                    this._parent.Switch = null;
                    return true;
                }
                if ((this._parent.Route == item))
                {
                    this._parent.Route = null;
                    return true;
                }
                return false;
            }
            
            /// <summary>
            /// Gets an enumerator that enumerates the collection
            /// </summary>
            /// <returns>A generic enumerator</returns>
            public override IEnumerator<IModelElement> GetEnumerator()
            {
                return Enumerable.Empty<IModelElement>().Concat(this._parent.Switch).Concat(this._parent.Route).GetEnumerator();
            }
        }
        
        /// <summary>
        /// Represents a proxy to represent an incremental access to the position property
        /// </summary>
        private sealed class PositionProxy : ModelPropertyChange<ISwitchPosition, Position>
        {
            
            /// <summary>
            /// Creates a new observable property access proxy
            /// </summary>
            /// <param name="modelElement">The model instance element for which to create the property access proxy</param>
            public PositionProxy(ISwitchPosition modelElement) : 
                    base(modelElement, "position")
            {
            }
            
            /// <summary>
            /// Gets or sets the value of this expression
            /// </summary>
            public override Position Value
            {
                get
                {
                    return this.ModelElement.Position;
                }
                set
                {
                    this.ModelElement.Position = value;
                }
            }
        }
        
        /// <summary>
        /// Represents a proxy to represent an incremental access to the switch property
        /// </summary>
        private sealed class SwitchProxy : ModelPropertyChange<ISwitchPosition, ISwitch>
        {
            
            /// <summary>
            /// Creates a new observable property access proxy
            /// </summary>
            /// <param name="modelElement">The model instance element for which to create the property access proxy</param>
            public SwitchProxy(ISwitchPosition modelElement) : 
                    base(modelElement, "switch")
            {
            }
            
            /// <summary>
            /// Gets or sets the value of this expression
            /// </summary>
            public override ISwitch Value
            {
                get
                {
                    return this.ModelElement.Switch;
                }
                set
                {
                    this.ModelElement.Switch = value;
                }
            }
        }
        
        /// <summary>
        /// Represents a proxy to represent an incremental access to the route property
        /// </summary>
        private sealed class RouteProxy : ModelPropertyChange<ISwitchPosition, IRoute>
        {
            
            /// <summary>
            /// Creates a new observable property access proxy
            /// </summary>
            /// <param name="modelElement">The model instance element for which to create the property access proxy</param>
            public RouteProxy(ISwitchPosition modelElement) : 
                    base(modelElement, "route")
            {
            }
            
            /// <summary>
            /// Gets or sets the value of this expression
            /// </summary>
            public override IRoute Value
            {
                get
                {
                    return this.ModelElement.Route;
                }
                set
                {
                    this.ModelElement.Route = value;
                }
            }
        }
    }
}
