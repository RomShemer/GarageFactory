using System;
using GarageLogic;
using System.Reflection;
using GarageLogic.VehicleClass;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using GarageLogic.FuelEngineClass;

namespace GarageUI
{
    public static class ConsoleUI
    {
        public static StringBuilder s_GarageMenu;
        private const int k_MinutesInHour = 60;
        public enum eMenuOptions
        {
            None = 0,
            AddVehicle = 1,
            DisplayAllVehiclesByStatus,
            ChangeVehicleStatus,
            InflateTyresToMax,
            RefuelVehicle,
            ChargeElectricVehicle,
            GetVehicleDetails
        }

        static ConsoleUI()
        {
            Array enumValuesArray = Enum.GetNames(typeof(eMenuOptions));
            s_GarageMenu = new StringBuilder();
            string splittedEnumName;
            int counter = 1;

            foreach (var enumName in enumValuesArray)
            {
                bool enterCurrentEnumValue = !enumName.Equals(eMenuOptions.None.ToString());

                if (enterCurrentEnumValue)
                {
                    splittedEnumName = SplitByCamelCase(enumName.ToString());
                    s_GarageMenu.AppendLine($"({counter}) for {splittedEnumName}");
                    counter += 1;
                }
            }
        }

        public static void PrintOpening()
        {
            Console.WriteLine("Welcome to the Garage Management System!");
            Thread.Sleep(800);
        }

        public static eMenuOptions GetMenuActionFromUser()
        {
            eMenuOptions menuAction = eMenuOptions.None;
            bool foundValidMenuOption = false;

            while (!foundValidMenuOption)
            {
                PrintGarageMenu();
                string statusStringInput = Console.ReadLine();

                try
                {
                    foundValidMenuOption = isValidGarageMenuOption(statusStringInput, out menuAction);
                    if (!foundValidMenuOption)
                    {
                        throw new ArgumentException("Invalid menu action, try again");
                    }
                }
                catch (ArgumentException argumentEx)
                {
                    Console.WriteLine(argumentEx.Message);
                }
            }

            return menuAction;
        }

        private static bool isValidGarageMenuOption(string i_stringMenuInput, out eMenuOptions o_validMenuInput)
        {
            o_validMenuInput = eMenuOptions.None;

            return Enum.TryParse(i_stringMenuInput, out o_validMenuInput);
        }

        public static void PrintGarageMenu()
        {
            Console.Clear();
            Console.WriteLine("Please select an action to perform in the garage:");
            Console.WriteLine(s_GarageMenu.ToString());
        }

        public static string GetLicenseNumber()
        {
            Console.Clear();
            string LicenseNumber;
            Console.WriteLine("Enter license Number");
            LicenseNumber = Console.ReadLine();
            return LicenseNumber;
        }

        public static void PrintMessage(string i_message, bool i_clearScreen)
        {
            if (i_clearScreen)
            {
                Console.Clear();

            }

            Console.WriteLine(i_message);
            Thread.Sleep(500);
        }

        private static void printEnumValues(Array i_enumValues, int i_indexOfStartingEnumValue)
        {
            int counter = 1;
            bool printCurrentEnumValue;

            foreach (var value in i_enumValues)
            {
                printCurrentEnumValue = counter >= i_indexOfStartingEnumValue;

                if (printCurrentEnumValue)
                {
                    Console.WriteLine($"({counter}) for {value}");
                }
                counter += 1;
            }
        }

        public static eVehicleTypes GetValidVehicleTypeFromUser()
        {
            eVehicleTypes vehicleType = 0;
            bool foundValidVehicleType = false;

            Console.WriteLine("Choose vehicle type from the options below:");
            while (!foundValidVehicleType)
            {
                printEnumValues(Enum.GetNames(typeof(eVehicleTypes)), 1);
                string vehicleTypeStringInput = Console.ReadLine();

                try
                {
                    foundValidVehicleType = isValidVehicleType(vehicleTypeStringInput, out vehicleType);
                    if (!foundValidVehicleType)
                    {
                        throw new ArgumentException("Invalid vehicle type, try again");
                    }
                }
                catch (ArgumentException argumentEx)
                {
                    Console.WriteLine(argumentEx.Message);
                }
            }

            return vehicleType;
        }

