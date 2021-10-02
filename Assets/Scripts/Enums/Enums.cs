namespace Enums
{
    public enum AnimationName{
       WalkUp,
       WalkDown,
       WalkRight,
       WalkLeft,
       RunUp,
       RunDown,
       RunRight,
       RunLeft,
       LiftUp,
       LiftDown,
       LiftLeft,
       LiftRight,
       AxeRight,
       AxeLeft,
       AxeUp,
       AxeDown,
       FishingRight,
       FishingLeft,
       FishingUp,
       FishingDown,
       MiscRight,
       MiscLeft,
       MiscUp,
       MiscDown,
       PickRight,
       PickLeft,
       PickUp,
       PickDown,
       SickleRight,
       SickleLeft,
       SickleUp,
       SickleDown,
       HammerRight,
       HammerLeft,
       HammerUp,
       HammerDown,
       ShovelRight,
       ShovelLeft,
       ShovelUp,
       ShovelDown,
       HoeRight,
       HoeLeft,
       HoeUp,
       HoeDown,
       IdleUp,
       IdleDown,
       IdleLeft,
       IdleRight,
       Count
    }

    public enum CharacterPartAnimator
    {
        Body,Arms,Hair,Tool,Hat,Count
    }

    public enum PartVariantColour
    {
        None,Count
    }

    public enum PartVariantType
    {
        none,Carry,Hoe,Pickaxe,Axe,Sickle,WateringCan,Count
    }
    public enum ToolEffect
    {
        None,Watering
    }

    public enum Direction
    {
        Up,Down,Left,Right,None
    }
    public enum ItemType
    {
        Seed,Commodity,Watering_Tool,HoeingTool,Chopping_Tool,BreakingTool,
        Reaping_Tool,Collecting_Tool,Reapable_scanary,Furniture,None,Count
    }

    public enum InventoryLocation
    {
        Player,Chest,Count
    }
}