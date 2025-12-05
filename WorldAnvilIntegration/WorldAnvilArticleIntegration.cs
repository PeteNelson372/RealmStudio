using FontAwesome.Sharp;
using System.ComponentModel;
/**************************************************************************************************************************
* Copyright 2025, Peter R. Nelson
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

namespace RealmStudio.WorldAnvilIntegration
{
    public partial class WorldAnvilArticleIntegration : Form
    {
        private static readonly ToolTip TOOLTIP = new();

        private string? _articleContent;
        private string? _articleTitle;
        private ArticleType _articleType;
        private string? _worldAnvilWorldId;
        private Guid? _worldAnvilArticleId;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal string? ArticleContent
        {
            get { return _articleContent; }
            set { _articleContent = value; }
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal string? ArticleTitle
        {
            get { return _articleTitle; }
            set { _articleTitle = value; }
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal ArticleType ArticleType
        {
            get { return _articleType; }
            set { _articleType = value; }
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal string? WorldAnvilWorldId
        {
            get { return _worldAnvilWorldId; }
            set { _worldAnvilWorldId = value; }
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal Guid? WorldAnvilArticleId
        {
            get { return _worldAnvilArticleId; }
            set { _worldAnvilArticleId = value; }
        }

        public WorldAnvilArticleIntegration()
        {
            InitializeComponent();
        }

        public DialogResult ShowDialog(Form parent)
        {
            WorldAnvilWorldId = IntegrationManager.WorldAnvilParameters.WorldId;
            MapWorldIdLabel.Text = WorldAnvilWorldId;
            ArticleTitleTextBox.Text = ArticleTitle;
            ArticleContentTextBox.Text = ArticleContent;

            return base.ShowDialog(parent);
        }

        private void CloseArticleIntegrationButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CreateDescriptionArticleButton_Click(object sender, EventArgs e)
        {

        }

        private void CreateDescriptionArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Create the World Anvil article with the given title, type, and content.", this, new Point(CreateDescriptionArticleButton.Left, CreateDescriptionArticleButton.Top - 20), 3000);
        }

        private void SetArticleType(ArticleType articleType, IconButton articleButton)
        {
            foreach (Control control in ArticleTypeTable.Controls)
            {
                if (control is IconButton button && button != articleButton)
                {
                    button.FlatAppearance.BorderColor = Color.Black;
                    button.FlatAppearance.BorderSize = 1;
                }
            }

            ArticleType = articleType;
            articleButton.FlatAppearance.BorderSize = 4;
            articleButton.FlatAppearance.BorderColor = Color.FromArgb(92, 140, 10, 10);
        }


        private void GenericArticleButton_Click(object sender, EventArgs e)
        {
            SetArticleType(ArticleType.Generic, GenericArticleButton);
        }

        private void GenericArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Set the World Anvil article type to GENERIC.", this, new Point(GenericArticleButton.Left, GenericArticleButton.Top - 20), 3000);
        }

        private void BuildingArticleButton_Click(object sender, EventArgs e)
        {
            SetArticleType(ArticleType.Building, BuildingArticleButton);
        }

        private void BuildingArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Set the World Anvil article type to BUILDING.", this, new Point(BuildingArticleButton.Left, BuildingArticleButton.Top - 20), 3000);
        }

        private void CharacterArticleButton_Click(object sender, EventArgs e)
        {
            SetArticleType(ArticleType.Character, CharacterArticleButton);
        }

        private void CharacterArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Set the World Anvil article type to CHARACTER.", this, new Point(CharacterArticleButton.Left, CharacterArticleButton.Top - 20), 3000);
        }

        private void CountryArticleButton_Click(object sender, EventArgs e)
        {
            SetArticleType(ArticleType.Country, CountryArticleButton);
        }

        private void CountryArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Set the World Anvil article type to COUNTRY.", this, new Point(CountryArticleButton.Left, CountryArticleButton.Top - 20), 3000);
        }

        private void MilitaryArticleButton_Click(object sender, EventArgs e)
        {
            SetArticleType(ArticleType.Military, MilitaryArticleButton);
        }

        private void MilitaryArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Set the World Anvil article type to MILITARY.", this, new Point(MilitaryArticleButton.Left, MilitaryArticleButton.Top - 20), 3000);
        }

        private void DeityArticleButton_Click(object sender, EventArgs e)
        {
            SetArticleType(ArticleType.Deity, DeityArticleButton);
        }

        private void DeityArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Set the World Anvil article type to GOD/DEITY.", this, new Point(DeityArticleButton.Left, DeityArticleButton.Top - 20), 3000);
        }

        private void GeographyArticleButton_Click(object sender, EventArgs e)
        {
            SetArticleType(ArticleType.Geography, GeographyArticleButton);
        }

        private void GeographyArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Set the World Anvil article type to GEOGRAPHY.", this, new Point(GeographyArticleButton.Left, GeographyArticleButton.Top - 20), 3000);
        }

        private void ItemArticleButton_Click(object sender, EventArgs e)
        {
            SetArticleType(ArticleType.Item, ItemArticleButton);
        }

        private void ItemArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Set the World Anvil article type to ITEM.", this, new Point(ItemArticleButton.Left, ItemArticleButton.Top - 20), 3000);
        }

        private void OrganizationArticleButton_Click(object sender, EventArgs e)
        {
            SetArticleType(ArticleType.Organization, OrganizationArticleButton);
        }

        private void OrganizationArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Set the World Anvil article type to ORGANIZATION.", this, new Point(OrganizationArticleButton.Left, OrganizationArticleButton.Top - 20), 3000);
        }

        private void ReligionArticleButton_Click(object sender, EventArgs e)
        {
            SetArticleType(ArticleType.Religion, ReligionArticleButton);
        }

        private void ReligionArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Set the World Anvil article type to RELIGION.", this, new Point(ReligionArticleButton.Left, ReligionArticleButton.Top - 20), 3000);
        }

        private void SpeciesArticleButton_Click(object sender, EventArgs e)
        {
            SetArticleType(ArticleType.Species, SpeciesArticleButton);
        }

        private void SpeciesArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Set the World Anvil article type to SPECIES.", this, new Point(SpeciesArticleButton.Left, SpeciesArticleButton.Top - 20), 3000);
        }

        private void VehicleArticleButton_Click(object sender, EventArgs e)
        {
            SetArticleType(ArticleType.Vehicle, VehicleArticleButton);
        }

        private void VehicleArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Set the World Anvil article type to VEHICLE.", this, new Point(VehicleArticleButton.Left, VehicleArticleButton.Top - 20), 3000);
        }

        private void SettlementArticleButton_Click(object sender, EventArgs e)
        {
            SetArticleType(ArticleType.Settlement, SettlementArticleButton);
        }

        private void SettlementArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Set the World Anvil article type to SETTLEMENT.", this, new Point(SettlementArticleButton.Left, SettlementArticleButton.Top - 20), 3000);
        }

        private void ConditionArticleButton_Click(object sender, EventArgs e)
        {
            SetArticleType(ArticleType.Condition, ConditionArticleButton);
        }

        private void ConditionArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Set the World Anvil article type to CONDITION.", this, new Point(ConditionArticleButton.Left, ConditionArticleButton.Top - 20), 3000);
        }

        private void ConflictArticleButton_Click(object sender, EventArgs e)
        {
            SetArticleType(ArticleType.Conflict, ConflictArticleButton);
        }

        private void ConflictArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Set the World Anvil article type to CONFLICT.", this, new Point(ConflictArticleButton.Left, ConflictArticleButton.Top - 20), 3000);
        }

        private void DocumentArticleButton_Click(object sender, EventArgs e)
        {
            SetArticleType(ArticleType.Document, DocumentArticleButton);
        }

        private void DocumentArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Set the World Anvil article type to DOCUMENT.", this, new Point(DocumentArticleButton.Left, DocumentArticleButton.Top - 20), 3000);
        }

        private void CultureArticleButton_Click(object sender, EventArgs e)
        {
            SetArticleType(ArticleType.Culture, CultureArticleButton);
        }

        private void CultureArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Set the World Anvil article type to CULTURE/ETHNICITY.", this, new Point(CultureArticleButton.Left, CultureArticleButton.Top - 20), 3000);
        }

        private void LanguageArticleButton_Click(object sender, EventArgs e)
        {
            SetArticleType(ArticleType.Language, LanguageArticleButton);
        }

        private void LanguageArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Set the World Anvil article type to LANGUAGE.", this, new Point(LanguageArticleButton.Left, LanguageArticleButton.Top - 20), 3000);
        }

        private void MaterialArticleButton_Click(object sender, EventArgs e)
        {
            SetArticleType(ArticleType.Material, MaterialArticleButton);
        }

        private void MaterialArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Set the World Anvil article type to MATERIAL.", this, new Point(MaterialArticleButton.Left, MaterialArticleButton.Top - 20), 3000);
        }

        private void FormationArticleButton_Click(object sender, EventArgs e)
        {
            SetArticleType(ArticleType.MilitaryFormation, FormationArticleButton);
        }

        private void FormationArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Set the World Anvil article type to MILITARY FORMATION.", this, new Point(FormationArticleButton.Left, FormationArticleButton.Top - 20), 3000);
        }

        private void MythArticleButton_Click(object sender, EventArgs e)
        {
            SetArticleType(ArticleType.Myth, MythArticleButton);
        }

        private void MythArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Set the World Anvil article type to MYTH.", this, new Point(MythArticleButton.Left, MythArticleButton.Top - 20), 3000);
        }

        private void NaturalLawArticleButton_Click(object sender, EventArgs e)
        {
            SetArticleType(ArticleType.NaturalLaw, NaturalLawArticleButton);
        }

        private void NaturalLawArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Set the World Anvil article type to NATURAL LAW.", this, new Point(NaturalLawArticleButton.Left, NaturalLawArticleButton.Top - 20), 3000);
        }

        private void PlotArticleButton_Click(object sender, EventArgs e)
        {
            SetArticleType(ArticleType.Plot, PlotArticleButton);
        }

        private void PlotArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Set the World Anvil article type to PLOT.", this, new Point(PlotArticleButton.Left, PlotArticleButton.Top - 20), 3000);
        }

        private void ProfessionArticleButton_Click(object sender, EventArgs e)
        {
            SetArticleType(ArticleType.Profession, ProfessionArticleButton);
        }

        private void ProfessionArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Set the World Anvil article type to PROFESSION.", this, new Point(ProfessionArticleButton.Left, ProfessionArticleButton.Top - 20), 3000);
        }

        private void ProseArticleButton_Click(object sender, EventArgs e)
        {
            SetArticleType(ArticleType.Prose, ProseArticleButton);
        }

        private void ProseArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Set the World Anvil article type to PROSE.", this, new Point(ProseArticleButton.Left, ProseArticleButton.Top - 20), 3000);
        }

        private void TitleArticleButton_Click(object sender, EventArgs e)
        {
            SetArticleType(ArticleType.Title, TitleArticleButton);
        }

        private void TitleArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Set the World Anvil article type to TITLE.", this, new Point(TitleArticleButton.Left, TitleArticleButton.Top - 20), 3000);
        }

        private void SpellArticleButton_Click(object sender, EventArgs e)
        {
            SetArticleType(ArticleType.Spell, SpellArticleButton);
        }

        private void SpellArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Set the World Anvil article type to SPELL.", this, new Point(SpellArticleButton.Left, SpellArticleButton.Top - 20), 3000);
        }

        private void TechnologyArticleButton_Click(object sender, EventArgs e)
        {
            SetArticleType(ArticleType.Technology, TechnologyArticleButton);
        }

        private void TechnologyArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Set the World Anvil article type to TECHNOLOGY.", this, new Point(TechnologyArticleButton.Left, TechnologyArticleButton.Top - 20), 3000);
        }

        private void TraditionArticleButton_Click(object sender, EventArgs e)
        {
            SetArticleType(ArticleType.Tradition, TraditionArticleButton);
        }

        private void TraditionArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Set the World Anvil article type to TRADITION.", this, new Point(TraditionArticleButton.Left, TraditionArticleButton.Top - 20), 3000);
        }

        private void SessionReportArticleButton_Click(object sender, EventArgs e)
        {
            SetArticleType(ArticleType.SessionReport, SessionReportArticleButton);
        }

        private void SessionReportArticleButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Set the World Anvil article type to SESSION REPORT.", this, new Point(SessionReportArticleButton.Left, SessionReportArticleButton.Top - 20), 3000);
        }
    }
}
