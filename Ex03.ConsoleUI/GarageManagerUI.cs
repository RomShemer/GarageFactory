using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GarageLogic.VehicleClass;
using B24_Ex03;
using GarageLogic;
using System.Threading;
using TyreClass;

namespace GarageUI
{
    public class GarageManagerUI
    {

        private Garage m_Garage = new Garage();
        private bool m_IsExit = false;

        public void runGarage()
        {
            ConsoleUI.PrintOpening();

            while (!m_IsExit)
            {
                ConsoleUI.eMenuOptions menuAction = ConsoleUI.GetMenuActionFromUser();
                MenuMethod(menuAction);
            }
        }

        public void MenuMethod(ConsoleUI.eMenuOptions i_options)
        {
            switch (i_options)
            {
                case ConsoleUI.eMenuOptions.AddVehicle:
                    addVehicle();
                    break;
                case ConsoleUI.eMenuOptions.DisplayAllVehiclesByStatus:
                    displayAllVehiclesByStatus();
                    break;
                case ConsoleUI.eMenuOptions.ChangeVehicleStatus:
                    changeVehicleStatus();
                    break;
                case ConsoleUI.eMenuOptions.InflateTyresToMax:
                    inflateTyresToMax();
                    break;
                case ConsoleUI.eMenuOptions.RefuelVehicle:
                    refuelVehicle();
                    break;
                case ConsoleUI.eMenuOptions.ChargeElectricVehicle:
                    chargeElectricVehicle();
                    break;
                case ConsoleUI.eMenuOptions.GetVehicleDetails:
                    displayVehicleDetails();
                    break;
                default:
                    break;
            }
        }

        private void addVehicle()
        {
            string message = null;
            bool isVehicleAdded = false;

            while (!isVehicleAdded)
            {
                string licenseNumber = ConsoleUI.GetLicenseNumber();
                try
                {
                    isVehicleAdded = m_Garage.AddVehicle(licenseNumber, out message);
                }
                catch (GarageLogic.NotFoundException objectNotFound)
                {
                    ConsoleUI.PrintMessage(objectNotFound.Message, false);
                    Thread.Sleep(2000);
                    isVehicleAdded = addNewVehicleFromUser(licenseNumber, out message);
                }

                ConsoleUI.PrintMessage(message, false);
            }

            ConsoleUI.PrintMessage("Press any key to return to the menu", false);
            Console.ReadKey(true);
        }

        private bool addNewVehicleFromUser(string i_licenseNumber, out string message)
        {
            int numberOfTyres;
            int indexOfLicense;
            ParameterInfo[] vehicleParameters;
            bool isVehicleAdded = false;
            message = null;

            ConsoleUI.PrintMessage("Enter new vehicle data:", true);
            eVehicleTypes vehicleType = ConsoleUI.GetValidVehicleTypeFromUser();

            m_Garage.GetCreationDetails(vehicleType, out numberOfTyres, out indexOfLicense, out vehicleParameters);
            try
            {
                Tyre[] tyres = getTyers(numberOfTyres);
                int tyersIndexInVehicleParameters = findTyresIndexInVehicleParameters(vehicleParameters);
                object[] parametersDetails = new object[vehicleParameters.Length];

                parametersDetails[indexOfLicense] = i_licenseNumber;
                parametersDetails[tyersIndexInVehicleParameters] = tyres;
                ConsoleUI.GetReflectionParametersFromUser(vehicleParameters, parametersDetails);
                try
                {
                    isVehicleAdded = m_Garage.AddVehicle(i_licenseNumber, out message, vehicleType, parametersDetails);
                }
                catch (FormatException ex)
                {
                    ConsoleUI.PrintMessage(ex.Message, false);
                    message = null;
                    ConsoleUI.PrintMessage("Press any key to try again", false);
                    Console.ReadKey(true);
                }
                catch (ArgumentException ex)
                {
                    ConsoleUI.PrintMessage(ex.Message, false);
                    ConsoleUI.PrintMessage("Press any key to try again", false);
                    Console.ReadKey(true);
                }
            }
            catch (InvalidCastException ex)
            {
                ConsoleUI.PrintMessage(ex.Message, false);
                message = null;
                ConsoleUI.PrintMessage("Press any key to try again", false);
                Console.ReadKey(true);
            }
            catch (ArgumentException ex)
            {
                ConsoleUI.PrintMessage(ex.Message, false);
                ConsoleUI.PrintMessage("Press any key to try again", false);
                Console.ReadKey(true);
            }
            catch (FormatException ex)
            {
                ConsoleUI.PrintMessage(ex.Message, false);
                ConsoleUI.PrintMessage("Press any key to try again", false);
                Console.ReadKey(true);
            }

            return isVehicleAdded;
        }

        private Tyre[] getTyers(int i_numberOfTyres)
        {
            Tyre[] tyres = new Tyre[i_numberOfTyres];
            string typecrationChoice = ConsoleUI.GetTyreCreationOption(i_numberOfTyres);
            ParameterInfo[] tyreParametrs = m_Garage.GetTyreParameters();
            object[] paramtersValues = new object[tyreParametrs.Length];
            ConstructorInfo tyreCtor = typeof(Tyre).GetConstructors()[0];

            if (typecrationChoice == "1")
            {
                ConsoleUI.GetReflectionParametersFromUser(tyreParametrs, paramtersValues);
                for (int i = 0; i < i_numberOfTyres; i++)
                {
                    tyres[i] = (Tyre)tyreCtor.Invoke(paramtersValues);
                }
            }
            else
            {
                for (int i = 0; i < i_numberOfTyres; i++)
                {
                    ConsoleUI.PrintMessage($"Tyre number {i + 1} data:", false);
                    ConsoleUI.GetReflectionParametersFromUser(tyreParametrs, paramtersValues);
                    tyres[i] = (Tyre)tyreCtor.Invoke(paramtersValues);
                    initObjectArray(paramtersValues);
                }
            }

            return tyres;
        }

