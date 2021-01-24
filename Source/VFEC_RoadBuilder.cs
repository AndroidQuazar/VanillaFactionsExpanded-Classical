using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using RimWorld.Planet;

namespace VFEC
{
    public class VFEC_RoadBuilder: IExposable
    {
        public List<VFERoman_RoadQueue> queues = new List<VFERoman_RoadQueue>();


        public void ExposeData()
        {
            Scribe_Collections.Look<VFERoman_RoadQueue>(ref queues, "queues", LookMode.Deep);
        }

        public VFERoman_RoadQueue queueExists(Caravan caravan) //Queue for world? Queue selection?
        {
            foreach (VFERoman_RoadQueue queue in queues)
            {
                if (queue.caravan == caravan)
                {
                    return queue;
                }
            }

            return null;
        }

        public void buildNewQueue()
        {
            VFERoman_RoadQueue queue = new VFERoman_RoadQueue();
            queues.Add(queue);
        }

        public void removeQueue(VFERoman_RoadQueue queue)
        {
            queues.Remove(queue);
        }

        public VFERoman_RoadQueue findQueueAtCaravanPosition(Caravan caravan)
        {
            foreach (VFERoman_RoadQueue queue in queues)
            {
                if (queue.CurrentTile == caravan.Tile)
                {
                    return queue;
                }
            }
            Log.Message("No queue found with that position");
            return null;
        }
    }





    //Queue of actual road
    public class VFERoman_RoadQueue: IExposable
    {
        public Caravan caravan;
        public string queueName;
        public string planetName;
        public RoadDef roadDef;
        public List<Node> nodes; //Nodes of general path
        public List<Node> pathNodes;

        public List<WorldPath> WorldPaths = new List<WorldPath>();
        public int currentTile = -1;
        public int nextTile = -1;
        public int currentNode;
        public int ticksLeftToBuild;
        public void ExposeData()
        {
            Scribe_References.Look<Caravan>(ref caravan, "caravan");
            Scribe_Values.Look<string>(ref queueName, "queueName");
            Scribe_Values.Look<string>(ref planetName, "planetName");
            Scribe_Defs.Look<RoadDef>(ref roadDef, "roadDef");
            Scribe_Collections.Look<Node>(ref nodes, "nodes", LookMode.Deep);
            Scribe_Collections.Look<Node>(ref pathNodes, "pathNodes", LookMode.Deep);
            Scribe_Values.Look<int>(ref currentNode, "currentNode");
            Scribe_Values.Look<int>(ref ticksLeftToBuild, "ticksLeftToBuild");
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                findFirstPoint();
            }
        }

        public VFERoman_RoadQueue()
        {
            this.planetName = Find.World.info.name;
            this.roadDef = RoadDefOf.AncientAsphaltRoad;
            this.queueName = "Empty Name";
            this.WorldPaths = new List<WorldPath>();
            this.currentNode = 0;
            this.ticksLeftToBuild = 60000;
        }

        public void tick()
        {
            if (caravan != null)
            {
                //If Caravna is at node and is building   
                //if caravan is at node and is not moving (building)
                if (this.CurrentTile == this.caravan.Tile && this.caravan.pather.Moving == false)
                {

                    this.ticksLeftToBuild -= 1;
                    if (ticksLeftToBuild <= 0)
                    {
                        //Create road segment and move to next node
                        WorldGrid grid = Find.WorldGrid;
                        RoadDef def = grid.GetRoadDef(CurrentTile, NextTile);

                        //Check if potential roads is null and reset if so
                        if (grid.tiles[CurrentTile].potentialRoads == null)
                            grid.tiles[CurrentTile].potentialRoads = new List<Tile.RoadLink>();
                        if (grid.tiles[NextTile].potentialRoads == null)
                            grid.tiles[NextTile].potentialRoads = new List<Tile.RoadLink>();

                        //If previously occupied road, remove
                        if (def != null && isNewRoadBetter(def, roadDef))
                        {
                            grid.tiles[CurrentTile].potentialRoads.RemoveAll((Tile.RoadLink r1) => r1.neighbor == NextTile);
                            grid.tiles[NextTile].potentialRoads.RemoveAll((Tile.RoadLink r1) => r1.neighbor == CurrentTile);
                        }

                        //Add road
                        grid.tiles[CurrentTile].potentialRoads.Add(new Tile.RoadLink { neighbor = NextTile, road = roadDef });
                        grid.tiles[NextTile].potentialRoads.Add(new Tile.RoadLink { neighbor = CurrentTile, road = roadDef });

                        //Find next node
                        this.ticksLeftToBuild = 60000;
                        this.findFirstPoint();
                        this.caravan.pather.StartPath(CurrentTile, new CaravanArrivalAction_BuildRoad());
                    }
                }
            }


        }

        public void cull()
        {
            Find.World.GetComponent<VFEC_RepublicFaction>().roadBuilder.removeQueue(this);
        }

        public void rename(string str)
        {
            queueName = str;
        }

        public void viewInfo()
        {
            //Open window
            //Window shows following:
                //Name
                //Nodes in order
                //Ticks till construction && percentage
        }

