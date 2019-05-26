using ipsFormsDesign.Model;
using ipsFormsDesign.Services;
using sysAdminwp.ComplexDictionariesBaseControls;
using System.Windows.Forms;
using WebAPIConnector;

namespace sysAdminwp.PresenterComplexDictionaries.DefineComplexLayers
{
    /// <summary>
    /// Layer - define behaviour for BaseGenreTypeControl
    /// </summary>
    class GenreTypeLayer : IDictionaryActions<GenreType>
    {
        private readonly DictionaryService _dictService = DictionaryService.GetInstance();

        //initialize control
        public UserControl InitControl(ApiConnector connector, string url)
        {
            return new BaseGenreTypeControl(connector, url);
        }

        //get dictionary by id
        public GenreType GetDictionary(int id)
        {
            return _dictService.GetGenreType(id);
        }

        //create new dictionary
        public GenreType Create(GenreType sors)
        {
            return _dictService.CreateGenreType(sors);
        }

        //update current dictionary
        public GenreType Update(GenreType sors)
        {
            return _dictService.UpdateGenreType(sors);
        }

        //delete dictionary by id
        public bool Delete(int id)
        {
            return _dictService.DeleteGenreType(id);
        }
    }
}