        public static void GetReflectionParametersFromUser(ParameterInfo[] i_vehicleParameters, object[] io_desireParameters)
        {
            int index = 0;
            string input;
            bool isValidParam = false;
            bool isEnumParameter = false;
            bool isBoolParamrter = false;
            bool toSkipParmeter;

            foreach (ParameterInfo parameter in i_vehicleParameters)
            {
                while (!isValidParam)
                {
                    try
                    {
                        toSkipParmeter = io_desireParameters[index] != null;
                        if (toSkipParmeter)
                        {
                            isValidParam = true;
                            continue;
                        }

                        input = getParameter(parameter, out isEnumParameter, out isBoolParamrter);
                        if (!isEnumParameter && !isBoolParamrter)
                        {
                            isValidParam = checkValidParam(input, parameter, out io_desireParameters[index]);
                        }
                        else
                        {
                            isValidParam = true;
                            if (isEnumParameter)
                            {
                                io_desireParameters[index] = input;
                            }
                            else
                            {
                                io_desireParameters[index] = int.Parse(input);
                            }
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                isValidParam = false;
                index += 1;
            }
        }

        public static void PrintVehiclesInfo(List<string> i_filterVehiclesList, Vehicle.eVehicleStatus i_statusFilter)
        {
            Console.Clear();
            Console.WriteLine($"Vehicles filter by {i_statusFilter}:");
            foreach (string vehicleInfo in i_filterVehiclesList)
            {
                Console.WriteLine(vehicleInfo);
            }

            Console.WriteLine("\nPress any key to return to the main menu");
            Console.ReadKey(true);
        }

        private static string getParameter(ParameterInfo i_parameter, out bool o_isEnum, out bool o_isBool)
        {
            string input;
            o_isBool = i_parameter.ParameterType == typeof(bool);
            o_isEnum = i_parameter.ParameterType.IsEnum;

            Console.Write($"Please enter {getParameterName(i_parameter.Name)}:");

            if (o_isEnum)
            {
                Console.WriteLine("from the options below: ");
                printEnumValues(Enum.GetNames(i_parameter.ParameterType), 1);
                input = Console.ReadLine();
                bool isValidEnumChoice = int.TryParse(input, out int integerInput) && i_parameter.ParameterType.IsEnumDefined(integerInput);
                if (!isValidEnumChoice)
                {
                    throw new ArgumentException("Invalid: input is not one of the options");
                }
                else
                {
                    input = Enum.GetName(i_parameter.ParameterType, integerInput);
                }
            }
            else if (o_isBool)
            {
                Console.WriteLine("for the options below: ");
                Console.WriteLine("(1) for No\n(2) for Yes");
                input = Console.ReadLine();
                bool isValidBoolChoice = int.TryParse(input, out int integerInput) && (integerInput - 1 == 0 || integerInput - 1 == 1);
                if (!isValidBoolChoice)
                {
                    throw new ArgumentException("Invalid: input is not one of the options");
                }
                else
                {
                    integerInput = -1;
                    input = integerInput.ToString();
                }
            }
            else
            {
                input = Console.ReadLine();
            }

            return input;
        }

        private static string getParameterName(string i_paramterName)
        {
            string paramterName;
            string nameWithOutStart;
            char[] remove = { 'i', '_' };

            nameWithOutStart = i_paramterName.Remove(0, 2);
            paramterName = SplitByCamelCase(nameWithOutStart);

            return paramterName;
        }

        private static string SplitByCamelCase(string i_input)
        {
            StringBuilder result = new StringBuilder();

            result.Append(i_input[0]);
            for (int i = 1; i < i_input.Length; i++)
            {
                if (char.IsUpper(i_input[i]))
                {
                    result.Append(' ');
                }
                result.Append(i_input[i]);
            }

            return result.ToString();
        }

        private static bool checkValidParam(string i_input, ParameterInfo i_paramter, out object o_parametervalue)
        {
            o_parametervalue = 0;
            bool validInput = true;
            string parameterName = getParameterName(i_paramter.Name.ToString());

            try
            {
                o_parametervalue = Convert.ChangeType(i_input, i_paramter.ParameterType);
            }
            catch
            {
                validInput = false;
                throw new FormatException($"Input is not in a valid format for {parameterName}");
            }

            return validInput;
        }

        public static Vehicle.eVehicleStatus GetValidStatusFilterForVehiclesByStatusFromUser()
        {
            Console.Clear();
            Console.WriteLine("Chose status filter of vehicles in the Garage:");

            return getValidStatusFromUser(1);
        }

        public static Vehicle.eVehicleStatus GetValidStatusToChange()
        {
            Console.Clear();
            Console.WriteLine("Chose status of vehicles in the Garage:");

            return getValidStatusFromUser(1);
        }

        public static void PrintChangeVehicleStatus(Vehicle.eVehicleStatus i_oldStatus, Vehicle.eVehicleStatus i_newStatus)
        {
            Console.WriteLine($"Vehicle status in the garage changed from {i_oldStatus} to {i_newStatus} successfully");
        }

        public static string GetValidFuelTypeFromUser()
        {
            bool validFuelTypeInput = false;
            string fuelTypeStringInput = null;
            Console.Clear();
            Console.WriteLine("Chose Fuel type of vehicle from the options below:");

            while (!validFuelTypeInput)
            {
                printEnumValues(Enum.GetNames(typeof(FuelEngine.eFuelType)), 1);
                fuelTypeStringInput = Console.ReadLine();
                validFuelTypeInput = Enum.TryParse(fuelTypeStringInput, out FuelEngine.eFuelType fuelType);
                if (!validFuelTypeInput)
                {
                    Console.WriteLine("Invalid vehicle status, try again");
                }
                else
                {
                    fuelTypeStringInput = Enum.GetName(typeof(FuelEngine.eFuelType), fuelType);
                }

            }

            return fuelTypeStringInput;
        }

        public static float GetFuelAmountToFill()
        {
            Console.Clear();
            Console.WriteLine($"Please enter the desire Fuel amount to fill (in liters) the vehicle:");

            return getValidEnergyAmountToFill();
        }

        public static float GetBatteryAmountToFillInHours()
        {
            Console.Clear();
            Console.WriteLine($"Please enter the desire time amount to charge (in minutes) the vehicle:");

            return (getValidEnergyAmountToFill() / k_MinutesInHour);
        }

        private static float getValidEnergyAmountToFill()
        {
            bool validAmountToFill = false;
            float amountToFill = 0;

            while (!validAmountToFill)
            {
                string amountToFillStringInput = Console.ReadLine();
                validAmountToFill = float.TryParse(amountToFillStringInput, out amountToFill) && (amountToFill > 0);
                if (!validAmountToFill)
                {
                    Console.WriteLine("Invalid: input should be a positive number, try again");
                }

            }

            return amountToFill;
        }

        public static string GetTyreCreationOption(int i_numberOfTyres)
        {
            Console.WriteLine($"there are {i_numberOfTyres} tyres to create,\n would you like to create {i_numberOfTyres} same tyres or to create each tyre individually");
            string chosenOption = validateOptionTyers();

            return chosenOption;
        }

        private static string validateOptionTyers()
        {
            bool isValidOption = false;
            string chosenOption = null;

            while (!isValidOption)
            {
                Console.WriteLine("(1) for create all the tyres together\n(2) for create each tyre indevidualy");
                chosenOption = Console.ReadLine();
                isValidOption = chosenOption == "1" || chosenOption == "2";
                if (!isValidOption)
                {
                    Console.WriteLine("Invalid choice, try again");
                }
            }

            return chosenOption;
        }

        private static Vehicle.eVehicleStatus getValidStatusFromUser(int i_indexOfStartingEnumValue)
        {
            Vehicle.eVehicleStatus vehicleStatus = Vehicle.eVehicleStatus.Non;
            bool foundValidStatus = false;

            while (!foundValidStatus)
            {
                printEnumValues(Enum.GetNames(typeof(Vehicle.eVehicleStatus)), i_indexOfStartingEnumValue);
                string statusStringInput = Console.ReadLine();

                try
                {
                    foundValidStatus = isValidStatus(statusStringInput, out vehicleStatus);
                    if (!foundValidStatus)
                    {
                        throw new ArgumentException("Invalid vehicle status, try again");
                    }
                }
                catch (ArgumentException argumentEx)
                {
                    ConsoleUI.PrintMessage(argumentEx.Message, true);
                }
            }

            return vehicleStatus;
        }

        private static bool isValidStatus(string i_statusInput, out Vehicle.eVehicleStatus o_validFilterStatus)
        {
            o_validFilterStatus = Vehicle.eVehicleStatus.Non;

            return Enum.TryParse(i_statusInput, out o_validFilterStatus);
        }

        private static bool isValidVehicleType(string i_statusInput, out eVehicleTypes o_validFilterStatus)
        {
            o_validFilterStatus = 0;

            return Enum.TryParse(i_statusInput, out o_validFilterStatus);
        }
    }
}

