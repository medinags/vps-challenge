# GEO AR-Cade

Step into the future with Geo AR-Cade, an Augmented Reality experience powered by Niantic's cutting-edge technology (VPS, Meshing, and Semantic Segmentation).
Get ready to explore and compete against players from your city and around the world playing a clasing a Snake clasic Arcade Game.
Master each Wayspot to become the top player in Geo AR-Cade and watch as public spaces transform into vibrant hubs of social interaction.

# Gameplay

Geo AR-Cade is a groundbreaking augmented reality game that utilizes VPS from ARDK and other advanced ARDK features. The mission is to explore different Wayspots while playing the classic Snake arcade game in AR, aiming to achieve the highest score at each location.

1. **Open Geo AR-Cade:** At the start, you'll see a toggle with VPS enabled by default, as VPS is a core feature of our experience. You can disable it for indoor gameplay, which is perfect for testing and debugging. When disabled, all game features remain accessible.

2. **Start the Game:** Upon starting, a button will prompt you to explore AR-Cade.

3. **Explore AR-Cade:** Using Coverage, it displays a list of VPS-enabled Wayspots. For each location, the highest player score achieved there is displayed.

4. If none exist, you're encouraged to be the first.

5. **Select a Location:** Navigate to the chosen Wayspot using Google Maps or iPhone Maps.

6. **Arrive at Location:** VPS automatically activates the game's mechanics upon arrival.

7. **Scan the Environment:** Generate a mesh by scanning the surroundings using ARDK Meshing, which is necessary for gameplay.

8. **Finish Scanning:** Display the top three players from our database for that location.

9. **Tap on the Ground:** Initiate the game by tapping the screen, using semantic segmentation to confirm if the surface is the floor.

10. **AR-Cade Game:** Start the classic Snake AR-Cade game.

11. **Game Objective:** Eat as many apples as possible in 2 minutes using power-ups and avoiding obstacles.

12. **User Interface:** Use lower controls to move the snake left or right. The top UI shows points, time, lives, and a radar at the top center indicating the locations of apples and power-ups.

13. **Apples and Powers:** Avoid rocks spawned after eating apples and utilize various powers available.

14. **End of Game:** After 2 minutes, enter your name to rank on the Wayspot leaderboard.

# Test And Build

The name of the main scene is: **Geo AR-Cade GAME - Open it** find it in *Assets/Scenes*. You can test the application using Playback mode, allowing you to test ARDK and VPS features. But it's crucial to control camera movement during playing, so it's best to build the application, as previously explained if you wish to test indoors, you can disable VPS mode.

For Android, you can download the APK from this link: [APK Download Link](https://drive.google.com/uc?export=download&id=11Qt5aU6tHuDR2ulvXHqf3vmubk0XlToe)

To build for iPhone, follow these requirements:

## Requirements:

- Unity 2022.3.24f1
- ARDK 3.6.
- Xcode 15.0.1

## Tested Devices:

- iPhone 13 Pro
- Pixel 4XL


## Troubleshooting:

Sometimes, the build from Xcode may fail due to a known issue. The solution is:

1. Open XCodeProject
2. Build Setting
3. UnityFramework
4. Linkin - General
5. Other Linker Flasg
6. Add "-ld64"
7. Build