        public void assignCaravan()
        {
            List<FloatMenuOption> options = new List<FloatMenuOption>();
            if (Find.WorldObjects.PlayerControlledCaravansCount == 0)
            {
                options.Add(new FloatMenuOption("No Player Caravans", null));
            } else
            {
                foreach (Caravan caravan in Find.WorldObjects.Caravans.Where(x => x.IsPlayerControlled == true))
                {
                    options.Add(new FloatMenuOption(caravan.Name, delegate 
                    {
                        caravan.pather.StartPath(CurrentTile, new CaravanArrivalAction_BuildRoad());
                        this.caravan = caravan;
                    }
                    ));
                }
            }
        }

        public void returnNextPoint()
        {

            //remove this function if not replace.
            findFirstPoint();
        }

        public void findFirstPoint()
        {
            currentNode = 0;
            currentTile = -1;

            //Cycle through all but last node
            for (int selectedNode = 0; selectedNode < pathNodes.Count() - 1; selectedNode++)
            {
                RoadDef def = Find.WorldGrid.GetRoadDef(pathNodes[selectedNode].tile, pathNodes[selectedNode + 1].tile);
                if (def == null || isNewRoadBetter(def, roadDef))
                {
                    Log.Message($"Debug - First tile found in node: {selectedNode}");
                    currentNode = selectedNode;
                    currentTile = pathNodes[selectedNode].tile;
                    nextTile = pathNodes[selectedNode + 1].tile;
                    
                    return;
                }

            }

            //No next point found
            Messages.Message("Debug - No next point found. Road completed", MessageTypeDefOf.NeutralEvent);
            this.cull();

        }

        public void calculateNodePaths()
        {
            pathNodes = new List<Node>();
            for (int i = 0; i < nodes.Count(); i++)
            { //Cycle through each player node
                if (i + 1 != nodes.Count)
                {
                    //if there is a node after this node

                    //Create worldpath
                    Log.Message($"Starting path creation {i}/{nodes.Count()}");
                    WorldPath tempPath = Find.WorldPathFinder.FindPath(nodes[i].tile, nodes[i + 1].tile, null, null);

                    //Check if null path
                    if (tempPath == null)
                        Log.Message("tempPath returned null");

                    pathNodes.Add(new Node(tempPath.FirstNode));

                    int totalNodes = tempPath.NodesLeftCount; //set so does not change from original value

                    for (int k = 0; i < totalNodes; i++)
                    {
                        //For each node in worldpath, create node
                        Log.Message($"Starting inner path creation {k}/{totalNodes}");
                        if (tempPath.NodesLeftCount == 1)
                        {
                            //if Last node
                            pathNodes.Add(new Node(tempPath.LastNode));
                        }
                        else
                        {
                            //If not last node
                            pathNodes.Add(new Node(tempPath.ConsumeNextNode()));
                        }
                    }
                }

                else
                    Log.Message("Last Node step");

                //Else if no node after this node
            }

            if (pathNodes.Count() == 0)
                Log.Message("No nodes generated");
            else
            {
                Log.Message("Nodes successfully generated");
                //reset current node & tile then rebuild
                findFirstPoint();

            }
        }

        public int CurrentTile
        {
            get
            {
                if (currentTile == -1)
                {
                    // get next point
                    findFirstPoint();
                }

                return currentTile;
            }
        }

        public int NextTile
        {
            get
            {
                if (nextTile == -1)
                {
                    // get next point
                    findFirstPoint();
                }

                return nextTile;
            }
        }

        public bool isNewRoadBetter(RoadDef old, RoadDef newroad)
        {
            if (old == newroad)
            {
                return false;
            }
            if (newroad == RoadDefOf.AncientAsphaltHighway)
            {
                return true;
            }
            if (newroad == RoadDefOf.AncientAsphaltRoad && old != RoadDefOf.AncientAsphaltHighway)
            {
                return true;
            }
            if (newroad == RoadDefOf.DirtRoad && old == DefDatabase<RoadDef>.GetNamed("DirtPath"))
            {
                return true;
            }
            return false;
        }
    }




    //Mini class for future ref
    public class Node: IExposable
    {
        public int tile;

        public void ExposeData()
        {
            Scribe_Values.Look<int>(ref tile, "tile");
        }

        public Node(int tile)
        {
            this.tile = tile;
        }
    }

    public class CaravanArrivalAction_BuildRoad : CaravanArrivalAction
    {
        public int tile;
        public override string Label
        {
            get
            {
                return "VERomanBuildRoad".Translate();
            }
        }

        public override string ReportString
        {
            get
            {
                return "VERomanBuildingRoad".Translate();
            }
        }

        public CaravanArrivalAction_BuildRoad()
        {

        }

        public CaravanArrivalAction_BuildRoad(int tile)
        {
            this.tile = tile;
        }

        public override void Arrived(Caravan caravan)
        {
            // Can alert here
            caravan.pather.StopDead();
            Messages.Message("Debug - Caravan has started constructing road", MessageTypeDefOf.NeutralEvent);
        }
    }
}
