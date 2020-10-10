
public class LooseObject
{
    public ItemStack stack { get; protected set; }
    public Tile tile { get; protected set; }

    public LooseObject(Tile tile, ItemStack stack)
    {
        this.tile = tile;
        this.stack = stack;
    }
}
