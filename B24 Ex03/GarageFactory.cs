using System;
using TyreClass;
using System.Reflection;
using GarageLogic.VehicleClass;
using GarageLogic.CarClass;
using GarageLogic.MotorcycleClass;
using GarageLogic.TruckClass;
using System.Linq;

namespace GarageLogic
{
    public enum eVehicleTypes
    {
        Car = 1,
        Motorcycle,
        Truck
    }

    public abstract class GarageFactory
    {
        protected Vehicle CreateVehicle(eVehicleTypes i_vehicleType, object[] i_vehicleParameter)
        {
            Vehicle newVehicle = null;
            ConstructorInfo constructorInfo = null;

            switch (i_vehicleType)
            {
                case eVehicleTypes.Car:
                    Car.ValidateParameters(i_vehicleParameter);
                    constructorInfo = typeof(Car).GetConstructors()[0];
                    i_vehicleParameter = ConvertParameters(constructorInfo.GetParameters(), i_vehicleParameter);
                    break;
                case eVehicleTypes.Motorcycle:
                    Motorcycle.ValidateParameters(i_vehicleParameter);
                    constructorInfo = typeof(Motorcycle).GetConstructors()[0];
                    i_vehicleParameter = ConvertParameters(constructorInfo.GetParameters(), i_vehicleParameter);
                    break;
                case eVehicleTypes.Truck:
                    Truck.ValidateParameters(i_vehicleParameter);
                    constructorInfo = typeof(Truck).GetConstructors()[0];
                    i_vehicleParameter = ConvertParameters(constructorInfo.GetParameters(), i_vehicleParameter);
                    break;
                default:
                    throw new FormatException("Invalid vehicle type");
            }

            if (constructorInfo == null)
            {
                throw new ArgumentException("No matching constructor found for the provided parameters.", nameof(i_vehicleParameter));
            }
            else
            {
                newVehicle = (Vehicle)constructorInfo.Invoke(i_vehicleParameter);
            }

            return newVehicle;
        }

        public object[] ConvertParameters(ParameterInfo[] i_paramsInfo, object[] i_vParameter)
        {
            object[] convertedParameters = new object[i_vParameter.Length];

            for (int i = 0; i < i_vParameter.Length; i++)
            {
                Type parameterType = i_paramsInfo[i].ParameterType;
                object pararmeterValue = i_vParameter[i];

                if (parameterType.IsEnum)
                {
                    convertedParameters[i] = Enum.Parse(parameterType, pararmeterValue.ToString());
                }
                else if (parameterType.IsArray)
                {
                    Array inputArray = pararmeterValue as Array;
                    convertedParameters[i] = inputArray;
                }
                else if (parameterType == typeof(bool))
                {
                    convertedParameters[i] = Convert.ToBoolean(pararmeterValue);
                }
                else
                {
                    convertedParameters[i] = Convert.ChangeType(pararmeterValue.ToString(), parameterType);
                }
            }

            return convertedParameters;
        }

        public void GetCreationDetails(eVehicleTypes i_vehicleType, out int o_numberOfTyres, out int o_indexOfLicense, out ParameterInfo[] o_vehicleParameters)
        {
            o_indexOfLicense = 0;
            ConstructorInfo constructorInfo = null;

            switch (i_vehicleType)
            {
                case eVehicleTypes.Car:
                    constructorInfo = typeof(Car).GetConstructors()[0];
                    o_vehicleParameters = constructorInfo.GetParameters();
                    o_numberOfTyres = Car.k_NumberOfTyres;
                    break;
                case eVehicleTypes.Motorcycle:
                    constructorInfo = typeof(Motorcycle).GetConstructors()[0];
                    o_vehicleParameters = constructorInfo.GetParameters();
                    o_numberOfTyres = Motorcycle.k_NumberOfTyres;
                    break;
                case eVehicleTypes.Truck:
                    constructorInfo = typeof(Truck).GetConstructors()[0];
                    o_vehicleParameters = constructorInfo.GetParameters();
                    o_numberOfTyres = Truck.k_NumberOfTyres;
                    break;
                default:
                    throw new ArgumentException("Invalid vehicle type");
            }

            o_indexOfLicense = findIndexOfParameterName("i_licenseNumber", o_vehicleParameters);
        }

        public ParameterInfo[] GetTyreParameters()
        {
            ConstructorInfo constructorInfo = typeof(Tyre).GetConstructors()[0];
            ParameterInfo[] tyreParameters = constructorInfo.GetParameters();
            return tyreParameters;
        }

        private int findIndexOfParameterName(string i_parameterName, ParameterInfo[] i_parameters)
        {
            return i_parameters.ToList().FindIndex(parameter => parameter.Name == i_parameterName);
        }

    }
}
