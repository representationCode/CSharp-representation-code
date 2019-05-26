using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using WebAPIConnector;
using ipsFormsDesign.Model;
using ipsFormsDesign.Dictionary;
using Telerik.WinControls.UI;
using ipsFormsDesign.Services;
using IpsViewComponentLibrary.Extensions;
using sysAdminwp.PresenterComplexDictionaries.DefineComplexLayers;
using sysAdminwp.PresenterComplexDictionaries;
using System.Threading;

namespace sysAdminwp.ComplexDictionariesBaseControls
{
    public partial class BaseCreativeroleTypeControl : UserControl
    {
        CreativeroleTypeLayer _layer = new CreativeroleTypeLayer();
        public delegate CreativeRoleType GetDictionaryLayer(int id);
        public delegate CreativeRoleType CreateLayer(CreativeRoleType sors);
        public delegate CreativeRoleType UpdateLayer(CreativeRoleType sors);
        public delegate bool DeleteLayer(int id);
        private readonly List<string> Langs = new List<string>
        {
            "Uk", "Ru", "En"
        };

        private const string DelMess = "Об'єкт не видалений!";
        private const string GetMess = "об'єкт не знайдено";
        private const string CreateMess = "Помилка формату";
        private const string UpdateMess = "Помилка формату";
        private const string UrlBaseDict = "dictionary/base/Category";

        private readonly DictionaryService _dictService = DictionaryService.GetInstance();
        private readonly ApiConnector _connector;

        //define data for differenet languages
        private CreativeRoleTypeTr ukCreativeroleType;
        private CreativeRoleTypeTr ruCreativeroleType;
        private CreativeRoleTypeTr enCreativeroleType;
        
        //define grid data
        private List<CreativeRoleType> _creativeroleTypeList;
        public List<CreativeRoleType> CreativeroleTypeList
        {
            get => _creativeroleTypeList;
            set
            {
                if (value != null)
                {
                    _creativeroleTypeList = value;
                    //show current count elements into grid
                    totalCreativeroleType.Text = _creativeroleTypeList.Count.ToString();
                }
            }
        }
        //item data
        private CreativeRoleType creativeroleTypeData;

        //url for get data to grid
        private readonly string urlDataGrid;

        //for control current select of grid
        private int indexSelectedElem;

        //control 'Enabled' state for elements
        private bool controlEnabledActiveElements = false;
        public bool ControlEnabledActiveElements
        {
            get => controlEnabledActiveElements;
            set
            {
                controlEnabledActiveElements = value;
                //'search' element
                txtSearchCreativeroleType.Enabled = value;
                //filter for grid
                ddlFilterCreativeroleType.Enabled = value;
                //grid element
                gridCreativeroleTypeList.Enabled = value;
                //'Add' button
                btnAddCreativeroleType.Enabled = value;
                //'Delete' button
                btnDeleteCreativeroleType.Enabled = value;
                //'Save' button (bottom panel)
                btnSaveCreativeroleType.Enabled = !value;
                //'Cancel' button (bottom panel)
                btnCancelCreativeroleType.Enabled = !value;
            }
        }

        public BaseCreativeroleTypeControl(ApiConnector connector, string urlForComplexDict)
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            _connector = connector;
            urlDataGrid = urlForComplexDict;
            //initialize data for drop down lists
            ddlFilterCreativeroleType.DataSource = bsSimpleDictCreativeRoleType.DataSource = InitializeDataForFilter();
            //initialize data for grid
            gridCreativeroleTypeList.DataSource = HandleDataForGrid(urlForComplexDict);
            //setup search posibility for textBox element
            txtSearchCreativeroleType.SetUsageGrid(gridCreativeroleTypeList);
            
            //set default data for bottom panel
            if (CreativeroleTypeList.Count != 0)
                DataUpdateForBottomPanel(
                    CreativeroleTypeList[0].CodeI
                    );
            ControlEnabledActiveElements = true;
        }

        //handle data for grid
        private List<ViewTable> HandleDataForGrid(string urlForGetDataDictionaries)
        {
            //check on valid state parameter
            if (urlForGetDataDictionaries == null) return new List<ViewTable>();
            //get data dictionaries -> for grid
            CreativeroleTypeList = _connector.GetList<CreativeRoleType>(urlForGetDataDictionaries);
            //return if data is not exist
            if (CreativeroleTypeList == null) return new List<ViewTable>();
            List<ViewTable> dataGrid = new List<ViewTable>();
            //initialize grid data
            foreach (var item in CreativeroleTypeList)
            {
                dataGrid.Add(new ViewTable
                {
                    Code = item.CodeI.ToString(),
                    NameUk = item.CreativeRoleTypeTr?.SingleOrDefault(i => i?.Lang == Langs[0])?.Name ?? item.Name,
                    NameRu = item.CreativeRoleTypeTr?.SingleOrDefault(i => i?.Lang == Langs[1])?.Name,
                    NameEn = item.CreativeRoleTypeTr?.SingleOrDefault(i => i?.Lang == Langs[2])?.Name,
                    Category = ddlFilterCreativeroleType.Items.First(el => (int)el.Value == item.CategoryCode).Text,
                });
            }
            return dataGrid;
        }

