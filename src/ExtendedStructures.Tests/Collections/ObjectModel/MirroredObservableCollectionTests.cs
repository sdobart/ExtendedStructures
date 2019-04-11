using ExtendedStructures.Collections.ObjectModel;
using ExtendedStructures.Tests.TestData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace ExtendedStructures.Tests.Collections.ObjectModel
{
    public class MirroredObservableCollectionTests
    {
        #region Properties

        private ObservableCollection<Car> ParentCollection { get; set; }
        private MirroredObservableCollection<Car, CarView> ChildCollection { get; set; }

        #endregion

        #region Test Objects

        private static Car Corvette = new Car(Guid.NewGuid(), 14248, 2011, "Chevrolet", "Corvette", Color.Red, 16.0, 4.2, 248);
        private static Car Malibu   = new Car(Guid.NewGuid(), 14248, 2011, "Chevrolet", "Malibu", Color.Blue, 18.0, 8.0, 176);
        private static Car Jetta    = new Car(Guid.NewGuid(), 148248, 2003, "Volkswagen", "Jetta", Color.Black, 12.0, 6.0, 210);
        private static Car Civic    = new Car(Guid.NewGuid(), 65036, 2015, "Honda", "Civic", Color.Black, 10.0, 8.0, 78);
        private static Car Fit      = new Car(Guid.NewGuid(), 65036, 2015, "Honda", "Fit", Color.Silver, 10.0, 8.0, 82);

        #endregion

        #region Init

        public MirroredObservableCollectionTests()
        {
        }

        private void InitializeEmptyCollections()
        {
            ParentCollection = new ObservableCollection<Car>();
            ChildCollection = new MirroredObservableCollection<Car, CarView>(ParentCollection, CarView.MapFromCar);
        }

        private void InitializeParentCollectionWithData(IEnumerable<Car> parentData)
        {
            ParentCollection = new ObservableCollection<Car>(parentData);
            ChildCollection = new MirroredObservableCollection<Car, CarView>(ParentCollection, CarView.MapFromCar);
        }

        private void InitializeParentCollectionWithFilteredChildCollection(IEnumerable<Car> parentData, Expression<Func<Car, bool>> filter)
        {
            ParentCollection = new ObservableCollection<Car>(parentData);
            ChildCollection = new MirroredObservableCollection<Car, CarView>(ParentCollection, CarView.MapFromCar, filter);
        }

        #endregion

        #region Tests

        [Fact]
        public void AddItemToParentCollection_EmptyCollection_ItemAdded()
        {
            // Arrange.
            InitializeEmptyCollections();

            // Act.
            ParentCollection.Add(Corvette);

            // Assert.
            Assert.Contains(ChildCollection,  x => x.Vin == Corvette.Vin);
            Assert.Single(ChildCollection);
        }

        [Fact]
        public void AddItemsToParentCollection_EmptyCollection_ItemsAdded()
        {
            // Arrange.
            InitializeEmptyCollections();

            // Act.
            ParentCollection.Add(Corvette);
            ParentCollection.Add(Jetta);

            // Assert.
            Assert.Contains(ChildCollection,  x => x.Vin == Corvette.Vin);
            Assert.Contains(ChildCollection,  x => x.Vin == Jetta.Vin);
            Assert.Equal(2, ChildCollection.Count);
        }

        [Fact]
        public void AddItemToParentCollection_ExistingCollection_ItemAdded()
        {
            // Arrange.
            var cars = new List<Car>()
            {
                Corvette,
                Jetta,
            };

            InitializeParentCollectionWithData(cars);

            // Act.
            ParentCollection.Add(Civic);

            // Assert.
            Assert.Contains(ChildCollection,  x => x.Vin == Civic.Vin);
            Assert.Equal(3, ChildCollection.Count);
        }

        [Fact]
        public void RemoveItemFromParentCollection_EmptyCollection_ItemDoesNotExist()
        {
            // Arrange.
            InitializeEmptyCollections();

            // Act.
            ParentCollection.Remove(Corvette);

            // Assert.
            Assert.DoesNotContain(ChildCollection, x => x.Vin == Corvette.Vin);
            Assert.Empty(ChildCollection);
        }

        [Fact]
        public void RemoveItemFromParentCollection_ItemExists_ItemRemoved()
        {
            // Arrange.
            var cars = new List<Car>()
            {
                Corvette,
                Jetta,
            };

            InitializeParentCollectionWithData(cars);

            // Act.
            ParentCollection.Remove(Jetta);

            // Assert.
            Assert.DoesNotContain(ChildCollection,  x => x.Vin == Jetta.Vin);
            Assert.Single(ChildCollection);
        }

        [Fact]
        public void ModifyItemFromParentCollection_ItemExists_Modified()
        {
            //Arrange.
            var cars = new List<Car>()
            {
                Corvette,
                Jetta,
            };

            InitializeParentCollectionWithData(cars);

            // Act.
            var corvette = ParentCollection.Single(x => x.Vin == Corvette.Vin);
            corvette.MilesSinceLastFillUp = 200;

            // Assert.
            Assert.Equal(ChildCollection.Single(x => x.Vin == Corvette.Vin).MilesPerGallon, corvette.MilesSinceLastFillUp / (corvette.FuelCapacityTotal - corvette.FuelCapacityRemaining));
        }

        [Fact]
        public void InsertAndModifyParentCollection_FilteredCollection_Updated()
        {
            // Arrange.
            var cars = new List<Car>()
            {
                Corvette,
                Civic,
                Jetta,
                Malibu,
            };

            InitializeParentCollectionWithFilteredChildCollection(cars, x => x.FuelCapacityTotal > 10);

            // Act.
            ParentCollection.Add(Fit);
            var corvette = ParentCollection.First(x => x.Model == "Corvette");
            corvette.FuelCapacityTotal = 5;



            // Assert.
            Assert.DoesNotContain(ChildCollection, x => x.Vin == Civic.Vin);
            Assert.Contains(ChildCollection, x => x.Vin == Jetta.Vin);
            Assert.Contains(ChildCollection, x => x.Vin == Malibu.Vin);
            Assert.DoesNotContain(ChildCollection, x => x.Vin == Fit.Vin);
            Assert.DoesNotContain(ChildCollection, x => x.Vin == Corvette.Vin);

        }

        #endregion
    }
}
