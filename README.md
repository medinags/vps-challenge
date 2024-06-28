# GEO AR-Cade

Step into the future with Geo AR-Cade, an Augmented Reality experience powered by Niantic's cutting-edge technology (VPS, Meshing, and Semantic Segmentation).
Get ready to explore and compete against players from your city and around the world playing a clasing a Snake clasic Arcade Game
Master each Wayspot to become the top player in Geo AR-Cade and watch as public spaces transform into vibrant hubs of social interaction.

# Gameplay

"Geo AR-Cade" is an innovative augmented reality game that utilizes VPS from ARDK. 
The mission is to explore different Wayspots while playing Snake Arcade in AR, aiming to achieve the highest score at each location.

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


