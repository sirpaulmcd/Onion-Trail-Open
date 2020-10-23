using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGS
{
public class NoLivingPlayersException : ANoLivingPlayersException
{
    public NoLivingPlayersException(string message) : base(message) {}
}

public class PlayerPositionSystem : MonoBehaviour, IPlayerPositionSystem
{
    //============================================================================
    // Instance Variables
    //============================================================================

    /// <summary>
    ///   The players which this system should track. This should be all players
    ///   currently in the game, dead or alive. Public since it should be set by
    ///   the player creating functions.
    /// </summary>
    public List<GameObject> players = new List<GameObject>();

    /// <summary>
    ///   Exception message created by this module when a client attempts to use
    ///   the API and there are no living players.
    /// </summary>
    /// <remarks>
    ///   We don't provide a way for users to check if there are alive players, 
    ///   since it should be true for 99% of the time, and this would be a waste
    ///   of CPU cycles. Instead we assume the clients will use these API calls
    ///   in a try-catch block.
    /// </remarks>
    private const string _exceptionMsg = "PlayerPositionSystem requires at least "
        + "one living player; None found!";
    
    //============================================================================
    // Public Functions
    //============================================================================

    // NOTE: All these functions use LINQ methods to implement functional
    //       programming methods such as Map, Filter, and Reduce.
    //       Learn more at: https://blog.submain.com/csharp-functional-programming/

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
    public Vector3 FindClosest(Vector3 location)
    {
        var alivePlayers = players.Where(x => !x.GetComponent<IHealth>().IsDead);
        if (alivePlayers.Count() == 0)
            throw (new NoLivingPlayersException(_exceptionMsg));
        
        var positions = alivePlayers.Select(player => player.transform.position);
        return positions.OrderBy(pos => Vector3.Distance(location, pos)).First();
    }
    
    /// <summary>
    ///   Compute the centroid of all the living player's positions.
    /// </summary>
    /// <exception cref="ANoLivingPlayersException">
    ///   Thrown when all players are dead.
    /// </exception>
    /// <returns>
    ///   The Vector3 centroid of all the living players.
    /// </returns>
    public Vector3 GetCentroid()
    {
        var alivePlayers = players.Where(x => !x.GetComponent<IHealth>().IsDead);
        if (alivePlayers.Count() == 0)
            throw (new NoLivingPlayersException(_exceptionMsg));
        
        var positions = alivePlayers.Select(player => player.transform.position);
        return positions.Aggregate((x, y) => x + y) / positions.Count();
    }

    /// <summary>
    ///   Determine the positions of all living players.
    /// </summary>
    /// <exception cref="ANoLivingPlayersException">
    ///   Thrown when all players are dead.
    /// </exception>
    /// <returns>
    ///   A list of player positions.
    /// </returns>
    public List<Vector3> GetPlayerPositions()
    {
        var alivePlayers = players.Where(x => !x.GetComponent<IHealth>().IsDead);
        if (alivePlayers.Count() == 0)
            throw (new NoLivingPlayersException(_exceptionMsg));

        return alivePlayers.Select(player => player.transform.position).ToList();
    }

    /// <summary>
    ///   Get an axis-aligned bounding box (AABB) containing all the living players.
    /// </summary>
    /// <exception cref="ANoLivingPlayersException">
    ///   Thrown when all players are dead.
    /// </exception>
    /// <returns>
    ///   A Bounds which contains all the living players' positions.
    /// </returns>
    public Bounds GetBoundingBox()
    {
        var alivePlayers = players.Where(x => !x.GetComponent<IHealth>().IsDead);
        if (alivePlayers.Count() == 0)
            throw (new NoLivingPlayersException(_exceptionMsg));

        var positions = alivePlayers.Select(x => x.transform.position);
        Vector3 lowerBound = positions.Aggregate((x, y) => Vector3.Min(x, y));
        Vector3 upperBound = positions.Aggregate((x, y) => Vector3.Max(x, y));
        Vector3 center = positions.Aggregate((x, y) => x + y) / positions.Count();
        Vector3 size = upperBound - lowerBound;
        
        return new Bounds(center, size);
    }
}
}