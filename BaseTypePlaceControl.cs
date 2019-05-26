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

namespace sysAdminwp.ComplexDictionariesBaseControls
{
    public partial class BaseTypePlaceControl : UserControl
    {
        TypePlaceLayer _layer = new TypePlaceLayer();
        public delegate TypePlace GetDictionaryLayer(int id);
        public delegate TypePlace CreateLayer(TypePlace sors);
        public delegate TypePlace UpdateLayer(TypePlace sors);
        public delegate bool DeleteLayer(int id);
        private readonly List<string> Langs = new List<string>
        {
            "Uk", "Ru", "En"
        };

        private const string DelMess = "Об'єкт не видалений!";
        private const string GetMess = "об'єкт не знайдено";
        private const string CreateMess = "Помилка формату";
        private const string UpdateMess = "Помилка формату";
        private const string UrlBaseDict = "dictionary/base/typePlaceParam";

        private readonly DictionaryService _dictionaryService = DictionaryService.GetInstance();
        private readonly ApiConnector _connector;

        //define data for different languages
        private TypePlaceTr ukTypePlace;
        private TypePlaceTr ruTypePlace;
        private TypePlaceTr enTypePlace;

        //define grid data
        private List<TypePlace> _typePlaceList;
        public List<TypePlace> TypePlaceList
        {
            get { return _typePlaceList; }
            set
            {
                if (value != null)
                {
                    _typePlaceList = value;
                    totalTypePlace.Text = _typePlaceList.Count.ToString();
                }
            }
        }
        //item data
        private TypePlace typePlaceData;
        //url for get data to grid
        private readonly string urlDataGrid;
        //for control current select of grid
        private int indexSelectedElem;
        //control 'Enabled' state for elements
        private bool controlEnabledActiveElements = true;
        public bool ControlEnabledActiveElements
        {
            get { return controlEnabledActiveElements; }
            set
            {
                controlEnabledActiveElements = value;
                //'search' element
                txtSearchTypePlace.Enabled = value;
                //filter for grid
                ddlFilterTypePlace.Enabled = value;
                //grid element
                gridTypePlaceList.Enabled = value;
                //'Add' button
                btnAddTypePlace.Enabled = value;
                //'Delete' button
                btnDeleteTypePlace.Enabled = value;
                //'Save' button (bottom panel)
                btnSaveTypePlace.Enabled = !value;
                //'Cancel' button (bottom panel)
                btnCancelTypePlace.Enabled = !value;
            }
        }
        public BaseTypePlaceControl(ApiConnector connector, string urlForComplexDict)
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            _connector = connector;
            urlDataGrid = urlForComplexDict;
            //initialize data for drop down lists
            ddlFilterTypePlace.DataSource = bsSimpleDictTypePlace.DataSource = InitializeDataForFilter();
            //initialize data for grid
            gridTypePlaceList.DataSource = HandleDataForGrid(urlForComplexDict);
            //setup search posibility for textBox element
            txtSearchTypePlace.SetUsageGrid(gridTypePlaceList);
            
            //set default data for bottom panel
            if (TypePlaceList.Count != 0)
                DataUpdateForBottomPanel(
                    TypePlaceList[0].CodeI
                    );
            ControlEnabledActiveElements = true;
        }

