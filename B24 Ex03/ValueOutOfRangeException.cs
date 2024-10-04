using System;

namespace GarageLogic
{
    public class ValueOutOfRangeException : Exception
    {
        float m_MaxValue;
        float m_MinValue;

        public ValueOutOfRangeException(Exception i_innerException, float i_minValue, float i_maxValue) :
            base(string.Format("Invalid: value is out of range, must be in range of {0} to {1}", i_minValue, i_maxValue), i_innerException)
        {
            m_MaxValue = i_maxValue;
            m_MinValue = i_minValue;
        }
    }

    public class NotFoundException : Exception
    {
        string m_ObjectTypeString;
        public NotFoundException(Exception i_innerException, string i_objectType) : base(string.Format($"{i_objectType} was not found"))
        {
            m_ObjectTypeString = i_objectType;
        }
    }
}