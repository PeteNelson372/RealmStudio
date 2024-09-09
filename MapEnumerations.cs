﻿/**************************************************************************************************************************
* Copyright 2024, Peter R. Nelson
*
* This file is part of the RealmStudio application. The RealmStudio application is intended
* for creating fantasy maps for gaming and world building.
*
* RealmStudio is free software: you can redistribute it and/or modify it under the terms
* of the GNU General Public License as published by the Free Software Foundation,
* either version 3 of the License, or (at your option) any later version.
*
* This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
* without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
* See the GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License along with this program.
* The text of the GNU General Public License (GPL) is found in the LICENSE file.
* If the LICENSE file is not present or the text of the GNU GPL is not present in the LICENSE file,
* see https://www.gnu.org/licenses/.
*
* For questions about the RealmStudio application or about licensing, please email
* contact@brookmonte.com
*
***************************************************************************************************************************/
namespace RealmStudio
{
    public enum DrawingModeEnum
    {
        None,
        OceanPaint,
        OceanErase,
        LandPaint,
        LandErase,
        LandColor,
        LandColorErase,
        WaterPaint,
        WaterErase,
        WaterColor,
        WaterColorErase,
        LakePaint,
        RiverPaint,
        ColorSelect,
        LandformSelect,
        LandformAreaSelect,
        WaterFeatureSelect,
        PathSelect,
        PathPaint,
        PathEdit,
        SymbolSelect,
        SymbolPlace,
        SymbolErase,
        SymbolMove,
        SymbolColor,
        LabelSelect,
        DrawLabel,
        DrawArcLabelPath,
        DrawBezierLabelPath,
        DrawBox,
        PlaceWindrose,
        SelectMapScale,
        DrawMapMeasure,
        RegionSelect,
        RegionPaint
    }

    public enum GradientDirectionEnum
    {
        None,
        DarkToLight,
        LightToDark
    }

    public enum ColorPaintBrush
    {
        None,
        SoftBrush,
        HardBrush
    }

    public enum  WaterFeatureTypeEnum
    {
        NotSet,
        Lake,
        River,
        Other
        // could define other types, like swamp, canal, inland sea, etc.
    }

    public enum PathTypeEnum
    {
        SolidLinePath,
        DottedLinePath,
        DashedLinePath,
        DashDotLinePath,
        DashDotDotLinePath,
        DoubleSolidBorderPath,
        ChevronLinePath,
        LineAndDashesPath,
        ShortIrregularDashPath,
        ThickSolidLinePath,
        SolidBlackBorderPath,
        BorderedGradientPath,
        BorderedLightSolidPath,
        BearTracksPath,
        BirdTracksPath,
        FootprintsPath,
        RailroadTracksPath,
        TexturedPath,
        BorderAndTexturePath
    }

    public enum ParallelEnum
    {
        Above,
        Below
    }

    public enum SymbolFormatEnum
    {
        NotSet,
        PNG,
        JPG,
        BMP,
        Vector
    }

    public enum SymbolTypeEnum
    {
        NotSet,
        Structure,
        Vegetation,
        Terrain,
        Other
    }

    public enum ComponentMoveDirectionEnum
    {
        Up, Down
    }

    public enum LabelTextAlignEnum
    {
        AlignLeft,
        AlignCenter,
        AlignRight
    }

    public enum GridTypeEnum
    {
        NotSet,
        Square,
        FlatHex,
        PointedHex
    }

    public enum ScaleNumbersDisplayEnum
    {
        None,
        Ends,
        EveryOther,
        All
    }

    public enum GeneratedLandformTypeEnum
    {
        Random,
        Continent,
        Atoll,
        Archipelago,
        World,
        Equirectangular
    }

    public enum RealmTypeEnum
    {
        World,
        Region,
        City,
        Interior,
        Dungeon,
        SolarSystem,
        Ship,
        Other
    }
}