        //handle data for grid
        private List<ViewTable> HandleDataForGrid(string urlForGetDataDictionaries)
        {
            //check on valid state parameter
            if (urlForGetDataDictionaries == null) return new List<ViewTable>();
            //get data dictionaries -> for grid
            TypePlaceList = _connector.GetList<TypePlace>(urlForGetDataDictionaries);
            //data is not exist
            if (TypePlaceList == null)
            {
                TypePlaceList = new List<TypePlace>();
                return new List<ViewTable>();
            }
            List<ViewTable> dataGrid = new List<ViewTable>();
            //initialize grid data
            foreach (var item in TypePlaceList)
            {
                dataGrid.Add(new ViewTable
                {
                    Code = item.CodeI.ToString(),
                    NameUk = item.TypePlaceTr?.SingleOrDefault(i => i?.Lang.TrimEnd() == Langs[0])?.Name ?? item.Name,
                    NameRu = item.TypePlaceTr?.SingleOrDefault(i => i?.Lang.TrimEnd() == Langs[1])?.Name,
                    NameEn = item.TypePlaceTr?.SingleOrDefault(i => i?.Lang.TrimEnd() == Langs[2])?.Name,
                    Object = item.ParamType != null ? ddlFilterTypePlace.Items.First(el => (int)el.Value == item.ParamType).Text : "",
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
            if (!(gridTypePlaceList.CurrentRow.DataBoundItem is ViewTable currentRow))
                return;

            if (!int.TryParse(currentRow.Code, out int code))
                return;
            indexSelectedElem = TypePlaceList.FindIndex(el => el.CodeI == code);
            DataUpdateForBottomPanel(code);
        }

        //refresh changed data for bottom panel
        private void DataUpdateForBottomPanel(int codeI)
        {
            //get object by parameter 'code' from service
            typePlaceData = CheckAfterGetQuery(codeI);
            //use data for binding source
            bsTypePlace.DataSource = typePlaceData;
            //define binding for source of the 'Uk'
            ukTypePlace = typePlaceData.TypePlaceTr.FirstOrDefault(rec => rec.Lang.TrimEnd() == Langs[0]);
            if (ukTypePlace == null)
            {
                ukTypePlace = new TypePlaceTr { Lang = Langs[0] };
                typePlaceData.TypePlaceTr.Add(ukTypePlace);
            }
            bsUkLangTypePlace.DataSource = ukTypePlace;
            //defined binding for source of the 'Ru'
            ruTypePlace = typePlaceData.TypePlaceTr.FirstOrDefault(rec => rec.Lang.TrimEnd() == Langs[1]);
            if (ruTypePlace == null)
            {
                ruTypePlace = new TypePlaceTr { Lang = Langs[1] };
                typePlaceData.TypePlaceTr.Add(ruTypePlace);
            }
            bsRuLangTypePlace.DataSource = ruTypePlace;
            //define binding for source of the 'En'
            enTypePlace = typePlaceData.TypePlaceTr.FirstOrDefault(rec => rec.Lang.TrimEnd() == Langs[2]);
            if (enTypePlace == null)
            {
                enTypePlace = new TypePlaceTr { Lang = Langs[2] };
                typePlaceData.TypePlaceTr.Add(enTypePlace);
            }
            bsEnLangTypePlace.DataSource = enTypePlace;
        }

        private TypePlace CheckAfterGetQuery(int codeI)
        {
            GetDictionaryLayer dictDelegat = new GetDictionaryLayer(_layer.GetDictionary);
            TypePlace result = dictDelegat(codeI);
            if (result == null)
            {
                MessageShow.ShowError(GetMess);
                //set select first element of grid
                result = TypePlaceList[0];
            }
            return result;
        }

        //select item into drop down list for filtered data into grid
        private void ddlCommonKeyData_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            //stop perform if position is not exist
            if (e.Position == -1) return;
            //get index current data
            string valueDropList = ddlFilterTypePlace.Items[e.Position].Value.ToString();
            if (int.TryParse(valueDropList, out int selectedItemData))
                gridTypePlaceList.DataSource = FilterDataIntoGrid(selectedItemData);
            else
                return;
        }

        //filter data for grid
        private List<ViewTable> FilterDataIntoGrid(int code)
        {
            //get all data of the 'CopyrightKind'
            TypePlaceList = _connector.GetList<TypePlace>(urlDataGrid);
            //stop perform if data is not exist
            if (TypePlaceList == null) return new List<ViewTable>();
            //filter data by parameter 'code'
            TypePlaceList = TypePlaceList.Where(item => item.ParamType == code).ToList();
            List<ViewTable> filteredData = new List<ViewTable>();
            //initialize data for grid
            foreach (var item in _typePlaceList)
            {
                filteredData.Add(new ViewTable
                {
                    Code = item.CodeI.ToString(),
                    NameUk = item.TypePlaceTr?.SingleOrDefault(i => i?.Lang.TrimEnd() == Langs[0])?.Name ?? item.Name,
                    NameRu = item.TypePlaceTr?.SingleOrDefault(i => i?.Lang.TrimEnd() == Langs[1])?.Name,
                    NameEn = item.TypePlaceTr?.SingleOrDefault(i => i?.Lang.TrimEnd() == Langs[2])?.Name,
                    Object = ddlFilterTypePlace.SelectedItem.Text,
                });
            }
            return filteredData;
        }

        //action 'Add' new object to grid
        private void btnAddComplex_Click(object sender, EventArgs e)
        {
            //change property 'Text' of the btnSave for 'Add' action
            btnSaveTypePlace.Text = "Додати";
            //refresh data for field 'Code'
            typePlaceData = new TypePlace { ParamType = 1 };
            bsTypePlace.DataSource = typePlaceData;
            //refresh data for field 'Uk'
            ukTypePlace = new TypePlaceTr { Lang = Langs[0] };
            typePlaceData.TypePlaceTr.Add(ukTypePlace);
            bsUkLangTypePlace.DataSource = ukTypePlace;
            //refresh data for field 'Ru'
            ruTypePlace = new TypePlaceTr { Lang = Langs[1] };
            typePlaceData.TypePlaceTr.Add(ruTypePlace);
            bsRuLangTypePlace.DataSource = ruTypePlace;
            //refresh data for field 'Ru'
            enTypePlace = new TypePlaceTr { Lang = Langs[2] };
            typePlaceData.TypePlaceTr.Add(enTypePlace);
            bsEnLangTypePlace.DataSource = enTypePlace;
            //stop perform if elements have 'Enabled' -> false
            if (!ControlEnabledActiveElements) return;
            //change 'Enabled' state for elements
            ControlEnabledActiveElements = false;
        }

        //action 'Delete' exist object from grid
        private void btnDeleteComplex_Click(object sender, EventArgs e)
        {
            //return if element by index is not exist
            if (!(TypePlaceList.Count > indexSelectedElem))
                return;
            //delete selected item of grid by field 'Code'
            TypePlace item = TypePlaceList[indexSelectedElem];
            bool result = CheckAfterDelete(item.CodeI);
            //update data for grid
            gridTypePlaceList.DataSource = HandleDataForGrid(urlDataGrid);
            //step up in grid position
            if (result) --indexSelectedElem;
            if (indexSelectedElem < 0) indexSelectedElem = 0;
            gridTypePlaceList.CurrentRow = gridTypePlaceList.Rows[indexSelectedElem];
            //define new currentRow
            DataUpdateForBottomPanel(
                TypePlaceList[indexSelectedElem].CodeI
                );
        }

        /// <summary>
        /// control delete state
        /// </summary>
        /// <param name="codeI"></param>
        /// <returns></returns>
        private bool CheckAfterDelete(int codeI)
        {
            DeleteLayer dictDelegat = new DeleteLayer(_layer.Delete);
            bool result = dictDelegat(codeI);
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
            if (btnSaveTypePlace.Text == "Додати")
            {
                //send created object
                TypePlace result = CheckAfterCreate(typePlaceData);
                codeIAfter = result.CodeI;
                //update data for grid
                gridTypePlaceList.DataSource = HandleDataForGrid(urlDataGrid);
                indexSelectedElem = _typePlaceList.FindIndex(el => el.CodeI == result.CodeI);
                //select certain item into grid
                gridTypePlaceList.CurrentRow = gridTypePlaceList.Rows[indexSelectedElem];
                //set default state for 'Text' of the btnSave
                btnSaveTypePlace.Text = "Зберегти";
            } else if (btnSaveTypePlace.Text == "Зберегти")
            {
                //save changes
                TypePlace result = CheckAfterUpdate(typePlaceData);
                codeIAfter = result.CodeI;
                //save last selected item
                indexSelectedElem = gridTypePlaceList.CurrentRow.Index;
                //update data for grid
                gridTypePlaceList.DataSource = HandleDataForGrid(urlDataGrid);
                //select certain item into grid
                gridTypePlaceList.CurrentRow = gridTypePlaceList.Rows[indexSelectedElem];
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
        private TypePlace CheckAfterCreate(TypePlace createItem)
        {
            CreateLayer dictDelegat = new CreateLayer(_layer.Create);
            TypePlace result = dictDelegat(createItem);
            if (result == null)
            {
                MessageShow.ShowError(CreateMess);
                //select first element of grid by default
                result = TypePlaceList[0];
            }
            return result;
        }

        /// <summary>
        /// check result data after 'Update'
        /// </summary>
        /// <param name="updateItem"></param>
        /// <returns></returns>
        private TypePlace CheckAfterUpdate(TypePlace updateItem)
        {
            UpdateLayer dictDelegat = new UpdateLayer(_layer.Update);
            TypePlace result = dictDelegat(typePlaceData);
            if (result == null)
            {
                MessageShow.ShowError(UpdateMess);
                //select first element of grid by default
                result = TypePlaceList[0];
            }
            return result;
        }

        //action 'Cancel' reset changed data for selected item
        private void btnCancel_Click(object sender, EventArgs e)
        {
            int codeICurrent = TypePlaceList[indexSelectedElem].CodeI;
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
        private void txtUkLangTypePlace_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnSaveTypePlace.Text != "Додати")
            {
                //change property 'Text' of the btnSave for 'Save' action
                btnSaveTypePlace.Text = "Зберегти";
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
        private void txtRuLangTypePlace_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnSaveTypePlace.Text != "Додати")
            {
                //change property 'Text' of the btnSave for 'Save' action
                btnSaveTypePlace.Text = "Зберегти";
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
        private void txtEnLangTypePlace_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnSaveTypePlace.Text != "Додати")
            {
                //change property 'Text' of the btnSave for 'Save' action
                btnSaveTypePlace.Text = "Зберегти";
            }
            //stop perform if elements have 'Enabled' -> false
            if (!ControlEnabledActiveElements) return;
            //change 'Enabled' state for elements
            ControlEnabledActiveElements = false;
        }

        /// <summary>
        /// event for field 'Code' and for dropDownList (bottom panel)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ddlEditKeyTypePlace_Click(object sender, EventArgs e)
        {
            if (btnSaveTypePlace.Text != "Додати")
            {
                //change property 'Text' of the btnSave for 'Save' action
                btnSaveTypePlace.Text = "Зберегти";
            }
            //stop perform if elements have 'Enabled' -> false
            if (!ControlEnabledActiveElements) return;
            //change 'Enabled' state for elements
            ControlEnabledActiveElements = false;
        }

        //allow input only number data
        private void txtCodeTypePlace_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if it 'number' or 'Backspace' -> allow input
            if (char.IsDigit(e.KeyChar) || e.KeyChar == (char)Keys.Back)
            {
                if (btnSaveTypePlace.Text != "Додати")
                {
                    //change property 'Text' of the btnSave for 'Save' action
                    btnSaveTypePlace.Text = "Зберегти";
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

        private void checkBox_isHoll_Click(object sender, EventArgs e)
        {
            if (btnSaveTypePlace.Text != "Додати")
            {
                //change property 'Text' of the btnSave for 'Save' action
                btnSaveTypePlace.Text = "Зберегти";
            }
            //stop perform if elements have 'Enabled' -> false
            if (!ControlEnabledActiveElements) return;
            //change 'Enabled' state for elements
            ControlEnabledActiveElements = false;
        }
    }
}
