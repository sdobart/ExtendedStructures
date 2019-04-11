using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ExtendedStructures.Tests.TestData
{
    public class CarView : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            RaisePropertyChanged(propertyName);

            return true;
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Properties

        public Guid Vin { get => _vin; set => SetProperty(ref _vin, value); }
        private Guid _vin;

        public string Summary { get => _summary; set => SetProperty(ref _summary, value); }
        private string _summary;

        public double MilesPerGallon { get => _milesPerGallon; set => SetProperty(ref _milesPerGallon, value); }
        private double _milesPerGallon;

        public double EstimatedMilesRemaining { get => _estimatedMilesRemaning; set => SetProperty(ref _estimatedMilesRemaning, value); }
        private double _estimatedMilesRemaning;

        #endregion

        #region Constructors

        public CarView()
        { }

        public CarView(Car car)
        {
            Vin                     = car.Vin;
            Summary                 = $"{car.Year} {car.Make} {car.Model} - {car.Color} - {car.Odometer}";
            MilesPerGallon          = car.MilesSinceLastFillUp / (car.FuelCapacityTotal - car.FuelCapacityRemaining);
            EstimatedMilesRemaining = car.FuelCapacityRemaining * MilesPerGallon;
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return Summary;
        }

        public override bool Equals(object obj)
        {
            bool result = false;

            if (obj is CarView carView)
            {
                result = carView.Vin == Vin;
            }

            return result;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + Vin.GetHashCode();

            return hash;
        }

        #endregion

        #region Mapping

        public static CarView MapFromCar(Car car)
        {
            return new CarView(car);
        }
        #endregion
    }
}
