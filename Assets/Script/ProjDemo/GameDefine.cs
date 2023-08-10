using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameDefine
{
    #region str

    public static string spawnPointStr = "SpawnPoint";

    public static string unitMaxHpStr = "UnitMaxHp";
    public static string unitHpStr = "CurUnitHp";
    public static string unitAtkStr = "UnitAtk";

    #endregion str

    #region layer

    static bool Init;

    public static int Terrain_Mask = LayerMask.GetMask("Terrain");
    public static int EditElement_Mask = LayerMask.GetMask("EditElement");

    #endregion layer

    public static void DoInit()
    {

    }

    #region group

    /// <summary>
    /// 获得敌对组别（待完善（可考虑mask
    /// </summary>
    /// <param name="g"></param>
    /// <returns></returns>
    public static E_Group GetOppsiteGroup(E_Group g)
    {
        switch (g)
        {
            case E_Group.None:

                return E_Group.None;
            case E_Group.Group_1:
                return E_Group.Group_2;

            case E_Group.Group_2:
                return E_Group.Group_1;
            default:
                return E_Group.None;
        }
    }

    #endregion group

}

#region Unit_Enum

public enum E_UnitType
{

    //Unit
    None = -1,
    TestCactust = 0,
    TestRedDragon = 1,
    TestGreenBat = 2,

    //Scene Obj

}

#endregion Unit_Enum

#region Element_Enum

public enum E_ElementClass
{

}



public enum E_ElementType
{
    None = -1,

    #region terrain_group

    #region crystal_red

    RedCrystal_1 = 310001,
    RedCrystal_2 = 310002,
    RedCrystal_3 = 310003,
    RedCrystal_4 = 310004,
    RedCrystalGroup_5 = 310005,
    RedCrystalGroup_6 = 310006,
    RedCrystalGroup_7 = 310007,
    RedCrystalGroup_8 = 310008,
    RedCrystalGroup_9 = 310009,
    RedCrystalGroup_10 = 3100010,
    RedCrystalGroup_11 = 3100011,
    RedCrystalGroup_12 = 3100012,
    RedCrystalGroup_13 = 3100013,
    RedCrystalGroup_14 = 3100014,
    RedCrystalGroup_15 = 3100015,
    RedCrystalGroup_16 = 3100016,
    RedCrystalGroup_17 = 3100017,
    RedCrystalGroup_18 = 3100018,
    RedCrystalGroup_19 = 3100019,
    RedCrystalGroup_20 = 3100020,
    RedCrystalGroup_21 = 3100021,
    RedCrystalGroup_22 = 3100022,
    RedCrystalGroup_23 = 3100023,
    RedCrystalGroup_24 = 3100024,
    RedCrystalGroup_25 = 3100025,
    RedCrystalGroup_26 = 3100026,
    RedCrystalGroup_27 = 3100027,

    #endregion crystal_red

    #region crystal_blue

    BlueCrystal_1 = 320001,
    BlueCrystal_2 = 320002,
    BlueCrystal_3 = 320003,
    BlueCrystal_4 = 320004,
    BlueCrystalGroup_5 = 320005,
    BlueCrystalGroup_6 = 320006,
    BlueCrystalGroup_7 = 320007,
    BlueCrystalGroup_8 = 320008,
    BlueCrystalGroup_9 = 320009,
    BlueCrystalGroup_10 = 3200010,
    BlueCrystalGroup_11 = 3200011,
    BlueCrystalGroup_12 = 3200012,
    BlueCrystalGroup_13 = 3200013,
    BlueCrystalGroup_14 = 3200014,
    BlueCrystalGroup_15 = 3200015,
    BlueCrystalGroup_16 = 3200016,
    BlueCrystalGroup_17 = 3200017,
    BlueCrystalGroup_18 = 3200018,
    BlueCrystalGroup_19 = 3200019,
    BlueCrystalGroup_20 = 3200020,
    BlueCrystalGroup_21 = 3200021,
    BlueCrystalGroup_22 = 3200022,
    BlueCrystalGroup_23 = 3200023,
    BlueCrystalGroup_24 = 3200024,
    BlueCrystalGroup_25 = 3200025,
    BlueCrystalGroup_26 = 3200026,
    BlueCrystalGroup_27 = 3200027,

    #endregion crystal_blue

    #region crystal_green

