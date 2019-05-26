using ipsFormsDesign.Model;
using ipsFormsDesign.Services;
using sysAdminwp.ComplexDictionariesBaseControls;
using System.Windows.Forms;
using WebAPIConnector;

namespace sysAdminwp.PresenterComplexDictionaries.DefineComplexLayers
{
    /// <summary>
    /// Layer - define behaviour for BaseTypePlaceControl
    /// </summary>
    class TypePlaceLayer : IDictionaryActions<TypePlace>
    {
        private readonly DictionaryService _dictService = DictionaryService.GetInstance();

        //initialize control
        public UserControl InitControl(ApiConnector connector, string url)
        {
            return new BaseTypePlaceControl(connector, url);
        }

        //get dictionary by id
        public TypePlace GetDictionary(int id)
        {
            return _dictService.GetTypePlace(id);
        }

        //create new dictionary
        public TypePlace Create(TypePlace sors)
        {
            return _dictService.CreateTypePlace(sors);
        }

        //update current dictionary
        public TypePlace Update(TypePlace sors)
        {
            return _dictService.UpdateTypePlace(sors);
        }

        //delete dictionary by id
        public bool Delete(int id)
        {
            return _dictService.DeleteTypePlace(id);
        }
    }
}
