using System;
using GarageLogic.FuelEngineClass;
using GarageLogic.ElectricEngineClass;

namespace GarageLogic
{
    namespace EngineClass
    {
        public abstract class Engine
        {
            public enum eEngineType
            {
                Fuel = 1,
                Electric
            }

            public static Engine CreateEngineByType(Engine.eEngineType i_engineType, float i_maxEnergyAmount, float i_currentEnergyAmount, FuelEngine.eFuelType i_fuelType = 0)
            {
                Engine engine = null;

                switch (i_engineType)
                {
                    case Engine.eEngineType.Fuel:
                        engine = new FuelEngine(i_maxEnergyAmount, i_currentEnergyAmount, i_fuelType);
                        break;
                    case Engine.eEngineType.Electric:
                        engine = new ElectricEngine(i_maxEnergyAmount, i_currentEnergyAmount);
                        break;
                    default:
                        throw new ArgumentException("Invalid engine type");
                }

                return engine;
            }

            public abstract void GetCurrentEnergyDetails(out float o_currentAmountOfEnergy);

            public abstract void FillEnergy(float i_addEnergy);

            public abstract string ToString();
        }
    }
}