using System.Collections.Generic;
using System.Windows.Forms;
using ipsFormsDesign.Model;
using WebAPIConnector;
using sysAdminwp.ComplexDictionariesBaseControls;
using sysAdminwp.PresenterComplexDictionaries;

namespace sysAdminwp.viewPageDictionaryControls
{
    public partial class ComplexDictionaryControl : UserControl
    {
        private readonly ApiConnector _connector;

        private const string PrefixUpdate = "dictionary/complex/update/";
        private const string PrefixCreate = "dictionary/complex/addnew/";
        private const string PrefixDelete = "dictionary/complex/delete/";

        //for saved complex dictionaries from DictionaryForm and use for
        //- UrlForComplexDictionaries
        //- ComplexDictionaryName
        private List<DicCategoryItems> complexDictionary;
        public List<DicCategoryItems> ComplexDictionarySaved
        {
            get { return complexDictionary; }
            set
            {
                if (value != null)
                    complexDictionary = value;
            }
        }
        
        //mass for color setting for textBox color
        private byte[] borderDefault = new byte[4];
        //sequence of complex dictionaries
        private readonly List<string> _complexDictionaryList = new List<string>
        {
            "copyrightkind",
            "country",
            "creativeroletype",
            "genretype",
            "kl_r030",
            "systemconstant",
            "typeplace"
        };

        public string UrlForComplexDictionaries
        {
            get
            {
                var nodeComplexDictionary = complexTreeView.SelectedNode;
                if (nodeComplexDictionary == null) return string.Empty;
                var currentDataComplexdictionary = ComplexDictionarySaved[nodeComplexDictionary.Index];
                if (currentDataComplexdictionary == null) return string.Empty;
                return currentDataComplexdictionary.Url;
            }
        }

        public string ComplexDictionaryName
        {
            get
            {
                var node = complexTreeView.SelectedNode;
                if (node == null) return string.Empty;
                var currentDataDictionary = ComplexDictionarySaved[node.Index];
                if (currentDataDictionary == null) return string.Empty;
                return currentDataDictionary.DicName;
            }
        }
        public ComplexDictionaryControl(ApiConnector connector, List<DicCategoryItems> complexDictionaries)
        {
            InitializeComponent();
            //initialize connector
            _connector = connector;
            //save for NodeSelected property
            ComplexDictionarySaved = complexDictionaries;
            //initialize data for TreeView
            SetComplexDictionaryToTree(complexDictionaries);
        }

        //initialize sequence complex dictionaries for 'TreeView' element
        private void SetComplexDictionaryToTree(List<DicCategoryItems> complexDictionaries)
        {
            //set data for element
            complexTreeView.DataSource = complexDictionaries;
            complexTreeView.DisplayMember = "Name";
        }

        //initialize data for grid of the selected dictionary
        private void complexTreeView_SelectedNodeChanged(object sender, Telerik.WinControls.UI.RadTreeViewEventArgs e)
        {        
            switch (ComplexDictionaryName.ToLower())
            {
                case "copyrightkind":
                    //revome other controls
                    tableLayout_MarkupElement.Controls.Remove(tableLayout_MarkupElement.GetControlFromPosition(1, 0));
                    DirectorLayers<CopyrightKind> directorCopyright = new DirectorLayers<CopyrightKind>();
                    BaseCopyrightKindControl baseCopyrightKind = (BaseCopyrightKindControl)directorCopyright.InitCommon(_connector, UrlForComplexDictionaries);
                    tableLayout_MarkupElement.Controls.Add(baseCopyrightKind, 1, 0);
                    break;
                case "country":
                    //revome other controls
                    tableLayout_MarkupElement.Controls.Remove(tableLayout_MarkupElement.GetControlFromPosition(1, 0));
                    DirectorLayers<Country> directorCountry = new DirectorLayers<Country>();
                    BaseCountryControl baseCountry = (BaseCountryControl)directorCountry.InitCommon(_connector, UrlForComplexDictionaries);
                    tableLayout_MarkupElement.Controls.Add(baseCountry, 1, 0);
                    break;
                case "creativeroletype":
                    //revome other controls
                    tableLayout_MarkupElement.Controls.Remove(tableLayout_MarkupElement.GetControlFromPosition(1, 0));
                    DirectorLayers<CreativeRoleType> directorCreative = new DirectorLayers<CreativeRoleType>();
                    BaseCreativeroleTypeControl baseCreativeroleType = (BaseCreativeroleTypeControl)directorCreative.InitCommon(_connector, UrlForComplexDictionaries);
                    tableLayout_MarkupElement.Controls.Add(baseCreativeroleType, 1, 0);
                    break;
                case "genretype":
                    //revome other controls
                    tableLayout_MarkupElement.Controls.Remove(tableLayout_MarkupElement.GetControlFromPosition(1, 0));
                    DirectorLayers<GenreType> directorGenre = new DirectorLayers<GenreType>();
                    BaseGenreTypeControl baseGenreType = (BaseGenreTypeControl)directorGenre.InitCommon(_connector, UrlForComplexDictionaries);
                    tableLayout_MarkupElement.Controls.Add(baseGenreType, 1, 0);
                    break;
                case "kl_r030":
                    //revome other controls
                    tableLayout_MarkupElement.Controls.Remove(tableLayout_MarkupElement.GetControlFromPosition(1, 0));
                    DirectorLayers<KlR030> directorKlr = new DirectorLayers<KlR030>();
                    BaseKlr030Control baseKlr030 = (BaseKlr030Control)directorKlr.InitCommon(_connector, UrlForComplexDictionaries);
                    tableLayout_MarkupElement.Controls.Add(baseKlr030, 1, 0);
                    break;
                case "systemconstant":
                    //revome other controls
                    tableLayout_MarkupElement.Controls.Remove(tableLayout_MarkupElement.GetControlFromPosition(1, 0));
                    DirectorLayers<SystemConstant> directorSystem = new DirectorLayers<SystemConstant>();
                    BaseSystemConstantControl baseSystemConstant = (BaseSystemConstantControl)directorSystem.InitCommon(_connector, UrlForComplexDictionaries);
                    tableLayout_MarkupElement.Controls.Add(baseSystemConstant, 1, 0);
                    break;
                case "typeplace":
                    //revome other controls
                    tableLayout_MarkupElement.Controls.Remove(tableLayout_MarkupElement.GetControlFromPosition(1, 0));
                    DirectorLayers<TypePlace> directorType = new DirectorLayers<TypePlace>();
                    BaseTypePlaceControl baseTypePlace = (BaseTypePlaceControl)directorType.InitCommon(_connector, UrlForComplexDictionaries);
                    tableLayout_MarkupElement.Controls.Add(baseTypePlace, 1, 0);
                    break;
            }
        }

        private void ddlEditKeyField_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            if (e.Position == -1)
            {
                return;
            }

        }
    }
}
