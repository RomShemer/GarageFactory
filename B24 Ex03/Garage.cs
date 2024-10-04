using System;
using System.Collections.Generic;
using System.Linq;
using GarageLogic.VehicleClass;
using GarageLogic;
using static GarageLogic.VehicleClass.Vehicle;

namespace B24_Ex03
{
    public class Garage : GarageFactory
    {
        private Dictionary<string, Vehicle> m_Vehicles = new Dictionary<string, Vehicle>();

        public bool AddVehicle(string i_licenseNumber, out string o_message)
        {
            bool isVehicleAdded = false;
            o_message = null;
            bool changeStatus = false;

            if (isVehicleInTheGarage(i_licenseNumber))
            {
                changeStatus = m_Vehicles[i_licenseNumber].Status == Vehicle.eVehicleStatus.Non;
                if (changeStatus)
                {
                    m_Vehicles[i_licenseNumber].Status = Vehicle.eVehicleStatus.InRepair;
                    isVehicleAdded = true;
                    o_message = "Vehicle is already in the garage, Status updated to 'InRepair'";
                }
                else
                {
                    isVehicleAdded = true;
                    o_message = $"Vehicle is already in the garage. Vehicle current status:{m_Vehicles[i_licenseNumber].Status}";
                }
            }
            else
            {
                throw new NotFoundException(new Exception(), typeof(Vehicle).Name);
            }

            return isVehicleAdded;
        }

        public bool AddVehicle(string i_licenseNumber, out string o_message, eVehicleTypes i_vehicleType, object[] i_vehicleParameter)
        {
            bool isVehicleAdded = false;
            o_message = null;
            bool changeStatus = false;

            if (isVehicleInTheGarage(i_licenseNumber))
            {
                changeStatus = m_Vehicles[i_licenseNumber].Status == Vehicle.eVehicleStatus.Non;
                if (changeStatus)
                {
                    m_Vehicles[i_licenseNumber].Status = Vehicle.eVehicleStatus.InRepair;
                    isVehicleAdded = true;
                    o_message = "Vehicle is already in the garage, Status updated to 'InRepair'";
                }
                else
                {
                    isVehicleAdded = true;
                    o_message = $"Vehicle is already in the garage. Vehicle current status:{m_Vehicles[i_licenseNumber].Status}";
                }
            }
            else
            {
                Vehicle newVehicle = base.CreateVehicle(i_vehicleType, i_vehicleParameter);
                newVehicle.Status = eVehicleStatus.Non;
                m_Vehicles[newVehicle.LicenseNumber] = newVehicle;
                isVehicleAdded = true;
                o_message = "Vehicle was not found in the garage, but the addition process to the system completed successfully!";
            }

            return isVehicleAdded;
        }

        public void ChangeVehicleStatus(string i_licenseNumber, Vehicle.eVehicleStatus i_newStatus, out Vehicle.eVehicleStatus o_oldStatus)
        {
            bool foundVaidStatusToChange = false;
            o_oldStatus = eVehicleStatus.Non;

            if (isVehicleInTheGarage(i_licenseNumber))
            {
                foundVaidStatusToChange = isValidVehicleStatusToChange(i_newStatus);
                if (foundVaidStatusToChange)
                {
                    o_oldStatus = m_Vehicles[i_licenseNumber].Status;
                    m_Vehicles[i_licenseNumber].Status = i_newStatus;
                }
            }
            else
            {
                throw new ArgumentException($"Vehicle status has not changed, current status is {m_Vehicles[i_licenseNumber].Status}");
            }
        }

        public void InflateWheelsToMax(string i_licenseNumber, out string o_message)
        {
            if (isVehicleInTheGarage(i_licenseNumber))
            {
                m_Vehicles[i_licenseNumber].InflateAllTyresToMax();
            }
            else
            {
                throw new NotFoundException(new Exception(), typeof(Vehicle).Name);
            }

            float maxVehicleAirPressure = m_Vehicles[i_licenseNumber].GetMaxAirPressureInTyres();
            float relativeAirPressure = m_Vehicles[i_licenseNumber].GetRelativeAirPressureInTyres();

            o_message = $"Tyres inflation {relativeAirPressure}% out of maximum pressure (={maxVehicleAirPressure}) in vehicle:{i_licenseNumber}";
        }

        public void RefuelVehicle(string i_licenseNumber, string i_fuelType, float i_fuelAmount, out string o_message)
        {
            bool vehicleInTheGarage = isVehicleInTheGarage(i_licenseNumber);

            if (vehicleInTheGarage)
            {
                Vehicle currentVehicle = m_Vehicles[i_licenseNumber];

                currentVehicle.AddEnergy(i_fuelAmount, i_fuelType);
                o_message = $"Vehicle refuel successfully, current Fuel status is {currentVehicle.CurrentEnergy*100}%";
            }
            else
            {
                throw new NotFoundException(new Exception(), typeof(Vehicle).Name); ;
            }
        }

        public void ChargeElectricVehicle(string i_licenseNumber, float i_minutesToCharge, out string o_message)
        {
            bool vehicleInTheGarage = isVehicleInTheGarage(i_licenseNumber);

            if (vehicleInTheGarage)
            {
                Vehicle currentVehicle = m_Vehicles[i_licenseNumber];

                currentVehicle.AddEnergy(i_minutesToCharge);
                o_message = $"Vehicle charged successfully, current battery status is {currentVehicle.CurrentEnergy*100}%";
            }
            else
            {
                throw new NotFoundException(new Exception(), typeof(Vehicle).Name); ;
            }
        }

        public string GetVehicleDetails(string i_licenseNumber)
        {
            Vehicle desireVehicle = null;
            string vehicleDetailsInString = null;

            if (isVehicleInTheGarage(i_licenseNumber))
            {
                desireVehicle = m_Vehicles[i_licenseNumber];
                vehicleDetailsInString = desireVehicle.ToString();
            }
            else
            {
                throw new NotFoundException(new Exception(), typeof(Vehicle).Name);
            }

            return vehicleDetailsInString;
        }

        public List<string> GetVehiclesByStatus(Vehicle.eVehicleStatus i_status)
        {
            bool noStatusFilter = i_status.Equals(Vehicle.eVehicleStatus.Non);
            List<string> filterVehiclesList = new List<string>();

            if (noStatusFilter)
            {
                foreach (KeyValuePair<string, Vehicle> vehicle in m_Vehicles)
                {
                    filterVehiclesList.Add($"License Number: {vehicle.Value.LicenseNumber} -> i_status: {vehicle.Value.Status}");
                }
            }
            else
            {
                filterVehiclesList = m_Vehicles.Values.Where(vehicle => vehicle.Status == i_status).Select(vehicle => $"License Number: {vehicle.LicenseNumber} " +
                                                             $"-> i_status: {vehicle.Status}").ToList();
            }

            return filterVehiclesList;
        }

        private bool isVehicleInTheGarage(string i_licenseNumber)
        {
            return m_Vehicles.ContainsKey(i_licenseNumber);
        }

        private bool isValidVehicleStatusToChange(Vehicle.eVehicleStatus i_newStatus)
        {
            bool isValidVehicleStatus = !i_newStatus.Equals(Vehicle.eVehicleStatus.Non);

            if (!isValidVehicleStatus)
            {
                throw new ArgumentException("Invalid: vehicle i_status cannot be changed to a non-existent i_status. Please try again");
            }

            return isValidVehicleStatus;
        }
    }
}