        private void initObjectArray(object[] i_objects)
        {
            for (int i = 0; i < i_objects.Length; i++)
            {
                i_objects[i] = null;
            }
        }

        private int findTyresIndexInVehicleParameters(ParameterInfo[] i_vehicleParametersInfo)
        {
            return i_vehicleParametersInfo.ToList().FindIndex(parameter => parameter.ParameterType == typeof(Tyre[]));
        }

        private void displayAllVehiclesByStatus()
        {
            Vehicle.eVehicleStatus statusFilter = ConsoleUI.GetValidStatusFilterForVehiclesByStatusFromUser();
            List<string> filterVehicle = m_Garage.GetVehiclesByStatus(statusFilter);
            ConsoleUI.PrintVehiclesInfo(filterVehicle, statusFilter);
        }

        private void displayVehicleDetails()
        {
            string licenseNumber = ConsoleUI.GetLicenseNumber();

            try
            {
                string chosenVehicleDetails = m_Garage.GetVehicleDetails(licenseNumber);
                ConsoleUI.PrintMessage(chosenVehicleDetails, false);
            }
            catch (GarageLogic.NotFoundException notFoundEx)
            {
                ConsoleUI.PrintMessage(notFoundEx.Message, false);
            }

            ConsoleUI.PrintMessage("Press any key to return to the main menu.", false);
            Console.ReadKey(true);
        }

        private void changeVehicleStatus()
        {
            bool statusChangedSuccessfully = false;

            while (!statusChangedSuccessfully)
            {
                string licenseNumber = ConsoleUI.GetLicenseNumber();
                Vehicle.eVehicleStatus newStatus = ConsoleUI.GetValidStatusToChange();

                try
                {
                    Vehicle.eVehicleStatus oldVehicleStatus = Vehicle.eVehicleStatus.Non;
                    m_Garage.ChangeVehicleStatus(licenseNumber, newStatus, out oldVehicleStatus);
                    ConsoleUI.PrintChangeVehicleStatus(oldVehicleStatus, newStatus);
                    statusChangedSuccessfully = true;
                }
                catch (ArgumentException argumentEx)
                {
                    ConsoleUI.PrintMessage(argumentEx.Message + " ,try again", false); 
                }
                catch (NotFoundException notFoundEx)
                {
                    ConsoleUI.PrintMessage(notFoundEx.Message, false);
                }
            }

            ConsoleUI.PrintMessage("Press any key to return to the main menu.", false);
            Console.ReadKey(true);
        }

        private void inflateTyresToMax()
        {
            string licenseNumber = ConsoleUI.GetLicenseNumber();

            try
            {
                m_Garage.InflateWheelsToMax(licenseNumber, out string o_message);
                ConsoleUI.PrintMessage(o_message, true);
            }
            catch (NotFoundException notFoundEx)
            {
                ConsoleUI.PrintMessage(notFoundEx.Message, false);
            }

            ConsoleUI.PrintMessage("\nPress any key to return to the main menu.", false);
            Console.ReadKey(true);
        }

        private void refuelVehicle()
        {
            string licenseNumber = ConsoleUI.GetLicenseNumber();
            string fuelType = ConsoleUI.GetValidFuelTypeFromUser();
            float fuelAmountToFill = ConsoleUI.GetFuelAmountToFill();
            string message = null;

            try
            {
                m_Garage.RefuelVehicle(licenseNumber, fuelType, fuelAmountToFill, out message);
                ConsoleUI.PrintMessage(message, true);
            }
            catch (ValueOutOfRangeException valueOutOfRangeEx)
            {
                ConsoleUI.PrintMessage(valueOutOfRangeEx.Message, false);
            }
            catch (ArgumentException argumentEx)
            {
                ConsoleUI.PrintMessage(argumentEx.Message, false);
            }
            catch (Exception ex)
            {
                ConsoleUI.PrintMessage(ex.Message, false);
            }

            ConsoleUI.PrintMessage("\nPress any key to return to the main menu.", false);
            Console.ReadKey(true);
        }

        private void chargeElectricVehicle()
        {
            string licenseNumber = ConsoleUI.GetLicenseNumber();
            float fuelAmountToFill = ConsoleUI.GetBatteryAmountToFillInHours();
            string message = null;

            try
            {
                m_Garage.ChargeElectricVehicle(licenseNumber, fuelAmountToFill, out message);
                ConsoleUI.PrintMessage(message, true);
            }
            catch (ValueOutOfRangeException valueOutOfRangeEx)
            {
                ConsoleUI.PrintMessage(valueOutOfRangeEx.Message, false);
            }
            catch (NotFoundException notFoundEx)
            {
                ConsoleUI.PrintMessage(notFoundEx.Message, false);
            }
            catch (Exception ex)
            {
                ConsoleUI.PrintMessage(ex.Message, false);
            }

            ConsoleUI.PrintMessage("\nPress any key to return to the main menu.", false);
            Console.ReadKey(true);
        }
    }
}