        //handle data for drop down list - 'Спосіб використання'
        private List<DictBaseTypeI> InitializeDataForFilter()
        {
            //get data for drop down list
            return _connector.GetList<DictBaseTypeI>(UrlBaseDict) ?? new List<DictBaseTypeI>();
        }

        //show data for bottom panel
        private void gvCopyrightKindList_CellClick(object sender, GridViewCellEventArgs e)
        {
            if (!(gridCreativeroleTypeList.CurrentRow?.DataBoundItem is ViewTable currentRow))
                return;
            
            //check field 'Code' on correct type 'int'
            if (!int.TryParse(currentRow.Code, out int code))
                return;
            indexSelectedElem = CreativeroleTypeList.FindIndex(item => item.CodeI == code);
            DataUpdateForBottomPanel(code);
        }

        //refresh changed data for bottom panel
        private void DataUpdateForBottomPanel(int codeI)
        {
            //get object by parameter 'code' from service
            creativeroleTypeData = CheckAfterGetQuery(codeI);
            //use data for binding source
            bsCreativeRoleType.DataSource = creativeroleTypeData;
            //define binding for source of the 'Uk'
            ukCreativeroleType = creativeroleTypeData.CreativeRoleTypeTr.FirstOrDefault(rec => rec.Lang == Langs[0]);
            if (ukCreativeroleType == null)
            {
                ukCreativeroleType = new CreativeRoleTypeTr { Lang = Langs[0] };
                creativeroleTypeData.CreativeRoleTypeTr.Add(ukCreativeroleType);
            }
            bsUkLangCreativeRoleType.DataSource = ukCreativeroleType;
            //defined binding for source of the 'Ru'
            ruCreativeroleType = creativeroleTypeData.CreativeRoleTypeTr.FirstOrDefault(rec => rec.Lang == Langs[1]);
            if (ruCreativeroleType == null)
            {
                ruCreativeroleType = new CreativeRoleTypeTr { Lang = Langs[1] };
                creativeroleTypeData.CreativeRoleTypeTr.Add(ruCreativeroleType);
            }
            bsRuLangCreativeRoleType.DataSource = ruCreativeroleType;
            //define binding for source of the 'En'
            enCreativeroleType = creativeroleTypeData.CreativeRoleTypeTr.FirstOrDefault(rec => rec.Lang == Langs[2]);
            if (enCreativeroleType == null)
            {
                enCreativeroleType = new CreativeRoleTypeTr { Lang = Langs[2] };
                creativeroleTypeData.CreativeRoleTypeTr.Add(enCreativeroleType);
            }
            bsEnLangCreativeRoleType.DataSource = enCreativeroleType;
        }

        /// <summary>
        /// //check after 'Get' query
        /// </summary>
        /// <param name="codeI"></param>
        /// <returns></returns>
        private CreativeRoleType CheckAfterGetQuery(int codeI)
        {
            GetDictionaryLayer dictDelegat = new GetDictionaryLayer(_layer.GetDictionary);
            CreativeRoleType result = dictDelegat(codeI);
            if (result == null)
            {
                MessageShow.ShowError(GetMess);
                //set select first element of grid
                result = CreativeroleTypeList[0];
            }
            return result;
        }

