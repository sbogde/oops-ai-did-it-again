# oops-ai-did-it-again

C# scripts for Unity project focused on developing intelligent agents (IA) using A\* pathfinding.

### Changes

#### New Features

- **Waypoint Scoring and Collectibles**:

  - Each waypoint is now assigned a random score between 0 and 3 when the game starts.
  - The score is represented visually by spawning a corresponding number of collectibles (e.g., eggs or dogs) near the waypoint using the `SpawnCollectibles` method.
  - The fox "eats" these collectibles as it passes through the waypoints, with the score being updated in real-time and displayed on the screen via the `OnGUI()` method.

- **Final Animation Sequence in GameOver**:
  - Introduced a sequence of animations in the `GameOver()` method for the Fox character: "Fox_Attack_Tail," "Fox_Sit_No," and "Fox_Idle" are played in that order upon reaching the final waypoint.

#### Improvements

- **Animator Integration**:
  - Adjusted `PathfindingTester.cs` to properly target the Fox Animator component and execute the correct animation sequence when the game concludes.
  - `Fox_Animator_Controller_2` was updated to include the necessary states for the new animation sequence.
- **Score Display on GUI**:
  - The fox's total score is now displayed on the screen during gameplay using the `OnGUI()` method, enhancing the visual feedback of the gameplay.

#### Bug Fixes

- **Fox Movement and Animation**:
  - Resolved an issue where the Fox would continue walking infinitely instead of transitioning to the final animation upon returning to the endpoint.
  - Ensured that the fox's movement is controlled via the `isMovable` field in `PathfindingTester.cs`, allowing better control over when the fox should stop moving.

#### Miscellaneous

- **Code Cleanup and Debugging**:
  - Added debug logs in various parts of the code to help trace the flow and ensure components like the Animator are correctly targeted.
  - Refactored sections of the code for clarity and maintainability, ensuring that logic is properly encapsulated and easy to follow.
