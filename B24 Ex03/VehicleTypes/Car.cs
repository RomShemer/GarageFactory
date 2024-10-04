using System;
using System.Text;
using GarageLogic.EngineClass;
using GarageLogic.FuelEngineClass;
using GarageLogic.VehicleClass;
using TyreClass;
using GarageLogic.ElectricEngineClass;

namespace GarageLogic
{
    namespace CarClass
    {
        public class Car : Vehicle
        {
            public enum eCarColors
            {
                Yellow = 1,
                white,
                Red,
                Black
            }

            private int m_NumberOfDoors;
            private eCarColors m_CarColor;
            private const float k_FuelCarMaxAirPressure = 33;
            private const float k_ElectricCarMaxAirPressure = 31;
            private const float k_MaxFuelCapacity = 45;
            private const float k_MaxBatteryCapacity = (float)3.5;
            public static readonly int k_NumberOfTyres = 5;
            public const FuelEngine.eFuelType k_FuelType = FuelEngine.eFuelType.Octan95;

            public eCarColors Color
            {
                get
                {
                    return m_CarColor;
                }
            }

            public int NumberOfDoors
            {
                get
                {
                    return m_NumberOfDoors;
                }
            }

            public int NumberOfTyres
            {
                get
                {
                    return k_NumberOfTyres;
                }
            }

            public override float GetMaxAirPressureInTyres()
            {
                return m_Tyers[0].MaxAirPressure;
            }

            public override float GetRelativeAirPressureInTyres()
            {
                return (m_Tyers[0].CurrentAirPressure / m_Tyers[0].MaxAirPressure) * 100;
            }

            public Car(string i_ownerName, string i_ownerPhone, string i_licenseNumber, string i_modelName, eCarColors i_carColor, Engine.eEngineType i_engineType,
                float i_currentEnergy, Tyre[] i_tyre, int i_numberOfDoors)
                : base(i_ownerName, i_ownerPhone, i_modelName, i_licenseNumber, k_NumberOfTyres, i_tyre, i_engineType == EngineClass.Engine.eEngineType.Fuel ?
                EngineClass.Engine.CreateEngineByType(i_engineType, k_MaxFuelCapacity, i_currentEnergy, k_FuelType) : EngineClass.Engine.CreateEngineByType(i_engineType, k_MaxBatteryCapacity, i_currentEnergy), i_currentEnergy)
            {
                m_CarColor = i_carColor;
                m_NumberOfDoors = i_numberOfDoors;

                for (int i = 0; i < k_NumberOfTyres; i++)
                {
                    m_Tyers[i].MaxAirPressure = i_engineType.Equals(EngineClass.Engine.eEngineType.Fuel) ? k_FuelCarMaxAirPressure : k_ElectricCarMaxAirPressure;
                }
            }

            public static void ValidateParameters(params object[] i_vehicleParameters)
            {
                int numberOfDoors;

                if (i_vehicleParameters.Length != 9)
                {
                    throw new ArgumentException("Invalid number of parameters for creating a car.");
                }

                bool isValidCarColor = Enum.TryParse(i_vehicleParameters[4].ToString(), out eCarColors color);
                bool isValidEngineType = Enum.TryParse(i_vehicleParameters[5].ToString(), out Engine.eEngineType engineType);
                bool isValidCurrentEnergyAmountParse = float.TryParse(i_vehicleParameters[6].ToString(), out float energyAmount);
                bool isValidNumberOfCarDoorsParse = int.TryParse(i_vehicleParameters[8].ToString(), out numberOfDoors);
                float maxAirPressure = (engineType == EngineClass.Engine.eEngineType.Fuel) ? k_FuelCarMaxAirPressure : k_ElectricCarMaxAirPressure;
                checkValidTyresParameters(i_vehicleParameters[7], maxAirPressure);

                if (!isValidCarColor)
                {
                    throw new FormatException("Invalid car color");
                }

                if (!isValidEngineType)
                {
                    throw new FormatException("Invalid engine type");
                }

                if (!isValidCurrentEnergyAmountParse)
                {
                    throw new FormatException("Invalid: current energy input should be a positive real number");
                }

                float maxEnergy = engineType.Equals(EngineClass.Engine.eEngineType.Fuel) ? k_MaxFuelCapacity : k_MaxBatteryCapacity;
                bool isCurrentEnergyIsLessThenMaxEnergy = isValidCurrentEnergyAmountParse && energyAmount <= maxEnergy;

                if (!isCurrentEnergyIsLessThenMaxEnergy)
                {
                    throw new ArgumentException($"Invalid: current energy is bigger then the maximum capacity of vehicle, should be a non-negative real number up to {maxEnergy}");
                }

                bool isCurrentEnergyIsNonNegative = energyAmount >= 0;

                if (!isCurrentEnergyIsNonNegative)
                {
                    throw new ArgumentException($"Invalid: current energy should be a non-negative real number up to {maxEnergy}");
                }


                if (!isValidNumberOfCarDoorsParse)
                {
                    throw new FormatException("Invalid: number of doors must be integer");
                }
                else
                {
                    bool isValidNumberOfCarDoors = numberOfDoors >= 2 && numberOfDoors <= 5;

                    if (!isValidNumberOfCarDoors)
                    {
                        throw new ArgumentException("Invalid: number of doors must be an integer between 2 and 5 inclusive.");
                    }
                }
            }

            public override void AddEnergy(float i_addEnergy)
            {
                bool isElectricEngine = m_Engine is ElectricEngine;

                if (isElectricEngine)
                {
                    base.AddEnergy(i_addEnergy, "Electric");
                }
                else
                {
                    throw new ArgumentException($"Fuel vehicle can not be charged");
                }
            }

            public override string ToString()
            {
                StringBuilder carInfo = new StringBuilder();
                string tyreInfo;
                string engineInfo;
                carInfo.AppendLine("CarInfo:\nTyres:");

                for (int i = 0; i < k_NumberOfTyres; i++)
                {
                    tyreInfo = m_Tyers[i].ToString();
                    carInfo.AppendLine($"\ttyre number {i + 1}: {tyreInfo}");
                }

                carInfo.AppendLine($"owner name: {m_OwnerName}");
                carInfo.AppendLine($"owner phone: {m_OwnerPhone}");
                carInfo.AppendLine($"license number: {m_LicenseNumber}");
                carInfo.AppendLine($"color: {Enum.GetName(typeof(eCarColors), m_CarColor)}");
                engineInfo = m_Engine.ToString();
                carInfo.AppendLine(engineInfo);
                carInfo.AppendLine($"number of doors: {m_NumberOfDoors}"); // door

                return carInfo.ToString();
            }

            public override void InflateAllTyresToMax()
            {
                for (int i = 0; i < k_NumberOfTyres; i++)
                {
                    m_Tyers[i].CurrentAirPressure = m_Tyers[i].MaxAirPressure;
                }
            }
            private static void checkValidTyresParameters(object i_parameterArray, float i_maxAirPressure)
            {
                Array inputArray = i_parameterArray as Array;

                foreach (object input in inputArray)
                {
                    bool isValid = input.GetType() == typeof(Tyre);
                    if (isValid)
                    {
                        Tyre tyre = (Tyre)input;
                        tyre.isValidParameters(i_maxAirPressure, tyre);
                    }
                }
            }
        }
    }
}

