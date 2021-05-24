using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class PathAStar
{
    private Queue<Cell> path;
    
    public float Duration { get; private set; }

    public PathAStar() { }

    public PathAStar(Queue<Cell> path)
    {
        if (path == null || !path.Any())
        {
            Debug.LogWarning("Created path with no tiles, is this intended?");
        }

        this.path = path;
    }

    public PathAStar(Cell startCell, Cell endCell, Map map = null, Pathfinder.PathfindingHeuristic costEstimate = null)
    {
        if (costEstimate == null) costEstimate = Pathfinder.DefaultDistanceHeuristic(endCell);
        if (map == null) map = startCell.map;

        float startTime = Time.realtimeSinceStartup;

        // Set path to empty Queue so that there always is something to check count on
        path = new Queue<Cell>();

        // if tileEnd is null, then we are simply scanning for the nearest objectType.
        // We can do this by ignoring the heuristic component of AStar, which basically
        // just turns this into an over-engineered Dijkstra's algo

        // Check to see if we have a valid tile graph
        if (map.cellGraph == null)
        {
            map.cellGraph = new PathCellGraph(map);
        }

        // A dictionary of all valid, walkable nodes.
        Dictionary<Cell, PathNode<Cell>> nodes = map.cellGraph.nodes;

        // Make sure our start/end tiles are in the list of nodes!
        if (nodes.ContainsKey(startCell) == false)
        {
            Debug.LogError("The starting tile isn't in the list of nodes!");

            return;
        }

        PathNode<Cell> start = nodes[startCell];

        /*
         * Mostly following this pseusocode:
         * https://en.wikipedia.org/wiki/A*_search_algorithm
         */
        HashSet<PathNode<Cell>> closedSet = new HashSet<PathNode<Cell>>();

        /*
         * List<Path_Node<Tile>> openSet = new List<Path_Node<Tile>>();
         *        openSet.Add( start );
         */

        PathfindingPriorityQueue<PathNode<Cell>> openSet = new PathfindingPriorityQueue<PathNode<Cell>>();
        openSet.Enqueue(start, 0);

        Dictionary<PathNode<Cell>, PathNode<Cell>> came_From = new Dictionary<PathNode<Cell>, PathNode<Cell>>();

        Dictionary<PathNode<Cell>, float> g_score = new Dictionary<PathNode<Cell>, float>();
        g_score[start] = 0;

        Dictionary<PathNode<Cell>, float> f_score = new Dictionary<PathNode<Cell>, float>();
        f_score[start] = costEstimate(start.data);

        while (openSet.Count > 0)
        {
            PathNode<Cell> current = openSet.Dequeue();

            // Check to see if we are there.
            if (current.data == endCell)
            {
                Duration = Time.realtimeSinceStartup - startTime;
                Reconstruct_path(came_From, current);
                return;
            }

            closedSet.Add(current);

            foreach (PathEdge<Cell> edge_neighbor in current.edges)
            {
                PathNode<Cell> neighbor = edge_neighbor.node;

                if (closedSet.Contains(neighbor))
                {
                    continue; // ignore this already completed neighbor
                }

                float pathfinding_cost_to_neighbor = neighbor.data.PathfindingCost * Dist_between(current, neighbor);

                float tentative_g_score = g_score[current] + pathfinding_cost_to_neighbor;

                if (openSet.Contains(neighbor) && tentative_g_score >= g_score[neighbor])
                {
                    continue;
                }

                came_From[neighbor] = current;
                g_score[neighbor] = tentative_g_score;
                f_score[neighbor] = g_score[neighbor] + costEstimate(neighbor.data);

                openSet.EnqueueOrUpdate(neighbor, f_score[neighbor]);
            } // foreach neighbour
        } // while

        // If we reached here, it means that we've burned through the entire
        // openSet without ever reaching a point where current == goal.
        // This happens when there is no path from start to goal
        // (so there's a wall or missing floor or something).

        // We don't have a failure state, maybe? It's just that the
        // path list will be null.
        Duration = Time.realtimeSinceStartup - startTime;
    }

    public Cell Dequeue()
    {
        if (path == null)
        {
            // Debug.LogError("Attempting to dequeue from an null path.");
            return null;
        }

        if (path.Count <= 0)
        {
            // Debug.LogError("Path queue is zero or less elements long.");
            return null;
        }

        return path.Dequeue();
    }

    public int Length()
    {
        if (path == null)
        {
            return 0;
        }

        return path.Count;
    }

    public Cell EndTile()
    {
        if (path == null || path.Count == 0)
        {
            Debug.Log("Path is null or empty.");
            return null;
        }

        return path.Last();
    }

    public IEnumerable<Cell> Reverse()
    {
        return path == null ? null : path.Reverse();
    }

    public List<Cell> GetList()
    {
        return path.ToList();
    }

    public Queue<Cell> GetQueue()
    {
        return path;
    }

    private float Dist_between(PathNode<Cell> a, PathNode<Cell> b)
    {
        // We can make assumptions because we know we're working
        // on a grid at this point.

        // Hori/Vert neighbours have a distance of 1
        if (Mathf.Abs(a.data.TrueX - b.data.TrueX) + Mathf.Abs(a.data.TrueY - b.data.TrueY) == 1 )
        {
            return 1f;
        }

        // Diag neighbours have a distance of 1.41421356237
        if (Mathf.Abs(a.data.TrueX - b.data.TrueX) == 1 && Mathf.Abs(a.data.TrueY - b.data.TrueY) == 1)
        {
            return 1.41421356237f;
        }

        // Otherwise, do the actual math.
        return Mathf.Sqrt(
            Mathf.Pow(a.data.TrueX - b.data.TrueX, 2) +
            Mathf.Pow(a.data.TrueY - b.data.TrueY, 2));
    }

    private void Reconstruct_path(
        Dictionary<PathNode<Cell>, PathNode<Cell>> came_From,
        PathNode<Cell> current)
    {
        // So at this point, current IS the goal.
        // So what we want to do is walk backwards through the Came_From
        // map, until we reach the "end" of that map...which will be
        // our starting node!
        Queue<Cell> total_path = new Queue<Cell>();
        total_path.Enqueue(current.data); // This "final" step is the path is the goal!

        while (came_From.ContainsKey(current))
        {
            /*    Came_From is a map, where the
            *    key => value relation is real saying
            *    some_node => we_got_there_from_this_node
            */

            current = came_From[current];
            total_path.Enqueue(current.data);
        }

        // At this point, total_path is a queue that is running
        // backwards from the END tile to the START tile, so let's reverse it.
        path = new Queue<Cell>(total_path.Reverse());
    }
}
