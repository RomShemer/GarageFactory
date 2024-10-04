Garage Management System
This project is a Garage Management System implemented in C#, designed to handle various types of vehicles in a garage. The system allows adding vehicles, managing their state, inflating tires, refueling or recharging, and more.

Project Structure
The project is divided into several classes, each responsible for different aspects of the system:

1. GarageFactory
A class that provides mechanisms to create different vehicles (car, motorcycle, truck) using reflection to verify and convert parameters according to the needs of each vehicle type.

2. Garage
Manages the vehicles in the garage, allowing the user to:

Add new vehicles
Change vehicle states (e.g., refueled, repaired, paid)
Inflate tires
Refuel gas-powered vehicles
Recharge electric vehicles
Retrieve and filter vehicle details by state
3. Vehicle
An abstract class representing vehicles in the garage. It contains common fields such as:

EngineType: The type of engine the vehicle uses
Tyres: An array of tyres associated with the vehicle
4. Specific Vehicle Types
Car: Represents a car with additional attributes such as number of doors and color.
Motorcycle: Represents a motorcycle with attributes like engine capacity and license type.
Truck: Represents a truck with attributes like hazardous materials transport and cargo volume.
5. Tyre
Represents a tyre with attributes like:

Manufacturer
Current air pressure
Maximum air pressure
6. Engine
An abstract class representing the engine type, with specific subclasses for:

FuelType: Handles fuel-based vehicles.
ElectricType: Handles electric vehicles.
7. ConsoleUI and GarageManagerUI
ConsoleUI: A static class responsible for all input/output with the user.
GarageManagerUI: Manages the interaction between the user and the garage.
Functionality
The system provides the following functionalities:

Add a Vehicle: The user can input details to add new vehicles to the garage.
Display Vehicles: List all vehicles currently in the garage, with filtering options.
Change Vehicle State: Update the state of a vehicle (e.g., from 'In Repair' to 'Repaired').
Inflate Tyres: Set the air pressure of a vehicle’s tyres to the maximum allowed value.
Refuel: Add fuel to fuel-based vehicles.
Recharge: Add charging hours to electric vehicles.
View Vehicle Details: Show detailed information about a specific vehicle.
Requirements
C# 7.0 or higher
.NET Framework 4.7.2 or higher

Class Diagram
The project utilizes object-oriented principles such as inheritance and polymorphism. The key relationships between the classes are depicted in the included diagram (p).
