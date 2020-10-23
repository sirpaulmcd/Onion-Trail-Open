using System;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
/// <summary>
///   Exception thrown by IPlayerPositionSystem when there are no living players
///   and the user tries to call one of the system's public methods.
/// </summary>
public abstract class ANoLivingPlayersException : Exception
{
    /// <summary>
    ///   A constructor which creates an exception.
    /// </summary>
    public ANoLivingPlayersException(string message) : base(message) {}
}

/// <summary>
///   API which provides information about the positions of all living players.
/// </summary>
/// <invariant>
///   Assumes there is at least one living player. If this invariant is broken,
///   a ANoLivingPlayersException is thrown.
/// </invariant>
public interface IPlayerPositionSystem
{
    //============================================================================
    // Public Methods
    //============================================================================

    /// <summary>
    ///   Find the closest living player to a given coordinate.
    /// </summary>
    /// <param name="location">
    ///   Location from which to search for players.
    /// </param>
    /// <returns>
    ///   The Vector3 position of the nearest living player.
    /// </returns>
    /// <exception cref="ANoLivingPlayersException">
    ///   Thrown when all players are dead.
    /// </exception>
    /// <remarks>
    ///   The distance between two vectors is calculated using a Euclidian (L2) norm.
    /// </remarks>
    Vector3 FindClosest(Vector3 location);

    /// <summary>
    ///   Compute the centroid of all the living player's positions.
    /// </summary>
    /// <exception cref="ANoLivingPlayersException">
    ///   Thrown when all players are dead.
    /// </exception>
    /// <returns>
    ///   The Vector3 centroid of all the living players.
    /// </returns>
    Vector3 GetCentroid();

    /// <summary>
    ///   Determine the positions of all living players.
    /// </summary>
    /// <exception cref="ANoLivingPlayersException">
    ///   Thrown when all players are dead.
    /// </exception>
    /// <returns>
    ///   A list of player positions.
    /// </returns>
    List<Vector3> GetPlayerPositions();

    /// <summary>
    ///   Get an axis-aligned bounding box (AABB) containing all the living players.
    /// </summary>
    /// <exception cref="ANoLivingPlayersException">
    ///   Thrown when all players are dead.
    /// </exception>
    /// <returns>
    ///   A Bounds which contains all the living players' positions.
    /// </returns>
    Bounds GetBoundingBox();
}
}