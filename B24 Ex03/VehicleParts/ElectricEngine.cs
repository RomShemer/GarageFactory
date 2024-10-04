using System;
using GarageLogic.EngineClass;

namespace GarageLogic
{
    namespace ElectricEngineClass
    {
        public class ElectricEngine : Engine
        {
            private float m_BatteryTimeLeft;
            private readonly float m_MaxBatteryTime;
            private const float k_MinBatteryTime = 0;

            public float BatteryTimeLeftLeft
            {
                get
                {
                    return m_BatteryTimeLeft;
                }
            }

            public float MaxBatteryTime
            {
                get
                {
                    return m_MaxBatteryTime;
                }
            }

            public ElectricEngine(float i_maxBatteryEnergy, float i_currentBatteryEnergy)
            {
                m_MaxBatteryTime = i_maxBatteryEnergy;
                m_BatteryTimeLeft = i_currentBatteryEnergy;
            }

            public override void FillEnergy(float i_addEnergy)
            {
                bool isValidFillEnergyAmount = m_BatteryTimeLeft + i_addEnergy <= m_MaxBatteryTime;

                if (isValidFillEnergyAmount)
                {
                    m_BatteryTimeLeft += i_addEnergy;
                }
                else
                {
                    float maxAmount = m_MaxBatteryTime - m_BatteryTimeLeft;

                    throw new ValueOutOfRangeException(new Exception(), k_MinBatteryTime, maxAmount);
                }
            }

            public override void GetCurrentEnergyDetails(out float o_currentAmountOfEnergy)
            {
                o_currentAmountOfEnergy = (m_BatteryTimeLeft/m_MaxBatteryTime);
            }

            public override string ToString()
            {
                string engineInfo = $"Engine Type: Electric:"+Environment.NewLine+"\tbattery time left in hours:" +
                    " {m_BatteryTimeLeft}"+ Environment.NewLine +"\tmaximum battery capacity: {m_MaxBatteryTime}";
                return engineInfo;
            }
        }
    }
}