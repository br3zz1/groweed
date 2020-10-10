
public class LooseObject
{
    public ItemStack stack { get; protected set; }
    public Tile tile { get; protected set; }

    private LooseObject(Tile tile, ItemStack stack)
    {
        
        this.tile = tile;
        this.stack = stack;
    }

    public static bool PlaceLooseObject(Tile tile, ItemStack stack)
    {
        if (tile.installedObject != null || tile.looseObject != null) return false;
        LooseObject obj = new LooseObject(tile,stack);
        return tile.SetLooseObject(obj);
    }
}
