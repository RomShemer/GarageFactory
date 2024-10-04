using System;
using GarageLogic.EngineClass;

namespace GarageLogic
{
    namespace FuelEngineClass
    {
        public class FuelEngine : Engine
        {
            public enum eFuelType
            {
                Soler = 1,
                Octan95,
                Octan96,
                Octan98
            }

            private float m_CurrentFuelAmount;
            private readonly float m_MaxFuelAmount;
            private eFuelType m_FuelType;
            private const float k_MinFuelAmout = 0;

            public float CurrentFuelAmount
            {
                get
                {
                    return m_CurrentFuelAmount;
                }
            }

            public eFuelType FuelType
            {
                get
                {
                    return m_FuelType;
                }
            }

            public float MaxFuelAmount
            {
                get
                {
                    return m_MaxFuelAmount;
                }
            }

            public FuelEngine(float i_maxFuel, float i_currentFuelAmount, eFuelType fuelType)
            {
                m_MaxFuelAmount = i_maxFuel;
                m_CurrentFuelAmount = i_currentFuelAmount;
                m_FuelType = fuelType;
            }

            public override void FillEnergy(float i_addEnergy)
            {
                bool isValidFillEnergyAmount = (m_CurrentFuelAmount + i_addEnergy) <= m_MaxFuelAmount;

                if (isValidFillEnergyAmount)
                {
                    m_CurrentFuelAmount += i_addEnergy;
                }
                else
                {
                    float maxAmount = m_MaxFuelAmount - m_CurrentFuelAmount;

                    throw new ValueOutOfRangeException(new Exception(), k_MinFuelAmout, maxAmount);
                }
            }

            public override void GetCurrentEnergyDetails(out float o_currentAmountOfEnergy)
            {
                o_currentAmountOfEnergy = (m_CurrentFuelAmount/m_MaxFuelAmount);
            }

            public override string ToString()
            {
                string engineInfo = $"Engine type: Fuel\n\tfuel type: {Enum.GetName(typeof(eFuelType), m_FuelType)}\n\tcurrent Fuel amount: {m_CurrentFuelAmount} (liters)\n\tmaximum tank capacity: {m_MaxFuelAmount}";
                return engineInfo;
            }
        }
    }
}