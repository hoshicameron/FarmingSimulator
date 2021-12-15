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

    public enum SoundName
    {
        None = 0,
        EffectFootstepSoftGround = 10,
        EffectFootstepHardGround = 20,
        EffectAxe = 30,
        EffectPickaxe = 40,
        EffectScythe = 50,
        EffectHoe = 60,
        EffectWateringCan = 70,
        EffectBasket = 80,
        EffectPickupSound = 90,
        EffectRustle = 100,
        EffectTreeFalling = 110,
        EffectPlantingSound = 120,
        EffectPluck = 130,
        EffectStoneShatter = 140,
        EffectWoodSplinters = 150,
        AmbientCountryside1 = 1000,
        AmbientCountryside2 = 1010,
        AmbientIndoors1 = 1020,
        MusicCalm3 = 2000,
        MusicCalm1 = 2010
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

    public enum Season
    {
        Spring,
        Summer,
        Autumn,
        Winter,
        None,
        Count
    }

    public enum HarvestActionEffect
    {
        DeciduousLeavesFalling,
        PineConesFalling,
        ChoppingTreeTrunk,
        BreakingStone,
        Reaping,
        None
    }

    public enum Weather
    {
        Dry,
        Raining,
        Snowing,
        None,
        Count
    }

    public enum GridBoolProperty
    {
        Diggable,
        canDropItem,
        canPlaceFurniture,
        isPath,
        isNPCObstcale
    }

    public enum SceneName
    {
        Scene1_Farm,
        Scene2_Field,
        Scene3_Cabin
    }

}