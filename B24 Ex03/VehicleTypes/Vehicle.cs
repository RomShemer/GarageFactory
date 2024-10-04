using System;
using TyreClass;
using GarageLogic.EngineClass;
using GarageLogic.FuelEngineClass;
using GarageLogic.ElectricEngineClass;

namespace GarageLogic
{
    namespace VehicleClass
    {
        public abstract class Vehicle
        {
            public enum eVehicleStatus
            {
                InRepair = 1,
                Fixed,
                Paid,
                Non
            }

            protected string m_OwnerName;
            protected string m_OwnerPhone;
            protected string m_ModelName;
            protected string m_LicenseNumber;
            protected Tyre[] m_Tyers;
            protected Engine m_Engine;
            private eVehicleStatus m_VehicleStatus;
            private float m_EnergyLeft = 0;

            public eVehicleStatus Status
            {
                get
                {
                    return m_VehicleStatus;
                }
                set
                {
                    m_VehicleStatus = value;
                }
            }

            public string OwnerName
            {
                get
                {
                    return m_OwnerName;
                }

            }

            public string OwnerPhone
            {
                get
                {
                    return m_OwnerPhone;
                }

            }

            public string LicenseNumber
            {
                get
                {
                    return m_LicenseNumber;
                }
                set
                {
                    m_LicenseNumber = value;
                }
            }

            public Engine Engine
            {
                get
                {
                    return m_Engine;
                }
                set
                {
                    m_Engine = value;
                }
            }

            public Engine.eEngineType EngineType
            {
                get
                {
                    return (m_Engine is FuelEngine) ? EngineClass.Engine.eEngineType.Fuel : EngineClass.Engine.eEngineType.Electric;
                }
            }

            public Tyre[] Tyres
            {
                get
                {
                    return m_Tyers;
                }
                set
                {
                    m_Tyers = value;
                }
            }

            public float CurrentEnergy
            {
                get
                {
                    return m_EnergyLeft;
                }
            }

            public abstract float GetRelativeAirPressureInTyres();

            public abstract float GetMaxAirPressureInTyres();

            protected Vehicle(string i_ownerName, string i_ownerPhone, string i_modelName, string i_licenseNumber,
                  int i_numberOfTyers, Tyre[] i_tyre, Engine i_engine, float i_currentEnergy)
            {
                m_OwnerName = i_ownerName;
                m_OwnerPhone = i_ownerPhone;
                m_ModelName = i_modelName;
                m_LicenseNumber = i_licenseNumber;
                m_Tyers = i_tyre;
                m_Engine = i_engine;
                m_EnergyLeft = i_currentEnergy;
            }

            public virtual void AddEnergy(float i_addEnergy, string i_EnergyType)
            {
                FuelEngine fuelEngine = m_Engine as FuelEngine;
                bool isVehicleHasFuelEngine = fuelEngine != null;
                bool isValidForElecticEngine = m_Engine is ElectricEngine && i_EnergyType == "Electric";

                if (isVehicleHasFuelEngine)
                {
                    FuelEngine.eFuelType fuelType;
                    bool isValidFuelType = Enum.TryParse(i_EnergyType, out fuelType) && (fuelType == fuelEngine.FuelType);

                    if (isValidFuelType)
                    {
                        m_Engine.FillEnergy(i_addEnergy);
                        Engine.GetCurrentEnergyDetails(out m_EnergyLeft);
                    }
                    else
                    {
                        throw new ArgumentException($"Incorrect Fuel type, Fuel type should be {fuelEngine.FuelType}");
                    }
                }
                else if (isValidForElecticEngine)
                {
                    m_Engine.FillEnergy(i_addEnergy);
                    Engine.GetCurrentEnergyDetails(out m_EnergyLeft);
                }
                else
                {
                    throw new ArgumentException($"Electric vehicle can not be refuel with {i_EnergyType}");
                }
            }

            public abstract void AddEnergy(float i_addEnergy);

            public abstract void InflateAllTyresToMax();
        }
    }
}