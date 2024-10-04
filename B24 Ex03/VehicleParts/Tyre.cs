using System;

namespace TyreClass
{
    public struct Tyre
    {
        private string m_ManufacturerName;
        private float m_CurrentAirPressure;
        private float m_MaxAirPressure;
        private const float k_MinAirPressure = 0;

        public string ManufacturerName
        {
            get
            {
                return m_ManufacturerName;
            }
            set
            {
                m_ManufacturerName = value;
            }
        }

        public float CurrentAirPressure
        {
            get
            {
                return m_CurrentAirPressure;
            }
            set
            {
                m_CurrentAirPressure = value;
            }
        }

        public float MaxAirPressure
        {
            get
            {
                return m_MaxAirPressure;
            }
            set
            {
                m_MaxAirPressure = value;
            }
        }

        public Tyre(string i_manufacturerName, float i_currentAirPressure)
        {
            m_CurrentAirPressure = i_currentAirPressure;
            m_ManufacturerName = i_manufacturerName;
            m_MaxAirPressure = 0;
        }

        public override string ToString()
        {
            return $"manufacturer name: {m_ManufacturerName}, current air pressure: {m_CurrentAirPressure}, maximum air pressure{m_MaxAirPressure}";
        }

        public void isValidParameters(float i_maxAirPressure, Tyre i_tyreParameters)
        {
            bool isValidCurrentAirPressure = i_tyreParameters.CurrentAirPressure.GetType() == typeof(float);
            bool isValidManufacturerName = i_tyreParameters.ManufacturerName.GetType() == typeof(string);

            if (!isValidManufacturerName)
            {
                throw new FormatException("Invalid: current manufacturer name should be an string");
            }

            if (!isValidCurrentAirPressure)
            {
                throw new FormatException("Invalid: current air pressure should be an integer");
            }

            isValidCurrentAirPressure = i_tyreParameters.CurrentAirPressure <= i_maxAirPressure && i_tyreParameters.CurrentAirPressure >= 0;
            if (!isValidCurrentAirPressure)
            {
                throw new FormatException($"Invalid: current air pressure should be a non-negative real number up to {i_maxAirPressure}");
            }
        }
    }
}