# Monopoly

A C# Monopoly game with a separate server and client.  
The server manages the game state and player turns; the client provides the graphical interface for each player.

---

## Project structure

- `MonopolyClient/`  
  C# client application (GUI). Connects to the server, shows the board, pieces, dice, and handles player actions.

- `MonopolyServer/`  
  C# server application. Holds the game state, enforces the rules, validates moves, and broadcasts updates to all clients.

- `Executables/`  
  Prebuilt binaries for quick testing (you can run the game without building from source).  
  Typically contains something like:
  - `MonopolyServer.exe`
  - `MonopolyClient.exe`

- `Pictures/`  
  Images and assets used by the client (board, pieces, dice, etc.).

---

## Requirements

- Windows
- .NET SDK (for building from source)  
  - Recommended: `.NET [version you actually use, e.g. 8.0]`
- Visual Studio [year/edition you use] with:
  - `.NET desktop development` workload installed

If you just want to run the prebuilt executables, you only need the appropriate .NET runtime.

---

## How to run (using the executables)

1. **Start the server**

   - Open `Executables/`.
   - Run `MonopolyServer.exe`.
   - The server will show his IP
   - The server will start listening for client connections

2. **Start one or more clients**

   - On the same machine (or other machines on the same network), run `MonopolyClient.exe` from `Executables/`.
   - When prompted (or in the client settings), enter:
     - Server IP:  
       - Prompted when running the server

3. **Join the game**

   - Each client represents one player.
   - Once enough players are connected, the server can start the game and turns will proceed according to Monopoly rules.

---

## How to build from source

1. **Clone the repository**

   ```bash
   git clone https://github.com/<your-username>/Monopoly.git
   cd Monopoly