    GreenCrystal_1 = 330001,
    GreenCrystal_2 = 330002,
    GreenCrystal_3 = 330003,
    GreenCrystal_4 = 330004,
    GreenCrystalGroup_5 = 330005,
    GreenCrystalGroup_6 = 330006,
    GreenCrystalGroup_7 = 330007,
    GreenCrystalGroup_8 = 330008,
    GreenCrystalGroup_9 = 330009,
    GreenCrystalGroup_10 = 3300010,
    GreenCrystalGroup_11 = 3300011,
    GreenCrystalGroup_12 = 3300012,
    GreenCrystalGroup_13 = 3300013,
    GreenCrystalGroup_14 = 3300014,
    GreenCrystalGroup_15 = 3300015,
    GreenCrystalGroup_16 = 3300016,
    GreenCrystalGroup_17 = 3300017,
    GreenCrystalGroup_18 = 3300018,
    GreenCrystalGroup_19 = 3300019,
    GreenCrystalGroup_20 = 3300020,
    GreenCrystalGroup_21 = 3300021,
    GreenCrystalGroup_22 = 3300022,
    GreenCrystalGroup_23 = 3300023,
    GreenCrystalGroup_24 = 3300024,
    GreenCrystalGroup_25 = 3300025,
    GreenCrystalGroup_26 = 3300026,
    GreenCrystalGroup_27 = 3300027,

    #endregion crystal_green

    #region crystal_orange

    OrangeCrystal_1 = 340001,
    OrangeCrystal_2 = 340002,
    OrangeCrystal_3 = 340003,
    OrangeCrystal_4 = 340004,
    OrangeCrystalGroup_5 = 340005,
    OrangeCrystalGroup_6 = 340006,
    OrangeCrystalGroup_7 = 340007,
    OrangeCrystalGroup_8 = 340008,
    OrangeCrystalGroup_9 = 340009,
    OrangeCrystalGroup_10 = 3400010,
    OrangeCrystalGroup_11 = 3400011,
    OrangeCrystalGroup_12 = 3400012,
    OrangeCrystalGroup_13 = 3400013,
    OrangeCrystalGroup_14 = 3400014,
    OrangeCrystalGroup_15 = 3400015,
    OrangeCrystalGroup_16 = 3400016,
    OrangeCrystalGroup_17 = 3400017,
    OrangeCrystalGroup_18 = 3400018,
    OrangeCrystalGroup_19 = 3400019,
    OrangeCrystalGroup_20 = 3400020,
    OrangeCrystalGroup_21 = 3400021,
    OrangeCrystalGroup_22 = 3400022,
    OrangeCrystalGroup_23 = 3400023,
    OrangeCrystalGroup_24 = 3400024,
    OrangeCrystalGroup_25 = 3400025,
    OrangeCrystalGroup_26 = 3400026,
    OrangeCrystalGroup_27 = 3400027,

    #endregion crystal_orange

    #region crystal_pink

    PinkCrystal_1 = 350001,
    PinkCrystal_2 = 350002,
    PinkCrystal_3 = 350003,
    PinkCrystal_4 = 350004,
    PinkCrystalGroup_5 = 350005,
    PinkCrystalGroup_6 = 350006,
    PinkCrystalGroup_7 = 350007,
    PinkCrystalGroup_8 = 350008,
    PinkCrystalGroup_9 = 350009,
    PinkCrystalGroup_10 = 3500010,
    PinkCrystalGroup_11 = 3500011,
    PinkCrystalGroup_12 = 3500012,
    PinkCrystalGroup_13 = 3500013,
    PinkCrystalGroup_14 = 3500014,
    PinkCrystalGroup_15 = 3500015,
    PinkCrystalGroup_16 = 3500016,
    PinkCrystalGroup_17 = 3500017,
    PinkCrystalGroup_18 = 3500018,
    PinkCrystalGroup_19 = 3500019,
    PinkCrystalGroup_20 = 3500020,
    PinkCrystalGroup_21 = 3500021,
    PinkCrystalGroup_22 = 3500022,
    PinkCrystalGroup_23 = 3500023,
    PinkCrystalGroup_24 = 3500024,
    PinkCrystalGroup_25 = 3500025,
    PinkCrystalGroup_26 = 3500026,
    PinkCrystalGroup_27 = 3500027,

    #endregion crystal_pink


    #region cube

