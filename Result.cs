using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeOptimization
{
    public class Result
    {
        public string Algorytm { get; set; }
        public string FunkcjaTestowa { get; set; }
        public int LiczbaSzukanychParametrów { get; set; }
        public int LiczbaIteracji { get; set; }
        public int RozmiarPopulacji { get; set; }
        public string ZnalezioneMinimum { get; set; }
        public string OdchylenieStandardowePoszukiwanychParametrów { get; set; }
        public string WskaźnikZmiennościPoszukiwanychParametrów { get; set; }
        public string WartośćFunkcjiCelu { get; set; }
        public string OdchylenieStandardoweWartościFunkcjiCelu { get; set; }
        public string WskaźnikZmiennościWartościFunkcjiCelu { get; set; }
    }
}
