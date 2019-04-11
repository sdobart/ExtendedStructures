using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace ExtendedStructures.Collections.ObjectModel
{
    public class MirroredObservableCollection<TParent, TChild> : ObservableCollection<TChild> where TParent : INotifyPropertyChanged
    {
        #region Properties

        private Func<TParent, TChild> _objectMapper;

        private Expression<Func<TParent, bool>> _filter;

        #endregion

        #region Constructors

        public MirroredObservableCollection(ObservableCollection<TParent> parentCollection, Func<TParent, TChild> mapper, Expression<Func<TParent,bool>> filter = null)
        {
            // Assign parent-to-child mapper.
            _objectMapper = mapper;

            // Assign filter.
            _filter = filter;

            // Call OnCollectionChanged for each existing item in parent collection, effectively building the starting child collection.
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, parentCollection);
            OnParentCollectionChanged(this, eventArgs);

            // Subscribe to collection changed event handler.
            parentCollection.CollectionChanged += OnParentCollectionChanged;

            // Subscribe to the PropertyChanged event for each item in the parent collection.
            foreach (INotifyPropertyChanged item in parentCollection)
            {
                item.PropertyChanged += OnParentItemChanged;
            }
        }

        #endregion

        #region Event Handlers

        private void OnParentItemChanged(object sender, PropertyChangedEventArgs e)
        {
            var parentItem = (TParent)sender;
            var childItem = _objectMapper(parentItem);

            // If there is no filter, or if filter includes changed item - replace.
            if (_filter == null || _filter.Compile().Invoke(parentItem))
            {
                // Update child item.
                Remove(childItem);
                Add(childItem);
            }
            else
            {
                // Item does not match filter criteria. Remove from child collection if exists.
                Remove(childItem);
            }
        }

        private void OnParentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (INotifyPropertyChanged item in e.NewItems)
                    {
                        // Add NotifiyPropertyChanged event handler.
                        item.PropertyChanged += OnParentItemChanged;

                        if(_filter == null || _filter.Compile().Invoke((TParent)item))
                        {
                            // Map TParent to TChild.
                            var child = _objectMapper((TParent)item);

                            // Add item to child collection.
                            Add(child);
                        }
                    }
                }
            }
            if (e.OldItems != null)
            {
                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (INotifyPropertyChanged item in e.OldItems)
                    {
                        // Remove NotifyPropertyChanged event handler.
                        item.PropertyChanged -= OnParentItemChanged;

                        // Map TParent to TChild.
                        var child = _objectMapper((TParent)item);

                        // Remove item from child collection.
                        var result = Remove(child);
                    }
                }
            }
        }

        #endregion
    }
}
