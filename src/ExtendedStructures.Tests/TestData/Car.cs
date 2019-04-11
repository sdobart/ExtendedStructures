using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace ExtendedStructures.Tests.TestData
{
    public class Car : INotifyPropertyChanged
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

        public double Odometer { get => _odometer; set => SetProperty(ref _odometer, value); }
        private double _odometer;

        public int Year { get => _year; set => SetProperty(ref _year, value); }
        private int _year;
        public string Make { get => _make; set => SetProperty(ref _make, value); }
        private string _make;

        public string Model { get => _model; set => SetProperty(ref _model, value); }
        private string _model;

        public Color Color { get => _color; set => SetProperty(ref _color, value); }
        private Color _color;

        public double FuelCapacityTotal { get => _fuelCapacityTotal; set => SetProperty(ref _fuelCapacityTotal, value); }
        private double _fuelCapacityTotal;

        public double FuelCapacityRemaining { get => _fuelCapacityRemaining; set => SetProperty(ref _fuelCapacityRemaining, value); }
        private double _fuelCapacityRemaining;

        public double MilesSinceLastFillUp { get => _milesSinceLastFillUp; set => SetProperty(ref _milesSinceLastFillUp, value); }
        private double _milesSinceLastFillUp;

        #endregion

        #region Constructors

        public Car()
        { }

        public Car(Guid vin, double odometer, int year, string make, string model, Color color, double fuelCapacityTotal, double fuelCapacityRemaining, double milesSinceLastFillUp)
        {
            Vin = vin;
            Odometer = odometer;
            Year = year;
            Make = make;
            Model = model;
            Color = color;
            FuelCapacityTotal = fuelCapacityTotal;
            FuelCapacityRemaining = fuelCapacityRemaining;
            MilesSinceLastFillUp = milesSinceLastFillUp;
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return $"{Year} {Make} {Model}";
        }

        public override bool Equals(object obj)
        {
            bool result = false;

            if (obj is Car car)
            {
                result = car.Vin == Vin;
            }
            else
            {
                result = base.Equals(obj);
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
    }
}
