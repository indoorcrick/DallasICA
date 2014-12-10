using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DallasICA.ViewModel
{
    public class PlayerViewModel
    {

        private IEnumerable<DallasICA.PLAYER_PACKAGE> _playerPackages;
        public IEnumerable<DallasICA.PLAYER_PACKAGE> PlayerPackages
        {
            get;
            set;
            //get
            //{
            //    var selectedItem = _playerPackages.Where(x =>x.p)
            //    return allFlavors;
            //}
            //set
            //{
            //    name = value;
            //}
        }

        public IEnumerable<DallasICA.PLAYER> Players { get; set; }

        public IEnumerable<DallasICA.PACKAGE> Packages { get; set; }
    }
}
