using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ataxx
{
    internal class Stapel<T>
    {
        private StapelItem bovenste { get; set; }

        public Stapel() { }

        public void duw(T inhoud)
        {
            StapelItem nieuw = new StapelItem(inhoud, bovenste);
            bovenste = nieuw;
        }

        public T pak()
        {
            if (bovenste != null)
            {
                T inhoud = bovenste.inhoud;
                bovenste = bovenste.vorige;
                return inhoud;
            }
            return default(T);
        }

        private class StapelItem
        {
            public T inhoud;
            public StapelItem vorige;

            public StapelItem(T inhoud, StapelItem vorige)
            {
                this.inhoud = inhoud;
                this.vorige = vorige;
            }
        }

    }
}
