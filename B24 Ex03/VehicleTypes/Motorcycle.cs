using System;
using GarageLogic.VehicleClass;
using GarageLogic.EngineClass;
using GarageLogic.FuelEngineClass;
using TyreClass;
using System.Text;
using GarageLogic.ElectricEngineClass;

namespace GarageLogic
{
    namespace MotorcycleClass
    {
        public class Motorcycle : Vehicle
        {
            public enum eLicenseType
            {
                A = 1,
                A1,
                AA,
                B1
            }

            private eLicenseType m_LicenseType;
            private int m_EngineVolume;
            public static readonly int k_NumberOfTyres = 2;
            public const float k_MaxPressureInTyers = 33;
            public const FuelEngine.eFuelType k_FuelType = FuelEngine.eFuelType.Octan98;
            public const float k_MaxFuelCapacity = (float)5.5;
            public const float k_MaxBatteryCapcity = (float)2.5;

            public eLicenseType LicenseType
            {
                get
                {
                    return m_LicenseType;
                }
            }

            public int EngineVolume
            {
                get
                {
                    return m_EngineVolume;
                }
                set
                {
                    m_EngineVolume = value;
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

            public Motorcycle(string i_ownerName, string i_ownerPhone, string i_modelName, string i_licenseNumber, eLicenseType i_licenseType,
                Engine.eEngineType i_engineType, float i_currentEnergy, int i_engineVolume, Tyre[] i_tyre)
                : base(i_ownerName, i_ownerPhone, i_modelName, i_licenseNumber, k_NumberOfTyres, i_tyre,
                      i_engineType == EngineClass.Engine.eEngineType.Fuel ? EngineClass.Engine.CreateEngineByType(i_engineType, k_MaxFuelCapacity, i_currentEnergy, k_FuelType) :
                      EngineClass.Engine.CreateEngineByType(i_engineType, k_MaxBatteryCapcity, i_currentEnergy), i_currentEnergy)
            {
                m_LicenseType = i_licenseType;
                m_EngineVolume = i_engineVolume;

                for (int i = 0; i < k_NumberOfTyres; i++)
                {
                    m_Tyers[i].MaxAirPressure = k_MaxPressureInTyers;
                }
            }

            public static void ValidateParameters(params object[] i_vehicleParameters)
            {
                if (i_vehicleParameters.Length != 9)
                {
                    throw new ArgumentException("Invalid number of parameters for creating a motorcycle.", nameof(i_vehicleParameters));
                }

                bool isValidLicenseType = Enum.TryParse(i_vehicleParameters[4].ToString(), out eLicenseType licenseType);
                bool isValidEngineType = Enum.TryParse(i_vehicleParameters[5].ToString(), out Engine.eEngineType engineType);
                bool isValidCurrentEnergyAmountParse = float.TryParse(i_vehicleParameters[6].ToString(), out float energyAmount);
                bool isValidEngineVolume = int.TryParse(i_vehicleParameters[7].ToString(), out int engineVolume);

                checkValidTyresParameters(i_vehicleParameters[8]);
                if (!isValidLicenseType)
                {
                    throw new FormatException("Invalid license type");
                }

                if (!isValidEngineType)
                {
                    throw new FormatException("Invalid engine type");
                }

                if (!isValidCurrentEnergyAmountParse)
                {
                    throw new FormatException("Invalid: current energy input should be a positive real number");
                }

                float maxEnergy = engineType.Equals(EngineClass.Engine.eEngineType.Fuel) ? k_MaxFuelCapacity : k_MaxBatteryCapcity;
                bool isCurrentEnergyIsLessThenMaxEnergy = isValidCurrentEnergyAmountParse && energyAmount <= maxEnergy;

                if (!isCurrentEnergyIsLessThenMaxEnergy)
                {
                    throw new ArgumentException($"Invalid: current energy is bigger then the maximum capacity of vehicle, should be a real number up to {maxEnergy}");
                }

                bool isCurrentEnergyIsNonNegative = energyAmount >= 0;

                if (!isCurrentEnergyIsNonNegative)
                {
                    throw new ArgumentException($"Invalid: current energy should be a non-negative real number up to {maxEnergy}");
                }

                if (!isValidEngineVolume)
                {
                    throw new FormatException("Invalid motorcycle engine volume. Must be integer");
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
                StringBuilder motorCycleInfo = new StringBuilder();
                string tyreInfo;
                string engineInfo;
                motorCycleInfo.AppendLine("Motorcycle info:\nTyres:");

                for (int i = 0; i < k_NumberOfTyres; i++)
                {
                    tyreInfo = m_Tyers[i].ToString();
                    motorCycleInfo.AppendLine($"\ttyre number {i + 1}: {tyreInfo}");
                }

                motorCycleInfo.AppendLine($"owner name: {m_OwnerName}");
                motorCycleInfo.AppendLine($"owner phone: {m_OwnerPhone}");
                motorCycleInfo.AppendLine($"license number: {m_LicenseNumber}");
                motorCycleInfo.AppendLine($"license type: {Enum.GetName(typeof(eLicenseType), m_LicenseType)}");
                engineInfo = m_Engine.ToString();
                motorCycleInfo.AppendLine(engineInfo);
                motorCycleInfo.AppendLine($"engine volume: {m_EngineVolume}");

                return motorCycleInfo.ToString();
            }

            public override void InflateAllTyresToMax()
            {
                for (int i = 0; i < k_NumberOfTyres; i++)
                {
                    m_Tyers[i].CurrentAirPressure = m_Tyers[i].MaxAirPressure;
                }
            }

            private static void checkValidTyresParameters(object i_parameterArray)
            {
                Array inputArray = i_parameterArray as Array;

                foreach (object input in inputArray)
                {
                    bool isValid = input.GetType() == typeof(Tyre);

                    if (isValid)
                    {
                        Tyre tyre = (Tyre)input;
                        tyre.isValidParameters(k_MaxPressureInTyers, tyre);
                    }
                }
            }
        }
    }
}