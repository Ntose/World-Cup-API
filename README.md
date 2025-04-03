# World Cup Statistics Project

## Overview
This project is a .NET-based application for displaying statistics from the FIFA World Cup 2018 (Men) and 2019 (Women). It consists of three components:

1. **Windows Forms Application** - Provides an interactive UI for selecting favorite teams and players, viewing rankings, and managing user preferences.
2. **WPF Application** - A responsive UI with match overviews, lineup displays, and player details.
3. **Data Layer (Class Library)** - Handles data retrieval from the API, data parsing, and storage.

## Features
### Windows Forms Application
- Select **preferred championship** (Men/Women) and **language** (English/Croatian)
- Choose **favorite team** and **favorite players**
- **Drag-and-drop** functionality for player selection
- Set and display **player images**
- View **ranking lists** based on goals and yellow cards
- Generate **printable rankings (PDF format)**
- Manage **application settings**

### WPF Application
- **Responsive UI** with adaptable window size options
- View **team and match details**
- Display **match lineups on a football field representation**
- View **detailed player statistics** with animations
- Synchronization of **preferences with Windows Forms application**

### Data Layer
- Retrieve data from **API** or **local JSON files**
- Handle **asynchronous data retrieval**
- Store and load **user preferences** from text files
- Ensure proper **error handling** to prevent application crashes

## API Endpoints
- [Men's Team Results](https://worldcup-vua.nullbit.hr/men/teams/results)
- [Women's Team Results](https://worldcup-vua.nullbit.hr/women/teams/results)
- [Men's Matches](https://worldcup-vua.nullbit.hr/men/matches)
- [Women's Matches](https://worldcup-vua.nullbit.hr/women/matches)
- Retrieve matches by country using FIFA code, e.g.:
  - `https://worldcup-vua.nullbit.hr/men/matches/country?fifa_code=ENG`

## Installation & Setup
1. **Clone the repository**
   ```sh
   git clone https://github.com/your-repo-url
   cd WorldCupStatsProject
   ```
2. **Open in Visual Studio**
   - Load the solution file (`.sln`)
   - Restore NuGet packages if required
3. **Build the Project**
   - Set `WinFormsApp` or `WPFApp` as the startup project
   - Run the application

## Error Handling
- API failures will trigger an error message instead of crashing the application.
- Missing settings files will prompt users to reconfigure preferences.
- Hardcoded paths are avoided to ensure compatibility across different environments.

## License
This project is licensed under the MIT License.

