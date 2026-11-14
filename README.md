
# Integrated Multi-Modal Transport Journey Planning System

This project is a public transport journey planning system developed using C#.

The program allows users to plan journeys using different modes of public transport, such as buses and trains. It includes a user account system, route calculation, saved journeys, ticket purchasing and ticket management.

---

## How It Works

When the program starts, the user can create an account or log in.

Once logged in, the user can enter their starting location and final destination to calculate the shortest route between the two locations.

The program supports:

- **User Account System** – Allows users to create accounts and log in.
- **Journey Planning** – Calculates routes between a starting location and destination.
- **Shortest Path Calculation** – Uses graph-based algorithms to determine the shortest journey.
- **Multiple Transport Modes** – Supports different forms of public transport, including buses and trains.
- **Route Information** – Displays journey details and the transport required.
- **Saved Routes** – Allows users to save journeys for future use.
- **Route Categories** – Organises saved journeys into different categories.
- **Ticket Purchasing** – Allows users to purchase tickets for selected journeys.
- **Ticket Generation** – Creates tickets that can be accessed through the program.
- **Ticket Management** – Allows users to view, cancel and delete tickets.
- **File Management** – Uses file handling to store and manage journey and ticket information.

---

## Journey Planning System

The program uses a graph-based system to represent transport connections between different locations.

Users enter their starting location and destination, and the program calculates the shortest available route.

The system supports journeys involving different modes of public transport and provides information about the connections required.

The project also explored the possibility of considering external factors, such as traffic signals and transport delays, to improve route calculations.

---

## User Account System

The program allows users to create an account and log in to access the journey planning features.

User information is managed using a database system.

Users can save frequently used routes and organise them into different categories, making it easier to access previous journeys.

---

## Ticket Management System

The program includes a ticket purchasing and management system.

Once a journey has been selected, users can purchase a ticket and generate a ticket file.

Tickets can be opened through the program and managed using the file directory system.

Users can also cancel or delete tickets when they are no longer required.

---

## Limitations and Future Improvements

Although the program successfully implemented its main features, there are several areas that could be improved.

- **Real-Time Transport Data** – Integrate APIs to provide live transport schedules and delays.
- **Processing Speed** – Optimise route calculations to reduce processing time.
- **Location Coverage** – Expand the number of supported locations.
- **Journey Accuracy** – Consider external factors such as traffic conditions and transport delays.
- **User Interface** – Improve navigation and accessibility for different users.

---

## Project Aim

The aim of this project was to develop a transport management system that integrates multiple forms of public transport into a single journey planning application.

The system was designed to improve the commuting experience by allowing users to plan journeys, identify transport connections and manage tickets through a user-friendly interface.

This project helped me improve my C# programming skills and develop a better understanding of graph algorithms, database management, regular expressions, dictionaries and file handling.

It also gave me experience in designing user interfaces, developing SQL databases and implementing multiple features within a single application.