    Cube_Balk = 400001,
    Cube_Balk2 = 400002,
    Cube_Balk2_Dark = 400003,
    Cube_Balk2_Light = 400004,
    Cube_Balk3 = 400005,
    Cube_Balk3_Dark = 400006,
    Cube_Balk3_Light = 400007,
    Cube_Balk_Dark = 400008,
    Cube_Balk_Light = 400009,
    Cube_BalkHalf = 4000010,
    Cube_BalkHalf_Dark = 4000011,
    Cube_BalkHalf_Light = 4000012,
    Cube_Concrete = 4000013,
    Cube_Grass = 4000014,
    Cube_Ground = 4000015,
    Cube_GroundDried = 4000016,
    Cube_GroundWGrass = 4000017,
    Cube_GruondWGrass_2 = 4000018,
    Cube_GroundWGrass_3 = 4000019,
    Cube_GroundWGrass_4 = 4000020,
    Cube_GroundWGrass_5 = 4000021,
    Cube_GroundWGrass_6 = 4000022,
    Cube_Half = 4000023,
    Cube_Half_Dark = 4000024,
    Cube_Half_Light = 4000025,
    Cube_Ice = 4000026,
    Cube_Lava = 4000027,
    Cube_Magma = 4000028,
    Cube_Quarter = 4000029,
    Cube_Quarter_Dark = 4000030,
    Cube_Quarter_Light = 4000031,
    Cube_Rock = 4000032,
    Cube_Rock2 = 4000033,
    Cube_RockWGrass_1 = 4000034,
    Cube_RockWGrass_2 = 4000035,
    Cube_RockWGrass_3 = 4000036,
    Cube_RockWGrass_4 = 4000037,
    Cube_RockWGrass_5 = 4000038,
    Cube_RockWGrass_6 = 4000039,
    Cube_Sand = 4000040,
    Cube_Snow = 4000041,
    Cube_Soil = 4000042,
    Cube_SoilWGrass_1 = 4000043,
    Cube_SoilWGrass_2 = 4000044,
    Cube_SoilWGrass_3 = 4000045,
    Cube_SoilWGrass_4 = 4000046,
    Cube_SoilWGrass_5 = 4000047,
    Cube_SoilWGrass_6 = 4000048,
    Cube_Stone1 = 4000049,
    Cube_Stone2 = 4000050,
    Cube_Stone3 = 4000051,
    Cube_Stone4 = 4000052,
    Cube_Water = 4000053,
    Cube_WaterGreen = 4000054,
    Cube_Wood_Dark = 4000055,
    Cube_Wood_Light = 4000056,
    Cube_WoodNormal = 4000057,

    #endregion cube


    #region item

    Bag = 500001,
    Barrel = 500002,
    BarrelsGroup1 = 500003,
    Box = 500004,
    Bridge = 500005,
    Bucket = 500006,
    Camp = 500007,
    Canon = 500008,
    Carriage = 500009,
    Chest = 5000010,
    ChestOpen = 5000011,
    Coin = 5000012,
    CoinsGroup1 = 5000013,
    CoinsGroup2 = 5000014,
    Fance = 5000015,
    FanceGate = 5000016,
    FanceWooden = 5000017,
    FanceWooden2 = 5000018,
    FirePlace = 5000019,
    Flag = 5000020,
    Pier = 5000021,
    Railway = 5000022,
    RailwayCorer = 5000023,
    Shield1 = 5000024,
    Shield2 = 5000025,
    Shield3 = 5000026,
    Stairs = 5000027,
    Torch_1 = 5000028,
    Torch = 5000029,
    Wood1 = 5000030,
    Wood2 = 5000031,
    Wood3 = 5000032,

    #endregion item


    #region rock_green

    GreenRock_1 = 610001,
    GreenRock_2 = 610002,
    GreenRock_3 = 610003,
    GreenRock_4 = 610004,
    GreenRock_5 = 610005,
    GreenRock_6 = 610006,
    GreenRock_7 = 610007,
    GreenRock_8 = 610008,
    GreenRock_9 = 610009,
    GreenRock_10 = 6100010,
    GreenRock_11 = 6100011,
    GreenRock_12 = 6100012,
    GreenRock_13 = 6100013,
    GreenRock14 = 6100014,
    GreenRock15 = 6100015,
    GreenRockFlat_1 = 6100016,
    GreenRockFlat_2 = 6100017,
    GreenRockFlat_3 = 6100018,
    GreenRockFlat_4 = 6100019,
    GreenRockGround_1 = 6100020,
    GreenRockGround_2 = 6100021,
    GreenRockGround_3 = 6100022,
    GreenRockGround_4 = 6100023,
    GreenRockLayered_1 = 6100024,
    GreenRockLayered_2 = 6100025,
    GreenRockLayered_3 = 6100026,
    GreenRockLayered_4 = 6100027,
    GreenRockLayered_5 = 6100028,
    GreenRockSmallGroup_1 = 6100029,
    GreenRockSmallGroup_2 = 6100030,
    GreenRockSmallGroup_3 = 6100031,
    GreenRockSmallGroup_4 = 6100032,
    GreenRockSmallGroup_5 = 6100033,
    GreenRockSmallGroup_6 = 6100034,
    GreenRockSmallGroup_7 = 6100035,
    GreenRockSmallGroup_8 = 6100036,
    GreenRockSmallGroup_9 = 6100037,
    GreenRockSmallGroup_10 = 6100038,
    GreenRockSmallGroup_11 = 6100039,