        //select item into drop down list for filtered data into grid
        private void ddlCommonKeyData_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            //stop perform if position is not exist
            if (e.Position == -1) return;
            //get index current data
            string valueDropList = ddlFilterCreativeroleType.Items[e.Position].Value.ToString();
            if (int.TryParse(valueDropList, out int selectedItemData))
                gridCreativeroleTypeList.DataSource = FilterDataIntoGrid(selectedItemData);
            else
                return;
        }

        //filter data for grid
        private List<ViewTable> FilterDataIntoGrid(int code)
        {
            
            //get all data of the 'CopyrightKind'
            CreativeroleTypeList = _connector.GetList<CreativeRoleType>(urlDataGrid);
            //stop perform if data is not exist
            if (CreativeroleTypeList == null)
            {
                CreativeroleTypeList = new List<CreativeRoleType>();
                return new List<ViewTable>();
            }
            //filter data by parameter 'code'
            CreativeroleTypeList = CreativeroleTypeList.Where(item => item.CategoryCode == code).ToList();
            List<ViewTable> filteredData = new List<ViewTable>();
            //initialize data for grid
            foreach (var item in CreativeroleTypeList)
            {
                filteredData.Add(new ViewTable
                {
                    Code = item.CodeI.ToString(),
                    NameUk = item.CreativeRoleTypeTr?.SingleOrDefault(i => i?.Lang == Langs[0])?.Name ?? item.Name,
                    NameRu = item.CreativeRoleTypeTr?.SingleOrDefault(i => i?.Lang == Langs[1])?.Name,
                    NameEn = item.CreativeRoleTypeTr?.SingleOrDefault(i => i?.Lang == Langs[2])?.Name,
                    Category = ddlFilterCreativeroleType.SelectedItem.Text,
                });
            }
            return filteredData;
        }

        //action 'Add' new object to grid
        private void btnAddComplex_Click(object sender, EventArgs e)
        {
            //change property 'Text' of the btnSave for 'Add' action
            btnSaveCreativeroleType.Text = "Додати";
            //refresh data for field 'Code'
            creativeroleTypeData = new CreativeRoleType { Crtype = 1, CategoryCode = 1 };
            bsCreativeRoleType.DataSource = creativeroleTypeData;
            //refresh data for field 'Uk'
            ukCreativeroleType = new CreativeRoleTypeTr { Lang = Langs[0] };
            creativeroleTypeData.CreativeRoleTypeTr.Add(ukCreativeroleType);
            bsUkLangCreativeRoleType.DataSource = ukCreativeroleType;
            //refresh data for field 'Ru'
            ruCreativeroleType = new CreativeRoleTypeTr { Lang = Langs[1] };
            creativeroleTypeData.CreativeRoleTypeTr.Add(ruCreativeroleType);
            bsRuLangCreativeRoleType.DataSource = ruCreativeroleType;
            //refresh data for field 'Ru'
            enCreativeroleType = new CreativeRoleTypeTr { Lang = Langs[2] };
            creativeroleTypeData.CreativeRoleTypeTr.Add(enCreativeroleType);
            bsEnLangCreativeRoleType.DataSource = enCreativeroleType;
            //stop perform if elements have 'Enabled' -> false
            if (!ControlEnabledActiveElements) return;
            //change 'Enabled' state for elements
            ControlEnabledActiveElements = false;
        }

        //action 'Delete' exist object from grid
        private void btnDeleteComplex_Click(object sender, EventArgs e)
        {
            //return if element by index is not exist
            if (!(CreativeroleTypeList.Count > indexSelectedElem))
                return;
            //delete selected item of grid by field 'Code'
            CreativeRoleType item = CreativeroleTypeList[indexSelectedElem];
            bool result = CheckAfterDelete(item.CodeI);
            //update data for grid
            gridCreativeroleTypeList.DataSource = HandleDataForGrid(urlDataGrid);
            //step up in grid position
            if (result) --indexSelectedElem;
            if (indexSelectedElem < 0) indexSelectedElem = 0;
            gridCreativeroleTypeList.CurrentRow = gridCreativeroleTypeList.Rows[indexSelectedElem];
            //define new currentRow
            DataUpdateForBottomPanel(
                CreativeroleTypeList[indexSelectedElem].CodeI
                );
        }

        /// <summary>
        /// control delete state
        /// </summary>
        /// <param name="codeI"></param>
        /// <returns></returns>
        private bool CheckAfterDelete(int codeI)
        {
            DeleteLayer delLayer = new DeleteLayer(_layer.Delete);
            bool result = delLayer(codeI);
            if (!result)
            {
                MessageShow.ShowError(DelMess);
            }
            return result;
        }

        //action 'Save' object to grid
        private void btnSave_Click(object sender, EventArgs e)
        {
            //refresh bottom panel
            int codeIAfter = 0;
            if (btnSaveCreativeroleType.Text == "Додати")
            {
                //send created object
                CreativeRoleType result = CheckAfterCreate(creativeroleTypeData);
                codeIAfter = result.CodeI;
                //update data for grid
                gridCreativeroleTypeList.DataSource = HandleDataForGrid(urlDataGrid);
                indexSelectedElem = CreativeroleTypeList.FindIndex(el => el.CodeI == result.CodeI);
                //select certain item into grid
                gridCreativeroleTypeList.CurrentRow = gridCreativeroleTypeList.Rows[indexSelectedElem];
                //set default state for 'Text' of the btnSave
                btnSaveCreativeroleType.Text = "Зберегти";
            } else if (btnSaveCreativeroleType.Text == "Зберегти")
            {
                //save changes
                CreativeRoleType result = CheckAfterUpdate(creativeroleTypeData);
                codeIAfter = result.CodeI;
                //save last selected item
                indexSelectedElem = gridCreativeroleTypeList.CurrentRow.Index;
                //update data for grid
                gridCreativeroleTypeList.DataSource = HandleDataForGrid(urlDataGrid);
                //select certain item into grid
                gridCreativeroleTypeList.CurrentRow = gridCreativeroleTypeList.Rows[indexSelectedElem];
            }
            DataUpdateForBottomPanel(codeIAfter);
            //stop perform if elements have 'Enabled' -> true
            if (ControlEnabledActiveElements) return;
            //change 'Enabled' state for elements
            ControlEnabledActiveElements = true;
        }

        /// <summary>
        /// check result data after 'Create'
        /// </summary>
        /// <param name="createItem"></param>
        /// <returns></returns>
        private CreativeRoleType CheckAfterCreate(CreativeRoleType createItem)
        {
            CreateLayer createLayer = new CreateLayer(_layer.Create);
            CreativeRoleType result = createLayer(createItem);
            if (result == null)
            {
                MessageShow.ShowError(CreateMess);
                //select first element of grid by default
                result = CreativeroleTypeList[0];
            }
            return result;
        }

        /// <summary>
        /// check result data after 'Update'
        /// </summary>
        /// <param name="updateItem"></param>
        /// <returns></returns>
        private CreativeRoleType CheckAfterUpdate(CreativeRoleType updateItem)
        {
            //DictionaryService dictService = new DictionaryService();
            UpdateLayer updateLayer = new UpdateLayer(_layer.Update);
            CreativeRoleType result = updateLayer(creativeroleTypeData);
            if (result == null)
            {
                MessageShow.ShowError(UpdateMess);
                //select first element of grid by default
                result = CreativeroleTypeList[0];
            }
            return result;
        }

        //action 'Cancel' reset changed data for selected item
        private void btnCancel_Click(object sender, EventArgs e)
        {
            int codeICurrent = CreativeroleTypeList[indexSelectedElem].CodeI;
            //reset changed data
            DataUpdateForBottomPanel(codeICurrent);
            //stop perform if elements have 'Enabled' -> true
            if (ControlEnabledActiveElements) return;
            //change 'Enabled' state for elements
            ControlEnabledActiveElements = true;
        }

        /// <summary>
        /// changed data of the field 'Uk'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtUkLangCreativeroleType_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnSaveCreativeroleType.Text != "Додати")
            {
                //change property 'Text' of the btnSave for 'Save' action
                btnSaveCreativeroleType.Text = "Зберегти";
            }
            //stop perform if elements have 'Enabled' -> false
            if (!ControlEnabledActiveElements) return;
            //change 'Enabled' state for elements
            ControlEnabledActiveElements = false;
        }

        /// <summary>
        /// changed data of the field 'Ru'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtRuLangCreativeroleType_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnSaveCreativeroleType.Text != "Додати")
            {
                //change property 'Text' of the btnSave for 'Save' action
                btnSaveCreativeroleType.Text = "Зберегти";
            }
            //stop perform if elements have 'Enabled' -> false
            if (!ControlEnabledActiveElements) return;
            //change 'Enabled' state for elements
            ControlEnabledActiveElements = false;
        }

        /// <summary>
        /// changed data of the field 'En'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtEnLangCreativeroleType_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnSaveCreativeroleType.Text != "Додати")
            {
                //change property 'Text' of the btnSave for 'Save' action
                btnSaveCreativeroleType.Text = "Зберегти";
            }
            //stop perform if elements have 'Enabled' -> false
            if (!ControlEnabledActiveElements) return;
            //change 'Enabled' state for elements
            ControlEnabledActiveElements = false;
        }

        /// <summary>
        /// define hide elements for dropDownList
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ddlEditKeyCreativeroleType_Click(object sender, EventArgs e)
        {
            if (btnSaveCreativeroleType.Text != "Додати")
            {
                //change property 'Text' of the btnSave for 'Save' action
                btnSaveCreativeroleType.Text = "Зберегти";
            }
            //stop perform if elements have 'Enabled' -> false
            if (!ControlEnabledActiveElements) return;
            //change 'Enabled' state for elements
            ControlEnabledActiveElements = false;
        }
        
        /// <summary>
        /// allow input only number data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCodeCreativeroleType_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if it 'number' or 'Backspace' -> allow input
            if (char.IsDigit(e.KeyChar) || e.KeyChar == (char)Keys.Back)
            {
                if (btnSaveCreativeroleType.Text != "Додати")
                {
                    //change property 'Text' of the btnSave for 'Save' action
                    btnSaveCreativeroleType.Text = "Зберегти";
                }
                //stop perform if elements have 'Enabled' -> false
                if (!ControlEnabledActiveElements) return;
                //change 'Enabled' state for elements
                ControlEnabledActiveElements = false;
                return;
            }
            else
                //disallow input
                e.Handled = true;
        }
    }
}
