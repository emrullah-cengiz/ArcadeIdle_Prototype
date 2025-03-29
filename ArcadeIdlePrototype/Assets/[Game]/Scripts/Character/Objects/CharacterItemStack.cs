public class CharacterItemStack : ItemStorage
{
    public override ItemType? ItemType
    {
        get
        {
            if (!Items.TryPeek(out var it))
                return null;

            return it.Type;
        }
    }
}