    #endregion rock_green

    #region rock_blue

    BlueRock_1 = 620001,
    BlueRock_2 = 620002,
    BlueRock_3 = 620003,
    BlueRock_4 = 620004,
    BlueRock_5 = 620005,
    BlueRock_6 = 620006,
    BlueRock_7 = 620007,
    BlueRock_8 = 620008,
    BlueRock_9 = 620009,
    BlueRock_10 = 6200010,
    BlueRock_11 = 6200011,
    BlueRock_12 = 6200012,
    BlueRock_13 = 6200013,
    BlueRock14 = 6200014,
    BlueRock15 = 6200015,
    BlueRockFlat_1 = 6200016,
    BlueRockFlat_2 = 6200017,
    BlueRockFlat_3 = 6200018,
    BlueRockFlat_4 = 6200019,
    BlueRockGround_1 = 6200020,
    BlueRockGround_2 = 6200021,
    BlueRockGround_3 = 6200022,
    BlueRockGround_4 = 6200023,
    BlueRockLayered_1 = 6200024,
    BlueRockLayered_2 = 6200025,
    BlueRockLayered_3 = 6200026,
    BlueRockLayered_4 = 6200027,
    BlueRockLayered_5 = 6200028,
    BlueRockSmallGroup_1 = 6200029,
    BlueRockSmallGroup_2 = 6200030,
    BlueRockSmallGroup_3 = 6200031,
    BlueRockSmallGroup_4 = 6200032,
    BlueRockSmallGroup_5 = 6200033,
    BlueRockSmallGroup_6 = 6200034,
    BlueRockSmallGroup_7 = 6200035,
    BlueRockSmallGroup_8 = 6200036,
    BlueRockSmallGroup_9 = 6200037,
    BlueRockSmallGroup_10 = 6200038,
    BlueRockSmallGroup_11 = 6200039,
    #endregion rock_blue

    #region rock_brown

    BrownRock_1 = 630001,
    BrownRock_2 = 630002,
    BrownRock_3 = 630003,
    BrownRock_4 = 630004,
    BrownRock_5 = 630005,
    BrownRock_6 = 630006,
    BrownRock_7 = 630007,
    BrownRock_8 = 630008,
    BrownRock_9 = 630009,
    BrownRock_10 = 6300010,
    BrownRock_11 = 6300011,
    BrownRock_12 = 6300012,
    BrownRock_13 = 6300013,
    BrownRock14 = 6300014,
    BrownRock15 = 6300015,
    BrownRockFlat_1 = 6300016,
    BrownRockFlat_2 = 6300017,
    BrownRockFlat_3 = 6300018,
    BrownRockFlat_4 = 6300019,
    BrownRockGround_1 = 6300020,
    BrownRockGround_2 = 6300021,
    BrownRockGround_3 = 6300022,
    BrownRockGround_4 = 6300023,
    BrownRockLayered_1 = 6300024,
    BrownRockLayered_2 = 6300025,
    BrownRockLayered_3 = 6300026,
    BrownRockLayered_4 = 6300027,
    BrownRockLayered_5 = 6300028,
    BrownRockSmallGroup_1 = 6300029,
    BrownRockSmallGroup_2 = 6300030,
    BrownRockSmallGroup_3 = 6300031,
    BrownRockSmallGroup_4 = 6300032,
    BrownRockSmallGroup_5 = 6300033,
    BrownRockSmallGroup_6 = 6300034,
    BrownRockSmallGroup_7 = 6300035,
    BrownRockSmallGroup_8 = 6300036,
    BrownRockSmallGroup_9 = 6300037,
    BrownRockSmallGroup_10 = 6300038,
    BrownRockSmallGroup_11 = 6300039,

    #endregion rock_brown

    #endregion terrain_group
}

#endregion Element_Enum


#region Battle_Enum

public enum E_LongDistAtkType
{
    None=0,
    Dragon_FireBall=1,

}

/// <summary>
/// 分组
/// </summary>
public enum E_Group
{
    None = 0,
    Group_1 = 1,
    Group_2 = 2,

}

#endregion Battle_Enum