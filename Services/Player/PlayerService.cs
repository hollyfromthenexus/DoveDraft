using Godot;
using Godot.Collections;

namespace DoveDraft;

public partial class PlayerService : Node, IPlayerService, ISaveLoadable
{
    [Export]
    public PackedScene PlayerPrefab { get; set; }

    public Player Current { get; private set; }

    //
    //  Godot Methods
    //

    public override void _EnterTree()
    {
        Services.Register<IPlayerService>(this);
        LocateAndUseExistingPlayer();
    }

    public override void _Ready()
    {
        LocateAndUseExistingPlayer();

        // If we still don't have a player, and a map is loaded...
        // ... force load the player.
        if (Current == null && Services.Map.Current != null)
        {
            Log.For<PlayerService>("Map loaded but no player loaded. Spawning player!");
            Current = SpawnPlayer();
        }
    }

    public override void _ExitTree()
    {
        Services.Unregister<IPlayerService>();
    }

    //
    //  ISaveLoadable Methods
    //

    public BaseSaveData Save()
    {
        return new PlayerSaveData() { PlayerExists = Current != null, Physics = Current?.Save() };
    }

    public void Load(BaseSaveData data)
    {
        PlayerSaveData playerData = data as PlayerSaveData;
        Log.For<PlayerService>("Loading data...");

        if (playerData.PlayerExists == false || playerData.Physics == null)
        {
            ClearPlayer();
            Log.For<PlayerService>("...player doesn't exist in save, cleared.");
            return;
        }

        if (Current == null)
        {
            Current = SpawnPlayer();
        }

        PlayerPhysicsSaveData physics = playerData.Physics;
        Current.Load(physics);
        Log.For<PlayerService>(
            $"...restored player to pos {physics.GlobalPosition} rot {physics.GlobalRotation}."
        );
    }

    //
    //  Private Methods
    //

    private Player SpawnPlayer()
    {
        Player newPlayer = PlayerPrefab.Instantiate<Player>();
        AddChild(newPlayer);

        return newPlayer;
    }

    private void ClearPlayer()
    {
        Current?.QueueFree();
        Current = null;
    }

    private void LocateAndUseExistingPlayer()
    {
        Log.For<PlayerService>("Checking for pre-existing player...");

        // Check for any pre-existing players and grab the first one.
        Array<Node> players = GetTree().GetNodesInGroup(Player.GroupName);
        if (players.Count > 1)
        {
            Log.For<PlayerService>($"Multiple players already exist ({players.Count}).");
        }
        Player preExistingPlayer = GetFirstPlayer(players);

        if (preExistingPlayer == null)
        {
            Log.For<PlayerService>("...none found.");
            return;
        }

        if (preExistingPlayer == Current)
        {
            Log.For<PlayerService>("...found but ignoring.");
            return;
        }

        Log.For<PlayerService>("...found!");

        preExistingPlayer.GetParent().RemoveChild(preExistingPlayer);
        AddChild(preExistingPlayer);
        Current = preExistingPlayer;
    }

    private Player GetFirstPlayer(Array<Node> potentialPlayers)
    {
        foreach (Node node in potentialPlayers)
        {
            if (node is Player player)
            {
                return player;
            }
        }

        return null;
    }
}

public partial class Services
{
    public IPlayerService Player => Get<IPlayerService>();
}
