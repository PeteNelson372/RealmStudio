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
* The text of the GNU General Public License (GPL) is found in the LICENSE.txt file.
* If the LICENSE.txt file is not present or the text of the GNU GPL is not present in the LICENSE.txt file,
* see https://www.gnu.org/licenses/.
*
* For questions about the RealmStudio application or about licensing, please email
* support@brookmonte.com
*
***************************************************************************************************************************/
namespace RealmStudio
{
    public enum MapDrawingMode
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
        RiverEdit,
        ColorSelect,
        LandformSelect,
        LandformHeightMapSelect,
        RealmAreaSelect,
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
        RegionPaint,
        HeightMapPaint,
        MapHeightIncrease,
        MapHeightDecrease,
    }

    public enum LandGradientDirection
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

    public enum WaterFeatureType
    {
        NotSet,
        Lake,
        River,
        Other
        // could define other types, like swamp, canal, inland sea, etc.
    }

    public enum PathType
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

    public enum ParallelDirection
    {
        Above,
        Below
    }

    public enum SymbolFileFormat
    {
        NotSet,
        PNG,
        JPG,
        BMP,
        Vector
    }

    public enum MapSymbolType
    {
        NotSet,
        Structure,
        Vegetation,
        Terrain,
        Marker,
        Other
    }

    public enum ComponentMoveDirection
    {
        Up, Down
    }

    public enum LabelTextAlignment
    {
        AlignLeft,
        AlignCenter,
        AlignRight
    }

    public enum MapGridType
    {
        NotSet,
        Square,
        FlatHex,
        PointedHex
    }

    public enum ScaleNumbersDisplayLocation
    {
        None,
        Ends,
        EveryOther,
        All
    }

    public enum GeneratedLandformType
    {
        NotSet,
        Region,
        Continent,
        Island,
        Archipelago,
        Atoll,
        World,
        Icecap
    }

    public enum RealmMapType
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

    public enum FontPanelOpener
    {
        NotSet,
        LabelFontButton,
        ScaleFontButton
    }

    public enum RealmMeasurementUnits
    {
        NotSet,
        Metric,
        USCustomary
    }

    public enum RealmMapExportFormat
    {
        NotSet,
        PNG,
        JPG,
        BMP,
        GIF,
    }

    public enum RealmExportType
    {
        NotSet,
        BitmapImage,
        UpscaledImage,
        MapLayers,
        Heightmap,
        HeightMap3DModel,
    }

    /// <summary>
    /// Enumeration of Panose Font Family Types.  These can be used for
    /// determining the similarity of two fonts or for detecting non-character
    /// fonts like WingDings.
    /// </summary>
    public enum PanoseFontFamilyTypes : int
    {
        /// <summary>
        ///  Any
        /// </summary>
        PanAny = 0,
        /// <summary>
        /// No Fit
        /// </summary>
        PanNoFit = 1,
        /// <summary>
        /// Text and Display
        /// </summary>
        PanFamilyTextDisplay = 2,
        /// <summary>
        /// Script
        /// </summary>
        PanFamilyScript = 3,
        /// <summary>
        /// Decorative
        /// </summary>
        PanFamilyDecorative = 4,
        /// <summary>
        /// Pictorial                      
        /// </summary>
        PanFamilyPictorial = 5
    }

    public enum ResizeMapAnchorPoint
    {
        TopLeft,
        TopCenter,
        TopRight,
        Center,
        CenterLeft,
        CenterRight,
        CenterZoomed,
        BottomLeft,
        BottomCenter,
        BottomRight,
        DetailMap
    }

    public enum VignetteShapeType
    {
        Oval,
        Rectangle
    }

    public enum LocalStarImageType
    {
        Sun,
        Nebula,
        GasGiant,
        Corona,
        BlackHole,
    }
}
