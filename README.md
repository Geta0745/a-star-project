# Pathfinding Assembly
## (note: not finish project)
This repository contains a pathfinding system implemented in Unity, designed to manage the movement of seekers in a dynamic environment. The system features advanced vehicle behavior, including waypoint navigation and obstacle avoidance.

## Overview

The `Seeker` class is the base class for all seeker objects, providing the core functionality for navigating toward a target using waypoints. The `AdvancedVehicle` class extends the `Seeker` class, adding features such as speed control, rotation, and advanced avoidance behaviors.

### Key Features

- **Waypoint Navigation**: Seekers can move toward specified waypoints, adjusting their path as needed.
- **Obstacle Avoidance**: Seekers can detect and avoid other seekers, ensuring smooth navigation without collisions.
- **Dynamic Speed Control**: The vehicle can accelerate and decelerate based on its movement direction.
- **Ground Detection**: Raycasting is used to ensure the vehicle remains grounded before movement.
- **Visual Debugging**: Gizmos are used to visualize waypoints, stop distances, and avoidance radii in the Unity editor.

## Classes

### Seeker

The `Seeker` class provides the basic functionality for pathfinding and movement. It includes:

- **Movement Direction**: A vector indicating the desired direction of movement.
- **Waypoint Management**: A list of waypoints to follow.
- **Pathfinding Updates**: Requests updates at regular intervals to recalculate the path to the target.

#### Public Methods

- `RequestPath()`: Requests a new path to the target.
- `CalculateDirection()`: Calculates the movement direction based on waypoints and avoidance.
- `AvoidOtherSeekers()`: Computes a vector to avoid nearby seekers.

### AdvancedVehicle

The `AdvancedVehicle` class extends `Seeker` and adds vehicle-specific behavior. It includes:

- **Speed Control**: Max speed, backward speed, acceleration, and deceleration settings.
- **Rotation**: Control over turning speed.
- **Ground Detection**: Checks if the vehicle is grounded before moving.

#### Public Methods

- `CalculateDirection()`: Overrides the base method to include advanced movement and avoidance.
- `FixedUpdate()`: Handles the physics-based movement and rotation of the vehicle.

## Usage

1. **Setup**: Add the `Seeker` component to your game objects. Assign the target and configure waypoints.
2. **Advanced Vehicle**: Use the `AdvancedVehicle` class for more complex movement mechanics, including speed and obstacle avoidance.
3. **Visualization**: Enable Gizmos in the Unity editor to visualize waypoints and detection ranges.

