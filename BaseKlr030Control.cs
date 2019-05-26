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
using System.Collections;

namespace sysAdminwp.ComplexDictionariesBaseControls
{
    public partial class BaseKlr030Control : UserControl
    {
        Klr030Layer _layer = new Klr030Layer();
        public delegate KlR030 GetDictionaryLayer(int id);
        public delegate KlR030 CreateLayer(KlR030 sors);
        public delegate KlR030 UpdateLayer(KlR030 sors);
        public delegate bool DeleteLayer(int id);
        private readonly List<string> Langs = new List<string>
        {
            "Uk", "Ru", "En"
        };

        private const string DelMess = "Об'єкт не видалений!";
        private const string GetMess = "об'єкт не знайдено";
        private const string CreateMess = "Помилка формату";
        private const string UpdateMess = "Помилка формату";
        private const string UrlBaseDict = "dictionary/base/Country";

        private readonly DictionaryService _dictService = DictionaryService.GetInstance();
        private readonly ApiConnector _connector;

        //define data for different languages
        private KlR030tr ukKlr030;
        private KlR030tr ruKlr030;
        private KlR030tr enKlr030;
        //define grid data
        private List<KlR030> _klr030List;
        public List<KlR030> Klr030List
        {
            get { return _klr030List; }
            set
            {
                if (value != null)
                {
                    _klr030List = value;
                    totalKlr030.Text = _klr030List.Count.ToString();
                }
            }
        }
        //item data
        private KlR030 klr030Data;
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
                txtSearchKlr030.Enabled = value;
                //filter for grid
                ddlFilterKlr030.Enabled = value;
                //grid element
                gridKlr030List.Enabled = value;
                //'Add' button
                btnAddKlr030.Enabled = value;
                //'Delete' button
                btnDeleteKlr030.Enabled = value;
                //'Save' button (bottom panel)
                btnSaveKlr030.Enabled = !value;
                //'Cancel' button (bottom panel)
                btnCancelKlr030.Enabled = !value;
            }
        }
        public BaseKlr030Control(ApiConnector connector, string urlForComplexDict)
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            txtSearchKlr030.SetUsageGrid(gridKlr030List);

            _connector = connector;
            urlDataGrid = urlForComplexDict;
            //initialize data for drop down lists
            countryBindingSource.DataSource = bsSimpleDictKlr030.DataSource = InitializeDataForFilter();
            //initialize data for grid
            gridKlr030List.DataSource = HandleDataForGrid(urlForComplexDict);
            //setup search posibility for textBox element

            //set default data for bottom panel
            if (Klr030List.Count != 0)
                DataUpdateForBottomPanel(
                    Klr030List[0].R030
                    );
            ControlEnabledActiveElements = true;
        }

        //handle data for grid
        private List<ViewTable> HandleDataForGrid(string urlForGetDataDictionaries)
        {
            //check on valid state parameter
            if (urlForGetDataDictionaries == null) return new List<ViewTable>();
            //get data dictionaries -> for grid
            Klr030List = _connector.GetList<KlR030>(urlForGetDataDictionaries);
            //return data is not exist
            if (Klr030List == null)
            {
                Klr030List = new List<KlR030>();
                return new List<ViewTable>();
            }
            List<ViewTable> dataGrid = new List<ViewTable>();
            List<Country> list = (List<Country>)countryBindingSource.List;
            //initialize grid data
            foreach (var item in Klr030List)
            {
                dataGrid.Add(new ViewTable
                {
                    Code = item.R030,
                    NameUk = item.KlR030tr?.SingleOrDefault(i => i?.Lang == Langs[0])?.R030Name ?? item.R030Name,
                    NameRu = item.KlR030tr?.SingleOrDefault(i => i?.Lang == Langs[1])?.R030Name,
                    NameEn = item.KlR030tr?.SingleOrDefault(i => i?.Lang == Langs[2])?.R030Name,
                    klrCountry = item.CountryCode != null ? list.Find(el => el.CountryCode == item.CountryCode).CountryName : "",
                });
            }
            return dataGrid;
        }

        //handle data for drop down list - 'Спосіб використання'
        private List<Country> InitializeDataForFilter()
        {
            //get data for drop down list
            return _connector.GetList<Country>(UrlBaseDict) ?? new List<Country>();
        }

        //show data for bottom panel
        private void gvCopyrightKindList_CellClick(object sender, GridViewCellEventArgs e)
        {
            if (!(gridKlr030List.CurrentRow.DataBoundItem is ViewTable currentRow))
                return;

            if (!int.TryParse(currentRow.Code, out int code))
                return;
            string strCode = code.ToString();
            indexSelectedElem = Klr030List.FindIndex(el => el.R030 == strCode);
            DataUpdateForBottomPanel(strCode);
        }

        //refresh changed data for bottom panel
        private void DataUpdateForBottomPanel(string codeI)
        {
            //get object by parameter 'code' from service
            klr030Data = CheckAfterGetQuery(codeI);
            //use data for binding source
            bsKlr030.DataSource = klr030Data;
            //define binding for source of the 'Uk'
            ukKlr030 = klr030Data.KlR030tr.FirstOrDefault(rec => rec.Lang == Langs[0]);
            if (ukKlr030 == null)
            {
                ukKlr030 = new KlR030tr { Lang = Langs[0] };
                klr030Data.KlR030tr.Add(ukKlr030);
            }
            bsUkLangKlr030.DataSource = ukKlr030;
            //defined binding for source of the 'Ru'
            ruKlr030 = klr030Data.KlR030tr.FirstOrDefault(rec => rec.Lang == Langs[1]);
            if (ruKlr030 == null)
            {
                ruKlr030 = new KlR030tr { Lang = Langs[1] };
                klr030Data.KlR030tr.Add(ruKlr030);
            }
            bsRuLangKlr030.DataSource = ruKlr030;
            //define binding for source of the 'En'
            enKlr030 = klr030Data.KlR030tr.FirstOrDefault(rec => rec.Lang == Langs[2]);
            if (enKlr030 == null)
            {
                enKlr030 = new KlR030tr { Lang = Langs[2] };
                klr030Data.KlR030tr.Add(enKlr030);
            }
            bsEnLangKlr030.DataSource = enKlr030;
        }

        /// <summary>
        /// //check after 'Get' query
        /// </summary>
        /// <param name="codeI"></param>
        /// <returns></returns>
        private KlR030 CheckAfterGetQuery(string code)
        {
            GetDictionaryLayer dictDelegat = new GetDictionaryLayer(_layer.GetDictionary);
            KlR030 result = new KlR030();
            if (int.TryParse(code, out int codeInt))
                result = dictDelegat(codeInt);
            else
                result = null;
            if (result == null)
            {
                MessageShow.ShowError(GetMess);
                //set select first element of grid
                result = Klr030List[0];
            }
            return result;
        }



        //select item into drop down list for filtered data into grid
        private void ddlCommonKeyData_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {//stop perform if position is not exist
            if (e.Position == -1) return;
            //get index current data
            string valueDropList = ddlFilterKlr030.Items[e.Position].Value.ToString();
            if (int.TryParse(valueDropList, out int selectedItemData))
                gridKlr030List.DataSource = FilterDataIntoGrid(selectedItemData);
            else
                return;
        }

        //filter data for grid
        private List<ViewTable> FilterDataIntoGrid(int code)
        {
            //get all data of the 'CopyrightKind'
            Klr030List = _connector.GetList<KlR030>(urlDataGrid);
            //stop perform if data is not exist
            if (Klr030List == null) return new List<ViewTable>();
            //filter data by parameter 'code'
            Klr030List = Klr030List.Where(item => item.R031 == code).ToList();
            List<ViewTable> filteredData = new List<ViewTable>();
            //initialize data for grid
            foreach (var item in Klr030List)
            {
                filteredData.Add(new ViewTable
                {
                    Code = item.R030,
                    NameUk = item.KlR030tr?.SingleOrDefault(i => i?.Lang == Langs[0])?.R030Name ?? item.R030Name,
                    NameRu = item.KlR030tr?.SingleOrDefault(i => i?.Lang == Langs[1])?.R030Name,
                    NameEn = item.KlR030tr?.SingleOrDefault(i => i?.Lang == Langs[2])?.R030Name,
                    klrCountry = ddlFilterKlr030.SelectedItem.Text,
                });
            }
            return filteredData;
        }

        //action 'Add' new object to grid
        private void btnAddComplex_Click(object sender, EventArgs e)
        {
            //change property 'Text' of the btnSave for 'Add' action
            btnSaveKlr030.Text = "Додати";
            //refresh data for field 'Code'
            klr030Data = new KlR030 { R031 = 1, CountryCode = ddlFilterKlr030.Items.First.Value.ToString() };
            bsKlr030.DataSource = klr030Data;
            //refresh data for field 'Uk'
            ukKlr030 = new KlR030tr { Lang = Langs[0] };
            klr030Data.KlR030tr.Add(ukKlr030);
            bsUkLangKlr030.DataSource = ukKlr030;
            //refresh data for field 'Ru'
            ruKlr030 = new KlR030tr { Lang = Langs[1] };
            klr030Data.KlR030tr.Add(ruKlr030);
            bsRuLangKlr030.DataSource = ruKlr030;
            //refresh data for field 'Ru'
            enKlr030 = new KlR030tr { Lang = Langs[2] };
            klr030Data.KlR030tr.Add(enKlr030);
            bsEnLangKlr030.DataSource = enKlr030;
            //stop perform if elements have 'Enabled' -> false
            if (!ControlEnabledActiveElements) return;
            //change 'Enabled' state for elements
            ControlEnabledActiveElements = false;
        }

        //action 'Delete' exist object from grid
        private void btnDeleteComplex_Click(object sender, EventArgs e)
        {
            //return if element by index is not exist
            if (!(Klr030List.Count > indexSelectedElem))
                return;
            //delete selected item of grid by field 'Code'
            KlR030 item = Klr030List[indexSelectedElem];
            bool result = CheckAfterDelete(Convert.ToInt32(item.R030));
            //update data for grid
            gridKlr030List.DataSource = HandleDataForGrid(urlDataGrid);
            //step up in grid position
            if (result) --indexSelectedElem;
            if (indexSelectedElem < 0) indexSelectedElem = 0;
            gridKlr030List.CurrentRow = gridKlr030List.Rows[indexSelectedElem];
            //define new currentRow
            DataUpdateForBottomPanel(
                Klr030List[indexSelectedElem].R030
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
            string codeIAfter = string.Empty;
            if (btnSaveKlr030.Text == "Додати")
            {
                //send created object
                KlR030 result = CheckAfterCreate(klr030Data);
                codeIAfter = result.R030;
                //update data for grid
                gridKlr030List.DataSource = HandleDataForGrid(urlDataGrid);
                indexSelectedElem = Klr030List.FindIndex(el => el.R031 == result.R031);
                //select certain item into grid
                gridKlr030List.CurrentRow = gridKlr030List.Rows[indexSelectedElem];
                btnSaveKlr030.Text = "Зберегти";
            } else if (btnSaveKlr030.Text == "Зберегти")
            {
                //save changes
                KlR030 result = CheckAfterUpdate(klr030Data);
                codeIAfter = result.R030;
                //save last selected item
                indexSelectedElem = gridKlr030List.CurrentRow.Index;
                //update data for grid
                gridKlr030List.DataSource = HandleDataForGrid(urlDataGrid);
                //select certain item into grid
                gridKlr030List.CurrentRow = gridKlr030List.Rows[indexSelectedElem];
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
        private KlR030 CheckAfterCreate(KlR030 createItem)
        {
            CreateLayer dictDelegat = new CreateLayer(_layer.Create);
            KlR030 result = dictDelegat(createItem);
            if (result == null)
            {
                MessageShow.ShowError(CreateMess);
                //select first element of grid by default
                result = Klr030List[0];
            }
            return result;
        }

        /// <summary>
        /// check result data after 'Update'
        /// </summary>
        /// <param name="updateItem"></param>
        /// <returns></returns>
        private KlR030 CheckAfterUpdate(KlR030 updateItem)
        {
            UpdateLayer dictDelegat = new UpdateLayer(_layer.Update);
            KlR030 result = dictDelegat(klr030Data);
            if (result == null)
            {
                MessageShow.ShowError(UpdateMess);
                //select first element of grid by default
                result = Klr030List[0];
            }
            return result;
        }

        //action 'Cancel' reset changed data for selected item
        private void btnCancel_Click(object sender, EventArgs e)
        {
            //reset changed data
            DataUpdateForBottomPanel(
                Klr030List[indexSelectedElem].R030
                );
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
        private void txtUkLangKlr030_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnSaveKlr030.Text != "Додати")
            {
                //change property 'Text' of the btnSave for 'Save' action
                btnSaveKlr030.Text = "Зберегти";
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
        private void txtRuLangKlr030_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnSaveKlr030.Text != "Додати")
            {
                //change property 'Text' of the btnSave for 'Save' action
                btnSaveKlr030.Text = "Зберегти";
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
        private void txtEnLangKlr030_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnSaveKlr030.Text != "Додати")
            {
                //change property 'Text' of the btnSave for 'Save' action
                btnSaveKlr030.Text = "Зберегти";
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
        private void ddlEditKeyKlr030_Click(object sender, EventArgs e)
        {
            if (btnSaveKlr030.Text != "Додати")
            {
                //change property 'Text' of the btnSave for 'Save' action
                btnSaveKlr030.Text = "Зберегти";
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
        private void txtCodeKlr030_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if it 'number' or 'Backspace' -> allow input
            if (char.IsDigit(e.KeyChar) || e.KeyChar == (char)Keys.Back)
            {
                if (btnSaveKlr030.Text != "Додати")
                {
                    //change property 'Text' of the btnSave for 'Save' action
                    btnSaveKlr030.Text = "Зберегти";
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
