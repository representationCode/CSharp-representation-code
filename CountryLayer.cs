using ipsFormsDesign.Model;
using ipsFormsDesign.Services;
using sysAdminwp.ComplexDictionariesBaseControls;
using System.Windows.Forms;
using WebAPIConnector;

namespace sysAdminwp.PresenterComplexDictionaries.DefineComplexLayers
{
    /// <summary>
    /// Layer - define behaviour for BaseCountryControl
    /// </summary>
    class CountryLayer : IDictionaryActions<Country>
    {
        private readonly DictionaryService _dictService = DictionaryService.GetInstance();

        //initialize control
        public UserControl InitControl(ApiConnector connector, string url)
        {
            return new BaseCountryControl(connector, url);
        }

        //get dictionary by id
        public Country GetDictionary(int id)
        {
            return _dictService.GetCountry(id);
        }

        //create new dictionary
        public Country Create(Country sors)
        {
            return _dictService.CreateCountry(sors);
        }

        //update current dictionary
        public Country Update(Country sors)
        {
            return _dictService.UpdateCountry(sors);
        }

        //delete dictionary
        public bool Delete(int id)
        {
            return _dictService.DeleteCountry(id);
        }
    }
}
