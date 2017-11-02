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
    /// The default implementation of the TrackElement class
    /// </summary>
    [XmlNamespaceAttribute("http://www.semanticweb.org/ontologies/2015/ttc/trainbenchmark")]
    [XmlNamespacePrefixAttribute("hu.bme.mit.trainbenchmark")]
    [ModelRepresentationClassAttribute("http://www.semanticweb.org/ontologies/2015/ttc/trainbenchmark#//TrackElement")]
    public abstract partial class TrackElement : RailwayElement, ITrackElement, IModelElement
    {
        
        private static Lazy<ITypedElement> _sensorReference = new Lazy<ITypedElement>(RetrieveSensorReference);
        
        private static Lazy<ITypedElement> _connectsToReference = new Lazy<ITypedElement>(RetrieveConnectsToReference);
        
        /// <summary>
        /// The backing field for the ConnectsTo property
        /// </summary>
        private ObservableAssociationList<ITrackElement> _connectsTo;
        
        private static IClass _classInstance;
        
        public TrackElement()
        {
            this._connectsTo = new ObservableAssociationList<ITrackElement>();
            this._connectsTo.CollectionChanging += this.ConnectsToCollectionChanging;
            this._connectsTo.CollectionChanged += this.ConnectsToCollectionChanged;
        }
        
        /// <summary>
        /// The sensor property
        /// </summary>
        [XmlElementNameAttribute("sensor")]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [XmlAttributeAttribute(true)]
        [XmlOppositeAttribute("elements")]
        public ISensor Sensor
        {
            get
            {
                return ModelHelper.CastAs<ISensor>(this.Parent);
            }
            set
            {
                this.Parent = value;
            }
        }
        
        /// <summary>
        /// The connectsTo property
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [XmlElementNameAttribute("connectsTo")]
        [XmlAttributeAttribute(true)]
        [ConstantAttribute()]
        public IListExpression<ITrackElement> ConnectsTo
        {
            get
            {
                return this._connectsTo;
            }
        }
        
        /// <summary>
        /// Gets the referenced model elements of this model element
        /// </summary>
        public override IEnumerableExpression<IModelElement> ReferencedElements
        {
            get
            {
                return base.ReferencedElements.Concat(new TrackElementReferencedElementsCollection(this));
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
                    _classInstance = ((IClass)(MetaRepository.Instance.Resolve("http://www.semanticweb.org/ontologies/2015/ttc/trainbenchmark#//TrackElement")));
                }
                return _classInstance;
            }
        }
        
        /// <summary>
        /// Gets fired before the Sensor property changes its value
        /// </summary>
        public event System.EventHandler<ValueChangedEventArgs> SensorChanging;
        
        /// <summary>
        /// Gets fired when the Sensor property changed its value
        /// </summary>
        public event System.EventHandler<ValueChangedEventArgs> SensorChanged;
        
        private static ITypedElement RetrieveSensorReference()
        {
            return ((ITypedElement)(((ModelElement)(TTC2015.TrainBenchmark.Railway.TrackElement.ClassInstance)).Resolve("sensor")));
        }
        
        /// <summary>
        /// Raises the SensorChanging event
        /// </summary>
        /// <param name="eventArgs">The event data</param>
        protected virtual void OnSensorChanging(ValueChangedEventArgs eventArgs)
        {
            System.EventHandler<ValueChangedEventArgs> handler = this.SensorChanging;
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
            ISensor oldSensor = ModelHelper.CastAs<ISensor>(oldParent);
            ISensor newSensor = ModelHelper.CastAs<ISensor>(newParent);
            ValueChangedEventArgs e = new ValueChangedEventArgs(oldSensor, newSensor);
            this.OnSensorChanging(e);
            this.OnPropertyChanging("Sensor", e, _sensorReference);
        }
        
        /// <summary>
        /// Raises the SensorChanged event
        /// </summary>
        /// <param name="eventArgs">The event data</param>
        protected virtual void OnSensorChanged(ValueChangedEventArgs eventArgs)
        {
            System.EventHandler<ValueChangedEventArgs> handler = this.SensorChanged;
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
            ISensor oldSensor = ModelHelper.CastAs<ISensor>(oldParent);
            ISensor newSensor = ModelHelper.CastAs<ISensor>(newParent);
            if ((oldSensor != null))
            {
                oldSensor.Elements.Remove(this);
            }
            if ((newSensor != null))
            {
                newSensor.Elements.Add(this);
            }
            ValueChangedEventArgs e = new ValueChangedEventArgs(oldSensor, newSensor);
            this.OnSensorChanged(e);
            this.OnPropertyChanged("Sensor", e, _sensorReference);
            base.OnParentChanged(newParent, oldParent);
        }
        
        private static ITypedElement RetrieveConnectsToReference()
        {
            return ((ITypedElement)(((ModelElement)(TTC2015.TrainBenchmark.Railway.TrackElement.ClassInstance)).Resolve("connectsTo")));
        }
        
        /// <summary>
        /// Forwards CollectionChanging notifications for the ConnectsTo property to the parent model element
        /// </summary>
        /// <param name="sender">The collection that raised the change</param>
        /// <param name="e">The original event data</param>
        private void ConnectsToCollectionChanging(object sender, NotifyCollectionChangingEventArgs e)
        {
            this.OnCollectionChanging("ConnectsTo", e, _connectsToReference);
        }
        
        /// <summary>
        /// Forwards CollectionChanged notifications for the ConnectsTo property to the parent model element
        /// </summary>
        /// <param name="sender">The collection that raised the change</param>
        /// <param name="e">The original event data</param>
        private void ConnectsToCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnCollectionChanged("ConnectsTo", e, _connectsToReference);
        }
        
        /// <summary>
        /// Gets the Model element collection for the given feature
        /// </summary>
        /// <returns>A non-generic list of elements</returns>
        /// <param name="feature">The requested feature</param>
        protected override System.Collections.IList GetCollectionForFeature(string feature)
        {
            if ((feature == "CONNECTSTO"))
            {
                return this._connectsTo;
            }
            return base.GetCollectionForFeature(feature);
        }
        
        /// <summary>
        /// Sets a value to the given feature
        /// </summary>
        /// <param name="feature">The requested feature</param>
        /// <param name="value">The value that should be set to that feature</param>
        protected override void SetFeature(string feature, object value)
        {
            if ((feature == "SENSOR"))
            {
                this.Sensor = ((ISensor)(value));
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
            if ((attribute == "Sensor"))
            {
                return new SensorProxy(this);
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
            if ((reference == "Sensor"))
            {
                return new SensorProxy(this);
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
                _classInstance = ((IClass)(MetaRepository.Instance.Resolve("http://www.semanticweb.org/ontologies/2015/ttc/trainbenchmark#//TrackElement")));
            }
            return _classInstance;
        }
        
        /// <summary>
        /// The collection class to to represent the children of the TrackElement class
        /// </summary>
        public class TrackElementReferencedElementsCollection : ReferenceCollection, ICollectionExpression<IModelElement>, ICollection<IModelElement>
        {
            
            private TrackElement _parent;
            
            /// <summary>
            /// Creates a new instance
            /// </summary>
            public TrackElementReferencedElementsCollection(TrackElement parent)
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
                    if ((this._parent.Sensor != null))
                    {
                        count = (count + 1);
                    }
                    count = (count + this._parent.ConnectsTo.Count);
                    return count;
                }
            }
            
            protected override void AttachCore()
            {
                this._parent.SensorChanged += this.PropagateValueChanges;
                this._parent.ConnectsTo.AsNotifiable().CollectionChanged += this.PropagateCollectionChanges;
            }
            
            protected override void DetachCore()
            {
                this._parent.SensorChanged -= this.PropagateValueChanges;
                this._parent.ConnectsTo.AsNotifiable().CollectionChanged -= this.PropagateCollectionChanges;
            }
            
            /// <summary>
            /// Adds the given element to the collection
            /// </summary>
            /// <param name="item">The item to add</param>
            public override void Add(IModelElement item)
            {
                if ((this._parent.Sensor == null))
                {
                    ISensor sensorCasted = item.As<ISensor>();
                    if ((sensorCasted != null))
                    {
                        this._parent.Sensor = sensorCasted;
                        return;
                    }
                }
                ITrackElement connectsToCasted = item.As<ITrackElement>();
                if ((connectsToCasted != null))
                {
                    this._parent.ConnectsTo.Add(connectsToCasted);
                }
            }
            
            /// <summary>
            /// Clears the collection and resets all references that implement it.
            /// </summary>
            public override void Clear()
            {
                this._parent.Sensor = null;
                this._parent.ConnectsTo.Clear();
            }
            
            /// <summary>
            /// Gets a value indicating whether the given element is contained in the collection
            /// </summary>
            /// <returns>True, if it is contained, otherwise False</returns>
            /// <param name="item">The item that should be looked out for</param>
            public override bool Contains(IModelElement item)
            {
                if ((item == this._parent.Sensor))
                {
                    return true;
                }
                if (this._parent.ConnectsTo.Contains(item))
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
                if ((this._parent.Sensor != null))
                {
                    array[arrayIndex] = this._parent.Sensor;
                    arrayIndex = (arrayIndex + 1);
                }
                IEnumerator<IModelElement> connectsToEnumerator = this._parent.ConnectsTo.GetEnumerator();
                try
                {
                    for (
                    ; connectsToEnumerator.MoveNext(); 
                    )
                    {
                        array[arrayIndex] = connectsToEnumerator.Current;
                        arrayIndex = (arrayIndex + 1);
                    }
                }
                finally
                {
                    connectsToEnumerator.Dispose();
                }
            }
            
            /// <summary>
            /// Removes the given item from the collection
            /// </summary>
            /// <returns>True, if the item was removed, otherwise False</returns>
            /// <param name="item">The item that should be removed</param>
            public override bool Remove(IModelElement item)
            {
                if ((this._parent.Sensor == item))
                {
                    this._parent.Sensor = null;
                    return true;
                }
                ITrackElement trackElementItem = item.As<ITrackElement>();
                if (((trackElementItem != null) 
                            && this._parent.ConnectsTo.Remove(trackElementItem)))
                {
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
                return Enumerable.Empty<IModelElement>().Concat(this._parent.Sensor).Concat(this._parent.ConnectsTo).GetEnumerator();
            }
        }
        
        /// <summary>
        /// Represents a proxy to represent an incremental access to the sensor property
        /// </summary>
        private sealed class SensorProxy : ModelPropertyChange<ITrackElement, ISensor>
        {
            
            /// <summary>
            /// Creates a new observable property access proxy
            /// </summary>
            /// <param name="modelElement">The model instance element for which to create the property access proxy</param>
            public SensorProxy(ITrackElement modelElement) : 
                    base(modelElement, "sensor")
            {
            }
            
            /// <summary>
            /// Gets or sets the value of this expression
            /// </summary>
            public override ISensor Value
            {
                get
                {
                    return this.ModelElement.Sensor;
                }
                set
                {
                    this.ModelElement.Sensor = value;
                }
            }
        }
    }
}

