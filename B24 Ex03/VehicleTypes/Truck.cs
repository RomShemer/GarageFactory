using System;
using GarageLogic.VehicleClass;
using GarageLogic.FuelEngineClass;
using TyreClass;
using static GarageLogic.MotorcycleClass.Motorcycle;
using System.Text;

namespace GarageLogic
{
    namespace TruckClass
    {
        public class Truck : Vehicle
        {
            private bool m_IsTransportDangerousmaterials;
            private float m_CargoVolume;
            public static readonly int k_NumberOfTyres = 12;
            public const int k_MaxAirPressureInTyre = 28;
            public const FuelEngine.eFuelType k_FuelType = FuelEngine.eFuelType.Soler;
            public const float k_MaxFuelCapacity = 120;

            public int NumberOfTyres
            {
                get
                {
                    return k_NumberOfTyres;
                }
            }

            public bool IsTransportDangerousMaterials
            {
                get
                {
                    return m_IsTransportDangerousmaterials;
                }
                set
                {
                    m_IsTransportDangerousmaterials = value;
                }
            }

            public float CargoVolume
            {
                get
                {
                    return m_CargoVolume;
                }
                set
                {
                    m_CargoVolume = value;
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

            public Truck(string i_ownerName, string i_ownerPhone, string i_modelName, string i_licenseNumber, Tyre[] i_tyre, float i_currentEnergy,
                         bool i_IsTransportDangerousMaterials, float i_cargoVolume)
                : base(i_ownerName, i_ownerPhone, i_modelName, i_licenseNumber, k_NumberOfTyres, i_tyre, new FuelEngine(k_MaxFuelCapacity, i_currentEnergy, k_FuelType), i_currentEnergy)
            {
                m_IsTransportDangerousmaterials = i_IsTransportDangerousMaterials;
                m_CargoVolume = i_cargoVolume;

                for (int i = 0; i < k_NumberOfTyres; i++)
                {
                    m_Tyers[i].MaxAirPressure = k_MaxPressureInTyers;
                }
            }

            public override void AddEnergy(float i_addEnergy)
            {
                throw new ArgumentException("Fuel vehicle can not be charge");
            }

            public static void ValidateParameters(params object[] i_vehicleParameters)
            {
                if (i_vehicleParameters.Length != 8)
                {
                    throw new ArgumentException("Invalid number of parameters for creating a truck.", nameof(i_vehicleParameters));
                }

                bool isValidTransportDangeroisMaterials = Convert.ToBoolean(i_vehicleParameters[6]) != null;
                bool isValidCargoVolume = float.TryParse(i_vehicleParameters[7].ToString(), out float cargoVolume);
                bool isValidCurrentEnergyAmountParse = float.TryParse(i_vehicleParameters[5].ToString(), out float energyAmount);
                checkValidTyresParameters(i_vehicleParameters[4], k_MaxPressureInTyers);

                if (!isValidTransportDangeroisMaterials)
                {
                    throw new FormatException("Invalid: truck 'is transport dangerous materials' parameter type");
                }

                if (!isValidCargoVolume)
                {
                    throw new FormatException("Invalid: cargo volume should be a non-negative number");
                }

                if (!isValidCurrentEnergyAmountParse)
                {
                    throw new FormatException($"Invalid: current energy input should be a non-negative real number");
                }

                bool isCurrentEnergyIsNonNegative = energyAmount >= 0;

                if (!isCurrentEnergyIsNonNegative)
                {
                    throw new ArgumentException($"Invalid: current energy should be a non-negative real number up to {k_MaxFuelCapacity}");
                }

                bool isCurrentEnergyIsLessThenMaxEnergy = isValidCurrentEnergyAmountParse && (energyAmount <= k_MaxFuelCapacity);

                if (!isCurrentEnergyIsLessThenMaxEnergy)
                {
                    throw new ArgumentException($"Invalid: current energy is bigger then the maximum capacity of vehicle, should be a real number up to {k_MaxFuelCapacity}");
                }
            }

            public override string ToString()
            {
                StringBuilder truckInfo = new StringBuilder();
                string tyreInfo;
                string engineInfo;
                truckInfo.AppendLine("TruckInfo:\nTyres:");

                for (int i = 0; i < k_NumberOfTyres; i++)
                {
                    tyreInfo = m_Tyers[i].ToString();
                    truckInfo.AppendLine($"\ttyre number {i + 1}: {tyreInfo}");
                }

                truckInfo.AppendLine($"owner name: {m_OwnerName}");
                truckInfo.AppendLine($"owner phone: {m_OwnerPhone}");
                truckInfo.AppendLine($"license number: {m_LicenseNumber}");
                truckInfo.AppendLine($"is transport dangerous materials: {m_IsTransportDangerousmaterials.ToString()}");
                engineInfo = m_Engine.ToString();
                truckInfo.AppendLine(engineInfo);
                truckInfo.AppendLine($"cargo volume: {m_CargoVolume}");

                return truckInfo.ToString();
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